using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FantaSim.Geosphere.Plate.Runtime.Des.Contracts;
using FantaSim.Geosphere.Plate.Runtime.Des.Core;
using FantaSim.Geosphere.Plate.Topology.Contracts.Events;

namespace FantaSim.Geosphere.Plate.Runtime.Des.Runtime;

public sealed class StandardDesDispatcher : IDesDispatcher
{
    private readonly Dictionary<DesWorkKind, (IAsyncDriver Driver, IExecutableTrigger Trigger)> _registry = new();

    public void Register(DesWorkKind kind, IAsyncDriver driver, IExecutableTrigger trigger)
    {
        _registry[kind] = (driver, trigger);
    }

    public async Task<IReadOnlyList<ITruthEventDraft>> DispatchAsync(
        ScheduledWorkItem item,
        DesContext context,
        CancellationToken ct)
    {
        if (!_registry.TryGetValue(item.Kind, out var handler))
        {
            throw new InvalidOperationException($"No handler registered for DesWorkKind {item.Kind}");
        }

        var (driver, trigger) = handler;

        var output = await driver.EvaluateAsync(context, ct).ConfigureAwait(false);

        return trigger.EmitDrafts(output, context.CurrentTick, context.Rng);
    }
}
