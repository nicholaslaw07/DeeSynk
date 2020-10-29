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
        private UI _ui;

        public SystemUI(World world, UI ui)
        {
            _world = world;
            _ui = ui;
        }


        public void Update(float time)
        {
            throw new NotImplementedException();
        }
    }
}
