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

            var gameObjects = _world.GameObjects;
            GL.Enable(EnableCap.CullFace);

            GL.CullFace(CullFaceMode.Front);
            for(int jdx = 0; jdx < _world.ObjectMemory; jdx++)
            {
                if (gameObjects[jdx].Components.HasFlag(Component.LIGHT))
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
                                if (gameObjects[idx].Components.HasFlag(RenderQualfier))
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

                        _world.LightComps[2].LightObject.ShadowMap.BindTexture(TextureUnit.Texture1, true);
                        _world.LightComps[3].LightObject.ShadowMap.BindTexture(TextureUnit.Texture2, true);
                        _world.LightComps[4].LightObject.ShadowMap.BindTexture(TextureUnit.Texture3, true);

                        _world.LightComps[5].LightObject.ShadowMap.BindTexture(TextureUnit.Texture5, true);

                        if(idx == 0)
                            GL.DrawElements(PrimitiveType.Triangles, elementCount, DrawElementsType.UnsignedInt, IntPtr.Zero);
                    }
                }
            }

            AlgorithmEdgeDetectMesh detectMesh = new AlgorithmEdgeDetectMesh(ModelManager.GetInstance().GetModel("TestCube"), _world.LightComps[4]);
            var edges = detectMesh.Start();
            Vector4[] vecs = new Vector4[edges.Length * 2];
            for(int i=0; i<edges.Length; i++)
            {
                vecs[2 * i + 0] = edges[i].p1;
                vecs[2 * i + 1] = edges[i].p2;
            }
            TEST_VAO.BindVAO();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, TEST_VAO.Buffers[0]);
            uint[] elements = new uint[vecs.Length];
            for (uint i = 0; i < elements.Length; i++)
                elements[i] = i;
            GL.GetBufferParameter(BufferTarget.ElementArrayBuffer, BufferParameterName.BufferSize, out int els);
            if(els == 0)
            {
                GL.NamedBufferStorage(TEST_VAO.Buffers[0], elements.Length * 4, elements, BufferStorageFlags.MapReadBit);
            }

            //Console.WriteLine("oh my");
            GL.UseProgram(ShaderManager.GetInstance().GetProgram("defaultColored"));
            int buffer = TEST_VAO.Buffers[1];
            GL.BindBuffer(BufferTarget.ArrayBuffer, buffer);
            GL.GetBufferParameter(BufferTarget.ArrayBuffer, BufferParameterName.BufferSize, out int size);
            if (size == 0)
            {
                GL.NamedBufferStorage(buffer, vecs.Length * 16, vecs, BufferStorageFlags.MapReadBit);
            }
            //Console.WriteLine(size);
            GL.BindVertexBuffer(0, buffer, IntPtr.Zero, 16);
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribFormat(0, 4, VertexAttribType.Float, false, 0);
            GL.VertexAttribBinding(0, 0);
            systemTransform.PushModelMatrix(0);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, TEST_VAO.Buffers[0]);
            GL.DrawElements(PrimitiveType.Lines, elements.Length, DrawElementsType.UnsignedInt, IntPtr.Zero);
            //Console.WriteLine(GL.GetError());
        }
    }
}


//Notes on ways to optimize for the future
//    Store multiple objects inside of the same vertex array to eliminate calls that bind the VAO as they are expensive
//    In addition to the previous point, objects that are not independent of one another (say terrain data) can be drawn by instancing with little or no updates to the buffer data
//    Use VAO's as a way of organizing objects into render layers and groups instead of a client side render group class
//    Store all texture locations within an atlas as a long buffer within the vao?  Then to update the animation position on sprite sheets simply call a different range on draw elements or something like that.
//    Store transformation matrices in a buffer and update only when necessary
