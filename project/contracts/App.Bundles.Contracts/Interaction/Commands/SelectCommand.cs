using FantaSim.App.Bundles.Contracts.Interaction.Selection;

namespace FantaSim.App.Bundles.Contracts.Interaction.Commands;

/// <summary>
/// Command to select an entity. Not reversible — selection is navigation, not mutation.
/// </summary>
public sealed record SelectCommand(
    Guid CommandId,
    SelectableRef Target) : IHudCommand;
