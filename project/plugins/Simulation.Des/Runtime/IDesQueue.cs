using FantaSim.Geosphere.Plate.Simulation.Des.Core;

namespace FantaSim.Geosphere.Plate.Simulation.Des.Runtime;

public interface IDesQueue
{
    void Enqueue(ScheduledWorkItem item);
    bool TryDequeue(out ScheduledWorkItem item);
    int Count { get; }
    bool TryPeek(out ScheduledWorkItem item);
}
