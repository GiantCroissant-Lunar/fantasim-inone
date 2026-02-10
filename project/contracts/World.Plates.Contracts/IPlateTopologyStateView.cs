using FantaSim.World.Plates.Contracts.Entities;
using FantaSim.World.Plates.Contracts.Identity;

namespace FantaSim.World.Plates.Contracts;

/// <summary>
/// Read-only materialized view of plate topology.
/// </summary>
public interface IPlateTopologyStateView
{
    IReadOnlyDictionary<PlateId, Plate> Plates { get; }
    IReadOnlyDictionary<BoundaryId, Boundary> Boundaries { get; }
    IReadOnlyDictionary<JunctionId, Junction> Junctions { get; }
    long LastEventSequence { get; }
}
