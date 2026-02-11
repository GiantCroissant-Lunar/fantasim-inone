namespace FantaSim.App.Bundles.Contracts.Interaction.Events;

/// <summary>
/// Published after an undo or redo operation completes.
/// </summary>
public sealed record CommandUndoneEvent(
    Guid CommandId,
    bool IsRedo) : IHudEvent;
