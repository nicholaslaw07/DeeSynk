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
    class SystemRender : ISystem
    {
        public int MonitoredComponents => (int)Component.RENDER |
                                          (int)Component.MODEL_STATIC |
                                          (int)Component.MODEL_DYNAMIC |
                                          (int)Component.TEXTURE |
                                          (int)Component.COLOR;

        private World _world;

        private bool[] _monitoredGameObjects;

        private ComponentRender[] _renderComps;
        private ComponentModelStatic[] _staticModelComps;
        private ComponentTexture[] _textureComps;

        //SHADOW START
        private Vector3 _lightLocation = new Vector3(0, 2, 6);
        private Vector3 _lightLookAt = new Vector3(0);
        private Vector3 _lightUp = new Vector3(0, 1, 0);
        private Matrix4 _lightView;
        private Matrix4 _lightOrtho;

        private Camera _camera;

        private int _fbo;      //Frame Buffer (Depth)
        private int _depthMap; //Texture

        private int _width = 8192;
        private int _height = 8192;

        private int ubo = 0;

        //SHADOW END

        public SystemRender(World world)
        {
            _world = world;

            _monitoredGameObjects = new bool[_world.ObjectMemory];

            _renderComps = _world.RenderComps;
            _staticModelComps = _world.StaticModelComps;
            _textureComps = _world.TextureComps;

            //UpdateMonitoredGameObjects();

            //SHADOW START

            _fbo = GL.GenFramebuffer();
            _depthMap = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, _depthMap);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.DepthComponent32, _width, _height, 0, PixelFormat.DepthComponent, PixelType.Float, IntPtr.Zero);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (float)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (float)TextureMagFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (float)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (float)TextureWrapMode.ClampToEdge);

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, _fbo);
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, TextureTarget.Texture2D, _depthMap, 0);

            GL.DrawBuffer(DrawBufferMode.None);
            GL.ReadBuffer(ReadBufferMode.None);

            var status = GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer);
            if (status != FramebufferErrorCode.FramebufferComplete)
                Console.WriteLine(status);

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);

            _lightView = Matrix4.LookAt(_lightLocation, _lightLookAt, _lightUp);
             _lightOrtho = Matrix4.CreatePerspectiveFieldOfView(1.0f, _width/(float)_height, 5f, 11f);
            //_lightOrtho = Matrix4.CreateOrthographic(12f, 8f, 10f, 25f);
            _lightView *= _lightOrtho;

            ubo = GL.GenBuffer();

            //SHADOW END
        }

        public void PushCameraRef(ref Camera camera)
        {
            _camera = camera;
            BuildUBO();
        }

        public void BuildUBO()
        {
            GL.BindBuffer(BufferTarget.UniformBuffer, ubo);
            float[] data = new float[16];
            data[0] = _camera.Projection.Column0.X;
            data[1] = _camera.Projection.Column0.Y;
            data[2] = _camera.Projection.Column0.Z;
            data[3] = _camera.Projection.Column0.W;
            data[4] = _camera.Projection.Column1.X;
            data[5] = _camera.Projection.Column1.Y;
            data[6] = _camera.Projection.Column1.Z;
            data[7] = _camera.Projection.Column1.W;
            data[8] = _camera.Projection.Column2.X;
            data[9] = _camera.Projection.Column2.Y;
            data[10] = _camera.Projection.Column2.Z;
            data[11] = _camera.Projection.Column2.W;
            data[12] = _camera.Projection.Column3.X;
            data[13] = _camera.Projection.Column3.Y;
            data[14] = _camera.Projection.Column3.Z;
            data[15] = _camera.Projection.Column3.W;
            //data[16] = _camera.Location.X;
            //data[17] = _camera.Location.Y;
            //data[18] = _camera.Location.Z;
            Matrix4[] matrices = new Matrix4[1];
            matrices[0] = _camera.ViewProjection;
            GL.BufferData(BufferTarget.UniformBuffer, 64, matrices, BufferUsageHint.DynamicDraw | BufferUsageHint.DynamicRead);
            GL.BindBufferRange(BufferRangeTarget.UniformBuffer, 17, ubo, IntPtr.Zero, 64);
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

        public void Bind(int idx)
        {
            _renderComps[idx].BindData();
        }

        public void Render(int idx)
        {
            //GL.DrawElements(BeginMode.Triangles, _staticModelComps[idx].IndexCount, DrawElementsType.UnsignedInt, 0);
        }

        public void UnBind()
        {
            GL.UseProgram(0);
            GL.BindVertexArray(0);
        }

        public void RenderAll(ref SystemTransform systemTransform)
        {
            for (int idx = 0; idx < _renderComps.Length; idx++)
            {
                Bind(idx);
                systemTransform.PushMatrixData(idx);
                Render(idx);
            }
        }
        
        //SHADOW START

        private void BindFBO()
        {
            GL.BindFramebuffer(FramebufferTarget.DrawFramebuffer, _fbo);
        }

        private void UnBindFBO()
        {
            GL.BindFramebuffer(FramebufferTarget.DrawFramebuffer, 0);
        }

        private void BindDepthMap()
        {
            GL.ActiveTexture(TextureUnit.Texture1);
            GL.BindTexture(TextureTarget.Texture2D, _depthMap);
        }

        public void RenderDepthMap(ref SystemTransform systemTransform)
        {
            GL.Viewport(0, 0, _width, _height);
            BindFBO();

            Bind(1);
            GL.UseProgram(ShaderManager.GetInstance().GetProgram("shadowTextured"));
            GL.UniformMatrix4(5, false, ref _lightView);
            systemTransform.PushModelMatrix(1);
            int y = ModelManager.GetInstance().GetModel(_staticModelComps[1].ModelID).ElementCount;
            GL.DrawElements(PrimitiveType.Triangles, y, DrawElementsType.UnsignedInt, IntPtr.Zero);

            GL.Clear(ClearBufferMask.DepthBufferBit);
            Bind(0);
            GL.UseProgram(ShaderManager.GetInstance().GetProgram("shadowTextured"));
            GL.UniformMatrix4(5, false, ref _lightView);
            systemTransform.PushModelMatrix(0);
            int x = ModelManager.GetInstance().GetModel(_staticModelComps[0].ModelID).ElementCount;
            GL.DrawElements(PrimitiveType.Triangles, x, DrawElementsType.UnsignedInt, IntPtr.Zero);
            UnBindFBO();

            GL.Viewport(0, 0, (int)_camera.Width, (int)_camera.Height);
        }

        //SHADOW END

        //RENDER PASS METHOD

        public void RenderInstanced(ref SystemTransform systemTransform, int renderIdx)
        {
            RenderDepthMap(ref systemTransform);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            Bind(0);
            GL.UseProgram(ShaderManager.GetInstance().GetProgram("coloredPhongShaded"));
            systemTransform.PushMatrixDataNoTransform();
            Matrix4[] matrices = new Matrix4[1];
            matrices[0] = _camera.ViewProjection;
            GL.BufferSubData(BufferTarget.UniformBuffer, IntPtr.Zero, 64, matrices);

            //GL.BindBuffer(BufferTarget.UniformBuffer, ubo);
            //var ptr = GL.MapBuffer(BufferTarget.UniformBuffer, BufferAccess.ReadWrite);
            //GL.GetBufferSubData(BufferTarget.UniformBuffer, IntPtr.Zero, 64, ptr);
            //GL.BindBufferRange(BufferRangeTarget.UniformBuffer, 17, ubo, IntPtr.Zero, 64);
            int loc = GL.GetUniformLocation(GL.GetInteger(GetPName.CurrentProgram), "color");
            var colorArr = _staticModelComps[0].GetConstructionParameter(ConstructionFlags.COLOR4_COLOR);
            Color4 color = new Color4(colorArr[0], colorArr[1], colorArr[2], colorArr[3]);
            GL.Uniform4(loc, color);
            GL.UniformMatrix4(9, false, ref _lightView);
            systemTransform.PushModelMatrix(0);
            BindDepthMap();
            int x = ModelManager.GetInstance().GetModel(_staticModelComps[0].ModelID).ElementCount;
            GL.DrawElementsInstanced(PrimitiveType.Triangles, x, DrawElementsType.UnsignedInt, IntPtr.Zero, 1);
            Bind(1);

            GL.UseProgram(ShaderManager.GetInstance().GetProgram("shadowTextured2"));
            systemTransform.PushMatrixDataNoTransform();
            GL.UniformMatrix4(9, false, ref _lightView);
            systemTransform.PushModelMatrix(1);
            _textureComps[1].BindTexture(TextureUnit.Texture0);
            BindDepthMap();
            int y = ModelManager.GetInstance().GetModel(_staticModelComps[1].ModelID).ElementCount;
            GL.DrawElementsInstanced(PrimitiveType.Triangles, y, DrawElementsType.UnsignedInt, IntPtr.Zero, 1);


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
