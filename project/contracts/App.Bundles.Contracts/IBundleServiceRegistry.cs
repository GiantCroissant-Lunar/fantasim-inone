namespace FantaSim.App.Bundles.Contracts;

/// <summary>
/// Simple typed dictionary for bundle services.
/// </summary>
public interface IBundleServiceRegistry
{
    void Register<T>(T service) where T : class;
    void Deregister<T>() where T : class;
    T? Resolve<T>() where T : class;
    IReadOnlyList<string> RegisteredTypeNames { get; }
}
