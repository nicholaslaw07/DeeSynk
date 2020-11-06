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
        public KeyActionType KeyActionType { get => _keyActionType; }

        public KeyboardAction(Action<float> action, Key key, KeyActionType keyActionType)
        {
            _action = action;
            _key = key;
            _keyActionType = keyActionType;
        }
    }

    public class InputConfiguration
    {
        private Dictionary<Key, KeyboardAction> _keyboardActions;
        public Dictionary<Key, KeyboardAction> KeyboardActions { get => _keyboardActions; set => _keyboardActions = value; }

        private Action<MouseMove> _moveAction;
        public Action<MouseMove> MoveAction { get => _moveAction; set => _moveAction = value; }

        public InputConfiguration()
        {
            _keyboardActions = new Dictionary<Key, KeyboardAction>();
        }
    }
}
