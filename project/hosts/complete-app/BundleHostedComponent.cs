using Crosscut.Hosting;
using FantaSim.App.Bundles.Core;

namespace FantaSim.App;

/// <summary>
/// Wraps BundleHost shutdown in the IHostedComponent lifecycle so that
/// StopAsync gracefully unloads all bundles before the process exits.
/// </summary>
public sealed class BundleHostedComponent : IHostedComponent
{
    private readonly BundleHost _bundleHost;

    public BundleHostedComponent(BundleHost bundleHost)
    {
        _bundleHost = bundleHost;
    }

    public int Priority => 100;

    public Task StartAsync(CancellationToken ct = default) => Task.CompletedTask;

    public async Task StopAsync(CancellationToken ct = default)
    {
        await _bundleHost.UnloadAllAsync(ct);
    }
}
