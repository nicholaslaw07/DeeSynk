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

        private bool[] _monitoredGameObjects;

        private ComponentTransform[]    _transComps;
        private ComponentModelStatic[] _modelCompsStatic;

        private Camera _camera;

        public SystemTransform(World world)
        {
            _world = world;

            _monitoredGameObjects = new bool[_world.ObjectMemory];

            _transComps = _world.TransComps;
            _modelCompsStatic = _world.StaticModelComps;

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
                        _monitoredGameObjects[i] = true;
                    }
                }
            }

        }

        public void Update(float time)
        {
            for(int i=0; i< _world.ObjectMemory; i++)
            {
                if(_monitoredGameObjects[i])
                    _transComps[i].Update(time);
            }
            _camera.UpdateMatrices();
        }

        public void InitLocation()
        {
            //TEST START
            for (int i = 0; i < _world.ObjectMemory; i++)
            {
                if(_monitoredGameObjects[i])
                    _transComps[i] = new ComponentTransform(ref _modelCompsStatic[i]);
            }

            //_transComps[0].RotationXComp.InterpolateRotation(5f, 30f, InterpolationMode.LINEAR);
            _transComps[0].RotationYComp.InterpolateRotation(2f, 30f, InterpolationMode.LINEAR);
            //_transComps[0].LocationComp.InterpolateTranslation(new Vector3(3, 0, 0), 30, InterpolationMode.LINEAR);

            //_transComps[1].RotationXComp.InterpolateRotation(5f, 5f, InterpolationMode.LINEAR);
            //_transComps[1].LocationComp.InterpolateTranslation(new Vector3(0, 0, 20), 15, InterpolationMode.LINEAR);
            //TEST END
        }

        public void PushMatrixData(int index)
        {
            Matrix4 m4 = _transComps[index].GetModelMatrix * _camera.ViewProjection; //MVP
            GL.UniformMatrix4(5, false, ref m4);
        }

        public void PushMatrixDataNoTransform()
        {
            GL.Uniform3(4, _camera.Location);
            GL.UniformMatrix4(5, false, ref _camera.ViewProjection);
        }

        public void PushModelMatrix(int index)
        {
            Matrix4 m4 = _transComps[index].GetModelMatrix;
            GL.UniformMatrix4(13, false, ref m4);
        }
    }
}
