using DeeSynk.Core.Components;
using DeeSynk.Core.Components.Input;
using DeeSynk.Core.Managers;
using OpenTK;
using OpenTK.Input;
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

        private Vector2 _screenCenter;
        public Vector2 ScreenCenter { get => _screenCenter; }

        private Vector2 _relativeCenter;

        public SystemUI(ref World world, ref UI ui)
        {
            _world = world;
            _ui = ui;
            _monitoredGameObjects = new bool[_ui.ObjectMemory];

            _relativeCenter = new Vector2(MainWindow.width / 2, MainWindow.height / 2);

            InputManager.GetInstance().Configurations.TryGetValue("unlocked mouse", out InputConfiguration config);
            Action<float, MouseClick, MouseMove> action = MouseAction;
            config.MouseButtonActions.Add(MouseButton.Left, new MouseButtonAction(action, MouseButton.Left, MouseActionType.Hybrid));
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

        public void SetScreenCenter()
        {
            var state = Mouse.GetState();
            _screenCenter = new Vector2(state.X, state.Y);
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
                if (_ui.ElementComps[idx].Element.ExistingElements[jdx])
                    MoveWithChildrenBy(_ui.ElementComps[idx].Element.ChildElementIDs[jdx], dP3);
            }
        }

        public void MouseAction(float time, MouseClick mouseClick, MouseMove mouseMove)
        {
            for (int idx = 0; idx < _monitoredGameObjects.Length; idx++)
            {
                if (_ui.GameObjects[idx].Components.HasFlag(Component.UI_CANVAS))
                {
                    var ids = _ui.CanvasComps[idx].Canvas.ElementIDs;
                    if(CheckClick(time, mouseClick, mouseMove, idx, new Vector2(0.0f, 0.0f), out Vector2 clickLocation) != -1)
                    {
                        Console.WriteLine("Wow");
                    }
                }
            }
        }

        private int CheckClick(float time, MouseClick mouseClick, MouseMove mouseMove, int idx, Vector2 parentLocation, out Vector2 clickLocation)
        {
            clickLocation = new Vector2(0.0f, 0.0f);
            Vector2 clickPos = _screenCenter - new Vector2(mouseClick.X, mouseClick.Y) + _relativeCenter;
            var elem = _ui.ElementComps[idx].Element;
            var pos = elem.Position - elem.ReferenceCoord;
            var thisPos = parentLocation + elem.Position;
            if(clickPos.X >= pos.X && clickPos.X <= pos.X + elem.Width && clickPos.Y >= pos.Y && clickPos.Y <= pos.Y + elem.Height)
            {
                var existing = elem.ExistingElements;
                for(int jdx = 0; jdx < existing.Length; jdx++)
                {
                    if (existing[jdx])
                    {
                        var childPos = thisPos + elem.Position;
                        int childCheck = CheckClick(time, mouseClick, mouseMove, elem.ChildElementIDs[jdx], childPos, out clickLocation);
                        if (childCheck != -1)
                        {
                            clickLocation = clickPos - childPos;
                            return childCheck;
                        }
                    }
                }
                clickLocation = clickPos - thisPos;
                return idx;
            }

            return -1;
        }

        public void Update(float time)
        {

        }
    }
}
