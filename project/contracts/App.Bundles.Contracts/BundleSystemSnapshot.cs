namespace Fantasim.App.Bundles.Contracts;

/// <summary>
/// Top-level aggregate snapshot of the entire bundle system state.
/// All fields are JSON-serializable (no Type references).
/// </summary>
public sealed record BundleSystemSnapshot(
    DateTimeOffset CapturedAt,
    IReadOnlyList<BundleSnapshot> Bundles,
    IReadOnlyList<string> RegisteredServiceTypes,
    MessageBusSnapshot? MessageBus
);

/// <summary>
/// Per-bundle state snapshot.
/// </summary>
public sealed record BundleSnapshot(
    string Id,
    BundleManifest Manifest,
    BundleStatus Status,
    DateTimeOffset LoadedAt,
    bool HasAssemblyLoadContext,
    IReadOnlyList<string> TrackedSceneNodes
);

/// <summary>
/// Message bus health snapshot.
/// </summary>
public sealed record MessageBusSnapshot(
    int ActiveChannelCount
);
