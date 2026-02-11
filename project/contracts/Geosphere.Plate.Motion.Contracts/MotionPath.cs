using System.Collections.Immutable;
using System.Runtime.InteropServices;
using UnifySerialization.Abstractions;
using Plate.TimeDete.Time.Primitives;
using FantaSim.Geosphere.Plate.Topology.Contracts.Entities;
using FantaSim.Geosphere.Plate.Kinematics.Contracts;
using FantaSim.Geosphere.Plate.Kinematics.Contracts.Derived;
using FantaSim.Geosphere.Plate.Topology.Contracts.Numerics;

namespace FantaSim.Geosphere.Plate.Motion.Contracts;

[StructLayout(LayoutKind.Auto)]
[UnifyModel]
public readonly record struct MotionPath(
    [property: UnifyProperty(0)] PlateId AnchorPlate,
    [property: UnifyProperty(1)] CanonicalTick StartTick,
    [property: UnifyProperty(2)] CanonicalTick EndTick,
    [property: UnifyProperty(3)] IntegrationDirection Direction,
    [property: UnifyProperty(4)] ReferenceFrameId Frame,
    [property: UnifyProperty(5)] ImmutableArray<MotionPathSample> Samples
);
