using System.Runtime.InteropServices;
using UnifySerialization.Abstractions;

namespace FantaSim.Geosphere.Plate.Velocity.Contracts;

[UnifyModel]
[StructLayout(LayoutKind.Sequential)]
public readonly record struct ConvergenceRateSummary(
    [property: UnifyProperty(0)] double MaxConvergenceRate,
    [property: UnifyProperty(1)] double MeanConvergenceRate,
    [property: UnifyProperty(2)] double TotalConvergentLength,
    [property: UnifyProperty(3)] int ConvergentSampleCount,
    [property: UnifyProperty(4)] double? MaxConvergenceUncertainty
);
