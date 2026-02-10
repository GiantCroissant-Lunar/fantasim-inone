using FantaSim.App.Bundles.Contracts;
using Godot;

namespace FantaSim.App;

/// <summary>
/// Wraps the host MenuBar. Bundles call this to add/remove menus and items.
/// </summary>
public sealed class MenuService : IMenuService
{
    private MenuBar? _menuBar;
    private readonly Dictionary<string, PopupMenu> _menus = new();

    public void SetMenuBar(MenuBar bar)
    {
        _menuBar = bar;
    }

    public PopupMenu AddMenu(string menuTitle)
    {
        if (_menuBar is null)
        {
            throw new System.InvalidOperationException("MenuBar not set");
        }

        var popup = new PopupMenu { Name = menuTitle };
        _menuBar.AddChild(popup);
        _menus[menuTitle] = popup;
        return popup;
    }

    public PopupMenu GetOrAddMenu(string menuTitle)
    {
        if (_menus.TryGetValue(menuTitle, out var existing))
        {
            return existing;
        }

        return AddMenu(menuTitle);
    }

    public void RemoveMenu(string menuTitle)
    {
        if (_menus.Remove(menuTitle, out var popup))
        {
            popup.QueueFree();
        }
    }

    public void AddMenuItem(string menuTitle, string itemLabel, System.Action callback, Key shortcut = Key.None)
    {
        var menu = GetOrAddMenu(menuTitle);
        var idx = menu.ItemCount;
        menu.AddItem(itemLabel);

        if (shortcut != Key.None)
        {
            var sc = new Shortcut();
            var ev = new InputEventKey { Keycode = shortcut };
            sc.Events = [ev];
            menu.SetItemShortcut(idx, sc);
        }

        menu.IdPressed += id =>
        {
            if (id == idx)
            {
                callback();
            }
        };
    }

    public void AddSeparator(string menuTitle)
    {
        var menu = GetOrAddMenu(menuTitle);
        menu.AddSeparator();
    }

    public void AddCheckItem(string menuTitle, string itemLabel, bool initialState, System.Action<bool> callback)
    {
        var menu = GetOrAddMenu(menuTitle);
        var idx = menu.ItemCount;
        menu.AddCheckItem(itemLabel);
        menu.SetItemChecked(idx, initialState);

        menu.IdPressed += id =>
        {
            if (id == idx)
            {
                var newState = !menu.IsItemChecked(idx);
                menu.SetItemChecked(idx, newState);
                callback(newState);
            }
        };
    }
}
