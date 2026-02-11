using System.Collections.Immutable;
using Plate.TimeDete.Time.Primitives;
using UnifyGeometry;
using FantaSim.Geosphere.Plate.Topology.Contracts.Entities;
using FantaSim.Geosphere.Plate.Topology.Contracts.Numerics;

namespace FantaSim.Geosphere.Plate.Motion.Contracts;

public sealed record Trajectory
{
    public required ImmutableArray<TrajectorySample> Samples { get; init; }
    public required double TotalAccumulatedError { get; init; }
    public required int BoundaryCrossingCount { get; init; }
    public required TrajectoryProvenance Provenance { get; init; }
    public required IntegrationStatistics Statistics { get; init; }
}

public sealed record TrajectorySample
{
    public required CanonicalTick Tick { get; init; }
    public required Point3 Position { get; init; }
    public required PlateId PlateId { get; init; }
    public required Vector3d Velocity { get; init; }
    public required double AccumulatedError { get; init; }
    public required SampleProvenance Provenance { get; init; }
    public BoundaryId? CrossedBoundary { get; init; }
}

public sealed record TrajectoryProvenance
{
    public string IntegratorVersion { get; init; } = "1.0";
}

public sealed record SampleProvenance
{
    public ReconstructionProvenance ReconstructionInfo { get; init; }
}

public sealed record IntegrationStatistics
{
    public int SampleCount { get; init; }
    public double ComputationTimeMs { get; init; }
}
