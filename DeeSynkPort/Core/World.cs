using System;
using DeeSynk.Core.Components;
using DeeSynk.Core.Components.Types.Transform;
using DeeSynk.Core.Components.Types.Render;
using DeeSynk.Core.Components.Types.UI;
using OpenTK;
using OpenTK.Graphics.OpenGL4;
using DeeSynk.Core.Components.GraphicsObjects;

namespace DeeSynk.Core
{
    using GameObject = Components.GameObject;
    using Component = Components.Component;


    //Possible configuations of entities
    //  Render - (Transform, Model, Texture*, Material*)
    //  Camera
    //  Camera with Objects (Render as above)
    //  Light - Transform?

    /// <summary>
    /// Storage location for everything located within the scene whether direct or not.  This includes all or most rendering components.  These components or objects are then passed down to the respective systems unless they are definitively exclusive to a system.
    /// </summary>
    public class World : GameObjectContainer
    {

        private const uint FBO_COUNT = 4;
        /// <summary>
        /// Total number of FBO objects available.
        /// </summary>
        public uint FBO_Count { get => FBO_COUNT; }

        private VAO[] _vaos;
        /// <summary>
        /// The array containing all VAOs that are in use by the GameObjects in this world object.
        /// </summary>
        public VAO[] VAOs { get => _vaos; }

        private FBO[] _fbos;
        /// <summary>
        /// Array of frame buffer objects.
        /// </summary>
        public FBO[] FBOs { get => _fbos; }

        public World(uint objectMemory) : base(objectMemory)
        {   
            _vaos             = new VAO[OBJECT_MEMORY];
            _fbos             = new FBO[FBO_COUNT];
        }

        public override void InitData()
        {
            _fbos[0] = new FBO(MainWindow.width, MainWindow.height);
        }



        public override void Update(float time)
        {

        }

        //CREATE NEW OBJECT WITH BITMASKID
        //ADD COMPONENTS VIA METHOD
        //IF GAMEOBJECT AT SPECIFIED INDEX ALLOWS COMPONENT, ADD IT
        //OTHERWISE DO NOTHING
        //ALLOW TEMPLATING (ADD CAMERA, ADD PLAYER, ETC)
    }
}
