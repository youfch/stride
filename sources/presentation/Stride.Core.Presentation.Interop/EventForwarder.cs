using System;

namespace Stride.Core.Presentation.Interop;

/// <summary>
/// Event forwarding utilities for bridging input events between WPF and Avalonia.
/// </summary>
public static class EventForwarder
{
    /// <summary>
    /// Forward types for input events.
    /// </summary>
    public enum EventType
    {
        MouseMove,
        MouseDown,
        MouseUp,
        MouseWheel,
        KeyDown,
        KeyUp,
        TextInput
    }

    /// <summary>
    /// Mouse event data.
    /// </summary>
    public readonly struct MouseEvent
    {
        public readonly EventType Type;
        public readonly float X;
        public readonly float Y;
        public readonly int Button;
        public readonly int Modifiers;

        public MouseEvent(EventType type, float x, float y, int button = 0, int modifiers = 0)
        {
            Type = type;
            X = x;
            Y = y;
            Button = button;
            Modifiers = modifiers;
        }
    }

    /// <summary>
    /// Key event data.
    /// </summary>
    public readonly struct KeyEvent
    {
        public readonly EventType Type;
        public readonly int KeyCode;
        public readonly string? Character;
        public readonly int Modifiers;

        public KeyEvent(EventType type, int keyCode, string? character = null, int modifiers = 0)
        {
            Type = type;
            KeyCode = keyCode;
            Character = character;
            Modifiers = modifiers;
        }
    }

    /// <summary>
    /// Handler for forwarded events.
    /// </summary>
    public delegate void EventHandler(object sender, object eventData);

    /// <summary>
    /// Registers an event handler for the specified event type.
    /// </summary>
    public static void RegisterHandler(EventType type, EventHandler handler)
    {
        // TODO: Implement event registration
    }

    /// <summary>
    /// Unregisters an event handler for the specified event type.
    /// </summary>
    public static void UnregisterHandler(EventType type, EventHandler handler)
    {
        // TODO: Implement event unregistration
    }

    /// <summary>
    /// Forwards a mouse event from WPF to Avalonia or vice versa.
    /// </summary>
    public static void ForwardMouseEvent(MouseEvent mouseEvent)
    {
        // TODO: Implement event forwarding
    }

    /// <summary>
    /// Forwards a key event from WPF to Avalonia or vice versa.
    /// </summary>
    public static void ForwardKeyEvent(KeyEvent keyEvent)
    {
        // TODO: Implement event forwarding
    }
}
