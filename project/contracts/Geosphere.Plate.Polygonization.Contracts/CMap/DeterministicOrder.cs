using FantaSim.Geosphere.Plate.Topology.Contracts.Entities;

namespace FantaSim.Geosphere.Plate.Polygonization.Contracts.CMap;

/// <summary>
/// Canonical deterministic ordering for darts and related topology elements.
/// </summary>
public static class DeterministicOrder
{
    public static int CompareDarts(
        AnglePolicy policy,
        double angleA, BoundaryDart dartA, JunctionId junctionIdA,
        double angleB, BoundaryDart dartB, JunctionId junctionIdB)
    {
        var angleCompare = policy.CompareAngles(angleA, angleB);
        if (angleCompare != 0) return angleCompare;

        var junctionCompare = junctionIdA.Value.CompareTo(junctionIdB.Value);
        if (junctionCompare != 0) return junctionCompare;

        return dartA.CompareTo(dartB);
    }

    public static int CompareDarts(
        AnglePolicy policy,
        double angleA, BoundaryDart dartA,
        double angleB, BoundaryDart dartB)
    {
        var angleCompare = policy.CompareAngles(angleA, angleB);
        if (angleCompare != 0) return angleCompare;

        return dartA.CompareTo(dartB);
    }

    public static int CompareDarts(
        double angleA, BoundaryDart dartA,
        double angleB, BoundaryDart dartB)
        => CompareDarts(AnglePolicy.Default, angleA, dartA, angleB, dartB);

    public static double ComputeAngle(double dx, double dy)
        => Math.Atan2(dy, dx);
}
