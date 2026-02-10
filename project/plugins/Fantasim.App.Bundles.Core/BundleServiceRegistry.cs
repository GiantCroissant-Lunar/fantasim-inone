using System.Collections.Concurrent;
using Fantasim.App.Bundles.Contracts;

namespace Fantasim.App.Bundles.Core;

/// <summary>
/// Thread-safe typed service registry backed by ConcurrentDictionary.
/// </summary>
public sealed class BundleServiceRegistry : IBundleServiceRegistry
{
    private readonly ConcurrentDictionary<Type, object> _services = new();

    public void Register<T>(T service) where T : class
    {
        ArgumentNullException.ThrowIfNull(service);
        if (!_services.TryAdd(typeof(T), service))
        {
            throw new InvalidOperationException($"Service of type {typeof(T).Name} is already registered.");
        }
    }

    public void Deregister<T>() where T : class
    {
        _services.TryRemove(typeof(T), out _);
    }

    public T? Resolve<T>() where T : class
    {
        return _services.TryGetValue(typeof(T), out var service) ? (T)service : null;
    }

    public IReadOnlyList<string> RegisteredTypeNames =>
        _services.Keys.Select(t => t.FullName ?? t.Name).ToList();
}
