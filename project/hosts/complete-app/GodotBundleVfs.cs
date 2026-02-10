using FantaSim.App.Bundles.Contracts;
using Godot;
using GdFileAccess = Godot.FileAccess;

namespace FantaSim.App;

/// <summary>
/// Thin adapter wrapping Godot's VFS for PCK bundle operations.
/// This is the ONLY Godot-dependent file in the bundle system.
/// </summary>
public sealed class GodotBundleVfs : IGodotBundleVfs
{
    public bool MountPck(string pckPath)
    {
        return ProjectSettings.LoadResourcePack(pckPath, replaceFiles: true);
    }

    public byte[] ReadFile(string resPath)
    {
        using var file = GdFileAccess.Open(resPath, GdFileAccess.ModeFlags.Read);
        if (file is null)
        {
            throw new System.IO.FileNotFoundException($"File not found in VFS: {resPath}");
        }

        return file.GetBuffer((long)file.GetLength());
    }

    public bool FileExists(string resPath)
    {
        return GdFileAccess.FileExists(resPath);
    }
}
