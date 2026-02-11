using FantaSim.App.Bundles.Contracts.Interaction.Selection;

namespace FantaSim.App.Bundles.Contracts.Interaction.Events;

/// <summary>
/// Published after a property mutation is applied.
/// </summary>
public sealed record PropertyChangedEvent(
    SelectableRef Target,
    string PropertyPath,
    object? OldValue,
    object? NewValue) : IHudEvent;
