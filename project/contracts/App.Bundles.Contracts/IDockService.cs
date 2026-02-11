namespace FantaSim.App.Bundles.Contracts;

/// <summary>
/// Query-only dock slot interface. Pure .NET — no Godot types.
/// Full docking API (DockPanel) stays on the concrete DockManager class
/// because it requires Godot Control parameters.
/// </summary>
public interface IDockService
{
    bool HasSlot(string name);
}
