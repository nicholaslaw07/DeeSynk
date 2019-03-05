using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeeSynk.Core.Components.Types.Render
{
    public class ComponentTexture : IComponent
    {
        public int BitMaskID => (int)Component.TEXTURE;

        public void Update(float time)
        {
            throw new NotImplementedException();
        }
    }
}
