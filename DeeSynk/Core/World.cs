using System;
using DeeSynk.Core.Components.Types.Transform;
using DeeSynk.Core.Components.Types.Render;
using OpenTK;
using DeeSynk.Core.Components;
using DeeSynk.Core.Managers;
using OpenTK.Graphics.OpenGL4;

namespace DeeSynk.Core
{
    using GameObject = Components.GameObject;
    using Component = Components.Component;
    using SystemRender = Systems.SystemRender;
    using SystemTransform = Systems.SystemTransform;
    using SystemVAO = Systems.SystemVAO;
    using SystemModel = Systems.SystemModel;
    using Buffers = Systems.Buffers;


    //Possible configuations of entities
    //  Render - (Transform, Model, Texture*, Material*)
    //  Camera
    //  Camera with Objects (Render as above)
    //  Light - Transform?


    public class World
    {
        private const uint OBJECT_MEMORY = 6;
        /// <summary>
        /// Maximum number of GameObjects that can be stored inside of this world.
        /// </summary>
        public uint ObjectMemory { get => OBJECT_MEMORY; }
        private GameObject[] _gameObjects;
        /// <summary>
        /// Array containing all GameObjects within this world.
        /// </summary>
        public GameObject[] GameObjects { get => _gameObjects; }
        private bool[] _existingGameObjects;
        /// <summary>
        /// Specifies if a game object does or does not exist at the specified index within the array.
        /// </summary>
        public bool[] ExistingGameObjects { get => _existingGameObjects; }
        private int MaxObjectCount;

        //Systems that act as a medium for components to communicate through, specific to certain purposes
        #region SYSTEMS
        private SystemRender    _systemRender;
        public  SystemRender     SystemRender    { get => _systemRender; }
        private SystemTransform _systemTransform;
        public  SystemTransform  SystemTransform { get => _systemTransform; }
        private SystemVAO       _systemVAO;
        public  SystemVAO        SystemVAO       { get => _systemVAO; }
        private SystemModel     _systemModel;
        public  SystemModel      SystemModel     { get => _systemModel; }
        #endregion

        //The arrays that store all of the components inside of this world object, their capactiy is limited by OBJECT_MEMORY
        #region COMPONENT_ARRAYS
        private ComponentTransform[]    _transComps;
        public ComponentTransform[]     TransComps       { get => _transComps; }
        private ComponentRender[]       _renderComps;
        public  ComponentRender[]       RenderComps      { get => _renderComps; }
        private ComponentModelStatic[]  _staticModelComps;
        public  ComponentModelStatic[]  StaticModelComps { get => _staticModelComps; }
        private ComponentTexture[]      _textureComps;
        public  ComponentTexture[]      TextureComps     { get => _textureComps; }
        private ComponentCamera[]       _cameraComps;
        public  ComponentCamera[]       CameraComps      { get => _cameraComps; }
        private ComponentLight[]        _lightComps;
        public  ComponentLight[]        LightComps       { get => _lightComps; }
        #endregion

        private VAO[] _vaos;
        /// <summary>
        /// The array containing all VAOs that are in use by the GameObjects in this world object.
        /// </summary>
        public VAO[] VAOs { get => _vaos; }

        public World()
        {
            _existingGameObjects = new bool[OBJECT_MEMORY];
            _gameObjects      = new GameObject[OBJECT_MEMORY];

            _systemRender     = new SystemRender(this);
            _systemTransform  = new SystemTransform(this);

            _transComps       = new ComponentTransform[OBJECT_MEMORY];
            _renderComps      = new ComponentRender[OBJECT_MEMORY];
            _staticModelComps = new ComponentModelStatic[OBJECT_MEMORY];
            _textureComps     = new ComponentTexture[OBJECT_MEMORY];
            _cameraComps      = new ComponentCamera[OBJECT_MEMORY];
            _lightComps       = new ComponentLight[OBJECT_MEMORY];

            _vaos             = new VAO[OBJECT_MEMORY];

            _systemRender     = new SystemRender(this);
            _systemModel      = new SystemModel(this);
            _systemTransform  = new SystemTransform(this);
            _systemVAO        = new SystemVAO(this);
        }

        public void InitData()
        {
            _systemModel.UpdateMonitoredGameObjects();
            _systemTransform.UpdateMonitoredGameObjects();

            _systemModel.InitModel();
            _systemTransform.InitLocation();
        }

        //CREATE NEW OBJECT WITH BITMASKID
        //ADD COMPONENTS VIA METHOD
        //IF GAMEOBJECT AT SPECIFIED INDEX ALLOWS COMPONENT, ADD IT
        //OTHERWISE DO NOTHING
        //ALLOW TEMPLATING (ADD CAMERA, ADD PLAYER, ETC)

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
        /// Performs an update call on the systems within this world object.
        /// </summary>
        /// <param name="time"></param>
        public void Update(float time)
        {
            _systemTransform.Update(time);
        }

        public void Render()
        {
            _systemRender.RenderAll(ref _systemTransform);
        }
    }
}
