using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DeeSynk.Core.Components;
using DeeSynk.Core.Components.Types.Render;
using DeeSynk.Core.Components.Types.Transform;
using OpenTK;
using OpenTK.Graphics.OpenGL4;

namespace DeeSynk.Core.Systems
{
    public class SystemTransform : ISystem
    {
        public Component MonitoredComponents => Component.TRANSFORM;

        private World _world;
        private UI _ui;

        private bool[] _monitoredGameObjects_W;
        private bool[] _monitoredGameObjects_U;

        private ComponentTransform[] _transComps_W;
        private ComponentModelStatic[] _modelCompsStatic_W;

        private ComponentTransform[] _transComps_U;
        private ComponentModelStatic[] _modelCompsStatic_U;

        private Camera _camera;

        public SystemTransform(World world, UI ui)
        {
            _world = world;
            _ui = ui;

            _monitoredGameObjects_W = new bool[_world.ObjectMemory];
            _monitoredGameObjects_U = new bool[_ui.ObjectMemory];

            _transComps_W = _world.TransComps;
            _modelCompsStatic_W = _world.StaticModelComps;

            _transComps_U = _ui.TransComps;
            _modelCompsStatic_U = _ui.StaticModelComps;

            UpdateMonitoredGameObjects();
        }

        public void PushCameraRef(ref Camera camera)
        {
            _camera = camera;
        }

        public void UpdateMonitoredGameObjects()
        {
            UpdateMonitoredGameObjects(_world, _monitoredGameObjects_W);
            UpdateMonitoredGameObjects(_ui, _monitoredGameObjects_U);
        }

        private void UpdateMonitoredGameObjects(GameObjectContainer c, bool[] monitor)
        {
            for (int i = 0; i < c.ObjectMemory; i++)
            {
                if (c.ExistingGameObjects[i])
                {
                    if (c.GameObjects[i].Components.HasFlag(MonitoredComponents))
                    {
                        monitor[i] = true;
                    }
                }
            }
        }

        public void Update(float time)
        {
            Update(time, _world, _monitoredGameObjects_W);
            Update(time, _ui, _monitoredGameObjects_U);
            _camera.UpdateMatrices();
        }

        private void Update(float time, GameObjectContainer c, bool[] monitor)
        {
            for (int i = 0; i < c.ObjectMemory; i++)
            {
                if (monitor[i])
                    c.TransComps[i].Update(time);
            }
        }

        public void InitLocation()
        {
            //TEST START
            CreateComponents(_world, _monitoredGameObjects_W);

            //_transComps[0].RotationXComp.InterpolateRotation(-5f, 30f, InterpolationMode.LINEAR);
            //_transComps[0].RotationYComp.InterpolateRotation(2f, 30f, InterpolationMode.LINEAR);
            //_transComps[0].LocationComp.InterpolateTranslation(new Vector3(3, 0, 0), 30, InterpolationMode.LINEAR);

            //_transComps[1].RotationXComp.InterpolateRotation(5f, 5f, InterpolationMode.LINEAR);
            //_transComps[1].LocationComp.InterpolateTranslation(new Vector3(0, 0, 20), 15, InterpolationMode.LINEAR);
            //TEST END
            CreateComponents(_ui, _monitoredGameObjects_U);

        }

        private void CreateComponents(GameObjectContainer c, bool[] monitor)
        {
            for (int i = 0; i < c.ObjectMemory; i++)
            {
                if (monitor[i])
                    c.TransComps[i] = new ComponentTransform(ref c.StaticModelComps[i]);
            }
        }

        public void PushMatrixData(int index, GameObjectContainer c)
        {
            Matrix4 m4 = c.TransComps[index].GetModelMatrix * _camera.ViewProjection; //MVP
            GL.UniformMatrix4(5, false, ref m4);
        }

        public void PushMatrixDataNoTransform()
        {
            GL.Uniform3(4, _camera.Location);
            GL.UniformMatrix4(5, false, ref _camera.ViewProjection);
        }

        public void PushModelMatrix(int index, GameObjectContainer c)
        {
            Matrix4 m4 = c.TransComps[index].GetModelMatrix;
            GL.UniformMatrix4(13, false, ref m4);
        }
    }
}

//This class is complete garbage