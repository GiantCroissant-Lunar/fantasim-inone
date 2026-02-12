using System.Text.Json;
using Plate.TimeDete.Time.Primitives;
using FantaSim.App.Bundles.Contracts;
using FantaSim.App.Bundles.Contracts.Interaction.Events;
using FantaSim.Geosphere.Plate.Topology.Contracts.Capabilities;
using FantaSim.Geosphere.Plate.Topology.Contracts.Derived;
using FantaSim.Geosphere.Plate.Topology.Contracts.Events;
using FantaSim.Geosphere.Plate.Topology.Contracts.Identity;
using ServiceArchi.Contracts;

namespace FantaSim.Bundles.PlateViewer;

/// <summary>
/// Publishes geosphere plate summaries as a HUD data channel.
/// </summary>
public sealed class PlateViewerModule : IBundleServiceModule
{
    public void Register(IRegistry registry)
    {
        var publisher = new GeosphereHudPublisher(registry);
        registry.Register<GeosphereHudPublisher>(publisher);
    }

    public void Deregister(IRegistry registry)
    {
        var publisher = registry.TryGet<GeosphereHudPublisher>();
        if (publisher is null)
        {
            return;
        }

        publisher.Dispose();
        registry.Unregister<GeosphereHudPublisher>(publisher);
    }

    private sealed class GeosphereHudPublisher : IDisposable
    {
        private const string Channel = "geosphere.plates.summary";

        private readonly IRegistry _registry;
        private readonly IBundleMessageBus? _bus;
        private readonly SemaphoreSlim _publishGate = new(1, 1);
        private readonly IDisposable? _headChangedSub;
        private readonly IDisposable? _tickSub;
        private readonly IDisposable? _bundleLoadedSub;
        private readonly IDisposable? _bundleUnloadedSub;

        private TruthStreamIdentity? _currentStream;
        private long? _lastTickHint;

        public GeosphereHudPublisher(IRegistry registry)
        {
            _registry = registry;
            _bus = registry.TryGet<IBundleMessageBus>();
            if (_bus is null)
            {
                return;
            }

            _headChangedSub = _bus.Subscribe<TruthStreamHeadChangedEvent>(OnHeadChangedAsync);
            _tickSub = _bus.Subscribe<TickScrubEvent>(OnTickScrubAsync);
            _bundleLoadedSub = _bus.Subscribe<BundleLoadedEvent>((_, ct) => PublishSummaryAsync(ct));
            _bundleUnloadedSub = _bus.Subscribe<BundleUnloadedEvent>((_, ct) => PublishSummaryAsync(ct));

            _ = PublishSummaryAsync(CancellationToken.None);
        }

        public void Dispose()
        {
            _headChangedSub?.Dispose();
            _tickSub?.Dispose();
            _bundleLoadedSub?.Dispose();
            _bundleUnloadedSub?.Dispose();
            _publishGate.Dispose();
        }

        private ValueTask OnTickScrubAsync(TickScrubEvent evt, CancellationToken cancellationToken)
        {
            _lastTickHint = evt.Tick;
            return PublishSummaryAsync(cancellationToken);
        }

        private ValueTask OnHeadChangedAsync(TruthStreamHeadChangedEvent evt, CancellationToken cancellationToken)
        {
            if (!TruthStreamIdentity.TryParse(evt.StreamIdentity, out var stream) ||
                stream.Domain != Domain.GeoPlatesTopology)
            {
                return ValueTask.CompletedTask;
            }

            _currentStream = stream;
            _lastTickHint = evt.LastTick >= 0 ? evt.LastTick : null;
            return PublishSummaryAsync(cancellationToken);
        }

        private async ValueTask PublishSummaryAsync(CancellationToken cancellationToken)
        {
            if (_bus is null)
            {
                return;
            }

            await _publishGate.WaitAsync(cancellationToken).ConfigureAwait(false);
            try
            {
                var payload = await BuildPayloadAsync(cancellationToken).ConfigureAwait(false);
                var json = JsonSerializer.Serialize(payload);
                _bus.Publish(new HudDataChangedEvent(Channel, json));
            }
            finally
            {
                _publishGate.Release();
            }
        }

        private async Task<GeospherePayload> BuildPayloadAsync(CancellationToken cancellationToken)
        {
            var materializer = _registry.TryGet<IPlateTopologyMaterializationService>();
            var eventStore = _registry.TryGet<ITopologyEventStore>();

            if (materializer is null || eventStore is null)
            {
                return GeospherePayload.Unavailable("Topology services are not loaded.");
            }

            if (_currentStream is null)
            {
                return GeospherePayload.Unavailable("Waiting for a topology stream head update.");
            }

            var stream = _currentStream.Value;
            var head = await eventStore.GetHeadAsync(stream, cancellationToken).ConfigureAwait(false);
            if (head.IsEmpty)
            {
                return GeospherePayload.Empty(stream.ToString());
            }

            var targetTickValue = _lastTickHint ?? head.LastTick;
            if (targetTickValue < 0)
            {
                targetTickValue = 0;
            }

            var targetTick = new CanonicalTick(targetTickValue);
            var state = await materializer.MaterializeAtTickAsync(
                stream,
                targetTick,
                TickMaterializationMode.Auto,
                cancellationToken).ConfigureAwait(false);

            var plates = state.Plates.Values
                .OrderBy(p => p.PlateId.ToString(), StringComparer.Ordinal)
                .Select(p => new PlateRow(p.PlateId.ToString(), p.IsRetired))
                .ToList();

            var activePlates = plates.Count(p => !p.IsRetired);
            var activeBoundaries = state.Boundaries.Values.Count(b => !b.IsRetired);
            var activeJunctions = state.Junctions.Values.Count(j => !j.IsRetired);

            return GeospherePayload.Ready(
                stream.ToString(),
                targetTickValue,
                state.LastEventSequence,
                new Counts(
                    plates.Count,
                    activePlates,
                    state.Boundaries.Count,
                    activeBoundaries,
                    state.Junctions.Count,
                    activeJunctions),
                plates);
        }
    }

    private sealed record Counts(
        int Plates,
        int ActivePlates,
        int Boundaries,
        int ActiveBoundaries,
        int Junctions,
        int ActiveJunctions);

    private sealed record PlateRow(string PlateId, bool IsRetired);

    private sealed record GeospherePayload(
        string Status,
        string Message,
        string? StreamIdentity,
        long Tick,
        long LastSequence,
        Counts Counts,
        IReadOnlyList<PlateRow> Plates)
    {
        public static GeospherePayload Unavailable(string message) =>
            new(
                "unavailable",
                message,
                null,
                -1,
                -1,
                new Counts(0, 0, 0, 0, 0, 0),
                []);

        public static GeospherePayload Empty(string streamIdentity) =>
            new(
                "empty",
                "Stream has no events.",
                streamIdentity,
                -1,
                -1,
                new Counts(0, 0, 0, 0, 0, 0),
                []);

        public static GeospherePayload Ready(
            string streamIdentity,
            long tick,
            long lastSequence,
            Counts counts,
            IReadOnlyList<PlateRow> plates) =>
            new(
                "ready",
                "Topology summary is current.",
                streamIdentity,
                tick,
                lastSequence,
                counts,
                plates);
    }
}
