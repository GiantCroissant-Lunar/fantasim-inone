using System.Runtime.InteropServices;
using FantaSim.Geosphere.Plate.Topology.Contracts.Entities;
using UnifySerialization.Abstractions;
using Plate.TimeDete.Time.Primitives;

namespace FantaSim.Geosphere.Plate.Partition.Contracts;

[UnifyModel]
[StructLayout(LayoutKind.Auto)]
public readonly record struct PartitionRequest
{
    [UnifyProperty(0)]
    public required CanonicalTick Tick { get; init; }

    [UnifyProperty(1)]
    public required TolerancePolicy TolerancePolicy { get; init; }

    [UnifyProperty(2)]
    public PartitionOptions? Options { get; init; }
}
