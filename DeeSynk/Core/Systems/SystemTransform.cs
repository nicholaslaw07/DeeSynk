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
            for (int i=0; i < _world.ObjectMemory; i++)
            {
                if (_world.ExistingGameObjects[i])
                {
                    if (_world.GameObjects[i].Components.HasFlag(MonitoredComponents))
                    {
                        _monitoredGameObjects_W[i] = true;
                    }
                }
            }

            for (int i = 0; i < _ui.ObjectMemory; i++)
            {
                if (_ui.ExistingGameObjects[i])
                {
                    if (_ui.GameObjects[i].Components.HasFlag(MonitoredComponents))
                    {
                        _monitoredGameObjects_U[i] = true;
                    }
                }
            }

        }

        public void Update(float time)
        {
            for(int i=0; i< _world.ObjectMemory; i++)
            {
                if(_monitoredGameObjects_W[i])
                    _transComps_W[i].Update(time);
            }
            for(int i=0; i< _ui.ObjectMemory; i++)
            {
                if (_monitoredGameObjects_U[i])
                    _transComps_U[i].Update(time);
            }
            _camera.UpdateMatrices();
        }

        public void InitLocation()
        {
            //TEST START
            for (int i = 0; i < _world.ObjectMemory; i++)
            {
                if(_monitoredGameObjects_W[i])
                    _transComps_W[i] = new ComponentTransform(ref _modelCompsStatic_W[i]);
            }

            //_transComps[0].RotationXComp.InterpolateRotation(-5f, 30f, InterpolationMode.LINEAR);
            //_transComps[0].RotationYComp.InterpolateRotation(2f, 30f, InterpolationMode.LINEAR);
            //_transComps[0].LocationComp.InterpolateTranslation(new Vector3(3, 0, 0), 30, InterpolationMode.LINEAR);

            //_transComps[1].RotationXComp.InterpolateRotation(5f, 5f, InterpolationMode.LINEAR);
            //_transComps[1].LocationComp.InterpolateTranslation(new Vector3(0, 0, 20), 15, InterpolationMode.LINEAR);
            //TEST END

            for (int i = 0; i < _ui.ObjectMemory; i++)
            {
                if (_monitoredGameObjects_U[i])
                    _transComps_U[i] = new ComponentTransform(ref _modelCompsStatic_U[i]);
            }
        }

        public void PushMatrixData(int index)
        {
            Matrix4 m4 = _transComps_W[index].GetModelMatrix * _camera.ViewProjection; //MVP
            GL.UniformMatrix4(5, false, ref m4);
        }

        public void PushMatrixDataUI(int index)
        {
            Matrix4 m4 = _transComps_U[index].GetModelMatrix * _camera.ViewProjection; //MVP
            GL.UniformMatrix4(5, false, ref m4);
        }

        public void PushMatrixDataNoTransform()
        {
            GL.Uniform3(4, _camera.Location);
            GL.UniformMatrix4(5, false, ref _camera.ViewProjection);
        }

        public void PushModelMatrix(int index)
        {
            Matrix4 m4 = _transComps_W[index].GetModelMatrix;
            GL.UniformMatrix4(13, false, ref m4);
        }

        public void PushModelMatrixUI(int index)
        {
            Matrix4 m4 = _transComps_U[index].GetModelMatrix;
            GL.UniformMatrix4(13, false, ref m4);
        }
    }
}

//This class is complete garbage