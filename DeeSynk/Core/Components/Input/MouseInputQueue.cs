using OpenTK.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeeSynk.Core.Components.Input
{
    public struct MouseLocation
    {
        int _x;
        public int X { get => _x; }
        int _y;
        public int Y { get => _y; }
        long _dt;
        public long DT { get => _dt; }

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

        public static MouseLocation operator +(MouseLocation ml1, MouseLocation ml2)
        {
            return new MouseLocation(ml1._x + ml2._x, ml1._y + ml2._y, ml1._dt + ml2._dt);
        }
    }

    public class MouseInputQueue
    {
        private Queue<MouseLocation> _locations;
        /// <summary>
        /// The queue of all previously entered mouse locations.
        /// </summary>
        public Queue<MouseLocation> Locations { get => _locations; }

        private Queue<MouseLocation> _subLocations;
        /// <summary>
        /// The queue of all previously entered mouse locations from when the last freeze was called.
        /// </summary>
        public Queue<MouseLocation> SubLocations { get => _subLocations; }

        private int _freezeCount;
        /// <summary>
        /// Represents the number of inputs in locations at the last read.
        /// </summary>
        public int FreezeCount { get => _freezeCount; }
        public bool Frozen { get => _freezeCount == -1 || _subLocations.Count == 0; }

        public MouseInputQueue()
        {
            _locations = new Queue<MouseLocation>();
            _subLocations = new Queue<MouseLocation>();
            _freezeCount = -1;

            _locations.Enqueue(new MouseLocation(0, 0, 0));
        }

        public void Freeze()
        {
            int count = _locations.Count;
            if(count > 0)
            {
                _freezeCount = _locations.Count;
                _subLocations = (Queue<MouseLocation>)_locations.Take(_freezeCount);
            }
        }

        public void Unfreeze()
        {
            RemoveFrozenValues();
            _freezeCount = -1;
            _subLocations.Clear();
        }

        public MouseLocation GetNetQueuedInputs(bool deleteAfter)
        {
            int count = (Frozen) ? _freezeCount : _locations.Count;
            MouseLocation ml = new MouseLocation(0, 0, 0);

            if (deleteAfter)
            {
                for (int idx = 0; idx < count; idx++)
                    ml += _locations.Dequeue();
            }
            else
            {
                Queue<MouseLocation> subLocations = (Queue<MouseLocation>)_locations.Take(count);
                foreach (MouseLocation m in subLocations)
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
                int max = (_freezeCount < _locations.Count) ? _freezeCount : _locations.Count - 1;
                for (int idx = 0; idx < max; idx++)
                    _locations.Dequeue();
            }
        }

        /// <summary>
        /// Removes all values but the last from the queue.
        /// </summary>
        public void Clear()
        {
            int count = _locations.Count;
            if (count > 0)
            {
                for (int idx = 0; idx < count - 1; idx++)
                    _locations.Dequeue();
                if (Frozen) Unfreeze();
            }
        }
    }
}
