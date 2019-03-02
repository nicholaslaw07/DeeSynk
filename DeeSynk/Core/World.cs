using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeeSynk.Core
{
    using GameObject = Components.GameObject;
    
    public class World
    {
        private const uint OBJECT_MEMORY = 10000;
        private GameObject[] _gameObjects;
        private bool[] _existingGameObjects;
        private int MaxObjectCount;

        //private Components.ComponentLocation[] componentLocations;
        //private Components.ComponentTransform[] componentTransforms;
        //private Components.ComponentVelocity[] componentVelocities;
        //private Components.ComponentGravity[] componentGravities;
        //private Components.ComponentRotation_X[] componentRotation_Xs;
        //private Components.ComponentRotation_Y[] componentRotation_Ys;
        //private Components.ComponentRotation_Z[] componentRotation_Zs;
        //private Components.ComponentScale[] componentScales;

        public World()
        {
            MaxObjectCount = 0;
            _gameObjects = new GameObject[OBJECT_MEMORY];
            _existingGameObjects = new bool[OBJECT_MEMORY];

            // load data provided by Game
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
