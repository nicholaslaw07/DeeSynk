using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;
using OpenTK;

namespace DeeSynk.Core.Components
{
    public struct SubTextureLocation
    {
        public Vector2 UVOffset;
        public Vector2 UVScale;

        public SubTextureLocation(Vector2 uvOffset, Vector2 uvScale)
        {
            UVOffset = uvOffset;
            UVScale = uvScale;
        }

        public SubTextureLocation(float offsetX, float offsetY, float scaleX, float scaleY)
        {
            UVOffset = new Vector2(offsetX, offsetY);
            UVScale = new Vector2(scaleX, scaleY);
        }

        public bool Exists()
        {
            if (UVOffset != null && UVScale != null)
                return true;
            return false;
        }
    }
    public class Texture
    {
        private int _textureId;
        public int TextureId
        {
            get => _textureId;
            set
            {
                if (GL.IsTexture(value))
                    _textureId = value;
            }
        }

        private int _width, _height;
        public int Width { get => _width; }
        public int Height { get => _height; }

        public float AspectRatio { get => ((float)_width) / _height; }

        private SubTextureLocation[] _subTextureLocations;
        public ref SubTextureLocation[] SubTextureLocations { get => ref _subTextureLocations; }

        private int _subTextureCount;
        public int SubTextureCount { get => _subTextureCount; }

        private int _availableSubLocations;
        public int AvailableSubLocations { get => _availableSubLocations; }

        public bool IsTextureAtlas { get => (_subTextureCount > 1); }

        public Texture(int textureId, int width, int height, int subTextureCount)
        {
            _textureId = textureId;
            _width = width;
            _height = height;
            _subTextureLocations = new SubTextureLocation[subTextureCount];
            _subTextureCount = subTextureCount;
            _availableSubLocations = 0;
        }

        public Texture(int textureId, int width, int height, SubTextureLocation[] subTextureLocations)
        {
            _textureId = textureId;
            _width = width;
            _height = height;
            _subTextureLocations = subTextureLocations;
            _subTextureCount = _subTextureLocations.Length;
            _availableSubLocations = _subTextureLocations.Length;
        }

        public bool AddSubTextureLocation(SubTextureLocation subTextureLocation)
        {
            int placementIndex = 0;
            for(placementIndex = 0; placementIndex < _subTextureCount; placementIndex++)
            {
                if(_subTextureLocations[placementIndex].UVScale == Vector2.Zero)
                {
                    _subTextureLocations[placementIndex] = subTextureLocation;
                    _availableSubLocations++;
                    return true;
                }
            }

            return false;
        }

        public bool AddSubTextureLocation(SubTextureLocation subTextureLocation, int idx)
        {
            if(idx >= 0 && idx < _subTextureCount)
            {
                if (_subTextureLocations[idx].UVOffset == null && _subTextureLocations[idx].UVScale == null)
                {
                    _subTextureLocations[idx] = subTextureLocation;
                    return true;
                }
            }
            return false;
        }

        public bool ValidSubLocation(int location)
        {
            if (!IsTextureAtlas)
            {
                if (location == 0)
                    return true;
                else
                    return false;
            }
            else if(location >= 0 && location < _subTextureCount)
            {
                if (_subTextureLocations[location].Exists())
                    return true;
            }

            return false;
        }

        public void Bind(TextureUnit textureUnit)
        {
            GL.ActiveTexture(textureUnit);
            if (GL.GetInteger(GetPName.ActiveTexture) != _textureId)
                GL.BindTexture(TextureTarget.Texture2D, _textureId);
        }
    }
}
