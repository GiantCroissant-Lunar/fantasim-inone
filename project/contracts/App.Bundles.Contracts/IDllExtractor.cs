namespace FantaSim.App.Bundles.Contracts;

/// <summary>
/// Extracts DLLs from mounted PCK to a temp directory for ALC loading.
/// GUID-suffixed directories enable reload safety (old DLLs not locked).
/// </summary>
public interface IDllExtractor
{
    /// <summary>
    /// Extracts DLLs from the VFS to a temp directory on disk.
    /// Returns the directory path containing the extracted DLLs.
    /// </summary>
    string ExtractDlls(string bundleId, IReadOnlyList<string> dllResPaths, IGodotBundleVfs vfs);

    /// <summary>
    /// Cleans up the temp directory for a bundle.
    /// </summary>
    void Cleanup(string bundleId);
}
