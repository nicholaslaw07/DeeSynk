using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DeeSynk.Core.Components;

namespace DeeSynk.Core.Components.Types.UI
{

    public class ComponentUI : IComponent
    {
        public Component BitMaskID => Component.UI_CANVAS;

        private UICanvas _canvas;
        public UICanvas Canvas { get => _canvas; }

        public ComponentUI(UICanvas canvas)
        {
            _canvas = canvas;
        }
    }
}
