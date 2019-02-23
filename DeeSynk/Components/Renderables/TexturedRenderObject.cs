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
        protected string _activeTextureReferenceName;  //IMPORTANT: Use blanched almond somewhere
        protected int _activeTextureID;

        public TexturedRenderObject(int renderID, int renderLayer, TexturedVertex[] vertices, int[] indices) : base(renderID, renderLayer)
        {
            _vertices = vertices;
            _vertexCount = _vertices.Length;

            _indices = indices;
            _indexCount = _indices.Length;
        }

        public TexturedRenderObject(int renderID, int renderLayer, Vector3 position, float rotX, float rotY, float rotZ, Vector3 scale, TexturedVertex[] vertices, int[] indices) : base(renderID, renderLayer, position, rotX, rotY, rotZ, scale)
        {
            _vertices = vertices;
            _vertexCount = _vertices.Length;

            _indices = indices;
            _indexCount = _indices.Length;
        }

        public abstract TexturedRenderObject AddTextureIDs();
    }
}
