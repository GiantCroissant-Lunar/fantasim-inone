namespace Fantasim.App.Bundles.Contracts;

/// <summary>
/// Abstraction over Godot's VFS for PCK mounting and file access.
/// Enables testing without Godot engine.
/// </summary>
public interface IGodotBundleVfs
{
    bool MountPck(string pckPath);
    byte[] ReadFile(string resPath);
    bool FileExists(string resPath);
}
