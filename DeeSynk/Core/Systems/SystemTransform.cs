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
        public int MonitoredComponents = (int)Component.LOCATION |
                                         (int)Component.VELOCITY |
                                         (int)Component.GRAVITY |
                                         (int)Component.ROTATION_X |
                                         (int)Component.ROTATION_Y |
                                         (int)Component.ROTATION_Z |
                                         (int)Component.SCALE;

        private World _world;

        private bool[] _monitoredGameObjects;
        
        private ComponentLocation[]     _locationComps;
        private ComponentVelocity[]     _velocityComps;
        private ComponentGravity[]      _gravityComps;
        private ComponentRotation_X[]   _rotXComps;
        private ComponentRotation_Y[]   _rotYComps;
        private ComponentRotation_Z[]   _rotZComps;
        private ComponentScale[]        _scaleComps;

        int ISystem.MonitoredComponents => throw new NotImplementedException();

        public SystemTransform(World world)
        {
            _world = world;

            _monitoredGameObjects = new bool[_world.ObjectMemory];

            _locationComps = _world.LocationComps;
            _velocityComps = _world.VelocityComps;
            _gravityComps = _world.GravityComps;
            _rotXComps = _world.RotXComps;
            _rotYComps = _world.RotYComps;
            _rotZComps = _world.RotZComps;
            _scaleComps = _world.ScaleComps;

            UpdateMonitoredGameObjects();
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
        }

        public void InitLocation()
        {
            for(int i=0; i < _locationComps.Length; i++)
            {
                _locationComps[i] = new ComponentLocation();
            }
        }

        public void SendMatrixData(int index)
        {
            var v = _locationComps[index].Location.Xyz;
            Matrix4 m4 = Matrix4.Identity;
            Matrix4.CreateTranslation(ref v, out m4);
            m4 *= Matrix4.CreateOrthographic(700, 500, -1, 1);
            GL.UniformMatrix4(2, false, ref m4);
        }
    }
}
