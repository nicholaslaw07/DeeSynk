using OpenTK.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;
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

    public struct MousePosition
    {
        int _x;
        public int X { get => _x; }
        int _y;
        public int Y { get => _y; }

        long _time;
        public long Time { get => _time; }

        public MousePosition(int x, int y, long time)
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

        int _scrollDelta;
        public int ScrollDelta { get => _scrollDelta; }

        long _time;
        public long Time { get => _time; }

        public MouseScroll(int scrollValue, int scrollDelta, long time)
        {
            _scrollValue = scrollValue;
            _scrollDelta = scrollDelta;
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
        private Action<float, MousePosition, MouseMove> _action;
        public Action<float, MousePosition, MouseMove> Action { get => _action; }

        private MouseButton _button;
        public MouseButton Button { get => _button; }

        private MouseActionType _buttonActionType;
        public MouseActionType ButtonAction { get => _buttonActionType; }

        public MouseButtonAction(Action<float, MousePosition, MouseMove> action, MouseButton button, MouseActionType buttonActionType)
        {
            _action = action;
            _button = button;
            _buttonActionType = buttonActionType;
        }
    }

    //+++++----------+++++//

    public enum InputType : byte
    {
        MouseMove = 0,
        Keyboard = 1,
        MouseButton = 2,
        MouseScroll = 3
    }

    [Flags]
    public enum ActionInvoke : byte
    {
        Down = 1,
        Hold = 1 << 1,
        Up = 1 << 2
    }

    [Flags]
    public enum Qualifiers : byte
    {
        DEFAULT = 0,  //Default option is that all keys must be pressed in order with no other keys inbetween
        NO_ORDER = 1,
        IGNORE_BREAKS = 1 << 1,
        IGNORE_BEFORE = 1 << 2,
        IGNORE_AFTER = 1 << 3
    }

    public struct MouseArgs
    {
        bool _rawMouse;
        public bool RawMouse { get => _rawMouse; }

        int _dx;
        public int dX { get => _dx; }

        int _dy;
        public int dY { get => _dy; }

        int _x;
        public int X { get => _x; }

        int _y;
        public int Y { get => _y; }

        int _scrollValue;
        public int ScrollValue { get => _scrollValue; }

        int _scrollDelta;
        public int ScrollDelta { get => _scrollDelta; }

        long _time;
        public long Time { get => _time; }

        public MouseArgs(bool rawMouse, int dx, int dy, int x, int y, int scroll, int dScroll, long time)
        {
            _rawMouse = rawMouse;
            _dx = dx;
            _dy = dy;
            _x = x;
            _y = y;
            _scrollValue = scroll;
            _scrollDelta = dScroll;
            _time = time;
        }
    }

    public class InputAssignment
    {
        private InputType _inputType;
        public InputType InputType { get => _inputType; }

        private Key _key;
        public Key Key
        {
            get
            {
                if (_inputType == InputType.Keyboard)
                    return _key;
                else
                    throw new Exception("Invalid query: this assignment does not match InputType.Keyboard.");
            }
        }

        private MouseButton _button;
        public MouseButton Button
        {
            get
            {
                if (_inputType == InputType.MouseButton)
                    return _button;
                else
                    throw new Exception("Invalid query: this assignment does not match InputType.MouseButton.");
            }
        }

        public InputAssignment(InputType type)
        {
            if (type == InputType.MouseMove || type == InputType.MouseScroll)
                _inputType = type;
            else
                throw new ArgumentException("Invalid input type: InputType.MouseMove or InputType.MouseScroll expected.");
        }

        public InputAssignment(InputType type, Key key)
        {
            if (type == InputType.Keyboard)
            {
                _inputType = type;
                _key = key;
            }
            else
                throw new ArgumentException("Invalid input type: InputType.Keyboard expected.");
        }

        public InputAssignment(InputType type, MouseButton button)
        {
            if (type == InputType.MouseButton)
            {
                _inputType = type;
                _button = button;
            }
            else
                throw new ArgumentException("Invalid input type: InputType.MouseButton expected.");
        }

        public InputAssignment(InputType type, Key key, MouseButton button)
        {
            if (type == InputType.Keyboard)
            {
                _inputType = type;
                _key = key;
            }
            else if (type == InputType.MouseButton)
            {
                _inputType = type;
                _button = button;
            }
            else
                throw new ArgumentException("Invalid input type: InputType.Keyboard or InputType.MouseButton expected.");
        }
    }

    //As a general convention, mouse movement should come after an input.  If mouse move comes first then this can cause issues.
    public class InputAction
    {
        private Qualifiers _qualifiers;
        /// <summary>
        /// Determines how the inputs are registered for a successful combination.
        /// </summary>
        public Qualifiers Qualifiers { get => _qualifiers; }

        private ActionInvoke _invokeCriteria;
        /// <summary>
        /// Gauges the situation in which the actions are invoked.
        /// </summary>
        public ActionInvoke InvokeCriteria { get => _invokeCriteria; }

        private LinkedList<InputAssignment> _inputCombination;
        /// <summary>
        /// Represents the order in which keys must be pressed and held (if qualifier flags is selected) in order to active the associated actions.
        /// </summary>
        public LinkedList<InputAssignment> InputCombination { get => _inputCombination; }

        private LinkedList<Action<float, MouseArgs>> _downActions;
        /// <summary>
        /// Represents all actions that are performed when the combination is initially satisfied.  These are in order of execution from first to last.
        /// </summary>
        public LinkedList<Action<float, MouseArgs>> DownActions { get => _downActions; set => _downActions = value; }

        private LinkedList<Action<float, MouseArgs>> _holdActions;
        /// <summary>
        /// Represents all actions that are performed when the combination is maintained.  These are in order of execution from first to last.
        /// </summary>
        public LinkedList<Action<float, MouseArgs>> HoldActions { get => _holdActions; set => _holdActions = value; }

        private LinkedList<Action<float, MouseArgs>> _upActions;
        /// <summary>
        /// Represents all actions that are performed when the combination is released.  These are in order of execution from first to last.
        /// </summary>
        public LinkedList<Action<float, MouseArgs>> UpActions { get => _upActions; set => _upActions = value; }

        public InputAction()
        {
            _inputCombination = new LinkedList<InputAssignment>();
            _downActions = new LinkedList<Action<float, MouseArgs>>();
            _holdActions = new LinkedList<Action<float, MouseArgs>>();
            _upActions = new LinkedList<Action<float, MouseArgs>>();
        }

        public InputAction(Qualifiers qualifiers, ActionInvoke invokeCriteria, LinkedList<InputAssignment> inputCombination, LinkedList<Action<float, MouseArgs>> downActions, LinkedList<Action<float, MouseArgs>> holdActions, LinkedList<Action<float, MouseArgs>> upActions)
        {
            if (inputCombination.Count() > 1 && (inputCombination.First().InputType == InputType.MouseScroll || inputCombination.First().InputType == InputType.MouseMove))
                throw new ArgumentException("Mouse scroll or mouse move cannot be the first argument in a combination.");

            _qualifiers = qualifiers;
            _invokeCriteria = invokeCriteria;
            _inputCombination = inputCombination;
            _downActions = downActions;
            _holdActions = holdActions;
            _upActions = upActions;
        }

        public InputAction(Qualifiers qualifiers, ActionInvoke invokeCriteria, LinkedList<InputAssignment> inputCombination, LinkedList<Action<float, MouseArgs>> actions)
        {
            if (inputCombination.Count() > 1 && (inputCombination.First().InputType == InputType.MouseScroll || inputCombination.First().InputType == InputType.MouseMove))
                throw new ArgumentException("Mouse scroll or mouse move cannot be the first argument in a combination.");

            _qualifiers = qualifiers;
            _invokeCriteria = invokeCriteria;
            _inputCombination = inputCombination;

            switch (invokeCriteria)
            {
                case (ActionInvoke.Down): _downActions = actions; _holdActions = new LinkedList<Action<float, MouseArgs>>(); _upActions = new LinkedList<Action<float, MouseArgs>>(); break;
                case (ActionInvoke.Hold): _holdActions = actions; _downActions = new LinkedList<Action<float, MouseArgs>>(); _upActions = new LinkedList<Action<float, MouseArgs>>(); break;
                case (ActionInvoke.Up): _upActions = actions; _downActions = new LinkedList<Action<float, MouseArgs>>(); _holdActions = new LinkedList<Action<float, MouseArgs>>(); break;
            }
        }

        public void RunDownActions(float time, MouseArgs mArg)
        {
            foreach (var a in _downActions)
                a(time, mArg);
        }

        public void RunHoldActions(float time, MouseArgs mArg)
        {
            foreach (var a in _holdActions)
                a(time, mArg);
        }

        public void RunUpActions(float time, MouseArgs mArg)
        {
            foreach (var a in _upActions)
                a(time, mArg);
        }
    }

    public class InputConfiguration
    {
        /*private Dictionary<Key, KeyboardAction> _keyboardActions;
        public Dictionary<Key, KeyboardAction> KeyboardActions { get => _keyboardActions; set => _keyboardActions = value; }

        private Dictionary<MouseButton, MouseButtonAction> _mouseButtonActions;
        public Dictionary<MouseButton, MouseButtonAction> MouseButtonActions { get => _mouseButtonActions; set => _mouseButtonActions = value; }

        private Action<MouseMove> _moveAction;
        public Action<MouseMove> MoveAction { get => _moveAction; set => _moveAction = value; }

        private Action<MouseScroll> _scrollAction;
        public Action<MouseScroll> ScrollAction { get => _scrollAction; set => _scrollAction = value; }*/


        //+++++---------+++++//

        private bool _rawMouse;
        public bool RawMouse { get => _rawMouse; set => _rawMouse = value; }

        private LinkedList<InputAction> _inputActions;
        public LinkedList<InputAction> InputActions { get => _inputActions; set => _inputActions = value; }

        private bool _hasMoveAction;
        public bool HasMoveAction { get => _hasMoveAction; }

        private bool _hasScrollAction;
        public bool HasScrollAction { get => _hasScrollAction; }

        private bool _hasButtonAction;
        public bool HasButtonAction { get => _hasButtonAction; }

        private bool _hasKeyAction;
        public bool HasKeyAction { get => _hasKeyAction; }

        public InputConfiguration()
        {
            _inputActions = new LinkedList<InputAction>();
        }

        public void UpdateActionState()
        {
            _hasMoveAction = false;
            _hasScrollAction = false;
            _hasButtonAction = false;
            _hasMoveAction = false;

            foreach(var action in _inputActions)
            {
                foreach(var assignment in action.InputCombination)
                {
                    switch (assignment.InputType)
                    {
                        case (InputType.MouseMove): _hasMoveAction |= true; break;
                        case (InputType.Keyboard): _hasKeyAction |= true; break;
                        case (InputType.MouseButton): _hasButtonAction |= true; break;
                        case (InputType.MouseScroll): _hasScrollAction|= true; break;
                    }
                }
            }
        }
    }
}
