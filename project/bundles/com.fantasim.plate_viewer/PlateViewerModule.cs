using FantaSim.App.Bundles.Contracts;

namespace FantaSim.Bundles.PlateViewer;

/// <summary>
/// Minimal service module proving the bundle pipeline:
/// build → pack PCK → load in Godot → Register → Deregister → unload.
/// </summary>
public sealed class PlateViewerModule : IBundleServiceModule
{
    public void Register(IBundleServiceRegistry registry)
    {
        // Future: register plate viewer services
    }

    public void Deregister(IBundleServiceRegistry registry)
    {
        // Future: deregister plate viewer services
    }
}
