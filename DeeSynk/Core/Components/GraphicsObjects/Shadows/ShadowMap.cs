using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeeSynk.Core.Components.GraphicsObjects.Shadows
{
    //NOTE: SM = Shadow Map
    //      TU = Texture Unit
    //      FBO = Frame Buffer Object
    public class ShadowMap
    {
        public const int RES_DEFAULT = 1024;
        public const TextureUnit TEXUNIT_DEFAULT = TextureUnit.Texture0;

        private bool _init;
        public bool Init { get => _init; }

        private int _fbo, _texture, _resX, _resY;

        public int FBO { get => _fbo; }
        public int Texture { get => _texture; }
        public int ResX { get => _resX; }
        public int ResY { get => _resY; }

        private TextureUnit _textureUnit;
        public TextureUnit TextureUnit { get => _textureUnit; }

        public ShadowMap()
        {
            InitShadowMap(RES_DEFAULT, RES_DEFAULT, TEXUNIT_DEFAULT);
        }

        public ShadowMap(int resX, int resY, TextureUnit textureUnit)
        {
            InitShadowMap(resX, resY, textureUnit);
        }

        private void InitShadowMap(int resX, int resY, TextureUnit textureUnit)
        {
            _resX = resX;
            _resY = resY;

            _textureUnit = textureUnit;

            GL.GenFramebuffers(1, out _fbo);
            GL.GenTextures(1, out _texture);

            GL.BindTexture(TextureTarget.Texture2D, _texture);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.DepthComponent32, _resX, _resY, 0, PixelFormat.DepthComponent, PixelType.Float, IntPtr.Zero);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (float)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (float)TextureMagFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (float)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (float)TextureWrapMode.ClampToEdge);

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, _fbo);
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, TextureTarget.Texture2D, _texture, 0);

            GL.DrawBuffer(DrawBufferMode.None);

            GL.ReadBuffer(ReadBufferMode.None);

            var status = GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer);
            if (status != FramebufferErrorCode.FramebufferComplete)
                Console.WriteLine(status);
            else
                _init = true;

            GL.BindTexture(TextureTarget.Texture2D, 0);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        }

        /// <summary>
        /// Binds the FBO associated with this shadow map to the active FBO slot
        /// </summary>
        public void BindFBO()
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, _fbo);
            GL.Viewport(0, 0, _resX, _resY);
        }

        /// <summary>
        /// Resets the active FBO slot to the rendering default (FBO ID = 0)
        /// </summary>
        public void UnbindFBO()
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        }

        /// <summary>
        /// Binds the texture associated with this shadow map to the texture unit field stored in this shadow map
        /// </summary>
        public void BindTexture()
        {
            GL.ActiveTexture(_textureUnit);
            GL.BindTexture(TextureTarget.Texture2D, _texture);
        }

        /// <summary>
        /// Sets the active texture in the texture unit assigned upon construction to none
        /// </summary>
        public void UnbindTexture()
        {
            GL.ActiveTexture(_textureUnit);
            GL.BindTexture(TextureTarget.Texture2D, 0);
        }

        /// <summary>
        /// Binds the texture associated with this shadow map to the texture unit provided as a parameter
        /// </summary>
        /// <param name="textureUnit">The texture unit to bind this shadow map's texture to</param>
        /// <param name="retain">Determines whether or not to store the passed texture unit as the new preference for this shadow map </param>
        public void BindTexture(TextureUnit textureUnit, bool retain)
        {
            GL.ActiveTexture(textureUnit);
            GL.BindTexture(TextureTarget.Texture2D, _texture);
        }

        /// <summary>
        /// Resets the texture id at the passed texture unit to the default of none
        /// </summary>
        /// <param name="textureUnit"></param>
        public void UnbindTexture(TextureUnit textureUnit)
        {
            GL.ActiveTexture(textureUnit);
            GL.BindTexture(TextureTarget.Texture2D, 0);
        }
    }
}
