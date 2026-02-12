using System.Threading;
using System.Threading.Tasks;
using FantaSim.World.Contracts.Time;
using FantaSim.Geosphere.Plate.Simulation.Des.Contracts;
using Plate.TimeDete.Time.Primitives;

namespace FantaSim.Geosphere.Plate.Simulation.Des.Drivers;

public sealed class GeospherePlateDriver : IAsyncDriver
{
    public DriverId Id => new("GeospherePlateDriver");
    public SphereId Sphere => SphereIds.Geosphere;

    public Task<DriverOutput> EvaluateAsync(DesContext context, CancellationToken ct = default)
    {
        var plateCount = context.State.Plates.Count;
        object? signal = null;

        if (plateCount == 0)
        {
            signal = "Genesis";
        }
        else
        {
            signal = "Step";
        }

        // Schedule next RunPlateSolver 10 ticks later
        var nextTick = context.CurrentTick + 10;
        context.Scheduler.Schedule(nextTick, SphereIds.Geosphere, DesWorkKind.RunPlateSolver);

        return Task.FromResult(new DriverOutput(signal));
    }
}
