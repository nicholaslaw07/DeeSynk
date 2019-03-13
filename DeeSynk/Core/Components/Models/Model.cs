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

            _normalCount = _vertexCount;
            _normals = new Vector3[_vertexCount];
            {
                int[] counts = new int[_vertexCount];
                for(int i=0; i< vertexIndices.Length / 3 + vertexIndices.Length % 3; i++)
                {
                    Vector3 p1 = _vertices[_vertexIndices[i * 3]];
                    Vector3 p2 = _vertices[_vertexIndices[i * 3 + 1]];
                    Vector3 p3 = _vertices[_vertexIndices[i * 3 + 2]];

                    Vector3 U = p2 - p1;
                    Vector3 V = p3 - p1;

                    float Nx = (U.Y * V.Z) - (U.Z * V.Y);
                    float Ny = (U.Z * V.X) - (U.X * V.Z);
                    float Nz = (U.X * V.Y) - (U.Y * V.X);

                    Vector3 normal = new Vector3(Nx, Ny, Nz);

                    normal.Normalize();

                    _normals[_vertexIndices[i * 3]] += normal;
                    _normals[_vertexIndices[i * 3 + 1]] += normal;
                    _normals[_vertexIndices[i * 3 + 2]] += normal;

                    counts[_vertexIndices[i * 3]]++;
                    counts[_vertexIndices[i * 3 + 1]]++;
                    counts[_vertexIndices[i * 3 + 2]]++;
                }

                for(int i=0; i<_normals.Length; i++)
                {
                    Vector3 n = _normals[i];
                    n.Normalize();
                    _normals[i] = n;
                }
            }

            _normalIndices = _vertexIndices;

            Random r = new Random();

            _colors = new Color4[_vertexCount];
            for (int i = 0; i < _vertexCount; i++)
            {
                _colors[i] = Color4.CornflowerBlue;//new Color4((byte)r.Next(0, 255), (byte)r.Next(0, 255), (byte)r.Next(0, 255), 255);
            }

            testLightComputations();
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

        public void testLightComputations()
        {
            Vector3 light = new Vector3(200f, 200f, 200f);

            Vector3 iA = new Vector3(1.0f, 1.0f, 1.0f);
            Vector3 iD = new Vector3(1.0f, 1.0f, 1.0f);
            Vector3 iS = new Vector3(1.0f, 1.0f, 1.0f);

            float kA = 1.0f;
            float kD = 2.0f;
            float kS = 3.0f;

            float a = 2;

            for (int i=0; i<_vertices.Length; i++)
            {
                Vector3 Lm = (light - _vertices[i]); Lm.Normalize();
                Vector3 N = _normals[i]; N.Normalize();
                Vector3 Rm = 2 * (Vector3.Dot(Lm, N)) * N - Lm; Rm.Normalize();
                Vector3 V  = new Vector3(-200f, 200f, 200f); V.Normalize();

                Vector3 Ip = kA * iA + (kD * Vector3.Dot(Lm, N) * iD + kS * Vector3.Dot(Rm, V)* iS);
                int x = 0;
            }
        }
    }
}
