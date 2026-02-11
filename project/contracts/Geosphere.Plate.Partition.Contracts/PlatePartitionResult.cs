using System.Collections.Immutable;
using System.Runtime.InteropServices;
using FantaSim.Geosphere.Plate.Topology.Contracts.Entities;
using UnifySerialization.Abstractions;

namespace FantaSim.Geosphere.Plate.Partition.Contracts;

[UnifyModel]
[StructLayout(LayoutKind.Auto)]
public readonly record struct PlatePartitionResult
{
    [UnifyProperty(0)]
    public required IReadOnlyDictionary<PlateId, PlatePolygon> PlatePolygons { get; init; }

    [UnifyProperty(1)]
    public required PartitionQualityMetrics QualityMetrics { get; init; }

    [UnifyProperty(2)]
    public required PartitionProvenance Provenance { get; init; }

    [UnifyProperty(3)]
    public required PartitionValidity Status { get; init; }
}
