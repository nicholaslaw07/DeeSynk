using DeeSynk.Core.Components;
using DeeSynk.Core.Components.Input;
using OpenTK;
using OpenTK.Graphics.ES11;
using OpenTK.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeeSynk.Core.Systems
{
    public enum EventType
    {
        KeyUp = 0,
        KeyDown = 1,
        KeyPress = 2
    }

    public struct InputEvent
    {
        KeyboardKeyEventArgs Args;
        EventType Type;
        long Time;

        public InputEvent(KeyboardKeyEventArgs args, EventType type, long time)
        {
            Args = args;
            Type = type;
            Time = time;
        }
    }

    public struct InputPress
    {
        char Key;
        long Time;

        public InputPress(char key, long time)
        {
            Key = key;
            Time = time;
        }
    }
    public class SystemInput : ISystem
    {
        public Component MonitoredComponents => throw new NotImplementedException();

        private const float v = 1f;

        private Vector3 V_W = new Vector3(0.0f, 0.0f, -v);
        private Vector3 V_S = new Vector3(0.0f, 0.0f, v);
        private Vector3 V_A = new Vector3(-v, 0.0f, 0.0f);
        private Vector3 V_D = new Vector3(v, 0.0f, 0.0f);
        private Vector3 V_Up = new Vector3(0.0f, v, 0.0f);
        private Vector3 V_Dn = new Vector3(0.0f, -v, 0.0f);


        private List<Key> _monitoredKeys;

        private World _world;
        private UI _ui;

        private MouseInputQueue _mouseInput;
        public MouseInputQueue MouseInput { get => _mouseInput; }

        private KeyboardInputQueue _keyboardInput;
        public KeyboardInputQueue KeyboardInput { get => _keyboardInput; }

        private Camera _camera; //only used if direct input is on.  this dramatically reduces latency.

        private bool _shutDownProgram;
        public bool ShutDownProgram { get => _shutDownProgram; }

        private Stopwatch sw;


        public SystemInput(ref World world, ref UI ui, ref MouseInputQueue mouseInput)
        {
            _world = world;
            _ui = ui;
            _mouseInput = mouseInput;

            _monitoredKeys = new List<Key>();

            
            _monitoredKeys.Add(Key.W);
            _monitoredKeys.Add(Key.A);
            _monitoredKeys.Add(Key.S);
            _monitoredKeys.Add(Key.D);

            _monitoredKeys.Add(Key.ShiftLeft);
            _monitoredKeys.Add(Key.Space);

            _monitoredKeys.Add(Key.Escape);

            _keyboardInput = new KeyboardInputQueue(ref _monitoredKeys);

            sw = new Stopwatch();
            sw.Start();
        }

        public void StartThreads()
        {
            _keyboardInput.StartMonitorThread(0);
            //Add mouse thread
        }

        public void PushCameraRef(ref Camera camera)
        {
            _camera = camera;
        }

        private void MouseMoveToCameraLook()
        {
            if (!_mouseInput.UsingDirectMouseMove)
            {
                //Do operations for non direct input
                //Pull all deltas
                //Add deltas to rotation
                //Clear deltas
            }
        }

        private void KeyboardMoveToCameraMove(float time)
        {
            if (!_keyboardInput.UsingDirectKeyboardMove)
            {
                if (_keyboardInput.KeysInEventList.Contains(Key.W))
                {
                    var t = _keyboardInput.GetTimeForAllInstances(Key.W);
                    _camera.AddLocation(ref V_W, t);
                    _keyboardInput.RemoveAllInstances(Key.W, _keyboardInput.DownKeys.Contains(Key.W));
                }
                if (_keyboardInput.KeysInEventList.Contains(Key.A))
                {
                    var t = _keyboardInput.GetTimeForAllInstances(Key.A);
                    _camera.AddLocation(ref V_A, t);
                    _keyboardInput.RemoveAllInstances(Key.A, _keyboardInput.DownKeys.Contains(Key.A));
                }
                if (_keyboardInput.KeysInEventList.Contains(Key.S))
                {
                    var t = _keyboardInput.GetTimeForAllInstances(Key.S);
                    _camera.AddLocation(ref V_S, t);
                    _keyboardInput.RemoveAllInstances(Key.S, _keyboardInput.DownKeys.Contains(Key.S));
                }
                if (_keyboardInput.KeysInEventList.Contains(Key.D))
                {
                    var t = _keyboardInput.GetTimeForAllInstances(Key.D);
                    _camera.AddLocation(ref V_D, t);
                    _keyboardInput.RemoveAllInstances(Key.D, _keyboardInput.DownKeys.Contains(Key.D));
                }
                if (_keyboardInput.KeysInEventList.Contains(Key.Space))
                {
                    var t = _keyboardInput.GetTimeForAllInstances(Key.Space);
                    _camera.AddLocation(ref V_Up, t);
                    _keyboardInput.RemoveAllInstances(Key.Space, _keyboardInput.DownKeys.Contains(Key.Space));
                }
                if (_keyboardInput.KeysInEventList.Contains(Key.LShift))
                {
                    var t = _keyboardInput.GetTimeForAllInstances(Key.LShift);
                    _camera.AddLocation(ref V_Dn, t);
                    _keyboardInput.RemoveAllInstances(Key.LShift, _keyboardInput.DownKeys.Contains(Key.LShift));
                }

            }
        }
        private void KeyboardToExitWindow()
        {
            if (_keyboardInput.KeysInEventList.Contains(Key.Escape))
            {
                _shutDownProgram = true;
                _keyboardInput.IsRunning = false;
            }
        }

        public void Update(float time)
        {
            MouseMoveToCameraLook();
            KeyboardToExitWindow();
            KeyboardMoveToCameraMove(time);
            _mouseInput.Clear();
        }
    }
}

/*
var keyState = Keyboard.GetState();

if (keyState.IsKeyDown(Key.Escape))
    _shutDownProgram = true;
if (keyState.IsKeyDown(Key.W))
    _camera.AddLocation(ref V_W, time);
if (keyState.IsKeyDown(Key.S))
    _camera.AddLocation(ref V_S, time);
if (keyState.IsKeyDown(Key.A))
    _camera.AddLocation(ref V_A, time);
if (keyState.IsKeyDown(Key.D))
    _camera.AddLocation(ref V_D, time);
if (keyState.IsKeyDown(Key.Space))
    _camera.AddLocation(ref V_Up, time);
if (keyState.IsKeyDown(Key.ShiftLeft))
    _camera.AddLocation(ref V_Dn, time);*/

/*if (!_keyboardInput.UsingDirectKeyboardMove)
{
    if (_keyboardInput.KeysInEventList.Contains(Key.W))
    {
        _camera.AddLocation(ref V_W, time);
        _keyboardInput.RemoveAllInstances(Key.W);
    }
                /*
                if (_keyboardInput.KeysInEventList.Contains(Key.A))
                {
                    _camera.AddLocation(ref V_A, time); //_keyboardInput.GetTimeForAllInstances(Key.A)
                    _keyboardInput.RemoveAllInstances(Key.A);
                }
                if (_keyboardInput.KeysInEventList.Contains(Key.W))
                {
                    _camera.AddLocation(ref V_S, time);
                    _keyboardInput.RemoveAllInstances(Key.S);
                }
                if (_keyboardInput.KeysInEventList.Contains(Key.D))
                {
                    _camera.AddLocation(ref V_D, time);
                    _keyboardInput.RemoveAllInstances(Key.D);
                }

                if (_keyboardInput.KeysInEventList.Contains(Key.Space))
                {
                    _camera.AddLocation(ref V_Up, time);
                    _keyboardInput.RemoveAllInstances(Key.Space);
                }
                if (_keyboardInput.KeysInEventList.Contains(Key.LShift))
                {
                    _camera.AddLocation(ref V_Dn, time);
                    _keyboardInput.RemoveAllInstances(Key.LShift);
                }*/

//if (_keyboardInput.KeysInEventList.Contains(Key.Escape))

//_shutDownProgram = true;
//_keyboardInput.Listener.Abort();
//_keyboardInput.IsRunning = false;
