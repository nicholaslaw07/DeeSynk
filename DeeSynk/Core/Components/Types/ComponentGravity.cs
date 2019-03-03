using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeeSynk.Core.Components.Types
{
    public class ComponentGravity : IComponent
    {
        public Component BitMaskID => Component.GRAVITY;

        public float WORLD_GRAVITY => 9.8f;

        //uses world gravity bool

        private float _gravityScalar;
        public float Gravity { get => _gravityScalar; }
        
        public ComponentGravity()
        {
            _gravityScalar = WORLD_GRAVITY;
        }


    }
}
