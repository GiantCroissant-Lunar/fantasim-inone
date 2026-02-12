using Crosscut.Config;
using Crosscut.Diagnostics;
using Crosscut.Hosting;
using Crosscut.Logging;
using UnifyStorage.Abstractions;
using UnifyStorage.Runtime.RocksDb;
using FantaSim.App.Bundles.Contracts;
using FantaSim.App.Bundles.Contracts.Interaction.Commands;
using FantaSim.App.Bundles.Contracts.Interaction.Selection;
using Plate.TimeDete.Time;
using Plate.TimeDete.Time.Runtime;
using FantaSim.App.Bundles.Core;
using FantaSim.App.Godot;
using Godot;
using Microsoft.Extensions.Logging;
using ServiceArchi.Contracts;

namespace FantaSim.App;

/// <summary>
/// Builds all application services and registers them in the registry.
/// Extracted from BootstrapShim._Ready() to keep the autoload node thin.
/// </summary>
public sealed class AppServiceBuilder
{
    public sealed record BuildResult(
        IRegistry Registry,
        MessagePipeBundleMessageBus MessageBus,
        BundleHost BundleHost,
        DockManager DockManager,
        MenuService MenuService,
        StatusService StatusService,
        SelectionService SelectionService,
        CommandHistory CommandHistory,
        ILogger Logger,
        RocksDbKeyValueStore? KvStore);

    public BuildResult Build(Node container, Node mainNode)
    {
        var vfs = new BundleVfs();
        var extractor = new DllExtractor();
        var bundleRegistry = new BundleRegistry();
        var registry = bundleRegistry.Registry;
        var messageBus = new MessagePipeBundleMessageBus();

        // Crosscut.Config — JSON file + in-memory defaults
        var configDir = OS.HasFeature("editor")
            ? ProjectSettings.GlobalizePath("res://")
            : OS.GetExecutablePath().GetBaseDir();
        registry.RegisterJsonConfig(System.IO.Path.Combine(configDir, "config.json"), optional: true);
        registry.RegisterMemoryConfig(new Dictionary<string, string>
        {
            ["Storage:RocksDbPath"] = System.IO.Path.Combine(configDir, "data", "rocksdb"),
            ["Logging:MinLevel"] = "Debug",
        });
        registry.RegisterConfigService();

        // Crosscut.Logging with Godot provider
        registry.Register<ILoggingBuilderConfigurator>(new LoggingBuilderConfigurator());
        registry.RegisterLoggingService();
        var logging = new Crosscut.Logging.ServiceProxy(registry);
        var log = logging.CreateLogger("Bootstrap");

        // Crosscut.Diagnostics + Hosting
        registry.RegisterDiagnosticsService();
        registry.RegisterHostingService();

        // RocksDB persistent key-value store
        RocksDbKeyValueStore? kvStore = null;
        var rocksDbPath = System.IO.Path.Combine(configDir, "data", "rocksdb");
        try
        {
            System.IO.Directory.CreateDirectory(rocksDbPath);
            kvStore = new RocksDbKeyValueStore(rocksDbPath, createIfMissing: true);
            registry.Register<IKeyValueStore>(kvStore);
            log.LogInformation("RocksDB opened at {Path}", rocksDbPath);
        }
        catch (System.Exception ex)
        {
            log.LogWarning(ex, "RocksDB unavailable at {Path}, storage features disabled", rocksDbPath);
        }

        // UI services
        var dockManager = new DockManager(logging.CreateLogger("DockManager"));
        var menuService = new MenuService();
        var statusService = new StatusService();

        // Scene host + BundleHost
        var sceneHost = new BundleSceneHost(container, logging.CreateLogger("SceneHost"), dockManager, shellTarget: mainNode);
        var bundleHost = new BundleHost(vfs, extractor, registry, sceneHost, messageBus, bundleRegistry);

        // Interaction services
        var selectionService = new SelectionService(messageBus);
        var commandHistory = new CommandHistory(messageBus);

        // Register all services
        registry.Register<IBundleMessageBus>(messageBus);
        registry.Register<Crosscut.Messaging.IMessageBus>(messageBus);
        registry.Register<IStatusService>(statusService);
        registry.Register<IDockService>(dockManager);
        registry.Register<IMenuService>(menuService);
        registry.Register<ISelectionService>(selectionService);
        registry.Register<ICommandHistory>(commandHistory);

        // Time-dete services: canonical clock
        var clock = new CanonicalClock();
        registry.Register<ICanonicalClock>(clock);

        // Hosted component for graceful bundle shutdown
        registry.RegisterHostedComponent(new BundleHostedComponent(bundleHost), priority: 100);

        return new BuildResult(
            registry, messageBus, bundleHost, dockManager, menuService,
            statusService, selectionService, commandHistory, log, kvStore);
    }
}
