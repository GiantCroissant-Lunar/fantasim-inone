namespace FantaSim.App.Bundles.Contracts;

/// <summary>
/// Platform-agnostic logging abstraction for FantaSim services.
/// Godot host provides GD.Print/GD.PrintErr implementation;
/// tests can use a no-op or capturing fake.
/// </summary>
public interface IFantaSimLog
{
    void Info(string category, string message);
    void Warn(string category, string message);
    void Error(string category, string message);
    void Error(string category, string message, Exception ex);
}
