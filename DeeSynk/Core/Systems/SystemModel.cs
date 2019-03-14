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
            _staticModelComps[0] = new ComponentModelStatic(ModelProperties.VERTICES_NORMALS_COLORS_ELEMENTS, ModelReferenceType.DISCRETE, "dragon_vripPLY", 
                                                            ConstructionParameterFlags.VECTOR3_OFFSET,
                                                            new Vector3(0, 1, -5));

            _staticModelComps[1] = new ComponentModelStatic(ModelProperties.VERTICES_COLORS_ELEMENTS, ModelReferenceType.TEMPLATE,
                                                            ConstructionParameterFlags.VECTOR3_OFFSET | 
                                                            ConstructionParameterFlags.VECTOR2_DIMENSIONS | 
                                                            ConstructionParameterFlags.COLOR4_COLOR, 
                                                            ModelTemplates.TemplatePlaneXZ,
                                                            new Vector3(0, -5, 0), new Vector2(100f, 100f), Color4.IndianRed);
        }

        private float[] Vector3(int v1, float v2, int v3)
        {
            throw new NotImplementedException();
        }

        //TEST END

        public void Update(float time)
        {
            throw new NotImplementedException();
        }
    }
}
