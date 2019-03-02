using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;
using OpenTK.Graphics;
using System.Threading.Tasks;

namespace DeeSynk.Core.Managers
{

    using GameObject = Renderables.GameObject;
    using ColoredVertex = Renderables.ColoredVertex;
    using TexturedVertex = Renderables.TexturedVertex;
    class ObjectManager : IManager
    {
        private static ObjectManager _objectManager;
        private const uint OBJECT_MEMORY = 10000;
        private GameObject[] _gameObjects;   // holds actual game objects
        private bool[] _existingGameObjects; // holds whether or not the object at corresponding index in _gameObjects has been deleted or not
        private int MaxObjectCount;          // number of objects as if none have been deleted


        private const float PI = (float)Math.PI;

        /// <summary>
        /// Constructor private to maintain singleton structure
        /// </summary>
        private ObjectManager()
        {
            MaxObjectCount = 0;
        }

        /// <summary>
        /// Singleton implementation means this is the only way to get a reference to the
        /// ObjectManager class.
        /// </summary>
        /// <returns>Sole instance of ObjectManager</returns>
        public static ref ObjectManager GetInstance()
        {
            if (_objectManager == null)
                _objectManager = new ObjectManager();

            return ref _objectManager;
        }

        public ref GameObject CreateGameObject()
        {
            int id = GetNewGameObjectID();
            int components = GetComponentsInt();
            _gameObjects[id] = new GameObject(id, components);
            return ref _gameObjects[id];
        }

        public int GetComponentsInt()
        {
            int components = 0;

            return components;
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

        /// <summary>
        /// Doesn't delete the GameObject immediately, but sets the corresponding _existingGameObjects flag to false,
        /// such that the memory is made available when necessary.
        /// </summary>
        public void DeleteGameObject(int idx)
        {
            _existingGameObjects[idx] = false;
        }

        public void Load()
        {
            _gameObjects = new GameObject[OBJECT_MEMORY];
            _existingGameObjects = new bool[OBJECT_MEMORY];
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

        /// <summary>
        /// Iterates through all existing game objects, and calls their render method.
        /// </summary>
        public void Render()
        {
        }

        public void UnLoad()
        {
            
        }
    }
}
