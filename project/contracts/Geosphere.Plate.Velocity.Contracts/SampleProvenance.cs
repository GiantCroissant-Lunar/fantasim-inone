using System.Runtime.InteropServices;
using UnifySerialization.Abstractions;

namespace FantaSim.Geosphere.Plate.Velocity.Contracts;

[UnifyModel]
[StructLayout(LayoutKind.Sequential)]
public readonly record struct SampleProvenance(
    [property: UnifyProperty(0)] int SampleIndex,
    [property: UnifyProperty(1)] int SegmentIndex,
    [property: UnifyProperty(2)] double SegmentT
);
