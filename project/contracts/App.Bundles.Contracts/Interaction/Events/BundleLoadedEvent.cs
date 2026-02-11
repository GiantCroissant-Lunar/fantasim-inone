namespace FantaSim.App.Bundles.Contracts.Interaction.Events;

/// <summary>
/// Published after a bundle finishes loading and is fully wired (scene host notified).
/// </summary>
public sealed record BundleLoadedEvent(string BundleId, BundleManifest Manifest) : IHudEvent;
