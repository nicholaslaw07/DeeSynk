using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DeeSynk.Core.Components;
using DeeSynk.Core.Components.Types.Render;
using DeeSynk.Core.Managers;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;

namespace DeeSynk.Core.Systems
{
    public enum VAOTypes
    {
        Colored = 0,
        Textured = 1
    }

    class SystemRender : ISystem
    {
        public const int RECANGLE_INDEX_COUNT = 6;

        public int MonitoredComponents => (int)Component.RENDER |
                                          (int)Component.MODEL |
                                          (int)Component.TEXTURE |
                                          (int)Component.COLOR;

        private World _world;

        private bool[] _monitoredGameObjects;
        
        private ComponentRender[]       _renderComps;
        private ComponentModel[]        _modelComps;
        private ComponentTexture[]      _textureComps;
        private ComponentColor[]        _colorComps;

        public SystemRender(World world)
        {
            _world = world;

            _monitoredGameObjects = new bool[_world.ObjectMemory];

            _renderComps = _world.RenderComps;
            _modelComps = _world.ModelComps;
            _textureComps = _world.TextureComps;
            _colorComps = _world.ColorComps;

            //UpdateMonitoredGameObjects();
        }

        public void UpdateMonitoredGameObjects()
        {
            for (int i=0; i < _world.ObjectMemory; i++)
            {
                if (_world.ExistingGameObjects[i])
                {
                    if ((_world.GameObjects[i].Components | MonitoredComponents) == MonitoredComponents)
                    {
                        _monitoredGameObjects[i] = true;
                    }
                }
            }
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

            int texID = TextureManager.GetInstance().GetTexture("Ball - Copy");

            var uvArr = new Vector2[6];
            uvArr[0] = new Vector2(0.0f, 0.0f);
            uvArr[1] = new Vector2(1.0f, 0.0f);
            uvArr[2] = new Vector2(1.0f, 1.0f);
            uvArr[3] = new Vector2(1.0f, 1.0f);
            uvArr[4] = new Vector2(0.0f, 1.0f);
            uvArr[5] = new Vector2(0.0f, 0.0f);

            for (int i=0; i< _world.ObjectMemory; i++)
            {
                _modelComps[i] = new ComponentModel(10f, 10f, true);
                //_colorComps[i] = new ComponentColor(color4Arr);
                _textureComps[i] = new ComponentTexture(ref uvArr, texID);
            }
        }

        public void InitVAO(VAOTypes type)
        {
            if (type == VAOTypes.Colored)
                InitVAO_Colored();
            else if (type == VAOTypes.Textured)
                InitVAO_Textured();
        }

        private void InitVAO_Colored()
        {
            int shaderID = ShaderManager.GetInstance().GetProgram("defaultColored");
            GL.UseProgram(shaderID);

            int vertexSize = 16;
            int colorSize = 16;
            int uintSize = 4;

            for (int idx=0; idx< _world.ObjectMemory; idx++)
            {
                int vao = GL.GenVertexArray();
                GL.BindVertexArray(vao);

                int vbo = GL.GenBuffer();
                int cbo = GL.GenBuffer();
                int ibo = GL.GenBuffer();


                int vertexCount = _modelComps[idx].VertexCount;

                GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
                GL.NamedBufferStorage(vbo, vertexSize * vertexCount, _modelComps[idx].Vertices, BufferStorageFlags.MapReadBit);

                GL.BindVertexBuffer(0, vbo, IntPtr.Zero, vertexSize);
                GL.EnableVertexAttribArray(0);
                GL.VertexAttribFormat(0, vertexSize * vertexCount, VertexAttribType.Float, false, 0);
                GL.VertexAttribBinding(0, 0);


                int colorCount = _colorComps[idx].ColorCount;

                GL.BindBuffer(BufferTarget.ArrayBuffer, cbo);
                GL.NamedBufferStorage(cbo, colorSize * colorCount, _colorComps[idx].Colors, BufferStorageFlags.MapReadBit);

                GL.BindVertexBuffer(1, cbo, IntPtr.Zero, colorSize);
                GL.EnableVertexAttribArray(1);
                GL.VertexAttribFormat(1, colorSize * colorCount, VertexAttribType.Float, false, 0);
                GL.VertexAttribBinding(1, 1);


                int indexCount = _modelComps[idx].IndexCount;

                GL.BindBuffer(BufferTarget.ElementArrayBuffer, ibo);
                GL.NamedBufferStorage(ibo, uintSize * indexCount, _modelComps[idx].Indices, BufferStorageFlags.MapReadBit);

                GL.BindVertexArray(0);

                _renderComps[idx] = new ComponentRender(vao, ibo, shaderID);
            }
        }

        private void InitVAO_Textured()
        {
            int shaderID = ShaderManager.GetInstance().GetProgram("defaultTextured");
            GL.UseProgram(shaderID);

            int vertexSize = 16;
            int texSize = 8;
            int uintSize = 4;

            for (int idx = 0; idx < _world.ObjectMemory; idx++)
            {
                int vao = GL.GenVertexArray();
                GL.BindVertexArray(vao);

                int vbo = GL.GenBuffer();
                int tbo = GL.GenBuffer();
                int ibo = GL.GenBuffer();


                int vertexCount = _modelComps[idx].VertexCount;

                GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
                GL.NamedBufferStorage(vbo, vertexSize * vertexCount, _modelComps[idx].Vertices, BufferStorageFlags.MapReadBit);

                GL.BindVertexBuffer(0, vbo, IntPtr.Zero, vertexSize);
                GL.EnableVertexAttribArray(0);
                GL.VertexAttribFormat(0, vertexSize * vertexCount, VertexAttribType.Float, false, 0);
                GL.VertexAttribBinding(0, 0);


                int texCount = _textureComps[idx].TextureCount;

                GL.BindBuffer(BufferTarget.ArrayBuffer, tbo);
                GL.NamedBufferStorage(tbo, texSize * texCount, _textureComps[idx].TextureCoodinates, BufferStorageFlags.MapReadBit);

                GL.BindVertexBuffer(1, tbo, IntPtr.Zero, texSize);
                GL.EnableVertexAttribArray(1);
                GL.VertexAttribFormat(1, texSize * texCount, VertexAttribType.Float, false, 0);
                GL.VertexAttribBinding(1, 1);


                int indexCount = _modelComps[idx].IndexCount;

                GL.BindBuffer(BufferTarget.ElementArrayBuffer, ibo);
                GL.NamedBufferStorage(ibo, uintSize * indexCount, _modelComps[idx].Indices, BufferStorageFlags.MapReadBit);

                GL.BindVertexArray(0);

                _renderComps[idx] = new ComponentRender(vao, ibo, shaderID);
            }
        }

        public void Update(float time)
        {
        }

        public void Bind(int idx)
        {
            _renderComps[idx].BindData();
            _textureComps[idx].BindTexture();
        }

        public void Render(int idx)
        {
            GL.DrawElements(BeginMode.Triangles, _modelComps[idx].IndexCount, DrawElementsType.UnsignedInt, 0);
        }

        public void UnBind()
        {
            GL.UseProgram(0);
            GL.BindVertexArray(0);
        }

        //Notes on ways to optimize for the future
        //    Store multiple objects inside of the same vertex array to eliminate calls that bind the VAO as they are expensive
        //    In addition to the previous point, objects that are not independent of one another (say terrain data) can be drawn by instancing with little or no updates to the buffer data
        //    Use VAO's as a way of organizing objects into render layers and groups instead of a client side render group class
        //    Store all texture locations within an atlas as a long buffer within the vao?  Then to update the animation position on sprite sheets simply call a different range on draw elements or something like that.
        //    Store transformation matrices in a buffer and update only when necessary
        //
        //    Create methods that organize data from multiple objects into a single array that can then be fed into VAO
        //    If the data is immutable then strided data will be much more efficient for this case
        //
        //    Store a reference to SystemTransform (which is currently in World) inside of System render to avoid having to send SystemTransform for every render call
        //
        //    Make a VAO management subsystem, probably will be a class called SystemVAO or SystemRenderData
        //
        //    Add a way for a window resize to update the orthographic matrix inside of SystemTransform, ideally this shouldn't happen often as it is expensive.

        public void RenderAll(ref SystemTransform systemTransform)
        {
            GL.Enable(EnableCap.DepthTest);
            for(int idx=0; idx<_renderComps.Length; idx++)
            {
                Bind(idx);
                systemTransform.PushMatrixData(idx);
                Render(idx);
            }
        }
    }
}
