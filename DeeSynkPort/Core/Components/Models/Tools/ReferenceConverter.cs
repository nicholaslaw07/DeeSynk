using DeeSynk.Core.Components.UI;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;

namespace DeeSynk.Core.Components.Models.Tools
{
    public static class ReferenceConverter
    {
        public static Vector2 GetReferenceOffset2(PositionReference reference, Vector2 size)
        {
            size /= 2.0f;
            switch (reference)
            {
                case (PositionReference.CENTER):
                    return new Vector2(0.0f, 0.0f);
                case (PositionReference.CORNER_BOTTOM_LEFT):
                    return new Vector2( size.X,  size.Y);
                case (PositionReference.CORNER_BOTTOM_RIGHT):
                    return new Vector2(-size.X,  size.Y);
                case (PositionReference.CORNER_TOP_LEFT):
                    return new Vector2( size.X, -size.Y);
                case (PositionReference.CORNER_TOP_RIGHT):
                    return new Vector2(-size.X, -size.Y);
                case (PositionReference.CENTER_LEFT):
                    return new Vector2( size.X, 0.0f);
                case (PositionReference.CENTER_RIGHT):
                    return new Vector2(-size.X, 0.0f);
                case (PositionReference.BOTTOM_CENTER):
                    return new Vector2( 0.0f, size.Y);
                case (PositionReference.TOP_CENTER):
                    return new Vector2( 0.0f,-size.Y);
                default:
                    return new Vector2(0.0f, 0.0f);
            }
        }

        public static Vector3 GetReferenceOffset3(PositionReference reference, Vector2 size)
        {
            size /= 2.0f;
            switch (reference)
            {
                case (PositionReference.CENTER):
                    return new Vector3(0.0f, 0.0f, 0.0f);
                case (PositionReference.CORNER_BOTTOM_LEFT):
                    return new Vector3(size.X, size.Y, 0.0f);
                case (PositionReference.CORNER_BOTTOM_RIGHT):
                    return new Vector3(-size.X, size.Y, 0.0f);
                case (PositionReference.CORNER_TOP_LEFT):
                    return new Vector3(size.X, -size.Y, 0.0f);
                case (PositionReference.CORNER_TOP_RIGHT):
                    return new Vector3(-size.X, -size.Y, 0.0f);
                case (PositionReference.CENTER_LEFT):
                    return new Vector3(size.X, 0.0f, 0.0f);
                case (PositionReference.CENTER_RIGHT):
                    return new Vector3(-size.X, 0.0f, 0.0f);
                case (PositionReference.BOTTOM_CENTER):
                    return new Vector3(0.0f, size.Y, 0.0f);
                case (PositionReference.TOP_CENTER):
                    return new Vector3(0.0f, -size.Y, 0.0f);
                default:
                    return new Vector3(0.0f, 0.0f, 0.0f);
            }
        }

        public static Vector4 GetReferenceOffset4(PositionReference reference, Vector2 size, bool includeW)
        {
            size /= 2.0f;
            float w = (includeW) ? 1.0f : 0.0f;
            switch (reference)
            {
                case (PositionReference.CENTER):
                    return new Vector4(0.0f, 0.0f, 0.0f, w);
                case (PositionReference.CORNER_BOTTOM_LEFT):
                    return new Vector4(size.X, size.Y, 0.0f, w);
                case (PositionReference.CORNER_BOTTOM_RIGHT):
                    return new Vector4(-size.X, size.Y, 0.0f, w);
                case (PositionReference.CORNER_TOP_LEFT):
                    return new Vector4(size.X, -size.Y, 0.0f, w);
                case (PositionReference.CORNER_TOP_RIGHT):
                    return new Vector4(-size.X, -size.Y, 0.0f, w);
                case (PositionReference.CENTER_LEFT):
                    return new Vector4(size.X, 0.0f, 0.0f, w);
                case (PositionReference.CENTER_RIGHT):
                    return new Vector4(-size.X, 0.0f, 0.0f, w);
                case (PositionReference.BOTTOM_CENTER):
                    return new Vector4(0.0f, size.Y, 0.0f, w);
                case (PositionReference.TOP_CENTER):
                    return new Vector4(0.0f, -size.Y, 0.0f, w);
                default:
                    return new Vector4(0.0f, 0.0f, 0.0f, w);
            }
        }
    }
}
