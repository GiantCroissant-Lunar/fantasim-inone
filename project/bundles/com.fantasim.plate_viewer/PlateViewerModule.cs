using FantaSim.App.Bundles.Contracts;
using ServiceArchi.Contracts;

namespace FantaSim.Bundles.PlateViewer;

/// <summary>
/// Minimal service module proving the bundle pipeline:
/// build → pack PCK → load in Godot → Register → Deregister → unload.
/// </summary>
public sealed class PlateViewerModule : IBundleServiceModule
{
    public void Register(IRegistry registry)
    {
        // Future: register plate viewer services
    }

    public void Deregister(IRegistry registry)
    {
        // Future: deregister plate viewer services
    }
}
