namespace FantaSim.App.Bundles.Contracts;

/// <summary>
/// Describes a bundle's metadata, loaded from manifest.json inside the PCK.
/// </summary>
public sealed record BundleManifest(
    string Id,
    string Version,
    string DisplayName,
    string? EntryAssembly,
    string? RootScene,
    IReadOnlyList<string> Dependencies,
    string? DockTarget = null,
    string? Role = null
);
