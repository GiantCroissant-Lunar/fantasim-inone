namespace FantaSim.App.Bundles.Contracts;

/// <summary>
/// Menu mutation interface. Pure .NET — no Godot types.
/// GetOrAddMenu returns Godot PopupMenu and stays on the concrete class.
/// </summary>
public interface IMenuService
{
    void RemoveMenu(string title);
}
