using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;

namespace DeeSynk.Core.Renderables
{
    public class GameObject
    {
        private int _objectID;
        public int ObjectID { get => _objectID; }

        private bool _visible;
        protected bool Visible { get => _visible; set => _visible = value; }

        private bool _initVAO; // is the VAO initialized
        protected bool InitVAO { get => _initVAO; set => _initVAO = value; }  //inside of the get, add a means of checking if the buffer references have been created

        private bool _initPrograms;
        protected bool InitPrograms { get => _initPrograms; }

        private int _VAO;  //Vertex Array Object ID number, aka an integer reference used to represent the objects location
        private int _VBO;  //Vertex Buffer Object
        private int _IBO;  //Index Buffer Object

        //protected int VAO { get => _VAO; }
        //protected int VBO { get => _VBO; }
        //protected int IBO { get => _IBO; }

        private ColoredVertex[] _verticesC;
        private TexturedVertex[] _verticesT;
        private IntPtr Vertices {
            get {
                unsafe {
                    IntPtr p;
                    if (isTextured) {
                        fixed (TexturedVertex* ptr = _verticesT)
                        { p = (IntPtr)ptr; } }
                    else {
                        fixed (ColoredVertex* ptr = _verticesC)
                        { p = (IntPtr)ptr; } }
                    return p;
                }
            }
        }

        private int _vertexCount;
        private int VertexCount { get => _vertexCount; }

        private uint[] _indices;
        private int _indexCount;
        protected uint[] Indices { get => _indices; }
        protected int IndexCount { get => _indexCount; }

        private bool _isTextured;
        protected bool isTextured { get => _isTextured; }

        private int[] _programIDs;
        private int _activeProgramID;

        protected ref int[] ProgramIDs { get => ref _programIDs; }
        protected int ActiveProgramID { get => _activeProgramID; }

        //Rendering Attributes
        private int _renderID;  //likely will be given from the render manager or similar class
        public int RenderID { get => _renderID; }

        private int _renderLayer;  //just like renderID, will likely be assigned from render manager or similar class
        public int RenderLayer { get => _renderLayer; set => _renderLayer = value; }  //somehow make sure that this render layer is not an invalid value...

        //Transformation Properties
        private Vector3 _location;  //position in world space (not sure if it is needed)
        public Vector3 Location { get => _location; }

        private const int UPDATE_FLAGS_COUNT = 5;
        private bool[] _matrixUpdateFlags;
        private const int UPDATE_SCALE = 0,
                          UPDATE_ROTX = 1,
                          UPDATE_ROTY = 2,
                          UPDATE_ROTZ = 3,
                          UPDATE_POS = 4;

        private Vector3 _origin;
        public  Vector3 Origin { get => _origin; }

        private bool _isOffCenter;
        public bool IsOffCenter { get => _isOffCenter; }

        private Vector3 _position;
        public  Vector2 PositionV2  { get => _position.Xy; }    //gets 2D (xy) parts of the 3D position vector | location in the rendering space
        public  Vector3 Position    { get => _position; set { _position = value; _matrixUpdateFlags[UPDATE_POS] = true; } }   //gets the original 3D vector
        public  Vector4 PositionV4  { get => new Vector4(_position); }  //returns a 4D position vector constructed from the 3D position vector

        private float _rotX, _rotY, _rotZ;
        public  float RotationX { get => _rotX; set { _rotX = value; _matrixUpdateFlags[UPDATE_ROTX] = true; } }
        public  float RotationY { get => _rotY; set { _rotY = value; _matrixUpdateFlags[UPDATE_ROTY] = true; } }
        public  float RotationZ { get => _rotZ; set { _rotZ = value; _matrixUpdateFlags[UPDATE_ROTZ] = true; } }

        private Vector3 _scale;
        public  Vector3 Scale   { get => _scale; set { _scale = value; _matrixUpdateFlags[UPDATE_SCALE] = true; } } //property for scale of the object

        //Transformation Matrix Properties
        private Matrix4 _translationMat4, _translationOriginMat4,
                        _rotXMat4, _rotYMat4, _rotZMat4,
                        _scaleMat4;
        private Matrix4 _transform;



        public GameObject(int objectID, int renderID, int renderLayer, ColoredVertex[] vertices, uint[] indices)
        {
            _objectID = objectID;

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

            _matrixUpdateFlags = new bool[UPDATE_FLAGS_COUNT];

            _position = new Vector3(0.0f, 0.0f, 0.0f);
            _rotX     = 0.0f;
            _rotY     = 0.0f;
            _rotZ     = 0.0f;
            _scale    = new Vector3(1.0f, 1.0f, 1.0f);

            _origin = _position * -1;

            _translationMat4 = Matrix4.Identity;
            _rotXMat4        = Matrix4.Identity;
            _rotYMat4        = Matrix4.Identity;
            _rotZMat4        = Matrix4.Identity;
            _scaleMat4       = Matrix4.Identity;

            _translationOriginMat4 = Matrix4.Identity;
            _transform = Matrix4.Identity;
        }

        public GameObject(int objectID, int renderID, int renderLayer, TexturedVertex[] vertices, uint[] indices)
        {
            _objectID = objectID;

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

            _matrixUpdateFlags = new bool[UPDATE_FLAGS_COUNT];

            _position = new Vector3(0.0f, 0.0f, 0.0f);
            _rotX = 0.0f;
            _rotY = 0.0f;
            _rotZ = 0.0f;
            _scale = new Vector3(1.0f, 1.0f, 1.0f);

            _origin = _position * -1;
            if (_position.LengthSquared > 0)
                _isOffCenter = true;
            else
                _isOffCenter = false;

            _translationMat4 = Matrix4.Identity;
            _rotXMat4 = Matrix4.Identity;
            _rotYMat4 = Matrix4.Identity;
            _rotZMat4 = Matrix4.Identity;
            _scaleMat4 = Matrix4.Identity;

            _translationOriginMat4 = Matrix4.Identity;
            _transform = Matrix4.Identity;
        }

        public GameObject(int objectID, int renderID, int renderLayer,
                    ColoredVertex[] vertices, uint[] indices,
                    Vector3 position, float rotX, float rotY, float rotZ, Vector3 scale)
        {
            _objectID = objectID;

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

            _matrixUpdateFlags = new bool[UPDATE_FLAGS_COUNT];
            for (int i = 0; i < UPDATE_FLAGS_COUNT; i++)
                _matrixUpdateFlags[i] = true;

            _position = position;
            _rotX = rotX;
            _rotY = rotY;
            _rotZ = rotZ;
            _scale = scale;

            _origin = _position * -1;
            if (_position.LengthSquared > 0)
                _isOffCenter = true;
            else
                _isOffCenter = false;

            _translationMat4 = Matrix4.Identity;
            _rotXMat4 = Matrix4.Identity;
            _rotYMat4 = Matrix4.Identity;
            _rotZMat4 = Matrix4.Identity;
            _scaleMat4 = Matrix4.Identity;

            _translationOriginMat4 = Matrix4.Identity;
            _transform = Matrix4.Identity;
        }

        public GameObject(int objectID, int renderID, int renderLayer,
                            TexturedVertex[] vertices, uint[] indices, 
                            Vector3 position, float rotX, float rotY, float rotZ, Vector3 scale)
        {
            _objectID = objectID;

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

            _matrixUpdateFlags = new bool[UPDATE_FLAGS_COUNT];
            for (int i = 0; i < UPDATE_FLAGS_COUNT; i++)
                _matrixUpdateFlags[i] = true;

            _position = position;
            _rotX     = rotX;
            _rotY     = rotY;
            _rotZ     = rotZ;
            _scale    = scale;

            _origin = _position * -1;
            if (_position.LengthSquared > 0)
                _isOffCenter = true;
            else
                _isOffCenter = false;

            _translationMat4 = Matrix4.Identity;
            _rotXMat4        = Matrix4.Identity;
            _rotYMat4        = Matrix4.Identity;
            _rotZMat4        = Matrix4.Identity;
            _scaleMat4       = Matrix4.Identity;

            _translationOriginMat4 = Matrix4.Identity;
            _transform = Matrix4.Identity;
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

        public void Update()
        {
            RotationZ -= 0.01f;

            UpdateMatrices();
        }

        //THIS IS A TEMPORARY ALGORITHM
        //I'm going to add a system that keeps track of which values are not their defaults by using a set of flags and only multiplying those matrices that are needed
        //Since martix multiplication is expensive, it's wise to minize their multiplications
        public void UpdateMatrices()
        {
            bool didUpdate = false;
            for (int i=0; i<UPDATE_FLAGS_COUNT; i++)
            {
                if (_matrixUpdateFlags[i])
                {
                    didUpdate = true;
                    break;
                }
            }

            if (_matrixUpdateFlags[UPDATE_SCALE]) { Matrix4.CreateScale(ref _scale, out _scaleMat4); _matrixUpdateFlags[UPDATE_SCALE] = false; }

            if (_matrixUpdateFlags[UPDATE_ROTZ]) { Matrix4.CreateRotationZ(_rotZ, out _rotZMat4); _matrixUpdateFlags[UPDATE_ROTZ] = false; }
            if (_matrixUpdateFlags[UPDATE_ROTY]) { Matrix4.CreateRotationY(_rotY, out _rotYMat4); _matrixUpdateFlags[UPDATE_ROTY] = false; }
            if (_matrixUpdateFlags[UPDATE_ROTX]) { Matrix4.CreateRotationX(_rotX, out _rotXMat4); _matrixUpdateFlags[UPDATE_ROTX] = false; }

            if (_matrixUpdateFlags[UPDATE_POS])
            {
                _origin = _position * -1;
                Matrix4.CreateTranslation(ref _position, out _translationMat4);
                Matrix4.CreateTranslation(ref _origin, out _translationOriginMat4);
                _matrixUpdateFlags[UPDATE_POS] = false;
            }

            if (didUpdate)
                _transform = _translationOriginMat4 * _scaleMat4 * _rotZMat4 * _translationMat4 * MainWindow.ORTHO_MATRIX;
            
        }

        public void Render()
        {
            Update(); //This is a temporary placement

            int data = 0;
            GL.GetInteger(GetPName.CurrentProgram, out data);  //gets the currently active program id
            if(data != _activeProgramID)  //if program id == the program that is to be used by this object then continue
                GL.UseProgram(_activeProgramID); //use the program as determined by this class
            GL.BindVertexArray(_VAO);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _IBO);
            GL.UniformMatrix4(2, false, ref _transform);
            GL.DrawElements(BeginMode.Triangles, IndexCount, DrawElementsType.UnsignedInt, 0);
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


