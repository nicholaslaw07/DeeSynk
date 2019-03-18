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

    public enum ModelDataState : int
    {
        ReadOnly = 0,
        ReadWrite = 1,
    }

    public class Model
    {
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
                _normals = new Vector3[count];
                {
                    int[] counts = new int[count];
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

                        normal.Normalize();

                        _normals[_elements[i * 3]] += normal;
                        _normals[_elements[i * 3 + 1]] += normal;
                        _normals[_elements[i * 3 + 2]] += normal;

                        counts[_elements[i * 3]]++;
                        counts[_elements[i * 3 + 1]]++;
                        counts[_elements[i * 3 + 2]]++;
                    }

                    for (int i = 0; i < _normals.Length; i++)
                    {
                        Vector3 n = _normals[i];
                        n.Normalize();
                        _normals[i] = n;
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

                if (_modelProperties.HasFlag(ModelProperties.NORMALS))
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
                }
            }
            return valid;
        }


        //***STATICS***\\

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
            Model model = new Model();

            if (modelComp.ModelProperties.HasFlag(ModelProperties.VERTICES))
            {
                Vector4[] vertices;
                Color4[] colors;
                Vector2[] uvs;
                uint[] elements;

                //CreateStaticModelMatrix(ref modelComp, out Matrix4 modelMatrix);
                //model = new Model(modelMatrix);

                //no error checking since this should almost always be the case, if not we compensate for it by having defaults (1.0f)
                bool hasDimensionFlag = modelComp.ConstructionFlags.HasFlag(ConstructionFlags.VECTOR3_DIMENSIONS);
                float[] WH = modelComp.GetConstructionParameter(ConstructionFlags.VECTOR3_DIMENSIONS); 

                float width  = (hasDimensionFlag) ? WH[0] : 1.0f;  //X
                width /= 2.0f;
                float height = (hasDimensionFlag) ? WH[2] : 1.0f;  //Z
                height /= 2.0f;

                if (modelComp.ModelProperties.HasFlag(ModelProperties.ELEMENTS))
                {
                    elements = new uint[6];

                    if (modelComp.ModelProperties.HasFlag(ModelProperties.UVS))
                    {
                        vertices = new Vector4[6];
                        uvs = new Vector2[6];
                        //Add vertices XZ
                        {
                            vertices[0] = new Vector4(-width, 0, -height, 0);
                            vertices[1] = new Vector4( width, 0, -height, 0);
                            vertices[2] = new Vector4( width, 0,  height, 0);
                            vertices[3] = new Vector4( width, 0,  height, 0);
                            vertices[4] = new Vector4(-width, 0,  height, 0);
                            vertices[5] = new Vector4(-width, 0, -height, 0);
                        }
                        model.Vertices = vertices;

                        //Add elements
                        {
                            elements[0] = 0;
                            elements[1] = 1;
                            elements[2] = 2;
                            elements[3] = 3;
                            elements[4] = 4;
                            elements[5] = 5;
                        }
                        model.Elements = elements;

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
                        model.UVs = uvs;
                    }
                    else
                    {
                        vertices = new Vector4[4];
                        colors = new Color4[4];
                        elements = new uint[6];
                        //Add vertices XZ
                        {
                            vertices[0] = new Vector4(-width, 0, -height, 1);
                            vertices[1] = new Vector4( width, 0, -height, 1);
                            vertices[2] = new Vector4( width, 0,  height, 1);
                            vertices[3] = new Vector4(-width, 0,  height, 1);
                        }
                        model.Vertices = vertices;

                        //Add elements
                        {
                            elements[0] = 0;
                            elements[1] = 1;
                            elements[2] = 2;
                            elements[3] = 2;
                            elements[4] = 3;
                            elements[5] = 0;
                        }
                        model.Elements = elements;

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

                        model.Colors = colors;
                    }
                }
                else
                {
                    vertices = new Vector4[6];
                    //Add vertices XZ
                    {
                        vertices[0] = new Vector4(-width, 0, -height, 1);
                        vertices[1] = new Vector4(width, 0, -height, 1);
                        vertices[2] = new Vector4(width, 0, height, 1);
                        vertices[3] = new Vector4(width, 0, height, 1);
                        vertices[4] = new Vector4(-width, 0, height, 1);
                        vertices[5] = new Vector4(-width, 0, -height, 1);
                    }
                    model.Vertices = vertices;

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
                        model.UVs = uvs;
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
                        model.Colors = colors;
                    }
                }

                model.SetReadOnly(false, true);
            }
            return model;
        }

        /// <summary>
        /// Constructs a model object for a plane of dimensions specified by the model component.  Created in the XY plane.
        /// </summary>
        /// <param name="modelComp">Model component that holds data for the creation of this object.</param>
        /// <returns></returns>
        public static Model CreateTemplatePlaneXY(ref ComponentModelStatic modelComp)
        {
            Model model = new Model();

            if (modelComp.ModelProperties.HasFlag(ModelProperties.VERTICES))
            {
                Vector4[] vertices;
                Color4[] colors;
                Vector2[] uvs;
                uint[] elements;

                //CreateStaticModelMatrix(ref modelComp, out Matrix4 modelMatrix);
                //model = new Model(modelMatrix);

                //no error checking since this should almost always be the case, if not we compensate for it by having defaults (1.0f)
                bool hasDimensionFlag = modelComp.ConstructionFlags.HasFlag(ConstructionFlags.VECTOR3_DIMENSIONS);
                float[] WH = modelComp.GetConstructionParameter(ConstructionFlags.VECTOR3_DIMENSIONS); 

                float width  = (hasDimensionFlag) ? WH[0] : 1.0f;  //X
                width /= 2.0f;
                float height = (hasDimensionFlag) ? WH[1] : 1.0f;  //Y
                height /= 2.0f;

                if (modelComp.ModelProperties.HasFlag(ModelProperties.ELEMENTS))
                {
                    elements = new uint[6];

                    if (modelComp.ModelProperties.HasFlag(ModelProperties.UVS))
                    {
                        vertices = new Vector4[6];
                        uvs = new Vector2[6];
                        //Add vertices XY
                        {
                            vertices[0] = new Vector4(-width, -height, 0, 1);
                            vertices[1] = new Vector4( width, -height, 0, 1);
                            vertices[2] = new Vector4( width,  height, 0, 1);
                            vertices[3] = new Vector4( width,  height, 0, 1);
                            vertices[4] = new Vector4(-width,  height, 0, 1);
                            vertices[5] = new Vector4(-width, -height, 0, 1);
                        }
                        model.Vertices = vertices;

                        //Add elements
                        {
                            elements[0] = 0;
                            elements[1] = 1;
                            elements[2] = 2;
                            elements[3] = 3;
                            elements[4] = 4;
                            elements[5] = 5;
                        }
                        model.Elements = elements;

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
                        model.UVs = uvs;
                    }
                    else
                    {
                        vertices = new Vector4[4];
                        colors = new Color4[4];
                        elements = new uint[6];
                        //Add vertices XY
                        {
                            vertices[0] = new Vector4(-width, -height, 0, 1);
                            vertices[1] = new Vector4( width, -height, 0, 1);
                            vertices[2] = new Vector4( width,  height, 0, 1);
                            vertices[3] = new Vector4(-width,  height, 0, 1);
                        }
                        model.Vertices = vertices;

                        //Add elements
                        {
                            elements[0] = 0;
                            elements[1] = 1;
                            elements[2] = 2;
                            elements[3] = 2;
                            elements[4] = 3;
                            elements[5] = 0;
                        }
                        model.Elements = elements;

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

                        model.Colors = colors;
                    }
                }
                else
                {
                    vertices = new Vector4[6];
                    //Add vertices XY
                    {
                        vertices[0] = new Vector4(-width, -height, 0, 1);
                        vertices[1] = new Vector4( width, -height, 0, 1);
                        vertices[2] = new Vector4( width,  height, 0, 1);
                        vertices[3] = new Vector4( width,  height, 0, 1);
                        vertices[4] = new Vector4(-width,  height, 0, 1);
                        vertices[5] = new Vector4(-width, -height, 0, 1);
                    }
                    model.Vertices = vertices;

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
                        model.UVs = uvs;
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
                        model.Colors = colors;
                    }
                }

                model.SetReadOnly(false, true);
            }
            return model;
        }

        /// <summary>
        /// Constructs a model object for a plane of dimensions specified by the model component.  Created in the YZ plane.
        /// </summary>
        /// <param name="modelComp">Model component that holds data for the creation of this object.</param>
        /// <returns></returns>
        public static Model CreateTemplatePlaneYZ(ref ComponentModelStatic modelComp)
        {

            Model model = new Model();

            if (modelComp.ModelProperties.HasFlag(ModelProperties.VERTICES))
            {
                Vector4[] vertices;
                Color4[] colors;
                Vector2[] uvs;
                uint[] elements;

                //CreateStaticModelMatrix(ref modelComp, out Matrix4 modelMatrix);
                //model = new Model(modelMatrix);

                //no error checking since this should almost always be the case, if not we compensate for it by having defaults (1.0f)
                bool hasDimensionFlag = modelComp.ConstructionFlags.HasFlag(ConstructionFlags.VECTOR3_DIMENSIONS);
                float[] WH = modelComp.GetConstructionParameter(ConstructionFlags.VECTOR3_DIMENSIONS);

                float width = (hasDimensionFlag) ? WH[1] : 1.0f;  //Y
                width /= 2.0f;
                float height = (hasDimensionFlag) ? WH[2] : 1.0f;  //Z
                height /= 2.0f;

                if (modelComp.ModelProperties.HasFlag(ModelProperties.ELEMENTS))
                {
                    elements = new uint[6];

                    if (modelComp.ModelProperties.HasFlag(ModelProperties.UVS))
                    {
                        vertices = new Vector4[6];
                        uvs = new Vector2[6];
                        //Add vertices YZ
                        {
                            vertices[0] = new Vector4(0, -width, -height, 1);
                            vertices[1] = new Vector4(0,  width, -height, 1);
                            vertices[2] = new Vector4(0,  width,  height, 1);
                            vertices[3] = new Vector4(0,  width,  height, 1);
                            vertices[4] = new Vector4(0, -width,  height, 1);
                            vertices[5] = new Vector4(0, -width, -height, 1);
                        }
                        model.Vertices = vertices;

                        //Add elements
                        {
                            elements[0] = 0;
                            elements[1] = 1;
                            elements[2] = 2;
                            elements[3] = 3;
                            elements[4] = 4;
                            elements[5] = 5;
                        }
                        model.Elements = elements;

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
                        model.UVs = uvs;
                    }
                    else
                    {
                        vertices = new Vector4[4];
                        colors = new Color4[4];
                        elements = new uint[6];
                        //Add vertices YZ
                        {
                            vertices[0] = new Vector4(0, -width, -height, 1);
                            vertices[1] = new Vector4(0,  width, -height, 1);
                            vertices[2] = new Vector4(0,  width,  height, 1);
                            vertices[3] = new Vector4(0, -width,  height, 1);
                        }
                        model.Vertices = vertices;

                        //Add elements
                        {
                            elements[0] = 0;
                            elements[1] = 1;
                            elements[2] = 2;
                            elements[3] = 2;
                            elements[4] = 3;
                            elements[5] = 0;
                        }
                        model.Elements = elements;

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

                        model.Colors = colors;
                    }
                }
                else
                {
                    vertices = new Vector4[6];
                    //Add vertices YZ
                    {
                        vertices[0] = new Vector4(0, -width, -height, 1);
                        vertices[1] = new Vector4(0,  width, -height, 1);
                        vertices[2] = new Vector4(0,  width,  height, 1);
                        vertices[3] = new Vector4(0,  width,  height, 1);
                        vertices[4] = new Vector4(0, -width,  height, 1);
                        vertices[5] = new Vector4(0, -width, -height, 1);
                    }
                    model.Vertices = vertices;

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
                        model.UVs = uvs;
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
                        model.Colors = colors;
                    }
                }

                model.SetReadOnly(false, true);
            }
            return model;
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
