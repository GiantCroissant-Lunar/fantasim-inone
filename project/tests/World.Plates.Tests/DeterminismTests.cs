using Fantasim.World.Plates.Contracts.Entities;
using Fantasim.World.Plates.Contracts.Events;
using Fantasim.World.Plates.Contracts.Identity;
using Fantasim.World.Plates.Topology;
using FluentAssertions;
using Xunit;

namespace Fantasim.World.Plates.Tests;

public class DeterminismTests
{
    [Fact]
    public void Same_events_produce_identical_state()
    {
        // Use fixed GUIDs for deterministic comparison
        var p1 = new PlateId(Guid.Parse("00000000-0000-0000-0000-000000000001"));
        var p2 = new PlateId(Guid.Parse("00000000-0000-0000-0000-000000000002"));
        var p3 = new PlateId(Guid.Parse("00000000-0000-0000-0000-000000000003"));
        var b1 = new BoundaryId(Guid.Parse("10000000-0000-0000-0000-000000000001"));
        var b2 = new BoundaryId(Guid.Parse("10000000-0000-0000-0000-000000000002"));

        var events = new IPlateTopologyEvent[]
        {
            new PlateCreatedEvent(Guid.Parse("e0000001-0000-0000-0000-000000000000"), p1, 1, 0),
            new PlateCreatedEvent(Guid.Parse("e0000002-0000-0000-0000-000000000000"), p2, 2, 0),
            new PlateCreatedEvent(Guid.Parse("e0000003-0000-0000-0000-000000000000"), p3, 3, 0),
            new BoundaryCreatedEvent(Guid.Parse("e0000004-0000-0000-0000-000000000000"), b1, p1, p2, BoundaryType.Divergent, 4, 1),
            new BoundaryCreatedEvent(Guid.Parse("e0000005-0000-0000-0000-000000000000"), b2, p2, p3, BoundaryType.Convergent, 5, 1),
            new BoundaryTypeChangedEvent(Guid.Parse("e0000006-0000-0000-0000-000000000000"), b1, BoundaryType.Transform, 6, 2),
        };

        // Materialize twice from identical events
        var state1 = MaterializeFrom(events);
        var state2 = MaterializeFrom(events);

        // Verify identical
        state1.Plates.Count.Should().Be(state2.Plates.Count);
        state1.Boundaries.Count.Should().Be(state2.Boundaries.Count);
        state1.LastEventSequence.Should().Be(state2.LastEventSequence);

        foreach (var (id, plate) in state1.Plates)
        {
            state2.Plates[id].Should().Be(plate);
        }

        foreach (var (id, boundary) in state1.Boundaries)
        {
            var other = state2.Boundaries[id];
            other.BoundaryId.Should().Be(boundary.BoundaryId);
            other.BoundaryType.Should().Be(boundary.BoundaryType);
            other.PlateIdLeft.Should().Be(boundary.PlateIdLeft);
            other.PlateIdRight.Should().Be(boundary.PlateIdRight);
        }
    }

    private static Contracts.IPlateTopologyStateView MaterializeFrom(IPlateTopologyEvent[] events)
    {
        var store = new InMemoryPlateTopologyEventStore();
        foreach (var evt in events)
        {
            store.Append(evt);
        }
        return new PlateTopologyMaterializer(store).Materialize();
    }
}
