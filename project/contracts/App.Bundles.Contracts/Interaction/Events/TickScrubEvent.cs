namespace FantaSim.App.Bundles.Contracts.Interaction.Events;

/// <summary>
/// Published when the timeline is scrubbed to a new tick. Placeholder for future timeline UI.
/// </summary>
public sealed record TickScrubEvent(
    long Tick,
    bool IsScrubbing) : IHudEvent;
