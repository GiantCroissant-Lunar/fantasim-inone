namespace FantaSim.App.Bundles.Contracts;

/// <summary>
/// Manages Godot scene tree nodes for loaded bundles.
/// Called by BundleHost during load/unload lifecycle.
/// </summary>
public interface IBundleSceneHost
{
    /// <summary>
    /// Called after a bundle is loaded. Implementation instantiates root scene and tracks nodes.
    /// </summary>
    void OnBundleLoaded(string bundleId, BundleManifest manifest);

    /// <summary>
    /// Called before a bundle is unloaded. Implementation removes tracked nodes from scene tree.
    /// </summary>
    void OnBundleUnloading(string bundleId);

    /// <summary>
    /// Returns all tracked node paths for a bundle (for debugging/bookkeeping).
    /// </summary>
    IReadOnlyList<string> GetTrackedNodes(string bundleId);
}
