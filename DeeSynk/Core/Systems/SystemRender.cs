using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DeeSynk.Core.Components;
using DeeSynk.Core.Components.Types.Render;
using OpenTK.Graphics.OpenGL4;

namespace DeeSynk.Core.Systems
{
    class SystemRender : ISystem
    {
        public int MonitoredComponents = (int)Component.RENDER |
                                         (int)Component.MODEL |
                                         (int)Component.TEXTURE |
                                         (int)Component.COLOR;

        private World _world;

        private bool[] _monitoredGameObjects;
        
        private ComponentRender[]       _renderComps;
        private ComponentModel[]        _modelComps;
        private ComponentTexture[]      _textureComps;
        private ComponentColor[]        _colorComps;

        int ISystem.MonitoredComponents => throw new NotImplementedException();

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
            for(int i=0; i< 10000; i++)
            {
                _modelComps[i] = new ComponentModel(100f, 100f);
            }
        }

        public void InitVAO()
        {
            int shaderID = 0;
            for(int i=0; i< 10000; i++)
            {
                int vao = GL.GenVertexArray();
                GL.BindVertexArray(vao);

                int vbo = GL.GenBuffer();
                int cbo = GL.GenBuffer();
                int ibo = GL.GenBuffer();

                GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
                GL.BindBuffer(BufferTarget.ArrayBuffer, cbo);
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, ibo);

                GL.NamedBufferStorage(vbo, 4 * _modelComps[i].VertexCount, _modelComps[i].GetVertices, BufferStorageFlags.MapWriteBit);

                GL.VertexArrayAttribBinding(vao, 0, 0);
                GL.EnableVertexArrayAttrib(vao, 0);
                GL.VertexArrayAttribFormat(vao, 0, 4, VertexAttribType.Float, false, 0);

                GL.VertexArrayVertexBuffer(vao, 0, vbo, IntPtr.Zero, 4);
                GL.NamedBufferStorage(ibo, 4 * _modelComps[i].IndexCount, _modelComps[i].GetIndices, BufferStorageFlags.MapWriteBit);

                GL.BindVertexArray(0);

                _renderComps[i] = new ComponentRender(vao, ibo, shaderID);
            }
        }

        public void Update(float time)
        {
        }

        public void Bind(int i)
        {
            GL.BindVertexArray(_renderComps[i].VAO_ID);
            GL.UseProgram(_renderComps[i].SHADER_ID);
            //GL.BindBuffer(BufferTarget.ElementArrayBuffer, _renderComps[i].IBO_ID);
        }

        public void Render()
        {
            GL.DrawArrays(PrimitiveType.Quads, 0, 1);
        }

        public void UnBind()
        {
            GL.UseProgram(0);
            GL.BindVertexArray(0);
        }
    }
}
