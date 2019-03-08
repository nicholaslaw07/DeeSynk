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
        
        private ComponentLocation[]     _locationComps;
        private ComponentVelocity[]     _velocityComps;
        private ComponentGravity[]      _gravityComps;
        private ComponentRotation_X[]   _rotXComps;
        private ComponentRotation_Y[]   _rotYComps;
        private ComponentRotation_Z[]   _rotZComps;
        private ComponentScale[]        _scaleComps;
        private ComponentTransform[]    _transComps;

        private Camera _camera;

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
            _transComps = _world.TransComps;

            UpdateMonitoredGameObjects();
        }

        public SystemTransform(World world, ref Matrix4 view, ref Matrix4 projection)
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
            {
                bool recomputeProduct = false;

                int tm = _transComps[i].TransformComponentsMask;
                var tc = _transComps[i];

                if ((tm & (int)Component.LOCATION) != 0)
                {
                    _locationComps[i].Update(time);
                    if (_locationComps[i].ValueUpdated)
                    {
                        recomputeProduct = true;
                        tc.PushLocation(ref _locationComps[i].GetLocationByRef());
                        _locationComps[i].CompleteUpdate();
                    }
                }
                if ((tm & (int)Component.VELOCITY) != 0)
                {
                    if (_velocityComps[i].ValueUpdated)
                    {
                        recomputeProduct = true;
                        _velocityComps[i].Update(time);
                        //similar, push location
                    }
                }
                if ((tm & (int)Component.GRAVITY)  != 0)
                {
                //    if (_gravityComps[i].ValueUpdated)
                //    {
                //        recomputeProduct = true;
                //        _gravityComps[i].Update(time);
                //        also a push location thingy
                //    }
                }
                if ((tm & (int)Component.ROTATION_X) != 0)
                {
                    _rotXComps[i].Update(time);
                    if (_rotXComps[i].ValueUpdated)
                    {
                        recomputeProduct = true;
                        tc.PushRotationX(_rotXComps[i].Rotation);
                        _rotXComps[i].CompleteUpdate();
                    }
                }
                if ((tm & (int)Component.ROTATION_Y) != 0)
                {
                    _rotYComps[i].Update(time);
                    if (_rotYComps[i].ValueUpdated)
                    {
                        recomputeProduct = true;
                        tc.PushRotationY(_rotYComps[i].Rotation);
                        _rotYComps[i].CompleteUpdate();
                    }
                }
                if ((tm & (int)Component.ROTATION_Z) != 0)
                {
                    _rotZComps[i].Update(time);
                    if (_rotZComps[i].ValueUpdated)
                    {
                        recomputeProduct = true;
                        tc.PushRotationZ(_rotZComps[i].Rotation);
                        _rotZComps[i].CompleteUpdate();
                    }
                }
                if ((tm & (int)Component.SCALE) != 0)
                {
                    _scaleComps[i].Update(time);
                    if (_scaleComps[i].ValueUpdated)
                    {
                        recomputeProduct = true;
                        tc.PushScale(ref _scaleComps[i].GetScaleByRef());
                        _scaleComps[i].CompleteUpdate();
                    }
                }

                if (recomputeProduct)
                    tc.ComputeModelViewProduct();
            }
            _camera.UpdateMatrices();
        }

        public void InitLocation()
        {
            Random r = new Random();
            for(int i=0; i < _transComps.Length; i++)
            {
                _transComps[i] = new ComponentTransform((int)(Component.LOCATION | Component.ROTATION_X | Component.ROTATION_Y | Component.ROTATION_Z | Component.SCALE));

                _locationComps[i] = new ComponentLocation(r.Next(-500, 500), r.Next(-500, 500), r.Next( -1000, -300));

                _rotXComps[i] = new ComponentRotation_X(0.0f); //(float)(6.28 * r.NextDouble())
                //_rotXComps[i].SetConstantRotation((float)r.NextDouble()*2f - 1f);

                _rotYComps[i] = new ComponentRotation_Y(0.0f);
                //_rotYComps[i].SetConstantRotation((float)r.NextDouble() * 2f - 1f);

                _rotZComps[i] = new ComponentRotation_Z(0.0f);
                //_rotZComps[i].SetConstantRotation((float)r.NextDouble() * 20f - 10f);

                _scaleComps[i] = new ComponentScale((float)(r.NextDouble() * r.NextDouble()) * 2f, false); //(float)r.NextDouble() * 2f, (float)r.NextDouble() * 2f
            }
        }

        public void PushMatrixData(int index)
        {
            Matrix4 m4 = _transComps[index].GetModelView * _camera.ViewProjection; //MVP
            GL.UniformMatrix4(2, false, ref m4);
        }
    }
}
