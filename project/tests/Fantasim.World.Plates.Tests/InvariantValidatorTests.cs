using Fantasim.World.Plates.Contracts.Entities;
using Fantasim.World.Plates.Contracts.Events;
using Fantasim.World.Plates.Contracts.Identity;
using Fantasim.World.Plates.Topology;
using FluentAssertions;
using Xunit;

namespace Fantasim.World.Plates.Tests;

public class InvariantValidatorTests
{
    [Fact]
    public void Valid_topology_has_no_violations()
    {
        var store = new InMemoryPlateTopologyEventStore();
        var p1 = PlateId.New();
        var p2 = PlateId.New();
        var b1 = BoundaryId.New();

        store.Append(new PlateCreatedEvent(Guid.NewGuid(), p1, 1, 0));
        store.Append(new PlateCreatedEvent(Guid.NewGuid(), p2, 2, 0));
        store.Append(new BoundaryCreatedEvent(Guid.NewGuid(), b1, p1, p2, BoundaryType.Divergent, 3, 0));

        var state = new PlateTopologyMaterializer(store).Materialize();
        var violations = PlateTopologyInvariantValidator.Validate(state);

        violations.Should().BeEmpty();
    }

    [Fact]
    public void Boundary_with_same_left_right_plate_violates()
    {
        var store = new InMemoryPlateTopologyEventStore();
        var p1 = PlateId.New();
        var b1 = BoundaryId.New();

        store.Append(new PlateCreatedEvent(Guid.NewGuid(), p1, 1, 0));
        store.Append(new BoundaryCreatedEvent(Guid.NewGuid(), b1, p1, p1, BoundaryType.Divergent, 2, 0));

        var state = new PlateTopologyMaterializer(store).Materialize();
        var violations = PlateTopologyInvariantValidator.Validate(state);

        violations.Should().ContainSingle().Which.Should().Contain("identical left and right");
    }

    [Fact]
    public void Orphan_junction_violates()
    {
        var store = new InMemoryPlateTopologyEventStore();
        var j1 = JunctionId.New();
        var nonExistentBoundary = BoundaryId.New();

        store.Append(new JunctionCreatedEvent(Guid.NewGuid(), j1, [nonExistentBoundary], 1, 0));

        var state = new PlateTopologyMaterializer(store).Materialize();
        var violations = PlateTopologyInvariantValidator.Validate(state);

        violations.Should().ContainSingle().Which.Should().Contain("non-existent boundary");
    }
}
