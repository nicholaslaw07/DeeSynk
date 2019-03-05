using System;
using DeeSynk.Core.Components.Types.Transform;
using OpenTK;

namespace DeeSynk.Core
{
    using GameObject = Components.GameObject;
    using Component = Components.Component;

    public class World
    {
        private const uint OBJECT_MEMORY = 10000;
        private GameObject[] _gameObjects;
        private bool[] _existingGameObjects;
        private int MaxObjectCount;

        private ComponentLocation[]     _locationComps;
        private ComponentVelocity[]     _velocityComps;
        private ComponentGravity[]      _gravityComps;
        private ComponentRotation_X[]   _rotXComps;
        private ComponentRotation_Y[]   _rotYComps;
        private ComponentRotation_Z[]   _rotZComps;
        private ComponentScale[]        _scaleComps;

        public World()
        {
            _existingGameObjects = new bool[OBJECT_MEMORY];
            _gameObjects    = new GameObject[OBJECT_MEMORY];

            _locationComps  = new ComponentLocation[OBJECT_MEMORY];
            _velocityComps  = new ComponentVelocity[OBJECT_MEMORY];
            _gravityComps   = new ComponentGravity[OBJECT_MEMORY];
            _rotXComps      = new ComponentRotation_X[OBJECT_MEMORY];
            _rotYComps      = new ComponentRotation_Y[OBJECT_MEMORY];
            _rotZComps      = new ComponentRotation_Z[OBJECT_MEMORY];
            _scaleComps     = new ComponentScale[OBJECT_MEMORY];
            
            //_locationComps.Initialize();
            //_velocityComps.Initialize();
            //_gravityComps.Initialize();
            //_rotXComps.Initialize();
            //_rotYComps.Initialize();
            //_rotZComps.Initialize();
            //_scaleComps.Initialize();
            //_transformComps.Initialize();
        }

        /// <summary>
        /// Decides where in the allocated memory a new GameObject can be created.
        /// This should only be used if the returned index is DEFINITELY used to 
        /// instantiate a new GameObject.
        /// </summary>
        /// <returns>Index of free space in _gameObjects</returns>
        private int GetNewGameObjectID()
        {
            if (MaxObjectCount == OBJECT_MEMORY)
            {
                for (int i=0; i < MaxObjectCount; i++)
                {
                    if (!_existingGameObjects[i])
                    {
                        _existingGameObjects[i] = true;
                        return i;                        
                    }
                }
                throw new InvalidOperationException("Allocated object memory full.");
            }
            else
            {
                _existingGameObjects[MaxObjectCount] = true;
                return MaxObjectCount++;
            }
        }

        public ref GameObject CreateGameObject(int componentMask)
        {
            int id = GetNewGameObjectID();
            _gameObjects[id] = new GameObject(id, componentMask);
            return ref _gameObjects[id];
        }
        
        /// <summary>
        /// Doesn't delete the GameObject immediately, but sets the corresponding _existingGameObjects flag to false,
        /// such that the memory is made available when necessary.
        /// </summary>
        public void DeleteGameObject(int idx)
        {
            _existingGameObjects[idx] = false;
        }

        
        /// <summary>
        /// Given an index, returns the GameObject stored at that index. I'm not sure when this would be used,
        /// since the only object that stores the index is the GameObject itself. Perhaps render groups would 
        /// want to store the ID's as a list of integers?
        /// </summary>
        /// <returns>GameObject reference at specified index</returns>
        public ref GameObject GetGameObject(int idx)
        {
            if (_existingGameObjects[idx]) 
            {
                return ref _gameObjects[idx]; //will return null if space is not yet occupied
            }
            else
            {
                throw new InvalidOperationException("Non-existent GameObject requested.");
            }
        }

        public void InitializeComponents(ref GameObject obj, Vector4 location, Vector4 velocity, float rotX, float rotY, float rotZ, Vector3 scale)
        {
            //Not permanent
            int id = obj.ID;
            int bitMask = obj.Components;
            if ((bitMask & (int)Component.LOCATION) == ((int)Component.LOCATION))
                _locationComps[id] = new ComponentLocation(ref location);

            if ((bitMask & (int)Component.VELOCITY) == ((int)Component.VELOCITY))
                _velocityComps[id] = new ComponentVelocity(ref velocity);

            if ((bitMask & (int)Component.ROTATION_X) == ((int)Component.ROTATION_X))
                _rotXComps[id] = new ComponentRotation_X(rotX);

            if ((bitMask & (int)Component.ROTATION_Y) == ((int)Component.ROTATION_Y))
                _rotYComps[id] = new ComponentRotation_Y(rotY);

            if ((bitMask & (int)Component.ROTATION_Z) == ((int)Component.ROTATION_Z))
                _rotZComps[id] = new ComponentRotation_Z(rotZ);

            //if ((bitMask & (int)Component.SCALE) == ((int)Component.SCALE))
            //    _scaleComps[id] = new ComponentScale(ref scale);

            //if ((bitMask & (int)Component.GRAVITY) == ((int)Component.GRAVITY))
            //    _gravityComps[id] = new ComponentGravity(ref location);

            //if ((bitMask & (int)Component.TRANSFORM) == ((int)Component.TRANSFORM))
            //    _transformComps[id] = new ComponentTransform(ref location);

            _rotXComps[id].SetConstantRotation(2.0f);
            _rotYComps[id].SetConstantRotation(2.0f);
            _rotZComps[id].SetConstantRotation(2.0f);

        }

        public void Update(float time)
        {
            //TestStart
            for(int i=0; i<OBJECT_MEMORY; i++)
            {
                _rotXComps[i].Update(time);
                _rotYComps[i].Update(time);
                _rotZComps[i].Update(time);
            }
            //TestEnd
        }
    }
}
