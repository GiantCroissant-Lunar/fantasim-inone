using FantaSim.App.Bundles.Contracts;

namespace FantaSim.App.Bundles.Tests.Fakes;

/// <summary>
/// In-memory VFS for testing. No Godot engine needed.
/// </summary>
public sealed class FakeBundleVfs : IBundleVfs
{
    private readonly Dictionary<string, byte[]> _files = new();
    private readonly HashSet<string> _mountedPcks = new();

    public void AddFile(string resPath, byte[] content)
    {
        _files[resPath] = content;
    }

    public bool MountPck(string pckPath)
    {
        _mountedPcks.Add(pckPath);
        return true;
    }

    public byte[] ReadFile(string resPath)
    {
        if (!_files.TryGetValue(resPath, out var content))
        {
            throw new FileNotFoundException($"File not found: {resPath}");
        }
        return content;
    }

    public bool FileExists(string resPath)
    {
        return _files.ContainsKey(resPath);
    }

    public IReadOnlyList<string> ListFiles(string resDir)
    {
        var prefix = resDir.EndsWith('/') ? resDir : $"{resDir}/";
        return _files.Keys
            .Where(path => path.StartsWith(prefix, StringComparison.Ordinal))
            .OrderBy(path => path, StringComparer.Ordinal)
            .ToList();
    }

    public bool WasMounted(string pckPath) => _mountedPcks.Contains(pckPath);
}
