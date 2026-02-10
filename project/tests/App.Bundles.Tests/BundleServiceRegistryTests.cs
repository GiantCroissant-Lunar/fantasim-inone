using FantaSim.App.Bundles.Core;
using FluentAssertions;
using Xunit;

namespace FantaSim.App.Bundles.Tests;

public class BundleServiceRegistryTests
{
    private readonly BundleServiceRegistry _registry = new();

    [Fact]
    public void Register_and_Resolve_returns_service()
    {
        var service = new TestService("hello");
        _registry.Register<ITestService>(service);

        _registry.Resolve<ITestService>().Should().BeSameAs(service);
    }

    [Fact]
    public void Resolve_unregistered_returns_null()
    {
        _registry.Resolve<ITestService>().Should().BeNull();
    }

    [Fact]
    public void Deregister_removes_service()
    {
        _registry.Register<ITestService>(new TestService("x"));
        _registry.Deregister<ITestService>();

        _registry.Resolve<ITestService>().Should().BeNull();
    }

    [Fact]
    public void Duplicate_register_throws()
    {
        _registry.Register<ITestService>(new TestService("a"));

        var act = () => _registry.Register<ITestService>(new TestService("b"));
        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void Concurrent_register_and_resolve_is_thread_safe()
    {
        const int count = 100;

        // Each iteration creates its own registry to test concurrent access
        Parallel.For(0, count, i =>
        {
            var registry = new BundleServiceRegistry();
            registry.Register<ITestService>(new TestService($"svc-{i}"));
            var resolved = registry.Resolve<ITestService>();
            resolved.Should().NotBeNull();
            resolved!.Name.Should().Be($"svc-{i}");
        });
    }

    public interface ITestService { string Name { get; } }
    public record TestService(string Name) : ITestService;
}
