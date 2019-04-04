using System;
using DeeSynk.Core.Components.Types.Transform;
using DeeSynk.Core.Components.Types.Render;
using OpenTK;
using DeeSynk.Core.Components;
using DeeSynk.Core.Managers;

namespace DeeSynk.Core
{
    using GameObject = Components.GameObject;
    using Component = Components.Component;
    using SystemRender = Systems.SystemRender;
    using SystemTransform = Systems.SystemTransform;
    using SystemVAO = Systems.SystemVAO;
    using SystemModel = Systems.SystemModel;
    using Buffers = Systems.Buffers;

    public class World
    {
        private const uint OBJECT_MEMORY = 2;
        public uint ObjectMemory { get => OBJECT_MEMORY; }
        private GameObject[] _gameObjects;
        public GameObject[] GameObjects { get => _gameObjects; }
        private bool[] _existingGameObjects;
        public bool[] ExistingGameObjects { get => _existingGameObjects; }
        private int MaxObjectCount;

        private SystemRender    _systemRender;
        private SystemTransform _systemTransform;
        private SystemVAO       _systemVAO;
        private SystemModel     _systemModel;

        private ComponentTransform[]        _transComps;
        public ComponentTransform[] TransComps { get => _transComps; }

        private ComponentRender[]       _renderComps;
        public  ComponentRender[]       RenderComps      { get => _renderComps; }
        private ComponentModelStatic[]  _staticModelComps;
        public  ComponentModelStatic[]  StaticModelComps { get => _staticModelComps; }
        private ComponentTexture[]      _textureComps;
        public  ComponentTexture[]      TextureComps     { get => _textureComps; }

        public World()
        {
            _existingGameObjects = new bool[OBJECT_MEMORY];
            _gameObjects    = new GameObject[OBJECT_MEMORY];

            _systemRender = new SystemRender(this);
            _systemTransform = new SystemTransform(this);

            _transComps     = new ComponentTransform[OBJECT_MEMORY];

            _renderComps = new ComponentRender[OBJECT_MEMORY];
            _staticModelComps = new ComponentModelStatic[OBJECT_MEMORY];
            _textureComps = new ComponentTexture[OBJECT_MEMORY];

            _systemRender = new SystemRender(this);

            _systemModel = new SystemModel(this);
            _systemModel.InitModel();

            _systemTransform = new SystemTransform(this);
            _systemTransform.InitLocation();

            //TEST START
            _textureComps[1] = new ComponentTexture(TextureManager.GetInstance().GetTexture("Atlas_0"), 0);

            _systemVAO = new SystemVAO(this);
            _systemVAO.InitVAOInRange(Buffers.VERTICES_NORMALS_COLORS_ELEMENTS | Buffers.INSTANCES, 0, 0, true);
            _systemVAO.InitVAOInRange(Buffers.VERTICES_ELEMENTS | Buffers.UVS, 1, 1, true);
            //TEST END
        }

        public void PushCameraRef(ref Camera camera)
        {
            _systemTransform.PushCameraRef(ref camera);
            _systemRender.PushCameraRef(ref camera);
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

        public void Update(float time)
        {
            _systemTransform.Update(time);
        }

        public void Render()
        {
            //_systemRender.RenderAll(ref _systemTransform);
            _systemRender.RenderInstanced(ref _systemTransform, 0);
        }
    }
}
