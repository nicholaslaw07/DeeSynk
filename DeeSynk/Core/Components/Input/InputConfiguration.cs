using OpenTK.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeeSynk.Core.Components.Input
{
    public enum KeyActionType
    {
        RepeatOnHold = 0,
        WaitForRelease = 1,
        SinglePress = 2
    }

    public enum MouseActionType
    {
        RepeatOnHold = 0,
        WaitForRelease = 1,
        SingleClick = 2,
        ClickAndDrag = 3,
        Hybrid = 4  //Hybrid is that short clicks do different things than a click and drag
    }

    public struct MouseMove
    {
        int _dx;
        public int dX { get => _dx; }

        int _dy;
        public int dY { get => _dy; }

        long _time;
        public long Time { get => _time; }

        public MouseMove(int dx, int dy, long time)
        {
            _dx = dx;
            _dy = dy;
            _time = time;
        }
    }

    public struct MouseClick
    {
        int _x;
        public int X { get => _x; }
        int _y;
        public int Y { get => _y; }

        long _time;
        public long Time { get => _time; }

        public MouseClick(int x, int y, long time)
        {
            _x = x;
            _y = y;
            _time = time;
        }
    }

    public struct MouseScroll
    {
        int _scrollValue;
        public int ScrollValue { get => _scrollValue; }

        long _time;
        public long Time { get => _time; }

        public MouseScroll(int scrollValue, long time)
        {
            _scrollValue = scrollValue;
            _time = time;
        }
    }

    public struct KeyPress
    {
        Key _key;
        public Key Key { get => _key; }

        long _time;
        public long Time { get => _time; }

        public KeyPress(Key key, long time)
        {
            _key = key;
            _time = time;
        }
    }
    public class KeyboardAction
    {
        private Action<float> _action;
        public Action<float> Action { get => _action; }

        private Key _key;
        public Key Key { get => _key; }

        private KeyActionType _keyActionType;
        public KeyActionType Type { get => _keyActionType; }

        public KeyboardAction(Action<float> action, Key key, KeyActionType keyActionType)
        {
            _action = action;
            _key = key;
            _keyActionType = keyActionType;
        }
    }

    public class MouseButtonAction
    {
        private Action<float, MouseClick, MouseMove> _action;
        public Action<float, MouseClick, MouseMove> Action { get => _action; }

        private MouseButton _button;
        public MouseButton Button { get => _button; }

        private MouseActionType _buttonActionType;
        public MouseActionType ButtonAction { get => _buttonActionType; }

        public MouseButtonAction(Action<float, MouseClick, MouseMove> action, MouseButton button, MouseActionType buttonActionType)
        {
            _action = action;
            _button = button;
            _buttonActionType = buttonActionType;
        }
    }

    public class InputConfiguration
    {
        private Dictionary<Key, KeyboardAction> _keyboardActions;
        public Dictionary<Key, KeyboardAction> KeyboardActions { get => _keyboardActions; set => _keyboardActions = value; }

        private Dictionary<MouseButton, MouseButtonAction> _mouseButtonActions;
        public Dictionary<MouseButton, MouseButtonAction> MouseButtonActions { get => _mouseButtonActions; set => _mouseButtonActions = value; }

        private Action<MouseMove> _moveAction;
        public Action<MouseMove> MoveAction { get => _moveAction; set => _moveAction = value; }

        public bool HasMoveAction { get => _moveAction != null; }

        private Action<MouseScroll> _scrollAction;
        public Action<MouseScroll> ScrollAction { get => _scrollAction; set => _scrollAction = value; }

        public bool HasScrollAction { get => _moveAction != null; }

        public InputConfiguration()
        {
            _keyboardActions = new Dictionary<Key, KeyboardAction>();
            _mouseButtonActions = new Dictionary<MouseButton, MouseButtonAction>();
        }
    }
}
