using DeeSynk.Core.Components;
using DeeSynk.Core.Components.Input;
using DeeSynk.Core.Managers;
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

        private World _world;
        private UI _ui;

        private Camera _camera; //only used if direct input is on.  this dramatically reduces latency.

        private bool _shutDownProgram;
        public bool ShutDownProgram { get => _shutDownProgram; }

        private Stopwatch sw;

        public delegate void k1();

        private Action<float> w, a, s, d, sp, ls, esc;
        private Action<MouseMove> mouseMove;


        public SystemInput(ref World world, ref UI ui, ref Camera camera)
        {
            _world = world;
            _ui = ui;

            sw = new Stopwatch();
            sw.Start();

            _camera = camera;

            w = CameraMoveFront;
            s = CameraMoveBack;
            a = CameraMoveLeft;
            d = CameraMoveRight;
            sp = CameraMoveUp;
            ls = CameraMoveDown;
            esc = KeyboardToExitWindow;

            mouseMove = CameraRotation;

            InputConfiguration config = new InputConfiguration();
            config.KeyboardActions.Add(Key.W, new KeyboardAction(w, Key.W, KeyActionType.RepeatOnHold));
            config.KeyboardActions.Add(Key.S, new KeyboardAction(s, Key.S, KeyActionType.RepeatOnHold));
            config.KeyboardActions.Add(Key.A, new KeyboardAction(a, Key.A, KeyActionType.RepeatOnHold));
            config.KeyboardActions.Add(Key.D, new KeyboardAction(d, Key.D, KeyActionType.RepeatOnHold));
            config.KeyboardActions.Add(Key.Space, new KeyboardAction(sp, Key.Space, KeyActionType.RepeatOnHold));
            config.KeyboardActions.Add(Key.LShift, new KeyboardAction(ls, Key.LShift, KeyActionType.RepeatOnHold));
            config.KeyboardActions.Add(Key.Escape, new KeyboardAction(esc, Key.Escape, KeyActionType.SinglePress));
            config.MoveAction = mouseMove;
            var im = InputManager.GetInstance();
            im.Configurations.Add("primary move", config);
            im.SetConfig("primary move");

            im.StartThreads(1);
        }

        public void StartThreads()
        {
            //_keyboardInput.StartMonitorThread(2);
            //Add mouse thread
        }

        public void PushCameraRef(ref Camera camera)
        {
            _camera = camera;
        }

        public void CameraMoveFront(float time) { _camera.AddLocation(ref V_W, time); }
        public void CameraMoveBack(float time) { _camera.AddLocation(ref V_S, time); }
        public void CameraMoveLeft(float time) { _camera.AddLocation(ref V_A, time); }
        public void CameraMoveRight(float time) { _camera.AddLocation(ref V_D, time); }
        public void CameraMoveUp(float time) { _camera.AddLocation(ref V_Up, time); }
        public void CameraMoveDown(float time) { _camera.AddLocation(ref V_Dn, time); }
        public void CameraRotation(MouseMove move) { _camera.AddRotation(move.dX * 0.001f, move.dY * 0.001f); }

        private void KeyboardToExitWindow(float time)
        {
            _shutDownProgram = true;
            InputManager.GetInstance().IsKeyboardThreadRunning = false;
            InputManager.GetInstance().IsMouseThreadRunning = false;
        }

        public void Update(float time)
        {

        }
    }
}
