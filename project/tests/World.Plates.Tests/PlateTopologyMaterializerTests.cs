using System.Collections.Immutable;
using Fantasim.World.Plates.Contracts.Entities;
using Fantasim.World.Plates.Contracts.Events;
using Fantasim.World.Plates.Contracts.Identity;
using Fantasim.World.Plates.Topology;
using FluentAssertions;
using Xunit;

namespace Fantasim.World.Plates.Tests;

public class PlateTopologyMaterializerTests
{
    [Fact]
    public void Materialize_creates_plates_boundaries_junctions()
    {
        var store = new InMemoryPlateTopologyEventStore();
        var p1 = PlateId.New();
        var p2 = PlateId.New();
        var p3 = PlateId.New();
        var b1 = BoundaryId.New();
        var b2 = BoundaryId.New();
        var b3 = BoundaryId.New();
        var b4 = BoundaryId.New();
        var j1 = JunctionId.New();
        var j2 = JunctionId.New();

        store.Append(new PlateCreatedEvent(Guid.NewGuid(), p1, 1, 0));
        store.Append(new PlateCreatedEvent(Guid.NewGuid(), p2, 2, 0));
        store.Append(new PlateCreatedEvent(Guid.NewGuid(), p3, 3, 0));
        store.Append(new BoundaryCreatedEvent(Guid.NewGuid(), b1, p1, p2, BoundaryType.Divergent, 4, 0));
        store.Append(new BoundaryCreatedEvent(Guid.NewGuid(), b2, p2, p3, BoundaryType.Convergent, 5, 0));
        store.Append(new BoundaryCreatedEvent(Guid.NewGuid(), b3, p3, p1, BoundaryType.Transform, 6, 0));
        store.Append(new BoundaryCreatedEvent(Guid.NewGuid(), b4, p1, p3, BoundaryType.Divergent, 7, 1));
        store.Append(new JunctionCreatedEvent(Guid.NewGuid(), j1, [b1, b2, b3], 8, 1));
        store.Append(new JunctionCreatedEvent(Guid.NewGuid(), j2, [b2, b3, b4], 9, 1));

        var materializer = new PlateTopologyMaterializer(store);
        var state = materializer.Materialize();

        state.Plates.Should().HaveCount(3);
        state.Boundaries.Should().HaveCount(4);
        state.Junctions.Should().HaveCount(2);
        state.LastEventSequence.Should().Be(9);

        state.Boundaries[b1].BoundaryType.Should().Be(BoundaryType.Divergent);
        state.Boundaries[b2].PlateIdLeft.Should().Be(p2);
        state.Junctions[j1].BoundaryIds.Should().HaveCount(3);
    }

    [Fact]
    public void MaterializeAtSequence_replays_partial()
    {
        var store = new InMemoryPlateTopologyEventStore();
        var p1 = PlateId.New();
        var p2 = PlateId.New();

        store.Append(new PlateCreatedEvent(Guid.NewGuid(), p1, 1, 0));
        store.Append(new PlateCreatedEvent(Guid.NewGuid(), p2, 2, 0));
        store.Append(new BoundaryCreatedEvent(Guid.NewGuid(), BoundaryId.New(), p1, p2, BoundaryType.Transform, 3, 1));

        var materializer = new PlateTopologyMaterializer(store);
        var state = materializer.MaterializeAtSequence(2);

        state.Plates.Should().HaveCount(2);
        state.Boundaries.Should().BeEmpty();
        state.LastEventSequence.Should().Be(2);
    }

    [Fact]
    public void MaterializeAtTick_filters_by_tick()
    {
        var store = new InMemoryPlateTopologyEventStore();
        var p1 = PlateId.New();
        var p2 = PlateId.New();

        store.Append(new PlateCreatedEvent(Guid.NewGuid(), p1, 1, 0));
        store.Append(new PlateCreatedEvent(Guid.NewGuid(), p2, 2, 10));

        var materializer = new PlateTopologyMaterializer(store);
        var atTick0 = materializer.MaterializeAtTick(0);
        var atTick10 = materializer.MaterializeAtTick(10);

        atTick0.Plates.Should().HaveCount(1);
        atTick10.Plates.Should().HaveCount(2);
    }

    [Fact]
    public void BoundaryTypeChanged_updates_type()
    {
        var store = new InMemoryPlateTopologyEventStore();
        var p1 = PlateId.New();
        var p2 = PlateId.New();
        var b1 = BoundaryId.New();

        store.Append(new PlateCreatedEvent(Guid.NewGuid(), p1, 1, 0));
        store.Append(new PlateCreatedEvent(Guid.NewGuid(), p2, 2, 0));
        store.Append(new BoundaryCreatedEvent(Guid.NewGuid(), b1, p1, p2, BoundaryType.Divergent, 3, 0));
        store.Append(new BoundaryTypeChangedEvent(Guid.NewGuid(), b1, BoundaryType.Convergent, 4, 1));

        var state = new PlateTopologyMaterializer(store).Materialize();
        state.Boundaries[b1].BoundaryType.Should().Be(BoundaryType.Convergent);
    }

    [Fact]
    public void PlateRetired_marks_as_retired()
    {
        var store = new InMemoryPlateTopologyEventStore();
        var p1 = PlateId.New();

        store.Append(new PlateCreatedEvent(Guid.NewGuid(), p1, 1, 0));
        store.Append(new PlateRetiredEvent(Guid.NewGuid(), p1, 2, 1));

        var state = new PlateTopologyMaterializer(store).Materialize();
        state.Plates[p1].IsRetired.Should().BeTrue();
    }
}
