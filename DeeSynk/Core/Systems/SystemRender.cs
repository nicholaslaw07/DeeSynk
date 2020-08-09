using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DeeSynk.Core.Algorithms;
using DeeSynk.Core.Components;
using DeeSynk.Core.Components.GraphicsObjects.Lights;
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

        private VAO TEST_VAO;

        //SHADOW END

        public SystemRender(World world)
        {
            _world = world;

            _monitoredGameObjects = new bool[_world.ObjectMemory];

            _renderComps = _world.RenderComps;
            _staticModelComps = _world.StaticModelComps;
            _textureComps = _world.TextureComps;

            TEST_VAO = new VAO(Buffers.VERTICES_ELEMENTS);

            //UpdateMonitoredGameObjects();
        }

        public void PushCameraRef(ref Camera camera)
        {
            _camera = camera;
            _camera.BuildUBO(2, 5);
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

            GL.UseProgram(ShaderManager.GetInstance().GetProgram("shadowTextured"));

            int lightNum = 0;

            GL.Enable(EnableCap.CullFace);

            GL.CullFace(CullFaceMode.Front);
            for(int jdx = 0; jdx < _world.ObjectMemory; jdx++)
            {
                if (_world.GameObjects[jdx].Components.HasFlag(Component.LIGHT))
                {
                    if (_world.LightComps[jdx].LightObject.HasShadowMap)
                    {
                        var lightComp = _world.LightComps[jdx];
                        var light = lightComp.LightObject;

                        light.ShadowMap.BindFBO();
                        GL.Clear(ClearBufferMask.DepthBufferBit);

                        int lightType = 0;
                        if (lightComp.LightType == LightType.SUN) lightType = 1;

                        GL.Uniform1(3, lightType);
                        GL.Uniform1(4, lightNum);

                        for (int idx = 0; idx < _world.ObjectMemory; idx++)
                        {
                            if (_world.ExistingGameObjects[idx])
                            {
                                if (_world.GameObjects [idx].Components.HasFlag(RenderQualfier))
                                {
                                    Bind(idx, false);
                                    systemTransform.PushModelMatrix(idx);
                                    int elementCount = ModelManager.GetInstance().GetModel(ref _staticModelComps[idx]).ElementCount;
                                    GL.DrawElements(PrimitiveType.Triangles, elementCount, DrawElementsType.UnsignedInt, IntPtr.Zero);
                                }
                            }
                        }
                        lightNum++;
                        lightNum %= 3;
                    }
                }
            }
            GL.CullFace(CullFaceMode.Back);
            GL.Disable(EnableCap.CullFace);

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            GL.Viewport(currentViewPort[0], currentViewPort[1], currentViewPort[2], currentViewPort[3]);
            GL.Clear(ClearBufferMask.DepthBufferBit);
        }

        public void RenderWorld(ref SystemTransform systemTransform)
        {
            RenderDepthMaps(ref systemTransform);

            for (int idx=0; idx<_world.ObjectMemory; idx++)
            {
                if (_world.ExistingGameObjects[idx])
                {
                    Component comps = _world.GameObjects[idx].Components;
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

                        //Bind ShadowMaps to their respective texture units
                        for(int i=0; i<_world.ObjectMemory; i++)
                        {
                            if (_world.GameObjects[i].Components.HasFlag(Component.LIGHT))
                                _world.LightComps[i].LightObject.ShadowMap.BindTexture();
                        }

                        GL.DrawElements(PrimitiveType.Triangles, elementCount, DrawElementsType.UnsignedInt, IntPtr.Zero);
                    }
                }
            }

            /*
             * ADD FBO RENDERING FOR POST PROCESSING
             * THIS CAN JUST BE SIMPLE FOR NOW
             * ADD LIGHT GLARE
             *   CREATE POST SHADER
             *   FIND LIGHT RELATIVE TO SCREEN
             *   USE GAUSSIAN TO DISTRUBTE ACROSS FRAME
             */

            //TEST
            /*
            GL.UseProgram(ShaderManager.GetInstance().GetProgram("detectEdges"));
            systemTransform.PushModelMatrix(0);
            Bind(0, false);
            if(GL.GetInteger(GetPName.TransformFeedbackBinding) != TEST_VAO.Buffers[1])
                GL.BindBufferBase(BufferRangeTarget.TransformFeedbackBuffer, 0, TEST_VAO.Buffers[1]);
            GL.BeginTransformFeedback(TransformFeedbackPrimitiveType.Lines);
            GL.DrawElements(PrimitiveType.Triangles, ModelManager.GetInstance().GetModel(ref _staticModelComps[0]).ElementCount, DrawElementsType.UnsignedInt, IntPtr.Zero);
            GL.EndTransformFeedback();
            */
            //ENDTEST
        }

        public void RenderUI()
        {

        }
    }
}


//Notes on ways to optimize for the future
//    Store multiple objects inside of the same vertex array to eliminate calls that bind the VAO as they are expensive
//    In addition to the previous point, objects that are not independent of one another (say terrain data) can be drawn by instancing with little or no updates to the buffer data
//    Use VAO's as a way of organizing objects into render layers and groups instead of a client side render group class
//    Store all texture locations within an atlas as a long buffer within the vao?  Then to update the animation position on sprite sheets simply call a different range on draw elements or something like that.
//    Store transformation matrices in a buffer and update only when necessary
