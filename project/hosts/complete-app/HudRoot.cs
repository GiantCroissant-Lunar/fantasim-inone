using Fantasim.App.Bundles.Contracts;
using Godot;

namespace Fantasim.App;

/// <summary>
/// Bundle system inspector HUD. Left panel reserved for 3D viewport,
/// right panel shows toolbar + live snapshot tree.
/// </summary>
public partial class HudRoot : HSplitContainer
{
    private IBundleHost? _bundleHost;

    private Button _loadBtn = null!;
    private Button _unloadBtn = null!;
    private Button _reloadBtn = null!;
    private Button _snapshotBtn = null!;
    private Tree _snapshotTree = null!;
    private Label _statusLabel = null!;
    private FileDialog _fileDialog = null!;
    private Godot.Timer _autoRefreshTimer = null!;

    public override void _Ready()
    {
        var shim = GetNode<BootstrapShim>("/root/Bootstrap");
        _bundleHost = shim.BundleHost;

        _loadBtn = GetNode<Button>("%LoadBtn");
        _unloadBtn = GetNode<Button>("%UnloadBtn");
        _reloadBtn = GetNode<Button>("%ReloadBtn");
        _snapshotBtn = GetNode<Button>("%SnapshotBtn");
        _snapshotTree = GetNode<Tree>("%SnapshotTree");
        _statusLabel = GetNode<Label>("%StatusLabel");
        _fileDialog = GetNode<FileDialog>("%PckFileDialog");

        _loadBtn.Pressed += OnLoadPressed;
        _unloadBtn.Pressed += OnUnloadPressed;
        _reloadBtn.Pressed += OnReloadPressed;
        _snapshotBtn.Pressed += OnSnapshotPressed;
        _fileDialog.FileSelected += OnFileSelected;

        _autoRefreshTimer = new Godot.Timer();
        _autoRefreshTimer.WaitTime = 2.0;
        _autoRefreshTimer.Autostart = true;
        _autoRefreshTimer.Timeout += OnSnapshotPressed;
        AddChild(_autoRefreshTimer);

        RefreshSnapshot();
        GD.Print("[HUD] Bundle inspector ready");
    }

    private void OnLoadPressed()
    {
        _fileDialog.PopupCentered(new Vector2I(600, 400));
    }

    private async void OnFileSelected(string path)
    {
        if (_bundleHost is null) return;
        try
        {
            _statusLabel.Text = $"Loading {path}...";
            await _bundleHost.LoadAsync(path);
            RefreshSnapshot();
        }
        catch (System.Exception ex)
        {
            _statusLabel.Text = $"Error: {ex.Message}";
            GD.PrintErr($"[HUD] Load failed: {ex}");
        }
    }

    private async void OnUnloadPressed()
    {
        if (_bundleHost is null) return;
        var bundleId = GetSelectedBundleId();
        if (bundleId is null)
        {
            _statusLabel.Text = "Select a bundle to unload";
            return;
        }

        try
        {
            _statusLabel.Text = $"Unloading {bundleId}...";
            await _bundleHost.UnloadAsync(bundleId);
            RefreshSnapshot();
        }
        catch (System.Exception ex)
        {
            _statusLabel.Text = $"Error: {ex.Message}";
            GD.PrintErr($"[HUD] Unload failed: {ex}");
        }
    }

    private async void OnReloadPressed()
    {
        if (_bundleHost is null) return;
        var bundleId = GetSelectedBundleId();
        if (bundleId is null)
        {
            _statusLabel.Text = "Select a bundle to reload";
            return;
        }

        try
        {
            _statusLabel.Text = $"Reloading {bundleId}...";
            await _bundleHost.ReloadAsync(bundleId);
            RefreshSnapshot();
        }
        catch (System.Exception ex)
        {
            _statusLabel.Text = $"Error: {ex.Message}";
            GD.PrintErr($"[HUD] Reload failed: {ex}");
        }
    }

    private void OnSnapshotPressed()
    {
        RefreshSnapshot();
    }

    private void RefreshSnapshot()
    {
        if (_bundleHost is null) return;

        // Preserve selection across refresh
        var previousSelection = GetSelectedBundleId();

        var snapshot = _bundleHost.CaptureSnapshot();
        _snapshotTree.Clear();

        var root = _snapshotTree.CreateItem();
        root.SetText(0, "Bundle System");

        // Bundles section
        var bundlesRoot = _snapshotTree.CreateItem(root);
        bundlesRoot.SetText(0, $"Bundles ({snapshot.Bundles.Count})");

        foreach (var bundle in snapshot.Bundles)
        {
            var bundleItem = _snapshotTree.CreateItem(bundlesRoot);
            bundleItem.SetText(0, bundle.Id);
            bundleItem.SetMetadata(0, bundle.Id);

            AddDetail(bundleItem, $"Status: {bundle.Status}");
            AddDetail(bundleItem, $"Version: {bundle.Manifest.Version}");
            AddDetail(bundleItem, $"Display: {bundle.Manifest.DisplayName}");
            AddDetail(bundleItem, $"LoadedAt: {bundle.LoadedAt:HH:mm:ss.fff}");
            AddDetail(bundleItem, $"HasALC: {bundle.HasAssemblyLoadContext}");

            if (bundle.TrackedSceneNodes.Count > 0)
            {
                var nodesItem = _snapshotTree.CreateItem(bundleItem);
                nodesItem.SetText(0, $"TrackedNodes ({bundle.TrackedSceneNodes.Count})");
                foreach (var node in bundle.TrackedSceneNodes)
                {
                    AddDetail(nodesItem, node);
                }
            }

            // Restore selection
            if (bundle.Id == previousSelection)
            {
                bundleItem.Select(0);
            }
        }

        // Services section
        var servicesRoot = _snapshotTree.CreateItem(root);
        servicesRoot.SetText(0, $"Services ({snapshot.RegisteredServiceTypes.Count})");

        foreach (var typeName in snapshot.RegisteredServiceTypes)
        {
            AddDetail(servicesRoot, typeName);
        }

        // MessageBus section
        var busRoot = _snapshotTree.CreateItem(root);
        var channelCount = snapshot.MessageBus?.ActiveChannelCount ?? 0;
        busRoot.SetText(0, $"MessageBus (channels: {channelCount})");

        _statusLabel.Text = $"Snapshot at {snapshot.CapturedAt:HH:mm:ss.fff}";
    }

    private void AddDetail(TreeItem parent, string text)
    {
        var item = _snapshotTree.CreateItem(parent);
        item.SetText(0, text);
    }

    private string? GetSelectedBundleId()
    {
        var selected = _snapshotTree.GetSelected();
        if (selected is null) return null;

        // Walk up to find the bundle-level item (which has metadata set)
        var current = selected;
        while (current is not null)
        {
            var meta = current.GetMetadata(0);
            if (meta.VariantType == Variant.Type.String)
            {
                var id = meta.AsString();
                if (!string.IsNullOrEmpty(id)) return id;
            }
            current = current.GetParent();
        }

        return null;
    }
}
