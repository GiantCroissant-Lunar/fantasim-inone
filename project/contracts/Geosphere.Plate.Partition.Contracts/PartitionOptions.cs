using System.Runtime.InteropServices;
using UnifySerialization.Abstractions;

namespace FantaSim.Geosphere.Plate.Partition.Contracts;

[UnifyModel]
[StructLayout(LayoutKind.Auto)]
public readonly record struct PartitionOptions
{
    [UnifyProperty(0)]
    public int MaxIterations { get; init; }

    [UnifyProperty(1)]
    public double MinPolygonArea { get; init; }

    [UnifyProperty(2)]
    public bool IncludeDiagnostics { get; init; }

    [UnifyProperty(3)]
    public bool ValidateTopology { get; init; }

    public PartitionOptions()
    {
        MaxIterations = 100;
        MinPolygonArea = 1e-12;
        IncludeDiagnostics = true;
        ValidateTopology = true;
    }
}
