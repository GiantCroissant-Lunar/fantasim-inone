using System.Collections.Immutable;
using System.Runtime.InteropServices;
using UnifySerialization.Abstractions;
using FantaSim.Geosphere.Plate.Kinematics.Contracts;
using FantaSim.Geosphere.Plate.Topology.Contracts.Entities;
using Plate.TimeDete.Time.Primitives;

namespace FantaSim.Geosphere.Plate.Velocity.Contracts;

[UnifyModel]
[StructLayout(LayoutKind.Sequential)]
public readonly record struct BoundaryRateProfile(
    [property: UnifyProperty(0)] BoundaryId BoundaryId,
    [property: UnifyProperty(1)] BoundaryType Type,
    [property: UnifyProperty(2)] CanonicalTick Tick,
    [property: UnifyProperty(3)] ReferenceFrameId Frame,
    [property: UnifyProperty(4)] ImmutableArray<BoundaryRateSample> Samples,
    [property: UnifyProperty(5)] RateStatistics Statistics,
    [property: UnifyProperty(6)] ConvergenceRateSummary? ConvergenceSummary,
    [property: UnifyProperty(7)] SpreadingRateMetrics? SpreadingMetrics,
    [property: UnifyProperty(8)] StrikeSlipSummary? StrikeSlipSummary
)
{
    [UnifyIgnore]
    public bool IsConvergent => Type == BoundaryType.Convergent;

    [UnifyIgnore]
    public bool IsDivergent => Type == BoundaryType.Divergent;

    [UnifyIgnore]
    public bool IsTransform => Type == BoundaryType.Transform;

    [UnifyIgnore]
    public double MaxConvergenceRate => ConvergenceSummary?.MaxConvergenceRate ?? 0.0;

    [UnifyIgnore]
    public double MeanDivergenceRate => SpreadingMetrics?.HalfRate ?? 0.0;

    [UnifyIgnore]
    public double FullSpreadingRate => SpreadingMetrics?.FullRate ?? 0.0;
}
