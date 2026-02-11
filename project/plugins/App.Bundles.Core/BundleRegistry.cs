using System.Collections.Concurrent;
using ServiceArchi.Contracts;
using ServiceArchi.Core;

namespace FantaSim.App.Bundles.Core;

/// <summary>
/// Thin wrapper around ServiceArchi's <see cref="ServiceRegistry"/> that
/// tracks registered type names for diagnostic snapshots.
/// </summary>
public sealed class BundleRegistry
{
    private readonly ServiceRegistry _inner = new();
    private readonly ConcurrentDictionary<Type, int> _typeCounts = new();

    public IRegistry Registry => _inner;

    public void Register<T>(T instance) where T : class
    {
        _inner.Register(instance);
        _typeCounts.AddOrUpdate(typeof(T), 1, (_, c) => c + 1);
    }

    public void UnregisterAll<T>() where T : class
    {
        _inner.UnregisterAll<T>();
        _typeCounts.TryRemove(typeof(T), out _);
    }

    public IReadOnlyList<string> RegisteredTypeNames =>
        _typeCounts.Keys.Select(t => t.FullName ?? t.Name).ToList();
}
