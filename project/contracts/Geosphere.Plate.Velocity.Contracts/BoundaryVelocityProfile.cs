using System.Collections.Immutable;
using System.Runtime.InteropServices;
using UnifySerialization.Abstractions;
using FantaSim.Geosphere.Plate.Topology.Contracts.Entities;

namespace FantaSim.Geosphere.Plate.Velocity.Contracts;

[UnifyModel]
[StructLayout(LayoutKind.Sequential)]
public readonly record struct BoundaryVelocityProfile(
    [property: UnifyProperty(0)] BoundaryId BoundaryId,
    [property: UnifyProperty(1)] int SampleCount,
    [property: UnifyProperty(2)] double MinNormalRate,
    [property: UnifyProperty(3)] double MaxNormalRate,
    [property: UnifyProperty(4)] double MeanNormalRate,
    [property: UnifyProperty(5)] double MeanSlipRate,
    [property: UnifyProperty(6)] int MinSampleIndex,
    [property: UnifyProperty(7)] int MaxSampleIndex
);
