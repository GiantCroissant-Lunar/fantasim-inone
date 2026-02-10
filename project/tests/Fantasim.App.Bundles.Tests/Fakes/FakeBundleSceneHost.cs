using Fantasim.App.Bundles.Contracts;

namespace Fantasim.App.Bundles.Tests.Fakes;

/// <summary>
/// Records calls to OnBundleLoaded / OnBundleUnloading for test assertions.
/// </summary>
public sealed class FakeBundleSceneHost : IBundleSceneHost
{
    private readonly List<(string BundleId, BundleManifest Manifest)> _loaded = [];
    private readonly List<string> _unloading = [];

    public IReadOnlyList<(string BundleId, BundleManifest Manifest)> LoadedCalls => _loaded;
    public IReadOnlyList<string> UnloadingCalls => _unloading;

    public void OnBundleLoaded(string bundleId, BundleManifest manifest)
    {
        _loaded.Add((bundleId, manifest));
    }

    public void OnBundleUnloading(string bundleId)
    {
        _unloading.Add(bundleId);
    }

    public IReadOnlyList<string> GetTrackedNodes(string bundleId)
    {
        return [];
    }
}
