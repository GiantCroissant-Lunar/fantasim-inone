namespace FantaSim.App.Bundles.Contracts;

/// <summary>
/// Shows status text to the user. Pure .NET interface — no Godot types.
/// </summary>
public interface IStatusService
{
    void ShowStatus(string text);
}
