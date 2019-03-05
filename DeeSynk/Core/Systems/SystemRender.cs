using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DeeSynk.Core.Components;
using DeeSynk.Core.Components.Types.Render;

namespace DeeSynk.Core.Systems
{
    class SystemRender : ISystem
    {
        public int MonitoredComponents = (int)Component.RENDER |
                                         (int)Component.MODEL |
                                         (int)Component.TEXTURE |
                                         (int)Component.COLOR;

        private World _world;
        
        private ComponentRender[]       _renderComps;
        private ComponentModel[]        _modelComps;
        private ComponentTexture[]      _textureComps;
        private ComponentColor[]        _colorComps;

        public SystemRender(World world)
        {
            _world = world;

        }

        public void Update(float time)
        {
            throw new NotImplementedException();
        }
    }
}
