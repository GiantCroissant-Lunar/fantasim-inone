using System.Text.Json;
using System.Text.Json.Serialization;
using Godot;
using Microsoft.Extensions.Logging;

namespace FantaSim.App;

/// <summary>
/// Captures screenshot, scene tree, and bundle snapshot after all bundles load.
/// Activated by --verify command-line flag. Outputs to _verify/ next to the executable.
/// </summary>
public sealed class VerificationService
{
    private const int SettleFrames = 90;

    private readonly BootstrapShim _bootstrap;
    private readonly ILogger _log;
    private readonly string _outputDir;
    private int _frameCount;
    private bool _capturing;

    public VerificationService(BootstrapShim bootstrap, ILogger log)
    {
        _bootstrap = bootstrap;
        _log = log;
        var exeDir = System.IO.Path.GetDirectoryName(OS.GetExecutablePath()) ?? ".";
        _outputDir = System.IO.Path.Combine(exeDir, "_verify");
    }

    public void Start()
    {
        _bootstrap.AllBundlesLoaded += OnAllBundlesLoaded;
        _log.LogInformation("Waiting for AllBundlesLoaded signal...");
    }

    private void OnAllBundlesLoaded()
    {
        _capturing = true;
        _frameCount = 0;
        _log.LogInformation("Settling for {SettleFrames} frames...", SettleFrames);
    }

    public void ProcessFrame()
    {
        if (!_capturing) return;

        _frameCount++;
        if (_frameCount >= SettleFrames)
        {
            _capturing = false;
            CaptureAndExit();
        }
    }

    private void CaptureAndExit()
    {
        System.IO.Directory.CreateDirectory(_outputDir);

        var ok = true;
        ok &= CaptureScreenshot();
        ok &= CaptureSceneTree();
        ok &= CaptureBundleSnapshot();

        if (ok)
        {
            _log.LogInformation("All artifacts written to {OutputDir}", _outputDir);
            _bootstrap.GetTree().Quit(0);
        }
        else
        {
            _log.LogError("Some captures failed");
            _bootstrap.GetTree().Quit(1);
        }
    }

    private bool CaptureScreenshot()
    {
        try
        {
            var image = _bootstrap.GetViewport().GetTexture().GetImage();
            var path = System.IO.Path.Combine(_outputDir, "screenshot.png");
            var err = image.SavePng(path);
            if (err != Error.Ok)
            {
                _log.LogError("Screenshot save failed: {Error}", err);
                return false;
            }
            _log.LogInformation("Screenshot: {Path} ({Width}x{Height})", path, image.GetWidth(), image.GetHeight());
            return true;
        }
        catch (System.Exception ex)
        {
            _log.LogError(ex, "Screenshot error");
            return false;
        }
    }

    private bool CaptureSceneTree()
    {
        try
        {
            var root = _bootstrap.GetTree().Root;
            var tree = SerializeNode(root);
            var json = JsonSerializer.Serialize(tree, new JsonSerializerOptions
            {
                WriteIndented = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            });
            var path = System.IO.Path.Combine(_outputDir, "scene_tree.json");
            System.IO.File.WriteAllText(path, json);
            _log.LogInformation("Scene tree: {Path}", path);
            return true;
        }
        catch (System.Exception ex)
        {
            _log.LogError(ex, "Scene tree error");
            return false;
        }
    }

    private bool CaptureBundleSnapshot()
    {
        try
        {
            if (_bootstrap.BundleHost is null)
            {
                _log.LogError("BundleHost is null");
                return false;
            }

            var snapshot = _bootstrap.BundleHost.CaptureSnapshot();
            var json = JsonSerializer.Serialize(snapshot, new JsonSerializerOptions
            {
                WriteIndented = true,
                Converters = { new JsonStringEnumConverter() }
            });
            var path = System.IO.Path.Combine(_outputDir, "snapshot.json");
            System.IO.File.WriteAllText(path, json);
            _log.LogInformation("Bundle snapshot: {Path}", path);
            return true;
        }
        catch (System.Exception ex)
        {
            _log.LogError(ex, "Snapshot error");
            return false;
        }
    }

    private static Dictionary<string, object?> SerializeNode(Node node)
    {
        var dict = new Dictionary<string, object?>
        {
            ["name"] = node.Name.ToString(),
            ["type"] = node.GetClass(),
            ["path"] = node.GetPath().ToString(),
        };

        if (node is Control control)
        {
            dict["visible"] = control.Visible;
            dict["rect"] = new Dictionary<string, float>
            {
                ["x"] = control.Position.X,
                ["y"] = control.Position.Y,
                ["w"] = control.Size.X,
                ["h"] = control.Size.Y,
            };
        }

        var childCount = node.GetChildCount();
        if (childCount > 0)
        {
            var children = new List<Dictionary<string, object?>>();
            for (int i = 0; i < childCount; i++)
            {
                children.Add(SerializeNode(node.GetChild(i)));
            }
            dict["children"] = children;
        }

        return dict;
    }
}
