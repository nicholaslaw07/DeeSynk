﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Graphics;

namespace DeeSynk.Core.Components.GraphicsObjects.Lights
{
    public class SpotLight : Light
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
        private bool _projectionUpdate; //should not have to be used often

        private Matrix4 _projection;

        private float _fov, _aspect, _zNear, _zFar;
        public float FOV { get => _fov; set { _fov = value; _projectionUpdate = true; } }
        public float Aspect { get => _aspect; set { _aspect = value; _projectionUpdate = true; } } // most cases this should be 1.0
        public float ZNear { get => _zNear; set { _zNear = value; _projectionUpdate = true; } }
        public float ZFar { get => _zFar; set { _zFar = value; _projectionUpdate = true; } }

        #endregion

        private Matrix4 _viewProjection;
        public ref Matrix4 ViewProjectionMatrix => ref _viewProjection;

        public override int BufferSize => 16 * 8; //matrix, location, lookAt, color, fov (assume 1.0 aspect ratio)  (rgb, fov)

        #region Shadow Map Properties
        private bool _hasShadowMap;
        public override bool HasShadowMap => _hasShadowMap;

        private int _mapResX, _mapResY;
        public override int MapResolutionX => _mapResX;
        public override int MapResolutionY => _mapResY;
        #endregion

        public SpotLight(Color4 emissionColor, Vector3 location, Vector3 lookAt, Vector3 up, float fov, float aspect, float zNear, float zFar)
        {
            _emissionColor = emissionColor;

            _location = location;
            _lookAt = lookAt;
            _up = up;

            _fov = fov;
            _aspect = aspect;
            _zNear = zNear;
            _zFar = zFar;

            _view = Matrix4.LookAt(_location, _lookAt, _up);
            Matrix4.CreatePerspectiveFieldOfView(_fov, _aspect, _zNear, _zFar, out _projection);

            Matrix4.Mult(ref _view, ref _projection, out _viewProjection);
        }

        public override void BindShadowMap()
        {
            GL.BindFramebuffer(FramebufferTarget.DrawFramebuffer, _fbo);
            GL.Viewport(0, 0, _mapResX, _mapResY);
        }

        public override void UnbindShadowMap()
        {
            GL.BindFramebuffer(FramebufferTarget.DrawFramebuffer, 0);
        }

        public override void AddShadowMap(int width, int height)
        {
            //GL.GetInteger(GetPName.MaxTextureSize, out int maxSize);
            //is there error handling in opentk for textures that are too large?
            _mapResX = width;
            _mapResY = height;

            _fbo = GL.GenFramebuffer();
            _depthMap = GL.GenTexture();

            GL.BindTexture(TextureTarget.Texture2D, _depthMap);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.DepthComponent32, _mapResX, _mapResX, 0, PixelFormat.DepthComponent, PixelType.Float, IntPtr.Zero);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (float)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (float)TextureMagFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (float)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (float)TextureWrapMode.ClampToEdge);

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, _fbo);
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, TextureTarget.Texture2D, _depthMap, 0);

            GL.DrawBuffer(DrawBufferMode.None);

            GL.ReadBuffer(ReadBufferMode.None);

            var status = GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer);
            if (status != FramebufferErrorCode.FramebufferComplete)
                Console.WriteLine(status);

            GL.BindTexture(TextureTarget.Texture2D, 0);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);

            _hasShadowMap = true;
        }

        #region UBO Managment
        public override void AttachUBO(int bindingLocation)
        {
            GL.BindBufferRange(BufferRangeTarget.UniformBuffer, _bindingLocation, _ubo_Id, IntPtr.Zero, BufferSize);
        }

        public override void BuildUBO(int bindingLocation, int numOfVec4s)
        {
            _ubo_Id = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.UniformBuffer, _ubo_Id);
            _bufferData = new Vector4[numOfVec4s];
            _bindingLocation = bindingLocation;
            FillBuffer();
            GL.BufferData(BufferTarget.UniformBuffer, BufferSize, _bufferData, BufferUsageHint.DynamicRead);
            AttachUBO(bindingLocation);

            GL.GetBufferParameter(BufferTarget.UniformBuffer, BufferParameterName.BufferSize, out int sizeQuery);
            if (GL.IsBuffer(_ubo_Id) && sizeQuery == BufferSize)
                _initUBO = true;

            GL.BindBuffer(BufferTarget.UniformBuffer, 0);
        }

        public override void DetatchUBO()
        {
            GL.BindBufferRange(BufferRangeTarget.UniformBuffer, _bindingLocation, 0, IntPtr.Zero, 0);
        }

        public override void FillBuffer()
        {
            _bufferData[0] = _viewProjection.Row0;
            _bufferData[1] = _viewProjection.Row1;
            _bufferData[2] = _viewProjection.Row2;
            _bufferData[3] = _viewProjection.Row3;
            _bufferData[4] = new Vector4(_location, 1.0f);
            _bufferData[5] = new Vector4(_lookAt, 1.0f);
            _bufferData[6] = new Vector4(_emissionColor.R, _emissionColor.G, _emissionColor.B, 1.0f);
            _bufferData[7] = new Vector4((float)Math.Cos(_fov)); //make fov the alpha value of the color if data size is an issue
        }

        public override void UpdateUBO()
        {
            GL.BindBuffer(BufferTarget.UniformBuffer, _ubo_Id);
            FillBuffer();
            GL.BufferSubData(BufferTarget.UniformBuffer, IntPtr.Zero, BufferSize, _bufferData);
            GL.BindBuffer(BufferTarget.UniformBuffer, 0);
        }
        #endregion

        public void Update()
        {
            if(_viewUpdate || _projectionUpdate)
            {
                if (_viewUpdate)
                {
                    _view = Matrix4.LookAt(_location, _lookAt, _up);
                    _viewUpdate = false;
                }

                if (_projectionUpdate)
                {
                    Matrix4.CreatePerspectiveFieldOfView(_fov, _aspect, _zNear, _zFar, out _projection);
                    _projectionUpdate = false;
                }

                Matrix4.Mult(ref _view, ref _projection, out _viewProjection);

                if (_initUBO)
                    UpdateUBO();
            }
        }
    }
}
