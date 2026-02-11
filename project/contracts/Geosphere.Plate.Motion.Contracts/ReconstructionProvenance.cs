using System.Runtime.InteropServices;
using UnifySerialization.Abstractions;
using FantaSim.Geosphere.Plate.Kinematics.Contracts.Entities;
using FantaSim.Geosphere.Plate.Topology.Contracts.Entities;

namespace FantaSim.Geosphere.Plate.Motion.Contracts;

[UnifyModel]
[StructLayout(LayoutKind.Sequential)]
public readonly record struct ReconstructionProvenance(
    [property: UnifyProperty(0)] MotionSegmentId KinematicsSegment,
    [property: UnifyProperty(1)] BoundaryId? CrossedBoundary,
    [property: UnifyProperty(2)] double InterpolationFactor
);
