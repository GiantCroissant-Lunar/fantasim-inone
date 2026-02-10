using Fantasim.App.Bundles.Contracts;

namespace Fantasim.App.Bundles.Core;

/// <summary>
/// Extracts DLLs from mounted PCK VFS to a temp directory.
/// GUID-suffixed directories ensure reload safety — old files aren't locked.
/// </summary>
public sealed class DllExtractor : IDllExtractor
{
    private readonly string _baseTempDir;
    private readonly Dictionary<string, string> _bundleDirs = new();

    public DllExtractor(string? baseTempDir = null)
    {
        _baseTempDir = baseTempDir ?? Path.Combine(Path.GetTempPath(), ".bundle-temp");
    }

    public string ExtractDlls(string bundleId, IReadOnlyList<string> dllResPaths, IGodotBundleVfs vfs)
    {
        var dir = Path.Combine(_baseTempDir, $"{bundleId}-{Guid.NewGuid():N}");
        Directory.CreateDirectory(dir);
        _bundleDirs[bundleId] = dir;

        foreach (var resPath in dllResPaths)
        {
            var bytes = vfs.ReadFile(resPath);
            var fileName = Path.GetFileName(resPath);
            File.WriteAllBytes(Path.Combine(dir, fileName), bytes);
        }

        return dir;
    }

    public void Cleanup(string bundleId)
    {
        if (_bundleDirs.Remove(bundleId, out var dir) && Directory.Exists(dir))
        {
            try
            {
                Directory.Delete(dir, recursive: true);
            }
            catch (IOException)
            {
                // Best-effort cleanup — OS may still hold handles briefly after ALC unload
            }
        }
    }
}
