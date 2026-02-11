using System.Runtime.InteropServices;
using UnifySerialization.Abstractions;

namespace FantaSim.Geosphere.Plate.Velocity.Contracts;

[UnifyModel]
[StructLayout(LayoutKind.Sequential)]
public readonly record struct StrikeSlipSummary(
    [property: UnifyProperty(0)] double MaxStrikeSlipRate,
    [property: UnifyProperty(1)] double MeanStrikeSlipRate,
    [property: UnifyProperty(2)] double TotalStrikeSlipLength,
    [property: UnifyProperty(3)] int StrikeSlipSampleCount,
    [property: UnifyProperty(4)] int RightLateralCount,
    [property: UnifyProperty(5)] int LeftLateralCount
);
