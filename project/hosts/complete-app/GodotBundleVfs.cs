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

    public IReadOnlyList<string> ListFiles(string resDir)
    {
        var files = new List<string>();
        CollectFiles(resDir, files);
        return files;
    }

    private static void CollectFiles(string resDir, List<string> files)
    {
        var dir = DirAccess.Open(resDir);
        if (dir is null)
        {
            return;
        }

        dir.ListDirBegin();
        var name = dir.GetNext();
        while (!string.IsNullOrEmpty(name))
        {
            if (!name.StartsWith('.'))
            {
                var childPath = $"{resDir}/{name}";
                if (dir.CurrentIsDir())
                {
                    CollectFiles(childPath, files);
                }
                else
                {
                    files.Add(childPath);
                }
            }

            name = dir.GetNext();
        }

        dir.ListDirEnd();
    }
}
