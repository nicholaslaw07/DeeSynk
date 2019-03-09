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
            for (int idx=0; idx<_renderComps.Length; idx++)
            {
                Bind(idx);
                systemTransform.PushMatrixData(idx);
                Render(idx);
            }
        }

        public void RenderInstanced(ref SystemTransform systemTransform, int renderIdx)
        {
            //Bind(renderIdx);
            //GL.BindVertexArray(1);
            Bind(renderIdx);

            systemTransform.PushMatrixDataNoTransform();
            //GL.DrawElementsInstanced(PrimitiveType.Triangles, _modelComps[renderIdx].IndexCount, DrawElementsType.UnsignedInt, _modelComps[renderIdx].Indices, _renderComps[renderIdx].OBJECT_COUNT);
            //GL.DrawElements(PrimitiveType.Triangles, _modelComps[renderIdx].IndexCount, DrawElementsType.UnsignedInt, _modelComps[renderIdx].Indices);
            GL.DrawArraysInstanced(PrimitiveType.Triangles, 0, 6, 100);
        }
    }
}
