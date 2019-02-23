using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;

namespace DeeSynk.Components.Renderables
{
    public class ColoredRenderObject : RenderObject
    {
        private ColoredVertex[]     _vertices;
        private int                 _vertexCount;
        protected ColoredVertex[]   Vertices { get => _vertices; }
        protected int               VertexCount { get => _vertexCount; }

        private int[]   _indices;
        private int     _indexCount;
        protected int[] Indices { get => _indices; }
        protected int   IndexCount { get => _indexCount; }

        public ColoredRenderObject(int renderID, int renderLayer, ColoredVertex[] vertices, int[] indices) : base(renderID, renderLayer)
        {
            _vertices = vertices;
            _vertexCount = _vertices.Length;

            _indices = indices;
            _indexCount = _indices.Length;
        }

        public ColoredRenderObject(int renderID, int renderLayer, Vector3 position, float rotX, float rotY, float rotZ, Vector3 scale, ColoredVertex[] vertices, int[] indices) : base(renderID, renderLayer, position, rotX, rotY, rotZ, scale)
        {
            _vertices = vertices;
            _vertexCount = _vertices.Length;

            _indices = indices;
            _indexCount = _indices.Length;
        }

        //It may be possible that we don't need to save the vertices if we can ensure that it will never be used again.  This is purely a small performance gain.
        //Must add a way to manage which objects have or don't have dynamic VBO's and determine the type that it is upon VAO Initialization (GL_DYNAMIC_STORAGE_BIT or GL_MAP_WRITE_BIT)
        public override RenderObject InitializeVAO()
        {
            if (_vertexCount <= _indexCount)
            {
                //IF VERTEX EQUAL 0 OR INDEX EQUAL 0 THROW ERROR
                GL.BindVertexArray(VAO);
                GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, IBO);

                GL.NamedBufferStorage(VBO, ColoredVertex.Size * _vertexCount, _vertices, BufferStorageFlags.MapWriteBit);

                GL.VertexArrayAttribBinding(VAO, 0, 0);
                GL.EnableVertexArrayAttrib(VAO, 0);
                GL.VertexArrayAttribFormat(VAO, 0, 4, VertexAttribType.Float, false, 0);

                GL.VertexArrayAttribBinding(VAO, 1, 0);
                GL.EnableVertexArrayAttrib(VAO, 1);
                GL.VertexArrayAttribFormat(VAO, 1, 4, VertexAttribType.Float, false, 16);

                GL.VertexArrayVertexBuffer(VAO, 0, VBO, IntPtr.Zero, ColoredVertex.Size);
                GL.NamedBufferStorage(IBO, 4 * _indexCount, _indices, BufferStorageFlags.MapWriteBit);

                GL.BindVertexArray(0);

                InitVAO = true;
            }
            else
            {
                Visible = false;
            }

            return this;
        }

        public override void Update()
        {
            throw new NotImplementedException();
        }

        public override void Render()
        {
            GL.UseProgram(ActiveProgramID);
            GL.BindVertexArray(VAO);

            GL.DrawElements(BeginMode.Triangles, IndexCount, DrawElementsType.UnsignedInt, 0);

            GL.BindVertexArray(0);
            GL.UseProgram(0);
        }
    }
}
