using FantaSim.Geosphere.Plate.Topology.Contracts.Derived;

namespace FantaSim.Geosphere.Plate.Polygonization.Contracts.CMap;

/// <summary>
/// Factory for building a boundary combinatorial map from topology state.
/// RFC-V2-0041 §11.1.
/// </summary>
public interface IBoundaryCMapBuilder
{
    IBoundaryCMap Build(IPlateTopologyStateView topology);
}
