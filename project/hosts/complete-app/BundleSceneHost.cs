using FantaSim.App.Bundles.Contracts;
using Godot;
using Microsoft.Extensions.Logging;

namespace FantaSim.App.Godot;

/// <summary>
/// Godot implementation of IBundleSceneHost.
/// Instantiates root scenes when bundles load, removes nodes when bundles unload.
/// If a bundle declares a DockTarget, the scene is docked via DockManager instead.
/// </summary>
public sealed class BundleSceneHost : IBundleSceneHost
{
    private readonly Node _container;
    private readonly ILogger _log;
    private readonly DockManager? _dockManager;
    private readonly Node? _shellTarget;
    private readonly Dictionary<string, List<Node>> _tracked = new();

    public BundleSceneHost(Node container, ILogger log, DockManager? dockManager = null, Node? shellTarget = null)
    {
        _container = container;
        _log = log;
        _dockManager = dockManager;
        _shellTarget = shellTarget;
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
            _log.LogError("Failed to load root scene: {RootScene}", manifest.RootScene);
            return;
        }

        var instance = scene.Instantiate();

        // Route: shell → shellTarget, dock → dockManager, fallback → container
        if (manifest.Role == "shell" && _shellTarget is not null && instance is Control shellControl)
        {
            shellControl.SetAnchorsAndOffsetsPreset(Control.LayoutPreset.FullRect);
            _shellTarget.AddChild(shellControl);
        }
        else if (manifest.DockTarget is not null && _dockManager is not null && instance is Control dockControl)
        {
            _dockManager.DockPanel(bundleId, manifest.DockTarget, dockControl, manifest.DisplayName);
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
        _log.LogInformation("Bundle '{BundleId}' root scene instantiated", bundleId);
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

        _log.LogInformation("Bundle '{BundleId}' nodes removed ({Count})", bundleId, nodes.Count);
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
