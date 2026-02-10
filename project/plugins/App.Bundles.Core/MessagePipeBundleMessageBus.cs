using System.Collections.Concurrent;
using FantaSim.App.Bundles.Contracts;
using MessagePipe;
using Microsoft.Extensions.DependencyInjection;

namespace FantaSim.App.Bundles.Core;

/// <summary>
/// Cross-bundle message bus backed by MessagePipe.
/// Uses a shared ServiceProvider internally; typed channels are created on demand.
/// </summary>
public sealed class MessagePipeBundleMessageBus : IBundleMessageBus, IDisposable
{
    private readonly ServiceProvider _provider;
    private readonly ConcurrentDictionary<Type, object> _publishers = new();
    private readonly ConcurrentDictionary<Type, object> _subscribers = new();

    public MessagePipeBundleMessageBus()
    {
        var services = new ServiceCollection();
        services.AddMessagePipe();
        _provider = services.BuildServiceProvider();
    }

    public void Publish<T>(T message) where T : class
    {
        var publisher = (IPublisher<T>)_publishers.GetOrAdd(
            typeof(T),
            _ => _provider.GetRequiredService<IPublisher<T>>());
        publisher.Publish(message);
    }

    public IDisposable Subscribe<T>(Action<T> handler) where T : class
    {
        var subscriber = (ISubscriber<T>)_subscribers.GetOrAdd(
            typeof(T),
            _ => _provider.GetRequiredService<ISubscriber<T>>());
        return subscriber.Subscribe(handler);
    }

    public IDisposable Subscribe<T>(Func<T, CancellationToken, ValueTask> handler) where T : class
    {
        var subscriber = (ISubscriber<T>)_subscribers.GetOrAdd(
            typeof(T),
            _ => _provider.GetRequiredService<ISubscriber<T>>());
        return subscriber.Subscribe(new AsyncHandlerAdapter<T>(handler));
    }

    public int ActiveChannelCount => _subscribers.Count;

    public void Dispose()
    {
        _provider.Dispose();
    }

    private sealed class AsyncHandlerAdapter<T>(Func<T, CancellationToken, ValueTask> handler)
        : IMessageHandler<T>
    {
        public void Handle(T message)
        {
            handler(message, CancellationToken.None).AsTask().GetAwaiter().GetResult();
        }
    }
}
