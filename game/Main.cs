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
/// something and interact. Text appears three ways over the live scene (Main.Lines): conversations in a
/// bar along the bottom with the speaker's portrait, Teo's thoughts as a compact note, overheard speech as a
/// caption towards its source. Phases without a room show a scene picture.
/// </summary>
/// <remarks>
/// Split by job: story flow, walking, input and saves here; the screen in Main.Ui; text in Main.Lines;
/// the pause menu in Main.Menu; review and automation tools in Main.DevTools.
/// Controls: arrows/WASD/stick to walk, E/Enter/Space (pad A) to interact or continue, number keys,
/// mouse or d-pad + A to choose, Esc (pad B) to back out of a menu, Esc (pad Start) for the pause menu,
/// F5 quick save, F9 quick load.
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

    public override void _Ready()
    {
        _seed = (int)(Time.GetTicksUsec() % int.MaxValue);
        if (HandleDevArgs(OS.GetCmdlineUserArgs())) return;

        string contentDir = ContentLocator.Find(ProjectSettings.GlobalizePath("res://"));
        _content = ContentBundle.Load(contentDir);
        _art = ArtManifest.Load(contentDir);
        _rooms = RoomLayout.LoadAll(contentDir);
        _saveDir = Path.Combine(OS.GetUserDataDir(), "saves");
        Directory.CreateDirectory(_saveDir);

        // Clicks that land on no button reach the input handlers below.
        MouseFilter = MouseFilterEnum.Ignore;
        Controls.Register();
        BuildUi();
        BuildMenu();
        LoadSettings();
        StartLife(_seed);
        if (_lookRoom is not null) LookAtRoom(_lookRoom);
    }

    // ───────────────────────── story flow ─────────────────────────

    private void StartLife(int seed)
    {
        _seed = seed;
        _game = OfficeMan.Core.Game.New(_content, seed);
        _phase = "";
        ClearLog();
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
        _roomView.Minute = _game.State.Minute % 1440;
        PlayLines(view.Lines, AfterStoryLines);
    }

    private void EnterPhase(string phaseId, string title, string when)
    {
        _phase = phaseId;
        _roomId = _art.RoomFor(_phase);
        bool room = _roomId is not null && _rooms.ContainsKey(_roomId);
        _roomView.Visible = room;
        _scene.Visible = !room;
        if (room)
        {
            _roomView.Load(_rooms[_roomId!], _game.Evaluate);
            _hailed.Clear();
        }
        else SetScene(_art.Scene(_phase));
        SetPortrait(null);
        _listener = null;
        _focus = null;
        _conversing = false;
        _moods.Clear();
        Arrive(string.IsNullOrEmpty(title) ? when : $"{title}  ·  {when}");
        if (UseColumn) ColumnHeading(string.IsNullOrEmpty(title) ? when : $"{title}  ·  {when}");
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
        ShowColumn(false);
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
            if (UseColumn) ColumnHeading(thing.Name);
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
        if (_reviewOnly) return;
        if (e.IsActionPressed("quicksave")) { Save("quicksave"); Toast("Saved"); return; }
        if (e.IsActionPressed("quickload")) { if (Load("quicksave")) Toast("Loaded"); return; }
        if (e is InputEventKey { Pressed: true, Echo: false, Keycode: Key.F2 } && _mode != Mode.Menu) { ToggleTextLayout(); return; }
        if (e is InputEventKey { Pressed: true, Echo: false, Keycode: Key.F3 } && _mode != Mode.Menu) { ToggleTalkZoom(); return; }
        if (MenuInput(e)) { GetViewport().SetInputAsHandled(); return; }

        switch (_mode)
        {
            case Mode.Talking when e.IsActionPressed("interact"):
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

    /// <summary>A left click anywhere moves the text on, over the text column too (buttons exist only when choosing).</summary>
    public override void _Input(InputEvent e)
    {
        if (_reviewOnly || _mode != Mode.Talking) return;
        if (e is InputEventMouseButton { Pressed: true, ButtonIndex: MouseButton.Left })
        {
            Continue();
            GetViewport().SetInputAsHandled();
        }
    }

    public override void _Process(double delta)
    {
        if (_reviewOnly) return;
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
        UpdateHail();
        if (_autoSteps >= 0) RunAutomation();
    }

    // ───────────────────────── ending ─────────────────────────

    private void ShowEnding()
    {
        if (UseColumn)
        {
            ShowColumnEnding();
            return;
        }
        SetMode(Mode.Ending);
        _note.Visible = false;
        _heard.Visible = false;
        _conversing = true;
        ShowBar(true);
        SetPortrait(null);
        _speaker.Visible = true;
        _speaker.Text = "To be continued";
        string today = string.Join("\n", _view.Today.Select(Bbcode));
        string happened = string.Join("\n", _view.Headlines.Select(h => "·  " + h).DefaultIfEmpty("Nothing much happened. That's also a life."));
        _text.Text = today.Length == 0 ? happened : $"[i]{today}[/i]\n\n{happened}";
        _text.VisibleRatio = 1;
        AddChoice("Live another Tuesday", () => StartLife((int)(Time.GetTicksUsec() % int.MaxValue)));
        AddChoice("Quit", () => GetTree().Quit());
        ((Control)_choices.GetChild(0)).CallDeferred(Control.MethodName.GrabFocus);
    }

    private void ShowColumnEnding()
    {
        SetMode(Mode.Ending);
        _note.Visible = false;
        _heard.Visible = false;
        ShowColumn(true);
        SetPortrait(null);
        ColumnHeading("To be continued");
        foreach (string line in _view.Today)
            AddLogEntry($"[color=#{Thought.ToHtml(false)}][i]{Bbcode(line)}[/i][/color]").VisibleRatio = 1;
        foreach (string h in _view.Headlines.DefaultIfEmpty("Nothing much happened. That's also a life."))
            AddLogEntry("·  " + Bbcode(h)).VisibleRatio = 1;
        _choices.Columns = 1;
        AddChoice("Live another Tuesday", () => StartLife((int)(Time.GetTicksUsec() % int.MaxValue)));
        AddChoice("Quit", () => GetTree().Quit());
        ScrollToEnd();
        ((Control)_choices.GetChild(0)).CallDeferred(Control.MethodName.GrabFocus);
    }
    // ───────────────────────── art ─────────────────────────

    private void SetScene(string path) => _scene.Texture = TextureFor(path);

    private Texture2D TextureFor(string path)
    {
        if (_textures.TryGetValue(path, out var t)) return t;
        t = ImageTexture.CreateFromImage(Image.LoadFromFile(path));
        _textures[path] = t;
        return t;
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
