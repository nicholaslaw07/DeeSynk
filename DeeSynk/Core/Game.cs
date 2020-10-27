using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DeeSynk.Core.Algorithms;
using DeeSynk.Core.Components;
using DeeSynk.Core.Components.GraphicsObjects.Lights;
using DeeSynk.Core.Components.GraphicsObjects.Shadows;
using DeeSynk.Core.Components.Types.Render;
using DeeSynk.Core.Managers;
using DeeSynk.Core.Systems;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;

namespace DeeSynk.Core
{
    /// <summary>
    /// All objects and mechanic modeling should be housed here.
    /// </summary>
    public class Game
    {
        private World _world;

        //This is used for loading objects into world.  Since each object has its own unique index, we only need a
        //global index counter that works for all objects.  (i.e. two seperate objects cannot both have index 4)
        private int _compIdx;

        //Systems that act as a medium for components to communicate through, specific to certain purposes
        #region SYSTEMS
        private SystemInput _systemInput;
        public SystemInput SystemInput { get => _systemInput; }

        private SystemRender _systemRender;
        public SystemRender SystemRender { get => _systemRender; }
        private SystemTransform _systemTransform;
        public SystemTransform SystemTransform { get => _systemTransform; }
        private SystemVAO _systemVAO;
        public SystemVAO SystemVAO { get => _systemVAO; }
        private SystemModel _systemModel;
        public SystemModel SystemModel { get => _systemModel; }
        #endregion

        public Game()
        {
            Load();
        }

        /// <summary>
        /// Responsible for loading all resources that will be used within the game and engine.
        /// </summary>
        public void Load()
        {
            Managers.ShaderManager.GetInstance().Load();
            Managers.TextureManager.GetInstance().Load();
            Managers.ModelManager.GetInstance().Load();

            _world = new World();

            _compIdx = 0;

            _systemInput = new SystemInput();

            _systemRender = new SystemRender(_world);
            _systemModel = new SystemModel(_world);
            _systemTransform = new SystemTransform(_world);
            _systemVAO = new SystemVAO(_world);
        }

        public void PushCameraRef(ref Camera camera)
        {
            _systemTransform.PushCameraRef(ref camera);
            _systemRender.PushCameraRef(ref camera);
        }

        public void LoadGameData()
        {

            _world.InitData();

            //Create the objects in the world array of GameObjects - these objects are blank and only hold the ComponentMask
            _world.CreateGameObject(Component.RENDER | Component.MODEL_STATIC | Component.TRANSFORM);
            _world.CreateGameObject(Component.RENDER | Component.MODEL_STATIC | Component.TRANSFORM | Component.TEXTURE);
            _world.CreateGameObject(Component.RENDER | Component.MODEL_STATIC | Component.TRANSFORM | Component.TEXTURE);

            _world.CreateGameObject(Component.LIGHT);
            _world.CreateGameObject(Component.LIGHT);
            _world.CreateGameObject(Component.LIGHT);

            _world.CreateGameObject(Component.LIGHT);

            _world.CreateGameObject(Component.CAMERA);

            _systemModel.UpdateMonitoredGameObjects();
            _systemModel.InitModel();
            _systemTransform.UpdateMonitoredGameObjects();
            _systemTransform.InitLocation();

            //Initialize all of the data in the world

            //Essentially, for each object we initialize its unique characteristics
            //TODO add the ability to load in these objects from a file to make the process act more how it would in a real world scenario
            _world.TextureComps[1] = new ComponentTexture(TextureManager.GetInstance().GetTexture("wood"), 0);



            var sm = ShaderManager.GetInstance();

            //the models are added via the SystemModel class

            _systemVAO.InitVAORange(Buffers.VERTICES_NORMALS_ELEMENTS | Buffers.INTERLEAVED, _compIdx, _compIdx);
            _world.RenderComps[_compIdx].PROGRAM_ID = sm.GetProgram("coloredPhongShaded");
            _world.RenderComps[_compIdx].ValidateData();

            IncrementComponentIndex();
            _systemVAO.InitVAORange(Buffers.VERTICES | Buffers.UVS | Buffers.FACE_ELEMENTS | Buffers.INTERLEAVED, _compIdx, _compIdx);
            _world.RenderComps[_compIdx].PROGRAM_ID = sm.GetProgram("shadowTextured2");
            _world.RenderComps[_compIdx].ValidateData();

            IncrementComponentIndex();
            _systemVAO.InitVAORange(Buffers.VERTICES | Buffers.UVS | Buffers.FACE_ELEMENTS | Buffers.INTERLEAVED, _compIdx, _compIdx);
            _world.RenderComps[_compIdx].PROGRAM_ID = sm.GetProgram("postLightGlare");
            _world.RenderComps[_compIdx].IsFinalRenderPlane = true;
            _world.RenderComps[_compIdx].ValidateData();

            //Automated UBO managment is a MUST

            IncrementComponentIndex();
            _world.LightComps[_compIdx] = new ComponentLight(LightType.SPOTLIGHT, 
                                                        new SpotLight(Color4.Red, 
                                                                      new Vector3(-3.0f, 5.0f, 6.0f), new Vector3(0.0f), new Vector3(0.0f, 1.0f, 0.0f),
                                                                      0.3f, 1.0f, 5f, 11f));

            _world.LightComps[_compIdx].LightObject.BuildUBO(3, 8);
            _world.LightComps[_compIdx].LightObject.ShadowMap = new ShadowMap(2048, 2048, TextureUnit.Texture1);

            IncrementComponentIndex();
            _world.LightComps[_compIdx] = new ComponentLight(LightType.SPOTLIGHT,
                                                        new SpotLight(Color4.Blue,
                                                                      new Vector3(3.0f, 5.0f, 6.0f), new Vector3(0.0f), new Vector3(0.0f, 1.0f, 0.0f),
                                                                      0.3f, 1.0f, 5f, 11f));

            _world.LightComps[_compIdx].LightObject.BuildUBO(4, 8);
            _world.LightComps[_compIdx].LightObject.ShadowMap = new ShadowMap(2048, 2048, TextureUnit.Texture2);

            IncrementComponentIndex();
            _world.LightComps[_compIdx] = new ComponentLight(LightType.SPOTLIGHT,
                                                        new SpotLight(new Color4(0.0f, 1.0f, 0.0f, 1.0f),
                                                                      new Vector3(0.0f, 5.0f, 6.0f * 1.118f), new Vector3(0.0f), new Vector3(0.0f, 1.0f, 0.0f),
                                                                      0.3f, 1.0f, 5.7f, 11.0f));

            _world.LightComps[_compIdx].LightObject.BuildUBO(5, 8);
            _world.LightComps[_compIdx].LightObject.ShadowMap = new ShadowMap(2048, 2048, TextureUnit.Texture3);

            IncrementComponentIndex();
            _world.LightComps[_compIdx] = new ComponentLight(LightType.SUN,
                                                        new SunLamp(Color4.White,
                                                                    new Vector3(0.0f, 5.0f, 7.0f), new Vector3(0.0f), new Vector3(0.0f, 1.0f, 0.0f),
                                                                    7.0f, 7.0f, 1.0f, 11.0f));

            _world.LightComps[_compIdx].LightObject.BuildUBO(7, 8);
            _world.LightComps[_compIdx].LightObject.ShadowMap = new ShadowMap(2048, 2048, TextureUnit.Texture5);

            IncrementComponentIndex();
            _world.CameraComps[_compIdx] = new ComponentCamera(new Camera(CameraMode.ORTHOGRAPHIC, 0.5f, 0.5f, -1.0f, 2.0f));
            _world.CameraComps[_compIdx].Camera.BuildUBO(11, 7);

        }

        /// <summary>
        /// Increases the current component index by 1 and returns the value.
        /// </summary>
        /// <returns></returns>
        public int NextComponentIndex()
        {
            return ++_compIdx;
        }

        /// <summary>
        /// Increases the current component index by 1, but does not return the value.
        /// </summary>
        public void IncrementComponentIndex()
        {
            _compIdx++;
        }

        /// <summary>
        /// Performs and update call on the world object associated with Game.
        /// </summary>
        /// <param name="time">Previous time step.</param>
        public void Update(float time)
        {
            _world.Update(time);
            _systemTransform.Update(time);
            //Console.WriteLine(GL.GetError().ToString());
        }

        public void Render()
        {
            _systemRender.Render(ref _systemTransform);
            _systemRender.RenderUI();
        }
    }
}
