using OpenTK;
using OpenTK.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeeSynk.Core.Components.Models
{
    public class Model
    {

        private int _vertexCount;
        private Vector3[] _vertices;
        public Vector3[] Vertices { get => _vertices; }

        private Color4[] _colors;
        public Color4[] Colors { get => _colors; }

        private int _normalCount;
        private Vector3[] _normals;
        public Vector3[] Normals { get => _normals; }

        private uint[] _vertexIndices;
        public uint[] VertexIndices { get => _vertexIndices; }

        private uint[] _normalIndices;
        public uint[] NormalIndices { get => _normalIndices; }

        public Model(Vector3[] vertices, uint[] vertexIndices)
        {
            _vertexCount = vertices.Length;
            _vertices = new Vector3[_vertexCount];
            for (int idx = 0; idx < _vertexCount; idx++)
                _vertices[idx] = vertices[idx];

            _vertexIndices = new uint[vertexIndices.Length];
            _vertexIndices = vertexIndices;

            Random r = new Random();

            _colors = new Color4[_vertexCount];
            for (int i = 0; i < _vertexCount; i++)
            {
                _colors[i] = new Color4((byte)r.Next(64, 120), (byte)r.Next(64, 120), (byte)r.Next(64, 120), 255);
            }
        }

        public Model(ref Vector3[] vertices, ref Vector3[] normals, ref uint[] vertexIndices, ref uint[] normalIndices)
        {
            _vertexCount = vertices.Length;
            _vertices = new Vector3[_vertexCount];
            for (int idx = 0; idx < _vertexCount; idx++)
                _vertices[idx] = vertices[idx];
            
            _normalCount = normals.Length;
            _normals = new Vector3[_normalCount];
            for (int idx = 0; idx < _normalCount; idx++)
                _normals[idx] = normals[idx];

            _vertexIndices = new uint[vertexIndices.Length];
            _vertexIndices = vertexIndices;

            _normalIndices = new uint[normalIndices.Length];
            _normalIndices = normalIndices;

            Random r = new Random();

            _colors = new Color4[_vertexCount];
            for(int i=0; i<_vertexCount; i++)
            {
                _colors[i] = new Color4((byte)r.Next(0, 255), (byte)r.Next(0, 255), (byte)r.Next(0, 255), 255);
            }
        }
    }
}
