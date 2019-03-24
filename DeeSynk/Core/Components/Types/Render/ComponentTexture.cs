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

        public const TextureUnit DEFAULT_TEXTURE_UNIT = TextureUnit.Texture0;

        private int _subTextureLocationIndex;
        public int SubTextureLocationIndex
        {
            get => _subTextureLocationIndex;
            set
            {
                if (_texture.ValidSubLocation(value))
                    _subTextureLocationIndex = value;
            }
        }

        private Texture _texture;
        public ref Texture Texture { get => ref _texture; }

        public ComponentTexture(Texture texture, int subTextureLocationIndex)
        {
            _texture = texture;
            _subTextureLocationIndex = (_texture.ValidSubLocation(subTextureLocationIndex)) ? subTextureLocationIndex : 0; //default is 0
        }

        public ComponentTexture(string textureName)
        {
            //Look up texture from manager and feed into here?
        }

        public void BindTexture()
        {
            _texture.Bind(DEFAULT_TEXTURE_UNIT);
        }

        public void BindTexture(TextureUnit textureUnit)
        {
            _texture.Bind(textureUnit);
        }

        public void Update(float time)
        {
            throw new NotImplementedException();
        }
    }
}
