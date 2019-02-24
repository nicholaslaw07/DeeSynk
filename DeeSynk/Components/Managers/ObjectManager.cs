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
        private int MaxObjectCount;          // number of objects as if none have been deleted

        private ObjectManager()
        {
            _gameObjects = new GameObject[OBJECT_MEMORY];
            _existingGameObjects = new bool[OBJECT_MEMORY];
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

        public GameObject CreateRectangle(int layer, int width, int height, int x, int y, Color4 color)
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
                1,          // renderID
                layer, 
                vertices,
                indices,
                position,
                rotX,
                rotY,
                rotZ,
                scale);
           
            // get renderID, renderLayer, ColoredVertex vertices, uint indices


            return _gameObjects[idx];
        }

        public void DeleteGameObject(int idx)
        {
            _existingGameObjects[idx] = false;
        }

        public void Load()
        {

        }

        public GameObject GetGameObject(int idx)
        {
            if (_existingGameObjects[idx]) 
            {
                return _gameObjects[idx]; //will return null if space is not yet occupied
            }
            else
            {
                return null;
            }
        }

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
