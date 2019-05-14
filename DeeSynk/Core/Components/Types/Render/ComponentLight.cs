using OpenTK;
using OpenTK.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;
using DeeSynk.Core.Components.Models;

namespace DeeSynk.Core.Components.Types.Render
{
    public enum LightType
    {
        SPOTLIGHT = 1,
        SUN = 2,
        POINT = 3
    }

    public class ComponentLight : IComponent
    {
        public Component BitMaskID => throw new NotImplementedException();

        private LightType _lightType;
        public LightType LightType { get => _lightType; }

        //REMOVE-
        private Camera _camera;
        public Camera Camera { get => _camera; }
        //-REMOVE?

        /*
        private Vector3 _location, _lookAt, _up;
        public Vector3 Location { get => _location; }
        public Vector3 LookAt { get => _lookAt; }
        public Vector3 Up { get => _up; }

        private Matrix4 _view;
        public ref Matrix4 View { get => ref _view; }

        private Quaternion _qx, _qy, p, _qyi, _qxi;
        */

        public static readonly Color4 DEFAULT_COLOR = Color4.White;
        public static readonly TextureUnit DEFAULT_SHADOW_BINDING = TextureUnit.Texture1;
        public static readonly int DEFAULT_BINDING_BASE = 3; //will have multiple bindings for diferent types of lights

        private Color4 _lightColor, _shadowColor;
        public Color4 LightColor { get => _lightColor; }
        public Color4 ShadowColor { get => _shadowColor; }

        private bool _castShadows;
        public bool CastShadows { get => _castShadows; set => _castShadows = value; }

        private int _fbo;
        public int FBO { get => _fbo; }

        private int _depthMap;
        public int DepthMap { get => _depthMap; }

        private int _resX, _resY;
        public int ResolutionX { get => _resX; }
        public int ResolutionY { get => _resY; }

        private int _ubo;
        public int UBO { get => _ubo; }

        private int _uboBinding;
        public int UBOBinding { get => _uboBinding; }

        private Vector4[] vecs;

        public ComponentLight(Camera camera, Color4 lightColor, Color4 shadowColor, bool castShadows, int resolutionX, int resolutionY)
        {
            _camera = camera;
            if (camera.Mode == CameraMode.ORTHOGRAPHIC || camera.Mode == CameraMode.ORTHOGRAPHIC_OFF_CENTER)
                _lightType = LightType.SUN;
            else
                _lightType = LightType.SPOTLIGHT;

            _lightColor = lightColor;

            if (castShadows)
            {
                _castShadows = castShadows;
                _shadowColor = shadowColor;

                _resX = resolutionX;
                _resY = resolutionY;

                AddShadowMap();
            }
        }

        //*************NOTE*************//
        // DETERMINE A WAY TO ADD ALL   //
        // OBJECTS THAT WILL BE STORED  //
        // IN A UBO UNDER ONE MAIN UBO  //
        // IF POSSIBLE. SIZE IS LIMITED //
        // TO 16KB, SO IT IS POSSIBLE   //
        // THAT MULTIPLE WILL BE NEEDED //
        // THIS WILL SAVE THE ISSUE OF  //
        // CONSTANTLY CHANGING BUFFERS  //
        // AND INSTEAD DETERMINE WHICH  //
        // BUFFER LOCATIONS GO WHERE    //
        // *****************************//

        //*************NOTE*************//
        // ADD BAKED SHADOWS.  THAT IS, //
        // ONLY UPDATE SHADOWS FOR ANY  //
        // OBJECTS THAT MOVE, THERE CAN //
        // BE SEPERATE SHADOWS MAPS FOR //
        // OBJECTS THAT DON"T MOVE, EVEN//
        // IF THEY OCCUPY THE SAME FIELD//
        // AS ONES THAT ARE MOVING      //
        //******************************//

        public void Bind()
        {
            GL.BindFramebuffer(FramebufferTarget.DrawFramebuffer, _fbo);
            GL.Viewport(0, 0, _resX, _resY);
        }

        public void UnBind()
        {
            GL.BindFramebuffer(FramebufferTarget.DrawFramebuffer, 0);
        }

        private void AddShadowMap()
        {
            _fbo = GL.GenFramebuffer();
            _depthMap = GL.GenTexture();

            GL.BindTexture(TextureTarget.Texture2D, _depthMap);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.DepthComponent32, _resX, _resY, 0, PixelFormat.DepthComponent, PixelType.Float, IntPtr.Zero);
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

        }

        public void BuildUBO(int bindingLocation)
        {
            _ubo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.UniformBuffer, _ubo);

            _uboBinding = bindingLocation;
            vecs = new Vector4[7];
            FillVecsArray();

            GL.BufferData(BufferTarget.UniformBuffer, vecs.Length * Model.VERTEX_SIZE, vecs, BufferUsageHint.DynamicRead);
            GL.BindBufferRange(BufferRangeTarget.UniformBuffer, _uboBinding, _ubo, IntPtr.Zero, vecs.Length * Model.VERTEX_SIZE);
            GL.BindBuffer(BufferTarget.UniformBuffer, 0);
            //FINISH IMPLEMENTING UBO

            //DETERMINE IF CAMERA SHOULD MOVE TO COMPONENT CAMERA
            //THUS MAKING LIGHT REQUIRE ITS OWN UNIQUE IMPLEMENTATION
            //IS IT MORE EFFICIENT TO SHARE OBJECT TYPES OR CREATE NEW
            //ONES THAT ARE MORE SUITED TO THE APPLICATION?

            //PROBALBY THE LATTER
        }

        private void UpdateUBO()
        {
            GL.BindBuffer(BufferTarget.UniformBuffer, _ubo);
            FillVecsArray();
            GL.BufferSubData(BufferTarget.UniformBuffer, IntPtr.Zero, vecs.Length * Model.VERTEX_SIZE, vecs);
            GL.BindBuffer(BufferTarget.UniformBuffer, 0);
        }

        private void FillVecsArray()
        {
            //include sorting by types, or placing lights into the appropriate bucket (requires more robust lights implementation)
            var mat = _camera.ViewProjection;
            vecs[0] = mat.Row0;
            vecs[1] = mat.Row1;
            vecs[2] = mat.Row2;
            vecs[3] = mat.Row3;
            vecs[4] = new Vector4(_camera.Location);
            vecs[5] = new Vector4(_lightColor.R, _lightColor.G, _lightColor.B, _lightColor.A);
            vecs[6] = new Vector4(_shadowColor.R, _shadowColor.G, _shadowColor.B, _shadowColor.A);
        }

        public void Update()
        {
            _camera.UpdateMatrices();
            UpdateUBO();
        }
    }
}
