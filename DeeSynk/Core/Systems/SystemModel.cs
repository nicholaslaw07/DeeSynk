using DeeSynk.Core.Components.Models;
using DeeSynk.Core.Components.Types.Render;
using DeeSynk.Core.Managers;
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
            float[] data2 = new float[3];
            data2[0] = 0;
            data2[1] = 1000f;
            data2[2] = 0;
            _staticModelComps[0] = new ComponentModelStatic(ModelProperties.VERTICES_NORMALS_COLORS_ELEMENTS, ModelReferenceType.DISCRETE, "dragon_vripPLY",
                                            ConstructionParameterFlags.VECTOR3_OFFSET, data2);

            float[] data = new float[6];
            data[0] = 1000f;
            data[1] = 1000f;

            data[2] = 0.5f;
            data[3] = 0.5f;
            data[4] = 0.5f;
            data[5] = 1.0f;

            _staticModelComps[1] = new ComponentModelStatic(ModelProperties.VERTICES_COLORS_ELEMENTS, ModelReferenceType.TEMPLATE,
                                                            ConstructionParameterFlags.VECTOR2_DIMENSIONS | ConstructionParameterFlags.COLOR4_COLOR, data,
                                                            ModelTemplates.TemplatePlaneXZ);
        }
        //TEST END

        public void Update(float time)
        {
            throw new NotImplementedException();
        }
    }
}
