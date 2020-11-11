using DeeSynk.Core.Components;
using DeeSynk.Core.Components.Types.Render;
using DeeSynk.Core.Components.Types.Transform;
using DeeSynk.Core.Components.Types.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeeSynk.Core
{
    public abstract class GameObjectContainer
    {
        protected uint OBJECT_MEMORY;
        /// <summary>
        /// Maximum number of canvases that can be stored and switched between
        /// </summary>
        public uint ObjectMemory { get => OBJECT_MEMORY; }

        protected GameObject[] _gameObjects;
        /// <summary>
        /// Array containing all GameObjects within this world.
        /// </summary>
        public GameObject[] GameObjects { get => _gameObjects; }
        protected bool[] _existingGameObjects;
        /// <summary>
        /// Specifies if a game object does or does not exist at the specified index within the array.
        /// </summary>
        public bool[] ExistingGameObjects { get => _existingGameObjects; }
        protected int MaxObjectCount;

        protected int _compIdx;
        /// <summary>
        /// Index of the component currently being added.
        /// </summary>
        public int CompIdx { get => _compIdx; }

        //The arrays that store all of the components inside of this world object, their capactiy is limited by OBJECT_MEMORY
        #region COMPONENT_ARRAYS
        protected ComponentTransform[] _transComps;
        public ComponentTransform[] TransComps { get => _transComps; }
        protected ComponentRender[] _renderComps;
        public ComponentRender[] RenderComps { get => _renderComps; }
        protected ComponentModelStatic[] _staticModelComps;
        public ComponentModelStatic[] StaticModelComps { get => _staticModelComps; }
        protected ComponentTexture[] _textureComps;
        public ComponentTexture[] TextureComps { get => _textureComps; }
        protected ComponentMaterial[] _materialComps;
        public ComponentMaterial[] MaterialComps { get => _materialComps; }
        protected ComponentCamera[] _cameraComps;
        public ComponentCamera[] CameraComps { get => _cameraComps; }
        protected ComponentLight[] _lightComps;
        public ComponentLight[] LightComps { get => _lightComps; }
        protected ComponentCanvas[] _canvasComps;
        public ComponentCanvas[] CanvasComps { get => _canvasComps; }
        protected ComponentElement[] _elementComps;
        public ComponentElement[] ElementComps { get => _elementComps; }
        protected ComponentText[] _textComps;
        public ComponentText[] TextComps { get => _textComps; }
        #endregion


        public GameObjectContainer(uint objectMemory)
        {
            OBJECT_MEMORY = objectMemory;

            _existingGameObjects = new bool[OBJECT_MEMORY];
            _gameObjects = new GameObject[OBJECT_MEMORY];

            _transComps = new ComponentTransform[OBJECT_MEMORY];
            _renderComps = new ComponentRender[OBJECT_MEMORY];
            _staticModelComps = new ComponentModelStatic[OBJECT_MEMORY];
            _textureComps = new ComponentTexture[OBJECT_MEMORY];
            _materialComps = new ComponentMaterial[OBJECT_MEMORY];
            _cameraComps = new ComponentCamera[OBJECT_MEMORY];
            _lightComps = new ComponentLight[OBJECT_MEMORY];
            _canvasComps = new ComponentCanvas[OBJECT_MEMORY];
            _elementComps = new ComponentElement[OBJECT_MEMORY];
            _textComps = new ComponentText[OBJECT_MEMORY];

            _compIdx = 0;
        }

        public abstract void InitData();

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
                for (int i = 0; i < MaxObjectCount; i++)
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

        public ref GameObject CreateGameObject(Component componentMask)
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

        /// <summary>
        /// Increases the current world component index by 1 and returns the value.
        /// </summary>
        /// <returns>The incremented index.</returns>
        public int NextComponentIndex()
        {
            return ++_compIdx;
        }

        /// <summary>
        /// Increases the current world component index by 1, but does not return the value.
        /// </summary>
        public void IncrementComponentIndex()
        {
            _compIdx++;
        }

        /// <summary>
        /// Performs an update call on the systems within this world object.
        /// </summary>
        /// <param name="time"></param>
        public abstract void Update(float time);
    }
}
