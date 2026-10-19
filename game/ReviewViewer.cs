using System.Collections.Generic;
using System.IO;
using Godot;

namespace OfficeMan.Game;

/// <summary>
/// Review aid: shows the screenshots Review.bat captured, one at a time, full screen, with a label.
/// ← → (or the d-pad) to step, Esc to close. Reads a list file of lines "file;label".
/// </summary>
public partial class ReviewViewer : Control
{
    private readonly List<(Texture2D image, string label)> _shots = new();
    private TextureRect _picture = null!;
    private Label _caption = null!;
    private int _index;

    public ReviewViewer(string listFile)
    {
        string dir = Path.GetDirectoryName(listFile) ?? ".";
        foreach (string raw in File.ReadAllLines(listFile))
        {
            string line = raw.Trim();
            int split = line.IndexOf(';');
            if (split <= 0) continue;
            string path = Path.Combine(dir, line[..split].Trim());
            if (!File.Exists(path)) continue;
            var image = Image.LoadFromFile(path);
            if (image is not null) _shots.Add((ImageTexture.CreateFromImage(image), line[(split + 1)..].Trim()));
        }
    }

    public override void _Ready()
    {
        SetAnchorsPreset(LayoutPreset.FullRect);
        var backdrop = new ColorRect { Color = new Color(0.05f, 0.05f, 0.06f) };
        backdrop.SetAnchorsPreset(LayoutPreset.FullRect);
        AddChild(backdrop);

        _picture = new TextureRect { ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize, StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered };
        _picture.SetAnchorsPreset(LayoutPreset.FullRect);
        AddChild(_picture);

        var bar = new PanelContainer { Position = new Vector2(0, 1080 - 64), Size = new Vector2(1920, 64) };
        bar.AddThemeStyleboxOverride("panel", new StyleBoxFlat { BgColor = new Color(0, 0, 0, 0.75f), ContentMarginLeft = 28, ContentMarginTop = 14 });
        _caption = new Label();
        _caption.AddThemeFontSizeOverride("font_size", 26);
        _caption.AddThemeColorOverride("font_color", new Color(1f, 0.92f, 0.78f));
        bar.AddChild(_caption);
        AddChild(bar);
        Show(0);
    }

    private void Show(int index)
    {
        if (_shots.Count == 0)
        {
            _caption.Text = "No screenshots found. Run Review.bat first.";
            return;
        }
        _index = (index + _shots.Count) % _shots.Count;
        _picture.Texture = _shots[_index].image;
        _caption.Text = $"{_index + 1} / {_shots.Count}   ·   {_shots[_index].label}        ← →  to look through   ·   Esc to close";
    }

    public override void _UnhandledInput(InputEvent e)
    {
        if (e.IsActionPressed("ui_right") || e.IsActionPressed("move_right") || e.IsActionPressed("interact")) Show(_index + 1);
        else if (e.IsActionPressed("ui_left") || e.IsActionPressed("move_left")) Show(_index - 1);
        else if (e.IsActionPressed("menu") || e.IsActionPressed("cancel")) GetTree().Quit();
    }
}
