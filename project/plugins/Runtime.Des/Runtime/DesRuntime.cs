using System;
using System.Threading;
using System.Threading.Tasks;
using FantaSim.Geosphere.Plate.Runtime.Des.Events;
using FantaSim.Geosphere.Plate.Runtime.Des.Core;
using FantaSim.Geosphere.Plate.Runtime.Des.Contracts;
using FantaSim.Geosphere.Plate.Topology.Materializer;
using FantaSim.Geosphere.Plate.Topology.Contracts.Identity;
using FantaSim.Geosphere.Plate.Topology.Contracts.Determinism;
using Plate.TimeDete.Time.Primitives;
using FantaSim.Geosphere.Plate.Topology.Contracts.Capabilities;
using FantaSim.Geosphere.Plate.Runtime.Des.Contracts.Observations;

namespace FantaSim.Geosphere.Plate.Runtime.Des.Runtime;

public sealed class DesRuntime : IDesRuntime
{
    private readonly IDesQueue _queue;
    private readonly ITruthEventAppender _appender;
    private readonly PlateTopologyTimeline _timeline;
    private readonly IDesDispatcher _dispatcher;
    private readonly IDesScheduler _scheduler;
    private readonly ISolverSeedProvider _seedProvider;
    private readonly IObservationArtifactEmitter _emitter;

    public DesRuntime(
        IDesQueue queue,
        ITruthEventAppender appender,
        PlateTopologyTimeline timeline,
        IDesDispatcher dispatcher,
        ISolverSeedProvider seedProvider,
        IObservationArtifactEmitter emitter)
    {
        _queue = queue ?? throw new ArgumentNullException(nameof(queue));
        _appender = appender ?? throw new ArgumentNullException(nameof(appender));
        _timeline = timeline ?? throw new ArgumentNullException(nameof(timeline));
        _dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));
        _seedProvider = seedProvider ?? throw new ArgumentNullException(nameof(seedProvider));
        _emitter = emitter ?? throw new ArgumentNullException(nameof(emitter));
        _scheduler = new DesScheduler(_queue);
    }

    public async Task<DesRunResult> RunAsync(
        TruthStreamIdentity stream,
        DesRunOptions options,
        CancellationToken ct = default)
    {
        int itemsProcessed = 0;
        int eventsAppended = 0;

        while (itemsProcessed < options.MaxItemsProcessed && eventsAppended < options.MaxEventsAppended)
        {
            ct.ThrowIfCancellationRequested();

            if (!_queue.TryPeek(out var nextItem))
            {
                break;
            }

            // Check end tick
            if (options.EndTick.HasValue && nextItem.When > options.EndTick.Value)
            {
                break;
            }

            // Dequeue
            _queue.TryDequeue(out var item);

            // Materialize state at current tick
            var slice = await _timeline.GetSliceAtTickAsync(stream, item.When, TickMaterializationMode.Auto, ct).ConfigureAwait(false);

            // Create tick-scoped RNG for deterministic event ID generation
            var tickRng = _seedProvider.CreateRngForTick(options.ScenarioSeed, stream, item.When);

            var context = new DesContext
            {
                Stream = stream,
                CurrentTick = item.When,
                State = slice.State,
                Scheduler = _scheduler,
                Rng = tickRng
            };

            // Dispatch
            var drafts = await _dispatcher.DispatchAsync(item, context, ct).ConfigureAwait(false);

            // Append drafts
            if (drafts.Count > 0)
            {
                await _appender.AppendAsync(drafts, new AppendOptions { EnforceMonotonicity = true }, ct).ConfigureAwait(false);

                eventsAppended += drafts.Count;

                // Emit observation artifact
                await _emitter.EmitAsync(stream, item.When, drafts, ct).ConfigureAwait(false);
            }

            itemsProcessed++;
        }

        return new DesRunResult(itemsProcessed, eventsAppended);
    }
}
