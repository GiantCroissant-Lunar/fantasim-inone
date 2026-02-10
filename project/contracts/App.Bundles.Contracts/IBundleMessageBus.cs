namespace Fantasim.App.Bundles.Contracts;

/// <summary>
/// Cross-bundle publish/subscribe message bus.
/// Bundles publish messages; any bundle can subscribe.
/// </summary>
public interface IBundleMessageBus
{
    void Publish<T>(T message) where T : class;
    IDisposable Subscribe<T>(Action<T> handler) where T : class;
    IDisposable Subscribe<T>(Func<T, CancellationToken, ValueTask> handler) where T : class;
    int ActiveChannelCount { get; }
}
