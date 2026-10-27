using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Godot;
using OfficeMan.Core;
using OfficeMan.Core.Content;

namespace OfficeMan.Game;

/// <summary>Building and laying out the screen: the bar, the portrait, notes, captions, choices, the camera.</summary>
public partial class Main
{
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
        _barBox = box;
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
        BuildColumn();
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

    /// <summary>
    /// Places the bar, the portrait, the note and the overheard caption, and moves the camera only when
    /// the bar would cover the people in the conversation.
    /// </summary>
    private void Layout(double delta)
    {
        if (UseColumn && !_lookingAtRoom)
        {
            LayoutColumn(delta);
            _roomView.Lift += (float)((0 - _roomView.Lift) * (1 - Math.Exp(-delta * 4)));
            if (_note.Visible || _heard.Visible) LayoutCaptions();
            return;
        }
        _roomView.Pan += (float)((0 - _roomView.Pan) * (1 - Math.Exp(-delta * 4)));
        _roomView.Zoom = 1;
        _roomView.Shift = Vector2.Zero;
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

    /// <summary>The thought note and the overheard caption, when they show outside the column.</summary>
    private void LayoutCaptions()
    {
        _note.Size = Vector2.Zero;
        _note.Position = new Vector2((1920 - _note.Size.X) / 2, 1080 - 60 - _note.Size.Y);
        _heard.Size = Vector2.Zero;
        _heard.Position = new Vector2((1920 - _heard.Size.X) / 2, 110);
    }

    private Vector2 AboveThing(string anchor, Vector2 size)
    {
        float x = _roomView.ScreenX(anchor) ?? 960;
        return new Vector2(Math.Clamp(x - size.X / 2, 40, 1880 - size.X), 150);
    }

    private void AddChoice(string text, Action onPressed)
    {
        var b = MakeButton(text);
        if (UseColumn) StampStyle(b);
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
}
