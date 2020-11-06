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

        private List<KeyEvent> _eventList;
        public List<KeyEvent> EventList { get => _eventList; }

        private List<Key> _downKeys;
        public List<Key> DownKeys { get => _downKeys; }

        private List<Key> _keysInEventList;
        public List<Key> KeysInEventList { get => _keysInEventList; }

        private List<Key> _monitoredKeys;
        public List<Key> MonitoredKeys { get => _monitoredKeys; set => _monitoredKeys = value; }

        private bool _usingDirectKeyboardMove;
        public bool UsingDirectKeyboardMove { get => _usingDirectKeyboardMove; }

        private bool _pause;
        public bool Pause { get => _pause; set => _pause = value; }

        private Camera _camera;
        public Camera Camera { get => _camera; }

        public KeyboardInputQueue()
        {
            _eventList = new List<KeyEvent>();
            _downKeys = new List<Key>();
            _keysInEventList = new List<Key>();
            _monitoredKeys = new List<Key>();
        }

        public KeyboardInputQueue(ref List<Key> monitoredKeys)
        {
            _eventList = new List<KeyEvent>();
            _downKeys = new List<Key>();
            _keysInEventList = new List<Key>();
            _monitoredKeys = monitoredKeys;
        }

        public void SetDirectKeyboard(ref Camera camera)
        {
            _camera = camera;
            _usingDirectKeyboardMove = camera != null;
        }

        public void UnsetDirectKeyboard()
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
                            if (!_downKeys.Contains(k))
                            {
                                lock (_eventList)
                                {
                                    _eventList.Add(new KeyEvent(k, KeyEventType.KEY_DOWN, _sw.ElapsedTicks));
                                    if (!_keysInEventList.Contains(k))
                                        _keysInEventList.Add(k);
                                    _downKeys.Add(k);
                                }
                            }
                        }
                    }

                    if (_downKeys.Count > 0)
                    {
                        var remove = new List<Key>();
                        var downKeys = _downKeys.ToList();
                        foreach (Key k in downKeys)
                        {
                            if (kbs.IsKeyUp(k))
                            {
                                lock (_eventList)
                                {
                                    _eventList.Add(new KeyEvent(k, KeyEventType.KEY_UP, _sw.ElapsedTicks));
                                }
                                lock (_keysInEventList)
                                {
                                    if (!_keysInEventList.Contains(k))
                                        _keysInEventList.Add(k);
                                }
                                remove.Add(k);
                            }
                        }

                        if (remove.Count > 0)
                        {
                            lock (_downKeys)
                            {
                                foreach (Key k in remove)
                                    _downKeys.Remove(k);
                            }
                        }
                    }
                }
                Thread.Sleep(_updatePeriod); //Technically the time it takes to run the previous code should be subtracted from the update time.
            } while (_isRunning);
        }

        public List<KeyEvent> GetInstance(Key k)
        {
            List<KeyEvent> instance = new List<KeyEvent>();
            lock (_eventList)
            {
                if (!_downKeys.Contains(k) && _eventList.Select(ke => ke.Key).Contains(k))
                {
                    var list = _eventList.ToList();
                    var narrow = list.Where(ke => ke.Key == k);
                    var narrowDown = narrow.Where(ke => ke.KeyEventType == KeyEventType.KEY_DOWN);
                    var narrowUp = narrow.Where(ke => ke.KeyEventType == KeyEventType.KEY_UP);
                    if (narrowUp.Count() > 0)
                    {
                        if (narrowDown.Count() > 0)
                        {
                            var firstDown = narrowDown.First();
                            var firstUp = narrow.First();
                            if (list.IndexOf(firstDown) < list.IndexOf(firstUp))
                            {
                                instance.Add(firstDown);
                                instance.Add(firstUp);
                            }
                        }
                        else
                        {
                            foreach (KeyEvent ke in narrowUp)
                                _eventList.Remove(ke);
                        }
                    }
                }
            }
            return instance;
        }

        public List<KeyEvent> GetInstances(Key k, int count)
        {
            List<KeyEvent> instances = new List<KeyEvent>();

            lock (_eventList)
            {
                for (int idx = 0; idx < count; idx++)
                {
                    var instance = GetInstance(k);
                    if (instance.Count == 0)
                        return instances;
                    else
                        instances.Concat(instance);
                }
            }

            return instances;
        }

        public int GetAllInstances(Key k)
        {
            int idx = 0;
            var instances = new List<KeyEvent>();
            lock (_eventList)
            {
                var instance = GetInstance(k);
                while (instance.Count == 2)
                {
                    idx++;
                    instance = GetInstance(k);
                }
            }
            return idx;
        }

        public void RemoveDownKey(Key k)
        {
            if (_downKeys.Contains(k))
            {
                var list = _eventList.Where(ke => ke.Key == k);
                if (list.Count() == 1)
                {
                    var elem = list.Last();
                    _eventList.Remove(elem);
                    _downKeys.Remove(k);
                }
            }
        }

        /// <summary>
        /// Removes a full keypress from the event list.  Down keys are ignored.
        /// </summary>
        /// <param name="k">The key that will be attempted to be removed</param>
        public bool RemoveInstance(Key k)
        {
            lock (_eventList)
            {
                var list = _eventList.ToList();
                if (list.Select(ke => ke.Key).Contains(k))
                {
                    if (!_downKeys.Contains(k))
                    {
                        var narrow = list.Where(ke => ke.Key == k);
                        var narrowDown = narrow.Where(ke => ke.KeyEventType == KeyEventType.KEY_DOWN);
                        var narrowUp = narrow.Where(ke => ke.KeyEventType == KeyEventType.KEY_UP);

                        if(narrowUp.Count() > 0)
                        {
                            var firstUp = narrowUp.First();
                            if (narrowDown.Count() > 0)
                            {
                                var firstDown = narrowDown.First();
                                if (list.IndexOf(firstDown) < list.IndexOf(firstUp))
                                {
                                    _eventList.Remove(firstDown);
                                    _eventList.Remove(firstUp);
                                }
                            }
                            else
                            {
                                _eventList.Remove(firstUp);
                            }
                        }
                    }
                }
                else
                {
                    _keysInEventList.Remove(k);
                    return true;
                }
            }
            return false;
        }

        public int RemoveInstances(Key k, int count, bool removeDown)
        {
            lock (_eventList)
            {
                for (int idx = 0; idx < count; idx++)
                {
                    if (!RemoveInstance(k))
                        return idx;
                }
                if (removeDown)
                    RemoveDownKey(k);
            }
            return count;
        }

        public int RemoveAllInstances(Key k, bool removeDown)
        {
            int idx = 0;
            lock (_eventList)
            {
                var list = _eventList.ToList();
                idx = list.Where(ke => ke.KeyEventType == KeyEventType.KEY_UP).Count();
                RemoveInstances(k, idx, removeDown);
            }
            return idx;
        }

        public float GetTimeForInstance(Key k)
        {
            float time = 0.0f;
            lock (_eventList)
            {
                var list = _eventList.ToList();
                var instance = GetInstance(k);
                if (instance.Count == 2)
                {
                    time = (instance.Last().Time - instance.First().Time) / 10000000.0f;
                    _eventList.Remove(instance.First());
                    _eventList.Remove(instance.Last());
                }
                else if (instance.Count == 0 && _downKeys.Contains(k))
                {
                    long t0 = list.Where(ke => ke.Key == k).Where(ke => ke.KeyEventType == KeyEventType.KEY_DOWN).First().Time;
                    long t1 = _sw.ElapsedTicks;
                    time += (t1 - t0) / 10000000.0f;
                }
            }
            return time;
        }

        public float GetTimeForInstances(Key k, int count)
        {
            float time = 0.0f;
            lock (_eventList)
            {
                var list = _eventList.ToList();
                var instance = GetInstance(k);
                for (int idx = 0; idx < count; idx++)
                {
                    if (instance.Count == 2)
                    {
                        time += (instance.Last().Time - instance.First().Time) / 10000000.0f;
                        instance = GetInstance(k);
                    }

                }

                if (_downKeys.Contains(k) && _eventList.Count() > 0)
                {
                    long t0 = list.Where(ke => ke.Key == k).Where(ke => ke.KeyEventType == KeyEventType.KEY_DOWN).First().Time;
                    long t1 = _sw.ElapsedTicks;
                    time += (t1 - t0) / 10000000.0f;
                    long t2 = _sw.ElapsedTicks;
                    var ke1 = _eventList.Where(ke2 => ke2.Key == k).Last();
                    _eventList.Remove(ke1);
                    _eventList.Add(new KeyEvent(k, KeyEventType.KEY_DOWN, t2));
                }
            }
            return time;
        }

        public float GetTimeForAllInstances(Key k)
        {
            float time = 0.0f;
            lock (_eventList)
            {
                var list = _eventList.ToList();
                int number = list.Where(ke => ke.KeyEventType == KeyEventType.KEY_UP).Count();
                time = GetTimeForInstances(k, number);
            }
            return time;
        }

        public void UpdateKeysInEventsList()
        {
            List<Key> val = new List<Key>();
            var keysEL = _eventList.Select(ke => ke.Key);

            lock (_keysInEventList)
            {
                foreach (Key k in keysEL)
                {
                    if (!val.Contains(k))
                        val.Add(k);
                }

                _keysInEventList = val;
            }
        }
    }
}

//used to use BlockingCollection
