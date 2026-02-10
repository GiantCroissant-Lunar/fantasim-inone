using Fantasim.App.Bundles.Contracts;
using Fantasim.App.Bundles.Core;
using Godot;

namespace Fantasim.App;

/// <summary>
/// Godot autoload node. Creates BundleHost in _Ready(), calls UnloadAllAsync() in _ExitTree().
/// </summary>
public partial class BootstrapShim : Node
{
    private BundleHost? _bundleHost;
    private MessagePipeBundleMessageBus? _messageBus;

    public IBundleHost? BundleHost => _bundleHost;
    public IBundleServiceRegistry? Registry { get; private set; }

    public override void _Ready()
    {
        var vfs = new GodotBundleVfs();
        var extractor = new DllExtractor();
        var registry = new BundleServiceRegistry();
        var sceneHost = new GodotBundleSceneHost(this);
        _messageBus = new MessagePipeBundleMessageBus();

        _bundleHost = new BundleHost(vfs, extractor, registry, sceneHost, _messageBus);
        Registry = registry;

        registry.Register<IBundleMessageBus>(_messageBus);

        GD.Print("[Bootstrap] BundleHost created (scene host + message bus wired)");
    }

    public override void _ExitTree()
    {
        if (_bundleHost is not null)
        {
            _bundleHost.UnloadAllAsync().GetAwaiter().GetResult();
            GD.Print("[Bootstrap] BundleHost shut down");
        }

        _messageBus?.Dispose();
    }
}
