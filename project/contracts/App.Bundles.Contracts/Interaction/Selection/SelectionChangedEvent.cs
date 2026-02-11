namespace FantaSim.App.Bundles.Contracts.Interaction.Selection;

/// <summary>
/// Published when the current selection changes.
/// </summary>
public sealed record SelectionChangedEvent(
    SelectableRef? Previous,
    SelectableRef? Current) : IHudEvent;
