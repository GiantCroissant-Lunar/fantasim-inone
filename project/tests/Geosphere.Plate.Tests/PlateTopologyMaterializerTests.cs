using System.Collections.Immutable;
using FantaSim.Geosphere.Plate.Topology.Contracts.Entities;
using FantaSim.Geosphere.Plate.Topology.Contracts.Events;
using FantaSim.Geosphere.Plate.Topology.Contracts.Identity;
using FantaSim.Geosphere.Plate.Topology.Contracts.Numerics;
using FantaSim.Geosphere.Plate.Topology.Materializer;
using FantaSim.Geosphere.Plate.Testing.Storage;
using FluentAssertions;
using Xunit;
using Plate.TimeDete.Time.Primitives;
using UnifyGeometry;

namespace FantaSim.Geosphere.Plate.Tests;

public class PlateTopologyMaterializerTests
{
    private static readonly TruthStreamIdentity TestStream = new("test-variant", "main", 0, Domain.GeoPlatesTopology, "M0");

    [Fact]
    public async Task Materialize_creates_plates_boundaries_junctions()
    {
        var storage = new InMemoryOrderedKeyValueStore();
        var store = new PlateTopologyEventStore(storage);

        var p1 = PlateId.NewId();
        var p2 = PlateId.NewId();
        var p3 = PlateId.NewId();
        var b1 = BoundaryId.NewId();
        var b2 = BoundaryId.NewId();
        var b3 = BoundaryId.NewId();
        var b4 = BoundaryId.NewId();
        var j1 = JunctionId.NewId();
        var j2 = JunctionId.NewId();

        var events = new List<IPlateTopologyEvent>
        {
            TestEventFactory.PlateCreated(Guid.NewGuid(), p1, new CanonicalTick(0), 0, TestStream),
            TestEventFactory.PlateCreated(Guid.NewGuid(), p2, new CanonicalTick(0), 1, TestStream),
            TestEventFactory.PlateCreated(Guid.NewGuid(), p3, new CanonicalTick(0), 2, TestStream),
            TestEventFactory.BoundaryCreated(Guid.NewGuid(), b1, p1, p2, BoundaryType.Divergent, new Point2(0, 0), new CanonicalTick(0), 3, TestStream),
            TestEventFactory.BoundaryCreated(Guid.NewGuid(), b2, p2, p3, BoundaryType.Convergent, new Point2(0, 0), new CanonicalTick(0), 4, TestStream),
            TestEventFactory.BoundaryCreated(Guid.NewGuid(), b3, p3, p1, BoundaryType.Transform, new Point2(0, 0), new CanonicalTick(0), 5, TestStream),
            TestEventFactory.BoundaryCreated(Guid.NewGuid(), b4, p1, p3, BoundaryType.Divergent, new Point2(0, 0), new CanonicalTick(1), 6, TestStream),
            TestEventFactory.JunctionCreated(Guid.NewGuid(), j1, ImmutableArray.Create(b1, b2, b3), SurfacePoint.UnitSphere(UnitVector3d.UnitZ), new CanonicalTick(1), 7, TestStream),
            TestEventFactory.JunctionCreated(Guid.NewGuid(), j2, ImmutableArray.Create(b2, b3, b4), SurfacePoint.UnitSphere(UnitVector3d.UnitX), new CanonicalTick(1), 8, TestStream)
        };

        await store.AppendAsync(TestStream, events, CancellationToken.None);

        var materializer = new PlateTopologyMaterializer(store);
        var state = await materializer.MaterializeAtSequenceAsync(TestStream, long.MaxValue);

        state.Plates.Should().HaveCount(3);
        state.Boundaries.Should().HaveCount(4);
        state.Junctions.Should().HaveCount(2);
        state.LastEventSequence.Should().Be(8);

        state.Boundaries[b1].BoundaryType.Should().Be(BoundaryType.Divergent);
        state.Boundaries[b2].PlateIdLeft.Should().Be(p2);
        state.Junctions[j1].BoundaryIds.Should().HaveCount(3);
    }

    [Fact]
    public async Task MaterializeAtSequence_replays_partial()
    {
        var storage = new InMemoryOrderedKeyValueStore();
        var store = new PlateTopologyEventStore(storage);

        var p1 = PlateId.NewId();
        var p2 = PlateId.NewId();

        var events = new List<IPlateTopologyEvent>
        {
            TestEventFactory.PlateCreated(Guid.NewGuid(), p1, new CanonicalTick(0), 0, TestStream),
            TestEventFactory.PlateCreated(Guid.NewGuid(), p2, new CanonicalTick(0), 1, TestStream),
            TestEventFactory.BoundaryCreated(Guid.NewGuid(), BoundaryId.NewId(), p1, p2, BoundaryType.Transform, new Point2(0, 0), new CanonicalTick(1), 2, TestStream)
        };

        await store.AppendAsync(TestStream, events, CancellationToken.None);

        var materializer = new PlateTopologyMaterializer(store);
        var state = await materializer.MaterializeAtSequenceAsync(TestStream, 1);

        state.Plates.Should().HaveCount(2);
        state.Boundaries.Should().BeEmpty();
        state.LastEventSequence.Should().Be(1);
    }

    [Fact]
    public async Task MaterializeAtTick_filters_by_tick()
    {
        var storage = new InMemoryOrderedKeyValueStore();
        var store = new PlateTopologyEventStore(storage);

        var p1 = PlateId.NewId();
        var p2 = PlateId.NewId();

        var events = new List<IPlateTopologyEvent>
        {
            TestEventFactory.PlateCreated(Guid.NewGuid(), p1, new CanonicalTick(0), 0, TestStream),
            TestEventFactory.PlateCreated(Guid.NewGuid(), p2, new CanonicalTick(10), 1, TestStream)
        };

        await store.AppendAsync(TestStream, events, CancellationToken.None);

        var materializer = new PlateTopologyMaterializer(store);
        var atTick0 = await materializer.MaterializeAtTickAsync(TestStream, new CanonicalTick(0));
        var atTick10 = await materializer.MaterializeAtTickAsync(TestStream, new CanonicalTick(10));

        atTick0.Plates.Should().HaveCount(1);
        atTick10.Plates.Should().HaveCount(2);
    }

    [Fact]
    public async Task BoundaryTypeChanged_updates_type()
    {
        var storage = new InMemoryOrderedKeyValueStore();
        var store = new PlateTopologyEventStore(storage);

        var p1 = PlateId.NewId();
        var p2 = PlateId.NewId();
        var b1 = BoundaryId.NewId();

        var events = new List<IPlateTopologyEvent>
        {
            TestEventFactory.PlateCreated(Guid.NewGuid(), p1, new CanonicalTick(0), 0, TestStream),
            TestEventFactory.PlateCreated(Guid.NewGuid(), p2, new CanonicalTick(0), 1, TestStream),
            TestEventFactory.BoundaryCreated(Guid.NewGuid(), b1, p1, p2, BoundaryType.Divergent, new Point2(0, 0), new CanonicalTick(0), 2, TestStream),
            TestEventFactory.BoundaryTypeChanged(Guid.NewGuid(), b1, BoundaryType.Divergent, BoundaryType.Convergent, new CanonicalTick(1), 3, TestStream)
        };

        await store.AppendAsync(TestStream, events, CancellationToken.None);

        var state = await new PlateTopologyMaterializer(store).MaterializeAtSequenceAsync(TestStream, long.MaxValue);
        state.Boundaries[b1].BoundaryType.Should().Be(BoundaryType.Convergent);
    }

    [Fact]
    public async Task PlateRetired_marks_as_retired()
    {
        var storage = new InMemoryOrderedKeyValueStore();
        var store = new PlateTopologyEventStore(storage);

        var p1 = PlateId.NewId();

        var events = new List<IPlateTopologyEvent>
        {
            TestEventFactory.PlateCreated(Guid.NewGuid(), p1, new CanonicalTick(0), 0, TestStream),
            TestEventFactory.PlateRetired(Guid.NewGuid(), p1, "test", new CanonicalTick(1), 1, TestStream)
        };

        await store.AppendAsync(TestStream, events, CancellationToken.None);

        var state = await new PlateTopologyMaterializer(store).MaterializeAtSequenceAsync(TestStream, long.MaxValue);
        state.Plates[p1].IsRetired.Should().BeTrue();
    }
}
