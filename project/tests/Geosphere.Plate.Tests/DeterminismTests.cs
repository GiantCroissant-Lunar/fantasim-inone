using Plate.TimeDete.Time.Primitives;
using FantaSim.Geosphere.Plate.Topology.Contracts.Entities;
using FantaSim.Geosphere.Plate.Topology.Contracts.Events;
using FantaSim.Geosphere.Plate.Topology.Contracts.Identity;
using FantaSim.Geosphere.Plate.Topology.Materializer;
using FantaSim.Geosphere.Plate.Testing.Storage;
using FluentAssertions;
using Xunit;
using UnifyGeometry;

namespace FantaSim.Geosphere.Plate.Tests;

public class DeterminismTests
{
    private static readonly TruthStreamIdentity TestStream = new("test-variant", "main", 0, Domain.GeoPlatesTopology, "M0");

    [Fact]
    public async Task Same_events_produce_identical_state()
    {
        // Use fixed GUIDs for deterministic comparison (or just deterministic PlateId generation if supported)
        var p1 = PlateId.NewId();
        var p2 = PlateId.NewId();
        var p3 = PlateId.NewId();
        var b1 = BoundaryId.NewId();
        var b2 = BoundaryId.NewId();

        var events = new List<IPlateTopologyEvent>
        {
            TestEventFactory.PlateCreated(Guid.NewGuid(), p1, new CanonicalTick(0), 0, TestStream),
            TestEventFactory.PlateCreated(Guid.NewGuid(), p2, new CanonicalTick(0), 1, TestStream),
            TestEventFactory.PlateCreated(Guid.NewGuid(), p3, new CanonicalTick(0), 2, TestStream),
            TestEventFactory.BoundaryCreated(Guid.NewGuid(), b1, p1, p2, BoundaryType.Divergent, new Point2(0, 0), new CanonicalTick(0), 3, TestStream),
            TestEventFactory.BoundaryCreated(Guid.NewGuid(), b2, p2, p3, BoundaryType.Convergent, new Point2(0, 0), new CanonicalTick(0), 4, TestStream),
            TestEventFactory.BoundaryTypeChanged(Guid.NewGuid(), b1, BoundaryType.Divergent, BoundaryType.Transform, new CanonicalTick(1), 5, TestStream),
        };

        // Materialize twice from identical events
        var state1 = await MaterializeFrom(events);
        var state2 = await MaterializeFrom(events);

        // Verify identical
        state1.Plates.Count.Should().Be(state2.Plates.Count);
        state1.Boundaries.Count.Should().Be(state2.Boundaries.Count);
        state1.LastEventSequence.Should().Be(state2.LastEventSequence);

        foreach (var id in state1.Plates.Keys)
        {
            state2.Plates.Should().ContainKey(id);
            state2.Plates[id].Should().BeEquivalentTo(state1.Plates[id]);
        }

        foreach (var id in state1.Boundaries.Keys)
        {
            state2.Boundaries.Should().ContainKey(id);
            state2.Boundaries[id].Should().BeEquivalentTo(state1.Boundaries[id]);
        }
    }

    private static async Task<PlateTopologyState> MaterializeFrom(List<IPlateTopologyEvent> events)
    {
        var storage = new InMemoryOrderedKeyValueStore();
        var store = new PlateTopologyEventStore(storage);
        await store.AppendAsync(TestStream, events, CancellationToken.None);
        return await new PlateTopologyMaterializer(store).MaterializeAtSequenceAsync(TestStream, long.MaxValue);
    }
}
