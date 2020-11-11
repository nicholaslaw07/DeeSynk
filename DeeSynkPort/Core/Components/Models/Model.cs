using DeeSynk.Core.Components.Models.Templates;
using DeeSynk.Core.Components.Types.Render;
using DeeSynk.Core.Managers;
using DeeSynk.Core.Systems;
using OpenTK;
using OpenTK.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;

namespace DeeSynk.Core.Components.Models
{
    using Templates = DeeSynk.Core.Components.Models.Templates.ModelTemplates;
    public enum ModelDataState : int
    {
        ReadOnly = 0,
        ReadWrite = 1,
    }

    public class Model
    {
        public static readonly int FLOAT_SIZE = sizeof(float);

        public static readonly int VERTEX_DIMS = 4;
        public static readonly int VERTEX_SIZE = FLOAT_SIZE * VERTEX_DIMS;

        public static readonly int NORMAL_DIMS = 3;
        public static readonly int NORMAL_SIZE = FLOAT_SIZE * NORMAL_DIMS;

        public static readonly int COLOR_DIMS  = 4;
        public static readonly int COLOR_SIZE  = FLOAT_SIZE * COLOR_DIMS;

        public static readonly int UV_DIMS     = 2;
        public static readonly int UV_SIZE     = FLOAT_SIZE * UV_DIMS;

        public static readonly int UINT_SIZE = sizeof(uint);

        private bool _hasValidData;
        public bool HasValidData { get => _hasValidData; }

        private ModelDataState _modelDataState;
        public ModelDataState ModelDataState { get => _modelDataState; }

        private ModelProperties _modelProperties;
        public ModelProperties Properties { get => _modelProperties; }

        private Vector4[] _vertices;
        public Vector4[] Vertices
        {
            get => _vertices;
            set
            {
                if (_modelDataState == ModelDataState.ReadWrite && value.Length > 0)
                {
                    _modelProperties |= ModelProperties.VERTICES;
                    _vertices = value;
                }
            }
        }
        public int VertexCount { get => _vertices.Length; }

        private Color4[] _colors;
        public Color4[] Colors
        {
            get => _colors;
            set
            {
                if (_modelDataState == ModelDataState.ReadWrite && !_modelProperties.HasFlag(ModelProperties.UVS) && value.Length > 0)
                {
                    _modelProperties |= ModelProperties.COLORS;
                    _colors = value;
                }
            }
        }
        public int ColorCount { get => _colors.Length; }

        private Vector2[] _uvs;
        public Vector2[] UVs
        {
            get => _uvs;
            set
            {
                if (_modelDataState == ModelDataState.ReadWrite && !_modelProperties.HasFlag(ModelProperties.COLORS) && value.Length > 0)
                {
                    _modelProperties |= ModelProperties.UVS;
                    _uvs = value;
                }
            }
        }
        public int UVCount { get => _uvs.Length; }

        private Vector3[] _normals;
        public Vector3[] Normals
        {
            get => _normals;
            set
            {
                if (_modelDataState == ModelDataState.ReadWrite && value.Length > 0)
                {
                    _modelProperties |= ModelProperties.NORMALS;
                    _normals = value;
                }
            }
        }
        public int NormalCount { get => _normals.Length; }

        private uint[] _elements;
        public uint[] Elements
        {
            get => _elements;
            set
            {
                if (_modelDataState == ModelDataState.ReadWrite && value.Length > 0)
                {
                    _modelProperties |= ModelProperties.ELEMENTS;
                    _elements = value;
                }
            }
        }
        public int ElementCount { get => _elements.Length; }

        public Model()
        {
            _modelDataState = ModelDataState.ReadWrite;
        }

        /// <summary>
        /// Sets the model's data state such that it can no longer be written to.
        /// </summary>
        public void SetReadOnly()
        {
            _hasValidData = ValidateData();

            if (_modelDataState == ModelDataState.ReadWrite)
                _modelDataState = ModelDataState.ReadOnly;
        }

        /// <summary>
        /// Sets the model's data state such that it can no longer be written to.  Includes 
        /// </summary>
        /// <param name="centerToZero">Sets the model's zero position (center) equal to the average of it's vertices.</param>
        /// <param name="computeNormals">Computes the normals for this model.</param>
        public void SetReadOnly(bool centerToZero, bool computeNormals)
        {
            _hasValidData = ValidateData();

            if (_hasValidData)
            {
                if (centerToZero) CenterToZero();
                if (computeNormals) ComputeNormals();

                if (_modelDataState == ModelDataState.ReadWrite)
                    _modelDataState = ModelDataState.ReadOnly;
            }
        }

        private void CenterToZero()
        {
            int count = VertexCount;
            if(VertexCount > 0)
            {
                Vector4 offsetAvg = Vector4.Zero;
                for (int idx = 0; idx < count; idx++)
                    offsetAvg += _vertices[idx];

                offsetAvg /= (float)count;
                offsetAvg.W = 0f;

                for (int idx = 0; idx < count; idx++)
                    _vertices[idx] -= offsetAvg;
            }
        }
        private void ComputeNormals()
        {
            if(_normals == null)
            {
                int count = VertexCount;
                Normals = new Vector3[count];
                {
                    int[] counts = new int[count];
                    Vector3[] angles = new Vector3[count];
                    for (int i = 0; i < ElementCount / 3; i++)
                    {
                        Vector3 p1 = _vertices[_elements[i * 3]].Xyz;
                        Vector3 p2 = _vertices[_elements[i * 3 + 1]].Xyz;
                        Vector3 p3 = _vertices[_elements[i * 3 + 2]].Xyz;

                        Vector3 U = p2 - p1;
                        Vector3 V = p3 - p1;

                        float Nx = (U.Y * V.Z) - (U.Z * V.Y);
                        float Ny = (U.Z * V.X) - (U.X * V.Z);
                        float Nz = (U.X * V.Y) - (U.Y * V.X);

                        Vector3 normal = new Vector3(Nx, Ny, Nz);

                        /*
                        float r = normal.Length;
                        double theta = Math.Atan2(normal.Y, normal.X);
                        double psi = Math.Acos(normal.Z / r);

                        Vector3 spherical = new Vector3(1.0f, (float)theta, (float)psi);

                        for (int j=0; j<3; j++)
                        {
                            if (angles[_elements[i * 3 + j]] == Vector3.Zero)
                                angles[_elements[i * 3 + j]] = spherical;
                            else
                                angles[_elements[i * 3 + j]] += spherical;

                            counts[_elements[i * 3 + j]]++;
                        }*/

                        for(int j=0; j<3; j++)
                        {
                            var d = _normals[_elements[i * 3 + j]];
                            if (_normals[_elements[i * 3 + j]] == Vector3.Zero)
                                _normals[_elements[i * 3 + j]] = normal;
                            else
                                _normals[_elements[i * 3 + j]] = (d != -normal) ? Vector3.Lerp(d, normal, 1f/ (counts[_elements[i * 3 + j]] + 1)): Vector3.Zero;

                            counts[_elements[i * 3 + j]]++;
                        }
                    }

                    for (int i = 0; i < _normals.Length; i++)
                    {
                        Vector3 n = _normals[i];
                        n.Normalize();
                        _normals[i] = n;
                        /*angles[i] /= (float)counts[i];
                        float x = (float)(Math.Cos(angles[i].Y) * Math.Sin(angles[i].Z));
                        float y = (float)(Math.Sin(angles[i].Y) * Math.Sin(angles[i].Z));
                        float z = (float)(Math.Cos(angles[i].Z));
                        _normals[i] = new Vector3(x, y, z);*/
                    }
                }
            }
        }

        // Valid data consists of at least one vertex, and an elements buffer that has equal or more elements than vertices.
        // Additionally, if present:  vertices = normals = colors
        //                            elements = uvs

        private bool ValidateData()
        {
            bool valid = false;
            if(VertexCount > 0)
            {
                valid = true;

                if (_modelProperties.HasFlag(ModelProperties.ELEMENTS) && ElementCount >= VertexCount)
                {
                    if (_modelProperties.HasFlag(ModelProperties.UVS))
                    {
                        if(UVCount == ElementCount)
                        {
                            if (valid != false)
                                valid = true;
                        }
                        else
                        {
                            valid = false;
                        }
                    }
                }
                else
                {
                    valid = false;
                }

                if (_modelProperties.HasFlag(ModelProperties.COLORS))
                {
                    if (ColorCount == VertexCount)
                    {
                        if (valid != false)
                            valid = true;
                    }
                    else
                    {
                        valid = false;
                    }
                }

                /*if (_modelProperties.HasFlag(ModelProperties.NORMALS))
                {
                    if (NormalCount == VertexCount)
                    {
                        if (valid != false)
                            valid = true;
                    }
                    else
                    {
                        valid = false;
                    }
                }*/
            }
            return valid;
        }

        public void GetInterleavedData(ModelProperties props, Span<float> data)
        {
            ModelProperties properties = _modelProperties & props;

            if (!properties.HasFlag(ModelProperties.VERTICES))
                throw new ArgumentException("Model vertices expected");

            if (!properties.HasFlag(props))
                throw new ArgumentException("Model does not contain requested data");

            if (!HasValidData)
                throw new ArgumentException("Passed model has invalid data, will not read");

            PopulateArrayInterleaved(properties, FloatStride(properties), data);
        }

        private void PopulateArrayInterleaved(ModelProperties props, int stride, Span<float> data)
        {
            int offset = 0;
            if (props.HasFlag(ModelProperties.VERTICES))
            {
                for (int idx = 0; idx < _vertices.Length; idx++)
                {
                    data[idx * stride + offset + 0] = _vertices[idx].X;
                    data[idx * stride + offset + 1] = _vertices[idx].Y;
                    data[idx * stride + offset + 2] = _vertices[idx].Z;
                    data[idx * stride + offset + 3] = _vertices[idx].W;
                }

                offset += PropertyDimensions(ModelProperties.VERTICES);
            }
            if (props.HasFlag(ModelProperties.NORMALS))
            {
                for (int idx = 0; idx < _normals.Length; idx++)
                {
                    data[idx * stride + offset + 0] = _normals[idx].X;
                    data[idx * stride + offset + 1] = _normals[idx].Y;
                    data[idx * stride + offset + 2] = _normals[idx].Z;
                }

                offset += PropertyDimensions(ModelProperties.NORMALS);
            }
            if (props.HasFlag(ModelProperties.UVS))
            {
                for (int idx = 0; idx < _uvs.Length; idx++)
                {
                    data[idx * stride + offset + 0] = _uvs[idx].X;
                    data[idx * stride + offset + 1] = _uvs[idx].Y;
                }

                offset += PropertyDimensions(ModelProperties.UVS);
            }
            if (props.HasFlag(ModelProperties.COLORS))
            {
                for (int idx = 0; idx < _colors.Length; idx++)
                {
                    data[idx * stride + offset + 0] = _colors[idx].R;
                    data[idx * stride + offset + 1] = _colors[idx].G;
                    data[idx * stride + offset + 2] = _colors[idx].B;
                    data[idx * stride + offset + 3] = _colors[idx].A;
                }

                offset += PropertyDimensions(ModelProperties.UVS);
            }
        }

        //***STATICS***\\

        public static int FloatStride(ModelProperties props)
        {
            int stride = 0;
            stride += (props.HasFlag(ModelProperties.VERTICES)) ? VERTEX_DIMS : 0;
            stride += (props.HasFlag(ModelProperties.NORMALS )) ? NORMAL_DIMS : 0;
            stride += (props.HasFlag(ModelProperties.UVS     )) ? UV_DIMS     : 0;
            stride += (props.HasFlag(ModelProperties.COLORS  )) ? COLOR_DIMS  : 0;
            return stride;
        }
        public static int ByteStride(ModelProperties props)
        {
            int stride = 0;
            stride += (props.HasFlag(ModelProperties.VERTICES)) ? VERTEX_SIZE : 0;
            stride += (props.HasFlag(ModelProperties.NORMALS )) ? NORMAL_SIZE : 0;
            stride += (props.HasFlag(ModelProperties.UVS     )) ? UV_SIZE     : 0;
            stride += (props.HasFlag(ModelProperties.COLORS  )) ? COLOR_SIZE  : 0;
            return stride;
        }

        public static int PropertyDimensions(ModelProperties property)
        {
            switch (property)
            {
                case (ModelProperties.VERTICES): return VERTEX_DIMS;
                case (ModelProperties.NORMALS ): return NORMAL_DIMS;
                case (ModelProperties.COLORS  ): return COLOR_DIMS ;
                case (ModelProperties.UVS     ): return UV_DIMS    ;
                default: return 0;
            }
        }
        public static int PropertySize(ModelProperties property)
        {
            switch (property)
            {
                case (ModelProperties.VERTICES): return VERTEX_SIZE;
                case (ModelProperties.NORMALS ): return NORMAL_SIZE;
                case (ModelProperties.COLORS  ): return COLOR_SIZE ;
                case (ModelProperties.UVS     ): return UV_SIZE    ;
                default: return 0;
            }
        }

        public static string GetTemplateName(ModelTemplates template)
        {
            switch (template)
            {
                case (ModelTemplates.PlaneXZ): return Plane.NameXZ;
                case (ModelTemplates.PlaneXY): return Plane.NameXY;
                case (ModelTemplates.PlaneYZ): return Plane.NameYZ;
                default: return "NONE";
            }
        }

        /*
        private static void CreateStaticModelMatrix(ref ComponentModelStatic modelComp, out Matrix4 modelMatrix)
        {
            float tau = 6.2831853f;
            var flags = modelComp.ConstructionFlags;

            Matrix4 rotX = Matrix4.Identity, rotY = Matrix4.Identity, rotZ = Matrix4.Identity, scale = Matrix4.Identity, trns = Matrix4.Identity, trns_Inv = Matrix4.Identity;

            modelMatrix = Matrix4.Identity;
            bool hasRotation = false;
            if (flags.HasFlag(ConstructionFlags.FLOAT_ROTATION_X))
            {
                modelComp.GetConstructionParameter(ConstructionFlags.FLOAT_ROTATION_X, out float[] data);
                if(data[0] % tau != 0)
                {
                Matrix4.CreateRotationX(data[0], out rotX);
                Matrix4.Mult(ref rotX, ref modelMatrix, out modelMatrix);
                hasRotation = true;
                }
            }
            if (flags.HasFlag(ConstructionFlags.FLOAT_ROTATION_Y))
            {
                modelComp.GetConstructionParameter(ConstructionFlags.FLOAT_ROTATION_Y, out float[] data);
                if(data[0] % tau != 0)
                {
                    Matrix4.CreateRotationY(data[0], out rotY);
                    Matrix4.Mult(ref rotY, ref modelMatrix, out modelMatrix);
                    hasRotation = true;
                }
            }
            if (flags.HasFlag(ConstructionFlags.FLOAT_ROTATION_Z))
            {
                modelComp.GetConstructionParameter(ConstructionFlags.FLOAT_ROTATION_Z, out float[] data);
                if(data[0] % tau != 0)
                {
                    Matrix4.CreateRotationZ(data[0], out rotZ);
                    Matrix4.Mult(ref rotZ, ref modelMatrix, out modelMatrix);
                    hasRotation = true;
                }

            }
            if (flags.HasFlag(ConstructionFlags.VECTOR3_SCALE))
            {
                modelComp.GetConstructionParameter(ConstructionFlags.VECTOR3_SCALE, out float[] data);
                Matrix4.CreateScale(data[0], data[1], data[2], out scale);
                Matrix4.Mult(ref scale, ref modelMatrix, out modelMatrix);
            }
            if (flags.HasFlag(ConstructionFlags.VECTOR3_OFFSET))
            {
                modelComp.GetConstructionParameter(ConstructionFlags.VECTOR3_OFFSET, out float[] data);
                if (hasRotation)
                {
                    Matrix4.CreateTranslation(-data[0], -data[1], -data[2], out trns_Inv);
                    Matrix4.Mult(ref trns_Inv, ref modelMatrix, out modelMatrix);
                }

                Matrix4.CreateTranslation(data[0], data[1], data[2], out trns);
                Matrix4.Mult(ref modelMatrix, ref trns, out modelMatrix);
            }

            //modelMatrix = trns_Inv * scale * rotX * rotY * rotZ * trns;
        }*/
    }
}
