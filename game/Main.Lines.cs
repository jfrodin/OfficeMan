using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Godot;
using OfficeMan.Core;
using OfficeMan.Core.Content;

namespace OfficeMan.Game;

/// <summary>Playing the story's lines: conversations, thoughts, overheard speech, typing, choices.</summary>
public partial class Main
{
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
}
