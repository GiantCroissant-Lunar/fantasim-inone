using Crosscut.Config;
using Crosscut.Diagnostics;
using Crosscut.Hosting;
using Crosscut.Logging;
using UnifyStorage.Abstractions;
using UnifyStorage.Runtime.RocksDb;
using FantaSim.App.Bundles.Contracts;
using FantaSim.App.Bundles.Contracts.Interaction.Commands;
using FantaSim.App.Bundles.Contracts.Interaction.Events;
using FantaSim.App.Bundles.Contracts.Interaction.Selection;
using FantaSim.App.Bundles.Core;
using FantaSim.App.Godot;
using Godot;
using Microsoft.Extensions.Logging;
using ServiceArchi.Contracts;

namespace FantaSim.App;

/// <summary>
/// Godot autoload node. Creates BundleHost + services in _Ready(),
/// exposes methods callable from GDScript bundles.
/// Shell bundle wires dock slots via RegisterDockSlot/SetMenuBar/etc.
/// </summary>
public partial class BootstrapShim : Node
{
    private BundleHost? _bundleHost;
    private MessagePipeBundleMessageBus? _messageBus;
    private RocksDbKeyValueStore? _kvStore;
    private ILogger? _log;
    private DockManager? _dockManager;
    private MenuService? _menuService;
    private StatusService? _statusService;
    private VerificationService? _verificationService;
    private SelectionService? _selectionService;
    private CommandHistory? _commandHistory;
    private CommandRouter? _commandRouter;
    private IDisposable? _bundleLoadedSub;
    private IDisposable? _bundleUnloadedSub;
    private IDisposable? _tickScrubSub;
    private IDisposable? _truthHeadChangedSub;
    private IDisposable? _hudDataChangedSub;

    public IBundleHost? BundleHost => _bundleHost;
    public IRegistry? Registry { get; private set; }
    public DockManager? DockManager => _dockManager;
    public MenuService? MenuService => _menuService;
    public StatusService? StatusService => _statusService;

    [Signal]
    public delegate void ShellReadyEventHandler();

    [Signal]
    public delegate void AllBundlesLoadedEventHandler();

    [Signal]
    public delegate void BundleChangedEventHandler(string bundleId);

    [Signal]
    public delegate void TickScrubbedEventHandler(long tick, bool isScrubbing);

    [Signal]
    public delegate void TruthStreamHeadChangedEventHandler(string streamIdentity, long sequence, long lastTick);

    [Signal]
    public delegate void HudDataChangedEventHandler(string channel, string payloadJson);

    public override void _Ready()
    {
        var vfs = new BundleVfs();
        var extractor = new DllExtractor();
        var bundleRegistry = new BundleRegistry();
        var registry = bundleRegistry.Registry;
        _messageBus = new MessagePipeBundleMessageBus();

        // Set up Crosscut.Config — JSON file + in-memory defaults
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

        // Set up Crosscut.Logging with Godot provider
        registry.Register<ILoggingBuilderConfigurator>(new LoggingBuilderConfigurator());
        registry.RegisterLoggingService();
        var logging = new Crosscut.Logging.ServiceProxy(registry);

        _log = logging.CreateLogger("Bootstrap");

        // Set up Crosscut.Diagnostics (null tracer/metrics, health checks)
        registry.RegisterDiagnosticsService();

        // Set up Crosscut.Hosting for lifecycle management
        registry.RegisterHostingService();

        // Set up RocksDB persistent key-value store
        var rocksDbPath = System.IO.Path.Combine(configDir, "data", "rocksdb");
        try
        {
            System.IO.Directory.CreateDirectory(rocksDbPath);
            _kvStore = new RocksDbKeyValueStore(rocksDbPath, createIfMissing: true);
            registry.Register<IKeyValueStore>(_kvStore);
            _log?.LogInformation("RocksDB opened at {Path}", rocksDbPath);
        }
        catch (System.Exception ex)
        {
            _log?.LogWarning(ex, "RocksDB unavailable at {Path}, storage features disabled", rocksDbPath);
        }

        // Create services
        _dockManager = new DockManager(logging.CreateLogger("DockManager"));
        _menuService = new MenuService();
        _statusService = new StatusService();

        var mainNode = GetTree().Root.GetNode("Main");
        var sceneHost = new BundleSceneHost(this, logging.CreateLogger("SceneHost"), _dockManager, shellTarget: mainNode);

        _bundleHost = new BundleHost(vfs, extractor, registry, sceneHost, _messageBus, bundleRegistry);
        Registry = registry;

        // Interaction services
        _selectionService = new SelectionService(_messageBus);
        _commandHistory = new CommandHistory(_messageBus);

        registry.Register<IBundleMessageBus>(_messageBus);
        registry.Register<Crosscut.Messaging.IMessageBus>(_messageBus);
        registry.Register<IStatusService>(_statusService);
        registry.Register<IDockService>(_dockManager);
        registry.Register<IMenuService>(_menuService);
        registry.Register<ISelectionService>(_selectionService);
        registry.Register<ICommandHistory>(_commandHistory);

        // Command router (subscribes to GdScriptCommand on the bus)
        _commandRouter = new CommandRouter(_messageBus, logging.CreateLogger("CommandRouter"), _selectionService, _bundleHost);

        // Bridge bundle lifecycle events to Godot signals for GDScript consumers
        _bundleLoadedSub = _messageBus.Subscribe<BundleLoadedEvent>(e =>
            CallDeferred("emit_signal", SignalName.BundleChanged, e.BundleId));
        _bundleUnloadedSub = _messageBus.Subscribe<BundleUnloadedEvent>(e =>
            CallDeferred("emit_signal", SignalName.BundleChanged, e.BundleId));
        _tickScrubSub = _messageBus.Subscribe<TickScrubEvent>(e =>
            CallDeferred("emit_signal", SignalName.TickScrubbed, e.Tick, e.IsScrubbing));
        _truthHeadChangedSub = _messageBus.Subscribe<TruthStreamHeadChangedEvent>(e =>
            CallDeferred("emit_signal", SignalName.TruthStreamHeadChanged, e.StreamIdentity, e.Sequence, e.LastTick));
        _hudDataChangedSub = _messageBus.Subscribe<HudDataChangedEvent>(e =>
            CallDeferred("emit_signal", SignalName.HudDataChanged, e.Channel, e.PayloadJson));

        _log?.LogInformation("BundleHost created");

        // Start verification service if --verify flag is present
        var args = OS.GetCmdlineUserArgs();
        if (args.Contains("--verify"))
        {
            _verificationService = new VerificationService(this, logging.CreateLogger("Verify"));
            _verificationService.Start();
        }

        // Defer autoload to ensure Main scene is fully ready
        CallDeferred(MethodName.AutoLoadSystemBundles);
    }

    public override void _Process(double delta)
    {
        _verificationService?.ProcessFrame();
    }

    public override void _ExitTree()
    {
        _bundleLoadedSub?.Dispose();
        _bundleUnloadedSub?.Dispose();
        _tickScrubSub?.Dispose();
        _truthHeadChangedSub?.Dispose();
        _hudDataChangedSub?.Dispose();
        _commandRouter?.Dispose();

        if (_bundleHost is not null)
        {
            _bundleHost.UnloadAllAsync().GetAwaiter().GetResult();
            _log?.LogInformation("BundleHost shut down");
        }

        _messageBus?.Dispose();
        _kvStore?.Dispose();
    }

    // -- GDScript-callable service wiring (called by shell bundle) --

    public void RegisterDockSlot(string name, TabContainer container)
    {
        _dockManager?.RegisterSlot(name, container);
        _log?.LogInformation("Dock slot registered: {Name}", name);
    }

    public void SetMenuBar(MenuBar menuBar)
    {
        _menuService?.SetMenuBar(menuBar);
    }

    public void SetStatusLabel(Label label)
    {
        _statusService?.SetStatusLabel(label);
    }

    public void SetFileDialog(FileDialog dialog)
    {
        _statusService?.SetFileDialog(dialog);
    }

    // -- GDScript-callable command entry point --

    public void SendCommand(string action, global::Godot.Collections.Dictionary? @params = null)
    {
        if (_messageBus is null) return;

        var dict = new Dictionary<string, object?>();
        if (@params is not null)
        {
            foreach (var kv in @params)
            {
                if (kv.Key.Obj is string key)
                    dict[key] = kv.Value.Obj;
            }
        }

        var cmd = new GdScriptCommand(Guid.NewGuid(), action, dict);
        _messageBus.Publish(cmd);
    }

    // -- GDScript-callable methods --

    public void DockPanel(string bundleId, string slotName, Control panel, string tabTitle)
    {
        _dockManager?.DockPanel(bundleId, slotName, panel, tabTitle);
    }

    public PopupMenu? GetOrAddMenu(string menuTitle)
    {
        return _menuService?.GetOrAddMenu(menuTitle);
    }

    public void ShowStatus(string text)
    {
        _statusService?.ShowStatus(text);
    }

    public void ShowFileDialog(Callable onSelected)
    {
        _statusService?.ShowFileDialog(path => onSelected.Call(path));
    }

    public async void LoadBundle(string pckPath)
    {
        await ExecuteBundleHostOperationAsync(
            $"Loading {pckPath}...",
            "Bundle loaded",
            "Load failed",
            host => host.LoadAsync(pckPath));
    }

    public async void UnloadBundle(string bundleId)
    {
        await ExecuteBundleHostOperationAsync(
            $"Unloading {bundleId}...",
            "Bundle unloaded",
            "Unload failed",
            host => host.UnloadAsync(bundleId));
    }

    public async void ReloadBundle(string bundleId)
    {
        await ExecuteBundleHostOperationAsync(
            $"Reloading {bundleId}...",
            "Bundle reloaded",
            "Reload failed",
            host => host.ReloadAsync(bundleId));
    }

    public global::Godot.Collections.Dictionary CaptureSnapshotDict()
    {
        if (_bundleHost is null) return [];

        var snapshot = _bundleHost.CaptureSnapshot();
        var result = new global::Godot.Collections.Dictionary();

        var bundlesArray = new global::Godot.Collections.Array();
        foreach (var bundle in snapshot.Bundles)
        {
            var b = new global::Godot.Collections.Dictionary
            {
                ["id"] = bundle.Id,
                ["status"] = bundle.Status.ToString(),
                ["version"] = bundle.Manifest.Version,
                ["displayName"] = bundle.Manifest.DisplayName,
                ["loadedAt"] = bundle.LoadedAt.ToString("HH:mm:ss.fff"),
                ["hasALC"] = bundle.HasAssemblyLoadContext,
            };

            var nodesArray = new global::Godot.Collections.Array();
            foreach (var node in bundle.TrackedSceneNodes)
            {
                nodesArray.Add(node);
            }
            b["trackedNodes"] = nodesArray;

            bundlesArray.Add(b);
        }

        result["capturedAt"] = snapshot.CapturedAt.ToString("HH:mm:ss.fff");
        result["bundles"] = bundlesArray;

        var servicesArray = new global::Godot.Collections.Array();
        foreach (var t in snapshot.RegisteredServiceTypes)
        {
            servicesArray.Add(t);
        }
        result["services"] = servicesArray;
        result["messageBusChannels"] = snapshot.MessageBus?.ActiveChannelCount ?? 0;

        return result;
    }

    // -- Two-phase autoload --

    public async void AutoLoadSystemBundles()
    {
        if (_bundleHost is null) return;

        var directory = OS.HasFeature("editor")
            ? ProjectSettings.GlobalizePath("res://system_bundles")
            : OS.GetExecutablePath().GetBaseDir().PathJoin("bundles");
        var dir = DirAccess.Open(directory);
        if (dir is null)
        {
            _log?.LogInformation("No system bundles directory at {Directory}", directory);
            return;
        }

        // Discover all .pck files, separating shell from control bundles
        var shellPcks = new System.Collections.Generic.List<string>();
        var controlPcks = new System.Collections.Generic.List<string>();

        dir.ListDirBegin();
        var fileName = dir.GetNext();
        while (!string.IsNullOrEmpty(fileName))
        {
            if (fileName.EndsWith(".pck"))
            {
                var path = $"{directory}/{fileName}";
                if (fileName.Contains("shell"))
                    shellPcks.Add(path);
                else
                    controlPcks.Add(path);
            }
            fileName = dir.GetNext();
        }

        // Phase 1: Load shell bundle(s) first
        await LoadPcksAsync(shellPcks, "Shell loaded: {File}", "Failed to load shell {File}");

        // Wait one frame for shell's _ready() to wire dock slots
        await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
        EmitSignal(SignalName.ShellReady);

        // Phase 2: Load remaining control bundles
        await LoadPcksAsync(controlPcks, "Auto-loaded: {File}", "Failed to auto-load {File}");

        _log?.LogInformation("All bundles loaded");
        EmitSignal(SignalName.AllBundlesLoaded);
    }

    private async Task ExecuteBundleHostOperationAsync(
        string inProgressStatus,
        string successStatus,
        string errorLogMessage,
        Func<BundleHost, Task> operation)
    {
        if (_bundleHost is null)
            return;

        try
        {
            _statusService?.ShowStatus(inProgressStatus);
            await operation(_bundleHost);
            _statusService?.ShowStatus(successStatus);
        }
        catch (System.Exception ex)
        {
            _statusService?.ShowStatus($"Error: {ex.Message}");
            _log?.LogError(ex, errorLogMessage);
        }
    }

    private async Task LoadPcksAsync(
        IEnumerable<string> pcks,
        string successMessageTemplate,
        string errorMessageTemplate)
    {
        if (_bundleHost is null)
            return;

        foreach (var pck in pcks)
        {
            try
            {
                await _bundleHost.LoadAsync(pck);
                _log?.LogInformation(successMessageTemplate, pck.GetFile());
            }
            catch (System.Exception ex)
            {
                _log?.LogError(ex, errorMessageTemplate, pck.GetFile());
            }
        }
    }
}
