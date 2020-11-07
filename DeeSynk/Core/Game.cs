using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DeeSynk.Core.Algorithms;
using DeeSynk.Core.Components;
using DeeSynk.Core.Components.GraphicsObjects.Lights;
using DeeSynk.Core.Components.GraphicsObjects.Shadows;
using DeeSynk.Core.Components.Input;
using DeeSynk.Core.Components.Models.Templates;
using DeeSynk.Core.Components.Models.Templates.UI;
using DeeSynk.Core.Components.Types.Render;
using DeeSynk.Core.Components.Types.Transform;
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
        private bool _init;
        public bool Init { get => _init; }
        private World _world;
        public World World { get => _world; }

        private UI _ui;
        public UI UI {get => _ui;}

        //This is used for loading objects into world.  Since each object has its own unique index, we only need a
        //global index counter that works for all objects.  (i.e. two seperate objects cannot both have index 4)
        private int _compIdx;

        private Camera _camera;

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

        public Game(ref Camera camera)
        {
            _camera = camera;
            Load();
        }

        /// <summary>
        /// Responsible for loading all resources that will be used within the game and engine.
        /// </summary>
        public void Load()
        {
            var im = InputManager.GetInstance();
            im.Configurations.Add("primary move", new InputConfiguration());
            im.Configurations.Add("unlocked mouse", new InputConfiguration());

            _world = new World(8);
            _ui = new UI(32);

            _compIdx = 0;

            _systemInput = new SystemInput(ref _world, ref _ui, ref _camera);
            _systemRender = new SystemRender(ref _world, ref _ui);
            _systemModel = new SystemModel(ref _world, ref _ui);
            _systemTransform = new SystemTransform(ref _world, ref _ui);
            _systemVAO = new SystemVAO(ref _world, ref _ui);
            _systemUI = new SystemUI(ref _world, ref _ui);
        }

        public void PushCameraRef(ref Camera camera)
        {
            _systemTransform.PushCameraRef(ref camera);
            _systemRender.PushCameraRef(ref camera);
            _systemInput.PushCameraRef(ref camera);
        }

        public void LoadGameData()
        {
            CreateComponents();
            InjectWorldData();
            InjectUIData();
            _init = true;
            _systemInput.StartThreads();
        }

        public void CreateComponents()
        {
            _world.InitData();

            //Create the objects in the world array of GameObjects - these objects are blank and only hold the ComponentMask

            //=====WORLD=====//
            _world.CreateGameObject(Component.RENDER | Component.MODEL_STATIC | Component.TRANSFORM | Component.MATERIAL);
            //_world.CreateGameObject(Component.RENDER | Component.MODEL_STATIC | Component.TRANSFORM); //Component.MATERIAL
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
            _ui.CreateGameObject(Component.UI_STANDARD);
            _ui.CreateGameObject(Component.UI_STANDARD);
            _ui.CreateGameObject(Component.UI_STANDARD);

            /*for (int idx = 8; idx < 1192; idx++)
            {
                _world.CreateGameObject(Component.RENDER | Component.MODEL_STATIC | Component.TRANSFORM);
            }*/
        }

        private void InjectWorldData()
        {
            //Essentially, for each object we initialize its unique characteristics
            //TODO add the ability to load in these objects from a file to make the process act more how it would in a real world scenario

            //the models are added
            {
                float s = 0.25f;
                _world.StaticModelComps[0] = new ComponentModelStatic(ModelProperties.VERTICES_NORMALS_ELEMENTS, "TestCube");
                _world.MaterialComps[0] = new ComponentMaterial(Color4.White);  //input
                var tComps = TransformComponents.TRANSLATION | TransformComponents.SCALE;
                _world.TransComps[0] = new ComponentTransform(tComps, true, locY: 0.29f, sclX: s, sclY: s, sclZ: s);  //input

                Texture t = TextureManager.GetInstance().GetTexture("wood");  //input

                var v14 = new Vector3(1 / 20f * t.AspectRatio, 0f, 1 / 20f);
                //var v14 = new Vector3(0);
                _world.StaticModelComps[1] = new ComponentModelStatic(ModelProperties.VERTICES_UVS_ELEMENTS, ModelTemplates.PlaneXZ);  //input
                _world.StaticModelComps[1].TemplateData = new Plane(_world.StaticModelComps[1].ModelProperties, new Vector2(100.0f, 100.0f), t.SubTextureLocations[0].UVOffset, t.SubTextureLocations[0].UVScale); //kinda input
                tComps = TransformComponents.TRANSLATION | TransformComponents.SCALE;
                _world.TransComps[1] = new ComponentTransform(tComps, true, sclX: v14.X, sclY: v14.Y, sclZ: v14.Z);  //input

                _world.StaticModelComps[2] = new ComponentModelStatic(ModelProperties.VERTICES_UVS_ELEMENTS, ModelTemplates.PlaneXY); //input
                _world.StaticModelComps[2].TemplateData = new Plane(_world.StaticModelComps[1].ModelProperties, new Vector2(1.0f, 1.0f), new Vector2(0.0f, 0.0f), new Vector2(1.0f, 1.0f)); //kinda input
                tComps = TransformComponents.TRANSLATION;
                _world.TransComps[2] = new ComponentTransform(tComps, true, locX: -0.5f, locY: -0.5f, locZ: -1.0f); //input

                //int total = 0;
                //int totalElements = ModelManager.GetInstance().GetModel("1").ElementCount;
                /*
                for(int idx = 8; idx < 1192; idx++)
                {
                    _world.StaticModelComps[idx] = new ComponentModelStatic(ModelProperties.VERTICES_NORMALS_COLORS_ELEMENTS, (idx - 6).ToString());
                    tComps = TransformComponents.TRANSLATION | TransformComponents.SCALE;
                    _world.TransComps[idx] = new ComponentTransform(tComps, true, locY: 0.0f, sclX: s, sclY: s, sclZ: s);

                    total += ModelManager.GetInstance().GetModel((idx - 6).ToString()).ElementCount;
                    totalElements += ModelManager.GetInstance().GetModel((idx - 6).ToString()).ElementCount;
                }*/
            }
            _systemModel.LinkModels(_world);
            //_systemTransform.CreateComponents(_world);
            //_world.TransComps[0].RotationXComp.InterpolateRotation(10.0f, 15, InterpolationMode.LINEAR);

            {
                _world.TextureComps[1] = new ComponentTexture(TextureManager.GetInstance().GetTexture("wood"), 0);

                var sm = ShaderManager.GetInstance();

                _systemVAO.InitVAORange(Buffers.VERTICES_NORMALS_ELEMENTS | Buffers.INTERLEAVED, _compIdx, _compIdx, _world);
                _world.RenderComps[_compIdx].PROGRAM_ID = sm.GetProgram("coloredPhongShaded2");  //kinda input
                _world.RenderComps[_compIdx].ValidateData();

                _compIdx = _world.NextComponentIndex();
                _systemVAO.InitVAORange(Buffers.VERTICES | Buffers.UVS | Buffers.FACE_ELEMENTS | Buffers.INTERLEAVED, _compIdx, _compIdx, _world);
                _world.RenderComps[_compIdx].PROGRAM_ID = sm.GetProgram("shadowTextured2");  //kinda input
                _world.RenderComps[_compIdx].ValidateData();

                _compIdx = _world.NextComponentIndex();
                _systemVAO.InitVAORange(Buffers.VERTICES | Buffers.UVS | Buffers.FACE_ELEMENTS | Buffers.INTERLEAVED, _compIdx, _compIdx, _world);
                _world.RenderComps[_compIdx].PROGRAM_ID = sm.GetProgram("postLightGlare");  //kinda input
                _world.RenderComps[_compIdx].IsFinalRenderPlane = true;  //kinda
                _world.RenderComps[_compIdx].ValidateData();

                /*
                int idx = 8;
                _systemVAO.InitVAORange(Buffers.VERTICES_NORMALS_ELEMENTS | Buffers.COLORS | Buffers.INTERLEAVED, idx, 1191, _world);
                _world.RenderComps[idx].PROGRAM_ID = sm.GetProgram("defaultColored");  //kinda input
                _world.RenderComps[idx].ValidateData();*/
                /*for (int idx = 8; idx < 1192; idx++)
                {
                    _systemVAO.InitVAORange(Buffers.VERTICES_NORMALS_ELEMENTS | Buffers.COLORS | Buffers.INTERLEAVED, idx, idx, _world);
                    _world.RenderComps[idx].PROGRAM_ID = sm.GetProgram("defaultColored");  //kinda input
                    _world.RenderComps[idx].ValidateData();
                }*/
            }
            //Automated UBO managment is a MUST
            {
                _compIdx = _world.NextComponentIndex();
                _world.LightComps[_compIdx] = new ComponentLight(LightType.SPOTLIGHT,
                                                            new SpotLight(Color4.Red,
                                                                          new Vector3(-3.0f, 5.0f, 6.0f), new Vector3(0.0f), new Vector3(0.0f, 1.0f, 0.0f),
                                                                          0.3f, 1.0f, 5f, 11f));

                _world.LightComps[_compIdx].LightObject.BuildUBO(3, 8);
                _world.LightComps[_compIdx].LightObject.ShadowMap = new ShadowMap(8192, 8192, TextureUnit.Texture1);

                _compIdx = _world.NextComponentIndex();
                _world.LightComps[_compIdx] = new ComponentLight(LightType.SPOTLIGHT,
                                                            new SpotLight(Color4.Blue,
                                                                          new Vector3(3.0f, 5.0f, 6.0f), new Vector3(0.0f), new Vector3(0.0f, 1.0f, 0.0f),
                                                                          0.3f, 1.0f, 5f, 11f));

                _world.LightComps[_compIdx].LightObject.BuildUBO(4, 8);
                _world.LightComps[_compIdx].LightObject.ShadowMap = new ShadowMap(8192, 8192, TextureUnit.Texture2);

                _compIdx = _world.NextComponentIndex();
                _world.LightComps[_compIdx] = new ComponentLight(LightType.SPOTLIGHT,
                                                            new SpotLight(new Color4(0.0f, 1.0f, 0.0f, 1.0f),
                                                                          new Vector3(0.0f, 5.0f, 6.0f * 1.118f), new Vector3(0.0f), new Vector3(0.0f, 1.0f, 0.0f),
                                                                          0.3f, 1.0f, 5.7f, 11.0f));

                _world.LightComps[_compIdx].LightObject.BuildUBO(5, 8);
                _world.LightComps[_compIdx].LightObject.ShadowMap = new ShadowMap(8192, 8192, TextureUnit.Texture3);

                _compIdx = _world.NextComponentIndex();
                _world.LightComps[_compIdx] = new ComponentLight(LightType.SUN,
                                                            new SunLamp(Color4.White,
                                                                        new Vector3(0.0f, 5.0f, 7.0f), new Vector3(0.0f), new Vector3(0.0f, 1.0f, 0.0f),
                                                                        7.0f, 7.0f, 1.0f, 11.0f));

                _world.LightComps[_compIdx].LightObject.BuildUBO(7, 8);
                _world.LightComps[_compIdx].LightObject.ShadowMap = new ShadowMap(8192, 8192, TextureUnit.Texture5);

                _compIdx = _world.NextComponentIndex();
                _world.CameraComps[_compIdx] = new ComponentCamera(new Camera(CameraMode.ORTHOGRAPHIC, 0.5f, 0.5f, -1.0f, 2.0f));
                _world.CameraComps[_compIdx].Camera.BuildUBO(11, 7);
            }
        }

        private void InjectUIData()
        {

            var sm = ShaderManager.GetInstance();

            _compIdx = _ui.CompIdx; //MainWindow.width/1.0f, MainWindow.height/1.0f
            var uiCamera = new Camera(CameraMode.ORTHOGRAPHIC, MainWindow.width, MainWindow.height, -1.0f, 2.0f);
            uiCamera.OverrideLookAtVector = true;
            var offset = new Vector3(MainWindow.width / 2, MainWindow.height / 2, 0.0f);
            uiCamera.AddLocation(offset);
            uiCamera.UpdateMatrices();
            _world.CameraComps[_compIdx] = new ComponentCamera(uiCamera);
            _world.CameraComps[_compIdx].Camera.BuildUBO(15, 7);

            _compIdx = _ui.NextComponentIndex();
            UICanvas activeCanvas = new UICanvas(16, MainWindow.width, MainWindow.height, _compIdx);
            _ui.CanvasComps[_compIdx] = new ComponentCanvas(activeCanvas);


            {
                //MAIN CONTAINER
                UIElementContainer element;
                _compIdx = _ui.NextComponentIndex(); //2558, 1438
                {
                    int w = MainWindow.width / 5 - 10; //input
                    int h = MainWindow.height - 10;  //input
                    //var s1 = new Vector2(-MainWindow.width / 2 + 5, -MainWindow.height / 2 + 5);  //input
                    var s1 = new Vector2(5, 5);
                    element = new UIElementContainer(4, UIElementType.UI_CONTAINER, w, h, s1, PositionType.GLOBAL, PositionReference.CORNER_BOTTOM_LEFT, 0, _compIdx);
                    activeCanvas.AddChild(element);

                    _ui.ElementComps[_compIdx] = new ComponentElement(element);

                    var v30 = new Vector3(element.Position.X, element.Position.Y, -1.0f);
                    _ui.StaticModelComps[_compIdx] = new ComponentModelStatic(ModelProperties.VERTICES_COLORS_ELEMENTS, ModelTemplates.UIContainer);

                    var s2 = new Vector2(element.Width, element.Height); //input
                    float b = 4; //input
                    var c1 = new Color4(120, 120, 120, 128);  //input
                    var c2 = new Color4(32, 32, 32, 128);  //input
                    _ui.StaticModelComps[_compIdx].TemplateData = new BorderedWindow(_ui.StaticModelComps[_compIdx].ModelProperties, _ui.ElementComps[_compIdx].Element.Reference, s2, b, c1, c2);

                    TransformComponents tComps = TransformComponents.TRANSLATION;
                    _ui.TransComps[_compIdx] = new ComponentTransform(tComps, true, locX: v30.X, locY: v30.Y, locZ: v30.Z);

                    _ui.RenderComps[_compIdx] = new ComponentRender(Buffers.VERTICES | Buffers.COLORS | Buffers.FACE_ELEMENTS | Buffers.INTERLEAVED);  //kinda input, can be widdled down though
                    _ui.RenderComps[_compIdx].PROGRAM_ID = sm.GetProgram("testUI");  //kinda input
                }

                //CHILD CONTAINER
                _compIdx = _ui.NextComponentIndex();
                UIElementContainer cElem;
                {
                    int w = element.Width - 20;
                    int h = element.Height / 5 - 20;
                    var s1 = new Vector2(10, 4 * element.Height / 5 + 10);
                    cElem = new UIElementContainer(4, UIElementType.UI_CONTAINER, w, h, s1, PositionType.LOCAL, PositionReference.CORNER_BOTTOM_LEFT, 1, _compIdx);
                    element.AddChild(cElem);

                    _ui.ElementComps[_compIdx] = new ComponentElement(cElem);

                    var v30 = new Vector3(element.Position.X + cElem.Position.X, element.Position.Y + cElem.Position.Y, -1.0f);
                    _ui.StaticModelComps[_compIdx] = new ComponentModelStatic(ModelProperties.VERTICES_COLORS_ELEMENTS, ModelTemplates.UIContainer);

                    var s2 = new Vector2(cElem.Width, cElem.Height); //container size
                    float b = 4; //border
                    var c1 = new Color4(120, 120, 120, 128);
                    var c2 = new Color4(32, 32, 32, 128);
                    _ui.StaticModelComps[_compIdx].TemplateData = new BorderedWindow(_ui.StaticModelComps[_compIdx].ModelProperties, _ui.ElementComps[_compIdx].Element.Reference, s2, b, c1, c2);

                    TransformComponents tComps = TransformComponents.TRANSLATION;
                    _ui.TransComps[_compIdx] = new ComponentTransform(tComps, true, locX: v30.X, locY: v30.Y, locZ: v30.Z);

                    _ui.RenderComps[_compIdx] = new ComponentRender(Buffers.VERTICES | Buffers.COLORS | Buffers.FACE_ELEMENTS | Buffers.INTERLEAVED);
                    _ui.RenderComps[_compIdx].PROGRAM_ID = sm.GetProgram("testUI");
                }

                //CHILD OF CHILD CONTAINER
                _compIdx = _ui.NextComponentIndex();
                UIElementContainer ccElem;
                {
                    int w = cElem.Width - 20;
                    int h = cElem.Height - 20;
                    var s1 = new Vector2(cElem.Width / 2, cElem.Height / 2);
                    ccElem = new UIElementContainer(4, UIElementType.UI_CONTAINER, w, h, s1, PositionType.LOCAL, PositionReference.CENTER, 2, _compIdx);
                    cElem.AddChild(ccElem);

                    _ui.ElementComps[_compIdx] = new ComponentElement(ccElem);

                    var v30 = new Vector3(element.Position.X + cElem.Position.X + ccElem.Position.X, element.Position.Y + cElem.Position.Y + ccElem.Position.Y, -1.0f);
                    _ui.StaticModelComps[_compIdx] = new ComponentModelStatic(ModelProperties.VERTICES_COLORS_ELEMENTS, ModelTemplates.UIContainer);

                    var s2 = new Vector2(ccElem.Width, ccElem.Height); //container size
                    float b = 4; //border
                    var c1 = new Color4(120, 120, 120, 128);
                    var c2 = new Color4(32, 32, 32, 128);
                    _ui.StaticModelComps[_compIdx].TemplateData = new BorderedWindow(_ui.StaticModelComps[_compIdx].ModelProperties, _ui.ElementComps[_compIdx].Element.Reference, s2, b, c1, c2);

                    TransformComponents tComps = TransformComponents.TRANSLATION;
                    _ui.TransComps[_compIdx] = new ComponentTransform(tComps, true, locX: v30.X, locY: v30.Y, locZ: v30.Z);

                    _ui.RenderComps[_compIdx] = new ComponentRender(Buffers.VERTICES | Buffers.COLORS | Buffers.FACE_ELEMENTS | Buffers.INTERLEAVED);
                    _ui.RenderComps[_compIdx].PROGRAM_ID = sm.GetProgram("testUI");
                }
            }

            _systemModel.LinkModels(_ui);
            _systemTransform.UpdateMonitoredGameObjects();
            _systemUI.UpdateMonitoredGameObjects();

            for(int idx = 0; idx < _ui.ObjectMemory; idx++)
            {
                if (_ui.GameObjects[idx].Components.HasFlag(SystemRender.RenderQualfier))
                {
                    _systemVAO.InitVAO(_ui, idx);
                    _ui.RenderComps[idx].ValidateData();
                }
            }
        }

        /// <summary>
        /// Performs and update call on the world object associated with Game.
        /// </summary>
        /// <param name="time">Previous time step.</param>
        public void Update(float time)
        {
            _systemInput.Update(time);
            //_systemUI.MoveElementBy(2, new Vector2(100.0f, 0.0f) * time);
            //_systemUI.MoveElementBy(3, new Vector2(0.0f, -100.0f) * time);
            _world.Update(time);
            _ui.Update(time);
            _systemTransform.Update(time);
            _systemUI.Update(time);

            //Console.WriteLine(GL.GetError().ToString());
        }

        public void Render()
        {
            _systemRender.Render(ref _systemTransform);
        }
    }
}
