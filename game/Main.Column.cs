using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using OfficeMan.Core;

namespace OfficeMan.Game;

/// <summary>
/// The text column: the alternative to the bottom bar. All text runs down a column on the right, as a log you can
/// scroll back through: what people say, Teo's thoughts, what he overhears, the choices at the bottom. The portrait
/// of whoever is talking sits at the top. While the column is open the room slides left, so the people in the
/// conversation stay in view. Chosen in Settings (Text: Column / Bottom bar) or with F2.
/// </summary>
public partial class Main
{
    private const float ColumnW = 640;
    private const int LogKeep = 80;   // lines kept in the log
    private static readonly Color Faded = new(1, 1, 1, 0.55f), Dim = new(0.62f, 0.66f, 0.74f);

    private PanelContainer _column = null!;
    private ScrollContainer _columnScroll = null!;
    private VBoxContainer _columnLog = null!;
    private Control _columnTop = null!;
    private VBoxContainer _barBox = null!;
    private float _panTarget;

    private bool UseColumn => _settings.TextLayout == 0;

    private void BuildColumn()
    {
        _column = new PanelContainer { Visible = false, MouseFilter = MouseFilterEnum.Ignore };
        var bg = new StyleBoxFlat { BgColor = new Color(0.04f, 0.035f, 0.05f, 0.95f), BorderColor = new Color(1f, 0.85f, 0.65f, 0.16f) };
        bg.BorderWidthLeft = 2;
        bg.ContentMarginLeft = 34;
        bg.ContentMarginRight = 30;
        bg.ContentMarginTop = 26;
        bg.ContentMarginBottom = 34;
        _column.AddThemeStyleboxOverride("panel", bg);

        var box = new VBoxContainer { MouseFilter = MouseFilterEnum.Ignore };
        box.AddThemeConstantOverride("separation", 14);
        // Room for the portrait card, which is placed over this space in Layout.
        _columnTop = new Control { CustomMinimumSize = new Vector2(0, PortraitH + 6), MouseFilter = MouseFilterEnum.Ignore };
        box.AddChild(_columnTop);

        _columnScroll = new ScrollContainer { SizeFlagsVertical = Control.SizeFlags.ExpandFill, HorizontalScrollMode = ScrollContainer.ScrollMode.Disabled };
        _columnLog = new VBoxContainer { SizeFlagsHorizontal = Control.SizeFlags.ExpandFill, MouseFilter = MouseFilterEnum.Ignore };
        _columnLog.AddThemeConstantOverride("separation", 16);
        _columnScroll.AddChild(_columnLog);
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
            _column.Visible = false;
            _panTarget = 0;
        }
    }

    private void ShowColumn(bool show)
    {
        _column.Visible = show;
        if (!show) SetPortrait(null);
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
            string name = speaker == "TEO" ? "#d8d0c4" : "#" + Accent.ToHtml(false);
            text = $"[color={name}]{speaker}[/color]   {Bbcode(line.Body)}";
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
        var label = AddLogEntry($"[color=#9a917f]>  {Bbcode(text)}[/color]");
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

    /// <summary>Places the column and slides the room so the people in the conversation stay in view.</summary>
    private void LayoutColumn(double delta)
    {
        _column.Position = new Vector2(1920 - ColumnW, 0);
        _column.Size = new Vector2(ColumnW, 1080);
        _columnTop.CustomMinimumSize = new Vector2(0, _portraitCard.Visible ? PortraitH + 6 : 0);
        if (_portraitCard.Visible) _portraitCard.Position = new Vector2(1920 - ColumnW + 34, 26);
        if (!_typing && _mode == Mode.Talking) _more.Position = new Vector2(1920 - 70, 1080 - 64);

        if (_column.Visible && _roomView.Visible)
        {
            // Keep Teo and whoever he's with inside the part of the screen the column leaves free.
            float teo = _roomView.TeoVisible ? _roomView.TeoScreenX : 960;
            float other = _focus is null ? teo : _roomView.ScreenX(_focus.ToLowerInvariant()) ?? teo;
            float right = Math.Max(teo, other) + 170;
            _panTarget = Math.Clamp(right - (1920 - ColumnW), 0, ColumnW);
        }
        else _panTarget = 0;
        _roomView.Pan += (float)((_panTarget - _roomView.Pan) * (1 - Math.Exp(-delta * 7)));
    }
}
