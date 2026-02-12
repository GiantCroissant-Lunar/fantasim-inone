namespace FantaSim.App.Bundles.Contracts.Interaction.Events;

/// <summary>
/// Published when a bundle emits HUD data for a channel.
/// Payload is JSON to keep the event transport contract-only.
/// </summary>
public sealed record HudDataChangedEvent(
    string Channel,
    string PayloadJson) : IHudEvent;
