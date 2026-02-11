using FantaSim.Geosphere.Plate.Topology.Contracts.Derived;

namespace FantaSim.Geosphere.Plate.Topology.Contracts.Derived;

public interface IPlateTopologyIndexedStateView : IPlateTopologyStateView
{
    PlateTopologyIndices Indices { get; }
}
