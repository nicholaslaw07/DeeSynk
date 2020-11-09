using DeeSynk.Core.Components.Input;
using Microsoft.EntityFrameworkCore.Internal;
using OpenTK.Graphics.ES11;
using OpenTK.Input;
using System;
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
    public enum PressType : byte
    {
        INVALID = 0,
        Keyboard = 1,
        MouseButton = 2
    }

    public class InputEvent
    {
        private PressType _pressType;
        public PressType PressType { get => _pressType; }

        private Key _key;
        public Key Key { get => _key; }

        private MouseButton _button;
        public MouseButton Button { get => _button; }

        public InputEvent()
        {
            _pressType = PressType.INVALID;
        }

        public InputEvent(Key key)
        {
            _pressType = PressType.Keyboard;
            _key = key;
        }

        public InputEvent(MouseButton button)
        {
            _pressType = PressType.MouseButton;
            _button = button;
        }

        public InputEvent(InputAssignment assignment)
        {
            if (assignment.InputType == InputType.Keyboard)
            {
                _pressType = PressType.Keyboard;
                _key = assignment.Key;
            }
            else if (assignment.InputType == InputType.MouseButton)
            {
                _pressType = PressType.MouseButton;
                _button = assignment.Button;
            }
            else
                _pressType = PressType.INVALID;
        }
    }

    public class InputManager : IManager
    {
        //Can we justify this???
        //We could load control sets and schemes that then bind serialized actions to control configurations
        //This would make the Input a more global thing.  This removes the need possible for some functionality of system input.
        //This also allows us to more easily bind functions to the MainWindow.
        //This paves the way for action and delegate bindings which goes slightly against ECS but removes the need for constantly scanning all objects.
        //However this could also be harmful.
        //I guess the issue is that I'm having a hard time justifying SystemInput or InputManager.

        private static InputManager _inputManager;

        private Thread _inputListener;
        public Thread InputListener { get => _inputListener; }

        private bool _isInputThreadRunning;
        public bool IsInputThreadRunning { get => _isInputThreadRunning; set => _isInputThreadRunning = value; }

        private InputConfiguration _activeConfig;
        public InputConfiguration ActiveConfig { get => _activeConfig; }

        private Dictionary<string, InputConfiguration> _configurations;
        public Dictionary<string, InputConfiguration> Configurations { get => _configurations; }

        private Dictionary<InputEvent, long> _events;
        public Dictionary<InputEvent, long> Events { get => _events; }

        private int _sleep;
        public int Sleep { get => _sleep; }

        private MouseState _msRaw;
        public MouseState MouseStateRaw { get => _msRaw; set => _msRaw = value; }

        private MouseState _msScreen;
        public MouseState MouseStateScreen { get => _msScreen; set => _msScreen = value; }

        private Stopwatch _kSw, _mSw;

        private bool _rawMouseInput;
        public bool RawMouseInput { get => _rawMouseInput; set => _rawMouseInput = value; }

        private List<InputAction> _completeActions;

        private Stopwatch _mouseTimer;

        private Stopwatch _t;
        private long totTime;
        private long count;

        private InputManager()
        {
            Load();
            _rawMouseInput = true;
        }

        public static ref InputManager GetInstance()
        {
            if (_inputManager == null)
            {
                _inputManager = new InputManager();
            }

            return ref _inputManager;
        }


        public void Load()
        {
            _kSw = new Stopwatch();
            _mSw = new Stopwatch();

            _inputListener = new Thread(new ThreadStart(InputListen));

            _events = new Dictionary<InputEvent, long>();

            _t = new Stopwatch();
            totTime = 0;
            count = 0;

            _configurations = new Dictionary<string, InputConfiguration>();
            _completeActions = new List<InputAction>();

            _mouseTimer = new Stopwatch();
        }

        public bool SetConfig(string name)
        {
            //wait until the thread has finished its cycle?
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

        private void InputListen() //stopwatch should never need to be restarted in theory
        {
            _isInputThreadRunning = true;
            _msRaw = Mouse.GetState();
            _msScreen = Mouse.GetCursorState();
            _kSw.Start();
            _mSw.Start();
            _t.Start();
            do
            {
                var kbs = Keyboard.GetState();
                var msRaw = Mouse.GetState();
                var msScreen = Mouse.GetCursorState();

                long t = _kSw.ElapsedTicks;

                bool mouseMoveRaw = ((msRaw.X - _msRaw.X) != 0) || ((msRaw.Y - _msRaw.Y) != 0);
                bool mouseMoveScreen = ((msScreen.X - _msScreen.X) != 0) || ((msScreen.Y - _msScreen.Y) != 0);
                bool mouseScroll = (msRaw.ScrollWheelValue - _msRaw.ScrollWheelValue) != 0;

                //Console.WriteLine("{0} {1} {2} {3} {4}   {5} {6} {7} {8} {9}", _msRaw.X, _msRaw.Y, msRaw.X, msRaw.Y, mouseMoveRaw, _msScreen.X, _msScreen.Y, msScreen.X, msScreen.Y, mouseMoveScreen);
                Console.WriteLine(_events.Keys.Count());

                for (int idx = 1; idx < 132; idx++)
                {
                    var ie = new InputEvent((Key)idx);
                    if (kbs.IsKeyDown((Key)idx))
                    {
                        if (_events.Keys.Where(ke => ke.PressType == PressType.Keyboard).Where(k => k.Key == (Key)idx).Count() == 0)
                            _events.Add(ie, t);
                    }
                    else if(kbs.IsKeyUp((Key)idx))
                    {
                        if (_events.TryGetValue(ie, out long tt))
                        {
                            if (_events.Remove(ie))
                                Console.WriteLine("yes");
                        }
                    }
                }

                var _keys = _events.Keys;
                for (int idx = 0; idx < 13; idx++)
                {
                    var ie = new InputEvent((MouseButton)idx);
                    if (msRaw.IsButtonDown((MouseButton)idx))
                    {
                        if (!_events.Keys.Contains(ie))
                            _events.Add(ie, t);
                    }
                    else
                    {
                        if (_events.Keys.Contains(ie))
                            _events.Remove(ie);
                    }
                }

                //Add error checking on InputAction so that certain qualifiers aren't paired with mouse movements and such (NO_ORDER)

                float mDt = _mouseTimer.ElapsedTicks / 10000000.0f; //converts ticks to seconds
                _mouseTimer.Restart();
                MouseArgs mArgs = (_rawMouseInput) ? new MouseArgs(_rawMouseInput, msRaw.X - _msRaw.X, msRaw.Y - _msRaw.Y, msRaw.X, msRaw.Y, msRaw.ScrollWheelValue, msRaw.ScrollWheelValue - _msRaw.ScrollWheelValue, t) :
                    new MouseArgs(_rawMouseInput, msScreen.X - _msScreen.X, msScreen.Y - _msScreen.Y, msScreen.X, msScreen.Y, _msScreen.ScrollWheelValue, _msScreen.ScrollWheelValue - _msScreen.ScrollWheelValue, t);

                foreach (InputAction inputAction in _activeConfig.InputActions)
                {
                    bool pass = true;
                    var node = inputAction.InputCombination.First;
                    var first = node.Value;
                    var lastNode = inputAction.InputCombination.Last;
                    var last = lastNode.Value;
                    bool mUpdate = (first.InputType == InputType.MouseMove && (mouseMoveRaw || mouseMoveScreen)) || (first.InputType == InputType.MouseScroll && mouseScroll);
                    if (mUpdate)
                    {
                        inputAction.RunDownActions(mDt, mArgs);
                        continue;
                    }

                    if (!inputAction.Qualifiers.HasFlag(Qualifiers.IGNORE_BEFORE))
                    {
                        if (_events.Keys.First() != new InputEvent(first))
                            continue;
                    }

                    if (!inputAction.Qualifiers.HasFlag(Qualifiers.IGNORE_AFTER))
                    {
                        if (_events.Keys.Last() != new InputEvent(last))
                            continue;
                    }

                    int endOffset = 0;
                    if (last.InputType == InputType.MouseMove || last.InputType == InputType.MouseScroll)
                    {
                        if (mUpdate)
                            endOffset = -1;
                        else
                            continue;
                    }

                    int index = _events.Keys.IndexOf(new InputEvent(first));

                    if(index != -1)
                    {
                        if (inputAction.Qualifiers.HasFlag(Qualifiers.IGNORE_BREAKS))
                        {
                            if(_events.Count() >= inputAction.InputCombination.Count())
                            {
                                long prevT = 0L;
                                for (var n = node; (endOffset == -1) ? ((n.Next != null) ? n.Next.Next : null ) != null : n.Next != null; n = n.Next)
                                {
                                    int idx = _events.Keys.IndexOf(new InputEvent(n.Value));
                                    if(idx != -1)
                                    {
                                        long tt = _events.Values.ElementAt(idx);
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
                        else if (inputAction.Qualifiers.HasFlag(Qualifiers.NO_ORDER))
                        {
                            foreach(var assign in inputAction.InputCombination)
                            {
                                if(!_events.Keys.Contains(new InputEvent(assign)))
                                {
                                    pass = false;
                                    break;
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
                                    if (_events.Keys.ElementAt(index + idx) == new InputEvent(node.Value))
                                    {
                                        long tt = _events.Values.ElementAt(index + idx);
                                        if (tt >= prevT)
                                        {
                                            prevT = tt;
                                            node = node.Next;
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
                    else
                    {
                        pass = false;
                    }

                    if (pass == true)
                    {
                        if (!_completeActions.Contains(inputAction))
                        {
                            inputAction.RunDownActions(mDt, mArgs);
                            _completeActions.Add(inputAction);
                        }
                        else if (_completeActions.Contains(inputAction))
                            inputAction.RunHoldActions(mDt, mArgs);
                    }
                    else
                    {
                        if (_completeActions.Contains(inputAction))
                        {
                            inputAction.RunUpActions(mDt, mArgs);
                            _completeActions.Remove(inputAction);
                        }
                        continue;
                    }
                    Console.WriteLine(pass);
                }

                _msRaw = msRaw;
                _msScreen = msScreen;

                totTime += _t.ElapsedTicks;
                count++;
                if(count == 1000)
                {
                    Console.WriteLine(totTime / count / 10000.0f);
                    count = 0;
                    totTime = 0;
                }
                _t.Reset();

                Thread.Sleep(_sleep);
                _t.Start();
            } while (_isInputThreadRunning);
        }

        public void UnLoad()
        {
            throw new NotImplementedException();
        }
    }
}

/*
foreach(Key k in _monitoredKeys)
{
    if (_activeConfig.KeyboardActions.TryGetValue(k, out KeyboardAction keyAction))
    {
        if (kbs.IsKeyDown(k))
        {
            if (!_eventList.Select(ke => ke.Key).Contains(k))
            {
                _eventList.Add(new KeyPress(k, _kSw.ElapsedTicks));
                if (keyAction.Type == KeyActionType.SinglePress)
                    keyAction.Action(_kSw.ElapsedTicks / 10000000.0f);
            }
            else
            {
                if (keyAction.Type == KeyActionType.RepeatOnHold)
                {
                    var keyEvent = _eventList.Where(ke => ke.Key == k).First();
                    long time = _kSw.ElapsedTicks;
                    float elapsed = (time - keyEvent.Time) / 10000000.0f;
                    keyAction.Action(elapsed);
                    _eventList.Remove(keyEvent);
                    _eventList.Add(new KeyPress(k, time));
                }
            }
        }
        if (_eventList.Select(ke => ke.Key).Contains(k))
        {
            if (kbs.IsKeyUp(k))
            {
                var keyEvent = _eventList.Where(ke => ke.Key == k).First();
                if (keyAction.Type == KeyActionType.RepeatOnHold || keyAction.Type == KeyActionType.WaitForRelease)
                {
                    long time = _kSw.ElapsedTicks;
                    float elapsed = (time - keyEvent.Time) / 10000000.0f;
                    keyAction.Action(elapsed);
                    _eventList.Remove(keyEvent);
                }
                else
                    _eventList.Remove(keyEvent);
            }
        }
    }
}

if (_rawMouseInput)
{
    var _currentState = Mouse.GetState();
    if (_msRaw.X != _currentState.X || _msRaw.Y != _currentState.Y)
    {
        _activeConfig.MoveAction(new MouseMove((_msRaw.Y - _currentState.Y), (_msRaw.X - _currentState.X), _mSw.ElapsedTicks));
        _msRaw = _currentState;
    }

    if (_currentState.IsButtonDown(MouseButton.Left))
    {
        _activeConfig.MouseButtonActions.TryGetValue(MouseButton.Left, out MouseButtonAction action);
        action.Action(0.0f, new MousePosition(_currentState.X, _currentState.Y, _mSw.ElapsedTicks), new MouseMove((_msRaw.Y - _currentState.Y), (_msRaw.X - _currentState.X), _mSw.ElapsedTicks));
        _msRaw = _currentState;
    }
}
else //cursor input
{
    var _currentState = Mouse.GetCursorState();
    if (_msRaw.X != _currentState.X || _msRaw.Y != _currentState.Y)
    {
        _activeConfig.MoveAction(new MouseMove((_msRaw.Y - _currentState.Y), (_msRaw.X - _currentState.X), _mSw.ElapsedTicks));
    }

    if (_currentState.IsButtonDown(MouseButton.Left))
    {
        _activeConfig.MouseButtonActions.TryGetValue(MouseButton.Left, out MouseButtonAction action);
        action.Action(0.0f, new MousePosition(_currentState.X, _currentState.Y, _mSw.ElapsedTicks), new MouseMove((_currentState.X - _msRaw.X), (_msRaw.Y - _currentState.Y), _mSw.ElapsedTicks));
    }

    _msRaw = _currentState;
}*/
