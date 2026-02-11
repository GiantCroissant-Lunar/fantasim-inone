using FantaSim.App.Bundles.Contracts;
using Godot;
using Microsoft.Extensions.Logging;

namespace FantaSim.App;

/// <summary>
/// Manages named dock slots backed by TabContainers.
/// Bundles dock panels into slots by name; undocking removes them.
/// </summary>
public sealed class DockManager : IDockService
{
    public static class Slots
    {
        public const string Inspector = "inspector";
        public const string Bottom = "bottom";
    }

    private readonly ILogger _log;
    private readonly Dictionary<string, TabContainer> _slots = new();
    private readonly Dictionary<string, List<(string SlotName, Control Panel)>> _dockedPanels = new();

    public DockManager(ILogger log)
    {
        _log = log;
    }

    public bool HasSlot(string name) => _slots.ContainsKey(name);

    public void RegisterSlot(string name, TabContainer container)
    {
        _slots[name] = container;
    }

    public bool DockPanel(string bundleId, string slotName, Control panel, string? tabTitle = null)
    {
        if (!_slots.TryGetValue(slotName, out var container))
        {
            _log.LogError("Unknown slot: {SlotName}", slotName);
            return false;
        }

        panel.Name = tabTitle ?? bundleId;
        container.AddChild(panel);

        if (!_dockedPanels.TryGetValue(bundleId, out var panels))
        {
            panels = [];
            _dockedPanels[bundleId] = panels;
        }

        panels.Add((slotName, panel));
        _log.LogInformation("'{BundleId}' docked into '{SlotName}'", bundleId, slotName);
        return true;
    }

    public void UndockBundle(string bundleId)
    {
        if (!_dockedPanels.Remove(bundleId, out var panels))
        {
            return;
        }

        foreach (var (_, panel) in panels)
        {
            panel.QueueFree();
        }

        _log.LogInformation("'{BundleId}' undocked ({Count} panels)", bundleId, panels.Count);
    }
}
