using System.Text.Json;
using System.Text.Json.Serialization;
using Godot;

namespace Fantasim.App;

/// <summary>
/// Captures screenshot, scene tree, and bundle snapshot after all bundles load.
/// Activated by --verify command-line flag. Outputs to build/_verify/.
/// </summary>
public sealed class VerificationService
{
    private const int SettleFrames = 90;

    private readonly BootstrapShim _bootstrap;
    private readonly string _outputDir;
    private int _frameCount;
    private bool _capturing;

    public VerificationService(BootstrapShim bootstrap)
    {
        _bootstrap = bootstrap;
        _outputDir = System.IO.Path.Combine(
            System.IO.Directory.GetCurrentDirectory(), "build", "_verify");
    }

    public void Start()
    {
        _bootstrap.AllBundlesLoaded += OnAllBundlesLoaded;
        GD.Print("[Verify] Waiting for AllBundlesLoaded signal...");
    }

    private void OnAllBundlesLoaded()
    {
        _capturing = true;
        _frameCount = 0;
        GD.Print($"[Verify] Settling for {SettleFrames} frames...");
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
            GD.Print($"[Verify] All artifacts written to {_outputDir}");
            _bootstrap.GetTree().Quit(0);
        }
        else
        {
            GD.PrintErr("[Verify] Some captures failed");
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
                GD.PrintErr($"[Verify] Screenshot save failed: {err}");
                return false;
            }
            GD.Print($"[Verify] Screenshot: {path} ({image.GetWidth()}x{image.GetHeight()})");
            return true;
        }
        catch (System.Exception ex)
        {
            GD.PrintErr($"[Verify] Screenshot error: {ex.Message}");
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
            GD.Print($"[Verify] Scene tree: {path}");
            return true;
        }
        catch (System.Exception ex)
        {
            GD.PrintErr($"[Verify] Scene tree error: {ex.Message}");
            return false;
        }
    }

    private bool CaptureBundleSnapshot()
    {
        try
        {
            if (_bootstrap.BundleHost is null)
            {
                GD.PrintErr("[Verify] BundleHost is null");
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
            GD.Print($"[Verify] Bundle snapshot: {path}");
            return true;
        }
        catch (System.Exception ex)
        {
            GD.PrintErr($"[Verify] Snapshot error: {ex.Message}");
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
