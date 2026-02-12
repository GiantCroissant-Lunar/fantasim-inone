using FantaSim.Geosphere.Plate.Simulation.Des.Contracts;
using FantaSim.Geosphere.Plate.Simulation.Des.Runtime;
using FantaSim.World.Contracts.Time;
using FluentAssertions;
using Plate.TimeDete.Time.Primitives;
using Xunit;

namespace FantaSim.Geosphere.Plate.Simulation.Des.Tests;

public class DesSchedulerTests
{
    [Fact]
    public void Schedule_AssignsMonotonicallyIncreasingTieBreak()
    {
        var queue = new PriorityQueueDesQueue();
        var scheduler = new DesScheduler(queue);
        var tick = new CanonicalTick(0);

        scheduler.Schedule(tick, SphereIds.Geosphere, DesWorkKind.RunPlateSolver);
        scheduler.Schedule(tick, SphereIds.Geosphere, DesWorkKind.RunPlateSolver);
        scheduler.Schedule(tick, SphereIds.Geosphere, DesWorkKind.RunPlateSolver);

        queue.Count.Should().Be(3);

        queue.TryDequeue(out var first).Should().BeTrue();
        queue.TryDequeue(out var second).Should().BeTrue();
        queue.TryDequeue(out var third).Should().BeTrue();

        first.TieBreak.Should().Be(0);
        second.TieBreak.Should().Be(1);
        third.TieBreak.Should().Be(2);
    }

    [Fact]
    public void PriorityQueue_DequeueDeterministicOrder()
    {
        var queue = new PriorityQueueDesQueue();
        var scheduler = new DesScheduler(queue);

        // Schedule in reverse order — queue should still dequeue in deterministic order
        scheduler.Schedule(new CanonicalTick(10), SphereIds.Geosphere, DesWorkKind.RunPlateSolver);
        scheduler.Schedule(new CanonicalTick(0), SphereIds.Geosphere, DesWorkKind.RunPlateSolver);
        scheduler.Schedule(new CanonicalTick(5), SphereIds.Biosphere, DesWorkKind.EmitObservations);

        queue.TryDequeue(out var first).Should().BeTrue();
        first.When.Should().Be(new CanonicalTick(0));

        queue.TryDequeue(out var second).Should().BeTrue();
        second.When.Should().Be(new CanonicalTick(5));

        queue.TryDequeue(out var third).Should().BeTrue();
        third.When.Should().Be(new CanonicalTick(10));
    }
}
