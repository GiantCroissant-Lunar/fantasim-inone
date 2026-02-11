namespace FantaSim.App.Bundles.Contracts;

/// <summary>
/// Cross-bundle publish/subscribe message bus.
/// Bundles publish messages; any bundle can subscribe.
/// Extends <see cref="Crosscut.Messaging.IMessageBus"/> so crosscut
/// services can resolve the standard messaging interface.
/// </summary>
public interface IBundleMessageBus : Crosscut.Messaging.IMessageBus
{
    IDisposable Subscribe<T>(Func<T, CancellationToken, ValueTask> handler) where T : class;
    int ActiveChannelCount { get; }
}
