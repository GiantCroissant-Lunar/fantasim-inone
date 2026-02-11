using FantaSim.Geosphere.Plate.Topology.Contracts.Derived;

namespace FantaSim.Geosphere.Plate.Topology.Materializer;

public static class PlateTopologyIndexAccess
{
    public static PlateTopologyIndices GetPlateAdjacency(IPlateTopologyStateView state)
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(state);
#else
        if (state is null) throw new ArgumentNullException(nameof(state));
#endif

        if (state is IPlateTopologyIndexedStateView indexed)
            return indexed.Indices;

        return PlateTopologyIndicesBuilder.BuildPlateAdjacency(state);
    }
}
