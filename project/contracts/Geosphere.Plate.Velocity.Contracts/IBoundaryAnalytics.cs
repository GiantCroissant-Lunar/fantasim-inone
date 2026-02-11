using Plate.TimeDete.Time.Primitives;
using FantaSim.Geosphere.Plate.Kinematics.Contracts;
using FantaSim.Geosphere.Plate.Topology.Contracts.Entities;

namespace FantaSim.Geosphere.Plate.Velocity.Contracts;

/// <summary>
/// Boundary-local relative velocity decomposition (RFC-V2-0048).
/// </summary>
public interface IBoundaryAnalytics
{
    ValueTask<BoundaryRateProfile> SampleBoundaryVelocitiesAsync(
        BoundaryId boundaryId,
        CanonicalTick tick,
        BoundarySampleSpec sampling,
        ReferenceFrameId? frame = null);

    ValueTask<IReadOnlyList<BoundaryRateProfile>> SampleBoundariesAsync(
        IEnumerable<BoundaryId> boundaryIds,
        CanonicalTick tick,
        BoundarySampleSpec sampling,
        ReferenceFrameId? frame = null);

    SpreadingRateMetrics ComputeSpreadingMetrics(BoundaryRateProfile profile);

    ConvergenceRateSummary GetConvergenceRateSummary(BoundaryRateProfile profile);

    StrikeSlipSummary GetStrikeSlipSummary(BoundaryRateProfile profile);

    StrikeSlipSense GetStrikeSlipSense(double tangentialRate);

    double GetConvergenceRate(double normalRate);

    double GetDivergenceRate(double normalRate);

    double GetStrikeSlipRate(double tangentialRate);

    double ComputeObliquityAngle(double normalRate, double tangentialRate);
}
