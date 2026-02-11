using System.Collections.Immutable;
using System.Runtime.InteropServices;
using FantaSim.Geosphere.Plate.Topology.Contracts.Entities;
using MessagePack;
using Plate.TimeDete.Time.Primitives;

namespace FantaSim.Geosphere.Plate.Polygonization.Contracts.Products;

[StructLayout(LayoutKind.Auto)]
[MessagePackObject]
public readonly record struct BoundaryFaceAdjacency(
    [property: Key(0)] BoundaryId BoundaryId,
    [property: Key(1)] int SegmentIndex,
    [property: Key(2)] PlateId LeftPlateId,
    [property: Key(3)] PlateId RightPlateId
);

[StructLayout(LayoutKind.Auto)]
[MessagePackObject]
public readonly record struct BoundaryFaceAdjacencyMap(
    [property: Key(0)] CanonicalTick Tick,
    [property: Key(1)] ImmutableArray<BoundaryFaceAdjacency> Adjacencies
);
