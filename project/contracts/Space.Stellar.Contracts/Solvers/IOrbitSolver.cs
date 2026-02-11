using FantaSim.Space.Stellar.Contracts.Mechanics;
using FantaSim.Space.Stellar.Contracts.Numerics;

namespace FantaSim.Space.Stellar.Contracts.Solvers;

/// <summary>
/// Solver for orbital mechanics calculations.
/// Converts between orbital elements and Cartesian state vectors.
/// </summary>
public interface IOrbitSolver
{
    Vector3d CalculatePosition(OrbitalElements orbit, double centralMassKg, double timeS);

    Vector3d CalculateVelocity(OrbitalElements orbit, double centralMassKg, double timeS);

    OrbitalState CalculateOrbitalState(OrbitalElements orbit, double centralMassKg, double timeS);

    double FindTimeAtTrueAnomaly(OrbitalElements orbit, double centralMassKg, double targetTrueAnomalyRad, double afterTimeS);

    double MeanToTrueAnomaly(double meanAnomalyRad, double eccentricity);

    double TrueToEccentricAnomaly(double trueAnomalyRad, double eccentricity);
}
