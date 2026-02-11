using System.Runtime.InteropServices;
using UnifySerialization.Abstractions;

namespace FantaSim.Geosphere.Plate.Velocity.Contracts;

[UnifyModel]
[StructLayout(LayoutKind.Auto)]
public readonly record struct BoundarySampleSpec
{
    [property: UnifyProperty(0)]
    public int? SampleCount { get; init; }

    [property: UnifyProperty(1)]
    public double? Spacing { get; init; }

    [property: UnifyProperty(2)]
    public SamplingMode Mode { get; init; }

    [property: UnifyProperty(3)]
    public double? JunctionBufferDistance { get; init; }

    [property: UnifyProperty(4)]
    public InterpolationMethod Interpolation { get; init; }
}
