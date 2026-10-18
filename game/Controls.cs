using Godot;

namespace OfficeMan.Game;

/// <summary>Keyboard and controller bindings, registered in code so they live with the game.</summary>
public static class Controls
{
    public static void Register()
    {
        Bind("move_left", Key.Left, Key.A, JoyButton.DpadLeft, JoyAxis.LeftX, -1);
        Bind("move_right", Key.Right, Key.D, JoyButton.DpadRight, JoyAxis.LeftX, 1);
        Bind("move_up", Key.Up, Key.W, JoyButton.DpadUp, JoyAxis.LeftY, -1);
        Bind("move_down", Key.Down, Key.S, JoyButton.DpadDown, JoyAxis.LeftY, 1);
        Bind("interact", new[] { Key.E, Key.Enter, Key.KpEnter, Key.Space }, JoyButton.A);
        Bind("cancel", new[] { Key.Escape, Key.Backspace }, JoyButton.B);
        Bind("quicksave", new[] { Key.F5 }, null);
        Bind("quickload", new[] { Key.F9 }, null);
    }

    private static void Bind(string action, Key key1, Key key2, JoyButton button, JoyAxis axis, float direction)
    {
        Bind(action, new[] { key1, key2 }, button);
        InputMap.ActionAddEvent(action, new InputEventJoypadMotion { Axis = axis, AxisValue = direction });
    }

    private static void Bind(string action, Key[] keys, JoyButton? button)
    {
        if (InputMap.HasAction(action)) InputMap.EraseAction(action);
        InputMap.AddAction(action, 0.25f);
        foreach (var k in keys) InputMap.ActionAddEvent(action, new InputEventKey { Keycode = k });
        if (button is JoyButton b) InputMap.ActionAddEvent(action, new InputEventJoypadButton { ButtonIndex = b });
    }
}
