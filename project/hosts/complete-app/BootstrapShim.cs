using FantaSim.App.Bundles.Contracts;
using FantaSim.App.Bundles.Contracts.Interaction.Commands;
using FantaSim.App.Bundles.Core;
using FantaSim.App.Godot;
using Godot;
using Microsoft.Extensions.Logging;
using ServiceArchi.Contracts;
using UnifyStorage.Runtime.RocksDb;

namespace FantaSim.App;

/// <summary>
/// Godot autoload node. Creates BundleHost + services in _Ready(),
/// exposes methods callable from GDScript bundles.
/// Shell bundle wires dock slots via RegisterDockSlot/SetMenuBar/etc.
/// </summary>
public partial class BootstrapShim : Node
{
    private BundleHost? _bundleHost;
    private EventBridge? _eventBridge;
    private CommandRouter? _commandRouter;
    private DockManager? _dockManager;
    private MenuService? _menuService;
    private StatusService? _statusService;
    private MessagePipeBundleMessageBus? _messageBus;
    private RocksDbKeyValueStore? _kvStore;
    private ILogger? _log;
    private VerificationService? _verificationService;

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
        var builder = new AppServiceBuilder();
        var result = builder.Build(this, GetTree().Root.GetNode("Main"));

        Registry = result.Registry;
        _bundleHost = result.BundleHost;
        _messageBus = result.MessageBus;
        _dockManager = result.DockManager;
        _menuService = result.MenuService;
        _statusService = result.StatusService;
        _kvStore = result.KvStore;
        _log = result.Logger;

        // Command router (subscribes to GdScriptCommand on the bus)
        _commandRouter = new CommandRouter(_messageBus, _log, result.SelectionService, _bundleHost);

        // Bridge C# events to Godot signals for GDScript consumers
        _eventBridge = new EventBridge(_messageBus, this);

        _log.LogInformation("BundleHost created");

        // Start verification service if --verify flag is present
        var args = OS.GetCmdlineUserArgs();
        if (args.Contains("--verify"))
        {
            _verificationService = new VerificationService(this, result.Logger);
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
        _eventBridge?.Dispose();
        _commandRouter?.Dispose();

        if (Registry is not null)
        {
            new Crosscut.Hosting.ServiceProxy(Registry).StopAsync().GetAwaiter().GetResult();
            _log?.LogInformation("Hosted components shut down");
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
