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
        UI_BOX = 0
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
    }
}
