using System;
using System.Collections.Generic;
using Godot;

namespace GoDogKit
{
    public partial class GlobalInputManager : Singleton<GlobalInputManager>
    {
        private readonly static Dictionary<Type, InputMode> m_InputModeMap = new()
        {
            {typeof(InputEventKey), InputMode.KeyboardAndMouse },
            {typeof(InputEventMouseButton), InputMode.KeyboardAndMouse },
            {typeof(InputEventMouseMotion), InputMode.KeyboardAndMouse },

            {typeof(InputEventJoypadMotion), InputMode.Gamepad },
            {typeof(InputEventJoypadButton), InputMode.Gamepad },

            {typeof(InputEventScreenTouch), InputMode.Screen },
            {typeof(InputEventScreenDrag), InputMode.Screen },
        };

        public static InputMode CurrentInputMode { get; set; }
        private static InputMode m_PreviousInputMode;
        public static event Action<InputMode> InputModeChanged = delegate { };

        public override void _Input(InputEvent @event)
        {
            DetectInputMode(@event);
        }

        private static void DetectInputMode(InputEvent @event)
        {
            if (!m_InputModeMap.TryGetValue(@event.GetType(), out InputMode inputMode)) return;

            if (inputMode != m_PreviousInputMode)
            {
                InputModeChanged.Invoke(CurrentInputMode);
                m_PreviousInputMode = CurrentInputMode = inputMode;
            }
        }
    }
}

