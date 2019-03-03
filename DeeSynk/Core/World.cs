using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DeeSynk.Core.Components.Groups;
using DeeSynk.Core.Components.Types;

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
        private ComponentTransform[]    _transformComps;

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
            _transformComps = new ComponentTransform[OBJECT_MEMORY];
            
            _locationComps.Initialize();
            _velocityComps.Initialize();
            _gravityComps.Initialize();
            _rotXComps.Initialize();
            _rotYComps.Initialize();
            _rotZComps.Initialize();
            _scaleComps.Initialize();
            _transformComps.Initialize();
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
    }
}
