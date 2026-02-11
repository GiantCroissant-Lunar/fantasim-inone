using FantaSim.Geosphere.Plates.Contracts.Entities;
using FantaSim.Geosphere.Plates.Contracts.Identity;

namespace FantaSim.Geosphere.Plates.Contracts;

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
