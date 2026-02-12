using FantaSim.World.Contracts.Time;
using Plate.TimeDete.Time.Primitives;
using FantaSim.Geosphere.Plate.Simulation.Des.Contracts;

namespace FantaSim.Geosphere.Plate.Simulation.Des.Core;

public readonly record struct ScheduledWorkItem(
    CanonicalTick When,
    SphereId Sphere,
    DesWorkKind Kind,
    ulong TieBreak,
    object? Payload = null
) : IComparable<ScheduledWorkItem>
{
    public int CompareTo(ScheduledWorkItem other)
    {
        // 1. When (ascending)
        int tickComparison = When.CompareTo(other.When);
        if (tickComparison != 0) return tickComparison;

        // 2. Sphere (ascending by fixed priority)
        int sphereComparison = GetSpherePriority(Sphere).CompareTo(GetSpherePriority(other.Sphere));
        if (sphereComparison != 0) return sphereComparison;

        // 3. Kind (ascending)
        int kindComparison = Kind.CompareTo(other.Kind);
        if (kindComparison != 0) return kindComparison;

        // 4. TieBreak (ascending)
        return TieBreak.CompareTo(other.TieBreak);
    }

    private static int GetSpherePriority(SphereId sphere)
    {
        if (sphere == SphereIds.Geosphere) return 100;
        if (sphere == SphereIds.Biosphere) return 200;
        if (sphere == SphereIds.Noosphere) return 300;
        return 999;
    }
}
