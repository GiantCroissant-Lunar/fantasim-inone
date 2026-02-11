using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FantaSim.Geosphere.Plate.Topology.Contracts.Events;
using FantaSim.Geosphere.Plate.Topology.Contracts.Identity;

namespace FantaSim.Geosphere.Plate.Runtime.Des.Events;

/// <summary>
/// Appends truth events to the topology event store with optimistic concurrency control.
/// </summary>
public sealed class PlateTopologyEventAppender : ITruthEventAppender
{
    private readonly ITopologyEventStore _store;

    public PlateTopologyEventAppender(ITopologyEventStore store)
    {
        _store = store ?? throw new ArgumentNullException(nameof(store));
    }

    public async Task<AppendDraftsResult> AppendAsync(
        IReadOnlyList<ITruthEventDraft> drafts,
        AppendOptions options,
        CancellationToken ct = default)
    {
        if (drafts.Count == 0)
        {
            return new AppendDraftsResult(0, ReadOnlyMemory<byte>.Empty);
        }

        var stream = drafts[0].Stream;

        // 1. Get the current head state for optimistic concurrency
        var head = await _store.GetHeadAsync(stream, ct).ConfigureAwait(false);
        long nextSeq = head.Sequence + 1;

        var eventsToAppend = new List<IPlateTopologyEvent>(drafts.Count);

        foreach (var draft in drafts)
        {
            if (draft.Stream != stream)
            {
                throw new ArgumentException("All drafts must belong to the same stream", nameof(drafts));
            }
            eventsToAppend.Add(draft.ToTruthEvent(nextSeq++));
        }

        // 2. Append with optimistic concurrency precondition
        var storeOptions = new FantaSim.Geosphere.Plate.Topology.Contracts.Events.AppendOptions
        {
            TickPolicy = options.EnforceMonotonicity
                ? TickMonotonicityPolicy.Reject
                : TickMonotonicityPolicy.Allow,
            ExpectedHead = head.ToPrecondition()
        };

        await _store.AppendAsync(stream, eventsToAppend, storeOptions, ct).ConfigureAwait(false);

        // 3. Return result from the new head
        var newHead = await _store.GetHeadAsync(stream, ct).ConfigureAwait(false);

        return new AppendDraftsResult(newHead.Sequence, newHead.Hash);
    }
}
