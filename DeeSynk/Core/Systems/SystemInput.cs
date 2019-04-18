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
    public class SystemInput
    {
        private List<InputEvent> _frameInputEvents;
        public List<InputEvent> FrameInputEvents { get => _frameInputEvents; }

        private List<InputPress> _framePressEvents;
        public List<InputPress> FramePressEvents { get => _framePressEvents; }

        public SystemInput()
        {
            _frameInputEvents = new List<InputEvent>();
            _framePressEvents = new List<InputPress>();
        }

        public void AddEvent(KeyboardKeyEventArgs args, EventType type, long time)
        {
            _frameInputEvents.Add(new InputEvent(args, type, time));
        }

        public void AddEvent(InputEvent e)
        {
            _frameInputEvents.Add(e);
        }

        public void AddPress(char key, long time)
        {
            _framePressEvents.Add(new InputPress(key, time));
        }

        public void AddPress(InputPress e)
        {
            _framePressEvents.Add(e);
        }
    }
}
