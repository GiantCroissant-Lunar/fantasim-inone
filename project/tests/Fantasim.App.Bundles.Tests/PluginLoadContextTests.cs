using Fantasim.App.Bundles.Core;
using FluentAssertions;
using Xunit;

namespace Fantasim.App.Bundles.Tests;

public class PluginLoadContextTests
{
    [Fact]
    public void Host_assembly_names_delegate_to_default_ALC()
    {
        // PluginLoadContext with host assembly names should be collectible
        var hostAssemblyNames = new[] { "Fantasim.App.Bundles.Contracts", "System.Runtime" };
        var tempDll = typeof(PluginLoadContext).Assembly.Location;

        var alc = new PluginLoadContext(tempDll, hostAssemblyNames);
        alc.IsCollectible.Should().BeTrue();

        // Unload to verify collectibility
        alc.Unload();
    }

    [Fact]
    public void IsCollectible_is_true()
    {
        var tempDll = typeof(PluginLoadContext).Assembly.Location;
        var alc = new PluginLoadContext(tempDll, Array.Empty<string>());

        alc.IsCollectible.Should().BeTrue();

        alc.Unload();
    }
}
