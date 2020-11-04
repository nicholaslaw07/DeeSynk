using DeeSynk.Core.Components;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeeSynk.Core.Systems
{
    public class SystemUI : ISystem
    {
        public Component MonitoredComponents => Component.UI_ELEMENT | Component.TRANSFORM;

        private World _world;
        private UI _ui;

        private bool[] _monitoredGameObjects;

        public SystemUI(ref World world, ref UI ui)
        {
            _world = world;
            _ui = ui;
            _monitoredGameObjects = new bool[_ui.ObjectMemory];
        }

        public void UpdateMonitoredGameObjects()
        {
            for (int i = 0; i < _ui.ObjectMemory; i++)
            {
                if (_ui.ExistingGameObjects[i])
                {
                    if (_ui.GameObjects[i].Components.HasFlag(MonitoredComponents))
                    {
                        _monitoredGameObjects[i] = true;
                    }
                }
            }
        }

        private void UpdateMonitoredGameObjects(GameObjectContainer c, bool[] monitor)
        {

        }

        /// <summary>
        /// Moves the selected element by the specified distance in pixels.
        /// </summary>
        /// <param name="idx">Index of the selected UIElement.</param>
        /// <param name="position">The change in position measured in pixels.</param>
        public void MoveElementBy(int idx, Vector2 dP)
        {
            if (_ui.GameObjects[idx].Components.HasFlag(Component.UI_STANDARD))
            {
                _ui.ElementComps[idx].Element.Position += dP;
                Vector3 dP3 = new Vector3(dP.X, dP.Y, 0.0f);
                MoveWithChildrenBy(idx, dP3);
            }
        }

        private void MoveWithChildrenBy(int idx, Vector3 dP3)
        {
            _ui.TransComps[idx].LocationComp.Location += dP3;
            for (int jdx = 0; jdx < _ui.ElementComps[idx].Element.ChildElementCount; jdx++)
            {
                if (_ui.ElementComps[idx].Element.ExistingElement[jdx])
                    MoveWithChildrenBy(_ui.ElementComps[idx].Element.ChildElementIDs[jdx], dP3);
            }
        }

        public void Update(float time)
        {

        }
    }
}
