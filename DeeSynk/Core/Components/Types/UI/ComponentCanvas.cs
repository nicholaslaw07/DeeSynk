using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DeeSynk.Core.Components;

namespace DeeSynk.Core.Components.Types.UI
{

    public class ComponentCanvas : IComponent
    {
        public Component BitMaskID => Component.UI_CANVAS;

        private UICanvas _canvas;
        public UICanvas Canvas { get => _canvas; }

        public ComponentCanvas(UICanvas canvas)
        {
            _canvas = canvas;
        }
    }
}
//for the rendering we can store all of the menu data into one vao and
//organize it in such a way so that we can just flip between different
//sections for the different lawyers of boxes, lines, borders, sliders,
//radio buttons, check boxes, and text.

//text can be stored as an array buffer with text bounds equaling some
//integer product of the width and height of the characters

//vaos are stored in world but id is of course stored in the render comp.
//where to store and easily manage buffer offsets for the different sections
//needs to be figured out, but it will likely be stored in _canvas as much
//as I don't want to do that lmao