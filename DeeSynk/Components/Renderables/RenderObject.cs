using System;
using System.Collections.Generic;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;

namespace DeeSynk.Components.Renderables
{
    public abstract class RenderObject
    {
        private bool _isVisible;
        protected bool IsVisible { get => _isVisible; set => _isVisible = value; }

        //THESE SIX FIELDS ARE VERY TEMPORARY AND ARE SUBJECT TO FUTURE CHANGES
        protected List<string>  _programReferenceNames;  //List of the string reference names (used in ShaderManager) that this specific object may use to render itself, NOT the program IDs, however.
        protected string        _activeProgramReferenceName;  //string reference name that is currently active  
        protected int           _activeProgramID;        //**NOTE: It is conceivable that an object may need to use multiple ProgramIDs. We must account for this somehow...

        //Rendering Attributes
        private int _renderID;  //likely will be given from the render manager or similar class
        public  int RenderID { get => _renderID; }

        private int _renderLayer;  //just like renderID, will likely be assigned from render manager or similar class
        public  int RenderLayer { get => _renderLayer; set => _renderLayer = value; }  //somehow make sure that this render layer is not an invalid value...

        //Transformation Properties
        private Vector3 _location;  //position in world space (not sure if it is needed)
        public  Vector3 Location { get => _location; }

        private Vector3 _position;
        public  Vector2 PositionV2  { get => _position.Xy; }    //gets 2D (xy) parts of the 3D position vector | location in the rendering space
        public  Vector3 Position    { get => _position; set => _position = value; }   //gets the original 3D vector
        public  Vector4 PositionV4  { get => new Vector4(_position); }  //returns a 4D position vector constructed from the 3D position vector

        private float _rotX, _rotY, _rotZ;
        public  float RotationX     { get => _rotX; set => _rotX = value; } //property for x rotation
        public  float RotationY     { get => _rotY; set => _rotY = value; } //property for y rotation
        public  float RotationZ     { get => _rotZ; set => _rotZ = value; } //property for z rotation

        private Vector3 _scale;
        public  Vector3 Scale       { get => _scale; set => _scale = value; } //property for scale of the object

        //Transformation Matrix Properties
        private Matrix4 _translationMat4,
                        _rotXMat4, _rotYMat4, _rotZMat4,
                        _scaleMat4;

        protected Matrix4 TranslationMat4 { get { Matrix4.CreateTranslation(ref _position, out _translationMat4); return _translationMat4; } } 
        protected Matrix4 RotationXMat4   { get { Matrix4.CreateRotationX(_rotX, out _rotXMat4); return _rotXMat4; } }   //since there should be few, if any, reason to use the actual matrix                       
        protected Matrix4 RotationYMat4   { get { Matrix4.CreateRotationY(_rotY, out _rotYMat4); return _rotYMat4; } }   //more than once per frame, when the matrix is called                        
        protected Matrix4 RotationZMat4   { get { Matrix4.CreateRotationZ(_rotZ, out _rotZMat4); return _rotZMat4; } }   //it is automatically updated to the most current state of its specific transformation
        protected Matrix4 ScaleMat4       { get { Matrix4.CreateScale(ref _scale, out _scaleMat4); return _rotXMat4; } }

        protected Matrix4 TranslationMat4_NoUpdate { get => _translationMat4; }
        protected Matrix4 RotationXMat4_NoUpdate   { get => _rotXMat4; }    //although these will likely be rarely used, should these need to be called they return the current state of matrix
        protected Matrix4 RotationYMat4_NoUpdate   { get => _rotYMat4; }    //not necessarily the current state of the transformation fields
        protected Matrix4 RotationZMat4_NoUpdate   { get => _rotZMat4; }
        protected Matrix4 ScaleMat4_NoUpdate       { get => _scaleMat4; }



        public RenderObject(int renderID, int renderLayer)
        {
            _renderID    = renderID;
            _renderLayer = renderLayer;

            _position = new Vector3(0.0f, 0.0f, 0.0f);
            _rotX     = 0.0f;
            _rotY     = 0.0f;
            _rotZ     = 0.0f;
            _scale    = new Vector3(1.0f, 1.0f, 1.0f);

            _translationMat4 = Matrix4.Identity;
            _rotXMat4        = Matrix4.Identity;
            _rotYMat4        = Matrix4.Identity;
            _rotZMat4        = Matrix4.Identity;
            _scaleMat4       = Matrix4.Identity;
        }

        public RenderObject(int renderID, int renderLayer, Vector3 position, float rotX, float rotY, float rotZ, Vector3 scale)
        {
            _renderID    = renderID;
            _renderLayer = renderLayer;

            _position = position;
            _rotX     = rotX;
            _rotY     = rotY;
            _rotZ     = rotZ;
            _scale    = scale;

            _translationMat4 = Matrix4.Identity;
            _rotXMat4        = Matrix4.Identity;
            _rotYMat4        = Matrix4.Identity;
            _rotZMat4        = Matrix4.Identity;
            _scaleMat4       = Matrix4.Identity;
        }

        public abstract RenderObject InitializeVAO();
        public abstract RenderObject AddProgramIDs();

        public abstract void Update();

        public abstract void Bind();

        public abstract void Render();

        //render layer increment
        //render layer decrement
            //will need error checking
    }

    public enum RenderingTypes
    {
        TexuredVertices = 0,
        ColoredVertices = 1
    }
}
