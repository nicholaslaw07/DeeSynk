using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DeeSynk.Core.Components;
using DeeSynk.Core.Components.Models;
using DeeSynk.Core.Components.Types.Render;
using DeeSynk.Core.Managers;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;

namespace DeeSynk.Core.Systems
{
    public class SystemRender : ISystem
    {
        public Component MonitoredComponents => Component.RENDER |
                                                Component.MODEL_STATIC |
                                                Component.TEXTURE;

        private static readonly Component RenderQualfier = Component.RENDER | Component.MODEL_STATIC | Component.TRANSFORM;

        private World _world;

        private bool[] _monitoredGameObjects;

        private ComponentRender[] _renderComps;
        private ComponentModelStatic[] _staticModelComps;
        private ComponentTexture[] _textureComps;

        private Camera _camera;

        //SHADOW END

        public SystemRender(World world)
        {
            _world = world;

            _monitoredGameObjects = new bool[_world.ObjectMemory];

            _renderComps = _world.RenderComps;
            _staticModelComps = _world.StaticModelComps;
            _textureComps = _world.TextureComps;

            //UpdateMonitoredGameObjects();
        }

        public void PushCameraRef(ref Camera camera)
        {
            _camera = camera;
            _camera.BuildUBO(2);
        }

        public void UpdateMonitoredGameObjects()
        {
            for (int i = 0; i < _world.ObjectMemory; i++)
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

        public void Bind(int idx, bool useShader)
        {
            if (useShader)
                _renderComps[idx].BindData();
            else
                _renderComps[idx].BindDataNoShader();
        }

        public void UnBind()
        {
            GL.UseProgram(0);
            GL.BindVertexArray(0);
        }

        //RENDER PASS METHOD

        private void RenderDepthMaps(ref SystemTransform systemTransform)
        {
            int[] currentViewPort = new int[4];
            GL.GetInteger(GetPName.Viewport, currentViewPort);

            var gameObjects = _world.GameObjects;
            for(int jdx = 0; jdx < _world.ObjectMemory; jdx++)
            {
                if (gameObjects[jdx].Components.HasFlag(Component.LIGHT))
                {

                    var light = _world.LightComps[jdx];
                    light.Bind();

                    GL.Clear(ClearBufferMask.DepthBufferBit);

                    for (int idx = 0; idx < _world.ObjectMemory; idx++)
                    {
                        if (_world.ExistingGameObjects[idx])
                        {
                            if (gameObjects[idx].Components.HasFlag(RenderQualfier))
                            {
                                Bind(idx, false);
                                GL.UseProgram(ShaderManager.GetInstance().GetProgram("shadowTextured"));
                                systemTransform.PushModelMatrix(idx);
                                int elementCount = ModelManager.GetInstance().GetModel(ref _staticModelComps[idx]).ElementCount;
                                GL.DrawElements(PrimitiveType.Triangles, elementCount, DrawElementsType.UnsignedInt, IntPtr.Zero);
                            }
                        }
                    }

                    light.UnBind();
                }
            }

            GL.BindFramebuffer(FramebufferTarget.DrawFramebuffer, 0);
            GL.Viewport(currentViewPort[0], currentViewPort[1], currentViewPort[2], currentViewPort[3]);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        }

        public void RenderAll(ref SystemTransform systemTransform)
        {
            RenderDepthMaps(ref systemTransform);

            var gameObjects = _world.GameObjects;

            for (int idx=0; idx<_world.ObjectMemory; idx++)
            {
                if (_world.ExistingGameObjects[idx])
                {
                    Component comps = gameObjects[idx].Components;
                    if (comps.HasFlag(RenderQualfier))
                    {
                        Bind(idx, true);

                        if (_staticModelComps[idx].ConstructionFlags.HasFlag(ConstructionFlags.COLOR4_COLOR))
                        {
                            var colorArr = _staticModelComps[idx].GetConstructionParameter(ConstructionFlags.COLOR4_COLOR);
                            Color4 color = new Color4(colorArr[0], colorArr[1], colorArr[2], colorArr[3]);
                            GL.Uniform4(17, color);
                        }

                        if (comps.HasFlag(Component.TEXTURE))
                        {
                            _textureComps[idx].BindTexture(TextureUnit.Texture0);
                        }

                        systemTransform.PushModelMatrix(idx);
                        int elementCount = ModelManager.GetInstance().GetModel(ref _staticModelComps[idx]).ElementCount;

                        GL.ActiveTexture(TextureUnit.Texture1);
                        GL.BindTexture(TextureTarget.Texture2D, _world.LightComps[2].DepthMap);

                        GL.DrawElements(PrimitiveType.Triangles, elementCount, DrawElementsType.UnsignedInt, IntPtr.Zero);
                    }
                }
            }
        }
    }
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
