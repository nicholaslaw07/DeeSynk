﻿using DeeSynk.Core.Components.Input;
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

        private Thread _keyboardListener;
        public Thread KeyboardListener { get => _keyboardListener; }

        private bool _isKeyboardThreadRunning;
        public bool IsKeyboardThreadRunning { get => _isKeyboardThreadRunning; set => _isKeyboardThreadRunning = value; }

        private bool _isMouseThreadRunning;
        public bool IsMouseThreadRunning { get => _isMouseThreadRunning; set => _isMouseThreadRunning = value; }

        private Thread _mouseListener;
        public Thread MouseListener { get => _mouseListener; }

        private InputConfiguration _activeConfig;
        public InputConfiguration ActiveConfig { get => _activeConfig; }

        private Dictionary<string, InputConfiguration> _configurations;
        public Dictionary<string, InputConfiguration> Configurations { get => _configurations; }

        private List<Key> _monitoredKeys;

        private List<KeyPress> _eventList;
        public List<KeyPress> EventList { get => _eventList; }

        private List<MouseClick> _clickList;
        public List<MouseClick> ClickList { get => _clickList; }

        private int _sleep;
        public int Sleep { get => _sleep; }

        private MouseState _ms;

        private Stopwatch _kSw, _mSw;

        private InputManager()
        {
            Load();
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

            _keyboardListener = new Thread(new ThreadStart(KeyboardListen));
            _mouseListener = new Thread(new ThreadStart(MouseListen));

            _monitoredKeys = new List<Key>();
            _eventList = new List<KeyPress>();

            _clickList = new List<MouseClick>();

            _configurations = new Dictionary<string, InputConfiguration>();
        }

        public bool SetConfig(string name)
        {
            //wait until the thread has finished its cycle?
            if (_configurations.TryGetValue(name, out InputConfiguration config))
            {
                _activeConfig = config;
                _monitoredKeys = _activeConfig.KeyboardActions.Keys.ToList();
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
            _keyboardListener.Start();
            _mouseListener.Start();
        }

        private void KeyboardListen() //stopwatch should never need to be restarted in theory
        {
            _isKeyboardThreadRunning = true;
            _kSw.Start();
            do
            {
                var kbs = Keyboard.GetState();

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

                Thread.Sleep(_sleep);
            } while (_isKeyboardThreadRunning);
        }

        private void MouseListen() //stopwatch should never need to be restarted in theory
        {
            _isMouseThreadRunning = true;
            _ms = Mouse.GetState();
            _mSw.Start();
            Thread.Sleep(1);
            do
            {
                var _currentState = Mouse.GetState();
                if(_ms.X != _currentState.X || _ms.Y != _currentState.Y)
                {
                    _activeConfig.MoveAction(new MouseMove((_ms.Y - _currentState.Y), (_ms.X - _currentState.X), _mSw.ElapsedTicks));
                    _ms = _currentState;
                }

                if (_currentState.IsButtonDown(MouseButton.Left))
                {
                    _activeConfig.MouseButtonActions.TryGetValue(MouseButton.Left, out MouseButtonAction action);
                    action.Action(0.0f, new MouseClick(_currentState.X, _currentState.Y, _mSw.ElapsedTicks), new MouseMove((_ms.Y - _currentState.Y), (_ms.X - _currentState.X), _mSw.ElapsedTicks));
                    _ms = _currentState;
                }

                //ADD BUTTON LISTENING
                //MAYBE COMBINE THIS WITH THE OTHER THREAD
                //ADD FUNCTION SO THAT CLICK AND DRAG ON A UI ELEMENT MOVES IT WITH THE MOUSE

                Thread.Sleep(_sleep);
            } while (_isMouseThreadRunning);
        }

        public void UnLoad()
        {
            throw new NotImplementedException();
        }
    }
}
