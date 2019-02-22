using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace DeeSynk.Components.Renderables
{
    public abstract class TexturedRenderObject : RenderObject
    {
        private bool _initVAO; // is the VAO initialized
        protected bool InitVAO { get => _initVAO; set => _initVAO = value; }  //inside of the get, add a means of checking if the buffer references have been created

        private int _VAO;  //Vertex Array Object integer reference
        private int _VBO;  //Vertex Buffer Object
        private int _IBO;  //Index Buffer Object

        protected int VAO { get => _VAO; }
        protected int VBO { get => _VBO; }
        protected int IBO { get => _IBO; }


        private TexturedVertex[]    _vertices;
        private int                 _vertexCount;
        protected TexturedVertex[]  Vertices    { get => _vertices; }
        protected int               VertexCount { get => _vertexCount; }

        private int[]   _indices;
        private int     _indexCount;
        protected int[] Indices     { get => _indices; }
        protected int   IndexCount  { get => _indexCount; }

        //THESE VARIABLES ARE SUBJECT TO CHANGE
        protected List<string> _textureReferenceNames;  //Same as for Programs but with Textures instead
        protected string _activeTextureReferenceName;
        protected int _activeTextureID;

        public TexturedRenderObject(int renderID, int renderLayer, TexturedVertex[] vertices, int[] indices) : base(renderID, renderLayer)
        {
            _initVAO = false;

            _vertices = vertices;
            _vertexCount = _vertices.Length;

            _indices = indices;
            _indexCount = _indices.Length;
        }

        public TexturedRenderObject(int renderID, int renderLayer, Vector3 position, float rotX, float rotY, float rotZ, Vector3 scale, TexturedVertex[] vertices, int[] indices) : base(renderID, renderLayer, position, rotX, rotY, rotZ, scale)
        {
            _initVAO = false;

            _vertices = vertices;
            _vertexCount = _vertices.Length;

            _indices = indices;
            _indexCount = _indices.Length;
        }

        public abstract TexturedRenderObject AddTextureIDs();
    }
}
