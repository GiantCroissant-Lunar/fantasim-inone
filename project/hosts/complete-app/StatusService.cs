using Godot;

namespace Fantasim.App;

/// <summary>
/// Thin wrapper for the status label and shared file dialog.
/// Bundles call this to show status text or trigger the PCK file picker.
/// </summary>
public sealed class StatusService
{
    private Label? _statusLabel;
    private FileDialog? _fileDialog;
    private System.Action<string>? _pendingCallback;

    public void SetStatusLabel(Label label)
    {
        _statusLabel = label;
    }

    public void SetFileDialog(FileDialog dialog)
    {
        _fileDialog = dialog;
        _fileDialog.FileSelected += OnFileSelected;
    }

    public void ShowStatus(string text)
    {
        if (_statusLabel is not null)
        {
            _statusLabel.Text = text;
        }
    }

    public void ShowFileDialog(System.Action<string> onSelected)
    {
        if (_fileDialog is null) return;
        _pendingCallback = onSelected;
        _fileDialog.PopupCentered(new Vector2I(600, 400));
    }

    private void OnFileSelected(string path)
    {
        _pendingCallback?.Invoke(path);
        _pendingCallback = null;
    }
}
