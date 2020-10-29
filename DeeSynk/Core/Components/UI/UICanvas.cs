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
        public static readonly string ID_DEFAULT = "CANVAS";

        private int _uiElementCount;
        /// <summary>
        /// The number of IUIElements stored in this context.
        /// </summary>
        public int UIElementCount { get => _uiElementCount; }

        private int[] _elementIDs;
        /// <summary>
        /// Array of the indexes of all elements contained within the canvas.
        /// </summary>
        public int[] ElementIDs
        {
            get => _elementIDs;

            set
            {
                if (value.Length != _uiElementCount)
                    throw new Exception("Element count does not match the expected count.");
                else
                    _elementIDs = value;
            }
        }

        private bool[] _existingElements;
        /// <summary>
        /// Each index is true if the corresponding _elementID exists.
        /// </summary>
        public bool[] ExistingElement { get => _existingElements; }

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

        private int _globalIndex;
        /// <summary>
        /// Index of this game object inside of the UI class.
        /// </summary>
        public int GlobalIndex { get => _globalIndex; }

        private string _globalID;
        /// <summary>
        /// A global name or identifier mostly used for debug purposes.
        /// </summary>
        public string GlobalID { get => _globalID; }


        public UICanvas()
        {
            _uiElementCount = 0;

            _elementIDs = new int[_uiElementCount];

            _existingElements = new bool[_uiElementCount];

            _width = 1;
            _height = 1;

            _globalIndex = 0;
            _globalID = $"{ID_DEFAULT}_{_globalIndex}";

        }

        public UICanvas(int elementCount, int width, int height, int globalIndex)
        {
            _uiElementCount = elementCount;
            _elementIDs = new int[_uiElementCount];

            _existingElements = new bool[_uiElementCount];

            _width = width;
            _height = height;

            _globalIndex = globalIndex;
            _globalID = $"{ID_DEFAULT}_{_globalIndex}";
        }

        public UICanvas(int elementCount, int width, int height, int globalIndex, string globalID)
        {
            _uiElementCount = elementCount;
            _elementIDs = new int[_uiElementCount];

            _existingElements = new bool[_uiElementCount];

            _width = width;
            _height = height;

            _globalIndex = globalIndex;
            _globalID = globalID;
        }

        public void AddChild(UIElement e)
        {
            try
            {
                int idx = FindEmptyElementSpot();
                _elementIDs[idx] = e.GlobalIndex;
                _existingElements[idx] = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Console.WriteLine("Element not added.");
            }
        }

        private int FindEmptyElementSpot()
        {
            for (int i = 0; i < _uiElementCount; i++)
            {
                if (!_existingElements[i])
                {
                    return i;
                }
            }
            throw new Exception("Allocated element memory full.");
        }
    }
}
//It seems as if the only way to have a seperate UI system while maintaining incapsulation
//is to move SystemRender into the Game class which means the SystemRender must be coded to
//operate on both the World and UI classes simultaneously.  This also requires removing all
//depence of World on SystemRender and vice versa. 