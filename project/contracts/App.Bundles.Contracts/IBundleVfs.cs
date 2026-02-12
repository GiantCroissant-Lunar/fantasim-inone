namespace FantaSim.App.Bundles.Contracts;

/// <summary>
/// Abstraction over the host VFS for PCK mounting and file access.
/// Enables testing without Godot engine.
/// </summary>
public interface IBundleVfs
{
    bool MountPck(string pckPath);
    byte[] ReadFile(string resPath);
    bool FileExists(string resPath);
    IReadOnlyList<string> ListFiles(string resDir);
}
