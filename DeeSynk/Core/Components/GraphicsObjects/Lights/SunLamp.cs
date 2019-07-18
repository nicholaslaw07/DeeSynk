using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Graphics;

namespace DeeSynk.Core.Components.GraphicsObjects.Lights
{
    public class SunLamp : Light
    {
        public override Color4 EmissionColor { get => _emissionColor; set => _emissionColor = value; }

        #region View Properties
        private bool _viewUpdate;

        private Matrix4 _view;

        private Vector3 _location, _lookAt, _up;
        public Vector3 Location { get => _location; set { _location = value; _viewUpdate = true; } }
        public Vector3 LookAt { get => _lookAt; set { _lookAt = value; _viewUpdate = true; } }
        public Vector3 Up { get => _up; set { _up = value; _viewUpdate = true; } }
        #endregion

        #region Projection Properties
        private bool _projectionUpdate;

        private Matrix4 _projection;

        private float _width, _height, _zNear, _zFar;
        public float Width { get => _zNear; set { _zNear = value; _projectionUpdate = true; } }
        public float Height { get => _zFar; set { _zFar = value; _projectionUpdate = true; } }
        public float ZNear { get => _zNear; set { _zNear = value; _projectionUpdate = true; } }
        public float ZFar { get => _zFar; set { _zFar = value; _projectionUpdate = true; } }

        #endregion

        private Matrix4 _viewProjection;
        public ref Matrix4 ViewProjectionMatrix => ref _viewProjection;
        public override int BufferSize => 16 * 8; //matrix, location, lookAt, color, fov (assume 1.0 aspect ratio)  (rgb, fov)

        private TextureUnit _textureUnit;
        public TextureUnit TextureUnit => _textureUnit;

        #region Shadow Map Properties
        private bool _hasShadowMap;
        public override bool HasShadowMap => _hasShadowMap;

        private int _mapResX, _mapResY;
        public override int MapResolutionX => _mapResX;
        public override int MapResolutionY => _mapResY;
        #endregion

        public override void AddShadowMap(int width, int height, TextureUnit textureUnit)
        {
            throw new NotImplementedException();
        }

        public override void AttachUBO(int bindingLocation)
        {
            throw new NotImplementedException();
        }

        public override void BindShadowMapFBO()
        {
            throw new NotImplementedException();
        }

        public override void BindShadowMapTex()
        {
            throw new NotImplementedException();
        }

        public override void BindShadowMapTex(TextureUnit textureUnit)
        {
            throw new NotImplementedException();
        }

        public override void BuildUBO(int bindingLocation, int numOfVec4s)
        {
            throw new NotImplementedException();
        }

        public override void BuildUBO(int uboId, int uboSize, int offset, int bindingLocation, int numOfVec4s)
        {
            throw new NotImplementedException();
        }

        public override void DetatchUBO()
        {
            throw new NotImplementedException();
        }

        public override void FillBuffer()
        {
            throw new NotImplementedException();
        }

        public override void UnbindShadowMapFBO()
        {
            throw new NotImplementedException();
        }

        public override void UnbindShadowMapTex()
        {
            throw new NotImplementedException();
        }

        public override void UnbindShadowMapTex(TextureUnit textureUnit)
        {
            throw new NotImplementedException();
        }

        public override void UpdateUBO()
        {
            throw new NotImplementedException();
        }
    }
}
