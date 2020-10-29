using DeeSynk.Core.Components.Types.Render;
using OpenTK;
using OpenTK.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeeSynk.Core.Components.Models.Templates
{
    public static class Plane
    {
        //A static helper class to create certain templates
        public static string NameXZ => "PlaneXZ";
        public static string NameXY => "PlaneXY";
        public static string NameYZ => "PlaneYZ";

        /// <summary>
        /// Constructs a model object for a plane of dimensions specified by the model component.  Created in the XZ plane.
        /// </summary>
        /// <param name="modelComp">Model component that holds data for the creation of this object.</param>
        /// <returns></returns>
        public static Model XZ(ref ComponentModelStatic modelComp)
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

                float width = (hasDimensionFlag) ? WH[0] : 1.0f;  //X
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
                            vertices[1] = new Vector4(width, 0, -height, 0);
                            vertices[2] = new Vector4(width, 0, height, 0);
                            vertices[3] = new Vector4(width, 0, height, 0);
                            vertices[4] = new Vector4(-width, 0, height, 0);
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
                        //Add vertices XZ
                        {
                            vertices[0] = new Vector4(-width, 0, -height, 1);
                            vertices[1] = new Vector4(width, 0, -height, 1);
                            vertices[2] = new Vector4(width, 0, height, 1);
                            vertices[3] = new Vector4(-width, 0, height, 1);
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
        public static Model XY(ref ComponentModelStatic modelComp)
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
                //checks for an instance of a dimension flag - if none is present use default values
                bool hasDimensionFlag = modelComp.ConstructionFlags.HasFlag(ConstructionFlags.VECTOR3_DIMENSIONS);
                float[] WH = modelComp.GetConstructionParameter(ConstructionFlags.VECTOR3_DIMENSIONS);

                float width = (hasDimensionFlag) ? WH[0] : 1.0f;  //X
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
                            vertices[1] = new Vector4(width, -height, 0, 1);
                            vertices[2] = new Vector4(width, height, 0, 1);
                            vertices[3] = new Vector4(width, height, 0, 1);
                            vertices[4] = new Vector4(-width, height, 0, 1);
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
                        //Add vertices XY
                        {
                            vertices[0] = new Vector4(-width, -height, 0, 1);
                            vertices[1] = new Vector4(width, -height, 0, 1);
                            vertices[2] = new Vector4(width, height, 0, 1);
                            vertices[3] = new Vector4(-width, height, 0, 1);
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
                        vertices[1] = new Vector4(width, -height, 0, 1);
                        vertices[2] = new Vector4(width, height, 0, 1);
                        vertices[3] = new Vector4(width, height, 0, 1);
                        vertices[4] = new Vector4(-width, height, 0, 1);
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
        public static Model YZ(ref ComponentModelStatic modelComp)
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
                            vertices[1] = new Vector4(0, width, -height, 1);
                            vertices[2] = new Vector4(0, width, height, 1);
                            vertices[3] = new Vector4(0, width, height, 1);
                            vertices[4] = new Vector4(0, -width, height, 1);
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
                            vertices[1] = new Vector4(0, width, -height, 1);
                            vertices[2] = new Vector4(0, width, height, 1);
                            vertices[3] = new Vector4(0, -width, height, 1);
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
                        vertices[1] = new Vector4(0, width, -height, 1);
                        vertices[2] = new Vector4(0, width, height, 1);
                        vertices[3] = new Vector4(0, width, height, 1);
                        vertices[4] = new Vector4(0, -width, height, 1);
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
    }
}
