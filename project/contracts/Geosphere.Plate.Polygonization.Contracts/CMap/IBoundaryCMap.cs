using FantaSim.Geosphere.Plate.Topology.Contracts.Entities;

namespace FantaSim.Geosphere.Plate.Polygonization.Contracts.CMap;

/// <summary>
/// Minimal combinatorial map interface for boundary network polygonization.
/// RFC-V2-0041 §11.
/// </summary>
public interface IBoundaryCMap
{
    IEnumerable<JunctionId> Junctions { get; }
    IEnumerable<BoundaryDart> Darts { get; }
    JunctionId Origin(BoundaryDart dart);
    BoundaryDart Twin(BoundaryDart dart);
    BoundaryDart Next(BoundaryDart dart);
    IReadOnlyList<BoundaryDart> IncidentOrdered(JunctionId junction);
    bool ContainsDart(BoundaryDart dart);
}
