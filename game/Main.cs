using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Godot;
using OfficeMan.Core;
using OfficeMan.Core.Content;

namespace OfficeMan.Game;

/// <summary>
/// The dialogue screen: location backdrop, a large portrait of whoever is speaking, a text panel that
/// types at reading pace, and choices. Everything it shows comes from the engine's Presentation and
/// the art manifest; it knows nothing about the story.
/// </summary>
/// <remarks>
/// Controls: click / Space / Enter to continue (or finish the line), 1–9 to choose, F5 quick save, F9 quick load.
/// Command line (after --): --seed N, --shot path.png --steps N (play N steps automatically, save a screenshot, quit).
/// </remarks>
public partial class Main : Control
{
    private const double CharsPerSecond = 55;
    private const float PortraitHeight = 820;

    private ContentBundle _content = null!;
    private ArtManifest _art = null!;
    private OfficeMan.Core.Game _game = null!;
    private int _seed;

    private TextureRect _backdrop = null!;
    private PanelContainer _portraitCard = null!;
    private TextureRect _portrait = null!;
    private Label _location = null!;
    private Label _lifeLabel = null!;
    private Label _speaker = null!;
    private RichTextLabel _text = null!;
    private Label _more = null!;
    private VBoxContainer _choices = null!;
    private Label _toast = null!;

    private Presentation _view = null!;
    private int _lineIndex;
    private bool _typing;
    private double _typed;
    private string _phase = "";
    private string? _portraitPath;
    private readonly Dictionary<string, string> _moods = new();
    private readonly Dictionary<string, Texture2D> _textures = new();
    private string _saveDir = "";

    // Automation for screenshots.
    private int _autoSteps = -1;
    private int _autoChoices;
    private string? _shotPath;

    public override void _Ready()
    {
        var args = OS.GetCmdlineUserArgs();
        _seed = (int)(Time.GetTicksUsec() % int.MaxValue);
        for (int i = 0; i < args.Length; i++)
        {
            if (args[i] == "--seed" && i + 1 < args.Length) _seed = int.Parse(args[++i]);
            else if (args[i] == "--shot" && i + 1 < args.Length) _shotPath = args[++i];
            else if (args[i] == "--steps" && i + 1 < args.Length) _autoSteps = int.Parse(args[++i]);
        }

        string contentDir = ContentLocator.Find(ProjectSettings.GlobalizePath("res://"));
        _content = ContentBundle.Load(contentDir);
        _art = ArtManifest.Load(contentDir);
        _saveDir = Path.Combine(OS.GetUserDataDir(), "saves");
        Directory.CreateDirectory(_saveDir);

        BuildUi();
        StartLife(_seed);
    }

    // ───────────────────────── layout ─────────────────────────

    private void BuildUi()
    {
        _backdrop = new TextureRect
        {
            ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize,
            StretchMode = TextureRect.StretchModeEnum.KeepAspectCovered,
            MouseFilter = MouseFilterEnum.Ignore,
        };
        _backdrop.SetAnchorsPreset(LayoutPreset.FullRect);
        AddChild(_backdrop);

        // A soft darkening at the bottom so text reads on any backdrop.
        var shade = new TextureRect
        {
            Texture = VerticalShade(),
            ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize,
            StretchMode = TextureRect.StretchModeEnum.Scale,
            MouseFilter = MouseFilterEnum.Ignore,
        };
        shade.SetAnchorsPreset(LayoutPreset.FullRect);
        AddChild(shade);

        _portraitCard = new PanelContainer { MouseFilter = MouseFilterEnum.Ignore, Visible = false };
        _portraitCard.AddThemeStyleboxOverride("panel", Box(new Color(0.08f, 0.07f, 0.08f, 0.9f), 6, 4, new Color(0.9f, 0.85f, 0.78f, 0.9f)));
        _portraitCard.Position = new Vector2(1920 - 60 - PortraitHeight * 600 / 760f - 12, 1080 - 40 - PortraitHeight - 12);
        _portraitCard.Size = new Vector2(PortraitHeight * 600 / 760f + 12, PortraitHeight + 12);
        _portrait = new TextureRect
        {
            ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize,
            StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered,
            CustomMinimumSize = new Vector2(PortraitHeight * 600 / 760f, PortraitHeight),
            MouseFilter = MouseFilterEnum.Ignore,
        };
        _portraitCard.AddChild(_portrait);
        AddChild(_portraitCard);

        var header = new PanelContainer { Position = new Vector2(40, 32), MouseFilter = MouseFilterEnum.Ignore };
        header.AddThemeStyleboxOverride("panel", Box(new Color(0, 0, 0, 0.55f), 14, 0, default, 18, 10));
        _location = MakeLabel(26, new Color(1f, 0.92f, 0.75f));
        header.AddChild(_location);
        AddChild(header);

        _lifeLabel = MakeLabel(18, new Color(1, 1, 1, 0.45f));
        _lifeLabel.Position = new Vector2(44, 1040);
        AddChild(_lifeLabel);

        var textPanel = new PanelContainer { Position = new Vector2(60, 742), Size = new Vector2(1120, 300), MouseFilter = MouseFilterEnum.Ignore };
        textPanel.AddThemeStyleboxOverride("panel", Box(new Color(0.06f, 0.05f, 0.06f, 0.86f), 14, 2, new Color(1, 1, 1, 0.12f), 34, 22));
        var textBox = new VBoxContainer { MouseFilter = MouseFilterEnum.Ignore };
        textBox.AddThemeConstantOverride("separation", 8);
        _speaker = MakeLabel(30, new Color(1f, 0.78f, 0.55f));
        _text = new RichTextLabel
        {
            BbcodeEnabled = true,
            FitContent = false,
            ScrollActive = false,
            CustomMinimumSize = new Vector2(1040, 200),
            MouseFilter = MouseFilterEnum.Ignore,
        };
        _text.AddThemeFontSizeOverride("normal_font_size", 31);
        _text.AddThemeFontSizeOverride("italics_font_size", 31);
        _text.AddThemeColorOverride("default_color", new Color(0.95f, 0.93f, 0.9f));
        textBox.AddChild(_speaker);
        textBox.AddChild(_text);
        textPanel.AddChild(textBox);
        AddChild(textPanel);

        _more = MakeLabel(26, new Color(1, 1, 1, 0.6f));
        _more.Text = "▼";
        _more.Position = new Vector2(1140, 1000);
        AddChild(_more);

        _choices = new VBoxContainer { Position = new Vector2(60, 300), Size = new Vector2(1120, 420), Alignment = BoxContainer.AlignmentMode.End };
        _choices.AddThemeConstantOverride("separation", 10);
        AddChild(_choices);

        _toast = MakeLabel(22, new Color(1, 1, 1, 0.8f));
        _toast.Position = new Vector2(1500, 40);
        AddChild(_toast);
    }

    private static Label MakeLabel(int size, Color color)
    {
        var l = new Label { MouseFilter = MouseFilterEnum.Ignore };
        l.AddThemeFontSizeOverride("font_size", size);
        l.AddThemeColorOverride("font_color", color);
        return l;
    }

    private static StyleBoxFlat Box(Color bg, int radius, int border, Color borderColor, int padX = 0, int padY = 0)
    {
        var b = new StyleBoxFlat { BgColor = bg, BorderColor = borderColor };
        b.SetCornerRadiusAll(radius);
        b.SetBorderWidthAll(border);
        b.ContentMarginLeft = b.ContentMarginRight = padX;
        b.ContentMarginTop = b.ContentMarginBottom = padY;
        return b;
    }

    private static Texture2D VerticalShade()
    {
        var gradient = new Gradient();
        gradient.SetColor(0, new Color(0, 0, 0, 0));
        gradient.SetColor(1, new Color(0, 0, 0, 0.55f));
        gradient.SetOffset(0, 0.45f);
        return new GradientTexture2D { Gradient = gradient, FillFrom = new Vector2(0, 0), FillTo = new Vector2(0, 1), Width = 4, Height = 256 };
    }

    // ───────────────────────── flow ─────────────────────────

    private void StartLife(int seed)
    {
        _seed = seed;
        _game = OfficeMan.Core.Game.New(_content, seed);
        _phase = "";
        _moods.Clear();
        _lifeLabel.Text = $"life #{seed}";
        Show(_game.Current);
    }

    private void Show(Presentation view)
    {
        _view = view;
        if (view.PhaseId != _phase)
        {
            _phase = view.PhaseId;
            SetBackdrop(_art.Scene(_phase));
            SetPortrait(null);
        }
        if (!string.IsNullOrEmpty(view.PhaseTitle)) _location.Text = $"{view.PhaseTitle}  ·  {view.Clock}";
        ClearChoices();
        _lineIndex = -1;
        Advance();
    }

    /// <summary>Finishes the current line, or moves to the next one, or shows the choices.</summary>
    private void Advance()
    {
        if (_typing)
        {
            _typing = false;
            _text.VisibleRatio = 1;
            _more.Visible = true;
            return;
        }
        if (_choices.GetChildCount() > 0) return;

        while (++_lineIndex < _view.Lines.Count)
        {
            var line = _view.Lines[_lineIndex];
            if (line.Kind == LineKind.Header)
            {
                _location.Text = line.Text;
                continue;
            }
            ShowLine(line);
            return;
        }
        ShowChoices();
    }

    private void ShowLine(StoryLine line)
    {
        string? scene = Tag(line, "scene");
        if (scene is not null) SetBackdrop(_art.Scene(_phase, scene));

        string? speaker = line.Speaker;
        string? mood = Tag(line, "mood");
        if (speaker is not null && speaker != "TEO")
        {
            if (mood is not null) _moods[speaker] = mood;
            SetPortrait(_art.Portrait(speaker, _moods.GetValueOrDefault(speaker)));
        }

        _speaker.Visible = speaker is not null;
        _speaker.Text = speaker is null ? "" : Title(speaker);
        _text.Text = Bbcode(line.Body);
        _text.AddThemeColorOverride("default_color", speaker is null ? new Color(0.86f, 0.88f, 0.92f) : new Color(0.97f, 0.95f, 0.9f));
        _text.VisibleCharacters = 0;
        _typed = 0;
        _typing = true;
        _more.Visible = false;
    }

    private void ShowChoices()
    {
        _more.Visible = false;
        if (_view.Kind == PresentationKind.End)
        {
            ShowEnding();
            return;
        }
        if (_view.Kind == PresentationKind.Hub)
        {
            SetPortrait(null); // nobody is talking; show the room
            _speaker.Visible = false;
            _text.Text = $"[color=#c8c0b0]{_view.Clock}. What now?[/color]";
            _text.VisibleRatio = 1;
        }
        for (int i = 0; i < _view.Choices.Count; i++)
        {
            var c = _view.Choices[i];
            int index = i;
            string arrow = c.Tags.Contains("exit") ? "   →" : "";
            AddChoice($"{i + 1}.  {StripTags(c.Text)}{arrow}", () => Choose(index));
        }
    }

    private void ShowEnding()
    {
        SetPortrait(null);
        _speaker.Visible = true;
        _speaker.Text = "To be continued";
        var lines = _view.Headlines.Select(h => "·  " + h);
        _text.Text = string.Join("\n", lines.DefaultIfEmpty("Nothing much happened. That's also a life."));
        _text.VisibleRatio = 1;
        AddChoice("Live another Tuesday", () => StartLife((int)(Time.GetTicksUsec() % int.MaxValue)));
        AddChoice("Quit", () => GetTree().Quit());
    }

    private void Choose(int index)
    {
        ClearChoices();
        var next = _game.Choose(index);
        Save("autosave");
        Show(next);
    }

    private void AddChoice(string text, Action onPressed)
    {
        var b = new Button { Text = text, Alignment = HorizontalAlignment.Left, FocusMode = FocusModeEnum.None };
        b.AddThemeFontSizeOverride("font_size", 28);
        b.AddThemeColorOverride("font_color", new Color(0.97f, 0.94f, 0.88f));
        b.AddThemeColorOverride("font_hover_color", new Color(1f, 0.82f, 0.55f));
        b.AddThemeStyleboxOverride("normal", Box(new Color(0.07f, 0.06f, 0.07f, 0.82f), 10, 1, new Color(1, 1, 1, 0.12f), 22, 10));
        b.AddThemeStyleboxOverride("hover", Box(new Color(0.16f, 0.11f, 0.09f, 0.92f), 10, 1, new Color(1f, 0.8f, 0.55f, 0.6f), 22, 10));
        b.AddThemeStyleboxOverride("pressed", Box(new Color(0.22f, 0.14f, 0.1f, 0.95f), 10, 1, new Color(1f, 0.8f, 0.55f, 0.8f), 22, 10));
        b.Pressed += onPressed;
        _choices.AddChild(b);
    }

    private void ClearChoices()
    {
        foreach (var child in _choices.GetChildren()) child.QueueFree();
        // QueueFree is deferred; detach so GetChildCount reflects the new state immediately.
        foreach (var child in _choices.GetChildren()) _choices.RemoveChild(child);
    }

    // ───────────────────────── input ─────────────────────────

    public override void _UnhandledInput(InputEvent e)
    {
        if (e is InputEventMouseButton { Pressed: true, ButtonIndex: MouseButton.Left })
            Advance();
        else if (e is InputEventKey { Pressed: true, Echo: false } k)
        {
            switch (k.Keycode)
            {
                case Key.Space or Key.Enter or Key.KpEnter:
                    Advance();
                    break;
                case >= Key.Key1 and <= Key.Key9:
                    int i = (int)(k.Keycode - Key.Key1);
                    if (i < _choices.GetChildCount() && !_typing) ((Button)_choices.GetChild(i)).EmitSignal(BaseButton.SignalName.Pressed);
                    break;
                case Key.F5:
                    Save("quicksave");
                    Toast("Saved");
                    break;
                case Key.F9:
                    if (Load("quicksave")) Toast("Loaded");
                    break;
            }
        }
    }

    public override void _Process(double delta)
    {
        if (_typing)
        {
            _typed += delta * CharsPerSecond;
            int total = _text.GetTotalCharacterCount();
            _text.VisibleCharacters = (int)_typed;
            if (_typed >= total)
            {
                _typing = false;
                _text.VisibleRatio = 1;
                _more.Visible = true;
            }
        }
        if (_autoSteps >= 0) RunAutomation();
    }

    private void RunAutomation()
    {
        if (_autoSteps > 0)
        {
            _autoSteps--;
            if (_typing) Advance();
            if (_choices.GetChildCount() > 0 && _view.Kind != PresentationKind.End)
            {
                int pick = Rng.Range(_seed, $"shot:{_autoChoices++}", 0, _choices.GetChildCount());
                Choose(Math.Min(pick, _view.Choices.Count - 1));
            }
            else Advance();
            return;
        }
        if (_typing) Advance();
        _autoSteps = -1;
        // Wait two frames for the layout to settle, then capture.
        GetTree().CreateTimer(0.3).Timeout += () =>
        {
            GetViewport().GetTexture().GetImage().SavePng(_shotPath ?? Path.Combine(OS.GetUserDataDir(), "shot.png"));
            GetTree().Quit();
        };
    }

    // ───────────────────────── art ─────────────────────────

    private void SetBackdrop(string path) => _backdrop.Texture = TextureFor(path);

    private void SetPortrait(string? path)
    {
        if (path == _portraitPath) return;
        _portraitPath = path;
        if (path is null)
        {
            _portraitCard.Visible = false;
            return;
        }
        _portrait.Texture = TextureFor(path);
        _portraitCard.Visible = true;
        _portraitCard.Modulate = new Color(1, 1, 1, 0);
        CreateTween().TweenProperty(_portraitCard, "modulate:a", 1.0, 0.18);
    }

    private Texture2D TextureFor(string path)
    {
        if (_textures.TryGetValue(path, out var t)) return t;
        var image = Image.LoadFromFile(path);
        t = ImageTexture.CreateFromImage(image);
        _textures[path] = t;
        return t;
    }

    // ───────────────────────── saves ─────────────────────────

    private void Save(string slot) =>
        File.WriteAllText(Path.Combine(_saveDir, slot + ".json"), _game.Save().ToJson());

    private bool Load(string slot)
    {
        string path = Path.Combine(_saveDir, slot + ".json");
        if (!File.Exists(path)) return false;
        _game = OfficeMan.Core.Game.Load(_content, SaveGame.FromJson(File.ReadAllText(path)));
        _seed = _game.State.Seed;
        _lifeLabel.Text = $"life #{_seed}";
        _phase = "";
        Show(_game.Current);
        return true;
    }

    private void Toast(string text)
    {
        _toast.Text = text;
        _toast.Modulate = Colors.White;
        CreateTween().TweenProperty(_toast, "modulate:a", 0.0, 1.2).SetDelay(0.8);
    }

    // ───────────────────────── text helpers ─────────────────────────

    private static string? Tag(StoryLine line, string key)
    {
        foreach (string t in line.Tags)
        {
            int colon = t.IndexOf(':');
            if (colon > 0 && t[..colon].Trim().Equals(key, StringComparison.OrdinalIgnoreCase)) return t[(colon + 1)..].Trim();
        }
        return null;
    }

    private static string Title(string name) =>
        string.Join(' ', name.Split(' ').Select(w => w.Length == 0 ? w : w[0] + w[1..].ToLowerInvariant()));

    private static string Bbcode(string text) =>
        text.Replace("[", "[lb]").Replace("<i>", "[i]").Replace("</i>", "[/i]");

    private static string StripTags(string text) => text.Replace("<i>", "").Replace("</i>", "");
}
