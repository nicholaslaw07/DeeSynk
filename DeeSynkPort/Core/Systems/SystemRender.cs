using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DeeSynk.Core.Algorithms;
using DeeSynk.Core.Components;
using DeeSynk.Core.Components.GraphicsObjects;
using DeeSynk.Core.Components.GraphicsObjects.Lights;
using DeeSynk.Core.Components.GraphicsObjects.Shadows;
using DeeSynk.Core.Components.Models;
using DeeSynk.Core.Components.Types.Render;
using DeeSynk.Core.Managers;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace DeeSynk.Core.Systems
{
    public class SystemRender : ISystem
    {
        public Component MonitoredComponents => Component.RENDER |
                                                Component.MODEL_STATIC |
                                                Component.TEXTURE;

        public static readonly Component RenderQualfier = Component.RENDER | Component.MODEL_STATIC | Component.TRANSFORM;

        public static readonly int FinalFBO_ID = 0;

        private World _world;
        private UI _ui;

        private bool[] _monitoredGameObjects_W;
        private bool[] _monitoredGameObjects_U;

        private ComponentRender[] _renderComps;
        private ComponentModelStatic[] _staticModelComps;
        private ComponentTexture[] _textureComps;

        private Camera _camera;
        private Camera _postCamera; //This is the camera used to render post-processing to a quad.

        private VAO TEST_VAO;

        private FBO[] _fbos;

        private Stopwatch sw;


        int[] ssbos;
        private ShadowMap sm;
        private int _tex;

        //SHADOW END

        public SystemRender(ref World world, ref UI ui)
        {
            _world = world;
            _ui = ui;

            _monitoredGameObjects_W = new bool[_world.ObjectMemory];
            _monitoredGameObjects_U = new bool[_ui.ObjectMemory];

            _renderComps = _world.RenderComps;
            _staticModelComps = _world.StaticModelComps;
            _textureComps = _world.TextureComps;

            TEST_VAO = new VAO(Buffers.VERTICES_ELEMENTS);

            _fbos = _world.FBOs;

            sw = new Stopwatch();
        }

        public void LoadTestCompute()
        {
            ssbos = new int[2];
            GL.GenBuffers(2, ssbos);

            Vector4[] inputData = new Vector4[3];
            inputData[0] = new Vector4(0.0f, 0.0f, 1.0f, 1.0f);
            inputData[1] = new Vector4(1.0f, 0.0f, 1.0f, 1.0f);
            inputData[2] = new Vector4(1.0f, 1.0f, 1.0f, 1.0f);

            float[] outputData = new float[MainWindow.width * MainWindow.height * 4 * 4];

            GL.BindBuffer(BufferTarget.ShaderStorageBuffer, ssbos[0]);
            GL.BufferData(BufferTarget.ShaderStorageBuffer, 12 * 4, inputData, BufferUsageHint.DynamicRead);
            GL.BindBufferBase(BufferRangeTarget.ShaderStorageBuffer, 1, ssbos[0]);
            GL.BindBuffer(BufferTarget.ShaderStorageBuffer, 0);

            GL.BindBuffer(BufferTarget.ShaderStorageBuffer, ssbos[1]);
            GL.BufferData(BufferTarget.ShaderStorageBuffer, MainWindow.width * MainWindow.height * 4 * 4, outputData, BufferUsageHint.DynamicRead);
            GL.BindBufferBase(BufferRangeTarget.ShaderStorageBuffer, 2, ssbos[1]);
            GL.BindBuffer(BufferTarget.ShaderStorageBuffer, 0);

            sm = new ShadowMap(MainWindow.width, MainWindow.height, TextureUnit.Texture9);

            GL.GenTextures(1, out _tex);

            GL.BindTexture(TextureTarget.Texture2D, _tex);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, MainWindow.width, MainWindow.height, 0, PixelFormat.Bgra, PixelType.Float, IntPtr.Zero);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (float)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (float)TextureMagFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (float)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (float)TextureWrapMode.ClampToEdge);
            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

            GL.BindTexture(TextureTarget.Texture2D, 0);
        }

        public void PushCameraRef(ref Camera camera)
        {
            _camera = camera;
            _camera.BuildUBO(2, 7);
        }

        public void UpdateMonitoredGameObjects()
        {
            UpdateMonitoredGameObjects(_world, _monitoredGameObjects_W);
            UpdateMonitoredGameObjects(_ui, _monitoredGameObjects_U);
        }

        private void UpdateMonitoredGameObjects(GameObjectContainer c, bool[] monitor)
        {
            for (int i = 0; i < c.ObjectMemory; i++)
            {
                if (c.ExistingGameObjects[i])
                {
                    if (c.GameObjects[i].Components.HasFlag(MonitoredComponents))
                    {
                        monitor[i] = true;
                    }
                }
            }
        }

        public void Update(float time)
        {
        }

        public void Bind(int idx, bool useShader, GameObjectContainer c)
        {
            if (useShader)
                c.RenderComps[idx].BindData();
            else
                c.RenderComps[idx].BindDataNoShader();
        }

        public void UnBind()
        {
            GL.UseProgram(0);
            GL.BindVertexArray(0);
        }

        //RENDER PASS METHOD

        public void Render(ref SystemTransform systemTransform)
        {
            //MAYBE:  Add a component class that adds unique general purpose functions?  Maybe this is where indirect rendering comes in.
            //Custom rendering function queues may be in order.  Fuck.

            //Render camera depth map then convert points to kd tree
            //Test visibility of points in frame of the light save to stencil
            //Render normally
            //RunCompute(ref systemTransform);

            RenderDepthMaps(ref systemTransform);
            RenderScene(ref systemTransform);

            //float[] data = new float[10000];
            //GL.GetTextureSubImage(1, 0, 500, 500, 0, 100, 100, 0, PixelFormat.Rgba, PixelType.Float, 10000 * 4, data);

            RenderPost(ref systemTransform);
            RenderUI(ref systemTransform);

            //Debug.WriteLine(GL.GetError());
        }

        public void RunCompute(ref SystemTransform systemTransform)
        {
            RenderCameraDepth(ref systemTransform);
            int s = ShaderManager.GetInstance().GetProgram("computeTest");
            GL.UseProgram(s);
            GL.BindImageTexture(0, sm.Texture, 0, false, 0, TextureAccess.ReadWrite, SizedInternalFormat.Rgba32f);
            //GL.Uniform1(GL.GetUniformLocation(s, "cameraDepth"), 0);
            GL.DispatchCompute(MainWindow.width, MainWindow.height, 1);
            GL.MemoryBarrier(MemoryBarrierFlags.AllBarrierBits);
            //GL.BindBuffer(BufferTarget.ShaderStorageBuffer, ssbos[1]);
            //float[] od = new float[MainWindow.width * MainWindow.height * 4 * 4];
            //GL.GetBufferSubData(BufferTarget.ShaderStorageBuffer, IntPtr.Zero, MainWindow.width * MainWindow.height * 4 * 4, od);
            //Console.WriteLine(od[12000] + " " + od[101010]);
            //for(int idx = 3; idx < od.Length; idx+=4)
            //{
            //    if(od[idx] != 1 && od[idx] != 0)
            //    {
            //        Console.WriteLine(od[idx]);
            //    }
            //}
            //Console.WriteLine(GL.GetError());
        }

        public void RenderCameraDepth(ref SystemTransform systemTransform)
        {
            int[] currentViewPort = new int[4];
            GL.GetInteger(GetPName.Viewport, currentViewPort);

            GL.UseProgram(ShaderManager.GetInstance().GetProgram("cameraDepth"));

            GL.Enable(EnableCap.CullFace);

            GL.CullFace(CullFaceMode.Front);
            sm.BindFBO();
            GL.Clear(ClearBufferMask.DepthBufferBit);

            for (int idx = 0; idx < _world.ObjectMemory; idx++)
            {
                if (_world.ExistingGameObjects[idx] && idx < 9)
                {
                    if (_world.GameObjects[idx].Components.HasFlag(RenderQualfier))
                    {
                        Bind(idx, false, _world);
                        systemTransform.PushModelMatrix(idx, _world);
                        int elementCount = (idx == 8) ? 36001686 : ModelManager.GetInstance().GetModel(ref _staticModelComps[idx]).ElementCount;
                        GL.DrawElements(PrimitiveType.Triangles, elementCount, DrawElementsType.UnsignedInt, IntPtr.Zero);
                    }
                }
            }
            GL.CullFace(CullFaceMode.Back);
            GL.Disable(EnableCap.CullFace);

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            GL.Viewport(currentViewPort[0], currentViewPort[1], currentViewPort[2], currentViewPort[3]);
            GL.Clear(ClearBufferMask.DepthBufferBit);
        }

        private void RenderDepthMaps(ref SystemTransform systemTransform)
        {
            //sw.Start();
            for (int i = 0; i < _world.ObjectMemory; i++)
            {
                if (_world.GameObjects[i].Components.HasFlag(Component.LIGHT))
                    _world.LightComps[i].LightObject.ShadowMap.BindTexture();
            }

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
                            if (_world.ExistingGameObjects[idx] && idx < 9)
                            {
                                if (_world.GameObjects [idx].Components.HasFlag(RenderQualfier))
                                {
                                    Bind(idx, false, _world);
                                    systemTransform.PushModelMatrix(idx, _world);
                                    int elementCount = (idx == 8) ? 36001686 : ModelManager.GetInstance().GetModel(ref _staticModelComps[idx]).ElementCount;
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

            //sw.Stop();
            //Console.Out.WriteLine("Shadows: " + sw.ElapsedMilliseconds);
            //sw.Reset();
        }


        /*
         * ADD FBO CLASS?
         * THIS IS CONTINGENT ON HOW MUCH INFORMATION IS STORED IN AN FBO
         * COULD BE EXTENDED TO A BUFFER OBJECT CLASS AND FBO INHERITS?
         * 
         * ADD FBO RENDERING FOR POST PROCESSING
         * THIS CAN JUST BE SIMPLE FOR NOW
         * ADD LIGHT GLARE
         *   CREATE POST SHADER
         *   FIND LIGHT RELATIVE TO SCREEN
         *   USE GAUSSIAN TO DISTRUBTE ACROSS FRAME
         */


        /* Steps to add FBO rendering and post.
         * 
         * Create seperate orthographic camera sized to fit the window.  (A new camera component).
         * Create a VAO dedicated to rendering the scene to a quad.  This will need a distinguising qualifier so that it isn't used in the scene.
         *      A visibility or scene membership attribute might work.
         * Create a shader dedicated to fbo texture rendering, although defaultTextured should work if there is no post processing.
         * 
         * Create the glare shader.
         */

        /*
         * For the future, make the creation of FBO rendering and post processing more modular if possible.
         */

        public void RenderScene(ref SystemTransform systemTransform)
        {
            //sw.Start();
            int[] currentViewPort = new int[4];
            GL.GetInteger(GetPName.Viewport, currentViewPort);  //automate this somehow, feels clunky

            _fbos[0].Bind(true);
            BindShadowMaps();

            for (int idx = 0; idx < _world.ObjectMemory; idx++)
            {
                if (_world.ExistingGameObjects[idx] && idx < 9){
                    Component comps = _world.GameObjects[idx].Components;
                    if (comps.HasFlag(RenderQualfier) && !_renderComps[idx].IsFinalRenderPlane)
                    {

                        Bind(idx, true, _world);

                        if (comps.HasFlag(Component.MATERIAL))
                        {
                            Color4 color = _world.MaterialComps[idx].Color;
                            GL.Uniform4(17, color);
                        }

                        if (comps.HasFlag(Component.TEXTURE))
                        {
                            _textureComps[idx].BindTexture(TextureUnit.Texture0);
                        }

                        systemTransform.PushModelMatrix(idx, _world);
                        int elementCount = (idx == 8) ? 36001686 : ModelManager.GetInstance().GetModel(ref _staticModelComps[idx]).ElementCount;

                        GL.DrawElements(PrimitiveType.Triangles, elementCount, DrawElementsType.UnsignedInt, IntPtr.Zero);
                    }
                }
            }

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            GL.Viewport(currentViewPort[0], currentViewPort[1], currentViewPort[2], currentViewPort[3]);
            //sw.Stop();
            //Console.Out.WriteLine("Scene: " + sw.ElapsedMilliseconds);
            //sw.Reset();
        }

        public void ShadowVolumes(ref SystemTransform systemTransform)
        {
            //using http://ogldev.org/www/tutorial40/tutorial40.html
            GL.Enable(EnableCap.StencilTest);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);
            GL.DepthMask(true);
            GL.DrawBuffer(DrawBufferMode.None);
            for (int idx = 0; idx < _world.ObjectMemory; idx++)
            {
                if (_world.ExistingGameObjects[idx])
                {
                    Component comps = _world.GameObjects[idx].Components;
                    if (comps.HasFlag(RenderQualfier) && !_renderComps[idx].IsFinalRenderPlane)
                    {
                        Bind(idx, true, _world);

                        if (comps.HasFlag(Component.TEXTURE))
                        {
                            _textureComps[idx].BindTexture(TextureUnit.Texture0);
                        }

                        systemTransform.PushModelMatrix(idx, _world);
                        int elementCount = ModelManager.GetInstance().GetModel(ref _staticModelComps[idx]).ElementCount;

                        //Bind ShadowMaps to their respective texture units
                        for (int i = 0; i < _world.ObjectMemory; i++)
                        {
                            if (_world.GameObjects[i].Components.HasFlag(Component.LIGHT))
                                _world.LightComps[i].LightObject.ShadowMap.BindTexture();
                        }

                        GL.DrawElements(PrimitiveType.Triangles, elementCount, DrawElementsType.UnsignedInt, IntPtr.Zero);
                    }
                }
            }
            GL.DepthMask(false);
            GL.Enable(EnableCap.DepthClamp);
            GL.Disable(EnableCap.CullFace);
            GL.StencilFunc(StencilFunction.Always, 0, 0xff);
            GL.StencilOpSeparate(StencilFace.Back, StencilOp.Keep, StencilOp.IncrWrap, StencilOp.Keep);
            GL.StencilOpSeparate(StencilFace.Front, StencilOp.Keep, StencilOp.DecrWrap, StencilOp.Keep);

            GL.UseProgram(ShaderManager.GetInstance().GetProgram("shadowVolume"));
            systemTransform.PushModelMatrix(0, _world);
            Bind(0, false, _world);
            if (GL.GetInteger(GetPName.TransformFeedbackBinding) != TEST_VAO.Buffers[1])
                GL.BindBufferBase(BufferRangeTarget.TransformFeedbackBuffer, 0, TEST_VAO.Buffers[1]);
            GL.BeginTransformFeedback(TransformFeedbackPrimitiveType.Triangles);
            GL.DrawElements(PrimitiveType.Triangles, ModelManager.GetInstance().GetModel(ref _staticModelComps[0]).ElementCount, DrawElementsType.UnsignedInt, IntPtr.Zero);
            GL.EndTransformFeedback();

            GL.Disable(EnableCap.DepthClamp);
            GL.Enable(EnableCap.CullFace);
            ShadowVolumePart2(ref systemTransform);
        }
        public void ShadowVolumePart2(ref SystemTransform systemTransform)
        {
            GL.DrawBuffer(DrawBufferMode.Back);
            GL.StencilFunc(StencilFunction.Equal, 0x0, 0xFF);
            GL.StencilOpSeparate(StencilFace.Back, StencilOp.Keep, StencilOp.Keep, StencilOp.Keep);
        }
        public void StencilTest(ref SystemTransform systemTransform)
        {
            GL.Enable(EnableCap.StencilTest);
            GL.Disable(EnableCap.CullFace);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);
            GL.StencilOp(StencilOp.Replace, StencilOp.Replace, StencilOp.IncrWrap);
            GL.StencilFunc(StencilFunction.Always, 1, 0xFF);
            GL.StencilMask(0xFF);

            GL.Disable(EnableCap.DepthTest);

            //GL.DrawBuffer(DrawBufferMode.None);
            //RenderVolume(ref systemTransform);
            //GL.DrawBuffer(DrawBufferMode.ColorAttachment0);
            RenderObject(ref systemTransform, 1);

            GL.StencilFunc(StencilFunction.Equal, 1, 0xFF);
            GL.StencilMask(0x00);

            RenderObject(ref systemTransform, 0);
            GL.Enable(EnableCap.DepthTest);
            GL.StencilMask(0xFF);
            GL.StencilFunc(StencilFunction.Always, 1, 0xFF);
            GL.Disable(EnableCap.StencilTest);
            GL.Enable(EnableCap.CullFace);
        }
        public void RenderVolume(ref SystemTransform systemTransform)
        {
            GL.UseProgram(ShaderManager.GetInstance().GetProgram("shadowVolume"));
            systemTransform.PushModelMatrix(0, _world);
            Bind(0, false, _world);
            if (GL.GetInteger(GetPName.TransformFeedbackBinding) != TEST_VAO.Buffers[1])
                GL.BindBufferBase(BufferRangeTarget.TransformFeedbackBuffer, 0, TEST_VAO.Buffers[1]);
            GL.BeginTransformFeedback(TransformFeedbackPrimitiveType.Triangles);
            GL.DrawElements(PrimitiveType.Triangles, ModelManager.GetInstance().GetModel(ref _staticModelComps[0]).ElementCount, DrawElementsType.UnsignedInt, IntPtr.Zero);
            GL.EndTransformFeedback();
        }
        public void RenderObject(ref SystemTransform systemTransform, int idx)
        {
            if (_world.ExistingGameObjects[idx])
            {
                Component comps = _world.GameObjects[idx].Components;
                if (comps.HasFlag(RenderQualfier) && !_renderComps[idx].IsFinalRenderPlane)
                {
                    Bind(idx, true, _world);

                    if (comps.HasFlag(Component.TEXTURE))
                    {
                        _textureComps[idx].BindTexture(TextureUnit.Texture0);
                    }

                    systemTransform.PushModelMatrix(idx, _world);
                    int elementCount = ModelManager.GetInstance().GetModel(ref _staticModelComps[idx]).ElementCount;

                    //Bind ShadowMaps to their respective texture units
                    for (int i = 0; i < _world.ObjectMemory; i++)
                    {
                        if (_world.GameObjects[i].Components.HasFlag(Component.LIGHT))
                            _world.LightComps[i].LightObject.ShadowMap.BindTexture();
                    }

                    GL.DrawElements(PrimitiveType.Triangles, elementCount, DrawElementsType.UnsignedInt, IntPtr.Zero);
                }
            }
        }


        public void RenderPost(ref SystemTransform systemTransform)
        {
            for (int idx = 0; idx < _world.ObjectMemory; idx++)
            {
                if (_world.ExistingGameObjects[idx])
                {
                    Component comps = _world.GameObjects[idx].Components;
                    if (comps.HasFlag(RenderQualfier) && _renderComps[idx].IsFinalRenderPlane)
                    {
                        Bind(idx, true, _world);
                        //var s = ShaderManager.GetInstance().GetProgram("shadowTextured2");
                        //GL.UseProgram(s);


                        if (comps.HasFlag(Component.TEXTURE))
                        {
                            _fbos[0].Texture.Bind(TextureUnit.Texture9, false);
                            //GL.ActiveTexture(TextureUnit.Texture0);
                            //GL.BindTexture(TextureTarget.Texture2D, _fbos[0].Texture.TextureId);
                            //Debug.WriteLine(_fbos[0].Texture.TextureId);
                            //Debug.WriteLine(GL.GetInteger(GetPName.ActiveTexture));

                        }

                        systemTransform.PushModelMatrix(idx, _world);
                        int elementCount = ModelManager.GetInstance().GetModel(ref _staticModelComps[idx]).ElementCount;

                        GL.DrawElements(PrimitiveType.Triangles, elementCount, DrawElementsType.UnsignedInt, IntPtr.Zero);
                    }
                }
            }
        }

        public void RenderUI(ref SystemTransform systemTransform)
        {
            GL.Disable(EnableCap.DepthTest);
            //GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusDstColor);
            for (int idx = 0; idx < _ui.ObjectMemory; idx++)
            {
                if (_ui.ExistingGameObjects[idx])
                {
                    Component comps = _ui.GameObjects[idx].Components;
                    if (comps.HasFlag(RenderQualfier) && !_ui.RenderComps[idx].IsFinalRenderPlane && comps.HasFlag(Component.UI_ELEMENT))
                    {
                        Bind(idx, true, _ui);

                        if (comps.HasFlag(Component.MATERIAL))
                        {
                            Color4 color = _ui.MaterialComps[idx].Color;
                            GL.Uniform4(17, color);
                        }

                        if (comps.HasFlag(Component.TEXTURE))
                        {
                            _ui.TextureComps[idx].BindTexture();
                        }
                        //var m4 = _ui.TransComps[idx].GetModelMatrix;
                        //Console.Out.WriteLine(m4);
                        systemTransform.PushModelMatrix(idx, _ui);
                        int elementCount = ModelManager.GetInstance().GetModel(ref _ui.StaticModelComps[idx]).ElementCount;

                        GL.DrawElements(PrimitiveType.Triangles, elementCount, DrawElementsType.UnsignedInt, IntPtr.Zero);
                    }
                }
            }
            GL.Enable(EnableCap.DepthTest);
            //GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
        }

        private void BindShadowMaps()
        {
            for (int i = 0; i < _world.ObjectMemory; i++)
            {
                if (_world.GameObjects[i].Components.HasFlag(Component.LIGHT))
                    _world.LightComps[i].LightObject.ShadowMap.BindTexture();
            }
        }
    }
}

//Outline feature in Render
//TEST

/*
    GL.UseProgram(ShaderManager.GetInstance().GetProgram("detectEdgesTest"));
    systemTransform.PushModelMatrix(0, _world);
    Bind(0, false, _world);
    if (GL.GetInteger(GetPName.TransformFeedbackBinding) != TEST_VAO.Buffers[1])
        GL.BindBufferBase(BufferRangeTarget.TransformFeedbackBuffer, 0, TEST_VAO.Buffers[1]);
    GL.BeginTransformFeedback(TransformFeedbackPrimitiveType.Lines);
    GL.DrawElements(PrimitiveType.Triangles, ModelManager.GetInstance().GetModel(ref _staticModelComps[0]).ElementCount, DrawElementsType.UnsignedInt, IntPtr.Zero);
    GL.EndTransformFeedback();
*/

//ENDTEST

//Possibility for the future
/*
 * Add automatic shader attribute assigning so that shaders can be more optimized with layouts.
 */

//General advice for the future
/*
 * Make this game engine even more automated than it is.  A large and daunting, but good goal would be adding rendering cues and batched render calls to reduce load on cpu with gpu calls.
 * Difficult and long algorithms will need to be written so that more processes can be automated on startup and to make the engine more modular and versailte in its uses.
 * These algorithms should have the goal of not reducing the efficiency of rendering or real-time side of things but instead should only impact start-up.
 */

//Notes on ways to optimize for the future
//    Store multiple objects inside of the same vertex array to eliminate calls that bind the VAO as they are expensive
//    In addition to the previous point, objects that are not independent of one another (say terrain data) can be drawn by instancing with little or no updates to the buffer data
//    Use VAO's as a way of organizing objects into render layers and groups instead of a client side render group class
//    Store all texture locations within an atlas as a long buffer within the vao?  Then to update the animation position on sprite sheets simply call a different range on draw elements or something like that.
//    Store transformation matrices in a buffer and update only when necessary
