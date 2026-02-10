using Fantasim.App.Bundles.Contracts;
using Fantasim.App.Bundles.Core;
using Godot;

namespace Fantasim.App;

/// <summary>
/// Godot autoload node. Creates BundleHost + services in _Ready(),
/// exposes methods callable from GDScript bundles.
/// </summary>
public partial class BootstrapShim : Node
{
    private BundleHost? _bundleHost;
    private MessagePipeBundleMessageBus? _messageBus;
    private DockManager? _dockManager;
    private MenuService? _menuService;
    private StatusService? _statusService;

    public IBundleHost? BundleHost => _bundleHost;
    public IBundleServiceRegistry? Registry { get; private set; }
    public DockManager? DockManager => _dockManager;
    public MenuService? MenuService => _menuService;
    public StatusService? StatusService => _statusService;

    public override void _Ready()
    {
        var vfs = new GodotBundleVfs();
        var extractor = new DllExtractor();
        var registry = new BundleServiceRegistry();
        _messageBus = new MessagePipeBundleMessageBus();

        // Create services
        _dockManager = new DockManager();
        _menuService = new MenuService();
        _statusService = new StatusService();

        var mainNode = GetTree().Root.GetNode("Main");
        var sceneHost = new GodotBundleSceneHost(this, _dockManager, shellTarget: mainNode);

        _bundleHost = new BundleHost(vfs, extractor, registry, sceneHost, _messageBus);
        Registry = registry;

        registry.Register<IBundleMessageBus>(_messageBus);

        GD.Print("[Bootstrap] BundleHost created (scene host + message bus wired)");
    }

    public override void _ExitTree()
    {
        if (_bundleHost is not null)
        {
            _bundleHost.UnloadAllAsync().GetAwaiter().GetResult();
            GD.Print("[Bootstrap] BundleHost shut down");
        }

        _messageBus?.Dispose();
    }

    // -- GDScript-callable methods --

    public void DockPanel(string bundleId, string slotName, Control panel, string tabTitle)
    {
        _dockManager?.DockPanel(bundleId, slotName, panel, tabTitle);
    }

    public PopupMenu? GetOrAddMenu(string menuTitle)
    {
        return _menuService?.GetOrAddMenu(menuTitle);
    }

    public void ShowStatus(string text)
    {
        _statusService?.ShowStatus(text);
    }

    public void ShowFileDialog(Callable onSelected)
    {
        _statusService?.ShowFileDialog(path => onSelected.Call(path));
    }

    public async void LoadBundle(string pckPath)
    {
        if (_bundleHost is null) return;
        try
        {
            _statusService?.ShowStatus($"Loading {pckPath}...");
            await _bundleHost.LoadAsync(pckPath);
            _statusService?.ShowStatus("Bundle loaded");
        }
        catch (System.Exception ex)
        {
            _statusService?.ShowStatus($"Error: {ex.Message}");
            GD.PrintErr($"[Bootstrap] Load failed: {ex}");
        }
    }

    public async void UnloadBundle(string bundleId)
    {
        if (_bundleHost is null) return;
        try
        {
            _statusService?.ShowStatus($"Unloading {bundleId}...");
            await _bundleHost.UnloadAsync(bundleId);
            _statusService?.ShowStatus("Bundle unloaded");
        }
        catch (System.Exception ex)
        {
            _statusService?.ShowStatus($"Error: {ex.Message}");
            GD.PrintErr($"[Bootstrap] Unload failed: {ex}");
        }
    }

    public async void ReloadBundle(string bundleId)
    {
        if (_bundleHost is null) return;
        try
        {
            _statusService?.ShowStatus($"Reloading {bundleId}...");
            await _bundleHost.ReloadAsync(bundleId);
            _statusService?.ShowStatus("Bundle reloaded");
        }
        catch (System.Exception ex)
        {
            _statusService?.ShowStatus($"Error: {ex.Message}");
            GD.PrintErr($"[Bootstrap] Reload failed: {ex}");
        }
    }

    public Godot.Collections.Dictionary CaptureSnapshotDict()
    {
        if (_bundleHost is null) return [];

        var snapshot = _bundleHost.CaptureSnapshot();
        var result = new Godot.Collections.Dictionary();

        var bundlesArray = new Godot.Collections.Array();
        foreach (var bundle in snapshot.Bundles)
        {
            var b = new Godot.Collections.Dictionary
            {
                ["id"] = bundle.Id,
                ["status"] = bundle.Status.ToString(),
                ["version"] = bundle.Manifest.Version,
                ["displayName"] = bundle.Manifest.DisplayName,
                ["loadedAt"] = bundle.LoadedAt.ToString("HH:mm:ss.fff"),
                ["hasALC"] = bundle.HasAssemblyLoadContext,
            };

            var nodesArray = new Godot.Collections.Array();
            foreach (var node in bundle.TrackedSceneNodes)
            {
                nodesArray.Add(node);
            }
            b["trackedNodes"] = nodesArray;

            bundlesArray.Add(b);
        }

        result["capturedAt"] = snapshot.CapturedAt.ToString("HH:mm:ss.fff");
        result["bundles"] = bundlesArray;

        var servicesArray = new Godot.Collections.Array();
        foreach (var t in snapshot.RegisteredServiceTypes)
        {
            servicesArray.Add(t);
        }
        result["services"] = servicesArray;
        result["messageBusChannels"] = snapshot.MessageBus?.ActiveChannelCount ?? 0;

        return result;
    }
}
