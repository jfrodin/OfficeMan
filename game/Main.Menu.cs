using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Godot;
using OfficeMan.Core;
using OfficeMan.Core.Content;

namespace OfficeMan.Game;

/// <summary>
/// The pause menu: continue, save, load, settings, quit. Opens with Esc or the pad's Start button,
/// closes with the same or with Back. Settings are kept in the user folder as settings.json.
/// </summary>
public partial class Main
{
    private sealed class Settings
    {
        public bool Fullscreen { get; set; }
        public int TextSpeed { get; set; } = 1;   // index into TextSpeeds
    }

    private static readonly (string name, double cps)[] TextSpeeds = { ("Slow", 32), ("Normal", 55), ("Fast", 90), ("Instant", 100000) };
    private static readonly (string slot, string name)[] Slots = { ("manual", "Saved game"), ("quicksave", "Quick save (F5)"), ("autosave", "Autosave") };

    private Settings _settings = new();
    private PanelContainer _menu = null!;
    private Label _menuTitle = null!;
    private VBoxContainer _menuItems = null!;
    private ColorRect _menuShade = null!;
    private Mode _beforeMenu;
    private Action? _menuBack;

    private string SettingsPath => Path.Combine(OS.GetUserDataDir(), "settings.json");

    private void BuildMenu()
    {
        _menuShade = new ColorRect { Color = new Color(0, 0, 0, 0.55f), Visible = false };
        _menuShade.SetAnchorsPreset(LayoutPreset.FullRect);
        AddChild(_menuShade);

        _menu = new PanelContainer { Visible = false, Size = new Vector2(560, 0) };
        _menu.AddThemeStyleboxOverride("panel", Box(new Color(0.045f, 0.04f, 0.055f, 0.97f), 6, 2, new Color(1f, 0.85f, 0.65f, 0.25f), 44, 34));
        var box = new VBoxContainer();
        box.AddThemeConstantOverride("separation", 14);
        _menuTitle = MakeLabel(BodySize, Accent);
        _menuItems = new VBoxContainer();
        _menuItems.AddThemeConstantOverride("separation", 6);
        box.AddChild(_menuTitle);
        box.AddChild(_menuItems);
        _menu.AddChild(box);
        AddChild(_menu);
    }

    private void LoadSettings()
    {
        try
        {
            if (File.Exists(SettingsPath)) _settings = JsonSerializer.Deserialize<Settings>(File.ReadAllText(SettingsPath)) ?? new();
        }
        catch (Exception) { _settings = new(); }
        ApplySettings();
    }

    private void SaveSettings()
    {
        try { File.WriteAllText(SettingsPath, JsonSerializer.Serialize(_settings)); }
        catch (Exception) { /* a settings file that can't be written is not worth stopping the game for */ }
    }

    private void ApplySettings()
    {
        _settings.TextSpeed = Math.Clamp(_settings.TextSpeed, 0, TextSpeeds.Length - 1);
        _cps = TextSpeeds[_settings.TextSpeed].cps;
        if (_lookRoom is null && _shotPath is null)
            DisplayServer.WindowSetMode(_settings.Fullscreen ? DisplayServer.WindowMode.Fullscreen : DisplayServer.WindowMode.Windowed);
    }

    private void OpenMenu()
    {
        if (_mode == Mode.Menu) return;
        _beforeMenu = _mode;
        _mode = Mode.Menu;
        _roomView.Controllable = false;
        _menuShade.Visible = true;
        _menu.Visible = true;
        ShowMainMenu();
    }

    private void CloseMenu()
    {
        _menu.Visible = false;
        _menuShade.Visible = false;
        _mode = _beforeMenu;
        _roomView.Controllable = _mode == Mode.Walking;
        // Give focus back to the choices, if that's where we were.
        if (_mode is Mode.Choosing or Mode.Ending && _choices.GetChildCount() > 0)
            ((Control)_choices.GetChild(0)).CallDeferred(Control.MethodName.GrabFocus);
    }

    private void ShowMainMenu() => ShowMenuPage("Paused", CloseMenu, new List<(string, Action)>
    {
        ("Continue", CloseMenu),
        ("Save game", () =>
        {
            Save("manual");
            Toast("Saved");
            CloseMenu();
        }),
        ("Load game", ShowLoadMenu),
        ("Settings", ShowSettingsMenu),
        ("Quit to desktop", () => GetTree().Quit()),
    });

    private void ShowLoadMenu()
    {
        var items = new List<(string, Action)>();
        foreach (var (slot, name) in Slots)
        {
            string path = Path.Combine(_saveDir, slot + ".json");
            if (!File.Exists(path)) continue;
            string when = File.GetLastWriteTime(path).ToString("ddd HH:mm", System.Globalization.CultureInfo.InvariantCulture);
            string s = slot;
            items.Add(($"{name}   ·   {when}", () =>
            {
                CloseMenu();
                if (Load(s)) Toast("Loaded");
            }));
        }
        if (items.Count == 0) items.Add(("No saved games yet.", ShowMainMenu));
        items.Add(("Back", ShowMainMenu));
        ShowMenuPage("Load game", ShowMainMenu, items);
    }

    private void ShowSettingsMenu() => ShowMenuPage("Settings", ShowMainMenu, new List<(string, Action)>
    {
        ($"Fullscreen:  {(_settings.Fullscreen ? "On" : "Off")}", () =>
        {
            _settings.Fullscreen = !_settings.Fullscreen;
            ApplySettings();
            SaveSettings();
            ShowSettingsMenu();
        }),
        ($"Text speed:  {TextSpeeds[_settings.TextSpeed].name}", () =>
        {
            _settings.TextSpeed = (_settings.TextSpeed + 1) % TextSpeeds.Length;
            ApplySettings();
            SaveSettings();
            ShowSettingsMenu(1);
        }),
        ("Back", ShowMainMenu),
    });

    private void ShowSettingsMenu(int focus)
    {
        ShowSettingsMenu();
        if (focus < _menuItems.GetChildCount()) FocusLater((Control)_menuItems.GetChild(focus));
    }

    private void ShowMenuPage(string title, Action back, List<(string text, Action action)> items)
    {
        _menuTitle.Text = title;
        _menuBack = back;
        foreach (var child in _menuItems.GetChildren())
        {
            _menuItems.RemoveChild(child);
            child.QueueFree();
        }
        foreach (var (text, action) in items)
        {
            var b = MakeButton(text);
            b.Pressed += action;
            _menuItems.AddChild(b);
        }
        _menu.Size = new Vector2(560, 0);
        FocusLater((Control)_menuItems.GetChild(0));
    }

    /// <summary>Focus a control next frame, if it's still there by then (pages get rebuilt).</summary>
    private static void FocusLater(Control c) =>
        Callable.From(() => { if (GodotObject.IsInstanceValid(c) && c.IsInsideTree()) c.GrabFocus(); }).CallDeferred();

    private void LayoutMenu()
    {
        if (!_menu.Visible) return;
        _menu.Size = new Vector2(560, 0);
        _menu.Position = new Vector2((1920 - _menu.Size.X) / 2, (1080 - _menu.Size.Y) / 2);
    }

    /// <summary>Esc/Start opens the menu; inside it, Esc/Start/Back goes back a page.</summary>
    private bool MenuInput(InputEvent e)
    {
        if (_mode == Mode.Menu)
        {
            if (e.IsActionPressed("menu") || e.IsActionPressed("cancel")) _menuBack?.Invoke();
            return true;
        }
        if (!e.IsActionPressed("menu")) return false;
        // Esc in a "what do you do with this?" menu still means "not now".
        if (_mode == Mode.Choosing && _view.Kind == PresentationKind.Hub && _roomView.Visible) return false;
        OpenMenu();
        return true;
    }
}
