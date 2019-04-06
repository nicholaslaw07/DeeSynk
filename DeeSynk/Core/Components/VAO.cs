using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;

namespace DeeSynk.Core.Components
{
    [Flags]
    public enum Buffers
    {
        NONE = 0,
        FACE_ELEMENTS = 1,
        VERTICES = 1 << 1,
        NORMALS = 1 << 2,
        COLORS = 1 << 3,
        UVS = 1 << 4,
        INSTANCES = 1 << 5,

        VERTICES_ELEMENTS = VERTICES | FACE_ELEMENTS,

        VERTICES_NORMALS = VERTICES | NORMALS,
        VERTICES_NORMALS_ELEMENTS = VERTICES_NORMALS | FACE_ELEMENTS,

        VERTICES_NORMALS_COLORS = VERTICES_NORMALS | COLORS,
        VERTICES_NORMALS_COLORS_ELEMENTS = VERTICES_NORMALS_COLORS | FACE_ELEMENTS,

        VERTICES_NORMALS_UVS = VERTICES_NORMALS | UVS,
        VERTICES_NORMALS_UVS_ELEMENTS = VERTICES_NORMALS_UVS | FACE_ELEMENTS
    }

    public class VAO
    {
        private const int NULL_BUFF = 0;
        private Buffers _buffers;
        public Buffers Buffers { get => _buffers; }

        private int _vaoId;
        public int VAOId { get => _vaoId; }

        private int[] _arrayBuffers;

        private int _elementsBuffer;

        public VAO()
        {
            _vaoId = GL.GenVertexArray();

            _buffers = Buffers.VERTICES;

            _elementsBuffer = NULL_BUFF;

            GL.GenBuffers(1, _arrayBuffers);
        }

        public VAO(Buffers buffers)
        {
            //ensure that we are not creating an empty VAO, create with a purpose
            if (buffers == Buffers.NONE)
                throw new ArgumentException("Cannot declare a usable VAO with no buffers attached.");  //NOTE: it is possible to have a 'null' vao but for our purposes we make it illegal

            //check for specific types of buffers and create them
            if (buffers.HasFlag(Buffers.FACE_ELEMENTS))
                _elementsBuffer = GL.GenBuffer();

            int buffCount = 0;
            for(int idx = 1; idx < 5; idx++)
            {
                if (((int)buffers & (1 << idx)) != 0)
                    buffCount++;
            }

            GL.GenBuffers(buffCount, _arrayBuffers);
        }
    }
}
