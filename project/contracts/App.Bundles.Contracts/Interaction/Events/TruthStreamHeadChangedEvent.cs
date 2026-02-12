namespace FantaSim.App.Bundles.Contracts.Interaction.Events;

/// <summary>
/// Published when a truth stream head is updated after append.
/// </summary>
public sealed record TruthStreamHeadChangedEvent(
    string StreamIdentity,
    long Sequence,
    long LastTick) : IHudEvent;
