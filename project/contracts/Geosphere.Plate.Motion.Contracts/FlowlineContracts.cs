using System.Collections.Immutable;
using Plate.TimeDete.Time.Primitives;
using UnifyGeometry;
using FantaSim.Geosphere.Plate.Topology.Contracts.Entities;
using FantaSim.Geosphere.Plate.Topology.Contracts.Numerics;

namespace FantaSim.Geosphere.Plate.Motion.Contracts;

public record Flowline(
    Point3 SeedPoint,
    BoundaryId SourceBoundary,
    PlateSide Side,
    CanonicalTick StartTick,
    CanonicalTick EndTick,
    SpreadingModel SpreadingModel,
    ImmutableArray<FlowlineSample> Samples
);

public record FlowlineSample(
    CanonicalTick Tick,
    Point3 Position,
    PlateId PlateId,
    Vector3d Velocity,
    ReconstructionProvenance Provenance,
    double AccumulatedError,
    double SpreadingRate,
    double AccumulatedOpening,
    bool IsRidgeSegment,
    double? SubductionAge
);

public record SpreadingModel(SpreadingModelType Type);

public enum SpreadingModelType
{
    Uniform,
    VelocityBased,
    AgeBased
}

public interface IFlowlineSolver
{
    Flowline ComputeFlowline(
        Point3 seedPoint,
        BoundaryId boundaryId,
        PlateSide side,
        SpreadingModel spreadingModel,
        CanonicalTick tickA,
        CanonicalTick tickB,
        StepPolicy stepPolicy,
        FantaSim.Geosphere.Plate.Topology.Contracts.Derived.IPlateTopologyStateView topology,
        FantaSim.Geosphere.Plate.Kinematics.Contracts.Derived.IPlateKinematicsStateView kinematics);

    ImmutableArray<Flowline> ComputeFlowlineBundle(
        BoundaryId boundaryId,
        PlateSide side,
        double sampleSpacing,
        SpreadingModel spreadingModel,
        CanonicalTick tickA,
        CanonicalTick tickB,
        StepPolicy stepPolicy,
        FantaSim.Geosphere.Plate.Topology.Contracts.Derived.IPlateTopologyStateView topology,
        FantaSim.Geosphere.Plate.Kinematics.Contracts.Derived.IPlateKinematicsStateView kinematics);
}
