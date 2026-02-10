using System.Reflection;
using System.Runtime.Loader;

namespace FantaSim.App.Bundles.Core;

/// <summary>
/// Collectible AssemblyLoadContext for plugin bundles.
/// Host-first resolution: contract assemblies resolve from the Default ALC
/// so types match between host and plugins.
/// </summary>
public sealed class PluginLoadContext : AssemblyLoadContext
{
    private readonly AssemblyDependencyResolver _resolver;
    private readonly HashSet<string> _hostAssemblyNames;

    public PluginLoadContext(string entryDllPath, IEnumerable<string> hostAssemblyNames)
        : base(isCollectible: true)
    {
        _resolver = new AssemblyDependencyResolver(entryDllPath);
        _hostAssemblyNames = new HashSet<string>(hostAssemblyNames, StringComparer.OrdinalIgnoreCase);
    }

    protected override Assembly? Load(AssemblyName assemblyName)
    {
        // Host-first: if the assembly is a host/contract assembly, delegate to Default ALC
        if (assemblyName.Name is not null && _hostAssemblyNames.Contains(assemblyName.Name))
        {
            return null; // Fall back to Default ALC
        }

        var path = _resolver.ResolveAssemblyToPath(assemblyName);
        return path is not null ? LoadFromAssemblyPath(path) : null;
    }

    protected override nint LoadUnmanagedDll(string unmanagedDllName)
    {
        var path = _resolver.ResolveUnmanagedDllToPath(unmanagedDllName);
        return path is not null ? LoadUnmanagedDllFromPath(path) : nint.Zero;
    }
}
