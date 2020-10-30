using DeeSynk.Core.Algorithms;
using DeeSynk.Core.Components;
using DeeSynk.Core.Components.Models;
using DeeSynk.Core.Components.Types.Render;
using DeeSynk.Core.Managers;
using OpenTK;
using OpenTK.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeeSynk.Core.Systems
{
    using ModelTemplates = DeeSynk.Core.Components.Models.Templates.ModelTemplates;
    public class SystemModel : ISystem
    {
        public Component MonitoredComponents => Component.MODEL_STATIC;

        private World _world;
        private UI _ui;

        private bool[] _monitoredGameObjects_W;
        private bool[] _monitoredGameObjects_U;

        private ComponentModelStatic[] _staticModelComps_W;
        private ComponentModelStatic[] _staticModelComps_U;

        public SystemModel(World world, UI ui)
        {
            _world = world;
            _ui = ui;

            _monitoredGameObjects_W = new bool[_world.ObjectMemory];
            _monitoredGameObjects_U = new bool[_ui.ObjectMemory];

            _staticModelComps_W = _world.StaticModelComps;
            _staticModelComps_U = _ui.StaticModelComps;
        }

        public void UpdateMonitoredGameObjects()
        {
            UpdateMonitoredGameObjects(_world, _monitoredGameObjects_W);
            UpdateMonitoredGameObjects(_ui, _monitoredGameObjects_U);
        }

        private void UpdateMonitoredGameObjects(GameObjectContainer c, bool[] monitor)
        {
            for (int i = 0; i < c.ObjectMemory; i++)
            {
                if (c.ExistingGameObjects[i])
                {
                    if (c.GameObjects[i].Components.HasFlag(MonitoredComponents))
                    {
                        monitor[i] = true;
                    }
                }
            }
        }

        //TEST START
        public void InitModel()
        {
            CreateModels();
            LinkModels(_world, _monitoredGameObjects_W);
            LinkModels(_ui, _monitoredGameObjects_U);
        }

        /// <summary>
        /// Creates a ComponentModelStatic for each model that is to be registered.
        /// </summary>
        private void CreateModels()
        {
            var v00 = new Vector3(0, 8, 0);
            var v01 = new Vector3(5f, 5f, 5f);
            var v02 = new Vector2(0.0f, 0.0f);
            var v03 = new Vector2(1.0f, 1.0f);
            var v04 = Color4.White;

            Random r = new Random();


            _staticModelComps_W[0] = new ComponentModelStatic(ModelProperties.VERTICES_NORMALS_COLORS_ELEMENTS, ModelReferenceType.DISCRETE, "TestCube",
                                        ConstructionFlags.VECTOR3_OFFSET | ConstructionFlags.FLOAT_ROTATION_X | ConstructionFlags.COLOR4_COLOR | ConstructionFlags.VECTOR3_SCALE,
                                        new Vector3(0, 0.29f, 0), (float)(0),new Vector3(0.25f, 0.25f, 0.25f), v04);

            Texture t = TextureManager.GetInstance().GetTexture("wood");
            float width = t.Width;
            float height = t.Height;

            var v10 = new Vector3(0);
            var v14 = new Vector3(1 / 20f * t.AspectRatio, 0f, 1 / 20f);
            var v11 = new Vector3(100f, 0f, 100f);  //100
            var v12 = t.SubTextureLocations[0].UVOffset;
            var v13 = t.SubTextureLocations[0].UVScale;
            _staticModelComps_W[1] = new ComponentModelStatic(ModelProperties.VERTICES_UVS_ELEMENTS, ModelReferenceType.TEMPLATE, ModelTemplates.PlaneXZ,
                                                            ConstructionFlags.VECTOR3_OFFSET | ConstructionFlags.FLOAT_ROTATION_X | ConstructionFlags.VECTOR3_SCALE |
                                                            ConstructionFlags.VECTOR3_DIMENSIONS |
                                                            ConstructionFlags.VECTOR2_UV_OFFSET | ConstructionFlags.VECTOR2_UV_SCALE,
                                                            v10, 0.0f, v14, v11, v12, v13);

            var v20 = new Vector3(-0.5f, -0.5f, -1.0f);
            var v21 = new Vector3(1.0f, 1.0f, 0.0f);
            var v22 = new Vector2(0.0f, 0.0f);
            var v23 = new Vector2(1.0f, 1.0f);
            _staticModelComps_W[2] = new ComponentModelStatic(ModelProperties.VERTICES_UVS_ELEMENTS, ModelReferenceType.TEMPLATE, ModelTemplates.PlaneXY,
                                                            ConstructionFlags.VECTOR3_OFFSET | ConstructionFlags.VECTOR3_DIMENSIONS | ConstructionFlags.VECTOR2_UV_OFFSET | ConstructionFlags.VECTOR2_UV_SCALE,
                                                            v20, v21, v22, v23);

            //===UI===//

            var v30 = new Vector3(525.0f, 0.0f, -1.0f);
            var v31 = new Vector3(400.0f, 1400.0f, 0.0f);
            var v32 = v12;
            var v33 = v13;
            _staticModelComps_U[2] = new ComponentModelStatic(ModelProperties.VERTICES_UVS_ELEMENTS, ModelReferenceType.TEMPLATE, ModelTemplates.PlaneXY,
                                                            ConstructionFlags.VECTOR3_OFFSET | ConstructionFlags.VECTOR3_DIMENSIONS | ConstructionFlags.VECTOR2_UV_OFFSET | ConstructionFlags.VECTOR2_UV_SCALE,
                                                            v30, v31, v32, v33);
        }

        /// <summary>
        /// Links models from ModelManager, either prexisting or registered from template, to each ComponentModelStatic based on the specifications stored in CreateModels.
        /// </summary>
        private void LinkModels(GameObjectContainer c, bool[] monitor)
        {
            var modelManager = ModelManager.GetInstance();
            for (int idx = 0; idx < c.ObjectMemory; idx++)
            {
                if (monitor[idx])
                    modelManager.InitModel(ref c.StaticModelComps[idx]);
            }
        }

        //TEST END

        public void Update(float time)
        {
            throw new NotImplementedException();
        }
    }
}
