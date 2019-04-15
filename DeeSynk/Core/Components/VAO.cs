using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DeeSynk.Core.Systems;
using OpenTK.Graphics.OpenGL4;

namespace DeeSynk.Core.Components
{
    public class VAO
    {
        public static readonly int IndexData = 0;
        public static readonly int VertexDataNoIBO= 0;
        public static readonly int VertexDataWithIBO = 1;

        private Buffers _bufferConfig;
        public Buffers BufferConfig { get => _bufferConfig; }

        private int _id;
        public int Id {get => _id;}

        private readonly int[] _buffers;
        public int[] Buffers { get => _buffers; }

        /// <summary>
        /// Auto-generates a new VAO and binds it
        /// </summary>
        public VAO(Buffers bufferConfig)
        {
            _bufferConfig = bufferConfig;
            _id = GL.GenVertexArray();
            GL.BindVertexArray(_id);
            int[] buffs = new int[BufferCount(_bufferConfig)];
            GL.GenBuffers(buffs.Length, buffs);
            _buffers = buffs;
        }

        private int BufferCount(Buffers buffers)
        {
            int count = 0;
            if (buffers.HasFlag(Systems.Buffers.FACE_ELEMENTS)) count++;
            if (buffers.HasFlag(Systems.Buffers.INSTANCES)) count++;
            if (buffers.HasFlag(Systems.Buffers.INTERLEAVED)) count++;
            else for (int i = 1; i <= 4; i++) { count += (buffers.HasFlag((Buffers)(1 << i))) ? 1 : 0; }

            return count;
        }

        public void Bind()
        {
            BindVAO();
            if(_bufferConfig.HasFlag(Systems.Buffers.FACE_ELEMENTS))
                BindIBO();
        }

        public void BindVAO()
        {
            if (GL.GetInteger(GetPName.VertexArrayBinding) != _id)
                GL.BindVertexArray(_id);
        }

        public void BindIBO()
        {
            if (GL.GetInteger(GetPName.ElementArrayBufferBinding) != _buffers[IndexData])
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, _buffers[IndexData]);

        }

        public void UnBind()
        {
            GL.BindVertexArray(0);
        }

        public void UnBindIBO()
        {
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
        }
    }
}
