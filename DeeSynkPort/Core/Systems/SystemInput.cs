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
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;

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
            AddCameraMove(config, Keys.W, w);
            AddCameraMove(config, Keys.A, a);
            AddCameraMove(config, Keys.S, s);
            AddCameraMove(config, Keys.D, d);
            AddCameraMove(config, Keys.Space, sp);
            AddCameraMove(config, Keys.LeftShift, ls);
            AddEscape(config, Keys.Escape, esc);
            AddMouseAction(config, mouseMove);
            config.RawMouse = true;

            im.Configurations.TryGetValue("unlocked mouse", out InputConfiguration config1);
            config1.RawMouse = false;
            AddEscape(config1, Keys.Escape, esc);
        }

        public void PushCameraRef(ref Camera camera)
        {
            _camera = camera;
        }

        public void CameraMoveFront(float time, MouseArgs args) { _camera.AddLocation(ref V_W, time); }
        public void CameraMoveBack(float time, MouseArgs args) { _camera.AddLocation(ref V_S, time); }
        public void CameraMoveLeft(float time, MouseArgs args) { _camera.AddLocation(ref V_A, time); }
        public void CameraMoveRight(float time, MouseArgs args) { _camera.AddLocation(ref V_D, time); }
        public void CameraMoveUp(float time, MouseArgs args) { _camera.AddLocation(ref V_Up, time); }
        public void CameraMoveDown(float time, MouseArgs args) { _camera.AddLocation(ref V_Dn, time); }
        public void CameraRotation(float time, MouseArgs args) { _camera.AddRotation(-args.dY * 0.001f, -args.dX * 0.001f); }

        private void MouseAction(float time, MouseArgs args) { }

        private void KeyboardToExitWindow(float time, MouseArgs args)
        {
            _shutDownProgram = true;
        }

        public void Update(float time)
        {

        }

        //Currently these functions have no reason to be in SystemInput.  It's also the case that SystemInput has no defined purpose right now.
        //In theory, SystemInput should act as the mediator between controllable objects or those that accept input and those which are affected by the objects that accept input.
        //It is very possible this class will not exist in the future since all control interactions are now invoked by InputManager so using the Update function in this class is useless.

        private void AddCameraMove(InputConfiguration config, Keys k, Action<float, MouseArgs> function)
        {
            var combo = new List<InputAssignment>();
            combo.Add(new InputAssignment(InputType.Keyboard, k));
            var functions = new List<Action<float, MouseArgs>>();
            functions.Add(function);
            var inputAction = new Components.Input.InputAction(Qualifiers.IN_ORDER_IGNORE_ALL, combo);
            inputAction.DownActions = functions;
            inputAction.HoldActions = functions;
            config.InputActions.AddLast(inputAction);
        }
        private void AddCameraMove(InputConfiguration config, Keys k, List<Action<float, MouseArgs>> functions)
        {
            var combo = new List<InputAssignment>();
            combo.Add(new InputAssignment(InputType.Keyboard, k));
            var inputAction = new Components.Input.InputAction(Qualifiers.IN_ORDER_IGNORE_ALL, combo);
            inputAction.DownActions = functions;
            inputAction.HoldActions = functions;
            config.InputActions.AddLast(inputAction);
        }
        private void AddEscape(InputConfiguration config, Keys k, Action<float, MouseArgs> function)
        {
            var combo = new List<InputAssignment>();
            combo.Add(new InputAssignment(InputType.Keyboard, k));
            var functions = new List<Action<float, MouseArgs>>();
            functions.Add(function);
            var inputAction = new Components.Input.InputAction(Qualifiers.IN_ORDER_IGNORE_ALL, combo);
            inputAction.UpActions = functions;
            config.InputActions.AddLast(inputAction);
        }
        private void AddEscape(InputConfiguration config, Keys k, List<Action<float, MouseArgs>> functions)
        {
            var combo = new List<InputAssignment>();
            combo.Add(new InputAssignment(InputType.Keyboard, k));
            var inputAction = new Components.Input.InputAction(Qualifiers.IN_ORDER_IGNORE_ALL, combo);
            inputAction.UpActions = functions;
            config.InputActions.AddLast(inputAction);
        }
        private void AddMouseAction(InputConfiguration config, Action<float, MouseArgs> function)
        {
            var combo = new List<InputAssignment>();
            combo.Add(new InputAssignment(InputType.MouseMove));
            var functions = new List<Action<float, MouseArgs>>();
            functions.Add(function);
            var inputAction = new Components.Input.InputAction(Qualifiers.IN_ORDER_IGNORE_ALL, combo);
            inputAction.DownActions = functions;
            config.InputActions.AddLast(inputAction);
        }
        private void AddMouseAction(InputConfiguration config, List<Action<float, MouseArgs>> functions)
        {
            var combo = new List<InputAssignment>();
            combo.Add(new InputAssignment(InputType.MouseMove));
            var inputAction = new Components.Input.InputAction(Qualifiers.IN_ORDER_IGNORE_ALL, combo);
            inputAction.DownActions = functions;
            config.InputActions.AddLast(inputAction);
        }
    }
}
