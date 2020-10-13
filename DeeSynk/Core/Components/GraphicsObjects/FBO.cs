using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK.Graphics.OpenGL4;

namespace DeeSynk.Core.Components.GraphicsObjects
{
    public class FBO
    {
        private int _fboID;
        /// <summary>
        /// OpenGL refereced ID for this FBO.
        /// </summary>
        public int FBO_ID { get => _fboID; }


        private bool _init;
        public bool Init { get => _init; }

        private Texture _texture;
        /// <summary>
        /// The DeeSynk Texture object associated with this FBO.
        /// </summary>
        public Texture Texture { get => _texture; }

        public bool HasTexture2D { get => _texture != null; }

        public FBO(bool addTex)
        {
            GL.CreateFramebuffers(1, out _fboID);

            if (addTex) { AddTexture(); }
        }

        //Ideally this should be more customizable but I can't think of a use case for such customizability now.
        //The general philsophy that I have right now is if I don't see a need for it I'm not going to implement it.
        //Maybe this will bite me in the ass as I'm not doing as much preventative maintanence then.
        public void AddTexture()
        {
            _texture = new Texture(Program.window.Width, Program.window.Height);

            GL.GenRenderbuffers(1, out int rbo);
            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, rbo);
            GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, RenderbufferStorage.DepthComponent, _texture.Width, _texture.Height);

            //standard texture setup
            GL.BindTexture(TextureTarget.Texture2D, _texture.TextureId);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, _texture.Width, _texture.Height, 0, PixelFormat.Bgra, PixelType.Float, IntPtr.Zero);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.LinearMipmapLinear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToBorder);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToBorder);
            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);


            GL.BindFramebuffer(FramebufferTarget.Framebuffer, _fboID);
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D, _texture.TextureId, 0);
            GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, RenderbufferTarget.Renderbuffer, rbo);

            var status = GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer);
            if (status != FramebufferErrorCode.FramebufferComplete)
                Console.WriteLine(status);
            else
                _init = true;

            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, 0);
            GL.BindTexture(TextureTarget.Texture2D, 0);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        }

        public void Bind(bool clearBuffer)
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, _fboID);
            GL.Viewport(0, 0, _texture.Width, _texture.Height);

            if (clearBuffer) { GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit); }
        }
    }
}
