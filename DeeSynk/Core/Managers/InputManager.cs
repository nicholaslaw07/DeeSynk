using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Input;

namespace DeeSynk.Core.Managers
{
    public class InputManager : IManager
    {
        private static InputManager _inputManager;

        private Dictionary<Key, List<Action>> _callbacks;
        private Dictionary<Key, long> _keyDownTimestamps;
        private Dictionary<Key, long> _keyUpTimestamps;
        private Dictionary<Key, long> _keyPressTimestamps;
        private List<Key> _currentFrameKeys;

        private InputManager()
        { 
            _callbacks = new Dictionary<Key, List<Action>>();
            _keyDownTimestamps = new Dictionary<Key, long>();
            _keyUpTimestamps = new Dictionary<Key, long>();
            _keyPressTimestamps = new Dictionary<Key, long>();
            _currentFrameKeys = new List<Key>();
        }

        public static ref InputManager GetInstance()
        {
            if (_inputManager == null)
            {
                _inputManager = new InputManager();
            }
            return ref _inputManager;
        }

        public void RegisterCallback(Key key, Action callback)
        {
            if (!_callbacks.ContainsKey(key))
                _callbacks.Add(key, new List<Action> { callback ,});
            else
                _callbacks[key].Add(callback);

            Console.WriteLine("CALLBACK REGISTERED: " + key.ToString() + ", "+ callback.ToString());
        }

        public void Update()
        {
            foreach (Key key in _currentFrameKeys)
                InvokeCallbacks(key);

            _currentFrameKeys.Clear();
        }

        public void InvokeCallbacks(Key key)
        {
            if (_callbacks.ContainsKey(key))
            {
                foreach (Action callback in _callbacks[key])
                {
                    callback();
                }
            }
        }

        public void KeyDown(Key key, long time)
        {
            if (!_keyDownTimestamps.ContainsKey(key))
                _keyDownTimestamps.Add(key, time);
            else
                _keyDownTimestamps[key] = time;
        }
        public void KeyUp(Key key, long time)
        {
            if (!_keyUpTimestamps.ContainsKey(key))
                _keyUpTimestamps.Add(key, time);
            else
                _keyUpTimestamps[key] = time;

            _currentFrameKeys.Add(key);
        }
        public void KeyPress(Key key, long time)
        {
            if (!_keyPressTimestamps.ContainsKey(key))
                _keyPressTimestamps.Add(key, time);
            else
                _keyPressTimestamps[key] = time;
        }

        public void Load()
        {
        }

        public void UnLoad()
        {
        }
    }
}
