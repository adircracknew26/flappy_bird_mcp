using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>Centraliza la lectura de input para teclado, mouse, touch y gamepad.</summary>
public static class GameInput
{
    public static bool PressedThisFrame()
    {
        bool pressed = false;

        if (Keyboard.current != null)
            pressed |= Keyboard.current.spaceKey.wasPressedThisFrame
                    || Keyboard.current.enterKey.wasPressedThisFrame;

        if (Mouse.current != null)
            pressed |= Mouse.current.leftButton.wasPressedThisFrame;

        if (Touchscreen.current != null)
            pressed |= Touchscreen.current.primaryTouch.press.wasPressedThisFrame;

        if (Gamepad.current != null)
            pressed |= Gamepad.current.buttonSouth.wasPressedThisFrame;

        return pressed;
    }
}
