using FantaSim.Geosphere.Plate.Runtime.Des.Contracts;
using FantaSim.Geosphere.Plate.Runtime.Des.Core;
using FantaSim.World.Contracts.Time;
using FluentAssertions;
using Plate.TimeDete.Time.Primitives;
using Xunit;

namespace FantaSim.Geosphere.Plate.Runtime.Des.Tests;

public class ScheduledWorkItemTests
{
    [Fact]
    public void CompareTo_OrdersByTick_ThenSphere_ThenKind_ThenTieBreak()
    {
        var tick0 = new CanonicalTick(0);
        var tick1 = new CanonicalTick(1);

        var a = new ScheduledWorkItem(tick0, SphereIds.Geosphere, DesWorkKind.RunPlateSolver, 0);
        var b = new ScheduledWorkItem(tick1, SphereIds.Geosphere, DesWorkKind.RunPlateSolver, 0);

        a.CompareTo(b).Should().BeNegative("earlier tick should come first");
    }

    [Fact]
    public void CompareTo_SameTick_GeosphereBeforeBiosphere()
    {
        var tick = new CanonicalTick(5);

        var geo = new ScheduledWorkItem(tick, SphereIds.Geosphere, DesWorkKind.RunPlateSolver, 0);
        var bio = new ScheduledWorkItem(tick, SphereIds.Biosphere, DesWorkKind.RunPlateSolver, 0);

        geo.CompareTo(bio).Should().BeNegative("geosphere should be processed before biosphere");
    }

    [Fact]
    public void CompareTo_SameTickAndSphere_OrderByKind()
    {
        var tick = new CanonicalTick(5);

        var solver = new ScheduledWorkItem(tick, SphereIds.Geosphere, DesWorkKind.RunPlateSolver, 0);
        var derived = new ScheduledWorkItem(tick, SphereIds.Geosphere, DesWorkKind.ComputeDerivedFields, 0);

        solver.CompareTo(derived).Should().BeNegative("RunPlateSolver (0) should come before ComputeDerivedFields (1)");
    }

    [Fact]
    public void CompareTo_AllEqual_UseTieBreak()
    {
        var tick = new CanonicalTick(5);

        var first = new ScheduledWorkItem(tick, SphereIds.Geosphere, DesWorkKind.RunPlateSolver, 0);
        var second = new ScheduledWorkItem(tick, SphereIds.Geosphere, DesWorkKind.RunPlateSolver, 1);

        first.CompareTo(second).Should().BeNegative("lower tiebreak should come first");
    }
}
