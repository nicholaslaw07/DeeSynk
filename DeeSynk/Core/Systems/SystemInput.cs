using DeeSynk.Core.Components;
using DeeSynk.Core.Components.Input;
using OpenTK.Input;
using System;
using System.Collections.Generic;
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

        private World _world;
        private UI _ui;

        private MouseInputQueue _mouseInput;
        public MouseInputQueue MouseInput { get => _mouseInput; }

        private Camera _camera;


        public SystemInput(ref World world, ref UI ui, ref MouseInputQueue mouseInput)
        {
            _world = world;
            _ui = ui;
            _mouseInput = mouseInput;
        }

        public void PushCameraRef(ref Camera camera)
        {
            _camera = camera;
        }

        private void MouseMoveToCameraLook()
        {
            var ml = _mouseInput.GetNetQueuedInputs(true);
            _camera.AddRotation(ml.X * 0.01f, ml.Y * 0.01f);
            Console.WriteLine(_mouseInput.Locations.Count);
        }

        public void Update(float time)
        {
            MouseMoveToCameraLook();
            _mouseInput.Clear();
        }
    }
}
