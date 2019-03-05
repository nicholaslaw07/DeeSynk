using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeeSynk.Core.Components.Types
{
    public class ComponentScale : IComponent
    {
        public Component BitMaskID => Component.SCALE;

        private Vector3 _scale;
        public Vector3 Scale { get => _scale; set => _scale = value; }

        //DEFAULT CONSTRUCTOR
        

        public void Update(float time)
        {
            throw new NotImplementedException();
        }
    }
}
