using FantaSim.App.Bundles.Contracts.Interaction.Selection;

namespace FantaSim.App.Bundles.Contracts.Interaction.Commands;

/// <summary>
/// Command to change a property on a selectable entity. Reversible for undo/redo.
/// </summary>
public sealed record ChangePropertyCommand(
    Guid CommandId,
    SelectableRef Target,
    string PropertyPath,
    object? OldValue,
    object? NewValue,
    string Description) : IReversibleCommand;
