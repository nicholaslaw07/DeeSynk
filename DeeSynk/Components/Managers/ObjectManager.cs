using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeeSynk.Components.Managers
{
    class ObjectManager : IManager
    {
        private static ObjectManager _objectManager;
        private const uint OBJECT_MEMORY = 10000;
        private Renderables.GameObject[] _gameObjects;
        private uint ObjectCount;

        private ObjectManager()
        {
            _gameObjects = new Renderables.GameObject[OBJECT_MEMORY];
            ObjectCount = 0;
        }

        public static ref ObjectManager GetInstance()
        {
            if (_objectManager == null)
                _objectManager = new ObjectManager();

            return ref _objectManager;
        }

        public Renderables.GameObject AddGameObject()
        {
            Renderables.GameObject newGameObject = _gameObjects[ObjectCount];



            return newGameObject;
        }

        public void Load()
        {

        }

        public Renderables.GameObject GetGameObject(int idx)
        {
            return _gameObjects[idx]; //will return null if space is not yet occupied
        }

         public void UnLoad()
        {
            
        }
    }
}
