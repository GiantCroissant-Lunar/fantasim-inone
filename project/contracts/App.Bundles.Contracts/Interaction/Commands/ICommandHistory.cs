namespace FantaSim.App.Bundles.Contracts.Interaction.Commands;

/// <summary>
/// Linear undo/redo stack. Callers push reversible commands with their undo/redo actions.
/// </summary>
public interface ICommandHistory
{
    bool CanUndo { get; }
    bool CanRedo { get; }
    string? UndoDescription { get; }
    string? RedoDescription { get; }
    void Push(IReversibleCommand command, Action undo, Action redo);
    void Undo();
    void Redo();
    void Clear();
}
