using FantaSim.App.Bundles.Contracts;
using FantaSim.App.Bundles.Contracts.Interaction.Commands;
using FantaSim.App.Bundles.Contracts.Interaction.Selection;
using Godot;

namespace FantaSim.App;

/// <summary>
/// Subscribes to <see cref="GdScriptCommand"/> on the message bus and routes
/// by action name to the appropriate service. New GDScript actions are added here,
/// NOT to BootstrapShim.
/// </summary>
public sealed class GdScriptCommandRouter : IDisposable
{
    private readonly IDisposable _subscription;
    private readonly ISelectionService _selection;
    private readonly IBundleHost _bundleHost;

    public GdScriptCommandRouter(
        IBundleMessageBus bus,
        ISelectionService selection,
        IBundleHost bundleHost)
    {
        _selection = selection;
        _bundleHost = bundleHost;
        _subscription = bus.Subscribe<GdScriptCommand>(Route);
    }

    private void Route(GdScriptCommand cmd)
    {
        switch (cmd.Action)
        {
            case "select":
                RouteSelect(cmd);
                break;

            case "clear_selection":
                _selection.Clear();
                break;

            case "load_bundle":
                RouteLoadBundle(cmd);
                break;

            case "unload_bundle":
                RouteUnloadBundle(cmd);
                break;

            default:
                GD.PrintErr($"[GdScriptCommandRouter] Unknown action: {cmd.Action}");
                break;
        }
    }

    private void RouteSelect(GdScriptCommand cmd)
    {
        if (!cmd.Params.TryGetValue("kind", out var kindObj) ||
            !cmd.Params.TryGetValue("id", out var idObj) ||
            kindObj is not string kind ||
            idObj is not string id)
        {
            GD.PrintErr("[GdScriptCommandRouter] select requires 'kind' and 'id' string params");
            return;
        }

        _selection.Select(new SelectableRef(kind, id));
    }

    private async void RouteLoadBundle(GdScriptCommand cmd)
    {
        if (!cmd.Params.TryGetValue("path", out var pathObj) || pathObj is not string path)
        {
            GD.PrintErr("[GdScriptCommandRouter] load_bundle requires 'path' string param");
            return;
        }

        try
        {
            await _bundleHost.LoadAsync(path);
        }
        catch (Exception ex)
        {
            GD.PrintErr($"[GdScriptCommandRouter] load_bundle failed: {ex.Message}");
        }
    }

    private async void RouteUnloadBundle(GdScriptCommand cmd)
    {
        if (!cmd.Params.TryGetValue("id", out var idObj) || idObj is not string bundleId)
        {
            GD.PrintErr("[GdScriptCommandRouter] unload_bundle requires 'id' string param");
            return;
        }

        try
        {
            await _bundleHost.UnloadAsync(bundleId);
        }
        catch (Exception ex)
        {
            GD.PrintErr($"[GdScriptCommandRouter] unload_bundle failed: {ex.Message}");
        }
    }

    public void Dispose()
    {
        _subscription.Dispose();
    }
}
