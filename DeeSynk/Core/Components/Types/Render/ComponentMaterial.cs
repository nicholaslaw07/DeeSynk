using OpenTK.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeeSynk.Core.Components.Types.Render
{
    public class ComponentMaterial : IComponent
    {
        public Component BitMaskID => Component.MATERIAL;

        private Color4 _color;
        public Color4 Color { get => _color; }

        public ComponentMaterial(Color4 color)
        {
            _color = color;
        }
    }
}
