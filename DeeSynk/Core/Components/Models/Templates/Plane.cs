using DeeSynk.Core.Components.Models.Tools;
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
    public class Plane : ITemplate
    {
        public static string NameXZ => "PlaneXZ";
        public static string NameXY => "PlaneXY";
        public static string NameYZ => "PlaneYZ";


        private Vector2 _dimensions;
        public Vector2 Dimension { get => _dimensions; }
        private Color4 _color;
        public Color4 Color { get => _color; }
        private Vector2 _uvOffset;
        public Vector2 UVOffset { get => _uvOffset; }
        private Vector2 _uvScale;
        public Vector2 UVScale { get => _uvScale; }

        //In the future, maybe use Color as a form of tint.  Then add a tint factor.

        //Add error handling for invalid configurations???

        public Plane(ModelProperties properties, Vector2 dimensions, Color4 color)
        {
            _dimensions = dimensions;
            if (properties.HasFlag(ModelProperties.COLORS))
                _color = color;
            else
                throw new Exception("Invalid configuration: model does not support color designation.");
        }

        public Plane(ModelProperties properties, Vector2 dimensions, Vector2 uvOffset, Vector2 uvScale)
        {
            _dimensions = dimensions;
            if (properties.HasFlag(ModelProperties.UVS))
            {
                _uvOffset = uvOffset;
                _uvScale = uvScale;
            }
            else
                throw new Exception("Invalid configuration: model does not support uv designation.");
        }

        public Model ConstructModel(ComponentModelStatic modelComp)
        {
            switch (modelComp.TemplateID)
            {
                case (ModelTemplates.PlaneXZ):
                    return XZ(modelComp);
                case (ModelTemplates.PlaneXY):
                    return XY(modelComp);
                case (ModelTemplates.PlaneYZ):
                    return YZ(modelComp);
                default:
                    return null;
            }
        }

        /// <summary>
        /// Constructs a model object for a plane of dimensions specified by the model component.  Created in the XZ plane.
        /// </summary>
        /// <param name="modelComp">Model component that holds data for the creation of this object.</param>
        /// <returns></returns>
        private Model XZ(ComponentModelStatic modelComp)
        {
            Model model = new Model();

            if (modelComp.ModelProperties.HasFlag(ModelProperties.VERTICES))
            {
                Vector4[] vertices;
                Color4[] colors;
                Vector2[] uvs;
                uint[] elements;

                var size = _dimensions;

                if (modelComp.ModelProperties.HasFlag(ModelProperties.ELEMENTS))
                {
                    elements = new uint[6];

                    if (modelComp.ModelProperties.HasFlag(ModelProperties.UVS))
                    {
                        vertices = new Vector4[6];
                        uvs = new Vector2[6];
                        //Add vertices XZ
                        vertices = MeshGenerator.Square4(size, Orientation.XZ, false);
                        model.Vertices = vertices; //elements is false since we are using UVs

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

                        var offset = _uvOffset;
                        float offsetU = offset.X;
                        float offsetV = offset.Y;

                        var scale = _uvScale;
                        float scaleU = (scale.X == 0.0f) ? scale.X : 1.0f;
                        float scaleV = (scale.Y == 0.0f) ? scale.Y : 1.0f;

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
                        vertices = MeshGenerator.Square4(size, Orientation.XZ, true);
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
                            Color4 color = _color;
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
                    vertices = MeshGenerator.Square4(size, Orientation.XZ, false);
                    model.Vertices = vertices;

                    //Add UVs
                    if (modelComp.ModelProperties.HasFlag(ModelProperties.UVS))
                    {
                        uvs = new Vector2[6];

                        var offset = _uvOffset;
                        float offsetU = offset.X;
                        float offsetV = offset.Y;

                        var scale = _uvScale;
                        float scaleU = (scale.X == 0.0f) ? scale.X : 1.0f;
                        float scaleV = (scale.Y == 0.0f) ? scale.Y : 1.0f;

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
                            Color4 color = _color;
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
        private Model XY(ComponentModelStatic modelComp)
        {
            Model model = new Model();

            if (modelComp.ModelProperties.HasFlag(ModelProperties.VERTICES))
            {
                Vector4[] vertices;
                Color4[] colors;
                Vector2[] uvs;
                uint[] elements;

                var size = _dimensions;

                if (modelComp.ModelProperties.HasFlag(ModelProperties.ELEMENTS))
                {
                    elements = new uint[6];

                    if (modelComp.ModelProperties.HasFlag(ModelProperties.UVS))
                    {
                        vertices = new Vector4[6];
                        uvs = new Vector2[6];
                        //Add vertices XY
                        vertices = MeshGenerator.Square4(size, Orientation.XY, false);
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

                        var offset = _uvOffset;
                        float offsetU = offset.X;
                        float offsetV = offset.Y;

                        var scale = _uvScale;
                        float scaleU = (scale.X == 0.0f) ? scale.X : 1.0f;
                        float scaleV = (scale.Y == 0.0f) ? scale.Y : 1.0f;

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
                        vertices = MeshGenerator.Square4(size, Orientation.XY, true);
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
                            Color4 color = _color;
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
                    vertices = MeshGenerator.Square4(size, Orientation.XY, false);
                    model.Vertices = vertices;

                    //Add UVs
                    if (modelComp.ModelProperties.HasFlag(ModelProperties.UVS))
                    {
                        uvs = new Vector2[6];

                        var offset = _uvOffset;
                        float offsetU = offset.X;
                        float offsetV = offset.Y;

                        var scale = _uvScale;
                        float scaleU = (scale.X == 0.0f) ? scale.X : 1.0f;
                        float scaleV = (scale.Y == 0.0f) ? scale.Y : 1.0f;

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
                            Color4 color = _color;
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
        private Model YZ(ComponentModelStatic modelComp)
        {

            Model model = new Model();

            if (modelComp.ModelProperties.HasFlag(ModelProperties.VERTICES))
            {
                Vector4[] vertices;
                Color4[] colors;
                Vector2[] uvs;
                uint[] elements;

                var size = _dimensions;

                if (modelComp.ModelProperties.HasFlag(ModelProperties.ELEMENTS))
                {
                    elements = new uint[6];

                    if (modelComp.ModelProperties.HasFlag(ModelProperties.UVS))
                    {
                        vertices = new Vector4[6];
                        uvs = new Vector2[6];
                        //Add vertices YZ
                        vertices = MeshGenerator.Square4(size, Orientation.YZ, false);
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

                        var offset = _uvOffset;
                        float offsetU = offset.X;
                        float offsetV = offset.Y;

                        var scale = _uvScale;
                        float scaleU = (scale.X == 0.0f) ? scale.X : 1.0f;
                        float scaleV = (scale.Y == 0.0f) ? scale.Y : 1.0f;

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
                        vertices = MeshGenerator.Square4(size, Orientation.YZ, true);
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
                            Color4 color = _color;
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
                    vertices = MeshGenerator.Square4(size, Orientation.YZ, false);
                    model.Vertices = vertices;

                    //Add UVs
                    if (modelComp.ModelProperties.HasFlag(ModelProperties.UVS))
                    {
                        uvs = new Vector2[6];

                        var offset = _uvOffset;
                        float offsetU = offset.X;
                        float offsetV = offset.Y;

                        var scale = _uvScale;
                        float scaleU = (scale.X == 0.0f) ? scale.X : 1.0f;
                        float scaleV = (scale.Y == 0.0f) ? scale.Y : 1.0f;

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
                            Color4 color = _color;
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
