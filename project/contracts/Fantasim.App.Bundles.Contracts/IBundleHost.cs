namespace Fantasim.App.Bundles.Contracts;

/// <summary>
/// THE single lifecycle manager for bundles.
/// Handles Load → Register → Unload → Reload.
/// </summary>
public interface IBundleHost
{
    IReadOnlyDictionary<string, BundleInfo> LoadedBundles { get; }

    Task<BundleInfo> LoadAsync(string pckPath, CancellationToken cancellationToken = default);
    Task UnloadAsync(string bundleId, CancellationToken cancellationToken = default);
    Task<BundleInfo> ReloadAsync(string bundleId, CancellationToken cancellationToken = default);
    Task UnloadAllAsync(CancellationToken cancellationToken = default);
}
