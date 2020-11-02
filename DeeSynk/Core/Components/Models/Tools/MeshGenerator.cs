using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics.OpenGL4;

namespace DeeSynk.Core.Components.Models.Tools
{
    public enum Orientation : int
    {
        XZ = 0,
        XY = 1,
        YZ = 2
    }
    public static class MeshGenerator
    {
        public static void OffsetVectors(ref Vector4[] vertices, Vector4 offset)
        {
            for (int idx = 0; idx < vertices.Length; idx++)
                vertices[idx] += offset;
        }
        public static Vector4[] Square4(Vector2 size, Orientation orientation, bool usingElements)
        {
            Vector4[] vertices;

            size /= 2.0f;
            float width = size.X;
            float height = size.Y;

            if(usingElements)
            {
                vertices = new Vector4[4];

                switch (orientation)
                {
                    case (Orientation.XZ):
                        vertices[0] = new Vector4(-width, 0, -height, 1);
                        vertices[1] = new Vector4(width, 0, -height, 1);
                        vertices[2] = new Vector4(width, 0, height, 1);
                        vertices[3] = new Vector4(-width, 0, height, 1);
                        break;
                    case (Orientation.XY):
                        vertices[0] = new Vector4(-width, -height, 0, 1);
                        vertices[1] = new Vector4(width, -height, 0, 1);
                        vertices[2] = new Vector4(width, height, 0, 1);
                        vertices[3] = new Vector4(-width, height, 0, 1);
                        break;
                    case (Orientation.YZ):
                        vertices[0] = new Vector4(0, -width, -height, 1);
                        vertices[1] = new Vector4(0, width, -height, 1);
                        vertices[2] = new Vector4(0, width, height, 1);
                        vertices[3] = new Vector4(0, -width, height, 1);
                        break;
                    default: //simply do XY otherwise
                        vertices[0] = new Vector4(-width, -height, 0, 1);
                        vertices[1] = new Vector4(width, -height, 0, 1);
                        vertices[2] = new Vector4(width, height, 0, 1);
                        vertices[3] = new Vector4(-width, height, 0, 1);
                        break;
                }
            }
            else
            {
                vertices = new Vector4[6];

                switch (orientation)
                {
                    case (Orientation.XZ):
                        vertices[0] = new Vector4(-width, 0, -height, 0);
                        vertices[1] = new Vector4(width, 0, -height, 0);
                        vertices[2] = new Vector4(width, 0, height, 0);
                        vertices[3] = new Vector4(width, 0, height, 0);
                        vertices[4] = new Vector4(-width, 0, height, 0);
                        vertices[5] = new Vector4(-width, 0, -height, 0);
                        break;
                    case (Orientation.XY):
                        vertices[0] = new Vector4(-width, -height, 0, 1);
                        vertices[1] = new Vector4(width, -height, 0, 1);
                        vertices[2] = new Vector4(width, height, 0, 1);
                        vertices[3] = new Vector4(width, height, 0, 1);
                        vertices[4] = new Vector4(-width, height, 0, 1);
                        vertices[5] = new Vector4(-width, -height, 0, 1);
                        break;
                    case (Orientation.YZ):
                        vertices[0] = new Vector4(0, -width, -height, 1);
                        vertices[1] = new Vector4(0, width, -height, 1);
                        vertices[2] = new Vector4(0, width, height, 1);
                        vertices[3] = new Vector4(0, width, height, 1);
                        vertices[4] = new Vector4(0, -width, height, 1);
                        vertices[5] = new Vector4(0, -width, -height, 1);
                        break;
                    default: //simply do XY otherwise
                        vertices[0] = new Vector4(-width, -height, 0, 1); 
                        vertices[1] = new Vector4(width, -height, 0, 1);
                        vertices[2] = new Vector4(width, height, 0, 1);
                        vertices[3] = new Vector4(-width, height, 0, 1);
                        break;
                }
            }

            return vertices;
        }
    }
}
