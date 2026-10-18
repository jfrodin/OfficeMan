using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Godot;
using OfficeMan.Core;
using OfficeMan.Core.Content;

namespace OfficeMan.Game;

/// <summary>
/// Runs the story in a walkable world. In a room, the hub is the room itself: walk up to someone or
/// something and interact. Conversations open over the live scene, Deadhouse-style: a portrait
/// bottom-left, the words and choices bottom-right. Phases without a room show a scene picture.
/// </summary>
/// <remarks>
/// Controls: arrows/WASD/stick to walk, E/Enter/Space (pad A) to interact or continue, number keys,
/// mouse or d-pad + A to choose, Esc (pad B) to back out of a menu, F5 quick save, F9 quick load.
/// Command line (after --): --seed N, --shot path.png --steps N (play N steps automatically, then screenshot).
/// </remarks>
public partial class Main : Control
{
    private enum Mode { Talking, Choosing, Walking, Ending }

    private const double CharsPerSecond = 55;

    private ContentBundle _content = null!;
    private ArtManifest _art = null!;
    private Dictionary<string, RoomLayout> _rooms = null!;
    private OfficeMan.Core.Game _game = null!;
    private int _seed;

    private RoomView _roomView = null!;
    private TextureRect _scene = null!;
    private ColorRect _dim = null!;
    private PanelContainer _portraitCard = null!;
    private TextureRect _portrait = null!;
    private PanelContainer _panel = null!;
    private Label _speaker = null!;
    private RichTextLabel _text = null!;
    private VBoxContainer _choices = null!;
    private Label _more = null!;
    private PanelContainer _heard = null!;
    private Label _heardWho = null!;
    private RichTextLabel _heardText = null!;
    private RichTextLabel _typingLabel = null!;
    private string? _listener;   // who Teo is talking to: their portrait stays up (dimmed) during narration
    private PanelContainer _arrival = null!;
    private Label _arrivalText = null!;
    private VBoxContainer _prompt = null!;
    private Label _promptName = null!;
    private Label _promptAction = null!;
    private Label _lifeLabel = null!;
    private Label _toast = null!;

    private Mode _mode;
    private Presentation _view = null!;
    private List<StoryLine> _lines = new();
    private int _lineIndex;
    private bool _typing;
    private double _typed;
    private string _phase = "";
    private string? _roomId;
    private string? _portraitPath;
    private readonly Dictionary<string, string> _moods = new();
    private readonly Dictionary<string, Texture2D> _textures = new();
    private string _saveDir = "";
    private Action? _afterLines;

    private int _autoSteps = -1;
    private int _autoChoices;
    private string? _shotPath;
    private string? _lookRoom;
    private int _lookAt = -1;

    public override void _Ready()
    {
        var args = OS.GetCmdlineUserArgs();
        _seed = (int)(Time.GetTicksUsec() % int.MaxValue);
        for (int i = 0; i < args.Length; i++)
        {
            if (args[i] == "--seed" && i + 1 < args.Length) _seed = int.Parse(args[++i]);
            else if (args[i] == "--shot" && i + 1 < args.Length) _shotPath = args[++i];
            else if (args[i] == "--steps" && i + 1 < args.Length) _autoSteps = int.Parse(args[++i]);
            else if (args[i] == "--room" && i + 1 < args.Length) _lookRoom = args[++i];
            else if (args[i] == "--at" && i + 1 < args.Length) { var hm = args[++i].Split(':'); _lookAt = int.Parse(hm[0]) * 60 + int.Parse(hm[1]); }
        }

        string contentDir = ContentLocator.Find(ProjectSettings.GlobalizePath("res://"));
        _content = ContentBundle.Load(contentDir);
        _art = ArtManifest.Load(contentDir);
        _rooms = RoomLayout.LoadAll(contentDir);
        _saveDir = Path.Combine(OS.GetUserDataDir(), "saves");
        Directory.CreateDirectory(_saveDir);

        Controls.Register();
        BuildUi();
        StartLife(_seed);
        if (_lookRoom is not null) LookAtRoom(_lookRoom);
    }

    /// <summary>Review aid: show one room, empty of dialogue, at a given time of day, and screenshot it.</summary>
    private void LookAtRoom(string id)
    {
        _autoSteps = -1;
        if (_lookAt >= 0) _game.State.Minute = _lookAt;
        _roomView.Visible = true;
        _scene.Visible = false;
        _roomView.Load(_rooms[id], _game.Evaluate);
        _panel.Visible = false;
        _choices.Visible = false;
        _arrival.Visible = false;
        SetPortrait(null);
        GetTree().CreateTimer(0.8).Timeout += () =>
        {
            GetViewport().GetTexture().GetImage().SavePng(_shotPath ?? Path.Combine(OS.GetUserDataDir(), "shot.png"));
            GetTree().Quit();
        };
    }

    // ───────────────────────── layout ─────────────────────────

    private void BuildUi()
    {
        _scene = new TextureRect
        {
            ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize,
            StretchMode = TextureRect.StretchModeEnum.KeepAspectCovered,
            TextureFilter = TextureFilterEnum.Nearest,
            MouseFilter = MouseFilterEnum.Ignore,
        };
        _scene.SetAnchorsPreset(LayoutPreset.FullRect);
        AddChild(_scene);

        _roomView = new RoomView();
        AddChild(_roomView);
        _roomView.Init(_art, TextureFor);

        _dim = new ColorRect { Color = new Color(0, 0, 0, 0.28f), MouseFilter = MouseFilterEnum.Ignore };
        _dim.SetAnchorsPreset(LayoutPreset.FullRect);
        AddChild(_dim);

        // Portrait: bottom-left, over the scene.
        const float ph = 640, pw = ph * 600 / 760f;
        _portraitCard = new PanelContainer { Position = new Vector2(36, 1080 - 36 - ph - 10), Size = new Vector2(pw + 10, ph + 10), MouseFilter = MouseFilterEnum.Ignore, Visible = false };
        _portraitCard.AddThemeStyleboxOverride("panel", Box(new Color(0.08f, 0.07f, 0.08f, 0.92f), 6, 3, new Color(0.9f, 0.85f, 0.78f, 0.85f)));
        _portrait = new TextureRect { ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize, StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered, CustomMinimumSize = new Vector2(pw, ph), MouseFilter = MouseFilterEnum.Ignore };
        _portraitCard.AddChild(_portrait);
        AddChild(_portraitCard);

        // Words and choices: bottom-right.
        _panel = new PanelContainer { Position = new Vector2(620, 600), CustomMinimumSize = new Vector2(1264, 170), MouseFilter = MouseFilterEnum.Ignore };
        _panel.AddThemeStyleboxOverride("panel", Box(new Color(0.06f, 0.05f, 0.07f, 0.9f), 10, 2, new Color(1, 1, 1, 0.14f), 34, 24));
        var box = new VBoxContainer { MouseFilter = MouseFilterEnum.Ignore };
        box.AddThemeConstantOverride("separation", 10);
        _speaker = MakeLabel(28, new Color(1f, 0.78f, 0.55f));
        _text = new RichTextLabel { BbcodeEnabled = true, FitContent = true, ScrollActive = false, CustomMinimumSize = new Vector2(1190, 60), MouseFilter = MouseFilterEnum.Ignore };
        _text.AddThemeFontSizeOverride("normal_font_size", 30);
        _text.AddThemeFontSizeOverride("italics_font_size", 30);
        _choices = new VBoxContainer();
        _choices.AddThemeConstantOverride("separation", 6);
        box.AddChild(_speaker);
        box.AddChild(_text);
        box.AddChild(_choices);
        _panel.AddChild(box);
        AddChild(_panel);

        _more = MakeLabel(24, new Color(1, 1, 1, 0.55f));
        _more.Text = "▼";
        _more.Position = new Vector2(1846, 1004);
        AddChild(_more);

        // Overheard speech: a compact caption at the top, no portrait, no dialogue panel.
        _heard = new PanelContainer { Visible = false, MouseFilter = MouseFilterEnum.Ignore };
        _heard.AddThemeStyleboxOverride("panel", Box(new Color(0.04f, 0.04f, 0.05f, 0.72f), 8, 0, default, 26, 14));
        var heardBox = new VBoxContainer { MouseFilter = MouseFilterEnum.Ignore };
        _heardWho = MakeLabel(20, new Color(0.7f, 0.76f, 0.86f, 0.9f));
        _heardText = new RichTextLabel { BbcodeEnabled = true, FitContent = true, ScrollActive = false, AutowrapMode = TextServer.AutowrapMode.Off, MouseFilter = MouseFilterEnum.Ignore };
        _heardText.AddThemeFontSizeOverride("italics_font_size", 30);
        _heardText.AddThemeFontSizeOverride("normal_font_size", 30);
        _heardText.AddThemeColorOverride("default_color", new Color(0.92f, 0.9f, 0.86f));
        heardBox.AddChild(_heardWho);
        heardBox.AddChild(_heardText);
        _heard.AddChild(heardBox);
        AddChild(_heard);
        _typingLabel = _text;

        _arrival = new PanelContainer { Position = new Vector2(36, 30), MouseFilter = MouseFilterEnum.Ignore };
        _arrival.AddThemeStyleboxOverride("panel", Box(new Color(0, 0, 0, 0.55f), 12, 0, default, 18, 10));
        _arrivalText = MakeLabel(26, new Color(1f, 0.92f, 0.75f));
        _arrival.AddChild(_arrivalText);
        AddChild(_arrival);

        // Interaction prompt, bottom-left, as in Deadhouse: who, then what.
        _prompt = new VBoxContainer { Position = new Vector2(40, 968), MouseFilter = MouseFilterEnum.Ignore };
        _promptName = MakeLabel(26, new Color(0.75f, 0.85f, 1f));
        _promptAction = MakeLabel(24, new Color(1, 1, 1, 0.85f));
        _prompt.AddChild(_promptName);
        _prompt.AddChild(_promptAction);
        AddChild(_prompt);

        _lifeLabel = MakeLabel(16, new Color(1, 1, 1, 0.35f));
        _lifeLabel.Position = new Vector2(1790, 8);
        AddChild(_lifeLabel);
        _toast = MakeLabel(22, new Color(1, 1, 1, 0.85f));
        _toast.Position = new Vector2(1600, 40);
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

    // ───────────────────────── story flow ─────────────────────────

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
        // A restored save starts mid-phase without an arrival marker; arrive directly.
        bool arrives = view.Lines.Any(l => l.Kind == LineKind.Header);
        if (!arrives && view.PhaseId != _phase) EnterPhase(view.PhaseId, view.PhaseTitle, view.When);
        else if (_roomId is not null && _roomView.Visible) _roomView.Refresh(_game.Evaluate);
        _roomView.Drunk = OfficeMan.Core.World.Teo.Drunk(_game.State);
        PlayLines(view.Lines, AfterStoryLines);
    }

    private void EnterPhase(string phaseId, string title, string when)
    {
        _phase = phaseId;
        _roomId = _art.RoomFor(_phase);
        bool room = _roomId is not null && _rooms.ContainsKey(_roomId);
        _roomView.Visible = room;
        _scene.Visible = !room;
        if (room) _roomView.Load(_rooms[_roomId!], _game.Evaluate);
        else SetScene(_art.Scene(_phase));
        SetPortrait(null);
        _listener = null;
        _moods.Clear();
        Arrive(string.IsNullOrEmpty(title) ? when : $"{title}  ·  {when}");
        SetMode(_mode); // refresh the dim layer for the new kind of place
    }

    /// <summary>What happens once the story's lines have been read.</summary>
    private void AfterStoryLines()
    {
        switch (_view.Kind)
        {
            case PresentationKind.End:
                ShowEnding();
                break;
            case PresentationKind.Hub when _roomView.Visible:
                StartWalking();
                break;
            default:
                // Dialogue choices, or a hub in a place we can't walk yet: choose from a list.
                OpenChoices(_view.Choices.Select(c => (Clean(c.Text) + (c.Tags.Contains("exit") ? "   →" : ""), c.Index)).ToList(), allowBack: false);
                break;
        }
    }

    private void PlayLines(IReadOnlyList<StoryLine> lines, Action after)
    {
        _lines = lines.ToList();
        _afterLines = after;
        _lineIndex = -1;
        ClearChoices();
        if (_lines.Count == 0)
        {
            after();
            return;
        }
        SetMode(Mode.Talking);
        NextLine();
    }

    private void NextLine()
    {
        if (++_lineIndex >= _lines.Count)
        {
            var after = _afterLines;
            _afterLines = null;
            after?.Invoke();
            return;
        }
        var line = _lines[_lineIndex];
        if (line.Kind == LineKind.Header)
        {
            EnterPhase(Tag(line, "enter") ?? _view.PhaseId, Tag(line, "title") ?? "", Tag(line, "daypart") ?? _view.When);
            NextLine();
            return;
        }
        string? scene = Tag(line, "scene");
        if (scene is not null && !_roomView.Visible) SetScene(_art.Scene(_phase, scene));

        string? speaker = line.Speaker;
        string? heardFrom = HeardFrom(line);
        if (speaker is not null && heardFrom is not null)
        {
            ShowOverheard(speaker, heardFrom, line.Body);
            return;
        }
        _heard.Visible = false;
        _panel.Visible = true;

        if (speaker is not null && speaker != "TEO")
        {
            // Someone speaking to Teo: their portrait if they have one, otherwise just their name.
            string? mood = Tag(line, "mood");
            if (mood is not null) _moods[speaker] = mood;
            _listener = _art.HasPortraits(speaker) ? speaker : null;
            SetPortrait(_listener is null ? null : _art.Portrait(speaker, _moods.GetValueOrDefault(speaker)));
            DimPortrait(false);
        }
        else if (speaker is null)
        {
            // Teo's thoughts and descriptions: the listener's portrait (if any) dims.
            DimPortrait(true);
        }
        else DimPortrait(false);

        _speaker.Visible = speaker is not null;
        _speaker.Text = speaker is null ? "" : Title(speaker);
        _text.AddThemeColorOverride("default_color", speaker is null ? new Color(0.78f, 0.82f, 0.9f) : new Color(0.98f, 0.95f, 0.9f));
        _text.Text = speaker is null ? "[i]" + Bbcode(line.Body) + "[/i]" : Bbcode(line.Body);
        StartTyping(_text);
    }

    private void ShowOverheard(string speaker, string from, string body)
    {
        _panel.Visible = false;
        if (_listener is null) SetPortrait(null);
        else DimPortrait(true);
        _heardWho.Text = $"{Title(speaker).ToUpperInvariant()}  ·  {from}";
        _heardText.Text = "[i]" + Bbcode(body) + "[/i]";
        _heard.Visible = true;
        StartTyping(_heardText);
    }

    private void StartTyping(RichTextLabel label)
    {
        _typingLabel = label;
        label.VisibleCharacters = 0;
        _typed = 0;
        _typing = true;
        _more.Visible = false;
    }

    private void DimPortrait(bool dim) =>
        _portrait.Modulate = dim ? new Color(0.5f, 0.5f, 0.56f) : Colors.White;

    /// <summary>"heard" or "heard:through the wall" marks a line as overheard, not spoken to Teo.</summary>
    private static string? HeardFrom(StoryLine line)
    {
        foreach (string t in line.Tags)
        {
            string s = t.Trim();
            if (s.Equals("heard", StringComparison.OrdinalIgnoreCase)) return "overheard";
            if (s.StartsWith("heard:", StringComparison.OrdinalIgnoreCase)) return s[6..].Trim();
        }
        return null;
    }

    private void Continue()
    {
        if (_mode != Mode.Talking) return;
        if (_typing)
        {
            _typing = false;
            _typingLabel.VisibleRatio = 1;
            _more.Visible = _panel.Visible;
            return;
        }
        NextLine();
    }

    private void OpenChoices(List<(string text, int index)> options, bool allowBack)
    {
        SetMode(Mode.Choosing);
        if (_view.Kind == PresentationKind.Hub)
        {
            if (!allowBack) _speaker.Visible = false;
            _text.Text = !_roomView.Visible ? "[color=#c8c0b0]What now?[/color]" : "";
            _text.VisibleRatio = 1;
        }
        int n = 1;
        foreach (var (text, index) in options)
        {
            int i = index;
            AddChoice($"{n++}.  {text}", () => Choose(i));
        }
        if (allowBack) AddChoice("Not now.", StartWalking);
        if (_choices.GetChildCount() > 0) ((Control)_choices.GetChild(0)).CallDeferred(Control.MethodName.GrabFocus);
    }

    private void Choose(int index)
    {
        ClearChoices();
        var next = _game.Choose(index);
        Save("autosave");
        Show(next);
    }

    // ───────────────────────── walking ─────────────────────────

    private void StartWalking()
    {
        ClearChoices();
        _listener = null;
        _heard.Visible = false;
        SetPortrait(null);
        SetMode(Mode.Walking);
    }

    private void Interact()
    {
        var thing = _roomView.Reachable();
        if (thing is null) return;
        var options = Options(thing);
        _roomView.FaceTeo(thing.AnchorId);
        if (options.Count == 1)
        {
            Choose(options[0].index);
            return;
        }
        if (options.Count > 1)
        {
            _speaker.Visible = true;
            _speaker.Text = thing.Name;
            OpenChoices(options, allowBack: true);
            return;
        }
        if (thing.Look is not null)
        {
            string time = _view.Clock.Split(' ').Last();
            PlayLines(new[] { new StoryLine(thing.Look.Replace("{clock}", time), Array.Empty<string>()) }, StartWalking);
        }
    }

    private List<(string text, int index)> Options(RoomLayout.RoomThing thing) =>
        _view.Choices.Where(c => c.Tags.Contains("where:" + thing.AnchorId))
            .Select(c => (Clean(c.Text) + (c.Tags.Contains("exit") ? "   →" : ""), c.Index)).ToList();

    private void UpdatePrompt()
    {
        var thing = _mode == Mode.Walking ? _roomView.Reachable() : null;
        if (thing is null)
        {
            _prompt.Visible = false;
            return;
        }
        var options = Options(thing);
        string action = options.Count switch
        {
            0 when thing.Look is not null => "Look",
            0 => "",
            1 => options[0].text,
            _ => $"{options.Count} things",
        };
        _prompt.Visible = action.Length > 0;
        _promptName.Text = thing.Name.ToUpperInvariant();
        _promptAction.Text = $"[E]  {action}";
    }

    // ───────────────────────── modes and input ─────────────────────────

    private void SetMode(Mode mode)
    {
        _mode = mode;
        bool overlay = mode != Mode.Walking;
        _panel.Visible = overlay;
        if (mode != Mode.Talking) _heard.Visible = false;
        _dim.Visible = overlay && _roomView.Visible;
        _roomView.Controllable = mode == Mode.Walking;
        _more.Visible = false;
        if (mode == Mode.Walking) _prompt.Visible = false;
    }

    public override void _UnhandledInput(InputEvent e)
    {
        if (e.IsActionPressed("quicksave")) { Save("quicksave"); Toast("Saved"); return; }
        if (e.IsActionPressed("quickload")) { if (Load("quicksave")) Toast("Loaded"); return; }

        switch (_mode)
        {
            case Mode.Talking when e.IsActionPressed("interact") || e is InputEventMouseButton { Pressed: true, ButtonIndex: MouseButton.Left }:
                Continue();
                break;
            case Mode.Walking when e.IsActionPressed("interact"):
                Interact();
                break;
            case Mode.Choosing when e.IsActionPressed("cancel") && _view.Kind == PresentationKind.Hub && _roomView.Visible:
                StartWalking();
                break;
            case Mode.Choosing when e is InputEventKey { Pressed: true, Echo: false } k && k.Keycode >= Key.Key1 && k.Keycode <= Key.Key9:
                int i = (int)(k.Keycode - Key.Key1);
                if (i < _choices.GetChildCount()) ((Button)_choices.GetChild(i)).EmitSignal(BaseButton.SignalName.Pressed);
                break;
        }
    }

    public override void _Process(double delta)
    {
        if (_typing)
        {
            _typed += delta * CharsPerSecond;
            _typingLabel.VisibleCharacters = (int)_typed;
            if (_typed >= _typingLabel.GetTotalCharacterCount())
            {
                _typing = false;
                _typingLabel.VisibleRatio = 1;
                _more.Visible = _panel.Visible;
            }
        }
        // The panel hugs its content and sits on the bottom edge, leaving the room visible above it.
        // With a portrait the panel sits to its right; without one it is centred.
        bool beside = _portraitCard.Visible;
        float width = beside ? 1264 : 1180, left = beside ? 620 : (1920 - 1180) / 2f;
        _panel.CustomMinimumSize = new Vector2(width, 170);
        _panel.Size = new Vector2(width, 0);
        _panel.Position = new Vector2(left, 1044 - _panel.Size.Y);
        _more.Position = new Vector2(left + width - 38, 1004);
        _heard.Size = Vector2.Zero;
        _heard.Position = new Vector2((1920 - _heard.Size.X) / 2, 96);
        UpdatePrompt();
        if (_autoSteps >= 0) RunAutomation();
    }

    private void RunAutomation()
    {
        if (_autoSteps > 0)
        {
            _autoSteps--;
            switch (_mode)
            {
                case Mode.Talking:
                    if (_typing) Continue();
                    Continue();
                    break;
                case Mode.Choosing when _view.Kind != PresentationKind.End:
                    var buttons = _choices.GetChildren().OfType<Button>().ToList();
                    int pick = Rng.Range(_seed, $"auto:{_autoChoices++}", 0, Math.Max(1, buttons.Count));
                    buttons[pick].EmitSignal(BaseButton.SignalName.Pressed);
                    break;
                case Mode.Walking:
                    int c = Rng.Range(_seed, $"auto:{_autoChoices++}", 0, _view.Choices.Count);
                    var choice = _view.Choices[c];
                    string? where = choice.Tags.FirstOrDefault(t => t.StartsWith("where:"))?[6..];
                    if (where is not null) _roomView.StandNear(where);
                    Choose(choice.Index);
                    break;
            }
            return;
        }
        if (_typing) Continue();
        _autoSteps = -1;
        GetTree().CreateTimer(0.4).Timeout += () =>
        {
            GetViewport().GetTexture().GetImage().SavePng(_shotPath ?? Path.Combine(OS.GetUserDataDir(), "shot.png"));
            GetTree().Quit();
        };
    }

    // ───────────────────────── ending ─────────────────────────

    private void ShowEnding()
    {
        SetMode(Mode.Ending);
        SetPortrait(null);
        _speaker.Visible = true;
        _speaker.Text = "To be continued";
        _text.Text = string.Join("\n", _view.Headlines.Select(h => "·  " + h).DefaultIfEmpty("Nothing much happened. That's also a life."));
        _text.VisibleRatio = 1;
        AddChoice("Live another Tuesday", () => StartLife((int)(Time.GetTicksUsec() % int.MaxValue)));
        AddChoice("Quit", () => GetTree().Quit());
        ((Control)_choices.GetChild(0)).CallDeferred(Control.MethodName.GrabFocus);
    }

    // ───────────────────────── choices ─────────────────────────

    private void AddChoice(string text, Action onPressed)
    {
        var b = new Button { Text = text, Alignment = HorizontalAlignment.Left, FocusMode = FocusModeEnum.All };
        b.AddThemeFontSizeOverride("font_size", 27);
        b.AddThemeColorOverride("font_color", new Color(0.93f, 0.9f, 0.84f));
        b.AddThemeColorOverride("font_hover_color", new Color(1f, 0.82f, 0.55f));
        b.AddThemeColorOverride("font_focus_color", new Color(1f, 0.82f, 0.55f));
        var normal = Box(new Color(1, 1, 1, 0.03f), 6, 0, default, 14, 6);
        var hot = Box(new Color(1f, 0.8f, 0.55f, 0.12f), 6, 0, default, 14, 6);
        b.AddThemeStyleboxOverride("normal", normal);
        b.AddThemeStyleboxOverride("hover", hot);
        b.AddThemeStyleboxOverride("focus", hot);
        b.AddThemeStyleboxOverride("pressed", hot);
        b.Pressed += onPressed;
        _choices.AddChild(b);
    }

    private void ClearChoices()
    {
        foreach (var child in _choices.GetChildren())
        {
            _choices.RemoveChild(child);
            child.QueueFree();
        }
    }

    // ───────────────────────── art ─────────────────────────

    private void SetScene(string path) => _scene.Texture = TextureFor(path);

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
        t = ImageTexture.CreateFromImage(Image.LoadFromFile(path));
        _textures[path] = t;
        return t;
    }

    private void Arrive(string text)
    {
        _arrivalText.Text = text;
        _arrival.Modulate = Colors.White;
        CreateTween().TweenProperty(_arrival, "modulate:a", 0.0, 1.5).SetDelay(4);
    }

    private void Toast(string text)
    {
        _toast.Text = text;
        _toast.Modulate = Colors.White;
        CreateTween().TweenProperty(_toast, "modulate:a", 0.0, 1.2).SetDelay(0.8);
    }

    // ───────────────────────── saves ─────────────────────────

    private void Save(string slot) => File.WriteAllText(Path.Combine(_saveDir, slot + ".json"), _game.Save().ToJson());

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

    private static string Bbcode(string text) => text.Replace("[", "[lb]").Replace("<i>", "[i]").Replace("</i>", "[/i]");

    private static string Clean(string text) => text.Replace("<i>", "").Replace("</i>", "");
}
