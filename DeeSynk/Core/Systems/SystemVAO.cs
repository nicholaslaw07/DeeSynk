using DeeSynk.Core.Components;
using DeeSynk.Core.Components.Types.Render;
using DeeSynk.Core.Components.Types.Transform;
using DeeSynk.Core.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;
using OpenTK;
using OpenTK.Graphics;

namespace DeeSynk.Core.Systems
{
    public enum VAOTypes
    {
        Colored = 1,
        Textured = 1 << 1,
        Indexed = 1 << 2,
        Instanced = 1 << 3,
        Interleaved = 1 << 4
    }

    class SystemVAO : ISystem
    {
        public int MonitoredComponents => (int)Component.COLOR  | 
                                          (int)Component.MODEL  | 
                                          (int)Component.RENDER | 
                                          (int)Component.TEXTURE|
                                          (int)Component.TRANSFORM;  //will also probably include at the very least Transform for instanced rendering

        //purpose: used for the creation of VAO's to organize like rendered objects into groups
        // and create VAO's for different use cases like for textured or non-textured

        //      VAO SETUP
        //    0 - VERTICES
        //    1 - COLORS OR UVS
        //    2 - (occupied by MVP matrix or VP for instanced)
        //    3 - (normals if/when added)
        //    4 - MAT4 1
        //    5 - MAT4 2
        //    6 - MAT4 3
        //    7 - MAT4 4

        public static readonly int ATTR_VERTEX = 0;

        public static readonly int ATTR_COLOR = 1;
        public static readonly int ATTR_UV = 1;

        public static readonly int ATTR_MVP = 2;
        public static readonly int ATTR_VP = 2;

        public static readonly int ATTR_NORM = 3;

        public static readonly int ATTR_MAT4_0 = 4;
        public static readonly int ATTR_MAT4_2 = 5;
        public static readonly int ATTR_MAT4_3 = 6;
        public static readonly int ATTR_MAT4_4 = 7;


        private const int VERTEX_SIZE = 16;
        private const int COLOR_SIZE = 16;
        private const int UV_SIZE = 8;
        private const int UINT_SIZE = 4;

        private World _world;

        private bool[] _monitoredGameObjects;

        private ComponentRender[] _renderComps;
        private ComponentModel[] _modelComps;
        private ComponentTexture[] _textureComps;
        private ComponentColor[] _colorComps;

        private ComponentTransform[] _transComps;

        public SystemVAO(World world)
        {
            _world = world;

            _monitoredGameObjects = new bool[_world.ObjectMemory];

            _renderComps = _world.RenderComps;
            _modelComps = _world.ModelComps;
            _textureComps = _world.TextureComps;
            _colorComps = _world.ColorComps;

            _transComps = _world.TransComps;
        }

        public void InitModels()
        {
            /*
            var color4Arr = new Color4[4];
            color4Arr[0] = Color4.Red;
            color4Arr[1] = Color4.Green;
            color4Arr[2] = Color4.Blue;
            color4Arr[3] = Color4.Yellow;
            */

            int texID = TextureManager.GetInstance().GetTexture("Ball");

            var uvArr = new Vector2[6];
            uvArr[0] = new Vector2(0.0f, 0.0f);
            uvArr[1] = new Vector2(1.0f, 0.0f);
            uvArr[2] = new Vector2(1.0f, 1.0f);
            uvArr[3] = new Vector2(1.0f, 1.0f);
            uvArr[4] = new Vector2(0.0f, 1.0f);
            uvArr[5] = new Vector2(0.0f, 0.0f);

            for (int i = 0; i < _world.ObjectMemory; i++)
            {
                _modelComps[i] = new ComponentModel(10f, 10f, true);
                //_colorComps[i] = new ComponentColor(color4Arr);
                _textureComps[i] = new ComponentTexture(ref uvArr, texID);
            }
        }

        public void InitModels(int idx)
        {
            /*
            var color4Arr = new Color4[4];
            color4Arr[0] = Color4.Red;
            color4Arr[1] = Color4.Green;
            color4Arr[2] = Color4.Blue;
            color4Arr[3] = Color4.Yellow;
            */

            int texID = TextureManager.GetInstance().GetTexture("Ball");

            var uvArr = new Vector2[6];
            uvArr[0] = new Vector2(0.0f, 0.0f);
            uvArr[1] = new Vector2(1.0f, 0.0f);
            uvArr[2] = new Vector2(1.0f, 1.0f);
            uvArr[3] = new Vector2(1.0f, 1.0f);
            uvArr[4] = new Vector2(0.0f, 1.0f);
            uvArr[5] = new Vector2(0.0f, 0.0f);

            _modelComps[idx] = new ComponentModel(10f, 10f, true);
            //_colorComps[idx] = new ComponentColor(color4Arr);
            _textureComps[idx] = new ComponentTexture(ref uvArr, texID);
        }

        /// <summary>
        /// Initializes a VAO or VAOs for all objects within a specified range with a specified configuration.
        /// </summary>
        /// <param name="vaoMask">A bit mask to determine the setup of the VAO with the 'VAOTypes' enum.</param>
        /// <param name="start">The starting index in the GameObject components to pull data from.</param>
        /// <param name="end">The ending index in the GameObject components to pull data from (inclusive).</param>
        /// <param name="groupTogether">Determines whether or not group the objects within the specified range under the same VAO or with a unique VAO for each.</param>
        public void InitVAOInRange(int vaoMask, int start, int end, bool groupTogether)
        {
            if(start <= end &&
               start >= 0 &&
               start < _world.ObjectMemory &&
               end >= 0 &&
               end < _world.ObjectMemory)
            {
                int vao = 0;
                int ibo = 0;
                int programID = 0;

                var shaderManager = ShaderManager.GetInstance();

                if (groupTogether)
                {
                    vao = GL.GenVertexArray();
                    GL.BindVertexArray(vao);

                    AddVertices(start, end);

                    if ((vaoMask & (int)VAOTypes.Colored) != 0 && (vaoMask & (int)VAOTypes.Textured) == 0)
                    {
                        AddColorBuffer(start, end);
                        programID = shaderManager.GetProgram("defaultColored");
                    }else if((vaoMask & (int)VAOTypes.Colored) == 0 && (vaoMask & (int)VAOTypes.Textured) != 0)
                    {
                        AddUVBuffer(start, end);
                        programID = shaderManager.GetProgram("defaultTextured");
                    }

                    if ((vaoMask & (int)VAOTypes.Indexed) != 0)
                        AddElementsBuffer(start, end, out ibo);

                    GL.BindVertexArray(0);

                    for (int idx= start; idx <= end; idx++)
                    {
                        _renderComps[idx] = new ComponentRender(vaoMask);
                        _renderComps[idx].AddVAOData(vao, ibo, programID, end - start + 1);
                        _renderComps[idx].ValidateData();
                    }
                }
            }
        }

        /// <summary>
        /// Initialize an instanced VAO for objects within a specified index range.  Pulls model data from the object at the start index.
        /// </summary>
        /// <param name="vaoMask">A bit mask to determine the setup of the VAO with the 'VAOTypes' enum.</param>
        /// <param name="start">The starting index in the GameObject components to pull data from.</param>
        /// <param name="count">The number of objects (including the starting index) to include in the VAO beginning from the starting index.</param>
        public void InitVAOInRange(int vaoMask, int start, int count)
        {
            if (start >= 0 &&
               start < _world.ObjectMemory &&
               start + count - 1 < _world.ObjectMemory &&
               (vaoMask & (int)VAOTypes.Instanced) != 0)
            {
                int vao = 0;
                int ibo = 0;
                int programID = 0;

                var shaderManager = ShaderManager.GetInstance();

                vao = GL.GenVertexArray();
                GL.BindVertexArray(vao);

                AddVertices(start);

                if ((vaoMask & (int)VAOTypes.Colored) != 0 && (vaoMask & (int)VAOTypes.Textured) == 0)
                {
                    AddColorBuffer(start);
                    programID = shaderManager.GetProgram("defaultColored_Instanced");  //WARNING, DOESNT EXIST!
                }
                else if ((vaoMask & (int)VAOTypes.Colored) == 0 && (vaoMask & (int)VAOTypes.Textured) != 0)
                {
                    AddUVBuffer(start);
                    programID = shaderManager.GetProgram("defaultTextured_Instanced");
                }

                if ((vaoMask & (int)VAOTypes.Indexed) != 0)
                {
                    AddElementsBuffer(start, out ibo);
                    AddMatrixBuffers(start, count, 1);
                }
                else
                {
                    AddMatrixBuffers(start, count, 1);
                }

                GL.BindVertexArray(0);

                _renderComps[start] = new ComponentRender(vaoMask);
                _renderComps[start].AddVAOData(vao, ibo, programID, count);
                _renderComps[start].ValidateData();
            }
        }

        private void AddVertices(int lowerBound, int upperBound)
        {
            int vbo = GL.GenBuffer();
            int vertexCount = 0;
            for (int idx = lowerBound; idx <= upperBound; idx++)
                vertexCount += _modelComps[idx].VertexCount;

            int kdx = 0;
            Vector4[] vertices = new Vector4[vertexCount];
            for (int idx = lowerBound; idx <= upperBound; idx++)
            {
                for (int jdx = 0; jdx < _modelComps[idx].VertexCount; jdx++)
                {
                    vertices[kdx] = _modelComps[idx].Vertices[jdx];
                    kdx++;
                }
            }

            int dataSize = VERTEX_SIZE * vertexCount;

            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.NamedBufferStorage(vbo, dataSize, vertices, BufferStorageFlags.MapReadBit);

            GL.BindVertexBuffer(0, vbo, IntPtr.Zero, VERTEX_SIZE);
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribFormat(0, 4, VertexAttribType.Float, false, 0);
            GL.VertexAttribBinding(0, 0);
        }
        private void AddVertices(int idx)
        {
            int vbo = GL.GenBuffer();
            int vertexCount = _modelComps[idx].VertexCount;

            Vector4[] vertices = _modelComps[idx].Vertices;

            int dataSize = VERTEX_SIZE * vertexCount;

            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.NamedBufferStorage(vbo, dataSize, vertices, BufferStorageFlags.MapReadBit);

            GL.BindVertexBuffer(0, vbo, IntPtr.Zero, VERTEX_SIZE);
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribFormat(0, 4, VertexAttribType.Float, false, 0);
            GL.VertexAttribBinding(0, 0);
            GL.VertexAttribDivisor(0, 0);
        }

        private void AddColorBuffer(int lowerBound, int upperBound)
        {
            int cbo = GL.GenBuffer();
            int colorCount = 0;
            for (int idx = lowerBound; idx <= upperBound; idx++)
                colorCount += _colorComps[idx].ColorCount;

            int kdx = 0;
            Color4[] colors = new Color4[colorCount];
            for (int idx = lowerBound; idx <= upperBound; idx++)
            {
                for (int jdx = 0; jdx < _colorComps[idx].ColorCount; jdx++)
                {
                    colors[kdx] = _colorComps[idx].Colors[jdx];
                    kdx++;
                }
            }

            int dataSize = COLOR_SIZE * colorCount;

            GL.BindBuffer(BufferTarget.ArrayBuffer, cbo);
            GL.NamedBufferStorage(cbo, dataSize, colors, BufferStorageFlags.MapReadBit);

            GL.BindVertexBuffer(1, cbo, IntPtr.Zero, COLOR_SIZE);
            GL.EnableVertexAttribArray(1);
            GL.VertexAttribFormat(1, dataSize, VertexAttribType.Float, false, 0);
            GL.VertexAttribBinding(1, 1);
        }
        private void AddColorBuffer(int idx)
        {
            int cbo = GL.GenBuffer();
            int colorCount = _colorComps[idx].ColorCount;

            Color4[] colors = _colorComps[idx].Colors;

            int dataSize = COLOR_SIZE * colorCount;

            GL.BindBuffer(BufferTarget.ArrayBuffer, cbo);
            GL.NamedBufferStorage(cbo, dataSize, colors, BufferStorageFlags.MapReadBit);

            GL.BindVertexBuffer(1, cbo, IntPtr.Zero, COLOR_SIZE);
            GL.EnableVertexAttribArray(1);
            GL.VertexAttribFormat(1, dataSize, VertexAttribType.Float, false, 0);
            GL.VertexAttribBinding(1, 1);
            GL.VertexAttribDivisor(1, 0);
        }

        private void AddUVBuffer(int lowerBound, int upperBound)
        {
            int tbo = GL.GenBuffer();
            int uvCount = 0;
            for (int idx = lowerBound; idx <= upperBound; idx++)
                uvCount += _textureComps[idx].TextureCount;

            int kdx = 0;
            Vector2[] uvCoords = new Vector2[uvCount];
            for (int idx = lowerBound; idx <= upperBound; idx++)
            {
                for (int jdx = 0; jdx < _textureComps[idx].TextureCount; jdx++)
                {
                    uvCoords[kdx] = _textureComps[idx].TextureCoodinates[jdx];
                    kdx++;
                }
            }

            int dataSize = UV_SIZE * uvCount;

            GL.BindBuffer(BufferTarget.ArrayBuffer, tbo);
            GL.NamedBufferStorage(tbo, dataSize, uvCoords, BufferStorageFlags.MapReadBit);

            GL.BindVertexBuffer(1, tbo, IntPtr.Zero, UV_SIZE);
            GL.EnableVertexAttribArray(1);
            GL.VertexAttribFormat(1, 2, VertexAttribType.Float, false, 0);
            GL.VertexAttribBinding(1, 1);
        }
        private void AddUVBuffer(int idx)
        {
            int tbo = GL.GenBuffer();
            int uvCount = _textureComps[idx].TextureCount;

            Vector2[] uvCoords = _textureComps[idx].TextureCoodinates;

            int dataSize = UV_SIZE * uvCount;

            GL.BindBuffer(BufferTarget.ArrayBuffer, tbo);
            GL.NamedBufferStorage(tbo, dataSize, uvCoords, BufferStorageFlags.MapReadBit);

            GL.BindVertexBuffer(1, tbo, IntPtr.Zero, UV_SIZE);
            GL.EnableVertexAttribArray(1);
            GL.VertexAttribFormat(1, 2, VertexAttribType.Float, false, 0);
            GL.VertexAttribBinding(1, 1);
            GL.VertexAttribDivisor(1, 0);
        }

        private void AddElementsBuffer(int lowerBound, int upperBound, out int _ibo)
        {
            int ibo = GL.GenBuffer();
            _ibo = ibo;

            int indexCount = 0;
            for (int idx = lowerBound; idx <= upperBound; idx++)
                indexCount += _modelComps[idx].IndexCount;

            uint kdx = 0;
            uint[] indices = new uint[indexCount];
            for(int idx = lowerBound; idx <= upperBound; idx++)
            {
                uint count = (uint)_modelComps[idx].IndexCount;
                for(int jdx = 0; jdx < count; jdx++)
                {
                    indices[kdx + jdx] = kdx + _modelComps[idx].Indices[jdx];
                }
                kdx += count;
            }

            int dataSize = UINT_SIZE * indexCount;

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ibo);
            GL.NamedBufferStorage(ibo, dataSize, indices, BufferStorageFlags.MapReadBit);
        }
        private void AddElementsBuffer(int idx, out int _ibo)
        {
            int ibo = GL.GenBuffer();
            _ibo = ibo;

            int indexCount = _modelComps[idx].IndexCount;

            uint[] indices = _modelComps[idx].Indices;

            int dataSize = UINT_SIZE * indexCount;

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ibo);
            GL.NamedBufferStorage(ibo, dataSize, indices, BufferStorageFlags.MapReadBit);
        }

        private void AddMatrixBuffers(int start, int count, int verticesPerInstance)
        {
            int mbo = GL.GenBuffer();

            //Vector4[] m4Row0 = new Vector4[count];
            //Vector4[] m4Row1 = new Vector4[count];
            //Vector4[] m4Row2 = new Vector4[count];
            //Vector4[] m4Row3 = new Vector4[count];
            Matrix4[] m4Data = new Matrix4[count];

            for(int idx = 0; idx < count; idx++)
            {
                //m4Row0[idx] = _transComps[start + idx].GetModelView.Row0;
                //m4Row1[idx] = _transComps[start + idx].GetModelView.Row1;
                //m4Row2[idx] = _transComps[start + idx].GetModelView.Row2;
                //m4Row3[idx] = _transComps[start + idx].GetModelView.Row3;
                var m = _transComps[start + idx].GetModelView;
                var mm = new Matrix4(m.Row0, m.Row1, m.Row2, m.Row3);
                mm.Transpose();
                m4Data[idx] = mm;
            }

            int dataSize = VERTEX_SIZE * count * 4;

            GL.BindBuffer(BufferTarget.ArrayBuffer, mbo);
            GL.NamedBufferStorage(mbo, dataSize, m4Data, BufferStorageFlags.MapReadBit);
            //GL.BindVertexBuffer(2, mbo, IntPtr.Zero, VERTEX_SIZE*4);

            GL.EnableVertexAttribArray(4);
            GL.VertexAttribPointer(4, 4, VertexAttribPointerType.Float, false, 64, 0);
            GL.VertexAttribDivisor(4, verticesPerInstance);

            GL.EnableVertexAttribArray(5);
            GL.VertexAttribPointer(5, 4, VertexAttribPointerType.Float, false, 64, 16);
            GL.VertexAttribDivisor(5, verticesPerInstance);

            GL.EnableVertexAttribArray(6);
            GL.VertexAttribPointer(6, 4, VertexAttribPointerType.Float, false, 64, 32);
            GL.VertexAttribDivisor(6, verticesPerInstance);

            GL.EnableVertexAttribArray(7);
            GL.VertexAttribPointer(7, 4, VertexAttribPointerType.Float, false, 64, 48);
            GL.VertexAttribDivisor(7, verticesPerInstance);


            //GL.BindBuffer(BufferTarget.ArrayBuffer, mbo1);
            //GL.NamedBufferStorage(mbo1, dataSize, m4Row1, BufferStorageFlags.MapReadBit);

            //GL.BindVertexBuffer(5, mbo1, IntPtr.Zero, VERTEX_SIZE);
            //GL.EnableVertexAttribArray(5);
            //GL.VertexAttribFormat(5, 4, VertexAttribType.Float, false, 0);
            //GL.VertexAttribBinding(5, 5);
            //GL.VertexAttribDivisor(5, verticesPerInstance);


            //GL.BindBuffer(BufferTarget.ArrayBuffer, mbo2);
            //GL.NamedBufferStorage(mbo2, dataSize, m4Row2, BufferStorageFlags.MapReadBit);

            //GL.BindVertexBuffer(6, mbo2, IntPtr.Zero, VERTEX_SIZE);
            //GL.EnableVertexAttribArray(6);
            //GL.VertexAttribFormat(6, 4, VertexAttribType.Float, false, 0);
            //GL.VertexAttribBinding(6, 6);
            //GL.VertexAttribDivisor(6, verticesPerInstance);

            //GL.BindBuffer(BufferTarget.ArrayBuffer, mbo3);
            //GL.NamedBufferStorage(mbo3, dataSize, m4Row3, BufferStorageFlags.MapReadBit);

            //GL.BindVertexBuffer(7, mbo3, IntPtr.Zero, VERTEX_SIZE);
            //GL.EnableVertexAttribArray(7);
            //GL.VertexAttribFormat(7, 4, VertexAttribType.Float, false, 0);
            //GL.VertexAttribBinding(7, 7);
            //GL.VertexAttribDivisor(7, verticesPerInstance);

            float[] data = new float[dataSize / 4];
            GL.GetBufferSubData(BufferTarget.ArrayBuffer, IntPtr.Zero, dataSize, data);
            int x = 0;
        }

        public void Update(float time)  //this would include vertex data updates (vertice animations) and such
        {
            throw new NotImplementedException();
        }
    }
}
