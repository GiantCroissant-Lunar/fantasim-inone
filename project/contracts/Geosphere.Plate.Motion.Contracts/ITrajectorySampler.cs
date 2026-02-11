using System.Collections.Generic;
using Plate.TimeDete.Time.Primitives;
using UnifyGeometry;
using FantaSim.Geosphere.Plate.Topology.Contracts.Entities;
using FantaSim.Geosphere.Plate.Topology.Contracts.Numerics;

namespace FantaSim.Geosphere.Plate.Motion.Contracts;

public interface ITrajectorySampler
{
    bool ShouldSample(TrajectoryIntegrationState state);
    Dictionary<string, object> ComputeMetadata(TrajectoryIntegrationState state);
}

public sealed record TrajectoryIntegrationState
{
    public required Point3 CurrentPosition { get; init; }
    public required Vector3d CurrentVelocity { get; init; }
    public required PlateId CurrentPlateId { get; init; }
    public required CanonicalTick CurrentTick { get; init; }
    public required double AccumulatedError { get; init; }
    public required int StepCount { get; init; }
    public TrajectorySample? PreviousSample { get; init; }
}
