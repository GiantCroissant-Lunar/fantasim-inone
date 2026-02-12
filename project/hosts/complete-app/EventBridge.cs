using FantaSim.App.Bundles.Contracts;
using FantaSim.App.Bundles.Contracts.Interaction.Events;
using Godot;

namespace FantaSim.App;

/// <summary>
/// Bridges C# message bus events to Godot signals so GDScript bundles
/// can observe lifecycle and HUD events via standard Godot signal connections.
/// </summary>
public sealed class EventBridge : IDisposable
{
    private readonly List<IDisposable> _subscriptions = new();

    public EventBridge(IBundleMessageBus bus, Node signalSource)
    {
        _subscriptions.Add(bus.Subscribe<BundleLoadedEvent>(e =>
            signalSource.CallDeferred("emit_signal", BootstrapShim.SignalName.BundleChanged, e.BundleId)));
        _subscriptions.Add(bus.Subscribe<BundleUnloadedEvent>(e =>
            signalSource.CallDeferred("emit_signal", BootstrapShim.SignalName.BundleChanged, e.BundleId)));
        _subscriptions.Add(bus.Subscribe<TickScrubEvent>(e =>
            signalSource.CallDeferred("emit_signal", BootstrapShim.SignalName.TickScrubbed, e.Tick, e.IsScrubbing)));
        _subscriptions.Add(bus.Subscribe<TruthStreamHeadChangedEvent>(e =>
            signalSource.CallDeferred("emit_signal", BootstrapShim.SignalName.TruthStreamHeadChanged, e.StreamIdentity, e.Sequence, e.LastTick)));
        _subscriptions.Add(bus.Subscribe<HudDataChangedEvent>(e =>
            signalSource.CallDeferred("emit_signal", BootstrapShim.SignalName.HudDataChanged, e.Channel, e.PayloadJson)));
    }

    public void Dispose()
    {
        foreach (var s in _subscriptions)
            s.Dispose();
    }
}
