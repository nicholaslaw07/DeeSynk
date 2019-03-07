using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;

namespace DeeSynk.Core.Components.Types.Render
{
    public class ComponentTexture : IComponent
    {
        public int BitMaskID => (int)Component.TEXTURE;

        private Vector2[] _texCoords;
        public Vector2[] TextureCoodinates { get => _texCoords; }

        private int _texCount;
        public int TextureCount { get => _texCount; }

        private int _texID;
        public int TextureID { get => _texID; }

        public ComponentTexture()
        {
            _texCoords = new Vector2[0];
            _texCount = 0;
            _texID = 0;
        }

        public ComponentTexture(ref Vector2[] texCoords, int texID)
        {
            _texCoords = texCoords;
            _texCount = _texCoords.Length;
            _texID = texID;
        }

        public void BindTexture()
        {
            int data = 0;
            GL.GetInteger(GetPName.ActiveTexture, out data);
            if (data != _texID)
                GL.BindTexture(TextureTarget.Texture2D, _texID);
        }

        public void Update(float time)
        {
            throw new NotImplementedException();
        }
    }
}
