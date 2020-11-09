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

        private Action<float, MouseArgs> w, a, s, d, sp, ls, esc;
        private Action<float, MouseArgs> mouseMove, mouseAction;

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

            var im = InputManager.GetInstance();
            im.Configurations.TryGetValue("primary move", out InputConfiguration config);

            {
                var ll = new LinkedList<InputAssignment>();
                ll.AddLast(new InputAssignment(InputType.Keyboard, Key.W));
                var dl = new LinkedList<Action<float, MouseArgs>>();
                dl.AddLast(w);
                config.InputActions.AddLast(new InputAction(Qualifiers.IGNORE_AFTER | Qualifiers.IGNORE_BEFORE, ActionInvoke.Down, ll, dl));
            }
            {
                var ll = new LinkedList<InputAssignment>();
                ll.AddLast(new InputAssignment(InputType.Keyboard, Key.S));
                var dl = new LinkedList<Action<float, MouseArgs>>();
                dl.AddLast(s);
                config.InputActions.AddLast(new InputAction(Qualifiers.IGNORE_AFTER | Qualifiers.IGNORE_BEFORE, ActionInvoke.Down, ll, dl));
            }
            {
                var ll = new LinkedList<InputAssignment>();
                ll.AddLast(new InputAssignment(InputType.Keyboard, Key.A));
                var dl = new LinkedList<Action<float, MouseArgs>>();
                dl.AddLast(a);
                config.InputActions.AddLast(new InputAction(Qualifiers.IGNORE_AFTER | Qualifiers.IGNORE_BEFORE, ActionInvoke.Down, ll, dl));
            }
            {
                var ll = new LinkedList<InputAssignment>();
                ll.AddLast(new InputAssignment(InputType.Keyboard, Key.D));
                var dl = new LinkedList<Action<float, MouseArgs>>();
                dl.AddLast(d);
                config.InputActions.AddLast(new InputAction(Qualifiers.IGNORE_AFTER | Qualifiers.IGNORE_BEFORE, ActionInvoke.Down, ll, dl));
            }
            {
                var ll = new LinkedList<InputAssignment>();
                ll.AddLast(new InputAssignment(InputType.Keyboard, Key.Space));
                var dl = new LinkedList<Action<float, MouseArgs>>();
                dl.AddLast(sp);
                config.InputActions.AddLast(new InputAction(Qualifiers.IGNORE_AFTER | Qualifiers.IGNORE_BEFORE, ActionInvoke.Down, ll, dl));
            }
            {
                var ll = new LinkedList<InputAssignment>();
                ll.AddLast(new InputAssignment(InputType.Keyboard, Key.LShift));
                var dl = new LinkedList<Action<float, MouseArgs>>();
                dl.AddLast(ls);
                config.InputActions.AddLast(new InputAction(Qualifiers.IGNORE_AFTER | Qualifiers.IGNORE_BEFORE, ActionInvoke.Down, ll, dl));
            }
            {
                var ll = new LinkedList<InputAssignment>();
                ll.AddLast(new InputAssignment(InputType.Keyboard, Key.Escape));
                var dl = new LinkedList<Action<float, MouseArgs>>();
                dl.AddLast(esc);
                config.InputActions.AddLast(new InputAction(Qualifiers.IGNORE_AFTER | Qualifiers.IGNORE_BEFORE, ActionInvoke.Down, ll, dl));
            }
            {
                var ll = new LinkedList<InputAssignment>();
                ll.AddLast(new InputAssignment(InputType.MouseMove));
                var dl = new LinkedList<Action<float, MouseArgs>>();
                dl.AddLast(mouseMove);
                config.InputActions.AddLast(new InputAction(Qualifiers.IGNORE_AFTER | Qualifiers.IGNORE_BEFORE, ActionInvoke.Down, ll, dl));
            }

            im.Configurations.TryGetValue("unlocked mouse", out InputConfiguration config1);
            {
                var ll = new LinkedList<InputAssignment>();
                ll.AddLast(new InputAssignment(InputType.MouseMove));
                var dl = new LinkedList<Action<float, MouseArgs>>();
                dl.AddLast(mouseAction);
                config1.InputActions.AddLast(new InputAction(Qualifiers.IGNORE_AFTER | Qualifiers.IGNORE_BEFORE, ActionInvoke.Down, ll, dl));
            }
            {
                var ll = new LinkedList<InputAssignment>();
                ll.AddLast(new InputAssignment(InputType.Keyboard, Key.Escape));
                var dl = new LinkedList<Action<float, MouseArgs>>();
                dl.AddLast(esc);
                config.InputActions.AddLast(new InputAction(Qualifiers.IGNORE_AFTER | Qualifiers.IGNORE_BEFORE, ActionInvoke.Down, ll, dl));
            }
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

        private void CameraMoveFront(float time, MouseArgs mArgs) { _camera.AddLocation(ref V_W, time); }
        private void CameraMoveBack(float time, MouseArgs mArgs) { _camera.AddLocation(ref V_S, time); }
        private void CameraMoveLeft(float time, MouseArgs mArgs) { _camera.AddLocation(ref V_A, time); }
        private void CameraMoveRight(float time, MouseArgs mArgs) { _camera.AddLocation(ref V_D, time); }
        private void CameraMoveUp(float time, MouseArgs mArgs) { _camera.AddLocation(ref V_Up, time); }
        private void CameraMoveDown(float time, MouseArgs mArgs) { _camera.AddLocation(ref V_Dn, time); }
        private void CameraRotation(float time, MouseArgs mArgs) { _camera.AddRotation(-mArgs.dY * 0.001f, -mArgs.dX * 0.001f); }

        private void MouseAction(MouseMove move) { }

        private void KeyboardToExitWindow(float time, MouseArgs mArgs)
        {
            _shutDownProgram = true;
        }

        public void Update(float time)
        {

        }
    }
}
