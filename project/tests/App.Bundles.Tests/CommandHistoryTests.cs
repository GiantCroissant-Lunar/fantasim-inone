using FantaSim.App.Bundles.Contracts.Interaction.Commands;
using FantaSim.App.Bundles.Contracts.Interaction.Events;
using FantaSim.App.Bundles.Contracts.Interaction.Selection;
using FantaSim.App.Bundles.Core;
using FluentAssertions;
using Xunit;

namespace FantaSim.App.Bundles.Tests;

public class CommandHistoryTests
{
    private readonly MessagePipeBundleMessageBus _bus = new();
    private readonly CommandHistory _sut;

    public CommandHistoryTests()
    {
        _sut = new CommandHistory(_bus);
    }

    private static ChangePropertyCommand MakeCommand(string description = "test") =>
        new(Guid.NewGuid(), new SelectableRef("plate", "p1"), "prop", "old", "new", description);

    [Fact]
    public void Initially_cannot_undo_or_redo()
    {
        _sut.CanUndo.Should().BeFalse();
        _sut.CanRedo.Should().BeFalse();
        _sut.UndoDescription.Should().BeNull();
        _sut.RedoDescription.Should().BeNull();
    }

    [Fact]
    public void Push_enables_undo()
    {
        _sut.Push(MakeCommand("Set color"), () => { }, () => { });

        _sut.CanUndo.Should().BeTrue();
        _sut.CanRedo.Should().BeFalse();
        _sut.UndoDescription.Should().Be("Set color");
    }

    [Fact]
    public void Undo_calls_undo_action()
    {
        var undoCalled = false;
        _sut.Push(MakeCommand(), () => undoCalled = true, () => { });

        _sut.Undo();

        undoCalled.Should().BeTrue();
    }

    [Fact]
    public void Undo_enables_redo()
    {
        var cmd = MakeCommand("Set color");
        _sut.Push(cmd, () => { }, () => { });

        _sut.Undo();

        _sut.CanRedo.Should().BeTrue();
        _sut.CanUndo.Should().BeFalse();
        _sut.RedoDescription.Should().Be("Set color");
    }

    [Fact]
    public void Redo_calls_redo_action()
    {
        var redoCalled = false;
        _sut.Push(MakeCommand(), () => { }, () => redoCalled = true);
        _sut.Undo();

        _sut.Redo();

        redoCalled.Should().BeTrue();
    }

    [Fact]
    public void Push_clears_redo_stack()
    {
        _sut.Push(MakeCommand(), () => { }, () => { });
        _sut.Undo();
        _sut.CanRedo.Should().BeTrue();

        _sut.Push(MakeCommand(), () => { }, () => { });

        _sut.CanRedo.Should().BeFalse();
    }

    [Fact]
    public void Undo_publishes_CommandUndoneEvent_with_IsRedo_false()
    {
        var cmd = MakeCommand();
        _sut.Push(cmd, () => { }, () => { });

        CommandUndoneEvent? received = null;
        using var sub = _bus.Subscribe<CommandUndoneEvent>(e => received = e);
        _sut.Undo();

        received.Should().NotBeNull();
        received!.CommandId.Should().Be(cmd.CommandId);
        received.IsRedo.Should().BeFalse();
    }

    [Fact]
    public void Redo_publishes_CommandUndoneEvent_with_IsRedo_true()
    {
        var cmd = MakeCommand();
        _sut.Push(cmd, () => { }, () => { });
        _sut.Undo();

        CommandUndoneEvent? received = null;
        using var sub = _bus.Subscribe<CommandUndoneEvent>(e => received = e);
        _sut.Redo();

        received.Should().NotBeNull();
        received!.CommandId.Should().Be(cmd.CommandId);
        received.IsRedo.Should().BeTrue();
    }

    [Fact]
    public void Clear_empties_both_stacks()
    {
        _sut.Push(MakeCommand(), () => { }, () => { });
        _sut.Push(MakeCommand(), () => { }, () => { });
        _sut.Undo();

        _sut.Clear();

        _sut.CanUndo.Should().BeFalse();
        _sut.CanRedo.Should().BeFalse();
    }

    [Fact]
    public void Undo_when_empty_is_noop()
    {
        var eventCount = 0;
        using var sub = _bus.Subscribe<CommandUndoneEvent>(_ => eventCount++);

        _sut.Undo();

        eventCount.Should().Be(0);
    }

    [Fact]
    public void Redo_when_empty_is_noop()
    {
        var eventCount = 0;
        using var sub = _bus.Subscribe<CommandUndoneEvent>(_ => eventCount++);

        _sut.Redo();

        eventCount.Should().Be(0);
    }

    [Fact]
    public void Max_depth_evicts_oldest_entries()
    {
        var history = new CommandHistory(_bus, maxDepth: 3);
        for (var i = 0; i < 5; i++)
            history.Push(MakeCommand($"cmd-{i}"), () => { }, () => { });

        history.CanUndo.Should().BeTrue();
        // Only 3 entries should remain
        history.Undo();
        history.Undo();
        history.Undo();
        history.CanUndo.Should().BeFalse();
    }

    [Fact]
    public void Full_undo_redo_cycle()
    {
        var value = 0;
        _sut.Push(MakeCommand(), () => value = 0, () => value = 1);
        value = 1;

        _sut.Undo();
        value.Should().Be(0);

        _sut.Redo();
        value.Should().Be(1);
    }
}
