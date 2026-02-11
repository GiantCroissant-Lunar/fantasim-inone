using System.Runtime.InteropServices;
using UnifySerialization.Abstractions;

namespace FantaSim.Geosphere.Plate.Velocity.Contracts;

[UnifyModel]
[StructLayout(LayoutKind.Sequential)]
public readonly record struct SpreadingRateMetrics(
    [property: UnifyProperty(0)] double FullRate,
    [property: UnifyProperty(1)] double HalfRate,
    [property: UnifyProperty(2)] double Asymmetry,
    [property: UnifyProperty(3)] double Obliquity,
    [property: UnifyProperty(4)] double AlongStrikeVariation
);
