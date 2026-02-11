using FantaSim.App.Bundles.Contracts;
using Godot;

namespace FantaSim.App;

/// <summary>
/// Godot implementation of <see cref="IFantaSimLog"/>.
/// Routes Info/Warn to GD.Print and Error to GD.PrintErr.
/// </summary>
public sealed class GodotLog : IFantaSimLog
{
    public void Info(string category, string message) =>
        GD.Print($"[{category}] {message}");

    public void Warn(string category, string message) =>
        GD.Print($"[{category}] WARN: {message}");

    public void Error(string category, string message) =>
        GD.PrintErr($"[{category}] {message}");

    public void Error(string category, string message, Exception ex) =>
        GD.PrintErr($"[{category}] {message}: {ex}");
}
