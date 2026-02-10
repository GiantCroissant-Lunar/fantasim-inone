using System.Reflection;
using System.Runtime.Loader;
using System.Text.Json;
using Fantasim.App.Bundles.Contracts;

namespace Fantasim.App.Bundles.Core;

/// <summary>
/// THE single lifecycle manager for bundles.
/// Load → Mount PCK → Parse Manifest → Extract DLLs → Load ALC → Register Services
/// Unload → Deregister Services → Unload ALC → GC → Cleanup
/// </summary>
public sealed class BundleHost : IBundleHost
{
    private readonly IGodotBundleVfs _vfs;
    private readonly IDllExtractor _extractor;
    private readonly IBundleServiceRegistry _registry;
    private readonly IBundleSceneHost? _sceneHost;
    private readonly IBundleMessageBus? _messageBus;
    private readonly Dictionary<string, LoadedBundle> _bundles = new();
    private readonly HashSet<string> _hostAssemblyNames;

    public IReadOnlyDictionary<string, BundleInfo> LoadedBundles =>
        _bundles.ToDictionary(kv => kv.Key, kv => kv.Value.Info);

    public BundleHost(
        IGodotBundleVfs vfs,
        IDllExtractor extractor,
        IBundleServiceRegistry registry,
        IBundleSceneHost? sceneHost = null,
        IBundleMessageBus? messageBus = null)
    {
        _vfs = vfs;
        _extractor = extractor;
        _registry = registry;
        _sceneHost = sceneHost;
        _messageBus = messageBus;

        // Host assemblies whose types must match between host and plugins
        _hostAssemblyNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "Fantasim.App.Bundles.Contracts",
            "Fantasim.World.Plates.Contracts",
            "System.Runtime",
            "System.Private.CoreLib",
            "netstandard"
        };
    }

    public async Task<BundleInfo> LoadAsync(string pckPath, CancellationToken cancellationToken = default)
    {
        // 1. Mount PCK
        if (!_vfs.MountPck(pckPath))
        {
            throw new InvalidOperationException($"Failed to mount PCK: {pckPath}");
        }

        // 2. Parse manifest
        var manifest = ParseManifest(pckPath);
        if (_bundles.ContainsKey(manifest.Id))
        {
            throw new InvalidOperationException($"Bundle '{manifest.Id}' is already loaded.");
        }

        PluginLoadContext? alc = null;
        Assembly? assembly = null;

        // 3. Extract DLLs + load ALC only if bundle has an entry assembly
        if (manifest.EntryAssembly is not null)
        {
            var dllPaths = DiscoverDlls(manifest);
            var extractDir = _extractor.ExtractDlls(manifest.Id, dllPaths, _vfs);

            var entryDllPath = Path.Combine(extractDir, manifest.EntryAssembly);
            alc = new PluginLoadContext(entryDllPath, _hostAssemblyNames);
            assembly = alc.LoadFromAssemblyPath(entryDllPath);
        }

        // 4. Register in bundles dictionary
        var info = new BundleInfo(manifest.Id, manifest, BundleStatus.Loaded, DateTimeOffset.UtcNow);
        var loaded = new LoadedBundle(info, alc, assembly, pckPath);
        _bundles[manifest.Id] = loaded;

        // 5. Invoke service modules (only if assembly exists)
        if (assembly is not null)
        {
            InvokeServiceModules(assembly, register: true);
        }

        // 6. Notify scene host
        _sceneHost?.OnBundleLoaded(manifest.Id, manifest);

        await Task.CompletedTask;
        return info;
    }

    public async Task UnloadAsync(string bundleId, CancellationToken cancellationToken = default)
    {
        if (!_bundles.Remove(bundleId, out var loaded))
        {
            throw new InvalidOperationException($"Bundle '{bundleId}' is not loaded.");
        }

        // 1. Notify scene host before teardown
        _sceneHost?.OnBundleUnloading(bundleId);

        // 2. Deregister service modules + unload ALC (only if assembly exists)
        if (loaded.Assembly is not null)
        {
            InvokeServiceModules(loaded.Assembly, register: false);
            loaded.LoadContext!.Unload();

            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            _extractor.Cleanup(bundleId);
        }

        await Task.CompletedTask;
    }

    public async Task<BundleInfo> ReloadAsync(string bundleId, CancellationToken cancellationToken = default)
    {
        if (!_bundles.TryGetValue(bundleId, out var loaded))
        {
            throw new InvalidOperationException($"Bundle '{bundleId}' is not loaded.");
        }

        var pckPath = loaded.PckPath;

        // 1. Capture state if reload-aware (only if assembly exists)
        object? state = null;
        if (loaded.Assembly is not null)
        {
            var reloadAware = FindReloadAware(loaded.Assembly);
            if (reloadAware is not null)
            {
                state = reloadAware.CaptureState();
            }
        }

        // 2. Unload
        await UnloadAsync(bundleId, cancellationToken);

        // 3. Wait for OS file handle release (only needed for DLL bundles)
        if (loaded.Assembly is not null)
        {
            await Task.Delay(100, cancellationToken);
        }

        // 4. Reload
        var info = await LoadAsync(pckPath, cancellationToken);

        // 5. Restore state if reload-aware
        if (state is not null && _bundles.TryGetValue(bundleId, out var reloaded) && reloaded.Assembly is not null)
        {
            var newReloadAware = FindReloadAware(reloaded.Assembly);
            newReloadAware?.RestoreState(state);
        }

        return info;
    }

    public async Task UnloadAllAsync(CancellationToken cancellationToken = default)
    {
        var ids = _bundles.Keys.ToList();
        foreach (var id in ids)
        {
            await UnloadAsync(id, cancellationToken);
        }
    }

    public BundleSystemSnapshot CaptureSnapshot()
    {
        var bundles = _bundles.Values.Select(b => new BundleSnapshot(
            b.Info.Id,
            b.Info.Manifest,
            b.Info.Status,
            b.Info.LoadedAt,
            b.LoadContext is not null,
            _sceneHost?.GetTrackedNodes(b.Info.Id) ?? []
        )).ToList();

        var messageBusSnapshot = _messageBus is not null
            ? new MessageBusSnapshot(_messageBus.ActiveChannelCount)
            : null;

        return new BundleSystemSnapshot(
            DateTimeOffset.UtcNow,
            bundles,
            _registry.RegisteredTypeNames,
            messageBusSnapshot
        );
    }

    private BundleManifest ParseManifest(string pckPath)
    {
        // Convention: manifest.json is at res://bundles/{bundleDirName}/manifest.json
        // For now, derive the bundle dir from the PCK filename
        var bundleDirName = Path.GetFileNameWithoutExtension(pckPath);
        var manifestPath = $"res://bundles/{bundleDirName}/manifest.json";

        if (!_vfs.FileExists(manifestPath))
        {
            throw new InvalidOperationException($"Manifest not found at {manifestPath}");
        }

        var bytes = _vfs.ReadFile(manifestPath);
        var manifest = JsonSerializer.Deserialize<BundleManifest>(bytes, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        return manifest ?? throw new InvalidOperationException("Failed to deserialize manifest.");
    }

    private IReadOnlyList<string> DiscoverDlls(BundleManifest manifest)
    {
        if (manifest.EntryAssembly is null)
        {
            return [];
        }

        // Convention: DLLs are at res://bundles/{bundleId}/bin/{dllName}
        var basePath = $"res://bundles/{manifest.Id}/bin/";
        var entryPath = basePath + manifest.EntryAssembly;

        if (!_vfs.FileExists(entryPath))
        {
            throw new InvalidOperationException($"Entry assembly not found at {entryPath}");
        }

        return [entryPath];
    }

    private void InvokeServiceModules(Assembly assembly, bool register)
    {
        var moduleTypes = assembly.GetTypes()
            .Where(t => !t.IsAbstract && typeof(IBundleServiceModule).IsAssignableFrom(t));

        foreach (var type in moduleTypes)
        {
            if (Activator.CreateInstance(type) is IBundleServiceModule module)
            {
                if (register)
                    module.Register(_registry);
                else
                    module.Deregister(_registry);
            }
        }
    }

    private static IReloadAwareBundle? FindReloadAware(Assembly assembly)
    {
        var type = assembly.GetTypes()
            .FirstOrDefault(t => !t.IsAbstract && typeof(IReloadAwareBundle).IsAssignableFrom(t));

        return type is not null ? Activator.CreateInstance(type) as IReloadAwareBundle : null;
    }

    private sealed record LoadedBundle(
        BundleInfo Info,
        AssemblyLoadContext? LoadContext,
        Assembly? Assembly,
        string PckPath
    );
}
