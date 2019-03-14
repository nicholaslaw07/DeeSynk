using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DeeSynk.Core.Components;
using DeeSynk.Core.Components.Types.Transform;
using OpenTK;
using OpenTK.Graphics.OpenGL4;

namespace DeeSynk.Core.Systems
{
    class SystemTransform : ISystem
    {
        public int MonitoredComponents => (int)Component.LOCATION |
                                          (int)Component.VELOCITY |
                                          (int)Component.GRAVITY |
                                          (int)Component.ROTATION_X |
                                          (int)Component.ROTATION_Y |
                                          (int)Component.ROTATION_Z |
                                          (int)Component.SCALE;

        private World _world;

        private bool[] _monitoredGameObjects;

        private ComponentTransform[]    _transComps;

        private Camera _camera;

        public SystemTransform(World world)
        {
            _world = world;

            _monitoredGameObjects = new bool[_world.ObjectMemory];

            _transComps = _world.TransComps;

            UpdateMonitoredGameObjects();
        }

        public SystemTransform(World world, ref Matrix4 view, ref Matrix4 projection)
        {
            _world = world;

            _monitoredGameObjects = new bool[_world.ObjectMemory];

            _transComps = _world.TransComps;

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
                    if ((_world.GameObjects[i].Components | MonitoredComponents) == MonitoredComponents)
                    {
                        _monitoredGameObjects[i] = true;
                    }
                }
            }

        }

        public void Update(float time)
        {
            for(int i=0; i< _world.ObjectMemory; i++)
                _transComps[i].Update(time);
            _camera.UpdateMatrices();
        }

        public void InitLocation()
        {
            //TEST START
            
            Random r = new Random();
            for(int i=0; i < _world.ObjectMemory; i++)
            {
                //| Component.ROTATION_X | Component.ROTATION_Y | Component.ROTATION_Z | Component.SCALE
                _transComps[i] = new ComponentTransform((int)(Component.NONE));
            }

            //TEST END
        }

        public void InitLocation(int index)
        {
            //TEST START

            Random r = new Random();
           _transComps[index] = new ComponentTransform((int)(Component.LOCATION), r.Next(-500, 500), r.Next(-500, 500), r.Next(-1000, -300));
            //TEST END
        }

        public void PushMatrixData(int index)
        {
            Matrix4 m4 = _transComps[index].GetModelView * _camera.ViewProjection; //MVP
            GL.UniformMatrix4(4, false, ref m4);
        }

        public void PushMatrixDataNoTransform()
        {
            GL.Uniform3(4, _camera.Location);
            GL.Uniform3(5, new Vector3(50, 50, 50));
            GL.UniformMatrix4(6, false, ref _camera.ViewProjection);
        }
    }
}
