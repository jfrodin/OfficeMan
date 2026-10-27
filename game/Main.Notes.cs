using System;
using System.IO;
using System.Text.Json;
using Godot;

namespace OfficeMan.Game;

/// <summary>
/// F1: a note about what's on screen. The game keeps a picture of the screen (taken before the box opens), the
/// words, and where the life is (phase, clock, seed, the whole save), in inbox/ at the repository root, which git
/// ignores. The creator writes notes while playing; they're gone through together later, not fixed on the spot.
/// </summary>
public partial class Main
{
    private string _inboxDir = "";
    private PanelContainer _notePanel = null!;
    private LineEdit _noteEdit = null!;
    private Image? _noteShot;

    private bool Noting => _notePanel.Visible;

    private void BuildNotes(string contentDir)
    {
        _inboxDir = Path.Combine(Path.GetDirectoryName(contentDir)!, "inbox");

        _notePanel = new PanelContainer { Visible = false, MouseFilter = MouseFilterEnum.Stop };
        _notePanel.AddThemeStyleboxOverride("panel", new StyleBoxFlat
        {
            BgColor = new Color(0.082f, 0.072f, 0.064f, 0.97f),
            BorderColor = new Color(0.96f, 0.84f, 0.56f, 0.6f),
            BorderWidthTop = 2, BorderWidthBottom = 2, BorderWidthLeft = 2, BorderWidthRight = 2,
            ContentMarginLeft = 28, ContentMarginRight = 28, ContentMarginTop = 20, ContentMarginBottom = 22,
        });
        var box = new VBoxContainer();
        box.AddThemeConstantOverride("separation", 12);
        box.AddChild(MakeLabel(22, new Color(0.96f, 0.84f, 0.56f)));
        ((Label)box.GetChild(0)).Text = "A note about this moment.  Enter saves it, Esc throws it away.";
        _noteEdit = new LineEdit { CustomMinimumSize = new Vector2(1000, 0), PlaceholderText = "What do you see?" };
        _noteEdit.AddThemeFontSizeOverride("font_size", 26);
        _noteEdit.TextSubmitted += SaveNote;
        _noteEdit.GuiInput += e =>
        {
            if (e is InputEventKey { Pressed: true, Keycode: Key.Escape })
            {
                CloseNote();
                _noteEdit.AcceptEvent();
            }
        };
        box.AddChild(_noteEdit);
        _notePanel.AddChild(box);
        AddChild(_notePanel);
    }

    private void OpenNote()
    {
        _noteShot = GetViewport().GetTexture().GetImage();
        _noteEdit.Text = "";
        _notePanel.Visible = true;
        _notePanel.ResetSize();
        _notePanel.Position = new Vector2((1920 - _notePanel.Size.X) / 2, 1080 - _notePanel.Size.Y - 80);
        _noteEdit.GrabFocus();
    }

    private void CloseNote()
    {
        _notePanel.Visible = false;
        _noteEdit.ReleaseFocus();
        _noteShot = null;
    }

    private void SaveNote(string text)
    {
        if (string.IsNullOrWhiteSpace(text)) { CloseNote(); return; }
        try
        {
            Directory.CreateDirectory(_inboxDir);
            string stamp = DateTime.Now.ToString("yyyy-MM-dd_HHmmss");
            _noteShot?.SavePng(Path.Combine(_inboxDir, stamp + ".png"));
            var note = new
            {
                note = text.Trim(),
                written = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                phase = _phase,
                room = _roomId,
                clock = _view.Clock,
                seed = _game.State.Seed,
                save = JsonDocument.Parse(_game.Save().ToJson()).RootElement,
            };
            File.WriteAllText(Path.Combine(_inboxDir, stamp + ".json"),
                JsonSerializer.Serialize(note, new JsonSerializerOptions { WriteIndented = true }));
            Toast("Noted");
        }
        catch (Exception ex)
        {
            Toast("Couldn't save the note");
            GD.PrintErr($"note: {ex.Message}");
        }
        CloseNote();
    }
}
