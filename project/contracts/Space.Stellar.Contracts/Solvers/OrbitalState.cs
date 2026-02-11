using System.Runtime.InteropServices;
using FantaSim.Space.Stellar.Contracts.Numerics;

namespace FantaSim.Space.Stellar.Contracts.Solvers;

[StructLayout(LayoutKind.Auto)]
public readonly record struct OrbitalState(
    Vector3d PositionM,
    Vector3d VelocityMPerS,
    double DistanceM,
    double SpeedMPerS,
    double TrueAnomalyRad,
    double EccentricAnomalyRad,
    double MeanAnomalyRad,
    double TimeS
);
