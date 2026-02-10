using FantaSim.World.Plates.Contracts.Events;
using FantaSim.World.Plates.Contracts.Identity;
using FantaSim.World.Plates.Topology;
using FluentAssertions;
using Xunit;

namespace FantaSim.World.Plates.Tests;

public class InMemoryEventStoreTests
{
    [Fact]
    public void Append_and_ReadAll_returns_events_in_order()
    {
        var store = new InMemoryPlateTopologyEventStore();
        var p1 = PlateId.New();
        var p2 = PlateId.New();

        store.Append(new PlateCreatedEvent(Guid.NewGuid(), p1, 1, 0));
        store.Append(new PlateCreatedEvent(Guid.NewGuid(), p2, 2, 0));

        var events = store.ReadAll();
        events.Should().HaveCount(2);
        events[0].Sequence.Should().Be(1);
        events[1].Sequence.Should().Be(2);
    }

    [Fact]
    public void Append_non_monotonic_sequence_throws()
    {
        var store = new InMemoryPlateTopologyEventStore();
        store.Append(new PlateCreatedEvent(Guid.NewGuid(), PlateId.New(), 5, 0));

        var act = () => store.Append(new PlateCreatedEvent(Guid.NewGuid(), PlateId.New(), 3, 0));
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*not greater than*");
    }

    [Fact]
    public void Append_duplicate_sequence_throws()
    {
        var store = new InMemoryPlateTopologyEventStore();
        store.Append(new PlateCreatedEvent(Guid.NewGuid(), PlateId.New(), 1, 0));

        var act = () => store.Append(new PlateCreatedEvent(Guid.NewGuid(), PlateId.New(), 1, 0));
        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void ReadUpToSequence_filters_correctly()
    {
        var store = new InMemoryPlateTopologyEventStore();
        store.Append(new PlateCreatedEvent(Guid.NewGuid(), PlateId.New(), 1, 0));
        store.Append(new PlateCreatedEvent(Guid.NewGuid(), PlateId.New(), 2, 0));
        store.Append(new PlateCreatedEvent(Guid.NewGuid(), PlateId.New(), 3, 0));

        store.ReadUpToSequence(2).Should().HaveCount(2);
    }

    [Fact]
    public void ReadUpToTick_filters_correctly()
    {
        var store = new InMemoryPlateTopologyEventStore();
        store.Append(new PlateCreatedEvent(Guid.NewGuid(), PlateId.New(), 1, 0));
        store.Append(new PlateCreatedEvent(Guid.NewGuid(), PlateId.New(), 2, 5));
        store.Append(new PlateCreatedEvent(Guid.NewGuid(), PlateId.New(), 3, 10));

        store.ReadUpToTick(5).Should().HaveCount(2);
    }
}
