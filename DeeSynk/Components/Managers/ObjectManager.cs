using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;
using OpenTK.Graphics;
using System.Threading.Tasks;

namespace DeeSynk.Components.Managers
{
    using GameObject = Renderables.GameObject;
    using ColoredVertex = Renderables.ColoredVertex;
    class ObjectManager : IManager
    {
        private static ObjectManager _objectManager;
        private const uint OBJECT_MEMORY = 10000;
        private GameObject[] _gameObjects;   // holds actual game objects
        private bool[] _existingGameObjects; // holds whether or not the object at corresponding index in _gameObjects has been deleted or not
        private int[] _gameObjectLayers;     // holds the layer that the corresponding GameObject is on, might be useful for a collision detector
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
        /// Creates a rectangular GameObject. The origin (0,0) is at the center of the GameWindow, and x and y
        /// use an origin at the center of the rectangle as well.
        /// </summary>
        /// <returns>The created GameObject, stored in the ObjectManager</returns>
        public ref GameObject CreateRectangle(int layer, int width, int height, int x, int y, Color4 color)
        {
            float xdist = width / 2;
            float ydist = height / 2;
            ColoredVertex[] vertices = {new ColoredVertex(new Vector4(-xdist, -ydist, 1.0f, 1.0f), color),
                                        new ColoredVertex(new Vector4(xdist, -ydist, 1.0f, 1.0f), color),
                                        new ColoredVertex(new Vector4(xdist, ydist, 1.0f, 1.0f), color),
                                        new ColoredVertex(new Vector4(-xdist, ydist, 1.0f, 1.0f), color)};
            uint[] indices = { 0, 1, 2, 0, 2, 3 };
            Vector3 position = new Vector3((float)x, (float)y, 0f);
            float rotX = 0f, rotY = 0f, rotZ = 0f;
            Vector3 scale = new Vector3(0.2f, 0.2f, 0.0f);

            int idx = GetNewGameObjectID();

            _gameObjects[idx] = new GameObject(
                idx,
                1,          // renderID
                layer, 
                vertices,
                indices,
                position,
                rotX,
                rotY,
                rotZ,
                scale);

            _gameObjectLayers[idx] = layer;
           
            return ref _gameObjects[idx];
        }

        /// <summary>
        /// Creates a circular GameObject. The origin (0,0) is at the center of the GameWindow, and x and y
        /// use an origin at the center of the circle as well.
        /// </summary>
        /// <returns>The created GameObject, stored in the ObjectManager</returns>
        public ref GameObject CreateCircle(int layer, int x, int y, int radius, Color4 color)
        {
            int triangleCount = 100;
            ColoredVertex[] vertices = new ColoredVertex[triangleCount + 1];
            uint[] indices = new uint[triangleCount * 3];

            vertices[0] = new ColoredVertex(new Vector4((float)x, (float)y, 1.0f, 1.0f), color);
            for (int i = 1; i <= triangleCount; i++)
            {
                float fx = (float)x + ((float)radius * (float)Math.Cos((double)i * 2d * Math.PI / (double)triangleCount));
                float fy = (float)y + ((float)radius * (float)Math.Sin((double)i * 2d * Math.PI / (double)triangleCount));

                vertices[i] = new ColoredVertex(new Vector4(fx, fy, 1.0f, 1.0f), color);
            }
            for (uint i=0; i < triangleCount; i++)
            {
                indices[i*3] = 0;
                indices[i*3 + 1] = i+1;
                if (i == triangleCount-1)
                {
                    indices[i * 3 + 2] = 1;
                }
                else
                {
                    indices[i * 3 + 2] = i + 2;
                }
            }
            Vector3 position = new Vector3((float)x, (float)y, 0f);
            float rotX = 0f, rotY = 0f, rotZ = 0f;
            Vector3 scale = new Vector3(0.2f, 0.2f, 0.0f);

            int idx = GetNewGameObjectID();
            
            _gameObjects[idx] = new GameObject(
                idx,
                1,          // renderID
                layer, 
                vertices,
                indices,
                position,
                rotX,
                rotY,
                rotZ,
                scale);

            _gameObjectLayers[idx] = layer;
           
            return ref _gameObjects[idx];
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
            _gameObjectLayers = new int[OBJECT_MEMORY]; // could run into an issue is 0 is a possible render layer
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
        public void Render(Matrix4 ortho)
        {
            for (int i = 0; i < MaxObjectCount; i++)
            {
                if (_existingGameObjects[i])
                {
                    _gameObjects[i].Render(ortho);
                }
            }
        }

        public void UnLoad()
        {
            
        }
    }
}
