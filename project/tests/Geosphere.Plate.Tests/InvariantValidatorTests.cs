using System.Collections.Immutable;
using Plate.TimeDete.Time.Primitives;
using FantaSim.Geosphere.Plate.Topology.Contracts.Entities;
using FantaSim.Geosphere.Plate.Topology.Contracts.Events;
using FantaSim.Geosphere.Plate.Topology.Contracts.Identity;
using FantaSim.Geosphere.Plate.Topology.Contracts.Numerics;
using FantaSim.Geosphere.Plate.Topology.Materializer;
using FantaSim.Geosphere.Plate.Testing.Storage;
using FluentAssertions;
using Xunit;
using UnifyGeometry;

namespace FantaSim.Geosphere.Plate.Tests;

public class InvariantValidatorTests
{
    private static readonly TruthStreamIdentity TestStream = new("test-variant", "main", 0, Domain.GeoPlatesTopology, "M0");

    [Fact]
    public async Task Valid_topology_has_no_violations()
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
            TestEventFactory.BoundaryCreated(Guid.NewGuid(), b1, p1, p2, BoundaryType.Divergent, new Point2(0, 0), new CanonicalTick(0), 2, TestStream)
        };

        await store.AppendAsync(TestStream, events, CancellationToken.None);

        var state = await new PlateTopologyMaterializer(store).MaterializeAtSequenceAsync(TestStream, long.MaxValue);

        // Static state validation
        var act = () => InvariantValidator.Validate(state);
        act.Should().NotThrow();
    }

    [Fact]
    public async Task Boundary_with_same_left_right_plate_violates()
    {
        var storage = new InMemoryOrderedKeyValueStore();
        var store = new PlateTopologyEventStore(storage);
        var p1 = PlateId.NewId();
        var b1 = BoundaryId.NewId();

        var events = new List<IPlateTopologyEvent>
        {
            TestEventFactory.PlateCreated(Guid.NewGuid(), p1, new CanonicalTick(0), 0, TestStream),
            TestEventFactory.BoundaryCreated(Guid.NewGuid(), b1, p1, p1, BoundaryType.Divergent, new Point2(0, 0), new CanonicalTick(0), 1, TestStream)
        };

        await store.AppendAsync(TestStream, events, CancellationToken.None);

        var materializer = new PlateTopologyMaterializer(store);
        var act = async () => await materializer.MaterializeAtSequenceAsync(TestStream, long.MaxValue);

        // Validation happens during ApplyEvent -> ValidateEvent
        (await act.Should().ThrowAsync<InvalidOperationException>())
            .WithMessage("*identical left and right*");
    }

    [Fact]
    public async Task Orphan_junction_violates()
    {
        var storage = new InMemoryOrderedKeyValueStore();
        var store = new PlateTopologyEventStore(storage);
        var j1 = JunctionId.NewId();
        var nonExistentBoundary = BoundaryId.NewId();

        var events = new List<IPlateTopologyEvent>
        {
            TestEventFactory.JunctionCreated(Guid.NewGuid(), j1, ImmutableArray.Create(nonExistentBoundary), SurfacePoint.UnitSphere(UnitVector3d.UnitZ), new CanonicalTick(0), 0, TestStream)
        };

        await store.AppendAsync(TestStream, events, CancellationToken.None);

        var materializer = new PlateTopologyMaterializer(store);
        var act = async () => await materializer.MaterializeAtSequenceAsync(TestStream, long.MaxValue);

        (await act.Should().ThrowAsync<InvalidOperationException>())
            .WithMessage("*non-existent boundary*");
    }
}
