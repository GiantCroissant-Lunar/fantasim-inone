using Plate.TimeDete.Time.Primitives;
using FantaSim.Geosphere.Plate.Kinematics.Contracts.Derived;
using FantaSim.Geosphere.Plate.Topology.Contracts.Derived;
using FantaSim.Geosphere.Plate.Topology.Contracts.Entities;

namespace FantaSim.Geosphere.Plate.Velocity.Contracts;

public interface IBoundaryVelocitySolver
{
    BoundaryVelocityProfile AnalyzeBoundary(
        Boundary boundary,
        BoundarySampleSpec sampling,
        CanonicalTick tick,
        IPlateTopologyStateView topology,
        IPlateKinematicsStateView kinematics);

    BoundaryVelocityCollection AnalyzeAllBoundaries(
        IEnumerable<Boundary> boundaries,
        BoundarySampleSpec sampling,
        CanonicalTick tick,
        IPlateTopologyStateView topology,
        IPlateKinematicsStateView kinematics);
}
