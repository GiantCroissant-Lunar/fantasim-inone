using Fantasim.App.Bundles.Core;
using Godot;

namespace Fantasim.App;

/// <summary>
/// Godot autoload node. Creates BundleHost in _Ready(), calls UnloadAllAsync() in _ExitTree().
/// </summary>
public partial class BootstrapShim : Node
{
    private BundleHost? _bundleHost;

    public override void _Ready()
    {
        var vfs = new GodotBundleVfs();
        var extractor = new DllExtractor();
        var registry = new BundleServiceRegistry();
        _bundleHost = new BundleHost(vfs, extractor, registry);

        GD.Print("[Bootstrap] BundleHost created");
    }

    public override void _ExitTree()
    {
        if (_bundleHost is not null)
        {
            _bundleHost.UnloadAllAsync().GetAwaiter().GetResult();
            GD.Print("[Bootstrap] BundleHost shut down");
        }
    }
}
