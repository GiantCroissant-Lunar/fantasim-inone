using FantaSim.Geosphere.Plate.Polygonization.Contracts.Products;
using FantaSim.Geosphere.Plate.Topology.Contracts.Derived;
using Plate.TimeDete.Time.Primitives;

namespace FantaSim.Geosphere.Plate.Polygonization.Contracts.Solvers;

public interface IPlatePolygonizer
{
    PlatePolygonSet PolygonizeAtTick(
        CanonicalTick tick,
        IPlateTopologyStateView topology,
        PolygonizationOptions? options = null);

    BoundaryFaceAdjacencyMap GetBoundaryFaceAdjacency(
        CanonicalTick tick,
        IPlateTopologyStateView topology,
        PolygonizationOptions? options = null);

    PolygonizationDiagnostics Validate(
        CanonicalTick tick,
        IPlateTopologyStateView topology,
        PolygonizationOptions? options = null);
}
