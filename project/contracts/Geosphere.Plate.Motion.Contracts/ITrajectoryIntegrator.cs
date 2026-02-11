using System.Collections.Immutable;
using Plate.TimeDete.Time.Primitives;
using UnifyGeometry;
using FantaSim.Geosphere.Plate.Topology.Contracts.Derived;
using FantaSim.Geosphere.Plate.Kinematics.Contracts.Derived;
using FantaSim.Geosphere.Plate.Topology.Contracts.Entities;

namespace FantaSim.Geosphere.Plate.Motion.Contracts;

public interface ITrajectoryIntegrator
{
    Trajectory Integrate(TrajectoryIntegrationContext context);
}

public sealed record TrajectoryIntegrationContext
{
    public required Point3 SeedPoint { get; init; }
    public required PlateId SeedPlateId { get; init; }
    public required CanonicalTick StartTick { get; init; }
    public required CanonicalTick EndTick { get; init; }
    public required StepPolicy Policy { get; init; }
    public required IPlateTopologyStateView Topology { get; init; }
    public required IPlateKinematicsStateView Kinematics { get; init; }
    public ITrajectorySampler? CustomSampler { get; init; }
    public IntegrationDirection Direction { get; init; } = IntegrationDirection.Forward;
}
