namespace Fantasim.App.Bundles.Contracts;

/// <summary>
/// Read-only snapshot of a loaded bundle's state.
/// </summary>
public sealed record BundleInfo(
    string Id,
    BundleManifest Manifest,
    BundleStatus Status,
    DateTimeOffset LoadedAt
);

public enum BundleStatus
{
    Loading,
    Loaded,
    Unloading,
    Unloaded
}
