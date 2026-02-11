using System.Runtime.InteropServices;
using UnifySerialization.Abstractions;
using FantaSim.Geosphere.Plate.Topology.Contracts.Numerics;

namespace FantaSim.Geosphere.Plate.Velocity.Contracts;

[UnifyModel]
[StructLayout(LayoutKind.Sequential)]
public readonly record struct BoundaryRateSample(
    [property: UnifyProperty(0)] Vector3d Position,
    [property: UnifyProperty(1)] double ArcLength,
    [property: UnifyProperty(2)] Velocity3d RelativeVelocity,
    [property: UnifyProperty(3)] Vector3d Tangent,
    [property: UnifyProperty(4)] Vector3d Normal,
    [property: UnifyProperty(5)] Vector3d Vertical,
    [property: UnifyProperty(6)] double NormalRate,
    [property: UnifyProperty(7)] double TangentialRate,
    [property: UnifyProperty(8)] double? VerticalRate,
    [property: UnifyProperty(9)] double RelativeSpeed,
    [property: UnifyProperty(10)] double RelativeAzimuth,
    [property: UnifyProperty(11)] double ObliquityAngle,
    [property: UnifyProperty(12)] RateUncertainty Uncertainty,
    [property: UnifyProperty(13)] SampleProvenance Provenance
);
