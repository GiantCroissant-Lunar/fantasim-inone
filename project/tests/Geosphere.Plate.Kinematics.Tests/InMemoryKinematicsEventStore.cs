using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using FantaSim.Geosphere.Plate.Kinematics.Contracts.Events;
using FantaSim.Geosphere.Plate.Topology.Contracts.Identity;

namespace FantaSim.Geosphere.Plate.Kinematics.Tests;

/// <summary>
/// Minimal in-memory IKinematicsEventStore for unit testing.
/// </summary>
internal sealed class InMemoryKinematicsEventStore : IKinematicsEventStore
{
    private readonly Dictionary<string, List<IPlateKinematicsEvent>> _streams = new();

    private static string Key(TruthStreamIdentity s) => $"{s.VariantId}:{s.BranchId}:{s.LLevel}:{s.Domain}:{s.Model}";

    public Task AppendAsync(
        TruthStreamIdentity stream,
        IEnumerable<IPlateKinematicsEvent> events,
        CancellationToken cancellationToken)
        => AppendAsync(stream, events, KinematicsAppendOptions.Default, cancellationToken);

    public Task AppendAsync(
        TruthStreamIdentity stream,
        IEnumerable<IPlateKinematicsEvent> events,
        KinematicsAppendOptions options,
        CancellationToken cancellationToken)
    {
        var key = Key(stream);
        if (!_streams.TryGetValue(key, out var list))
        {
            list = new List<IPlateKinematicsEvent>();
            _streams[key] = list;
        }
        list.AddRange(events);
        return Task.CompletedTask;
    }

    public async IAsyncEnumerable<IPlateKinematicsEvent> ReadAsync(
        TruthStreamIdentity stream,
        long fromSequenceInclusive,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        await Task.Yield();
        var key = Key(stream);
        if (!_streams.TryGetValue(key, out var list))
            yield break;

        foreach (var evt in list.Where(e => e.Sequence >= fromSequenceInclusive))
        {
            cancellationToken.ThrowIfCancellationRequested();
            yield return evt;
        }
    }

    public Task<long?> GetLastSequenceAsync(
        TruthStreamIdentity stream,
        CancellationToken cancellationToken)
    {
        var key = Key(stream);
        if (!_streams.TryGetValue(key, out var list) || list.Count == 0)
            return Task.FromResult<long?>(null);

        return Task.FromResult<long?>(list.Max(e => e.Sequence));
    }
}
