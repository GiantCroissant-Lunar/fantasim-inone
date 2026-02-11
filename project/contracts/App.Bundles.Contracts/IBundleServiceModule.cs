using ServiceArchi.Contracts;

namespace FantaSim.App.Bundles.Contracts;

/// <summary>
/// Discovered via reflection inside bundle assemblies.
/// Called during LoadAsync (Register) and UnloadAsync (Deregister).
/// </summary>
public interface IBundleServiceModule
{
    void Register(IRegistry registry);
    void Deregister(IRegistry registry);
}
