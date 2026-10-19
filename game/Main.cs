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
/// mouse or d-pad + A to choose, Esc (pad B) to back out of a menu, Esc (pad Start) for the pause menu,
/// F5 quick save, F9 quick load.
/// Command line (after --): --seed N, --shot path.png --steps N (play N steps automatically, then screenshot),
/// --menu main|settings|load (open the pause menu before the screenshot), --room id --at HH:MM (one room, no dialogue).
/// </remarks>
public partial class Main : Control
{
    private enum Mode { Talking, Choosing, Walking, Ending, Menu }

    private double _cps = 55;   // characters per second, from the text speed setting
    private const int BodySize = 30;
    private const float PortraitW = 250, PortraitH = 270, BarMinHeight = 210;
    private static readonly Color Accent = new(1f, 0.78f, 0.55f), Spoken = new(0.98f, 0.95f, 0.9f), Thought = new(0.78f, 0.82f, 0.9f);
    /// <summary>The part of a 600×760 portrait shown in the bar: head and shoulders.</summary>
    private static readonly Rect2 PortraitCrop = new(40, 100, 520, 562);
    /// <summary>How far the camera may rise to keep people clear of the bar (screen pixels).</summary>
    private const float MaxLift = 120;

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
    private GridContainer _choices = null!;
    private Label _more = null!;
    private PanelContainer _heard = null!;
    private Label _heardWho = null!;
    private RichTextLabel _heardText = null!;
    private MarginContainer _barMargin = null!;
    private PanelContainer _note = null!;
    private RichTextLabel _noteText = null!;
    private RichTextLabel _typingLabel = null!;
    private string? _listener;   // who Teo is talking to: their portrait stays up (dimmed) during narration
    private string? _focus;      // whoever spoke to Teo last in this conversation: the camera keeps them in view
    private bool _conversing;    // someone has spoken since Teo last walked: thoughts stay in the bar
    private string _heardFrom = "";
    private float _liftTarget;
    private string _lastNote = "";
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
    private string? _shotMenu;
    private int _lookAt = -1;

    public override void _Ready()
    {
        var args = OS.GetCmdlineUserArgs();
        if (args.Length > 2 && args[0] == "--stitch")
        {
            Stitch(args[1], args[2..]);
            GetTree().Quit();
            return;
        }
        _seed = (int)(Time.GetTicksUsec() % int.MaxValue);
        for (int i = 0; i < args.Length; i++)
        {
            if (args[i] == "--seed" && i + 1 < args.Length) _seed = int.Parse(args[++i]);
            else if (args[i] == "--shot" && i + 1 < args.Length) _shotPath = args[++i];
            else if (args[i] == "--steps" && i + 1 < args.Length) _autoSteps = int.Parse(args[++i]);
            else if (args[i] == "--room" && i + 1 < args.Length) _lookRoom = args[++i];
            else if (args[i] == "--menu" && i + 1 < args.Length) _shotMenu = args[++i];
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
        BuildMenu();
        LoadSettings();
        StartLife(_seed);
        if (_lookRoom is not null) LookAtRoom(_lookRoom);
    }

    /// <summary>Review aid: puts screenshots side by side, two to a row, each at half size.</summary>
    private static void Stitch(string output, string[] inputs)
    {
        const int tw = 960, th = 540, cols = 2;
        int rows = (inputs.Length + cols - 1) / cols;
        var sheet = Image.CreateEmpty(tw * cols + 8 * (cols + 1), th * rows + 8 * (rows + 1), false, Image.Format.Rgb8);
        sheet.Fill(new Color(0.08f, 0.08f, 0.09f));
        for (int i = 0; i < inputs.Length; i++)
        {
            var img = Image.LoadFromFile(inputs[i]);
            if (img is null) continue;
            img.Convert(Image.Format.Rgb8);
            img.Resize(tw, th, Image.Interpolation.Lanczos);
            sheet.BlitRect(img, new Rect2I(0, 0, tw, th), new Vector2I(8 + (i % cols) * (tw + 8), 8 + (i / cols) * (th + 8)));
        }
        sheet.SavePng(output);
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
        _note.Visible = false;
        _heard.Visible = false;
        _dim.Visible = false;
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

        _dim = new ColorRect { Color = new Color(0, 0, 0, 0.18f), MouseFilter = MouseFilterEnum.Ignore };
        _dim.SetAnchorsPreset(LayoutPreset.FullRect);
        AddChild(_dim);

        // Three ways text appears, one typography:
        //  conversation  the bar along the bottom: portrait, name, words, choices;
        //  observation   a compact note low in the frame, for Teo's thoughts when nobody is talking to him;
        //  overheard     a small caption placed towards where the sound comes from.

        // The conversation bar, full width, growing upwards with its content.
        _panel = new PanelContainer { MouseFilter = MouseFilterEnum.Ignore, Visible = false };
        var bar = new StyleBoxFlat { BgColor = new Color(0.045f, 0.04f, 0.055f, 0.96f), BorderColor = new Color(1f, 0.85f, 0.65f, 0.18f) };
        bar.BorderWidthTop = 2;
        _panel.AddThemeStyleboxOverride("panel", bar);
        _barMargin = new MarginContainer { MouseFilter = MouseFilterEnum.Ignore };
        _barMargin.AddThemeConstantOverride("margin_top", 26);
        _barMargin.AddThemeConstantOverride("margin_bottom", 30);
        _barMargin.AddThemeConstantOverride("margin_right", 150);
        var box = new VBoxContainer { MouseFilter = MouseFilterEnum.Ignore };
        box.AddThemeConstantOverride("separation", 10);
        _speaker = MakeLabel(BodySize - 2, Accent);
        _text = new RichTextLabel { BbcodeEnabled = true, FitContent = true, ScrollActive = false, MouseFilter = MouseFilterEnum.Ignore };
        _text.AddThemeFontSizeOverride("normal_font_size", BodySize);
        _text.AddThemeFontSizeOverride("italics_font_size", BodySize);
        // Choices: one column for one or two, two columns from three up, so the bar stays low.
        _choices = new GridContainer { Columns = 1 };
        _choices.AddThemeConstantOverride("h_separation", 12);
        _choices.AddThemeConstantOverride("v_separation", 4);
        box.AddChild(_speaker);
        box.AddChild(_text);
        box.AddChild(_choices);
        _barMargin.AddChild(box);
        _panel.AddChild(_barMargin);
        AddChild(_panel);

        // The portrait stands in the bar's left end and rises a little above it: head and shoulders only.
        _portraitCard = new PanelContainer { Size = new Vector2(PortraitW + 6, PortraitH + 6), MouseFilter = MouseFilterEnum.Ignore, Visible = false };
        _portraitCard.AddThemeStyleboxOverride("panel", Box(new Color(0.08f, 0.07f, 0.08f, 1f), 4, 3, new Color(1f, 0.85f, 0.65f, 0.55f)));
        _portrait = new TextureRect { ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize, StretchMode = TextureRect.StretchModeEnum.KeepAspectCovered, CustomMinimumSize = new Vector2(PortraitW, PortraitH), MouseFilter = MouseFilterEnum.Ignore };
        _portraitCard.AddChild(_portrait);
        AddChild(_portraitCard);

        // Observations: a compact note, centred low in the frame, no portrait, no name.
        _note = new PanelContainer { Visible = false, MouseFilter = MouseFilterEnum.Ignore };
        _note.AddThemeStyleboxOverride("panel", Box(new Color(0.03f, 0.03f, 0.045f, 0.78f), 6, 0, default, 30, 16));
        _noteText = new RichTextLabel { BbcodeEnabled = true, FitContent = true, ScrollActive = false, AutowrapMode = TextServer.AutowrapMode.WordSmart, MouseFilter = MouseFilterEnum.Ignore };
        _noteText.AddThemeFontSizeOverride("normal_font_size", BodySize - 2);
        _noteText.AddThemeFontSizeOverride("italics_font_size", BodySize - 2);
        _noteText.AddThemeColorOverride("default_color", Thought);
        _note.AddChild(_noteText);
        AddChild(_note);

        _more = MakeLabel(22, new Color(1, 1, 1, 0.5f));
        _more.Text = "▼";
        AddChild(_more);

        // Overheard speech: a small caption, no box behind the words, placed towards the sound.
        _heard = new PanelContainer { Visible = false, MouseFilter = MouseFilterEnum.Ignore };
        _heard.AddThemeStyleboxOverride("panel", Box(new Color(0.03f, 0.03f, 0.04f, 0.55f), 6, 0, default, 20, 10));
        var heardBox = new VBoxContainer { MouseFilter = MouseFilterEnum.Ignore };
        heardBox.AddThemeConstantOverride("separation", 2);
        _heardWho = MakeLabel(19, new Color(0.7f, 0.76f, 0.86f, 0.85f));
        _heardText = new RichTextLabel { BbcodeEnabled = true, FitContent = true, ScrollActive = false, AutowrapMode = TextServer.AutowrapMode.Off, MouseFilter = MouseFilterEnum.Ignore };
        _heardText.AddThemeFontSizeOverride("italics_font_size", BodySize - 4);
        _heardText.AddThemeFontSizeOverride("normal_font_size", BodySize - 4);
        _heardText.AddThemeColorOverride("default_color", new Color(0.9f, 0.88f, 0.84f));
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
        _focus = null;
        _conversing = false;
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
        if (Tag(line, "begin") is not null && _conversing)
        {
            // A new scene: whoever Teo was talking to is no longer the one in front of him.
            _conversing = false;
            _listener = null;
            _focus = null;
            ShowBar(false);
        }
        string? scene = Tag(line, "scene");
        if (scene is not null && !_roomView.Visible) SetScene(_art.Scene(_phase, scene));

        string? speaker = line.Speaker;
        string? heardFrom = HeardFrom(line);
        _heard.Visible = false;
        if (speaker is not null && heardFrom is not null)
        {
            ShowOverheard(speaker, heardFrom, Tag(line, "from") ?? "", line.Body);
            return;
        }

        if (speaker is null && !_conversing)
        {
            // Teo looking around or thinking, nobody talking to him: a compact note, the scene left clear.
            ShowNote(line.Body);
            return;
        }

        _conversing = true;
        _note.Visible = false;
        ShowBar(true);
        if (speaker is not null && speaker != "TEO")
        {
            // Someone speaking to Teo: their portrait if they have one, otherwise just their name.
            string? mood = Tag(line, "mood");
            if (mood is not null) _moods[speaker] = mood;
            _listener = _art.HasPortraits(speaker) ? speaker : null;
            _focus = speaker;
            SetPortrait(_listener is null ? null : _art.Portrait(speaker, _moods.GetValueOrDefault(speaker)));
            DimPortrait(false);
        }
        else DimPortrait(speaker is null);   // Teo's thoughts mid-conversation: the other person's portrait dims

        _speaker.Visible = speaker is not null;
        _speaker.Text = speaker is null ? "" : Title(speaker);
        _text.AddThemeColorOverride("default_color", speaker is null ? Thought : Spoken);
        _text.Text = speaker is null ? "[i]" + Bbcode(line.Body) + "[/i]" : Bbcode(line.Body);
        StartTyping(_text);
    }

    private void ShowBar(bool show)
    {
        _panel.Visible = show;
        _dim.Visible = show && _roomView.Visible;
        if (!show) SetPortrait(null);
    }

    private void ShowNote(string body)
    {
        ShowBar(false);
        _noteText.Text = _lastNote = "[i]" + Bbcode(body) + "[/i]";
        // As wide as the words need, up to a comfortable reading width; longer thoughts wrap.
        float width = ThemeDB.FallbackFont.GetStringSize(Clean(body), HorizontalAlignment.Left, -1, BodySize - 2).X;
        _noteText.CustomMinimumSize = new Vector2(Math.Min(1000, width + 12), 0);
        _note.Visible = true;
        StartTyping(_noteText);
    }

    private void ShowOverheard(string speaker, string from, string direction, string body)
    {
        // An overheard line doesn't interrupt a conversation's layout, and doesn't open one.
        if (_conversing) DimPortrait(true);
        else ShowBar(false);
        _note.Visible = false;
        _heardFrom = direction;
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
            _more.Visible = true;
            return;
        }
        NextLine();
    }

    private void OpenChoices(List<(string text, int index)> options, bool allowBack)
    {
        SetMode(Mode.Choosing);
        _note.Visible = false;
        _heard.Visible = false;
        if (!_conversing) _speaker.Visible = allowBack;
        if (!_conversing && !allowBack)
        {
            // Choices straight after an observation: the observation moves into the bar as their context.
            _text.AddThemeColorOverride("default_color", Thought);
            _text.Text = _lastNote;
            _text.VisibleRatio = 1;
        }
        ShowBar(true);
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
        _choices.Columns = _choices.GetChildCount() >= 3 ? 2 : 1;
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
        _focus = null;
        _conversing = false;
        _lastNote = "";
        _heard.Visible = false;
        _note.Visible = false;
        ShowBar(false);
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
        if (mode == Mode.Walking) ShowBar(false);
        if (mode != Mode.Talking) _heard.Visible = false;
        _roomView.Controllable = mode == Mode.Walking;
        _more.Visible = false;
        if (mode == Mode.Walking) _prompt.Visible = false;
    }

    public override void _UnhandledInput(InputEvent e)
    {
        if (e.IsActionPressed("quicksave")) { Save("quicksave"); Toast("Saved"); return; }
        if (e.IsActionPressed("quickload")) { if (Load("quicksave")) Toast("Loaded"); return; }
        if (MenuInput(e)) { GetViewport().SetInputAsHandled(); return; }

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
        LayoutMenu();
        if (_typing && _mode != Mode.Menu)
        {
            _typed += delta * _cps;
            _typingLabel.VisibleCharacters = (int)_typed;
            if (_typed >= _typingLabel.GetTotalCharacterCount())
            {
                _typing = false;
                _typingLabel.VisibleRatio = 1;
                _more.Visible = true;
            }
        }
        Layout(delta);
        UpdatePrompt();
        if (_autoSteps >= 0) RunAutomation();
    }

    /// <summary>
    /// Places the bar, the portrait, the note and the overheard caption, and moves the camera only when
    /// the bar would cover the people in the conversation.
    /// </summary>
    private void Layout(double delta)
    {
        bool portrait = _portraitCard.Visible && _panel.Visible;
        _barMargin.AddThemeConstantOverride("margin_left", portrait ? (int)PortraitW + 90 : 150);
        _text.CustomMinimumSize = new Vector2(1920 - (portrait ? PortraitW + 90 : 150) - 150, 40);
        _panel.Size = new Vector2(1920, 0);
        float barH = Math.Max(BarMinHeight, _panel.Size.Y);
        _panel.Size = new Vector2(1920, barH);
        _panel.Position = new Vector2(0, 1080 - barH);
        _portraitCard.Position = new Vector2(48, 1080 - 26 - _portraitCard.Size.Y);

        _note.Size = Vector2.Zero;
        _note.Size = Vector2.Zero;
        _note.Position = new Vector2((1920 - _note.Size.X) / 2, 1080 - 60 - _note.Size.Y);

        _heard.Size = Vector2.Zero;
        _heard.Position = _heardFrom switch
        {
            "left" => new Vector2(60, 150),
            "right" => new Vector2(1920 - 60 - _heard.Size.X, 150),
            "" => new Vector2((1920 - _heard.Size.X) / 2, 110),
            var anchor => AboveThing(anchor, _heard.Size),
        };

        var active = _panel.Visible ? (Control)_panel : _note.Visible ? _note : _heard;
        // The "more" marker sits inside the bar, and just outside the smaller boxes so it never covers words.
        _more.Position = active == _panel
            ? active.Position + active.Size - new Vector2(110, 62)
            : active.Position + new Vector2(active.Size.X + 10, active.Size.Y - 34);

        // Camera: rise just enough to keep Teo and whoever is talking to him above the bar.
        float need = 0;
        if (_panel.Visible && _roomView.Visible)
        {
            float lowest = 0;
            if (_roomView.TeoVisible) lowest = _roomView.ScreenFeet(null) ?? 0;
            if (_focus is not null) lowest = Math.Max(lowest, _roomView.ScreenFeet(_focus.ToLowerInvariant()) ?? 0);
            float barTop = 1080 - barH;
            need = Math.Clamp(lowest + 12 - barTop, 0, MaxLift);
        }
        // Small differences are ignored, so the view doesn't drift line by line.
        if (need > _liftTarget + 6 || need < _liftTarget - 40 || need == 0) _liftTarget = need;
        _roomView.Lift += (float)((_liftTarget - _roomView.Lift) * (1 - Math.Exp(-delta * 4)));
    }

    private Vector2 AboveThing(string anchor, Vector2 size)
    {
        float x = _roomView.ScreenX(anchor) ?? 960;
        return new Vector2(Math.Clamp(x - size.X / 2, 40, 1880 - size.X), 150);
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
        GD.Print($"shot: mode {_mode} phase {_phase}");
        if (_shotMenu is not null)
        {
            OpenMenu();
            if (_shotMenu == "settings") ShowSettingsMenu();
            else if (_shotMenu == "load") ShowLoadMenu();
        }
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
        _note.Visible = false;
        _heard.Visible = false;
        _conversing = true;
        ShowBar(true);
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
        var b = MakeButton(text);
        b.Pressed += onPressed;
        _choices.AddChild(b);
    }

    private static Button MakeButton(string text)
    {
        var b = new Button { Text = text, Alignment = HorizontalAlignment.Left, FocusMode = FocusModeEnum.All, SizeFlagsHorizontal = SizeFlags.ExpandFill, ClipText = false, AutowrapMode = TextServer.AutowrapMode.WordSmart };
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
        return b;
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
        _portrait.Texture = new AtlasTexture { Atlas = TextureFor(path), Region = PortraitCrop };
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
