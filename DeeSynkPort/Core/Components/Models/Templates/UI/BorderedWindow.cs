using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DeeSynk.Core.Components.Models.Tools;
using DeeSynk.Core.Components.Types.Render;
using DeeSynk.Core.Components.UI;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Mathematics;

namespace DeeSynk.Core.Components.Models.Templates.UI
{
    public class BorderedWindow : ITemplate
    {

        private PositionReference _reference;
        /// <summary>
        /// Indicates the origin or reference point of the model.
        /// </summary>
        public PositionReference Reference { get => _reference; }
        private Vector2 _size;
        /// <summary>
        /// Size of the container in pixels.
        /// </summary>
        public Vector2 Size { get => _size; }
        private float _borderWidth;
        /// <summary>
        /// Width of the border in pixels.
        /// </summary>
        public float BorderWidth { get => _borderWidth; }
        private Color4 _primaryColor;
        /// <summary>
        /// Color of all primary elements such as the background.
        /// </summary>
        public Color4 PrimaryColor { get => _primaryColor; }
        private Color4 _secondaryColor;
        /// <summary>
        /// Color of all secondary elements such as the border.
        /// </summary>
        public Color4 SecondaryColor { get => _secondaryColor; }

        public BorderedWindow(ModelProperties properties, PositionReference reference, Vector2 size, float borderWidth, Color4 primaryColor, Color4 secondaryColor)
        {
            _reference = reference;
            _size = size;
            if (borderWidth > Math.Min(size.X / 2.0f, size.Y / 2.0f))
                throw new ArgumentOutOfRangeException("Border width is limited half of the minimum dimension of the container size.");
            else
                _borderWidth = borderWidth;
            if (properties.HasFlag(ModelProperties.COLORS))
            {
                _primaryColor = primaryColor;
                _secondaryColor = secondaryColor;
            }
            else
                throw new Exception("Invalid configuration: model does not support color designation.");
        }
        
        public Model ConstructModel(ComponentModelStatic modelComp)
        {
            if (modelComp.TemplateID == ModelTemplates.UIContainer)
                return Window(modelComp);
            return null;
        }

        private Model Window(ComponentModelStatic modelComp)
        {
            Model model = new Model();

            if (modelComp.ModelProperties.HasFlag(ModelProperties.VERTICES))
            {
                Vector4[] vertices;
                Color4[] colors;
                uint[] elements = new uint[30];


                //Adds elements, find a modular formula for fun :)
                {
                    elements[0] = 1;
                    elements[1] = 4;
                    elements[2] = 0;

                    elements[3] = 1;
                    elements[4] = 5;
                    elements[5] = 4;

                    elements[6] = 1;
                    elements[7] = 2;
                    elements[8] = 6;

                    elements[9] = 1;
                    elements[10] = 6;
                    elements[11] = 5;

                    elements[12] = 3;
                    elements[13] = 6;
                    elements[14] = 2;

                    elements[15] = 3;
                    elements[16] = 7;
                    elements[17] = 6;

                    elements[18] = 3;
                    elements[19] = 0;
                    elements[20] = 4;

                    elements[21] = 3;
                    elements[22] = 4;
                    elements[23] = 7;

                    elements[24] = 9;
                    elements[25] = 11;
                    elements[26] = 8;

                    elements[27] = 9;
                    elements[28] = 10;
                    elements[29] = 11;
                }

                Vector2 outSize = _size;
                Vector2 inSize = _size - new Vector2(_borderWidth);

                Vector4[] s1 = MeshGenerator.Square4(outSize, Orientation.XY, true);
                Vector4[] s2 = MeshGenerator.Square4(inSize, Orientation.XY, true);

                Vector4[] tempVertices = new Vector4[12];
                Color4[] tempColors = new Color4[12];
                for(int idx=0; idx<tempVertices.Length; idx++)
                {
                    tempVertices[idx] = (idx < 4) ? s1[idx] : s2[idx % 4];
                    tempColors[idx] = (idx < 8) ? _secondaryColor : _primaryColor;
                }

                MeshGenerator.OffsetVectors(ref tempVertices, ReferenceConverter.GetReferenceOffset4(_reference, outSize, false));

                if (modelComp.ModelProperties.HasFlag(ModelProperties.ELEMENTS))
                {
                    vertices = new Vector4[tempVertices.Length];
                    colors = new Color4[vertices.Length];

                    for (int idx = 0; idx < vertices.Length; idx++)
                    {
                        vertices[idx] = tempVertices[idx];
                        colors[idx] = tempColors[idx];
                    }

                    model.Vertices = vertices;
                    model.Colors = colors;
                    model.Elements = elements;
                }
                else
                {
                    vertices = new Vector4[elements.Length];
                    colors = new Color4[elements.Length];

                    for(int idx=0; idx < vertices.Length; idx++)
                    {
                        vertices[idx] = tempVertices[elements[idx]];
                        colors[idx] = tempColors[elements[idx]];
                    }
                }

                model.SetReadOnly(false, true);
            }

            return model;
        }
    }
}

//IF VERTS
//create data
//WIDTH HEIGHT
//IF ELEM
    //create ELEM