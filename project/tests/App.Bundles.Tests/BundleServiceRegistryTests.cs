using FantaSim.App.Bundles.Core;
using FluentAssertions;
using ServiceArchi.Contracts;
using Xunit;

namespace FantaSim.App.Bundles.Tests;

public class BundleRegistryTests
{
    private readonly BundleRegistry _bundleRegistry = new();
    private IRegistry Registry => _bundleRegistry.Registry;

    [Fact]
    public void Register_and_TryGet_returns_service()
    {
        var service = new TestService("hello");
        _bundleRegistry.Register<ITestService>(service);

        Registry.TryGet<ITestService>().Should().BeSameAs(service);
    }

    [Fact]
    public void TryGet_unregistered_returns_null()
    {
        Registry.TryGet<ITestService>().Should().BeNull();
    }

    [Fact]
    public void UnregisterAll_removes_service()
    {
        _bundleRegistry.Register<ITestService>(new TestService("x"));
        _bundleRegistry.UnregisterAll<ITestService>();

        Registry.TryGet<ITestService>().Should().BeNull();
    }

    [Fact]
    public void Multiple_registrations_allowed()
    {
        _bundleRegistry.Register<ITestService>(new TestService("a"));
        _bundleRegistry.Register<ITestService>(new TestService("b"));

        Registry.GetAll<ITestService>().Should().HaveCount(2);
    }

    [Fact]
    public void RegisteredTypeNames_tracks_registered_types()
    {
        _bundleRegistry.Register<ITestService>(new TestService("a"));

        _bundleRegistry.RegisteredTypeNames.Should().Contain(typeof(ITestService).FullName!);
    }

    [Fact]
    public void RegisteredTypeNames_cleared_after_UnregisterAll()
    {
        _bundleRegistry.Register<ITestService>(new TestService("a"));
        _bundleRegistry.UnregisterAll<ITestService>();

        _bundleRegistry.RegisteredTypeNames.Should().NotContain(typeof(ITestService).FullName!);
    }

    [Fact]
    public void Concurrent_register_and_resolve_is_thread_safe()
    {
        const int count = 100;

        Parallel.For(0, count, i =>
        {
            var reg = new BundleRegistry();
            reg.Register<ITestService>(new TestService($"svc-{i}"));
            var resolved = reg.Registry.TryGet<ITestService>();
            resolved.Should().NotBeNull();
            resolved!.Name.Should().Be($"svc-{i}");
        });
    }

    public interface ITestService { string Name { get; } }
    public record TestService(string Name) : ITestService;
}
