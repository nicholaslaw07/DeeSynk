using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeeSynk.Core.Components.Types
{
    public class ComponentTransform : IComponent
    {
        public Component BitMaskID => Component.TRANSFORM;

        public void Update(float time)
        {
            throw new NotImplementedException();
        }
    }
}
