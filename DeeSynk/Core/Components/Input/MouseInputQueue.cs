using OpenTK.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeeSynk.Core.Components.Input
{
    public struct MouseDelta
    {
        int _dx;
        public int dX { get => _dx; }
        int _dy;
        public int dY { get => _dy; }
        long _dt;
        public long dT { get => _dt; }

        public MouseDelta(int dx, int dy, long dt)
        {
            _dx = dx;
            _dy = dy;
            _dt = dt;
        }

        public static MouseDelta operator +(MouseDelta ml1, MouseDelta ml2)
        {
            return new MouseDelta(ml1._dx + ml2._dx, ml1._dy + ml2._dy, ml1._dt + ml2._dt);
        }
    }
    public struct MouseLocation
    {
        int _x;
        public int X { get => _x; }
        int _y;
        public int Y { get => _y; }
        long _dt;
        public long dT { get => _dt; }

        /// <summary>
        /// Creates a new struct that represents a snap of the position and the time it took to get there.
        /// </summary>
        /// <param name="x">Location of the mouse in the X direction.</param>
        /// <param name="y">Location of the mouse in the Y direction.</param>
        /// <param name="dt">The amount of time it took to arrive at the current location in ticks (100ns).</param>
        public MouseLocation(int x, int y, long dt)
        {
            _x = x;
            _y = y;
            _dt = dt;
        }

        /// <summary>
        /// Creates a new struct that represents a snap of the position and the time it took to get there.
        /// </summary>
        /// <param name="e">An event argument container with the current mouse state.</param>
        /// <param name="dt">The amount of time it took to arrive at the current location in ticks (100ns).</param>
        public MouseLocation(MouseState ms, long dt)
        {
            _x = ms.X;
            _y = ms.Y;
            _dt = dt;
        }

        public static MouseDelta operator -(MouseLocation ml1, MouseLocation ml2)
        {
            return new MouseDelta(ml1._x - ml2._x, ml1._y - ml2._y, ml1._dt);
        }
    }

    public class MouseInputQueue
    {
        private Queue<MouseDelta> _deltas;
        /// <summary>
        /// The queue of all previously entered mouse locations.
        /// </summary>
        public Queue<MouseDelta> Deltas { get => _deltas; }

        private Queue<MouseDelta> _subDeltas;
        /// <summary>
        /// The queue of all previously entered mouse locations from when the last freeze was called.
        /// </summary>
        public Queue<MouseDelta> SubDeltas { get => _subDeltas; }

        private MouseLocation _tempLocation;

        private int _freezeCount;
        /// <summary>
        /// Represents the number of inputs in locations at the last read.
        /// </summary>
        public int FreezeCount { get => _freezeCount; }
        public bool Frozen { get => _freezeCount >= 0 && _subDeltas.Count >= 0; }

        private bool _usingDirectMouseMove;
        public bool UsingDirectMouseMove { get => _usingDirectMouseMove; }

        private Camera _camera;
        public Camera Camera { get => _camera; }



        public MouseInputQueue()
        {
            _deltas = new Queue<MouseDelta>();
            _subDeltas = new Queue<MouseDelta>();
            _freezeCount = -1;

            var ms = Mouse.GetState();
            AddLocation(new MouseLocation(ms.Y, ms.X, 0));
        }

        public void SetDirectMouse(ref Camera camera)
        {
            _camera = camera;
            _usingDirectMouseMove = camera != null;
        }

        public void UnsetDirectMouse()
        {
            _usingDirectMouseMove = false;
        }

        public void AddLocation(MouseLocation ms)
        {
            var a = _tempLocation - ms;
            _tempLocation = ms;
            _deltas.Enqueue(a);

            if (_usingDirectMouseMove)
                _camera.AddRotation(a.dX * 0.001f, a.dY * 0.001f);
        }

        public void Freeze()
        {
            int count = _deltas.Count;
            if(count > 0)
            {
                _freezeCount = _deltas.Count;
                _subDeltas = (Queue<MouseDelta>)_deltas.Take(_freezeCount);
            }
        }

        public void Unfreeze()
        {
            RemoveFrozenValues();
            _freezeCount = -1;
            _subDeltas.Clear();
        }

        public MouseDelta GetNetQueuedInputs(bool deleteAfter)
        {
            int count = (Frozen) ? _freezeCount : _deltas.Count;
            MouseDelta ml = new MouseDelta(0, 0, 0);

            if (deleteAfter)
            {
                for (int idx = 0; idx < count; idx++)
                    ml += _deltas.Dequeue();
            }
            else
            {
                Queue<MouseDelta> subLocations = (Queue<MouseDelta>)_deltas.Take(count);
                foreach (MouseDelta m in subLocations)
                    ml += subLocations.Dequeue();
            }

            return ml;
        }

        /// <summary>
        /// Removes all values which are currently frozen from the main queue.  If all values in the main queue are frozen then all but the last are removed so that a single mouse input can be used a reference in the next use.
        /// </summary>
        private void RemoveFrozenValues()
        {
            if (Frozen)
            {
                int max = (_freezeCount < _deltas.Count) ? _freezeCount : _deltas.Count - 1;
                for (int idx = 0; idx < max; idx++)
                    _deltas.Dequeue();
            }
        }

        /// <summary>
        /// Removes all values but the last from the queue.
        /// </summary>
        public void Clear()
        {
            _deltas.Clear();
            _subDeltas.Clear();
        }
    }
}
