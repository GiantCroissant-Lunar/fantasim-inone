using System.Runtime.InteropServices;
using UnifySerialization.Abstractions;

namespace FantaSim.Geosphere.Plate.Velocity.Contracts;

[UnifyModel]
[StructLayout(LayoutKind.Sequential)]
public readonly record struct RateStatistics(
    [property: UnifyProperty(0)] double MinNormalRate,
    [property: UnifyProperty(1)] double MaxNormalRate,
    [property: UnifyProperty(2)] double MeanNormalRate,
    [property: UnifyProperty(3)] double MinTangentialRate,
    [property: UnifyProperty(4)] double MaxTangentialRate,
    [property: UnifyProperty(5)] double MeanTangentialRate,
    [property: UnifyProperty(6)] double MaxRelativeSpeed,
    [property: UnifyProperty(7)] double MeanRelativeSpeed
);
