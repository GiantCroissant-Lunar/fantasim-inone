namespace FantaSim.App.Bundles.Contracts;

/// <summary>
/// Optional interface for bundles that preserve state across hot-reloads.
/// </summary>
public interface IReloadAwareBundle
{
    object? CaptureState();
    void RestoreState(object? state);
}
