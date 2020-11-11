using DeeSynk.Core.Components.Input;
using Microsoft.EntityFrameworkCore.Internal;
using OpenTK.Graphics.ES11;
using OpenTK.Input;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common.Input;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DeeSynk.Core.Managers
{
    using InputAction = Components.Input.InputAction;
    public enum PressType : byte
    {
        INVALID = 0,
        Keyboard = 1,
        MouseButton = 2
    }

    public struct Press : IEquatable<Press>
    {
        private PressType _type;
        public PressType Type { get => _type; }

        private Keys _key;
        public Keys Key { get => _key; }

        private MouseButton _button;
        public MouseButton Button { get => _button; }

        public Press(Keys key)
        {
            _type = PressType.Keyboard;
            _key = key;
            _button = MouseButton.Last;
        }

        public Press(MouseButton button)
        {
            _type = PressType.MouseButton;
            _key = Keys.Unknown;
            _button = button;
        }

        public Press(InputAssignment assignment)
        {
            switch (assignment.InputType)
            {
                case (InputType.Keyboard): _type = PressType.Keyboard; _key = assignment.Key; _button = MouseButton.Last; break;
                case (InputType.MouseButton): _type = PressType.MouseButton; _key = Keys.Unknown; _button = assignment.Button; break;
                default: _type = PressType.INVALID; _key = Keys.Unknown; _button = MouseButton.Last; break;
            }
        }

        public bool Equals(Press other)
        {
            if (_type == other._type && _key == other._key && _button == other._button)
                return true;
            return false;
        }

        public static bool operator ==(Press p1, Press p2)
        {
            return p1.Equals(p2);
        }

        public static bool operator !=(Press p1, Press p2)
        {
            return !p1.Equals(p2);
        }

        public override string ToString()
        {
            return $"{_type} {_key} {_button}";
        }
    }

    public class InputEvent
    {
        private Press _press;
        public Press Press { get => _press; }

        private long _time;
        public long Time { get => _time; }

        public InputEvent()
        {
            _press = new Press();
            _time = 0L;
        }

        public InputEvent(Press press, long time)
        {
            _press = press;
            _time = time;
        }
    }

    public class InputManager : IManager
    {
        private static InputManager _inputManager;

        private Thread _inputListener;
        public Thread InputListener { get => _inputListener; }

        private bool _isInputThreadRunning;
        public bool IsInputThreadRunning { get => _isInputThreadRunning; set => _isInputThreadRunning = value; }

        private InputConfiguration _activeConfig;
        public InputConfiguration ActiveConfig { get => _activeConfig; }

        private Dictionary<string, InputConfiguration> _configurations;
        public Dictionary<string, InputConfiguration> Configurations { get => _configurations; }

        private List<InputEvent> _events;

        private int _sleep;
        public int Sleep { get => _sleep; }

        private Vector2 _msRaw;
        public Vector2 MouseStateRaw { get => _msRaw; set => _msRaw = value; }

        private Vector2 _msScreen;
        public Vector2 MouseStateScreen { get => _msScreen; set => _msScreen = value; }

        private Stopwatch _kSw;

        private bool _rawMouseInput;
        public bool RawMouseInput { get => _rawMouseInput; set => _rawMouseInput = value; }

        private List<InputAction> _completeActions;

        private Stopwatch _mouseTimer;

        private Stopwatch _t;
        private long totTime;
        private long count;
        private long _avgTime;
        private long _maxTime;
        public long AverageTime { get => _avgTime; }
        public long MaxTime { get => _maxTime; }

        private MainWindow _window;

        private InputManager(ref MainWindow window)
        {
            _window = window;
            Load();
            _rawMouseInput = true;
        }

        public static ref InputManager GetInstance()
        {
            if (_inputManager == null)
            {
                _inputManager = new InputManager(ref Program.window);
            }

            return ref _inputManager;
        }


        public void Load()
        {
            _kSw = new Stopwatch();

            _inputListener = new Thread(new ThreadStart(InputListen));

            _events = new List<InputEvent>();

            _t = new Stopwatch();
            totTime = 0;
            count = 0;

            _configurations = new Dictionary<string, InputConfiguration>();
            _completeActions = new List<InputAction>();

            _mouseTimer = new Stopwatch();
        }

        public bool SetConfig(string name)
        {
            if (_configurations.TryGetValue(name, out InputConfiguration config))
            {
                _activeConfig = config;
                return true;
            }
            return false;
        }

        public void NewConfig(string name)
        {
            _configurations.Add(name, new InputConfiguration());
        }

        public void StartThreads(int sleep)
        {
            _sleep = sleep;
            _inputListener.Start();
        }

        private void InputListen() //stopwatch should never need to be restarted in theory
        {
            _isInputThreadRunning = true;
            _msRaw = _window.MousePosition;
            _msScreen = new Vector2(_window.Cursor.X, _window.Cursor.Y);
            _kSw.Start();
            _t.Start();
            do
            {
                var kbs = _window.KeyboardState;
                var msRaw = _window.MouseState;
                var msScreen = _window.Cursor;

                var msRAW = _window.MousePosition;
                var msSCREEN = new Vector2(msScreen.X, msScreen.Y);

                long t = _kSw.ElapsedTicks;

                bool mouseMoveRaw = ((_window.MousePosition.X - _msRaw.X) != 0) || ((_window.MousePosition.Y - _msRaw.Y) != 0);
                bool mouseScroll = (_window.MouseState.ScrollDelta) != Vector2.Zero;

                for (int idx = 1; idx < (int)Keys.LastKey; idx++)
                {
                    Keys k = (Keys)idx;
                    var ie = new InputEvent(new Press(k), t);
                    if (kbs.IsKeyDown(k))
                    {
                        if (!IsEventRegistered(k))
                            _events.Add(ie);
                    }
                    else //if(kbs.IsKeyUp(k))
                    {
                        if (IsEventRegistered(k))
                            Remove(k);
                    }
                }

                for (int idx = 0; idx < (int)MouseButton.Last; idx++)
                {
                    MouseButton b = (MouseButton)idx;
                    var ie = new InputEvent(new Press((MouseButton)idx), t);
                    if (_window.IsMouseButtonDown(b))
                    {
                        //Debug.WriteLine(b);
                        if (!IsEventRegistered(b))
                            _events.Add(ie);
                    }
                    else //if(kbs.IsKeyUp(k))
                    {
                        if (IsEventRegistered(b))
                            Remove(b);
                    }
                }

                float mDt = _mouseTimer.ElapsedTicks / 10000000.0f; //converts ticks to seconds
                _mouseTimer.Restart();
                MouseArgs args = new MouseArgs(_activeConfig.RawMouse, _window.MousePosition.X - _msRaw.X, _window.MousePosition.Y - _msRaw.Y, _window.MousePosition.X, _window.MousePosition.Y, msRaw.Scroll, msRaw.ScrollDelta, t);

                foreach (InputAction ia in _activeConfig.InputActions)
                {
                    var inputAction = ia;
                    bool pass = true;
                    var first = inputAction.InputCombination.ElementAt(0);
                    var last = inputAction.InputCombination.ElementAt(inputAction.InputCombination.Count-1);
                    var q = inputAction.Qualifiers;
                    if (first.InputType == InputType.MouseButton)
                    {
                        //Debug.WriteLine($"{args.dX} {args.dY}  {args.X} {args.Y}");
                    }
                    //Debug.WriteLine($"{mouseMoveRaw}  {mouseScroll}");
                    if ((first.InputType == InputType.MouseMove && mouseMoveRaw) || (first.InputType == InputType.MouseScroll && mouseScroll))
                    {
                        inputAction.RunDownActions(mDt, args);
                        continue;
                    }

                    if (!q.HasFlag(Qualifiers.IGNORE_BEFORE)) //This requires that the first key pressed be the first key of the sequence (Only Ctrl+LShift+Y   not   A+Ctrl+Shift+Y)
                    {
                        if (_events.ElementAt(0).Press != new Press(first))
                            continue;
                    }

                    if (!q.HasFlag(Qualifiers.IGNORE_AFTER)) //This requires that the last key pressed be the first key of the sequence (Only Ctrl+LShift+Y   not   Ctrl+Shift+Y+A)
                    {
                        if (_events.ElementAt(_events.Count - 1).Press != new Press(last))
                            continue;
                    }

                    int endOffset = 0;
                    bool mUpdate = (mouseMoveRaw) || (mouseScroll);

                    if (((inputAction.InputCombination).Last().InputType == InputType.MouseMove) || ((inputAction.InputCombination.Last()).InputType == InputType.MouseScroll))
                    {
                        if (mUpdate)
                            endOffset = -1;
                        else
                            continue;
                    }

                    var press = _events.Select(ie => ie.Press).ToList<Press>();
                    int index = press.IndexOf(new Press(first));
                    

                    if (index != -1)
                    {
                        if (q.HasFlag(Qualifiers.NO_ORDER))
                        {
                            foreach (var assign in inputAction.InputCombination)
                            {
                                if (!press.Contains(new Press(assign)))
                                {
                                    pass = false;
                                    break;
                                }
                            }
                        }
                        else if (q.HasFlag(Qualifiers.IN_ORDER))
                        {
                            if (q.HasFlag(Qualifiers.IGNORE_BREAKS))
                            {
                                if (_events.Count() >= inputAction.InputCombination.Count())
                                {
                                    long prevT = 0L;
                                    foreach(InputAssignment a in inputAction.InputCombination)
                                    {
                                        int idx = press.IndexOf(new Press(a));
                                        if(idx != -1)
                                        {
                                            long tt = _events.ElementAt(idx).Time;
                                            if (tt >= prevT)
                                                prevT = tt;
                                            else
                                            {
                                                pass = false;
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if ((_events.Count() - index) >= inputAction.InputCombination.Count())
                                {
                                    long prevT = 0L;
                                    for (int idx = 0; idx < inputAction.InputCombination.Count() + endOffset; idx++)
                                    {
                                        if (_events.ElementAt(index + idx).Press == new Press(inputAction.InputCombination.ElementAt(idx)))
                                        {
                                            long tt = _events.ElementAt(index + idx).Time;
                                            if (tt >= prevT)
                                            {
                                                prevT = tt;
                                            }
                                            else
                                            {
                                                pass = false;
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        pass = false;
                    }

                    if (pass == true)
                    {
                        if (!_completeActions.Contains(inputAction))
                        {
                            if(inputAction.InvokeCriteria.HasFlag(ActionInvoke.Down))
                                inputAction.RunDownActions(mDt, args);
                            _completeActions.Add(inputAction);
                        }
                        else if (_completeActions.Contains(inputAction))
                        {
                            if (inputAction.InvokeCriteria.HasFlag(ActionInvoke.Hold))
                                inputAction.RunHoldActions(mDt, args);
                        }
                    }
                    else
                    {
                        if (_completeActions.Contains(inputAction))
                        {
                            if (inputAction.InvokeCriteria.HasFlag(ActionInvoke.Up))
                                inputAction.RunUpActions(mDt, args);
                            _completeActions.Remove(inputAction);
                        }
                    }
                }

                if (mouseMoveRaw || mouseScroll)
                {
                    _msRaw = msRAW;
                    _msScreen = msSCREEN;
                }

                var elapsed = _t.ElapsedTicks;
                totTime += elapsed;
                count++;
                if (elapsed > _maxTime)
                    _maxTime = elapsed;

                if(count == 1000)
                {
                    _avgTime = totTime / count;
                    count = 0;
                    totTime = 0;
                    _maxTime = 0;
                }
                _t.Reset();

                Thread.Sleep(_sleep);
                _t.Start();
            } while (_isInputThreadRunning);
        }

        private bool IsEventRegistered(Keys k)
        {
            foreach(InputEvent ie in _events)
            {
                if (ie.Press.Type == PressType.Keyboard && ie.Press.Key == k)
                    return true;
            }
            return false;
        }

        private bool IsEventRegistered(MouseButton b)
        {
            foreach (InputEvent ie in _events)
            {
                if (ie.Press.Type == PressType.MouseButton && ie.Press.Button == b)
                    return true;
            }
            return false;
        }

        private bool Remove(Keys k)
        {
            int idx = _events.Select(ie => ie.Press).ToList<Press>().IndexOf(new Press(k));
            if (idx > -1)
            {
                _events.RemoveAt(idx);
                return true;
            }
            return false;
        }

        private bool Remove(MouseButton b)
        {
            int idx = _events.Select(ie => ie.Press).ToList<Press>().IndexOf(new Press(b));
            if (idx > -1)
            {
                _events.RemoveAt(idx);
                return true;
            }
            return false;
        }

        public void UnLoad()
        {
            throw new NotImplementedException();
        }
    }
}

//Add error checking on the qualifiers when InputAction is constructed
//Remove the linked lists from the constructor
//Add a validate method that ensures nothing bad will happen.  Throws an exception if the user did not set up the configuration properly.
//Set up everything so that actions can be added after they are created?
//Add in the ability to load configurations.  We shouldn't have all of the configuration setup spread everywhere.
//Turn MouseArgs into a general inputArgs class so that key and button presses are passed.

//Add a general input event.
//This will allow for the detection of mouse movement, mouse buttons, and key presses simultaneously.
//The main challenge will be to find a way to distinguish say W versus Ctrl+W.\
//This will require that keys be pressed in the correct order.
//This is what the modifier keys are for.
//While me way not neccessarily disallow it.  We should establish convention that only modifier keys are used
//in combination with other keys.
//In the case where we have a key pair and single key that both begin with the same key then the first key will active and
//once the second key of the pair is added to the combo then the function for the pair will activate.
//We also need to add the ability to add multiple Actions to a single keypress.

//Update events list with up and down for kb and mb
//Check if actions are satisfied and compare to dictionary from previous loop.
//If action status is different, update dictionary to reflect this or simply refresh dictionary each loop.
//Ensure that the checks account for all qualifiers.
//After each check ensure that all necessary actions are completed depending on the state change type (up, down, hold)
//Get rid of old garbage code and update all action methods to include the general parameters.

//Add auto detect sleep if code takes more than a certain period of time to compelte.

//Ensure that this can be optimized also.  The principle seems inefficient as it is.  40-50 microsends seems excessive.


//Updates the current state to match all inputs that occured between updates.
