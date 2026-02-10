using Fantasim.App.Bundles.Contracts;
using Godot;

namespace Fantasim.App;

/// <summary>
/// Godot implementation of IBundleSceneHost.
/// Instantiates root scenes when bundles load, removes nodes when bundles unload.
/// If a bundle declares a DockTarget, the scene is docked via DockManager instead.
/// </summary>
public sealed class GodotBundleSceneHost : IBundleSceneHost
{
    private readonly Node _container;
    private readonly DockManager? _dockManager;
    private readonly Dictionary<string, List<Node>> _tracked = new();

    public GodotBundleSceneHost(Node container, DockManager? dockManager = null)
    {
        _container = container;
        _dockManager = dockManager;
    }

    public void OnBundleLoaded(string bundleId, BundleManifest manifest)
    {
        if (manifest.RootScene is null)
        {
            return;
        }

        var scene = ResourceLoader.Load<PackedScene>(
            manifest.RootScene,
            cacheMode: ResourceLoader.CacheMode.Replace);

        if (scene is null)
        {
            GD.PrintErr($"[SceneHost] Failed to load root scene: {manifest.RootScene}");
            return;
        }

        var instance = scene.Instantiate();

        // Route: if bundle declares a dock target, dock it; otherwise add as child
        if (manifest.DockTarget is not null && _dockManager is not null && instance is Control control)
        {
            _dockManager.DockPanel(bundleId, manifest.DockTarget, control, manifest.DisplayName);
        }
        else
        {
            _container.AddChild(instance);
        }

        if (!_tracked.TryGetValue(bundleId, out var nodes))
        {
            nodes = [];
            _tracked[bundleId] = nodes;
        }

        nodes.Add(instance);
        GD.Print($"[SceneHost] Bundle '{bundleId}' root scene instantiated");
    }

    public void OnBundleUnloading(string bundleId)
    {
        if (!_tracked.Remove(bundleId, out var nodes))
        {
            return;
        }

        // Undock from DockManager if applicable
        _dockManager?.UndockBundle(bundleId);

        foreach (var node in nodes)
        {
            if (node.IsInsideTree())
            {
                node.QueueFree();
            }
        }

        GD.Print($"[SceneHost] Bundle '{bundleId}' nodes removed ({nodes.Count})");
    }

    public IReadOnlyList<string> GetTrackedNodes(string bundleId)
    {
        if (!_tracked.TryGetValue(bundleId, out var nodes))
        {
            return [];
        }

        return nodes.Select(n => n.GetPath().ToString()).ToList();
    }
}
