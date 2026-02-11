using System.Runtime.InteropServices;
using UnifySerialization.Abstractions;

namespace FantaSim.Geosphere.Plate.Partition.Contracts;

[UnifyModel]
[StructLayout(LayoutKind.Auto)]
public readonly record struct PartitionQualityMetrics
{
    [UnifyProperty(0)]
    public double MinArea { get; init; }

    [UnifyProperty(1)]
    public double MaxArea { get; init; }

    [UnifyProperty(2)]
    public double AreaVariance { get; init; }

    [UnifyProperty(3)]
    public int SliverCount { get; init; }

    [UnifyProperty(4)]
    public int OpenBoundaryCount { get; init; }

    [UnifyProperty(5)]
    public int NonManifoldJunctionCount { get; init; }

    [UnifyProperty(6)]
    public int AmbiguousAttributionCount { get; init; }

    [UnifyProperty(7)]
    public int FaceCount { get; init; }

    [UnifyProperty(8)]
    public int HoleCount { get; init; }

    [UnifyProperty(9)]
    public double ComputationTimeMs { get; init; }
}
