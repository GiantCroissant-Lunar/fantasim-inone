using FantaSim.App.Bundles.Contracts;
using FantaSim.App.Bundles.Contracts.Interaction.Commands;
using FantaSim.App.Bundles.Contracts.Interaction.Events;

namespace FantaSim.App.Bundles.Core;

/// <summary>
/// Linear undo/redo stack with a configurable max depth (default 100).
/// Publishes <see cref="CommandUndoneEvent"/> after each undo/redo operation.
/// </summary>
public sealed class CommandHistory : ICommandHistory
{
    private readonly IBundleMessageBus _bus;
    private readonly int _maxDepth;
    private readonly LinkedList<Entry> _undoStack = new();
    private readonly Stack<Entry> _redoStack = new();

    public CommandHistory(IBundleMessageBus bus, int maxDepth = 100)
    {
        _bus = bus;
        _maxDepth = maxDepth;
    }

    public bool CanUndo => _undoStack.Count > 0;
    public bool CanRedo => _redoStack.Count > 0;
    public string? UndoDescription => _undoStack.Last?.Value.Command.Description;
    public string? RedoDescription => _redoStack.TryPeek(out var entry) ? entry.Command.Description : null;

    public void Push(IReversibleCommand command, Action undo, Action redo)
    {
        ArgumentNullException.ThrowIfNull(command);
        ArgumentNullException.ThrowIfNull(undo);
        ArgumentNullException.ThrowIfNull(redo);

        _redoStack.Clear();
        _undoStack.AddLast(new Entry(command, undo, redo));

        while (_undoStack.Count > _maxDepth)
            _undoStack.RemoveFirst();
    }

    public void Undo()
    {
        if (_undoStack.Count == 0)
            return;

        var entry = _undoStack.Last!.Value;
        _undoStack.RemoveLast();
        entry.UndoAction();
        _redoStack.Push(entry);
        _bus.Publish(new CommandUndoneEvent(entry.Command.CommandId, IsRedo: false));
    }

    public void Redo()
    {
        if (_redoStack.Count == 0)
            return;

        var entry = _redoStack.Pop();
        entry.RedoAction();
        _undoStack.AddLast(entry);
        _bus.Publish(new CommandUndoneEvent(entry.Command.CommandId, IsRedo: true));
    }

    public void Clear()
    {
        _undoStack.Clear();
        _redoStack.Clear();
    }

    private sealed record Entry(IReversibleCommand Command, Action UndoAction, Action RedoAction);
}
