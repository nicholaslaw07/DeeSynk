using System;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;

namespace DeeSynk.Components.Renderables
{
    public struct TexturedVertex
    {
        public const int Size = 24;

        private readonly Vector4 _position;
        public Vector4 Position { get => _position; }

        private readonly Vector2 _textureCoord;
        public Vector2 TextureCoord { get => _textureCoord; }

        public TexturedVertex(Vector4 position, Vector2 textureCoord)
        {
            _position = position;
            _textureCoord = textureCoord;
        }
    }

    public struct ColoredVertex
    {
        public const int Size = 32;

        private readonly Vector4 _position;
        public Vector4 Position { get => _position; }

        private readonly Color4 _color;
        public Color4 Color { get => _color; }

        public ColoredVertex(Vector4 position, Color4 color)
        {
            _position = position;
            _color = color;
        }
    }
}
