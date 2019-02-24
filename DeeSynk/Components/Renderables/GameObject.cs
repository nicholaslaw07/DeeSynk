using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;

namespace DeeSynk.Components.Renderables
{
    public class GameObject
    {
        public int ObjectID;

        private bool   _visible;
        protected bool Visible { get => _visible; set => _visible = value; }

        private bool   _initVAO; // is the VAO initialized
        protected bool InitVAO { get => _initVAO; set => _initVAO = value; }  //inside of the get, add a means of checking if the buffer references have been created

        private bool   _initPrograms;
        protected bool InitPrograms { get => _initPrograms; }

        private int _VAO;  //Vertex Array Object ID number, aka an integer reference used to represent the objects location
        private int _VBO;  //Vertex Buffer Object
        private int _IBO;  //Index Buffer Object

        protected int VAO { get => _VAO; }
        protected int VBO { get => _VBO; }
        protected int IBO { get => _IBO; }

        private ColoredVertex[]  _verticesC;
        private TexturedVertex[] _verticesT;
        private int              _vertexCount;
        protected IntPtr Vertices{
            get{
                unsafe {
                    IntPtr p;
                    if (isTextured){
                        fixed(TexturedVertex* ptr = _verticesT)
                        { p = (IntPtr)ptr; }}
                    else{
                        fixed (ColoredVertex* ptr = _verticesC)
                        { p = (IntPtr)ptr; }}
                    return p;
                }
            }
        }

        protected int   VertexCount { get => _vertexCount; }

        private uint[]    _indices;
        private int       _indexCount;
        protected uint[]  Indices      { get => _indices; }
        protected int     IndexCount   { get => _indexCount; }

        private bool   _isTextured;
        protected bool isTextured { get => _isTextured; }

        //THESE SIX FIELDS ARE VERY TEMPORARY AND ARE SUBJECT TO FUTURE CHANGES
        //private List<string>  _programReferenceNames;  //List of the string reference names (used in ShaderManager) that this specific object may use to render itself, NOT the program IDs, however.
        //private string        _activeProgramReferenceName;  //string reference name that is currently active  
        //private int           _activeProgramID;        //**NOTE: It is conceivable that an object may need to use multiple ProgramIDs. We must account for this somehow...

        //protected List<string> ProgramReferenceNames { get => _programReferenceNames; }
        //protected string ActiveProgramReferenceName { get => _activeProgramReferenceName; }
        //protected int ActiveProgramID { get => _activeProgramID; }

        private int[] _programIDs;
        private int _activeProgramID;

        protected ref int[] ProgramIDs { get => ref _programIDs; }
        protected int ActiveProgramID { get => _activeProgramID; }

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



        public GameObject(int objectID, int renderID, int renderLayer, ColoredVertex[] vertices, uint[] indices)
        {
            ObjectID = objectID;

            _visible = true;
            _initVAO = false;
            _initPrograms = false;

            _verticesC = vertices;
            _vertexCount = vertices.Length;

            _indices = indices;
            _indexCount = _indices.Length;

            _isTextured = false;

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

        public GameObject(int objectID, int renderID, int renderLayer, TexturedVertex[] vertices, uint[] indices)
        {
            ObjectID = objectID;

            _visible = true;
            _initVAO = false;
            _initPrograms = false;

            _verticesT = vertices;
            _vertexCount = vertices.Length;

            _indices = indices;
            _indexCount = _indices.Length;

            _isTextured = true;

            _renderID = renderID;
            _renderLayer = renderLayer;

            _position = new Vector3(0.0f, 0.0f, 0.0f);
            _rotX = 0.0f;
            _rotY = 0.0f;
            _rotZ = 0.0f;
            _scale = new Vector3(1.0f, 1.0f, 1.0f);

            _translationMat4 = Matrix4.Identity;
            _rotXMat4 = Matrix4.Identity;
            _rotYMat4 = Matrix4.Identity;
            _rotZMat4 = Matrix4.Identity;
            _scaleMat4 = Matrix4.Identity;
        }

        public GameObject(int objectID, int renderID, int renderLayer,
                    ColoredVertex[] vertices, uint[] indices,
                    Vector3 position, float rotX, float rotY, float rotZ, Vector3 scale)
        {
            ObjectID = objectID;

            _visible = true;
            _initVAO = false;
            _initPrograms = false;

            _verticesC = vertices;
            _vertexCount = vertices.Length;

            _indices = indices;
            _indexCount = _indices.Length;

            _isTextured = false;

            _renderID = renderID;
            _renderLayer = renderLayer;

            _position = position;
            _rotX = rotX;
            _rotY = rotY;
            _rotZ = rotZ;
            _scale = scale;

            _translationMat4 = Matrix4.Identity;
            _rotXMat4 = Matrix4.Identity;
            _rotYMat4 = Matrix4.Identity;
            _rotZMat4 = Matrix4.Identity;
            _scaleMat4 = Matrix4.Identity;
        }

        public GameObject(int objectID, int renderID, int renderLayer,
                            TexturedVertex[] vertices, uint[] indices, 
                            Vector3 position, float rotX, float rotY, float rotZ, Vector3 scale)
        {
            ObjectID = objectID;

            _visible = true;
            _initVAO = false;
            _initPrograms = false;

            _verticesT = vertices;
            _vertexCount = vertices.Length;

            _indices = indices;
            _indexCount = _indices.Length;

            _isTextured = true;

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

        public GameObject InitializeVAO()
        {
            if (_vertexCount <= _indexCount)
            {
                //IF VERTEX EQUAL 0 OR INDEX EQUAL 0 THROW ERROR
                int vertexSize = (isTextured) ? 24 : 32;
                int displayDataSize = (isTextured) ? 2 : 4;

                _VAO = GL.GenVertexArray();
                _VBO = GL.GenBuffer();
                _IBO = GL.GenBuffer();

                GL.BindVertexArray(_VAO);
                GL.BindBuffer(BufferTarget.ArrayBuffer, _VBO);
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, _IBO);

                GL.NamedBufferStorage(_VBO, vertexSize * _vertexCount, Vertices, BufferStorageFlags.MapWriteBit);

                GL.VertexArrayAttribBinding(_VAO, 0, 0);
                GL.EnableVertexArrayAttrib(_VAO, 0);
                GL.VertexArrayAttribFormat(_VAO, 0, 4, VertexAttribType.Float, false, 0);

                GL.VertexArrayAttribBinding(_VAO, 1, 0);
                GL.EnableVertexArrayAttrib(_VAO, 1);
                GL.VertexArrayAttribFormat(_VAO, 1, displayDataSize, VertexAttribType.Float, false, 16);

                GL.VertexArrayVertexBuffer(_VAO, 0, _VBO, IntPtr.Zero, vertexSize);
                GL.NamedBufferStorage(_IBO, 4 * _indexCount, _indices, BufferStorageFlags.MapWriteBit);

                GL.BindVertexArray(0);

                InitVAO = true;
            }
            else
            {
                Visible = false;
            }

            return this;
        }

        public GameObject AddProgramIDs(int[] programIDs)
        {
            if(programIDs.Length > 0)
            {
                _programIDs = programIDs;
                _activeProgramID = _programIDs[0]; //temporary
                _initPrograms = true;
            }
            else
            {
                Visible = false;
            }
            return this;
        }

        //public abstract TexturedRenderObject AddTextureIDs();

        //public abstract void Update();

        public virtual void Render(Matrix4 ortho)
        {

            RotationZ += 0.01f;
            var r = RotationZMat4;
            var t = TranslationMat4;
            var s = ScaleMat4;

            GL.UseProgram(ActiveProgramID);
            GL.BindVertexArray(_VAO);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _IBO);
            GL.UniformMatrix4(2, false, ref ortho);
            GL.UniformMatrix4(3, false, ref r);
            GL.UniformMatrix4(4, false, ref t);
            GL.UniformMatrix4(5, false, ref s);
            GL.DrawElements(BeginMode.Triangles, IndexCount, DrawElementsType.UnsignedInt, 0);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
            GL.BindVertexArray(0);
            GL.UseProgram(0);
        }
    }
}

//Add transformations to shaders and pass them via Render in this class
//Figure out how to best pass or set ortho as the current orthographic matrix

//Begin adding the capabilities to have components within GameObject 
//Texturing will likely be a component

//Determine the best ways to add tiling - Loop Render and modify transform matrix every time?
    //Custom VAOs?

//Optimize Optimize Optimize


