using Godot;

namespace Fantasim.App;

/// <summary>
/// Minimal editor shell. Wires dock slots and services to Godot nodes,
/// then auto-loads system bundles. All business logic lives in bundles.
/// </summary>
public partial class HudRoot : VBoxContainer
{
    public override void _Ready()
    {
        var shim = GetNode<BootstrapShim>("/root/Bootstrap");

        // Wire services to nodes
        shim.DockManager!.RegisterSlot(DockManager.Slots.Inspector, GetNode<TabContainer>("%InspectorSlot"));
        shim.DockManager!.RegisterSlot(DockManager.Slots.Bottom, GetNode<TabContainer>("%BottomSlot"));
        shim.MenuService!.SetMenuBar(GetNode<MenuBar>("%EditorMenuBar"));
        shim.StatusService!.SetStatusLabel(GetNode<Label>("%StatusLabel"));
        shim.StatusService!.SetFileDialog(GetNode<FileDialog>("%PckFileDialog"));

        // Auto-load system bundles
        AutoLoadBundles("res://system_bundles", shim);

        GD.Print("[HUD] Editor shell ready");
    }

    private async void AutoLoadBundles(string directory, BootstrapShim shim)
    {
        if (shim.BundleHost is null) return;

        var dir = DirAccess.Open(directory);
        if (dir is null)
        {
            GD.Print($"[HUD] No system bundles directory at {directory}");
            return;
        }

        dir.ListDirBegin();
        var fileName = dir.GetNext();
        while (!string.IsNullOrEmpty(fileName))
        {
            if (fileName.EndsWith(".pck"))
            {
                var path = $"{directory}/{fileName}";
                try
                {
                    await shim.BundleHost.LoadAsync(path);
                    GD.Print($"[HUD] Auto-loaded: {fileName}");
                }
                catch (System.Exception ex)
                {
                    GD.PrintErr($"[HUD] Failed to auto-load {fileName}: {ex.Message}");
                }
            }
            fileName = dir.GetNext();
        }
    }
}
