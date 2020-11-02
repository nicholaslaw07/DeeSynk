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
using System.Diagnostics;

namespace DeeSynk.Core.Systems
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
        INTERLEAVED = 1 << 6,

        VERTICES_ELEMENTS = VERTICES | FACE_ELEMENTS,

        VERTICES_NORMALS = VERTICES | NORMALS,
        VERTICES_NORMALS_ELEMENTS = VERTICES_NORMALS | FACE_ELEMENTS,

        VERTICES_NORMALS_COLORS = VERTICES_NORMALS | COLORS,
        VERTICES_NORMALS_COLORS_ELEMENTS = VERTICES_NORMALS_COLORS | FACE_ELEMENTS,

        VERTICES_NORMALS_UVS = VERTICES_NORMALS | UVS,
        VERTICES_NORMALS_UVS_ELEMENTS = VERTICES_NORMALS_UVS | FACE_ELEMENTS
    }

    public class SystemVAO : ISystem
    {
        public Component MonitoredComponents => Component.MODEL_STATIC |
                                                Component.RENDER |
                                                Component.TEXTURE |
                                                Component.TRANSFORM;  //will also probably include at the very least Transform for instanced rendering

        //purpose: used for the creation of VAO's to organize like rendered objects into groups
        // and create VAO's for different use cases like for textured or non-textured

        private const int VERTEX_SIZE = 16;
        private const int NORMAL_SIZE = 12;
        private const int COLOR_SIZE = 16;
        private const int UV_SIZE = 8;
        private const int UINT_SIZE = 4;

        private World _world;
        private UI _ui;

        private bool[] _monitoredGameObjects;

        private VAO[] _vaos;
        private int _vaoCount;
        public int VAOCount {get => _vaoCount;}

        public SystemVAO(World world, UI ui)
        {
            _world = world;
            _ui = ui;

            _monitoredGameObjects = new bool[_world.ObjectMemory]; 

            _vaos = _world.VAOs;
            _vaoCount = -1;
        }

        //simple, but will do for now
        private int NextArrayIndex()
        {
            if (_vaoCount < _world.ObjectMemory)
                return ++_vaoCount;
            else
                throw new InvalidOperationException("No more available VAO spaces.");
        }

        /// <summary>
        /// Initializes a vao containing vertex data for a single ComponentModel.
        /// </summary>
        /// <param name="c">The source to pull all buffer data from.</param>>
        /// <param name="idx">The index of the GameObjec that will be uploaded to the VAO.</param>
        public void InitVAO(GameObjectContainer c, int idx)
        {
            if (idx < c.ObjectMemory)
            {
                if (c.GameObjects[idx].Components.HasFlag(SystemRender.RenderQualfier))
                {
                    Buffers buffers = c.RenderComps[idx].BufferFlags;
                    //Creates a new VAO and adds it to the vao array
                    VAO vao = new VAO(buffers);
                    _vaos[NextArrayIndex()] = vao;
                    AddBuffers(vao, idx, idx, c);

                    //After the vao has its data, initialize the render components associated with the GameObjects in this vao
                    //c.RenderComps[idx] = new ComponentRender(buffers);
                    c.RenderComps[idx].VAO = vao;

                    //Clear bindings
                    GL.BindVertexArray(0);
                }
                else
                    throw new ArgumentException("The specified GameObject does not meet the qualifications for buffer storage.  Needs the render qualifier.");
            }
            else
                throw new ArgumentOutOfRangeException("The index falls outside of the memory range for the specified GameObjectContainer.");
        }

        /// <summary>
        /// Initializes a vao containing vertex data for all objects within the specified range.
        /// </summary>
        /// <param name="buffers">A bitmask specifying the configuration of the buffers within the vao.</param>
        /// <param name="start">The starting index in the components arrays, or the starting id of the range of GameObjects.</param>
        /// <param name="end">The ending index in the components arrays, or the ending id of the range of GameObjects.</param>
        /// <param name="c">The source to pull all buffer data from.</param>>
        public void InitVAORange(Buffers buffers, int start, int end, GameObjectContainer c)
        {

            if (start <= end && start >= 0 && start < c.ObjectMemory &&
                end >= 0 && end < c.ObjectMemory)
            {
                //Creates a new VAO and adds it to the vao array
                VAO vao = new VAO(buffers);
                _vaos[NextArrayIndex()] = vao;
                AddBuffers(vao, start, end, c);

                //After the vao has its data, initialize the render components associated with the GameObjects in this vao
                for(int idx = start; idx <= end; idx++)
                {
                    c.RenderComps[idx] = new ComponentRender(buffers);
                    c.RenderComps[idx].VAO = vao;
                }

                //Clear bindings
                GL.BindVertexArray(0);
            }
        }

        /// <summary>
        /// Adds the buffers and data to the newly created VAO object within the specified range.
        /// </summary>
        /// <param name="vao"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="c"></param>>
        private void AddBuffers(VAO vao, int start, int end, GameObjectContainer c)
        {
            Buffers buffers = vao.BufferConfig;

            int dataStart;

            //If this vao will contain an ibo then add one, otherwise don't.
            if (buffers.HasFlag(Buffers.FACE_ELEMENTS))
            {
                dataStart = VAO.VertexDataWithIBO;
                AddElementsBuffer(vao.Buffers[VAO.IndexData], start, end, c);
            }
            else
            {
                dataStart = VAO.VertexDataNoIBO;
            }

            //Adds an interleaved buffer if specified (multiple sets of data in one buffer)
            if (buffers.HasFlag(Buffers.INTERLEAVED))
            {
                AddInterleavedBuffer(vao.Buffers[dataStart++], buffers, start, end, c); //instanced buffers would go right after this
                int dataCount = 0;
                if (buffers.HasFlag(Buffers.FACE_ELEMENTS))
                {
                    GL.BindBuffer(BufferTarget.ElementArrayBuffer, vao.Buffers[VAO.IndexData]);
                    GL.GetBufferParameter(BufferTarget.ElementArrayBuffer, BufferParameterName.BufferSize, out int size);
                    dataCount = size / 4;
                }
                if (buffers.HasFlag(Buffers.INSTANCES))
                    AddLocationBuffer(vao.Buffers[dataStart++], start, end, dataCount, c);
            }
            //If there isn't an interleaved buffer, then just add all of the different types of data in their own buffers
            else
            {
                if (buffers.HasFlag(Buffers.VERTICES))
                    AddVertices(vao.Buffers[dataStart++], start, end, c);
                if(buffers.HasFlag(Buffers.NORMALS))
                    AddNormalBuffer(vao.Buffers[dataStart++], start, end, c);
                if (buffers.HasFlag(Buffers.UVS))
                    AddColorBuffer(vao.Buffers[dataStart++], start, end, c);
                if (buffers.HasFlag(Buffers.INSTANCES))
                {
                    if (buffers.HasFlag(Buffers.FACE_ELEMENTS))
                    {
                        GL.BindBuffer(BufferTarget.ElementArrayBuffer, vao.Buffers[VAO.IndexData]);
                        GL.GetBufferParameter(BufferTarget.ElementArrayBuffer, BufferParameterName.BufferSize, out int size);
                        AddLocationBuffer(vao.Buffers[dataStart++], start, end, size / 4, c);
                    }
                }
            }
        }

        private void AddElementsBuffer(int bufferId, int lowerBound, int upperBound, GameObjectContainer c)
        {
            var modelManager = ModelManager.GetInstance();

            //Counts number of elements that will be contained in this buffer
            //This must be done seperately since a buffer must be created with a specified size
            int indexCount = 0; 
            for (int idx = lowerBound; idx <= upperBound; idx++)
            {
                var model = modelManager.GetModel(c.StaticModelComps[idx].ModelID);
                if (!model.HasValidData)
                    continue;
                indexCount += model.ElementCount;
            }

            //Get the index data for all GameObjects in the specified range and add it to one continuous array
            uint kdx = 0;
            uint ldx = 0;
            uint[] indices = new uint[indexCount];
            for(int idx = lowerBound; idx <= upperBound; idx++)
            {
                var model = modelManager.GetModel(c.StaticModelComps[idx].ModelID);
                int ec = model.ElementCount;
                if (!model.HasValidData)
                    continue;
                for (int jdx = 0; jdx < model.ElementCount; jdx++)
                {
                    indices[kdx + jdx] = ldx + model.Elements[jdx];
                }
                var val = indices[kdx];
                kdx += (uint)model.ElementCount;
                ldx += (uint)model.VertexCount;

            }

            int dataSize = UINT_SIZE * indexCount;

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, bufferId);  //Sets the ibo as the active elements buffer
            GL.NamedBufferStorage(bufferId, dataSize, indices, BufferStorageFlags.MapReadBit); //Adds the index data the ibo
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0); //Clear bindings
        }


        //Not a good version, will be replaced with something more robust and worth while once we figure out how we want to handle transformation updates
        private void AddLocationBuffer(int bufferId, int start, int count, int verticesPerInstance, GameObjectContainer c)
        {
            Vector4[] offsets = new Vector4[count];
            for (int idx = start; idx < start + count; idx++)
            {
                if (c.StaticModelComps[idx].ConstructionFlags.HasFlag(ConstructionFlags.VECTOR3_OFFSET) &&
                    c.StaticModelComps[idx].ConstructionData.Length >= 3)
                {
                    offsets[idx - start] = new Vector4(c.StaticModelComps[idx].ConstructionData[0],
                                                       c.StaticModelComps[idx].ConstructionData[1],
                                                       c.StaticModelComps[idx].ConstructionData[2],
                                                       0.0f);
                }
            }

            int dataSize = VERTEX_SIZE * count;
            GL.BindBuffer(BufferTarget.ArrayBuffer, bufferId);
            GL.NamedBufferStorage(bufferId, dataSize, offsets, BufferStorageFlags.MapReadBit);

            GL.BindVertexBuffer(3, bufferId, IntPtr.Zero, VERTEX_SIZE);
            GL.EnableVertexAttribArray(3);
            GL.VertexAttribFormat(3, 4, VertexAttribType.Float, false, 0);
            GL.VertexAttribBinding(3, 3);
            GL.VertexAttribDivisor(3, verticesPerInstance);

            Console.WriteLine(GL.GetError().ToString());
        }

        // INTERLEAVED DATA :  VERTEX | NORMAL | UV | VERTEX | NORMNAL | UV | VERTEX | NORMAL | UV ...
        // INSTEAD OF :        VERTEX | VERTEX | VERTEX ...
        //                     NORMAL | NORMAL | NORMAL ...
        //                     UV     | UV     | UV     ...

        /// <summary>
        /// Adds a buffer to the currently bound vao with specified data from GameObjects in the specified range.
        /// </summary>
        /// <param name="bufferId">Id of the buffer to add the interleaved data to.</param>
        /// <param name="bufferMask">Which data to interleave into the buffer.</param>
        /// <param name="start">Starting index or id.</param>
        /// <param name="end">Ending index or id.</param>
        private void AddInterleavedBuffer(int bufferId, Buffers bufferMask, int start, int end, GameObjectContainer c)
        {
            ModelProperties properties = BuffersToModelProps(bufferMask);
            ModelManager modelManager = ModelManager.GetInstance();
            //The sum of number of floats that each type of data has based on the bufferMask (bytes / 4)
            int fStride = Model.FloatStride(properties);
            //The total data count for all models (vertex, uv, and color data should all be the same, so it is only represeted as vertex count)
            int count = 0;
            int totalModels = 0;
            for (int idx = start; idx <= end; idx++)
            {
                var model = modelManager.GetModel(ref c.StaticModelComps[idx]);
                if (!model.HasValidData)
                {
                    Console.WriteLine("Model at index {0} has invalid data, not writing to buffer", idx);
                    continue;
                }

                count += model.VertexCount;
                totalModels++;
            }

            float[] data = new float[fStride * count];
            int offset = 0; 
            //interleave data
            for(int idx = start; idx <= end; idx++)
            {
                Model model = modelManager.GetModel(ref c.StaticModelComps[idx]);
                int length = model.VertexCount * fStride;
                Span<float> subData = data.AsSpan().Slice(offset, length);
                model.GetInterleavedData(properties, subData);
                offset += length;
            }
            offset = 0;

            GL.BindBuffer(BufferTarget.ArrayBuffer, bufferId);
            GL.NamedBufferStorage(bufferId, data.Length * sizeof(float), data, BufferStorageFlags.MapReadBit);
            GL.BindVertexBuffer(0, bufferId, IntPtr.Zero, Model.ByteStride(properties));

            //Activate vertex attributes (allows access to data within shaders) and specify the offset and stride of the data
            if (properties.HasFlag(ModelProperties.VERTICES))
            {
                GL.EnableVertexAttribArray(0);
                GL.VertexAttribFormat(0, Model.VERTEX_DIMS, VertexAttribType.Float, false, offset);
                GL.VertexAttribBinding(0, 0);
                offset += Model.VERTEX_SIZE;
            }

            if (properties.HasFlag(ModelProperties.NORMALS))
            {
                GL.EnableVertexAttribArray(1);
                GL.VertexAttribFormat(1, Model.NORMAL_DIMS, VertexAttribType.Float, false, offset);
                GL.VertexAttribBinding(1, 0);
                offset += Model.NORMAL_SIZE;
            }

            if (properties.HasFlag(ModelProperties.UVS))
            {
                GL.EnableVertexAttribArray(2);
                GL.VertexAttribFormat(2, Model.UV_DIMS, VertexAttribType.Float, false, offset);
                GL.VertexAttribBinding(2, 0);
                offset += Model.UV_SIZE;
            }
            else if (properties.HasFlag(ModelProperties.COLORS))
            {
                GL.EnableVertexAttribArray(2);
                GL.VertexAttribFormat(2, Model.COLOR_DIMS, VertexAttribType.Float, false, offset);
                GL.VertexAttribBinding(2, 0);
                offset += Model.COLOR_SIZE;
            }

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }

        public void Update(float time)
        {
            throw new NotImplementedException();
        }

        #region TRANSLATION
        public static Buffers ModelPropsToBuffers(ModelProperties properties)
        {
            Buffers buffers = Buffers.NONE;
            buffers |= (properties.HasFlag(ModelProperties.VERTICES)) ? Buffers.VERTICES : Buffers.NONE;
            buffers |= (properties.HasFlag(ModelProperties.NORMALS )) ? Buffers.NORMALS  : Buffers.NONE;
            buffers |= (properties.HasFlag(ModelProperties.COLORS  )) ? Buffers.COLORS   : Buffers.NONE;
            buffers |= (properties.HasFlag(ModelProperties.UVS     )) ? Buffers.UVS      : Buffers.NONE;
            return buffers;
        }

        public static ModelProperties BuffersToModelProps(Buffers buffers)
        {
            ModelProperties props = ModelProperties.NONE;
            props |= (buffers.HasFlag(Buffers.VERTICES)) ? ModelProperties.VERTICES : ModelProperties.NONE;
            props |= (buffers.HasFlag(Buffers.NORMALS )) ? ModelProperties.NORMALS  : ModelProperties.NONE;
            props |= (buffers.HasFlag(Buffers.COLORS  )) ? ModelProperties.COLORS   : ModelProperties.NONE;
            props |= (buffers.HasFlag(Buffers.UVS     )) ? ModelProperties.UVS      : ModelProperties.NONE;
            return props;
        }
        #endregion

        #region TO_BE_DEPRECATED

        /// <summary>
        /// Initializes a VAO or VAOs for all objects within a specified range with a specified configuration.
        /// </summary>
        /// <param name="vaoMask">A bit mask to determine the setup of the VAO with the 'VAOTypes' enum.</param>
        /// <param name="start">The starting index in the GameObject components to pull data from.</param>
        /// <param name="end">The ending index in the GameObject components to pull data from (inclusive).</param>
        /// <param name="groupTogether">Determines whether or not group the objects within the specified range under the same VAO or with a unique VAO for each.</param>
        /// <param name="c">The source to pull all buffer data from.</param>>
        public void InitVAOInRange(Buffers bufferMask, int start, int end, bool groupTogether, GameObjectContainer c)
        {
            if (start <= end &&
               start >= 0 &&
               start < _world.ObjectMemory &&
               end >= 0 &&
               end < _world.ObjectMemory)
            {
                int programID = 0;


                var shaderManager = ShaderManager.GetInstance();
                var modelManager = ModelManager.GetInstance();

                for (int idx = start; idx <= start; idx++)
                    c.RenderComps[idx] = new ComponentRender(bufferMask);

                if (groupTogether)
                {
                    VAO vao = new VAO(bufferMask);

                    int dataStart;
                    if (bufferMask.HasFlag(Buffers.FACE_ELEMENTS))
                    {
                        AddElementsBuffer(vao.Buffers[VAO.IndexData], start, end, c);
                        dataStart = VAO.VertexDataWithIBO;
                    }
                    else
                    {
                        dataStart = VAO.VertexDataNoIBO;
                    }

                    if (bufferMask.HasFlag(Buffers.VERTICES))
                        AddVertices(vao.Buffers[dataStart++], start, end, c);
                    if (bufferMask.HasFlag(Buffers.COLORS) && !bufferMask.HasFlag(Buffers.UVS))
                    {
                        AddColorBuffer(vao.Buffers[dataStart++], start, end, c);
                        if (bufferMask.HasFlag(Buffers.NORMALS))
                        {
                            programID = shaderManager.GetProgram("coloredPhong");
                        }
                        else
                        {
                            programID = shaderManager.GetProgram("defaultColored");
                        }
                    }
                    else if (!bufferMask.HasFlag(Buffers.COLORS) && bufferMask.HasFlag(Buffers.UVS))
                    {
                        AddUVBuffer(vao.Buffers[dataStart++], start, end, c);
                        programID = shaderManager.GetProgram("shadowTextured2");
                    }
                    if (bufferMask.HasFlag(Buffers.INSTANCES))
                        AddLocationBuffer(vao.Buffers[dataStart++], start, end - start + 1, 1, c);

                    if (bufferMask.HasFlag(Buffers.NORMALS))
                        AddNormalBuffer(vao.Buffers[dataStart++], start, end, c);


                    GL.UseProgram(programID);

                    for (int idx = start; idx <= start; idx++)
                    {
                        c.RenderComps[idx].AddBufferData();
                        bool result = c.RenderComps[idx].ValidateData();
                    }

                    GL.BindVertexArray(0);
                }
            }
        }

        private void AddVertices(int bufferId, int lowerBound, int upperBound, GameObjectContainer c)
        {
            var modelManager = ModelManager.GetInstance();


            int vertexCount = 0;
            for (int idx = lowerBound; idx <= upperBound; idx++)
            {
                var model = modelManager.GetModel(ref c.StaticModelComps[idx]);
                if (!model.HasValidData)
                    continue;

                vertexCount += model.VertexCount;
            }

            int kdx = 0;
            Vector4[] vertices = new Vector4[vertexCount];
            for (int idx = lowerBound; idx <= upperBound; idx++)
            {
                var model = modelManager.GetModel(c.StaticModelComps[idx].ModelID);
                if (!model.HasValidData)
                    continue;

                int modelVertexCount = modelManager.GetModel(c.StaticModelComps[idx].ModelID).VertexCount;
                Matrix4 modelMatrix = c.TransComps[idx].GetModelMatrix;

                for (int jdx = 0; jdx < modelVertexCount; jdx++)
                {
                    //vertices[kdx] = Vector4.Transform(model.Vertices[jdx], modelMatrix);
                    vertices[kdx] = model.Vertices[jdx];
                    kdx++;
                }
            }

            int dataSize = VERTEX_SIZE * vertexCount;

            GL.BindBuffer(BufferTarget.ArrayBuffer, bufferId);
            GL.NamedBufferStorage(bufferId, dataSize, vertices, BufferStorageFlags.MapReadBit);
            GL.BindVertexBuffer(0, bufferId, IntPtr.Zero, VERTEX_SIZE);
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribFormat(0, 4, VertexAttribType.Float, false, 0);
            GL.VertexAttribBinding(0, 0);
        }

        private void AddColorBuffer(int bufferId, int lowerBound, int upperBound, GameObjectContainer c)
        {
            var modelManager = ModelManager.GetInstance();

            int colorCount = 0;
            for (int idx = lowerBound; idx <= upperBound; idx++)
            {
                var model = modelManager.GetModel(c.StaticModelComps[idx].ModelID);
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
                var model = modelManager.GetModel(c.StaticModelComps[idx].ModelID);

                if (!model.Properties.HasFlag(ModelProperties.COLORS))
                    throw new Exception("Invalid operation: cannot construct color buffer when no color data exists.");

                if (!model.HasValidData)
                    continue;

                modelColorCount = 0;
                useColorFromComponent = false;
                color = Color4.Black;

                if (model.Properties.HasFlag(ModelProperties.COLORS))
                    modelColorCount = model.ColorCount;
                else if (model.Properties.HasFlag(ModelProperties.VERTICES))
                    modelColorCount = (model.Properties.HasFlag(ModelProperties.ELEMENTS)) ? model.ElementCount : model.VertexCount;

                for (int jdx = 0; jdx < modelColorCount; jdx++)
                {
                    colors[kdx] = (useColorFromComponent) ? color : model.Colors[jdx];
                    kdx++;
                }
            }

            int dataSize = COLOR_SIZE * colorCount;

            GL.BindBuffer(BufferTarget.ArrayBuffer, bufferId);
            GL.NamedBufferStorage(bufferId, dataSize, colors, BufferStorageFlags.MapReadBit);

            GL.BindVertexBuffer(1, bufferId, IntPtr.Zero, COLOR_SIZE);
            GL.EnableVertexAttribArray(1);
            GL.VertexAttribFormat(1, 4, VertexAttribType.Float, false, 0);
            GL.VertexAttribBinding(1, 1);
        }

        private void AddUVBuffer(int bufferId, int lowerBound, int upperBound, GameObjectContainer c)
        {
            ModelManager modelManager = ModelManager.GetInstance();

            int uvCount = 0;
            for (int idx = lowerBound; idx <= upperBound; idx++)
            {
                var model = modelManager.GetModel(c.StaticModelComps[idx].ModelID);
                if (!model.HasValidData)
                    continue;

                uvCount += model.UVCount;
            }

            int kdx = 0;
            Vector2[] uvCoords = new Vector2[uvCount];
            for (int idx = lowerBound; idx <= upperBound; idx++)
            {
                var model = modelManager.GetModel(c.StaticModelComps[idx].ModelID);
                if (!model.HasValidData)
                    continue;

                for (int jdx = 0; jdx < model.UVCount; jdx++)
                {
                    uvCoords[kdx] = model.UVs[jdx];
                    kdx++;
                }
            }

            int dataSize = UV_SIZE * uvCount;

            GL.BindBuffer(BufferTarget.ArrayBuffer, bufferId);
            GL.NamedBufferStorage(bufferId, dataSize, uvCoords, BufferStorageFlags.MapReadBit);

            GL.BindVertexBuffer(2, bufferId, IntPtr.Zero, UV_SIZE);
            GL.EnableVertexAttribArray(2);
            GL.VertexAttribFormat(2, 2, VertexAttribType.Float, false, 0);
            GL.VertexAttribBinding(2, 2);
        }

        private void AddNormalBuffer(int bufferId, int lowerBound, int upperBound, GameObjectContainer c)
        {
            var modelManager = ModelManager.GetInstance();


            int normalCount = 0;
            for (int idx = lowerBound; idx <= upperBound; idx++)
            {
                var model = modelManager.GetModel(c.StaticModelComps[idx].ModelID);
                if (!model.HasValidData)
                    continue;

                normalCount += model.NormalCount;
            }

            int kdx = 0;
            Vector3[] normals = new Vector3[normalCount];
            for (int idx = lowerBound; idx <= upperBound; idx++)
            {
                var model = modelManager.GetModel(c.StaticModelComps[idx].ModelID);
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

            GL.BindBuffer(BufferTarget.ArrayBuffer, bufferId);
            GL.NamedBufferStorage(bufferId, dataSize, normals, BufferStorageFlags.MapReadBit);

            GL.BindVertexBuffer(3, bufferId, IntPtr.Zero, NORMAL_SIZE);
            GL.EnableVertexAttribArray(3);
            GL.VertexAttribFormat(3, 3, VertexAttribType.Float, false, 0);
            GL.VertexAttribBinding(3, 3);
        }
        #endregion
    }
}
