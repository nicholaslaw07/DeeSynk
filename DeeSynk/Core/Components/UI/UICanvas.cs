using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeeSynk.Core.Components.Types.UI
{
    /// <summary>
    /// A canvas for placing UIElements such as menus, lists, sliders, buttons, and custom.  Structured like an xml file but allows for customized interactions.
    /// </summary>
    public class UICanvas
    {
        private int _uiElementCount;
        /// <summary>
        /// The number of IUIElements stored in this context.
        /// </summary>
        public int UIElementCount { get => _uiElementCount; }

        private UIElement[] _elements;
        /// <summary>
        /// Array of the elements contained within the canvas.
        /// </summary>
        public UIElement[] Elements
        {
            get => _elements;

            set
            {
                if (value.Length != _uiElementCount)
                    throw new Exception("Element count does not match the expected count.");
                else
                    _elements = value;
            }
        }

        private int _width;
        /// <summary>
        /// Width of the canvas (in pixels for now).
        /// </summary>
        public int Width { get => _width; }

        private int _height;
        /// <summary>
        /// Height of the cavnas (in pixels for now).
        /// </summary>
        public int Height { get => _height; }

        private string _name;
        /// <summary>
        /// The name of this canvas used for lookups and debug.
        /// </summary>
        public string Name { get => _name; }

        public UICanvas()
        {
            _uiElementCount = 0;

            _elements = new UIElement[_uiElementCount];

            _width = 1;
            _height = 1;

            _name = "DEFAULT";

        }

        public UICanvas(int elementCount, int width, int height, string name)
        {
            _uiElementCount = elementCount;
            _elements = new UIElement[_uiElementCount];

            _width = width;
            _height = height;

            _name = name;
        }
    }
}
//It seems as if the only way to have a seperate UI system while maintaining incapsulation
//is to move SystemRender into the Game class which means the SystemRender must be coded to
//operate on both the World and UI classes simultaneously.  This also requires removing all
//depence of World on SystemRender and vice versa. 