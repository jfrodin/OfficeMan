using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using OfficeMan.Core;

namespace OfficeMan.Game;

/// <summary>
/// The text column: the alternative to the bottom bar. All text runs down a column on the right, as a log you can
/// scroll back through: what people say, Teo's thoughts, what he overhears, the choices at the bottom. It slides in
/// when there's text and out when Teo walks. The portrait of whoever is talking hangs on its left edge, over the
/// picture, level with the newest line. While the column is open the room slides left, so the people in the
/// conversation stay in view. Chosen in Settings (Text: Column / Bottom bar) or with F2.
///
/// Colours: a warm, dark, paper-like column that fades into the picture. Names in one warm family with small
/// differences per person, so people are recognisable without a rainbow. Teo's thoughts in one fixed colour
/// (his state shows in the words, not the colour). Choices in stamp blue, like an office stamp.
/// </summary>
public partial class Main
{
    private const float ColumnW = 640, FadeW = 150, PortraitOverlap = 26;
    private const int LogKeep = 80;   // lines kept in the log
    private static readonly Color Faded = new(1, 1, 1, 0.55f), Dim = new(0.62f, 0.66f, 0.74f);
    private static readonly Color Paper = new(0.082f, 0.072f, 0.064f, 0.96f);
    private static readonly Color Stamp = new(0.56f, 0.69f, 0.92f), StampHot = new(0.78f, 0.87f, 1f), StampUsed = new(0.48f, 0.54f, 0.66f);
    private static readonly Color TeoName = new(0.66f, 0.62f, 0.56f);

    // Small differences inside one warm family. Anyone not listed gets the accent.
    private static readonly Dictionary<string, Color> NameColours = new()
    {
        ["MAGDA"] = new(0.96f, 0.66f, 0.56f),    // coral, the dress
        ["VIKTOR"] = new(0.93f, 0.76f, 0.46f),   // tanned gold
        ["ZDENKA"] = new(0.76f, 0.79f, 0.66f),   // filing-cabinet grey-green
        ["KORNEL"] = new(0.88f, 0.8f, 0.52f),    // pale mustard
        ["RAMONA"] = new(0.92f, 0.89f, 0.84f),   // pearl
        ["STOJAN"] = new(0.8f, 0.7f, 0.52f),     // olive brown
    };

    private float _columnOut, _portraitY = -1;
    private bool _columnWanted;
    private TextureRect _columnFade = null!;

    private PanelContainer _column = null!;
    private ScrollContainer _columnScroll = null!;
    private VBoxContainer _columnLog = null!;
    private Control _columnTop = null!;
    private VBoxContainer _barBox = null!;
    private float _panTarget;

    private bool UseColumn => _settings.TextLayout == 0;

    private void BuildColumn()
    {
        // The soft edge: the picture darkens into the column instead of meeting a hard line.
        var fade = new Gradient();
        fade.SetColor(0, new Color(Paper, 0));
        fade.SetColor(1, Paper);
        _columnFade = new TextureRect
        {
            Texture = new GradientTexture2D { Gradient = fade, Width = 64, Height = 4 },
            ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize, StretchMode = TextureRect.StretchModeEnum.Scale,
            Size = new Vector2(FadeW, 1080), Visible = false, MouseFilter = MouseFilterEnum.Ignore,
        };
        AddChild(_columnFade);

        _column = new PanelContainer { Visible = false, MouseFilter = MouseFilterEnum.Ignore };
        var bg = new StyleBoxFlat { BgColor = Paper };
        bg.ContentMarginLeft = 34;
        bg.ContentMarginRight = 30;
        bg.ContentMarginTop = 26;
        bg.ContentMarginBottom = 34;
        _column.AddThemeStyleboxOverride("panel", bg);

        var box = new VBoxContainer { MouseFilter = MouseFilterEnum.Ignore };
        box.AddThemeConstantOverride("separation", 14);
        // The portrait hangs outside the column now; this spacer stays for the bar layout's sake and is empty.
        _columnTop = new Control { CustomMinimumSize = Vector2.Zero, MouseFilter = MouseFilterEnum.Ignore };
        box.AddChild(_columnTop);

        _columnScroll = new ScrollContainer { SizeFlagsVertical = Control.SizeFlags.ExpandFill, HorizontalScrollMode = ScrollContainer.ScrollMode.Disabled };
        _columnLog = new VBoxContainer { SizeFlagsHorizontal = Control.SizeFlags.ExpandFill, MouseFilter = MouseFilterEnum.Ignore };
        _columnLog.AddThemeConstantOverride("separation", 16);
        _columnScroll.AddChild(_columnLog);
        // Follow the log as it grows (new lines, the choices under them), once the layout has settled.
        var bar = _columnScroll.GetVScrollBar();
        bar.Changed += () => _columnScroll.ScrollVertical = (int)bar.MaxValue;
        box.AddChild(_columnScroll);

        _column.AddChild(box);
        AddChild(_column);
        // The portrait card and the "more" marker draw above the column.
        MoveChild(_portraitCard, -1);
        MoveChild(_more, -1);
    }

    /// <summary>Moves the choices into the column or back into the bar, for the current layout.</summary>
    private void ApplyTextLayout()
    {
        if (_column is null) return;
        var parent = _choices.GetParent();
        var home = UseColumn ? (Node)_columnLog : _barBox;
        if (parent != home)
        {
            parent?.RemoveChild(_choices);
            home.AddChild(_choices);
        }
        if (!UseColumn)
        {
            _columnWanted = false;
            _columnOut = 0;
            _column.Visible = _columnFade.Visible = false;
            _panTarget = 0;
        }
    }

    /// <summary>The column slides in when there's text and out when Teo walks again (see LayoutColumn).</summary>
    private void ShowColumn(bool show)
    {
        _columnWanted = show;
        if (show) _column.Visible = _columnFade.Visible = true;
        else SetPortrait(null);
    }

    private static Color NameColour(string speaker) =>
        speaker == "TEO" ? TeoName : NameColours.TryGetValue(speaker, out var c) ? c : Accent;

    /// <summary>Choice buttons in the column: stamp blue.</summary>
    private static void StampStyle(Button b)
    {
        b.AddThemeColorOverride("font_color", Stamp);
        b.AddThemeColorOverride("font_hover_color", StampHot);
        b.AddThemeColorOverride("font_focus_color", StampHot);
        var hot = Box(new Color(0.56f, 0.69f, 0.92f, 0.1f), 4, 0, default, 14, 6);
        b.AddThemeStyleboxOverride("normal", Box(new Color(1, 1, 1, 0), 4, 0, default, 14, 6));
        b.AddThemeStyleboxOverride("hover", hot);
        b.AddThemeStyleboxOverride("focus", hot);
        b.AddThemeStyleboxOverride("pressed", hot);
    }

    /// <summary>One line of the story, added to the log and typed out.</summary>
    private void ColumnLine(StoryLine line)
    {
        string? speaker = line.Speaker;
        string? heardFrom = HeardFrom(line);
        ShowColumn(true);
        _note.Visible = false;
        _heard.Visible = false;

        string text;
        if (speaker is not null && heardFrom is not null)
        {
            DimPortrait(true);
            text = $"[color=#{Dim.ToHtml(false)}]{Title(speaker)}, {heardFrom}:[/color]  [i]{Bbcode(line.Body)}[/i]";
        }
        else if (speaker is null)
        {
            // Teo's thoughts: italic, a cooler colour. The person he's with stays, dimmed.
            DimPortrait(true);
            text = $"[color=#{Thought.ToHtml(false)}][i]{Bbcode(line.Body)}[/i][/color]";
        }
        else
        {
            if (speaker != "TEO")
            {
                string? mood = Tag(line, "mood");
                if (mood is not null) _moods[speaker] = mood;
                _listener = _art.HasPortraits(speaker) ? speaker : null;
                _focus = speaker;
                SetPortrait(_listener is null ? null : _art.Portrait(speaker, _moods.GetValueOrDefault(speaker)));
                DimPortrait(false);
            }
            else DimPortrait(true);
            text = $"[color=#{NameColour(speaker).ToHtml(false)}]{speaker}[/color]   {Bbcode(line.Body)}";
        }
        _conversing = true;
        StartTyping(AddLogEntry(text));
    }

    /// <summary>A small heading in the log: a new place, or the person Teo walked up to.</summary>
    private void ColumnHeading(string text)
    {
        var label = AddLogEntry($"[color=#{Accent.ToHtml(false)}]{Bbcode(text.ToUpperInvariant())}[/color]", 20);
        label.VisibleRatio = 1;
    }

    /// <summary>What Teo chose, kept in the log in his own colour.</summary>
    private void ColumnEcho(string choice)
    {
        string text = choice.EndsWith("   →") ? choice[..^4] : choice;
        var label = AddLogEntry($"[color=#{StampUsed.ToHtml(false)}]>  {Bbcode(text)}[/color]");
        label.VisibleRatio = 1;
    }

    private RichTextLabel AddLogEntry(string bbcode, int size = BodySize - 3)
    {
        // Older lines fade a little, so the newest one is where the eye goes.
        foreach (var child in _columnLog.GetChildren())
            if (child is RichTextLabel old) old.Modulate = Faded;

        var label = new RichTextLabel
        {
            BbcodeEnabled = true, FitContent = true, ScrollActive = false, MouseFilter = MouseFilterEnum.Ignore,
            AutowrapMode = TextServer.AutowrapMode.WordSmart, SizeFlagsHorizontal = Control.SizeFlags.ExpandFill,
            Text = bbcode,
        };
        label.AddThemeFontSizeOverride("normal_font_size", size);
        label.AddThemeFontSizeOverride("italics_font_size", size);
        label.AddThemeColorOverride("default_color", Spoken);
        _columnLog.AddChild(label);
        _columnLog.MoveChild(_choices, -1);

        var entries = _columnLog.GetChildren().OfType<RichTextLabel>().ToList();
        for (int i = 0; i < entries.Count - LogKeep; i++)
        {
            _columnLog.RemoveChild(entries[i]);
            entries[i].QueueFree();
        }
        ScrollToEnd();
        return label;
    }

    private void ScrollToEnd() =>
        Callable.From(() => _columnScroll.ScrollVertical = (int)_columnScroll.GetVScrollBar().MaxValue).CallDeferred();

    private void ClearLog()
    {
        foreach (var child in _columnLog.GetChildren().OfType<RichTextLabel>().ToList())
        {
            _columnLog.RemoveChild(child);
            child.QueueFree();
        }
    }

    private void ToggleTextLayout()
    {
        _settings.TextLayout = UseColumn ? 1 : 0;
        SaveSettings();
        ApplyTextLayout();
        Toast(UseColumn ? "Text: column" : "Text: bottom bar");
        // Show the current moment again in the new layout.
        if (_mode is Mode.Talking or Mode.Choosing) Show(_view);
    }

    private void ToggleTalkZoom()
    {
        _settings.TalkZoom = !_settings.TalkZoom;
        SaveSettings();
        Toast(_settings.TalkZoom ? "Close-up in conversations: on" : "Close-up in conversations: off");
    }

    private const float TalkZoom = 2;

    /// <summary>
    /// Places the column and slides the room so the people in the conversation stay in view.
    /// With the close-up on, the camera moves in on Teo and whoever he's talking to, and back out when he walks.
    /// Only whole steps of art pixels (×2 or ×1.5 of the room's ×4), so the pixels stay square once it settles.
    /// </summary>
    private void LayoutColumn(double delta)
    {
        // Slide in and out.
        _columnOut += (float)(((_columnWanted ? 1 : 0) - _columnOut) * (1 - Math.Exp(-delta * 10)));
        if (!_columnWanted && _columnOut < 0.01f) { _columnOut = 0; _column.Visible = _columnFade.Visible = false; }
        float left = 1920 - ColumnW * _columnOut;
        _column.Position = new Vector2(left, 0);
        _column.Size = new Vector2(ColumnW, 1080);
        _columnFade.Position = new Vector2(left - FadeW, 0);
        _columnFade.Size = new Vector2(FadeW, 1080);
        _columnFade.Modulate = new Color(1, 1, 1, _columnOut);
        _columnTop.CustomMinimumSize = Vector2.Zero;

        // The portrait hangs on the column's left edge, over the picture, level with the newest line.
        if (_portraitCard.Visible)
        {
            var newest = _columnLog.GetChildren().OfType<RichTextLabel>().LastOrDefault();
            float target = newest is null ? 120 : newest.GlobalPosition.Y - 40;
            target = Math.Clamp(target, 40, 400);   // never low in the frame, where the people stand
            _portraitY = _portraitY < 0 ? target : _portraitY + (target - _portraitY) * (float)(1 - Math.Exp(-delta * 6));
            _portraitCard.Position = new Vector2(left - _portraitCard.Size.X + PortraitOverlap, _portraitY);
        }
        else _portraitY = -1;
        if (!_typing && _mode == Mode.Talking) _more.Position = new Vector2(1920 - 70, 1080 - 64);

        // What the column (and the portrait on its edge) leave free of the picture.
        float free = 1920 - ColumnW - (_portraitCard.Visible ? _portraitCard.Size.X - PortraitOverlap : 0);
        if (_column.Visible && _roomView.Visible)
        {
            // Keep Teo and whoever he's with inside the part of the screen the column leaves free.
            float teo = _roomView.TeoScreenX;
            float other = _focus is null ? teo : _roomView.ScreenX(_focus.ToLowerInvariant()) ?? teo;
            float right = Math.Max(teo, other) + 240;   // room for a body, and the soft edge of the column
            _panTarget = Math.Clamp(right - free, 0, ColumnW);
        }
        else _panTarget = 0;

        float zoom = 1;
        var shift = Vector2.Zero;
        var with = _focus is null ? null : _roomView.ScreenAt(_focus.ToLowerInvariant());
        if (_settings.TalkZoom && _column.Visible && _roomView.Visible && _roomView.TeoVisible && with is Vector2 them
            && _mode is Mode.Talking or Mode.Choosing)
        {
            // The two of them, about chest high, in the middle of the space the column leaves.
            var teo = new Vector2(_roomView.TeoScreenX, _roomView.ScreenFeet(null) ?? 900);
            var mid = (teo + them) / 2 - new Vector2(0, 110);
            // Closer when they stand close; further out when they're apart, so both stay in the picture.
            float apart = Math.Abs(teo.X - them.X);
            zoom = apart < 400 ? TalkZoom : apart < 600 ? 1.5f : 1;
            shift = new Vector2(free / 2f, 560) - mid * zoom;
            shift.X = Math.Clamp(shift.X, 1920 - 1920 * zoom - ColumnW, 0);
            shift.Y = Math.Clamp(shift.Y, 1080 - 1080 * zoom, 0);
            _panTarget = 0;
        }
        float k = (float)(1 - Math.Exp(-delta * 5));
        _roomView.Zoom = Math.Abs(zoom - _roomView.Zoom) < 0.004f ? zoom : _roomView.Zoom + (zoom - _roomView.Zoom) * k;
        _roomView.Shift = (shift - _roomView.Shift).Length() < 1 ? shift : _roomView.Shift + (shift - _roomView.Shift) * k;
        _roomView.Pan += (float)((_panTarget - _roomView.Pan) * (1 - Math.Exp(-delta * 7)));
    }
}
