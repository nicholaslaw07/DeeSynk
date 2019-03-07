using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeeSynk.Core.Components.Types.Render
{
    public class ComponentModel : IComponent
    {
        public int BitMaskID => (int)Component.MODEL;

        private bool _hasModelData;
        public bool HasModelData { get => _hasModelData; }

        private Vector4[] _vertices;
        public Vector4[] Vertices { get => _vertices; }
        public ref Vector4[] VerticesByRef { get => ref _vertices; }

        private int _vertexCount;
        public int VertexCount { get => _vertexCount; }

        private uint[] _indices;        //only used if drawing via elements
        public uint[] Indices { get => _indices; }
        public ref uint[] IndicesByRef { get => ref _indices; }

        private int _indexCount;
        public int IndexCount { get => _indexCount; }

        private bool _isDrawnUsingIndices;
        public bool IsDrawnUsingIndices { get => _isDrawnUsingIndices; }

        //private bool _isDataDynamic;
        //public bool  IsDataDynamic { get => _isDataDynamic; }

        public ComponentModel()
        {
            _hasModelData = false;

            _isDrawnUsingIndices = false;
        }

        public ComponentModel(Vector4[] vertices)
        {
            _hasModelData = true;

            _vertices = vertices;
            _vertexCount = _vertices.Length;
            _indices = new uint[0];
            _indexCount = 0;

            _isDrawnUsingIndices = false;
        }

        public ComponentModel(Vector4[] vertices, uint[] indices)
        {
            _hasModelData = true;

            _vertices = vertices;
            _vertexCount = _vertices.Length;
            _indices = indices;
            _indexCount = _indices.Length;

            _isDrawnUsingIndices = true;
        }

        /// <summary>
        /// Constructs a rectangle using the width and height provided.
        /// </summary>
        /// <param name="width">Width of the rectangle.</param>
        /// <param name="height">Height of the rectangle.</param>
        public ComponentModel(float width, float height, bool isTextured)
        {
            if (isTextured)
            {
                _vertices = new Vector4[6];
                _vertices[0] = new Vector4(-width, -height, 0.0f, 1.0f);
                _vertices[1] = new Vector4(width, -height, 0.0f, 1.0f);
                _vertices[2] = new Vector4(width, height, 0.0f, 1.0f);
                _vertices[3] = new Vector4(width, height, 0.0f, 1.0f);
                _vertices[4] = new Vector4(-width, height, 0.0f, 1.0f);
                _vertices[5] = new Vector4(-width, -height, 0.0f, 1.0f);

                _vertexCount = _vertices.Length;

                _indices = new uint[6];
                _indices[0] = 0; _indices[1] = 1; _indices[2] = 2; _indices[3] = 3; _indices[4] = 4; _indices[5] = 5;

                _indexCount = _indices.Length;
            }
            else
            {
                _vertices = new Vector4[4];
                _vertices[0] = new Vector4(-width, -height, 0.0f, 1.0f);
                _vertices[1] = new Vector4(width, -height, 0.0f, 1.0f);
                _vertices[2] = new Vector4(width, height, 0.0f, 1.0f);
                _vertices[3] = new Vector4(width, height, 0.0f, 1.0f);
                _vertices[4] = new Vector4(-width, height, 0.0f, 1.0f);
                _vertices[5] = new Vector4(-width, -height, 0.0f, 1.0f);

                _vertexCount = _vertices.Length;

                _indices = new uint[6];
                _indices[0] = 0; _indices[1] = 1; _indices[2] = 2; _indices[3] = 2; _indices[4] = 3; _indices[5] = 0;

                _indexCount = _indices.Length;
            }
        }

        /// <summary>
        /// Constructs a rectangle using the width and height provided with a specified offset from the center.
        /// </summary>
        /// <param name="width">Width of the rectangle.</param>
        /// <param name="height">Height of the rectangle.</param>
        /// <param name="offsetX">Offset of the center point of the rectangle.</param>
        /// <param name="offsetY">Offset of the center point of the rectangle.</param>
        public ComponentModel(float width, float height, float offsetX, float offsetY, bool isTextured)
        {
            if (isTextured)
            {
                _vertices = new Vector4[6];
                _vertices[0] = new Vector4(offsetX - width, offsetY - height, 0.0f, 1.0f);
                _vertices[1] = new Vector4(offsetX + width, offsetY - height, 0.0f, 1.0f);
                _vertices[2] = new Vector4(offsetX + width, offsetY + height, 0.0f, 1.0f);
                _vertices[3] = new Vector4(offsetX - width, offsetY + height, 0.0f, 1.0f);
                _vertices[4] = new Vector4(offsetX - width, offsetY - height, 0.0f, 1.0f);
                _vertices[5] = new Vector4(offsetX + width, offsetY + height, 0.0f, 1.0f);

                _vertexCount = _vertices.Length;

                _indices = new uint[6];
                _indices[0] = 0; _indices[1] = 1; _indices[2] = 2; _indices[3] = 3; _indices[4] = 4; _indices[5] = 5;

                _indexCount = _indices.Length;
            }
            else
            {
                _vertices = new Vector4[4];
                _vertices[0] = new Vector4(offsetX - width, offsetY - height, 0.0f, 1.0f);
                _vertices[1] = new Vector4(offsetX + width, offsetY - height, 0.0f, 1.0f);
                _vertices[2] = new Vector4(offsetX + width, offsetY + height, 0.0f, 1.0f);
                _vertices[3] = new Vector4(offsetX - width, offsetY + height, 0.0f, 1.0f);

                _vertexCount = _vertices.Length;

                _indices = new uint[6];
                _indices[0] = 0; _indices[1] = 1; _indices[2] = 2; _indices[3] = 2; _indices[4] = 3; _indices[5] = 0;

                _indexCount = _indices.Length;
            }
        }

        /// <summary>
        /// Used to either initialize model data if it is not already present (say it used the default constructor).
        /// </summary>
        /// <param name="vertices"></param>
        /// <param name="indices"></param>
        public void SetModelData(ref Vector4[] vertices, ref uint[] indices)
        {
            _vertices = vertices;
            _indices = indices;
        }

        public void Update(float time)
        {
            //throw new NotImplementedException();
        }
    }
}
