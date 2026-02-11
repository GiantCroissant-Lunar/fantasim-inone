using System.Runtime.InteropServices;
using FantaSim.Geosphere.Plate.Topology.Contracts.Identity;
using UnifySerialization.Abstractions;

namespace FantaSim.Geosphere.Plate.Partition.Contracts;

[UnifyModel]
[StructLayout(LayoutKind.Auto)]
public readonly record struct PartitionProvenance
{
    [UnifyProperty(0)]
    public required TruthStreamIdentity TopologySource { get; init; }

    [UnifyProperty(1)]
    public required string PolygonizerVersion { get; init; }

    [UnifyProperty(2)]
    public required DateTimeOffset ComputedAt { get; init; }

    [UnifyProperty(3)]
    public required string AlgorithmHash { get; init; }
}
