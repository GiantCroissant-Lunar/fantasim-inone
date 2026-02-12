using FantaSim.App.Bundles.Contracts;
using FantaSim.App.Bundles.Contracts.Interaction.Events;
using FantaSim.Geosphere.Plate.Topology.Contracts.Events;
using FantaSim.Geosphere.Plate.Topology.Contracts.Identity;

namespace FantaSim.Geosphere.Plate.Topology.Materializer;

/// <summary>
/// Decorates an <see cref="ITopologyEventStore"/> and publishes stream-head changes.
/// </summary>
public sealed class NotifyingTopologyEventStore : ITopologyEventStore
{
    private readonly ITopologyEventStore _inner;
    private readonly IBundleMessageBus _bus;

    public NotifyingTopologyEventStore(ITopologyEventStore inner, IBundleMessageBus bus)
    {
        _inner = inner;
        _bus = bus;
    }

    public ITopologyEventStore Inner => _inner;

    public async Task AppendAsync(
        TruthStreamIdentity stream,
        IEnumerable<IPlateTopologyEvent> events,
        CancellationToken cancellationToken)
    {
        await _inner.AppendAsync(stream, events, cancellationToken).ConfigureAwait(false);
        await PublishHeadChangedAsync(stream, cancellationToken).ConfigureAwait(false);
    }

    public async Task AppendAsync(
        TruthStreamIdentity stream,
        IEnumerable<IPlateTopologyEvent> events,
        AppendOptions options,
        CancellationToken cancellationToken)
    {
        await _inner.AppendAsync(stream, events, options, cancellationToken).ConfigureAwait(false);
        await PublishHeadChangedAsync(stream, cancellationToken).ConfigureAwait(false);
    }

    public IAsyncEnumerable<IPlateTopologyEvent> ReadAsync(
        TruthStreamIdentity stream,
        long fromSequenceInclusive,
        CancellationToken cancellationToken)
    {
        return _inner.ReadAsync(stream, fromSequenceInclusive, cancellationToken);
    }

    public Task<long?> GetLastSequenceAsync(
        TruthStreamIdentity stream,
        CancellationToken cancellationToken)
    {
        return _inner.GetLastSequenceAsync(stream, cancellationToken);
    }

    public Task<StreamHead> GetHeadAsync(
        TruthStreamIdentity stream,
        CancellationToken cancellationToken)
    {
        return _inner.GetHeadAsync(stream, cancellationToken);
    }

    private async Task PublishHeadChangedAsync(TruthStreamIdentity stream, CancellationToken cancellationToken)
    {
        var head = await _inner.GetHeadAsync(stream, cancellationToken).ConfigureAwait(false);
        _bus.Publish(new TruthStreamHeadChangedEvent(
            stream.ToString(),
            head.Sequence,
            head.LastTick));
    }
}
