namespace FantaSim.App.Bundles.Contracts.Interaction.Events;

/// <summary>
/// Published before a bundle is deregistered during unload.
/// Listeners can still resolve services at this point.
/// </summary>
public sealed record BundleUnloadedEvent(string BundleId) : IHudEvent;
