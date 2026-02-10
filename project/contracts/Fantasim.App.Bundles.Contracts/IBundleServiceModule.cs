namespace Fantasim.App.Bundles.Contracts;

/// <summary>
/// Discovered via reflection inside bundle assemblies.
/// Called during LoadAsync (Register) and UnloadAsync (Deregister).
/// </summary>
public interface IBundleServiceModule
{
    void Register(IBundleServiceRegistry registry);
    void Deregister(IBundleServiceRegistry registry);
}
