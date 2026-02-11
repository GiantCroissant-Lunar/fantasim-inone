using FantaSim.Geosphere.Plate.Kinematics.Contracts.Numerics;
using Plate.TimeDete.Time.Primitives;

namespace FantaSim.Geosphere.Plate.Kinematics.Contracts.TruePolarWander;

/// <summary>
/// Represents a True Polar Wander (TPW) model that provides rotations
/// accounting for the motion of Earth's spin axis relative to the mantle.
/// </summary>
public interface ITruePolarWanderModel
{
    FiniteRotation GetRotationAt(CanonicalTick tick);
}
