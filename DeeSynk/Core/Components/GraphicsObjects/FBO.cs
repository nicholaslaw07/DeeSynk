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

        public void AddTexture()
        {
            _texture = new Texture(MainWindow.width, MainWindow.height);
            GL.BindTexture(TextureTarget.Texture2D, _texture.TextureId);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.LinearMipmapLinear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToBorder);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToBorder);

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, _fboID);
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.Color, TextureTarget.Texture2D, _texture.TextureId, 0);

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

        public void Bind()
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, _fboID);
        }
    }
}
