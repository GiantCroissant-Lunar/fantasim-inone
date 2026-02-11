using System.Collections.Immutable;
using System.Runtime.InteropServices;
using UnifySerialization.Abstractions;
using Plate.TimeDete.Time.Primitives;

namespace FantaSim.Geosphere.Plate.Velocity.Contracts;

[UnifyModel]
[StructLayout(LayoutKind.Sequential)]
public readonly record struct BoundaryVelocityCollection(
    [property: UnifyProperty(0)] CanonicalTick Tick,
    [property: UnifyProperty(1)] ImmutableArray<BoundaryVelocityProfile> Profiles,
    [property: UnifyProperty(2)] string SolverId
);
