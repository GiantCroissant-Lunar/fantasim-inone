using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FantaSim.Geosphere.Plate.Simulation.Des.Core;
using FantaSim.Geosphere.Plate.Simulation.Des.Contracts;
using FantaSim.Geosphere.Plate.Topology.Contracts.Events;

namespace FantaSim.Geosphere.Plate.Simulation.Des.Runtime;

public interface IDesDispatcher
{
    Task<IReadOnlyList<ITruthEventDraft>> DispatchAsync(
        ScheduledWorkItem item,
        DesContext context,
        CancellationToken ct);
}
