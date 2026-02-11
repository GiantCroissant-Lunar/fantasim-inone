using System.Runtime.InteropServices;
using UnifySerialization.Abstractions;
using FantaSim.Geosphere.Plate.Topology.Contracts.Numerics;

namespace FantaSim.Geosphere.Plate.Velocity.Contracts;

[UnifyModel]
[StructLayout(LayoutKind.Sequential)]
public readonly record struct BoundaryVelocitySample(
    [property: UnifyProperty(0)] Vector3d Position,
    [property: UnifyProperty(1)] Velocity3d RelativeVelocity,
    [property: UnifyProperty(2)] Vector3d Tangent,
    [property: UnifyProperty(3)] Vector3d Normal,
    [property: UnifyProperty(4)] double TangentialRate,
    [property: UnifyProperty(5)] double NormalRate,
    [property: UnifyProperty(6)] int SampleIndex
);
