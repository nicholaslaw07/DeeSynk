using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeeSynk.Core.Components.Types.UI
{
    /// <summary>
    /// A type label for an abstract UIElement instance.
    /// </summary>
    public enum UIElementType
    {
        NONE = 0,
        UI_BOX = 1
    }

    public enum UIPositionType
    {
        GLOBAL = 0,
        LOCAL = 1
    }

    //Primarily used for organization and debug.  Most of the rendering protocols will be hand coded for each object.
    //This UI systems partially breaks away from the ECS structure in how the componoents are implemented.  This is expected though.
    //Only core components of the engine that are universal across all systems such as rendering and data managment should follow ECS strictly.
    //Other areas have freedom and in some cases are better suited to other organizations such as strong inherence.
    //ECS is still used in the UI for rendering, vao communication, models, textures, and materials.  The plan is to be able to add any component
    //to a UI Element and it most work as expected.

    /// <summary>
    /// An object type representing all menus, text boxes, containers, and custom types of UI components within a UICanvas.
    /// </summary>
    public abstract class UIElement
    {
        //for future reference, ensure that uint is not used since it CLS Comliant

        //for now these uiElements are static and cannot be modified after creation like most objects

        private int _childElementCount;
        /// <summary>
        /// Number of child UIElements this Element contains
        /// </summary>
        public int ChildElementCount { get => _childElementCount; }

        private UIElement[] _childElements;
        /// <summary>
        /// Array of UIElements this UIElement is a parent to.
        /// </summary>
        public UIElement[] ChildElements
        {
            get => _childElements;

            set
            {
                if (value.Length != _childElementCount)
                    throw new Exception("Element count does not match the expected count.");
                else _childElements = value;
            }
        }

        /// <summary>
        /// States whether or not this UIElement has children.
        /// </summary>
        public bool HasChildElements { get => _childElementCount == 0; }


        private UIElementType _elementType;
        /// <summary>
        /// Type of object this UIElement is.
        /// </summary>
        public UIElementType ElementType { get => _elementType; }

        private int _width;
        /// <summary>
        /// Width of the UIElement
        /// </summary>
        public int Width { get => _width; }

        private int _height;
        /// <summary>
        /// Height of the UIElement
        /// </summary>
        public int Height { get => _height; }

        private Vector2 _position;
        /// <summary>
        /// Position of the UIElement within a canvas or element
        /// </summary>
        public Vector2 Position { get => _position; }

        private UIPositionType _positionType;
        /// <summary>
        /// States whether or not the Position variable dictates global canvas position or local element position.
        /// </summary>
        public UIPositionType PositionType { get => _positionType; }

        private int _layer;
        /// <summary>
        /// This determines render order with 0 being the back most layer.
        /// </summary>
        public int Layer { get => _layer; }

        public UIElement()
        {
            _childElementCount = 0;
            _childElements = new UIElement[0];

            _elementType = UIElementType.NONE;

            _width = 0;
            _height = 0;

            _position = Vector2.Zero;

            _layer = 0;
        }
    }
}
