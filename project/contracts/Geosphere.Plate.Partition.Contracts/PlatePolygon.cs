using System.Collections.Immutable;
using System.Runtime.InteropServices;
using FantaSim.Geosphere.Plate.Topology.Contracts.Entities;
using UnifySerialization.Abstractions;
using UnifyGeometry;

namespace FantaSim.Geosphere.Plate.Partition.Contracts;

[UnifyModel]
[StructLayout(LayoutKind.Auto)]
public readonly record struct PlatePolygon
{
    [UnifyProperty(0)]
    public required PlateId PlateId { get; init; }

    [UnifyProperty(1)]
    public required Polygon OuterBoundary { get; init; }

    [UnifyProperty(2)]
    public ImmutableArray<Polygon> Holes { get; init; }

    [UnifyProperty(3)]
    public required double SphericalArea { get; init; }

    public PlatePolygon()
    {
        Holes = ImmutableArray<Polygon>.Empty;
    }
}
