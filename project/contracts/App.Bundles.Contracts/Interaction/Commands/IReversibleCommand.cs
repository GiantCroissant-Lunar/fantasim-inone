namespace FantaSim.App.Bundles.Contracts.Interaction.Commands;

/// <summary>
/// A command that can be undone/redone. Carries a human-readable description for undo menus.
/// </summary>
public interface IReversibleCommand : IHudCommand
{
    string Description { get; }
}
