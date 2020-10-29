using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeeSynk.Core.Components.Types.UI
{
    public class ComponentElement : IComponent
    {
        public Component BitMaskID => Component.UI_ELEMENT;

        private UIElement _element;
        public UIElement Element { get => _element; }

        public ComponentElement(UIElement element)
        {
            _element = element;
        }
    }
}
