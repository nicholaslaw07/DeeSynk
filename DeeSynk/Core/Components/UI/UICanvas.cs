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
        private uint _uiElementCount;
        /// <summary>
        /// The number of IUIElements stored in this context.
        /// </summary>
        public uint UIElementCount { get => _uiElementCount; }

        private uint _width;
        /// <summary>
        /// Width of the canvas (in pixels for now).
        /// </summary>
        public uint Width { get => _width; }

        private uint _height;
        /// <summary>
        /// Height of the cavnas (in pixels for now).
        /// </summary>
        public uint Height { get => _height; }

        private string _name;
        /// <summary>
        /// The name of this canvas used for lookups and debug.
        /// </summary>
        public string Name { get => _name; }

        private IUIElement[] _elements;
        /// <summary>
        /// Array of the elements contained within the canvas.
        /// </summary>
        public IUIElement[] Elements { get => _elements; }
    }
}
//It seems as if the only way to have a seperate UI system while maintaining incapsulation
//is to move SystemRender into the Game class which means the SystemRender must be coded to
//operate on both the World and UI classes simultaneously.  This also requires removing all
//depence of World on SystemRender and vice versa. 