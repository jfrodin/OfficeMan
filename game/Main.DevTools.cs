using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Godot;
using OfficeMan.Core;
using OfficeMan.Core.Content;

namespace OfficeMan.Game;

/// <summary>Tools for development and review: automated play, screenshots, single-room views, contact sheets.</summary>
public partial class Main
{
    private int _autoSteps = -1;
    private int _autoChoices;
    private string? _shotPath;
    private string? _lookRoom;
    private string? _lookHover;   // "--hover <thing id>": a room view with that thing lit up and named
    private string? _shotMenu;
    private bool _reviewOnly;   // showing Review.bat's screenshots, not playing
    private int _lookAt = -1;
    private string? _lookDoing;
    private string? _layoutArg;   // "bar" or "column": overrides the setting for screenshots
    private bool _lookingAtRoom;  // --room: the room alone, no text, no camera moves

    /// <summary>
    /// Command line (after --): --seed N; --shot path.png --steps N (play N steps automatically, then screenshot);
    /// --menu main|settings|load (open the pause menu before the screenshot); --room id --at HH:MM (one room, no
    /// dialogue); --stitch out.png in1.png … (a contact sheet); --review list.txt (look through screenshots).
    /// Returns true when a tool took over and the game shouldn't start.
    /// </summary>
    private bool HandleDevArgs(string[] args)
    {
        if (args.Length > 1 && args[0] == "--review")
        {
            Controls.Register();
            _reviewOnly = true;
            AddChild(new ReviewViewer(args[1]));
            return true;
        }
        if (args.Length > 2 && args[0] == "--stitch")
        {
            Stitch(args[1], args[2..]);
            GetTree().Quit();
            return true;
        }
        for (int i = 0; i < args.Length; i++)
        {
            if (args[i] == "--seed" && i + 1 < args.Length) _seed = int.Parse(args[++i]);
            else if (args[i] == "--shot" && i + 1 < args.Length) _shotPath = args[++i];
            else if (args[i] == "--steps" && i + 1 < args.Length) _autoSteps = int.Parse(args[++i]);
            else if (args[i] == "--room" && i + 1 < args.Length) _lookRoom = args[++i];
            else if (args[i] == "--menu" && i + 1 < args.Length) _shotMenu = args[++i];
            else if (args[i] == "--at" && i + 1 < args.Length) { var hm = args[++i].Split(':'); _lookAt = int.Parse(hm[0]) * 60 + int.Parse(hm[1]); }
            else if (args[i] == "--doing" && i + 1 < args.Length) _lookDoing = args[++i];
            else if (args[i] == "--layout" && i + 1 < args.Length) _layoutArg = args[++i];
            else if (args[i] == "--hover" && i + 1 < args.Length) _lookHover = args[++i];
        }
        return false;
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

    private void PlaceHoverName(string id)
    {
        var t = _roomView.Present.First(x => x.Id == id);
        _hoverName.ResetSize();
        var top = _roomView.TopOnScreen(t);
        _hoverName.Position = new Vector2(top.X - _hoverName.Size.X / 2, top.Y - _hoverName.Size.Y - 6);
    }

    /// <summary>Review aid: show one room, empty of dialogue, at a given time of day, and screenshot it.</summary>
    private void LookAtRoom(string id)
    {
        _autoSteps = -1;
        if (_lookAt >= 0) _game.State.Minute = _lookAt;
        _roomView.Minute = _game.State.Minute % 1440;
        // "--doing magda=bar,viktor=bar,flag": where people's plans have got to, and flags, for looking at a room.
        foreach (string part in (_lookDoing ?? "").Split(',', StringSplitOptions.RemoveEmptyEntries))
        {
            var kv = part.Split('=', 2);
            if (kv.Length == 2) _game.State.Character(kv[0].Trim()).Doing = kv[1].Trim();
            else _game.State.Set(kv[0].Trim(), 1);
        }
        _roomView.Visible = true;
        _scene.Visible = false;
        _roomView.Load(_rooms[id], _game.Evaluate);
        _panel.Visible = false;
        _choices.Visible = false;
        _note.Visible = false;
        _heard.Visible = false;
        _column.Visible = _columnFade.Visible = false;
        _lookingAtRoom = true;
        _typing = false;
        _more.Visible = false;
        _dim.Visible = false;
        _arrival.Visible = false;
        SetPortrait(null);
        if (_rooms[id].Things.FirstOrDefault(t => t.Id == _lookHover) is { } hover)
        {
            _roomView.Highlight(hover);
            _hoverName.Text = hover.Name;
            _hoverName.Visible = true;
            CallDeferred(MethodName.PlaceHoverName, hover.Id);
        }
        GetTree().CreateTimer(0.8).Timeout += () =>
        {
            GetViewport().GetTexture().GetImage().SavePng(_shotPath ?? Path.Combine(OS.GetUserDataDir(), "shot.png"));
            GetTree().Quit();
        };
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
        // Long enough for the camera to settle (the close-up eases in).
        GetTree().CreateTimer(1.6).Timeout += () =>
        {
            GetViewport().GetTexture().GetImage().SavePng(_shotPath ?? Path.Combine(OS.GetUserDataDir(), "shot.png"));
            GetTree().Quit();
        };
    }
}
