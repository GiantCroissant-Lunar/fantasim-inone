using Plate.TimeDete.Time.Primitives;
using FantaSim.Geosphere.Plate.Kinematics.Contracts.Derived;
using FantaSim.Geosphere.Plate.Topology.Contracts.Entities;
using FantaSim.Geosphere.Plate.Topology.Contracts.Numerics;

namespace FantaSim.Geosphere.Plate.Velocity.Contracts;

/// <summary>
/// Computes plate velocities from kinematics truth.
/// </summary>
public interface IPlateVelocitySolver
{
    Velocity3d GetAbsoluteVelocity(
        IPlateKinematicsStateView kinematics,
        PlateId plateId,
        Vector3d point,
        CanonicalTick tick);

    Velocity3d GetRelativeVelocity(
        IPlateKinematicsStateView kinematics,
        PlateId plateIdA,
        PlateId plateIdB,
        Vector3d point,
        CanonicalTick tick);

    AngularVelocity3d GetAngularVelocity(
        IPlateKinematicsStateView kinematics,
        PlateId plateId,
        CanonicalTick tick);
}
