using DeeSynk.Core.Components.Input;
using DeeSynk.Core.Components.Models.Tools;
using DeeSynk.Core.Components.UI;
using OpenTK;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        UI_CONTAINER = 1
    }

    public enum PositionType
    {
        GLOBAL = 0,
        LOCAL = 1,
        LOCAL_BOTTOM_LEFT = 2,
        LOCAL_BOTTOM_RIGHT = 3,
        LOCAL_TOP_LEFT = 4,
        LOCAL_TOP_RIGHT = 5
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

        public abstract string ID_GLOBAL_DEFAULT { get; }

        protected int _childElementCount;
        /// <summary>
        /// Number of child UIElements this Element contains
        /// </summary>
        public int ChildElementCount { get => _childElementCount; }

        protected int[] _childElementIDs;
        /// <summary>
        /// The integer indexes within the component arrays of the UI class for all UIElements in the layer directly above the layer of this UIElement.
        /// </summary>
        public int[] ChildElementIDs
        {
            get => _childElementIDs;

            set
            {
                if (value.Length != _childElementCount)
                    throw new Exception("Element count does not match the expected count.");
                else _childElementIDs = value;
            }
        }

        protected bool[] _existingElements;
        /// <summary>
        /// Each index is true if the corresponding _elementID exists.
        /// </summary>
        public bool[] ExistingElements { get => _existingElements; }

        /// <summary>
        /// States whether or not this UIElement has children.
        /// </summary>
        public bool HasChildElements { get => _childElementCount == 0; }


        protected UIElementType _elementType;
        /// <summary>
        /// Type of object this UIElement is.
        /// </summary>
        public UIElementType ElementType { get => _elementType; }

        protected int _width;
        /// <summary>
        /// Width of the UIElement
        /// </summary>
        public int Width { get => _width; }

        protected int _height;
        /// <summary>
        /// Height of the UIElement
        /// </summary>
        public int Height { get => _height; }

        protected Vector2 _position;
        /// <summary>
        /// Position of the UIElement within a canvas or element
        /// </summary>
        public Vector2 Position
        {
            get => _position;
            set => _position = value;
        }

        protected PositionType _positionType;
        /// <summary>
        /// States whether or not the Position variable dictates global canvas position or local element position.
        /// </summary>
        public PositionType PositionType { get => _positionType; }

        protected Vector2 _referenceCoord;
        /// <summary>
        /// The offset of the object from the center to the local origin.
        /// </summary>
        public Vector2 ReferenceCoord { get => _referenceCoord; }

        protected PositionReference _reference;
        /// <summary>
        /// Determines the origin of the object which dictates transformation.
        /// </summary>
        public PositionReference Reference { get => _reference; }

        protected int _layer;
        /// <summary>
        /// This determines render order with 0 being the back most layer.
        /// </summary>
        public int Layer { get => _layer; }

        protected string _globalID;  //examples include "healthBarContainer"
        /// <summary>
        /// A unique ID that is distinct from any other UIElement.
        /// </summary>
        public string GlobalID { get => _globalID; }

        protected int _globalIndex;
        /// <summary>
        /// The index of this UIElement in the component arrays of the UI class
        /// </summary>
        public int GlobalIndex { get => _globalIndex; }

        protected int _localID;
        /// <summary>
        /// An incrementing integer ID that represents the unique identifier of this UIElement within the current layer.  Two objects with different parents can have the same LocalID.
        /// </summary>
        public int LocalID { get => _localID; }

        protected string _canvasID;
        /// <summary>
        /// The ID that represents the canvas which contains this UIElement.
        /// </summary>
        public string CanvasID { get => _canvasID; set => _canvasID = value; }

        protected string _path;
        /// <summary>
        /// Path of the UIElement within the canvas.
        /// </summary>
        public string Path { get => _path; }

        public UIElement()
        {
            _childElementCount = 0;
            _childElementIDs = new int[0];
            _existingElements = new bool[_childElementCount];

            _elementType = UIElementType.NONE;

            _width = 0;
            _height = 0;

            _position = Vector2.Zero;

            _layer = 0;

            _canvasID = UICanvas.ID_DEFAULT;
        }

        public UIElement(int[] childElementIDs, UIElementType elementType, int width, int height, Vector2 position, 
                         PositionType positionType, PositionReference reference, int layer, int globalIndex)
        {
            _childElementCount = childElementIDs.Length;
            _childElementIDs = childElementIDs;
            _existingElements = new bool[_childElementCount];
            _elementType = elementType;
            _width = width;
            _height = height;
            _position = position;
            _positionType = positionType;
            _referenceCoord = Models.Tools.ReferenceConverter.GetReferenceOffset2(reference, new Vector2((float)width, (float)height));
            _reference = reference;
            _layer = layer;
            _globalIndex = globalIndex;
            //_localID = localID;
        }

        public UIElement(int childElementCount, UIElementType elementType, int width, int height, Vector2 position,
                        PositionType positionType, PositionReference reference, int layer, int globalIndex)
        {
            _childElementCount = childElementCount;
            _childElementIDs = new int[_childElementCount];
            _existingElements = new bool[_childElementCount];
            _elementType = elementType;
            _width = width;
            _height = height;
            _position = position;
            _positionType = positionType;
            _referenceCoord = Models.Tools.ReferenceConverter.GetReferenceOffset2(reference, new Vector2((float)width, (float)height));
            _reference = reference;
            _layer = layer;
            _globalIndex = globalIndex;
            //_localID = localID;
        }

        public void SetPath(string parentPath)
        {
            _path = parentPath + "\\" + ID_GLOBAL_DEFAULT + "_" + _globalIndex;
        }

        public virtual void AddChild(UIElement e)
        {
            try
            {
                int idx = FindEmptyElementSpot();
                _childElementIDs[idx] = e._globalIndex;
                _existingElements[idx] = true;
                e.CanvasID = _canvasID;
                e.SetPath(_path);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Console.WriteLine("Element not added.");
            }
        }

        private int FindEmptyElementSpot()
        {
            for(int i=0; i<_childElementCount; i++)
            {
                if (!_existingElements[i])
                {
                    return i;
                }
            }
            throw new Exception("Allocated element memory full.");
        }

        public abstract void ClickAt(float time, MouseClick mouseClick, MouseMove mouseMove);

        public abstract bool Update(float time);
    }
}
