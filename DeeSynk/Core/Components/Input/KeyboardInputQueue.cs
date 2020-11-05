using Microsoft.EntityFrameworkCore.Internal;
using OpenTK;
using OpenTK.Input;
using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DeeSynk.Core.Components.Input
{
    public enum KeyEventType
    {
        KEY_UP = 0,
        KEY_DOWN = 1
    }
    public struct KeyEvent
    {
        Key _key;
        public Key Key { get => _key; }

        KeyEventType _eventType;
        public KeyEventType KeyEventType { get => _eventType; }

        long _time;
        public long Time { get => _time; }

        public KeyEvent(Key key, KeyEventType eventType, long time)
        {
            _key = key;
            _eventType = eventType;
            _time = time;
        }
    }
    public class KeyboardInputQueue
    {
        private readonly object _lockObject = new object();

        public static readonly int MAX_KEY_VALUE = 131;

        private bool _isRunning;
        /// <summary>
        /// Allows the thread to begin to run and is used to stop the thread.
        /// </summary>
        public bool IsRunning { get => _isRunning; set => _isRunning = value; }

        private int _updatePeriod;
        public int UpdatePeriod { get => _updatePeriod; }

        private Stopwatch _sw;

        private Thread _listener;
        public Thread Listener { get => _listener; }

        private BlockingCollection<KeyEvent> _eventList;
        public BlockingCollection<KeyEvent> EventList { get => _eventList; }

        private BlockingCollection<Key> _downKeys;
        public BlockingCollection<Key> DownKeys { get => _downKeys; }

        private BlockingCollection<Key> _keysInEventList;
        public BlockingCollection<Key> KeysInEventList { get => _keysInEventList; }

        private List<Key> _monitoredKeys;
        public List<Key> MonitoredKeys { get => _monitoredKeys; set => _monitoredKeys = value; }

        private bool _usingDirectKeyboardMove;
        public bool UsingDirectKeyboardMove { get => _usingDirectKeyboardMove; }

        private bool _pause;
        public bool Pause { get => _pause; set => _pause = value; }

        private Camera _camera;
        public Camera Camera { get => _camera; }

        public KeyboardInputQueue(ref List<Key> monitoredKeys)
        {
            _eventList = new BlockingCollection<KeyEvent>();
            _downKeys = new BlockingCollection<Key>();
            _keysInEventList = new BlockingCollection<Key>();
            _monitoredKeys = monitoredKeys;
        }

        public void SetDirectKeyboard(ref Camera camera)
        {
            _camera = camera;
            _usingDirectKeyboardMove = camera != null;
        }

        public void UnsetDirectMouse()
        {
            _usingDirectKeyboardMove = false;
        }

        public void StartMonitorThread(int updatePeriod)
        {
            _listener = new Thread(new ThreadStart(Listen));
            _updatePeriod = updatePeriod;
            _isRunning = true;
            _listener.Start();
        }

        private void Listen() //stopwatch should never need to be restarted in theory
        {
            _sw = new Stopwatch();
            _sw.Start();
            do
            {
                if (!_pause)
                {
                    var kbs = Keyboard.GetState();
                    foreach (Key k in _monitoredKeys)
                    {
                        if (kbs.IsKeyDown(k))
                        {
                            _eventList.Add(new KeyEvent(k, KeyEventType.KEY_DOWN, _sw.ElapsedTicks));
                            if (!_keysInEventList.Contains(k))
                                _keysInEventList.Add(k);
                            _downKeys.Add(k);
                        }
                    }
                    if (_downKeys.Count > 0)
                    {
                        var remove = new ConcurrentBag<Key>();

                        foreach (Key k in _downKeys)
                        {
                            if (kbs.IsKeyUp(k))
                            {
                                _eventList.Add(new KeyEvent(k, KeyEventType.KEY_UP, _sw.ElapsedTicks));
                                if (!_keysInEventList.Contains(k))
                                    _keysInEventList.Add(k);
                                remove.Add(k);
                            }
                        }

                        if (remove.Count > 0)
                        {
                            foreach (Key k in remove)
                            {
                                RemoveDownKey(k);
                            }
                        }
                    }
                }
                Thread.Sleep(_updatePeriod); //Technically the time it takes to run the previous code should be subtracted from the update time.
            } while (_isRunning);
        }

        public BlockingCollection<KeyEvent> GetInstance(Key k)
        {
            BlockingCollection<KeyEvent> instance = new BlockingCollection<KeyEvent>();
            if (!_downKeys.Contains(k) && _eventList.Select(ke => ke.Key).Contains(k))
            {
                var narrow = _eventList.Where(ke => ke.Key == k);
                var firstDown = narrow.Where(ke => ke.KeyEventType == KeyEventType.KEY_DOWN).First();
                var firstUp = narrow.Where(ke => ke.KeyEventType == KeyEventType.KEY_UP).First();
                if (_eventList.IndexOf(firstDown) < _eventList.IndexOf(firstUp))
                {
                    instance.Add(firstDown);
                    instance.Add(firstUp);
                }
            }
            return instance;
        }

        public BlockingCollection<KeyEvent> GetInstances(Key k, int count)
        {
            BlockingCollection<KeyEvent> instances = new BlockingCollection<KeyEvent>();

            for (int idx = 0; idx < count; idx++)
            {
                var instance = GetInstance(k);
                if (instance.Count == 0)
                    return instances;
                else
                    instances.Concat(instance);
            }
            return instances;
        }

        public int GetAllInstances(Key k)
        {
            int idx = 0;
            var instances = new BlockingCollection<KeyEvent>();
            var instance = GetInstance(k);
            while(instance.Count == 2)
            {
                idx++;
                instance = GetInstance(k);
            }
            return idx;
        }

        /// <summary>
        /// Removes a full keypress from the event list.  Down keys are ignored.
        /// </summary>
        /// <param name="k">The key that will be attempted to be removed</param>
        public bool RemoveInstance(Key k)
        {
            if (!_downKeys.Contains(k) && _keysInEventList.Contains(k))
            {
                var narrow = _eventList.Where(ke => ke.Key == k);
                var firstDown = narrow.Where(ke => ke.KeyEventType == KeyEventType.KEY_DOWN).First();
                var firstUp = narrow.Where(ke => ke.KeyEventType == KeyEventType.KEY_UP).First();
                if (_eventList.IndexOf(firstDown) < _eventList.IndexOf(firstUp))
                {
                    RemoveKeyEvent(firstDown);
                    RemoveKeyEvent(firstUp);
                    UpdateKeysInEventsList();
                    return true;
                }
            }
            return false;
        }

        public int RemoveInstances(Key k, int count)
        {
            for(int idx = 0; idx < count; idx++)
            {
                if (!RemoveInstance(k))
                    return idx;
            }
            return count;
        }

        public int RemoveAllInstances(Key k)
        {
            int idx = 0;
            while(RemoveInstance(k))
            {
                idx++;
            }
            return idx;
        }

        public float GetTimeForInstance(Key k)
        {
            float time = 0.0f;
            var instance = GetInstance(k);
            if (instance.Count == 2)
            {
                time = (instance.First().Time + instance.Last().Time) / 10000.0f;
            }
            else if (instance.Count == 0 && _downKeys.Contains(k))
            {
                long t0 = _eventList.Where(ke => ke.Key == k).Where(ke => ke.KeyEventType == KeyEventType.KEY_DOWN).First().Time;
                long t1 = _sw.ElapsedTicks;
                time += (t1 - t0) / 10000.0f;
            }
            return time;
        }

        public float GetTimeForInstances(Key k, int count)
        {
            float time = 0.0f;
            var instance = GetInstance(k);
            for (int idx = 0; idx < count; idx++)
            {
                if (instance.Count == 2)
                {
                    time += (instance.First().Time + instance.Last().Time) / 10000.0f;
                    instance = GetInstance(k);
                }
                else if (instance.Count == 0 && _downKeys.Contains(k))
                {
                    long t0 = _eventList.Where(ke => ke.Key == k).Where(ke => ke.KeyEventType == KeyEventType.KEY_DOWN).First().Time;
                    long t1 = _sw.ElapsedTicks;
                    time += (t1 - t0) / 10000.0f;
                    break;
                }
            }
            return time;
        }

        public float GetTimeForAllInstances(Key k)
        {
            float time = 0.0f;
            var instance = GetInstance(k);
            while(instance.Count == 2)
            {
                time += (instance.First().Time + instance.Last().Time) / 10000.0f;
                instance = GetInstance(k);
            }
            if (_downKeys.Contains(k))
            {
                long t0 = _eventList.Where(ke => ke.Key == k).Where(ke => ke.KeyEventType == KeyEventType.KEY_DOWN).First().Time;
                long t1 = _sw.ElapsedTicks;
                time += (t1 - t0) / 10000.0f;
            }
            return time;
        }

        public void RemoveDownKey(Key ke)
        {
            BlockingCollection<Key> bag = new BlockingCollection<Key>();
            foreach (Key k in _downKeys)
            {
                if (k == ke)
                    continue;
                else
                    bag.Add(k);
            }

            _downKeys = bag;
        }

        public void RemoveKeyEvent(KeyEvent ke)
        {
            BlockingCollection<KeyEvent> bag = new BlockingCollection<KeyEvent>();
            foreach(KeyEvent k in _eventList)
            {
                if (k.Key == ke.Key && k.KeyEventType == ke.KeyEventType && k.Time == ke.Time)
                    continue;
                else
                    bag.Add(k);
            }

            _eventList = bag;
        }

        public void UpdateKeysInEventsList()
        {
            BlockingCollection<Key> val = new BlockingCollection<Key>();
            var keysEL = _eventList.Select(ke => ke.Key);

            foreach (Key k in _keysInEventList)
            {
                if (keysEL.Contains(k))
                    val.Add(k);
            }

            _keysInEventList = val;
        }
    }
}
