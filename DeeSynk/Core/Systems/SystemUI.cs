using DeeSynk.Core.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeeSynk.Core.Systems
{
    public class SystemUI : ISystem
    {
        public Component MonitoredComponents => throw new NotImplementedException();

        private World _world;

        public SystemUI(World world)
        {
            _world = world;
        }


        public void Update(float time)
        {
            throw new NotImplementedException();
        }
    }
}
