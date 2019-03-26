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
using DeeSynk.Core.Components.Models;

namespace DeeSynk.Core.Systems
{
    [Flags]
    public enum Buffers
    {
        NONE = 0,
        VERTICES = 1,
        FACE_ELEMENTS = 1 << 1,
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

    class SystemVAO : ISystem
    {
        public int MonitoredComponents => (int)Component.COLOR  | 
                                          (int)Component.MODEL_STATIC  |
                                          (int)Component.MODEL_DYNAMIC |
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

        private const int VERTEX_SIZE = 16;
        private const int NORMAL_SIZE = 12;
        private const int COLOR_SIZE = 16;
        private const int UV_SIZE = 8;
        private const int UINT_SIZE = 4;

        private World _world;

        private bool[] _monitoredGameObjects;

        private ComponentRender[] _renderComps;
        private ComponentModelStatic[] _staticModelComps;
        private ComponentTexture[] _textureComps;

        private ComponentTransform[] _transComps;

        public SystemVAO(World world)
        {
            _world = world;

            _monitoredGameObjects = new bool[_world.ObjectMemory];

            _renderComps = _world.RenderComps;
            _staticModelComps = _world.StaticModelComps;
            _textureComps = _world.TextureComps;

            _transComps = _world.TransComps;
        }

        /// <summary>
        /// Initializes a VAO or VAOs for all objects within a specified range with a specified configuration.
        /// </summary>
        /// <param name="vaoMask">A bit mask to determine the setup of the VAO with the 'VAOTypes' enum.</param>
        /// <param name="start">The starting index in the GameObject components to pull data from.</param>
        /// <param name="end">The ending index in the GameObject components to pull data from (inclusive).</param>
        /// <param name="groupTogether">Determines whether or not group the objects within the specified range under the same VAO or with a unique VAO for each.</param>
        public void InitVAOInRange(Buffers bufferMask, int start, int end, bool groupTogether)
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
                var modelManager = ModelManager.GetInstance();

                if (groupTogether)
                {
                    vao = GL.GenVertexArray();
                    GL.BindVertexArray(vao);

                    if(bufferMask.HasFlag(Buffers.VERTICES))
                        AddVertices(start, end);
                    if (bufferMask.HasFlag(Buffers.COLORS) && !bufferMask.HasFlag(Buffers.UVS))
                    {
                        AddColorBuffer(start, end);
                        if (bufferMask.HasFlag(Buffers.NORMALS))
                        {
                            programID = shaderManager.GetProgram("coloredPhong");
                        }
                        else
                        {
                            programID = shaderManager.GetProgram("defaultColored");
                        }
                    }
                    else if(!bufferMask.HasFlag(Buffers.COLORS) && bufferMask.HasFlag(Buffers.UVS))
                    {
                        AddUVBuffer(start, end);
                        programID = shaderManager.GetProgram("shadowTextured2");
                    }
                    if (bufferMask.HasFlag(Buffers.INSTANCES))
                        AddLocationBuffer(start, end - start + 1, 1);

                    if (bufferMask.HasFlag(Buffers.NORMALS))
                        AddNormalBuffer(start, end);

                    if (bufferMask.HasFlag(Buffers.FACE_ELEMENTS))
                        AddElementsBuffer(start, end, out ibo);
                    GL.BindVertexArray(0);

                    for (int idx= start; idx <= start; idx++)
                    {
                        _renderComps[idx] = new ComponentRender(bufferMask);
                        _renderComps[idx].AddVAOData(vao, ibo, programID);
                        bool result = _renderComps[idx].ValidateData();
                    }
                }
            }
        }

        private void AddVertices(int lowerBound, int upperBound)
        {
            int vbo = GL.GenBuffer();

            var modelManager = ModelManager.GetInstance();

            
            int vertexCount = 0;
            for (int idx = lowerBound; idx <= upperBound; idx++)
            {
                var model = modelManager.GetModel(ref _staticModelComps[idx]);
                if (!model.HasValidData)
                    continue;

                vertexCount += model.VertexCount;
            }

            int kdx = 0;
            Vector4[] vertices = new Vector4[vertexCount];
            for (int idx = lowerBound; idx <= upperBound; idx++)
            {
                var model = modelManager.GetModel(_staticModelComps[idx].ModelID);
                if (!model.HasValidData)
                    continue;

                int modelVertexCount = modelManager.GetModel(_staticModelComps[idx].ModelID).VertexCount;
                Matrix4 modelMatrix = _transComps[idx].GetModelMatrix;

                for (int jdx = 0; jdx < modelVertexCount; jdx++)
                {
                    //vertices[kdx] = Vector4.Transform(model.Vertices[jdx], modelMatrix);
                    vertices[kdx] = model.Vertices[jdx];
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

        private void AddColorBuffer(int lowerBound, int upperBound)
        {
            int cbo = GL.GenBuffer();

            var modelManager = ModelManager.GetInstance();

            int colorCount = 0;
            for (int idx = lowerBound; idx <= upperBound; idx++)
            {
                var model = modelManager.GetModel(_staticModelComps[idx].ModelID);
                if (model.Properties.HasFlag(ModelProperties.COLORS))
                    colorCount += model.ColorCount;
                else if (model.Properties.HasFlag(ModelProperties.VERTICES))
                {
                    colorCount += (model.Properties.HasFlag(ModelProperties.ELEMENTS)) ? model.ElementCount : model.VertexCount;
                }
            }


            Color4[] colors = new Color4[colorCount];

            int kdx = 0;
            int modelColorCount;
            bool useColorFromComponent;
            Color4 color = Color4.Black;

            for (int idx = lowerBound; idx <= upperBound; idx++)
            {
                var model = modelManager.GetModel(_staticModelComps[idx].ModelID);

                if (!model.HasValidData)
                    continue;

                modelColorCount = 0;
                useColorFromComponent = false;
                color = Color4.Black;

                if (model.Properties.HasFlag(ModelProperties.COLORS))
                    modelColorCount = model.ColorCount;
                else if(model.Properties.HasFlag(ModelProperties.VERTICES))
                    modelColorCount = (model.Properties.HasFlag(ModelProperties.ELEMENTS)) ? model.ElementCount : model.VertexCount;

                if(!model.Properties.HasFlag(ModelProperties.COLORS))
                {
                    _staticModelComps[idx].GetConstructionParameter(ConstructionFlags.COLOR4_COLOR, out float[] data);
                    color = new Color4(data[0], data[1], data[2], data[3]);
                    useColorFromComponent = true;
                }

                for (int jdx = 0; jdx < modelColorCount; jdx++)
                {
                    colors[kdx] = (useColorFromComponent) ? color : model.Colors[jdx];
                    kdx++;
                }
            }

            int dataSize = COLOR_SIZE * colorCount;

            GL.BindBuffer(BufferTarget.ArrayBuffer, cbo);
            GL.NamedBufferStorage(cbo, dataSize, colors, BufferStorageFlags.MapReadBit);

            GL.BindVertexBuffer(1, cbo, IntPtr.Zero, COLOR_SIZE);
            GL.EnableVertexAttribArray(1);
            GL.VertexAttribFormat(1, 4, VertexAttribType.Float, false, 0);
            GL.VertexAttribBinding(1, 1);
        }

        private void AddUVBuffer(int lowerBound, int upperBound)
        {
            int tbo = GL.GenBuffer();

            ModelManager modelManager = ModelManager.GetInstance();

            int uvCount = 0;
            for (int idx = lowerBound; idx <= upperBound; idx++)
            {
                var model = modelManager.GetModel(_staticModelComps[idx].ModelID);
                if (!model.HasValidData)
                    continue;

                uvCount += model.UVCount;
            }

            int kdx = 0;
            Vector2[] uvCoords = new Vector2[uvCount];
            for (int idx = lowerBound; idx <= upperBound; idx++)
            {
                var model = modelManager.GetModel(_staticModelComps[idx].ModelID);
                if (!model.HasValidData)
                    continue;

                for (int jdx = 0; jdx < model.UVCount; jdx++)
                {
                    uvCoords[kdx] = model.UVs[jdx];
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

        private void AddLocationBuffer(int start, int count, int verticesPerInstance)
        {
            int lbo = GL.GenBuffer();

            Vector4[] offsets = new Vector4[count];
            for (int idx = start; idx < start + count; idx++)
            {
                if (_staticModelComps[idx].ConstructionFlags.HasFlag(ConstructionFlags.VECTOR3_OFFSET) &&
                    _staticModelComps[idx].ConstructionData.Length >= 3)
                {
                    offsets[idx - start] = new Vector4(_staticModelComps[idx].ConstructionData[0],
                                                       _staticModelComps[idx].ConstructionData[1],
                                                       _staticModelComps[idx].ConstructionData[2],
                                                       0.0f);
                }
            }

            int dataSize = VERTEX_SIZE * count;
            GL.BindBuffer(BufferTarget.ArrayBuffer, lbo);
            GL.NamedBufferStorage(lbo, dataSize, offsets, BufferStorageFlags.MapReadBit);

            GL.BindVertexBuffer(2, lbo, IntPtr.Zero, VERTEX_SIZE);
            GL.EnableVertexAttribArray(2);
            GL.VertexAttribFormat(2, 4, VertexAttribType.Float, false, 0);
            GL.VertexAttribBinding(2, 2);
            GL.VertexAttribDivisor(2, verticesPerInstance);

            Console.WriteLine(GL.GetError().ToString());
        }

        private void AddNormalBuffer(int lowerBound, int upperBound)
        {
            int nbo = GL.GenBuffer();

            var modelManager = ModelManager.GetInstance();


            int normalCount = 0;
            for (int idx = lowerBound; idx <= upperBound; idx++)
            {
                var model = modelManager.GetModel(_staticModelComps[idx].ModelID);
                if (!model.HasValidData)
                    continue;

                normalCount += model.NormalCount;
            }

            int kdx = 0;
            Vector3[] normals = new Vector3[normalCount];
            for (int idx = lowerBound; idx <= upperBound; idx++)
            {
                var model = modelManager.GetModel(_staticModelComps[idx].ModelID);
                if (!model.HasValidData)
                    continue;

                int modelNormalCount = model.NormalCount;
                for (int jdx = 0; jdx < modelNormalCount; jdx++)
                {
                    //var norm = Vector3.TransformVector(model.Normals[jdx], _transComps[idx].GetModelMatrix);
                    //norm.Normalize();
                    //normals[kdx] = norm;
                    normals[kdx] = model.Normals[jdx];
                    kdx++;
                }
            }

            int dataSize = NORMAL_SIZE * normalCount;

            GL.BindBuffer(BufferTarget.ArrayBuffer, nbo);
            GL.NamedBufferStorage(nbo, dataSize, normals, BufferStorageFlags.MapReadBit);

            GL.BindVertexBuffer(3, nbo, IntPtr.Zero, NORMAL_SIZE);
            GL.EnableVertexAttribArray(3);
            GL.VertexAttribFormat(3, 3, VertexAttribType.Float, false, 0);
            GL.VertexAttribBinding(3, 3);
        }

        private void AddElementsBuffer(int lowerBound, int upperBound, out int iboID)
        {
            int ibo = GL.GenBuffer();
            iboID = ibo;

            var modelManager = ModelManager.GetInstance();

            int indexCount = 0;
            for (int idx = lowerBound; idx <= upperBound; idx++)
            {
                var model = modelManager.GetModel(_staticModelComps[idx].ModelID);
                if (!model.HasValidData)
                    continue;
                indexCount += model.ElementCount;
            }

            uint kdx = 0;
            uint[] indices = new uint[indexCount];
            for(int idx = lowerBound; idx <= upperBound; idx++)
            {
                var model = modelManager.GetModel(_staticModelComps[idx].ModelID);
                if (!model.HasValidData)
                    continue;
                for (int jdx = 0; jdx < model.ElementCount; jdx++)
                {
                    indices[kdx + jdx] = kdx + model.Elements[jdx];
                }
                kdx += (uint)model.ElementCount;
            }

            int dataSize = UINT_SIZE * indexCount;

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ibo);
            GL.NamedBufferStorage(ibo, dataSize, indices, BufferStorageFlags.MapReadBit);
        }

        public void Update(float time)  //this would include vertex data updates (vertice animations) and such
        {
            throw new NotImplementedException();
        }
    }
}
