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
    class SystemModel : ISystem
    {
        public int MonitoredComponents => throw new NotImplementedException();

        private World _world;

        private bool[] _monitoredGameObjects;

        private ComponentModelStatic[] _staticModelComps;

        public SystemModel(World world)
        {
            _world = world;

            _monitoredGameObjects = new bool[_world.ObjectMemory];

            _staticModelComps = _world.StaticModelComps;
        }

        //TEST START
        public void InitModel()
        {
            var v00 = new Vector3(0, 1, 0);
            _staticModelComps[0] = new ComponentModelStatic(ModelProperties.VERTICES_NORMALS_COLORS_ELEMENTS, ModelReferenceType.DISCRETE, "dragon_vripPLY",
                                                            ConstructionFlags.VECTOR3_OFFSET,
                                                            v00);

            var v10 = new Vector3(1, -5, -1);
            var v11 = new Vector3(13f, 13f, 13f);
            var v12 = new Vector2(0.0f, 0.0f);
            var v13 = new Vector2(1.0f, 1.0f);
            _staticModelComps[1] = new ComponentModelStatic(ModelProperties.VERTICES_UVS_ELEMENTS, ModelReferenceType.TEMPLATE, ModelTemplates.TemplatePlaneXZ,
                                                            ConstructionFlags.VECTOR3_OFFSET | ConstructionFlags.VECTOR3_DIMENSIONS | ConstructionFlags.VECTOR2_UV_OFFSET | ConstructionFlags.VECTOR2_UV_SCALE,
                                                            v10, v11, v12, v13);
        }

        //TEST END

        public void Update(float time)
        {
            throw new NotImplementedException();
        }
    }
}
