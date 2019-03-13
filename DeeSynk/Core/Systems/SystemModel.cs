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
            _staticModelComps[0] = new ComponentModelStatic(ModelProperties.VERTICES_ELEMENTS, ModelReferenceType.DISCRETE, "dragon_vripPLY");
        }
        //TEST END

        public void Update(float time)
        {
            throw new NotImplementedException();
        }
    }
}
