using System.Runtime.InteropServices;
using UnifySerialization.Abstractions;
using Plate.TimeDete.Time.Primitives;
using UnifyGeometry;
using FantaSim.Geosphere.Plate.Velocity.Contracts;
using FantaSim.Geosphere.Plate.Topology.Contracts.Entities;
using FantaSim.Geosphere.Plate.Topology.Contracts.Numerics;

namespace FantaSim.Geosphere.Plate.Motion.Contracts;

[UnifyModel]
[StructLayout(LayoutKind.Sequential)]
public readonly record struct MotionPathSample(
    [property: UnifyProperty(0)] CanonicalTick Tick,
    [property: UnifyProperty(1)] Point3 Position,
    [property: UnifyProperty(2)] PlateId PlateId,
    [property: UnifyProperty(3)] Vector3d Velocity,
    [property: UnifyProperty(4)] ReconstructionProvenance Provenance,
    [property: UnifyProperty(5)] double AccumulatedError
);
