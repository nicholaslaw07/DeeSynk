using DeeSynk.Core.Algorithms;
using DeeSynk.Core.Components;
using DeeSynk.Core.Components.Models;
using DeeSynk.Core.Components.Types.Render;
using DeeSynk.Core.Managers;
using OpenTK;
using OpenTK.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeeSynk.Core.Systems
{
    public class SystemModel : ISystem
    {
        public Component MonitoredComponents => Component.MODEL_STATIC;

        private World _world;

        private bool[] _monitoredGameObjects;

        private ComponentModelStatic[] _staticModelComps;

        public SystemModel(World world)
        {
            _world = world;


            _monitoredGameObjects = new bool[_world.ObjectMemory];

            _staticModelComps = _world.StaticModelComps;
        }

        public void UpdateMonitoredGameObjects()
        {
            for (int i = 0; i < _world.ObjectMemory; i++)
            {
                if (_world.ExistingGameObjects[i])
                {
                    if (_world.GameObjects[i].Components.HasFlag(MonitoredComponents))
                    {
                        _monitoredGameObjects[i] = true;
                    }
                }
            }
        }

        //TEST START
        public void InitModel()
        {

            CreateModels();

            var modelManager = ModelManager.GetInstance();
            for(int idx = 0; idx < _staticModelComps.Length; idx++)
            {
                if(_monitoredGameObjects[idx])
                    modelManager.InitModel(ref _staticModelComps[idx]);
            }
        }

        private void CreateModels()
        {
            var v00 = new Vector3(0, 8, 0);
            var v01 = new Vector3(5f, 5f, 5f);
            var v02 = new Vector2(0.0f, 0.0f);
            var v03 = new Vector2(1.0f, 1.0f);
            var v21 = Color4.White;

            Random r = new Random();


            _staticModelComps[0] = new ComponentModelStatic(ModelProperties.VERTICES_NORMALS_COLORS_ELEMENTS, ModelReferenceType.DISCRETE, "TestCube",
                                        ConstructionFlags.VECTOR3_OFFSET | ConstructionFlags.COLOR4_COLOR | ConstructionFlags.VECTOR3_SCALE,
                                        new Vector3(0, 0.5f, 0), new Vector3(5f, 5f, 5f), v21);

            Texture t = TextureManager.GetInstance().GetTexture("wood");
            float width = t.Width;
            float height = t.Height;

            var v10 = new Vector3(0);
            var v14 = new Vector3(1 / 20f * t.AspectRatio, 0f, 1 / 20f);
            var v11 = new Vector3(100f, 0f, 100f);
            var v12 = t.SubTextureLocations[0].UVOffset;
            var v13 = t.SubTextureLocations[0].UVScale;
            _staticModelComps[1] = new ComponentModelStatic(ModelProperties.VERTICES_UVS_ELEMENTS, ModelReferenceType.TEMPLATE, ModelTemplates.TemplatePlaneXZ,
                                                            ConstructionFlags.VECTOR3_OFFSET | ConstructionFlags.FLOAT_ROTATION_Y | ConstructionFlags.VECTOR3_SCALE |
                                                            ConstructionFlags.VECTOR3_DIMENSIONS |
                                                            ConstructionFlags.VECTOR2_UV_OFFSET | ConstructionFlags.VECTOR2_UV_SCALE,
                                                            v10, 3.1415927f / 2f, v14, v11, v12, v13);
        }

        //TEST END

        public void Update(float time)
        {
            throw new NotImplementedException();
        }
    }
}
