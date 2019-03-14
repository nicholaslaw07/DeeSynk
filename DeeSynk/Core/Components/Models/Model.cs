using DeeSynk.Core.Components.Types.Render;
using DeeSynk.Core.Managers;
using OpenTK;
using OpenTK.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DeeSynk.Core.Components.Models
{
    public enum ModelTemplates : int
    {
        NONE = 0,
        TemplatePlaneXZ = 1,
        TemplatePlaneXY = 2,
        TemplatePlaneYZ = 3
    }

    public class Model
    {
        private int _vertexCount;
        private Vector3[] _vertices;
        public Vector3[] Vertices { get => _vertices; }

        private Color4[] _colors;
        public Color4[] Colors { get => _colors; }

        private Vector2[] _uvs;
        public Vector2[] UVS { get => _uvs; }

        private int _normalCount;
        private Vector3[] _normals;
        public Vector3[] Normals { get => _normals; }

        private uint[] _vertexIndices;
        public uint[] VertexIndices { get => _vertexIndices; }

        private uint[] _normalIndices;
        public uint[] NormalIndices { get => _normalIndices; }

        public Model(Vector3[] vertices, uint[] vertexIndices, bool recomputeCenter, bool computeNormals)
        {
            _vertexCount = vertices.Length;
            _vertices = new Vector3[_vertexCount];
            for (int idx = 0; idx < _vertexCount; idx++)
                _vertices[idx] = vertices[idx];

            if (recomputeCenter)
            {
                Vector3 offsetAvg = Vector3.Zero;
                for (int idx = 0; idx < _vertexCount; idx++)
                    offsetAvg += _vertices[idx];

                offsetAvg /= (float)_vertexCount;

                for (int idx = 0; idx < _vertexCount; idx++)
                    _vertices[idx] -= offsetAvg;
            }

            _vertexIndices = vertexIndices;

            if (computeNormals)
            {
                _normalCount = _vertexCount;
                _normals = new Vector3[_vertexCount];
                {
                    int[] counts = new int[_vertexCount];
                    for (int i = 0; i < vertexIndices.Length / 3 + vertexIndices.Length % 3; i++)
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

                    for (int i = 0; i < _normals.Length; i++)
                    {
                        Vector3 n = _normals[i];
                        n.Normalize();
                        _normals[i] = n;
                    }
                }
            }

            Random r = new Random();

            _colors = new Color4[_vertexCount];
            for (int i = 0; i < _vertexCount; i++)
            {
                _colors[i] = Color4.CornflowerBlue;
            }
        }

        public Model(Vector3[] vertices, Vector2[] uvs, uint[] vertexIndices, bool recomputeCenter, bool computeNormals)
        {
            _vertexCount = vertices.Length;
            _vertices = new Vector3[_vertexCount];
            for (int idx = 0; idx < _vertexCount; idx++)
                _vertices[idx] = vertices[idx];

            if (recomputeCenter)
            {
                Vector3 offsetAvg = Vector3.Zero;
                for (int idx = 0; idx < _vertexCount; idx++)
                    offsetAvg += _vertices[idx];

                offsetAvg /= (float)_vertexCount;

                for (int idx = 0; idx < _vertexCount; idx++)
                    _vertices[idx] -= offsetAvg;
            }

            _vertexIndices = vertexIndices;

            if (computeNormals)
            {
                _normalCount = _vertexCount;
                _normals = new Vector3[_vertexCount];
                {
                    int[] counts = new int[_vertexCount];
                    for (int i = 0; i < vertexIndices.Length / 3 + vertexIndices.Length % 3; i++)
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

                    for (int i = 0; i < _normals.Length; i++)
                    {
                        Vector3 n = _normals[i];
                        n.Normalize();
                        _normals[i] = n;
                    }
                }

                _normalIndices = _vertexIndices;
            }

            _uvs = new Vector2[uvs.Length];
            for (int idx = 0; idx < uvs.Length; idx++)
                _uvs[idx] = uvs[idx];
        }

        public Model(Vector3[] vertices, Vector2[] uvs, bool recomputeCenter, bool computeNormals)
        {
            _vertexCount = vertices.Length;
            _vertices = new Vector3[_vertexCount];
            for (int idx = 0; idx < _vertexCount; idx++)
                _vertices[idx] = vertices[idx];

            if (recomputeCenter)
            {
                Vector3 offsetAvg = Vector3.Zero;
                for (int idx = 0; idx < _vertexCount; idx++)
                    offsetAvg += _vertices[idx];

                offsetAvg /= (float)_vertexCount;

                for (int idx = 0; idx < _vertexCount; idx++)
                    _vertices[idx] -= offsetAvg;
            }

            /*
            if (computeNormals)
            {
                _normalCount = _vertexCount;
                _normals = new Vector3[_vertexCount];
                {
                    int[] counts = new int[_vertexCount];
                    for (int i = 0; i < vertexIndices.Length / 3 + vertexIndices.Length % 3; i++)
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

                    for (int i = 0; i < _normals.Length; i++)
                    {
                        Vector3 n = _normals[i];
                        n.Normalize();
                        _normals[i] = n;
                    }
                }

                _normalIndices = _vertexIndices;
            }
            */
            _uvs = new Vector2[uvs.Length];
            for (int idx = 0; idx < uvs.Length; idx++)
                _uvs[idx] = uvs[idx];
        }

        public Model(Vector3[] vertices, Color4[] colors, uint[] vertexIndices, bool recomputeCenter, bool computeNormals)
        {
            _vertexCount = vertices.Length;
            _vertices = new Vector3[_vertexCount];
            for (int idx = 0; idx < _vertexCount; idx++)
                _vertices[idx] = vertices[idx];

            if (recomputeCenter)
            {
                Vector3 offsetAvg = Vector3.Zero;
                for (int idx = 0; idx < _vertexCount; idx++)
                    offsetAvg += _vertices[idx];

                offsetAvg /= (float)_vertexCount;

                for (int idx = 0; idx < _vertexCount; idx++)
                    _vertices[idx] -= offsetAvg;
            }

            _vertexIndices = new uint[vertexIndices.Length];
            _vertexIndices = vertexIndices;

            if (computeNormals)
            {
                _normalCount = _vertexCount;
                _normals = new Vector3[_vertexCount];
                {
                    int[] counts = new int[_vertexCount];
                    for (int i = 0; i < vertexIndices.Length / 3 + vertexIndices.Length % 3; i++)
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

                    for (int i = 0; i < _normals.Length; i++)
                    {
                        Vector3 n = _normals[i];
                        n.Normalize();
                        _normals[i] = n;
                    }
                }

                _normalIndices = _vertexIndices;
            }

            _colors = new Color4[colors.Length];
            for (int idx = 0; idx < colors.Length; idx++)
                _colors[idx] = colors[idx];
        }

        public Model(Vector3[] vertices, Color4[] colors, bool recomputeCenter, bool computeNormals)
        {
            _vertexCount = vertices.Length;
            _vertices = new Vector3[_vertexCount];
            for (int idx = 0; idx < _vertexCount; idx++)
                _vertices[idx] = vertices[idx];

            if (recomputeCenter)
            {
                Vector3 offsetAvg = Vector3.Zero;
                for (int idx = 0; idx < _vertexCount; idx++)
                    offsetAvg += _vertices[idx];

                offsetAvg /= (float)_vertexCount;

                for (int idx = 0; idx < _vertexCount; idx++)
                    _vertices[idx] -= offsetAvg;
            }

            if (computeNormals)
            {
                _normalCount = _vertexCount;
                _normals = new Vector3[_vertexCount];
                {
                    int[] counts = new int[_vertexCount];
                    for (int i = 0; i < _vertexCount / 3; i++)
                    {
                        Vector3 p1 = _vertices[i * 3];
                        Vector3 p2 = _vertices[i * 3 + 1];
                        Vector3 p3 = _vertices[i * 3 + 2];

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

                    for (int i = 0; i < _normals.Length; i++)
                    {
                        Vector3 n = _normals[i];
                        n.Normalize();
                        _normals[i] = n;
                    }
                }

                _normalIndices = _vertexIndices;
            }

            _colors = new Color4[colors.Length];
            for (int idx = 0; idx < colors.Length; idx++)
                _colors[idx] = colors[idx];
        }

        public Model(ref Vector3[] vertices, ref Vector3[] normals, ref uint[] vertexIndices, ref uint[] normalIndices) //fix
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


        public static string GetTemplateName(ModelTemplates template)
        {
            switch (template)
            {
                case (ModelTemplates.TemplatePlaneXZ): return "TemplatePlaneXZ";
                case (ModelTemplates.TemplatePlaneXY): return "TemplatePlaneXY";
                case (ModelTemplates.TemplatePlaneYZ): return "TemplatePlaneYZ";
            }

            return "NONE";
        }

        /// <summary>
        /// Constructs a model object for a plane of dimensions specified by the model component.  Created in the XZ plane.
        /// </summary>
        /// <param name="modelComp">Model component that holds data for the creation of this object.</param>
        /// <returns></returns>
        public static Model CreateTemplatePlaneXZ(ref ComponentModelStatic modelComp)
        {
            //for now, will not construct normals

            if (modelComp.ModelProperties.HasFlag(ModelProperties.VERTICES))
            {
                Vector3[] vertices;
                Color4[] colors;
                Vector2[] uvs;
                uint[] elements;

                //no error checking since this should almost always be the case, if not we compensate for it by having defaults (1.0f)
                bool hasDimensionFlag = modelComp.ConstructionFlags.HasFlag(ConstructionFlags.VECTOR3_DIMENSIONS);
                float[] WH = modelComp.GetConstructionParameter(ConstructionFlags.VECTOR3_DIMENSIONS); 

                float width  = (hasDimensionFlag) ? WH[0] : 1.0f;  //X
                float height = (hasDimensionFlag) ? WH[2] : 1.0f;  //Z

                if (modelComp.ModelProperties.HasFlag(ModelProperties.FACE_ELEMENTS))
                {
                    elements = new uint[6];

                    if (modelComp.ModelProperties.HasFlag(ModelProperties.UVS))
                    {
                        vertices = new Vector3[6];
                        uvs = new Vector2[6];
                        //Add vertices
                        {
                            vertices[0] = new Vector3(-width, 0, -height);
                            vertices[1] = new Vector3( width, 0, -height);
                            vertices[2] = new Vector3( width, 0,  height);
                            vertices[3] = new Vector3( width, 0,  height);
                            vertices[4] = new Vector3(-width, 0,  height);
                            vertices[5] = new Vector3(-width, 0, -height);
                        }
                        //Add elements
                        {
                            elements[0] = 0;
                            elements[1] = 1;
                            elements[2] = 2;
                            elements[3] = 3;
                            elements[4] = 4;
                            elements[5] = 5;
                        }

                        float offsetU = 0.0f;
                        float offsetV = 0.0f;
                        if (modelComp.ConstructionFlags.HasFlag(ConstructionFlags.VECTOR2_UV_OFFSET))
                        {
                            float[] UVOffsetRaw = modelComp.GetConstructionParameter(ConstructionFlags.VECTOR2_UV_OFFSET);
                            offsetU = UVOffsetRaw[0];
                            offsetV = UVOffsetRaw[1];
                        }

                        float scaleU = 1.0f;
                        float scaleV = 1.0f;
                        if (modelComp.ConstructionFlags.HasFlag(ConstructionFlags.VECTOR2_UV_SCALE))
                        {
                            float[] UVScaleRaw = modelComp.GetConstructionParameter(ConstructionFlags.VECTOR2_UV_SCALE);
                            scaleU = UVScaleRaw[0];
                            scaleV = UVScaleRaw[1];
                        }

                        //Add uv data, offset on the UV is not affected by the scaling (dimensions)
                        {
                            uvs[0] = new Vector2(offsetU         , offsetV         ); //0, 0
                            uvs[1] = new Vector2(offsetU + scaleU, offsetV         ); //1, 0
                            uvs[2] = new Vector2(offsetU + scaleU, offsetV + scaleV); //1, 1
                            uvs[3] = new Vector2(offsetU + scaleU, offsetV + scaleV); //1, 1
                            uvs[4] = new Vector2(offsetU         , offsetV + scaleV); //1, 0
                            uvs[5] = new Vector2(offsetU         , offsetV         ); //0, 0
                        }

                        return new Model(vertices, uvs, elements, false, false);
                    }
                    else
                    {
                        vertices = new Vector3[4];
                        colors = new Color4[4];
                        elements = new uint[6];
                        //Add vertices
                        {
                            vertices[0] = new Vector3(-width, 0, -height);
                            vertices[1] = new Vector3(width, 0, -height);
                            vertices[2] = new Vector3(width, 0, height);
                            vertices[3] = new Vector3(-width, 0, height);
                        }
                        //Add elements
                        {
                            elements[0] = 0;
                            elements[1] = 1;
                            elements[2] = 2;
                            elements[3] = 2;
                            elements[4] = 3;
                            elements[5] = 0;
                        }
                        //Add colors
                        if (modelComp.ModelProperties.HasFlag(ModelProperties.COLORS))
                        {

                            float[] ColorRaw = modelComp.GetConstructionParameter(ConstructionFlags.COLOR4_COLOR);
                            Color4 color = new Color4(ColorRaw[0], ColorRaw[1], ColorRaw[2], ColorRaw[3]);         

                            for (int idx = 0; idx < colors.Length; idx++)
                                colors[idx] = color;

                        }
                        //the default return if it doesn't have the property for some reason
                        else
                        {
                            colors[0] = Color4.HotPink;
                            colors[1] = Color4.Black;
                            colors[2] = Color4.HotPink;
                            colors[3] = Color4.Black;
                        }

                        return new Model(vertices, colors, elements, false, false);
                    }
                }
                else
                {
                    vertices = new Vector3[6];
                    //Add vertices
                    {
                        vertices[0] = new Vector3(-width, 0, -height);
                        vertices[1] = new Vector3(width, 0, -height);
                        vertices[2] = new Vector3(width, 0, height);
                        vertices[3] = new Vector3(width, 0, height);
                        vertices[4] = new Vector3(-width, 0, height);
                        vertices[5] = new Vector3(-width, 0, -height);
                    }
                    //Add UVs
                    if (modelComp.ModelProperties.HasFlag(ModelProperties.UVS))
                    {
                        uvs = new Vector2[6];

                        float offsetU = 0.0f;
                        float offsetV = 0.0f;
                        if (modelComp.ConstructionFlags.HasFlag(ConstructionFlags.VECTOR2_UV_OFFSET))
                        {
                            float[] UVOffsetRaw = modelComp.GetConstructionParameter(ConstructionFlags.VECTOR2_UV_OFFSET);
                            offsetU = UVOffsetRaw[0];
                            offsetV = UVOffsetRaw[1];
                        }

                        float scaleU = 1.0f;
                        float scaleV = 1.0f;
                        if (modelComp.ConstructionFlags.HasFlag(ConstructionFlags.VECTOR2_UV_SCALE))
                        {
                            float[] UVScaleRaw = modelComp.GetConstructionParameter(ConstructionFlags.VECTOR2_UV_SCALE);
                            scaleU = UVScaleRaw[0];
                            scaleV = UVScaleRaw[1];
                        }

                        //Add uv data, offset on the UV is not affected by the scaling (dimensions)
                        {
                            uvs[0] = new Vector2(offsetU, offsetV); //0, 0
                            uvs[1] = new Vector2(offsetU + scaleU, offsetV); //1, 0
                            uvs[2] = new Vector2(offsetU + scaleU, offsetV + scaleV); //1, 1
                            uvs[3] = new Vector2(offsetU + scaleU, offsetV + scaleV); //1, 1
                            uvs[4] = new Vector2(offsetU, offsetV + scaleV); //1, 0
                            uvs[5] = new Vector2(offsetU, offsetV); //0, 0
                        }

                        return new Model(vertices, uvs, false, false);
                    }
                    //Or Add colors
                    else
                    {
                        colors = new Color4[6];
                        if (modelComp.ModelProperties.HasFlag(ModelProperties.COLORS))
                        {
                            float[] ColorRaw = modelComp.GetConstructionParameter(ConstructionFlags.COLOR4_COLOR);
                            Color4 color = new Color4(ColorRaw[0], ColorRaw[1], ColorRaw[2], ColorRaw[3]);
                            for (int idx = 0; idx < colors.Length; idx++)
                                colors[idx] = color;
                        }
                        else
                        {
                            colors[0] = Color4.LightGreen;
                            colors[1] = Color4.Black;
                            colors[2] = Color4.LightGreen;
                            colors[3] = Color4.LightGreen;
                            colors[4] = Color4.Black;
                            colors[5] = Color4.LightGreen;
                        }

                        return new Model(vertices, colors, false, false);
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Constructs a model object for a plane of dimensions specified by the model component.  Created in the XY plane.
        /// </summary>
        /// <param name="modelComp">Model component that holds data for the creation of this object.</param>
        /// <returns></returns>
        public static Model CreateTemplatePlaneXY(ref ComponentModelStatic modelComp)
        {
            //for now, will not construct normals

            if (modelComp.ModelProperties.HasFlag(ModelProperties.VERTICES))
            {
                Vector3[] vertices;
                Color4[] colors;
                Vector2[] uvs;
                uint[] elements;

                //no error checking since this should almost always be the case, if not we compensate for it by having defaults (1.0f)
                bool hasDimensionFlag = modelComp.ConstructionFlags.HasFlag(ConstructionFlags.VECTOR3_DIMENSIONS);
                float[] WH = modelComp.GetConstructionParameter(ConstructionFlags.VECTOR3_DIMENSIONS);

                float width = (hasDimensionFlag) ? WH[0] : 1.0f;  //X
                float height = (hasDimensionFlag) ? WH[1] : 1.0f;  //Y

                if (modelComp.ModelProperties.HasFlag(ModelProperties.FACE_ELEMENTS))
                {
                    elements = new uint[6];

                    if (modelComp.ModelProperties.HasFlag(ModelProperties.UVS))
                    {
                        vertices = new Vector3[6];
                        uvs = new Vector2[6];
                        //Add vertices
                        {
                            vertices[0] = new Vector3(-width, -height, 0);
                            vertices[1] = new Vector3( width, -height, 0);
                            vertices[2] = new Vector3( width,  height, 0);
                            vertices[3] = new Vector3( width,  height, 0);
                            vertices[4] = new Vector3(-width,  height, 0);
                            vertices[5] = new Vector3(-width, -height, 0);
                        }
                        //Add elements
                        {
                            elements[0] = 0;
                            elements[1] = 1;
                            elements[2] = 2;
                            elements[3] = 3;
                            elements[4] = 4;
                            elements[5] = 5;
                        }

                        float offsetU = 0.0f;
                        float offsetV = 0.0f;
                        if (modelComp.ConstructionFlags.HasFlag(ConstructionFlags.VECTOR2_UV_OFFSET))
                        {
                            float[] UVOffsetRaw = modelComp.GetConstructionParameter(ConstructionFlags.VECTOR2_UV_OFFSET);
                            offsetU = UVOffsetRaw[0];
                            offsetV = UVOffsetRaw[1];
                        }

                        float scaleU = 1.0f;
                        float scaleV = 1.0f;
                        if (modelComp.ConstructionFlags.HasFlag(ConstructionFlags.VECTOR2_UV_SCALE))
                        {
                            float[] UVScaleRaw = modelComp.GetConstructionParameter(ConstructionFlags.VECTOR2_UV_SCALE);
                            scaleU = UVScaleRaw[0];
                            scaleV = UVScaleRaw[1];
                        }

                        //Add uv data, offset on the UV is not affected by the scaling (dimensions)
                        {
                            uvs[0] = new Vector2(offsetU         , offsetV         ); //0, 0
                            uvs[1] = new Vector2(offsetU + scaleU, offsetV         ); //1, 0
                            uvs[2] = new Vector2(offsetU + scaleU, offsetV + scaleV); //1, 1
                            uvs[3] = new Vector2(offsetU + scaleU, offsetV + scaleV); //1, 1
                            uvs[4] = new Vector2(offsetU         , offsetV + scaleV); //1, 0
                            uvs[5] = new Vector2(offsetU         , offsetV         ); //0, 0
                        }

                        return new Model(vertices, uvs, elements, false, false);
                    }
                    else
                    {
                        vertices = new Vector3[4];
                        colors = new Color4[4];
                        elements = new uint[6];
                        //Add vertices
                        {
                            vertices[0] = new Vector3(-width, -height, 0);
                            vertices[1] = new Vector3( width, -height, 0);
                            vertices[2] = new Vector3( width,  height, 0);
                            vertices[3] = new Vector3(-width,  height, 0);
                        }
                        //Add elements
                        {
                            elements[0] = 0;
                            elements[1] = 1;
                            elements[2] = 2;
                            elements[3] = 2;
                            elements[4] = 3;
                            elements[5] = 0;
                        }
                        //Add colors
                        if (modelComp.ModelProperties.HasFlag(ModelProperties.COLORS))
                        {

                            float[] ColorRaw = modelComp.GetConstructionParameter(ConstructionFlags.COLOR4_COLOR);
                            Color4 color = new Color4(ColorRaw[0], ColorRaw[1], ColorRaw[2], ColorRaw[3]);

                            for (int idx = 0; idx < colors.Length; idx++)
                                colors[idx] = color;

                        }
                        //the default return if it doesn't have the property for some reason
                        else
                        {
                            colors[0] = Color4.HotPink;
                            colors[1] = Color4.Black;
                            colors[2] = Color4.HotPink;
                            colors[3] = Color4.Black;
                        }

                        return new Model(vertices, colors, elements, false, false);
                    }
                }
                else
                {
                    vertices = new Vector3[6];
                    //Add vertices
                    {
                        vertices[0] = new Vector3(-width, -height, 0);
                        vertices[1] = new Vector3( width, -height, 0);
                        vertices[2] = new Vector3( width,  height, 0);
                        vertices[3] = new Vector3( width,  height, 0);
                        vertices[4] = new Vector3(-width,  height, 0);
                        vertices[5] = new Vector3(-width, -height, 0);
                    }
                    //Add UVs
                    if (modelComp.ModelProperties.HasFlag(ModelProperties.UVS))
                    {
                        uvs = new Vector2[6];

                        float offsetU = 0.0f;
                        float offsetV = 0.0f;
                        if (modelComp.ConstructionFlags.HasFlag(ConstructionFlags.VECTOR2_UV_OFFSET))
                        {
                            float[] UVOffsetRaw = modelComp.GetConstructionParameter(ConstructionFlags.VECTOR2_UV_OFFSET);
                            offsetU = UVOffsetRaw[0];
                            offsetV = UVOffsetRaw[1];
                        }

                        float scaleU = 1.0f;
                        float scaleV = 1.0f;
                        if (modelComp.ConstructionFlags.HasFlag(ConstructionFlags.VECTOR2_UV_SCALE))
                        {
                            float[] UVScaleRaw = modelComp.GetConstructionParameter(ConstructionFlags.VECTOR2_UV_SCALE);
                            scaleU = UVScaleRaw[0];
                            scaleV = UVScaleRaw[1];
                        }

                        //Add uv data, offset on the UV is not affected by the scaling (dimensions)
                        {
                            uvs[0] = new Vector2(offsetU         , offsetV         ); //0, 0
                            uvs[1] = new Vector2(offsetU + scaleU, offsetV         ); //1, 0
                            uvs[2] = new Vector2(offsetU + scaleU, offsetV + scaleV); //1, 1
                            uvs[3] = new Vector2(offsetU + scaleU, offsetV + scaleV); //1, 1
                            uvs[4] = new Vector2(offsetU         , offsetV + scaleV); //1, 0
                            uvs[5] = new Vector2(offsetU         , offsetV         ); //0, 0
                        }

                        return new Model(vertices, uvs, false, false);
                    }
                    //Or Add colors
                    else
                    {
                        colors = new Color4[6];
                        if (modelComp.ModelProperties.HasFlag(ModelProperties.COLORS))
                        {
                            float[] ColorRaw = modelComp.GetConstructionParameter(ConstructionFlags.COLOR4_COLOR);
                            Color4 color = new Color4(ColorRaw[0], ColorRaw[1], ColorRaw[2], ColorRaw[3]);
                            for (int idx = 0; idx < colors.Length; idx++)
                                colors[idx] = color;
                        }
                        else
                        {
                            colors[0] = Color4.LightGreen;
                            colors[1] = Color4.Black;
                            colors[2] = Color4.LightGreen;
                            colors[3] = Color4.LightGreen;
                            colors[4] = Color4.Black;
                            colors[5] = Color4.LightGreen;
                        }

                        return new Model(vertices, colors, false, false);
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Constructs a model object for a plane of dimensions specified by the model component.  Created in the YZ plane.
        /// </summary>
        /// <param name="modelComp">Model component that holds data for the creation of this object.</param>
        /// <returns></returns>
        public static Model CreateTemplatePlaneYZ(ref ComponentModelStatic modelComp)
        {
            //for now, will not construct normals

            if (modelComp.ModelProperties.HasFlag(ModelProperties.VERTICES))
            {
                Vector3[] vertices;
                Color4[] colors;
                Vector2[] uvs;
                uint[] elements;

                //no error checking since this should almost always be the case, if not we compensate for it by having defaults (1.0f)
                bool hasDimensionFlag = modelComp.ConstructionFlags.HasFlag(ConstructionFlags.VECTOR3_DIMENSIONS);
                float[] WH = modelComp.GetConstructionParameter(ConstructionFlags.VECTOR3_DIMENSIONS);

                float width  = (hasDimensionFlag) ? WH[1] : 1.0f;  //Y
                float height = (hasDimensionFlag) ? WH[2] : 1.0f;  //Z

                if (modelComp.ModelProperties.HasFlag(ModelProperties.FACE_ELEMENTS))
                {
                    elements = new uint[6];

                    if (modelComp.ModelProperties.HasFlag(ModelProperties.UVS))
                    {
                        vertices = new Vector3[6];
                        uvs = new Vector2[6];
                        //Add vertices
                        {
                            vertices[0] = new Vector3(0, -width, -height);
                            vertices[1] = new Vector3(0,  width, -height);
                            vertices[2] = new Vector3(0,  width,  height);
                            vertices[3] = new Vector3(0,  width,  height);
                            vertices[4] = new Vector3(0, -width,  height);
                            vertices[5] = new Vector3(0, -width, -height);
                        }
                        //Add elements
                        {
                            elements[0] = 0;
                            elements[1] = 1;
                            elements[2] = 2;
                            elements[3] = 3;
                            elements[4] = 4;
                            elements[5] = 5;
                        }

                        float offsetU = 0.0f;
                        float offsetV = 0.0f;
                        if (modelComp.ConstructionFlags.HasFlag(ConstructionFlags.VECTOR2_UV_OFFSET))
                        {
                            float[] UVOffsetRaw = modelComp.GetConstructionParameter(ConstructionFlags.VECTOR2_UV_OFFSET);
                            offsetU = UVOffsetRaw[0];
                            offsetV = UVOffsetRaw[1];
                        }

                        float scaleU = 1.0f;
                        float scaleV = 1.0f;
                        if (modelComp.ConstructionFlags.HasFlag(ConstructionFlags.VECTOR2_UV_SCALE))
                        {
                            float[] UVScaleRaw = modelComp.GetConstructionParameter(ConstructionFlags.VECTOR2_UV_SCALE);
                            scaleU = UVScaleRaw[0];
                            scaleV = UVScaleRaw[1];
                        }

                        //Add uv data, offset on the UV is not affected by the scaling (dimensions)
                        {
                            uvs[0] = new Vector2(offsetU         , offsetV         ); //0, 0
                            uvs[1] = new Vector2(offsetU + scaleU, offsetV         ); //1, 0
                            uvs[2] = new Vector2(offsetU + scaleU, offsetV + scaleV); //1, 1
                            uvs[3] = new Vector2(offsetU + scaleU, offsetV + scaleV); //1, 1
                            uvs[4] = new Vector2(offsetU         , offsetV + scaleV); //1, 0
                            uvs[5] = new Vector2(offsetU         , offsetV         ); //0, 0
                        }

                        return new Model(vertices, uvs, elements, false, false);
                    }
                    else
                    {
                        vertices = new Vector3[4];
                        colors = new Color4[4];
                        elements = new uint[6];
                        //Add vertices
                        {
                            vertices[0] = new Vector3(0, -width, -height);
                            vertices[1] = new Vector3(0,  width, -height);
                            vertices[2] = new Vector3(0,  width,  height);
                            vertices[3] = new Vector3(0, -width,  height);
                        }
                        //Add elements
                        {
                            elements[0] = 0;
                            elements[1] = 1;
                            elements[2] = 2;
                            elements[3] = 2;
                            elements[4] = 3;
                            elements[5] = 0;
                        }
                        //Add colors
                        if (modelComp.ModelProperties.HasFlag(ModelProperties.COLORS))
                        {

                            float[] ColorRaw = modelComp.GetConstructionParameter(ConstructionFlags.COLOR4_COLOR);
                            Color4 color = new Color4(ColorRaw[0], ColorRaw[1], ColorRaw[2], ColorRaw[3]);

                            for (int idx = 0; idx < colors.Length; idx++)
                                colors[idx] = color;

                        }
                        //the default return if it doesn't have the property for some reason
                        else
                        {
                            colors[0] = Color4.HotPink;
                            colors[1] = Color4.Black;
                            colors[2] = Color4.HotPink;
                            colors[3] = Color4.Black;
                        }

                        return new Model(vertices, colors, elements, false, false);
                    }
                }
                else
                {
                    vertices = new Vector3[6];
                    //Add vertices
                    {
                        vertices[0] = new Vector3(0, -width, -height);
                        vertices[1] = new Vector3(0,  width, -height);
                        vertices[2] = new Vector3(0,  width,  height);
                        vertices[3] = new Vector3(0,  width,  height);
                        vertices[4] = new Vector3(0, -width,  height);
                        vertices[5] = new Vector3(0, -width, -height);
                    }
                    //Add UVs
                    if (modelComp.ModelProperties.HasFlag(ModelProperties.UVS))
                    {
                        uvs = new Vector2[6];

                        float offsetU = 0.0f;
                        float offsetV = 0.0f;
                        if (modelComp.ConstructionFlags.HasFlag(ConstructionFlags.VECTOR2_UV_OFFSET))
                        {
                            float[] UVOffsetRaw = modelComp.GetConstructionParameter(ConstructionFlags.VECTOR2_UV_OFFSET);
                            offsetU = UVOffsetRaw[0];
                            offsetV = UVOffsetRaw[1];
                        }

                        float scaleU = 1.0f;
                        float scaleV = 1.0f;
                        if (modelComp.ConstructionFlags.HasFlag(ConstructionFlags.VECTOR2_UV_SCALE))
                        {
                            float[] UVScaleRaw = modelComp.GetConstructionParameter(ConstructionFlags.VECTOR2_UV_SCALE);
                            scaleU = UVScaleRaw[0];
                            scaleV = UVScaleRaw[1];
                        }

                        //Add uv data, offset on the UV is not affected by the scaling (dimensions)
                        {
                            uvs[0] = new Vector2(offsetU         , offsetV         ); //0, 0
                            uvs[1] = new Vector2(offsetU + scaleU, offsetV         ); //1, 0
                            uvs[2] = new Vector2(offsetU + scaleU, offsetV + scaleV); //1, 1
                            uvs[3] = new Vector2(offsetU + scaleU, offsetV + scaleV); //1, 1
                            uvs[4] = new Vector2(offsetU         , offsetV + scaleV); //1, 0
                            uvs[5] = new Vector2(offsetU         , offsetV         ); //0, 0
                        }

                        return new Model(vertices, uvs, false, false);
                    }
                    //Or Add colors
                    else
                    {
                        colors = new Color4[6];
                        if (modelComp.ModelProperties.HasFlag(ModelProperties.COLORS))
                        {
                            float[] ColorRaw = modelComp.GetConstructionParameter(ConstructionFlags.COLOR4_COLOR);
                            Color4 color = new Color4(ColorRaw[0], ColorRaw[1], ColorRaw[2], ColorRaw[3]);
                            for (int idx = 0; idx < colors.Length; idx++)
                                colors[idx] = color;
                        }
                        else
                        {
                            colors[0] = Color4.LightGreen;
                            colors[1] = Color4.Black;
                            colors[2] = Color4.LightGreen;
                            colors[3] = Color4.LightGreen;
                            colors[4] = Color4.Black;
                            colors[5] = Color4.LightGreen;
                        }

                        return new Model(vertices, colors, false, false);
                    }
                }
            }
            return null;
        }
    }
}
