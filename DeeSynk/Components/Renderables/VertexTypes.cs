using System;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;

namespace DeeSynk.Components.Renderables
{
    public struct TexturedVertex
    {
        public const int Size = 24;

        public Vector4 Position;
        public Vector2 TexCoord;

        public TexturedVertex(Vector4 position, Vector2 textureCoord)
        {
            Position = position;
            TexCoord = textureCoord;
        }
    }

    public struct ColoredVertex
    {
        public const int Size = 32;

        public Vector4 Position;
        public Color4 Color;

        public ColoredVertex(Vector4 position, Color4 color)
        {
            Position = position;
            Color = color;
        }
    }
}
