using Fantasim.App.Bundles.Contracts;
using Godot;

namespace Fantasim.App;

/// <summary>
/// Godot implementation of IBundleSceneHost.
/// Instantiates root scenes when bundles load, removes nodes when bundles unload.
/// </summary>
public sealed class GodotBundleSceneHost : IBundleSceneHost
{
    private readonly Node _container;
    private readonly Dictionary<string, List<Node>> _tracked = new();

    public GodotBundleSceneHost(Node container)
    {
        _container = container;
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
        _container.AddChild(instance);

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

        foreach (var node in nodes)
        {
            node.QueueFree();
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
