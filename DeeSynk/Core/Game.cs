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
using DeeSynk.Core.Components.Models.Templates;
using DeeSynk.Core.Components.Types.Render;
using DeeSynk.Core.Components.Types.UI;
using DeeSynk.Core.Components.UI;
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
        public World World { get => _world; }

        private UI _ui;
        public UI UI {get => _ui;}

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
        private SystemUI _systemUI;
        public SystemUI SystemUI { get => _systemUI; }
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

            _world = new World(8);
            _ui = new UI(32);

            _compIdx = 0;

            _systemInput = new SystemInput();

            _systemRender = new SystemRender(_world, _ui);
            _systemModel = new SystemModel(_world, _ui);
            _systemTransform = new SystemTransform(_world, _ui);
            _systemVAO = new SystemVAO(_world, _ui);
            _systemUI = new SystemUI(_world, _ui);
        }

        public void PushCameraRef(ref Camera camera)
        {
            _systemTransform.PushCameraRef(ref camera);
            _systemRender.PushCameraRef(ref camera);
        }

        public void LoadGameData()
        {
            CreateComponents();
            InjectWorldData();
            InjectUIData();
        }

        public void CreateComponents()
        {
            _world.InitData();

            //Create the objects in the world array of GameObjects - these objects are blank and only hold the ComponentMask

            //=====WORLD=====//
            _world.CreateGameObject(Component.RENDER | Component.MODEL_STATIC | Component.TRANSFORM);
            _world.CreateGameObject(Component.RENDER | Component.MODEL_STATIC | Component.TRANSFORM | Component.TEXTURE);
            _world.CreateGameObject(Component.RENDER | Component.MODEL_STATIC | Component.TRANSFORM | Component.TEXTURE);

            _world.CreateGameObject(Component.LIGHT);
            _world.CreateGameObject(Component.LIGHT);
            _world.CreateGameObject(Component.LIGHT);

            _world.CreateGameObject(Component.LIGHT);

            _world.CreateGameObject(Component.CAMERA);

            //=====UI=====//
            _ui.CreateGameObject(Component.CAMERA);

            _ui.CreateGameObject(Component.UI_CANVAS);
            _ui.CreateGameObject(Component.UI_STANDARD); //| Component.TEXTURE

            //_systemModel.UpdateMonitoredGameObjects();
            //_systemModel.InitModel();
            //_systemTransform.UpdateMonitoredGameObjects();
            //_systemTransform.InitLocation();
        }

        private void InjectWorldData()
        {
            //Essentially, for each object we initialize its unique characteristics
            //TODO add the ability to load in these objects from a file to make the process act more how it would in a real world scenario

            //the models are added
            {
                var v00 = new Vector3(0, 8, 0);
                var v01 = new Vector3(5f, 5f, 5f);
                var v02 = new Vector2(0.0f, 0.0f);
                var v03 = new Vector2(1.0f, 1.0f);
                var v04 = Color4.White;


                _world.StaticModelComps[0] = new ComponentModelStatic(ModelProperties.VERTICES_NORMALS_COLORS_ELEMENTS, ModelReferenceType.DISCRETE, "TestCube",
                                            ConstructionFlags.VECTOR3_OFFSET | ConstructionFlags.FLOAT_ROTATION_X | ConstructionFlags.COLOR4_COLOR | ConstructionFlags.VECTOR3_SCALE,
                                            new Vector3(0, 0.29f, 0), (float)(0), new Vector3(0.25f, 0.25f, 0.25f), v04);

                Texture t = TextureManager.GetInstance().GetTexture("wood");
                float width = t.Width;
                float height = t.Height;

                var v10 = new Vector3(0);
                var v14 = new Vector3(1 / 20f * t.AspectRatio, 0f, 1 / 20f);
                var v11 = new Vector3(100f, 0f, 100f);  //100
                var v12 = t.SubTextureLocations[0].UVOffset;
                var v13 = t.SubTextureLocations[0].UVScale;
                _world.StaticModelComps[1] = new ComponentModelStatic(ModelProperties.VERTICES_UVS_ELEMENTS, ModelReferenceType.TEMPLATE, ModelTemplates.PlaneXZ,
                                                                ConstructionFlags.VECTOR3_OFFSET | ConstructionFlags.FLOAT_ROTATION_X | ConstructionFlags.VECTOR3_SCALE |
                                                                ConstructionFlags.VECTOR3_DIMENSIONS |
                                                                ConstructionFlags.VECTOR2_UV_OFFSET | ConstructionFlags.VECTOR2_UV_SCALE,
                                                                v10, 0.0f, v14, v11, v12, v13);

                var v20 = new Vector3(-0.5f, -0.5f, -1.0f);
                var v21 = new Vector3(1.0f, 1.0f, 0.0f);
                var v22 = new Vector2(0.0f, 0.0f);
                var v23 = new Vector2(1.0f, 1.0f);
                _world.StaticModelComps[2] = new ComponentModelStatic(ModelProperties.VERTICES_UVS_ELEMENTS, ModelReferenceType.TEMPLATE, ModelTemplates.PlaneXY,
                                                                ConstructionFlags.VECTOR3_OFFSET | ConstructionFlags.VECTOR3_DIMENSIONS | ConstructionFlags.VECTOR2_UV_OFFSET | ConstructionFlags.VECTOR2_UV_SCALE,
                                                                v20, v21, v22, v23);
            }
            _systemModel.LinkModels(_world);
            _systemTransform.CreateComponents(_world);

            {
                _world.TextureComps[1] = new ComponentTexture(TextureManager.GetInstance().GetTexture("wood"), 0);

                var sm = ShaderManager.GetInstance();

                _systemVAO.InitVAORange(Buffers.VERTICES_NORMALS_ELEMENTS | Buffers.INTERLEAVED, _compIdx, _compIdx, _world);
                _world.RenderComps[_compIdx].PROGRAM_ID = sm.GetProgram("coloredPhongShaded");
                _world.RenderComps[_compIdx].ValidateData();

                _compIdx = _world.NextComponentIndex();
                _systemVAO.InitVAORange(Buffers.VERTICES | Buffers.UVS | Buffers.FACE_ELEMENTS | Buffers.INTERLEAVED, _compIdx, _compIdx, _world);
                _world.RenderComps[_compIdx].PROGRAM_ID = sm.GetProgram("shadowTextured2");
                _world.RenderComps[_compIdx].ValidateData();

                _compIdx = _world.NextComponentIndex();
                _systemVAO.InitVAORange(Buffers.VERTICES | Buffers.UVS | Buffers.FACE_ELEMENTS | Buffers.INTERLEAVED, _compIdx, _compIdx, _world);
                _world.RenderComps[_compIdx].PROGRAM_ID = sm.GetProgram("postLightGlare");
                _world.RenderComps[_compIdx].IsFinalRenderPlane = true;
                _world.RenderComps[_compIdx].ValidateData();
            }
            //Automated UBO managment is a MUST
            {
                _compIdx = _world.NextComponentIndex();
                _world.LightComps[_compIdx] = new ComponentLight(LightType.SPOTLIGHT,
                                                            new SpotLight(Color4.Red,
                                                                          new Vector3(-3.0f, 5.0f, 6.0f), new Vector3(0.0f), new Vector3(0.0f, 1.0f, 0.0f),
                                                                          0.3f, 1.0f, 5f, 11f));

                _world.LightComps[_compIdx].LightObject.BuildUBO(3, 8);
                _world.LightComps[_compIdx].LightObject.ShadowMap = new ShadowMap(2048, 2048, TextureUnit.Texture1);

                _compIdx = _world.NextComponentIndex();
                _world.LightComps[_compIdx] = new ComponentLight(LightType.SPOTLIGHT,
                                                            new SpotLight(Color4.Blue,
                                                                          new Vector3(3.0f, 5.0f, 6.0f), new Vector3(0.0f), new Vector3(0.0f, 1.0f, 0.0f),
                                                                          0.3f, 1.0f, 5f, 11f));

                _world.LightComps[_compIdx].LightObject.BuildUBO(4, 8);
                _world.LightComps[_compIdx].LightObject.ShadowMap = new ShadowMap(2048, 2048, TextureUnit.Texture2);

                _compIdx = _world.NextComponentIndex();
                _world.LightComps[_compIdx] = new ComponentLight(LightType.SPOTLIGHT,
                                                            new SpotLight(new Color4(0.0f, 1.0f, 0.0f, 1.0f),
                                                                          new Vector3(0.0f, 5.0f, 6.0f * 1.118f), new Vector3(0.0f), new Vector3(0.0f, 1.0f, 0.0f),
                                                                          0.3f, 1.0f, 5.7f, 11.0f));

                _world.LightComps[_compIdx].LightObject.BuildUBO(5, 8);
                _world.LightComps[_compIdx].LightObject.ShadowMap = new ShadowMap(2048, 2048, TextureUnit.Texture3);

                _compIdx = _world.NextComponentIndex();
                _world.LightComps[_compIdx] = new ComponentLight(LightType.SUN,
                                                            new SunLamp(Color4.White,
                                                                        new Vector3(0.0f, 5.0f, 7.0f), new Vector3(0.0f), new Vector3(0.0f, 1.0f, 0.0f),
                                                                        7.0f, 7.0f, 1.0f, 11.0f));

                _world.LightComps[_compIdx].LightObject.BuildUBO(7, 8);
                _world.LightComps[_compIdx].LightObject.ShadowMap = new ShadowMap(2048, 2048, TextureUnit.Texture5);

                _compIdx = _world.NextComponentIndex();
                _world.CameraComps[_compIdx] = new ComponentCamera(new Camera(CameraMode.ORTHOGRAPHIC, 0.5f, 0.5f, -1.0f, 2.0f));
                _world.CameraComps[_compIdx].Camera.BuildUBO(11, 7);
            }
        }

        private void InjectUIData()
        {

            var sm = ShaderManager.GetInstance();

            _compIdx = _ui.CompIdx;
            var uiCamera = new Camera(CameraMode.ORTHOGRAPHIC, MainWindow.width/2.0f, MainWindow.height/2.0f, -1.0f, 2.0f);
            _world.CameraComps[_compIdx] = new ComponentCamera(uiCamera);
            _world.CameraComps[_compIdx].Camera.BuildUBO(15, 7);

            _compIdx = _ui.NextComponentIndex();
            UICanvas activeCanvas = new UICanvas(16, MainWindow.width, MainWindow.height, _compIdx);
            _ui.CanvasComps[_compIdx] = new ComponentCanvas(activeCanvas);


            _compIdx = _ui.NextComponentIndex();
            UIElementContainer element = new UIElementContainer(4, UIElementType.UI_CONTAINER, 100, 100, new Vector2(200, 200), UIPositionType.GLOBAL, 0, _compIdx, activeCanvas.GlobalID, "");
            activeCanvas.AddChild(element);

            _ui.ElementComps[_compIdx] = new ComponentElement(element);
            {
                var v30 = new Vector3(-0.5f + element.Position.X, -0.5f + element.Position.Y, -1.0f);
                var v31 = new Vector3(element.Width, element.Height, 0.0f);
                _ui.StaticModelComps[2] = new ComponentModelStatic(ModelProperties.VERTICES_COLORS_ELEMENTS, ModelReferenceType.TEMPLATE, ModelTemplates.PlaneXY,
                                                                ConstructionFlags.VECTOR3_OFFSET | ConstructionFlags.VECTOR3_DIMENSIONS | ConstructionFlags.COLOR4_COLOR,
                                                                v30, v31, new Color4(255, 0, 255, 255));
                //new Vector2(0.0f, 0.0f), new Vector2(1.0f, 1.0f)
            }

            _systemModel.LinkModels(_ui);
            _systemTransform.CreateComponents(_ui);
            _systemVAO.InitVAORange(Buffers.VERTICES | Buffers.FACE_ELEMENTS | Buffers.INTERLEAVED, _compIdx, _compIdx, _ui);
            _ui.RenderComps[_compIdx].PROGRAM_ID = sm.GetProgram("testUI");
            _ui.RenderComps[_compIdx].ValidateData();
            //_ui.TextureComps[_compIdx] = new ComponentTexture(TextureManager.GetInstance().GetTexture("wood"), 0);

            //SystemUI.AddElementToCanvas(Canvas c, Element e)  => returns Element e
            //or
            //Canvas.AddElement(Element e)  adds a child element to the canvas  :: Note this is only objects on the next layer
            //Canvas.AddElementID(e.ID)  or Canvas.AddElementID(e)  store e.ID  :: Simply stores the ID of a child element anywhere on the tree
            //Element.AddElement(Element e)  adds a child element to the specified element  (Element.AddChild)
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
        }
    }
}
