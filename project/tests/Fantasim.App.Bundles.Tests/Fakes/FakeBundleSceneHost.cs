using Fantasim.App.Bundles.Contracts;

namespace Fantasim.App.Bundles.Tests.Fakes;

/// <summary>
/// Records calls to OnBundleLoaded / OnBundleUnloading for test assertions.
/// Tracks simulated scene nodes per bundle for snapshot testing.
/// </summary>
public sealed class FakeBundleSceneHost : IBundleSceneHost
{
    private readonly List<(string BundleId, BundleManifest Manifest)> _loaded = [];
    private readonly List<string> _unloading = [];
    private readonly Dictionary<string, List<string>> _trackedNodes = new();

    public IReadOnlyList<(string BundleId, BundleManifest Manifest)> LoadedCalls => _loaded;
    public IReadOnlyList<string> UnloadingCalls => _unloading;

    public void OnBundleLoaded(string bundleId, BundleManifest manifest)
    {
        _loaded.Add((bundleId, manifest));
        _trackedNodes[bundleId] = [$"/root/{bundleId}"];
    }

    public void OnBundleUnloading(string bundleId)
    {
        _unloading.Add(bundleId);
        _trackedNodes.Remove(bundleId);
    }

    public IReadOnlyList<string> GetTrackedNodes(string bundleId)
    {
        return _trackedNodes.TryGetValue(bundleId, out var nodes) ? nodes : [];
    }
}
