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
                                          (int)Component.MODEL_STATIC |
                                          (int)Component.MODEL_DYNAMIC |
                                          (int)Component.TEXTURE |
                                          (int)Component.COLOR;

        private World _world;

        private bool[] _monitoredGameObjects;
        
        private ComponentRender[]       _renderComps;
        private ComponentModelStatic[]  _staticModelComps;
        private ComponentTexture[]      _textureComps;
        private ComponentColor[]        _colorComps;

        private Camera _camera;

        //SHADOw STUFF START

        private int depthMapFBO;
        private const int sWidth = 2048;
        private const int sHeight = 2048;

        private int depthMap;


        //SHADOW STUFF END

        public SystemRender(World world)
        {
            _world = world;

            _monitoredGameObjects = new bool[_world.ObjectMemory];

            _renderComps = _world.RenderComps;
            _staticModelComps = _world.StaticModelComps;
            _textureComps = _world.TextureComps;
            _colorComps = _world.ColorComps;

            //UpdateMonitoredGameObjects();

            //SHADOW STUFF START

            depthMapFBO = GL.GenFramebuffer();

            depthMap = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, depthMap);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.DepthComponent, sWidth, sHeight, 0, PixelFormat.DepthComponent, PixelType.Float, IntPtr.Zero);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);

            //SHADOW STUFF END
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

        public void PushCameraRef(ref Camera camera)
        {
            _camera = camera;
        }

        public void Update(float time)
        {
        }

        public void Bind(int idx)
        {
            _renderComps[idx].BindData();
            //_textureComps[idx].BindTexture();
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
            //DEPTH MAP
            Vector3 light = new Vector3(50, 50, 50);

            GL.Viewport(0, 0, sWidth, sHeight);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, depthMapFBO);
            GL.Clear(ClearBufferMask.DepthBufferBit);

            Matrix4 lightProjection;
            Matrix4.CreateOrthographic(-10f, 10f, -1.0f, 1000f, out lightProjection);

            var lightView = Matrix4.LookAt(new Vector3(50, 50, 50), new Vector3(0, 0, 0), new Vector3(0, 1, 0));

            var lightSpace = lightProjection * lightView;

            _renderComps[0].BindData();

            GL.UseProgram(ShaderManager.GetInstance().GetProgram("depthShader"));

            GL.UniformMatrix4(3, false, ref lightSpace);

            int x = ModelManager.GetInstance().GetModel(_staticModelComps[0].ModelID).VertexIndices.Length;
            GL.DrawElementsInstanced(PrimitiveType.Triangles, x, DrawElementsType.UnsignedInt, IntPtr.Zero, (int)_world.ObjectMemory - 1);

            _renderComps[1].BindData();

            GL.UseProgram(ShaderManager.GetInstance().GetProgram("depthShader"));

            GL.UniformMatrix4(3, false, ref lightSpace);

            int y = ModelManager.GetInstance().GetModel(_staticModelComps[1].ModelID).VertexIndices.Length;
            GL.DrawElementsInstanced(PrimitiveType.Triangles, y, DrawElementsType.UnsignedInt, IntPtr.Zero, 1);

            //DEPTH MAP END
            GL.Viewport(0, 0, (int)_camera.Width, (int)_camera.Height);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);

            //NORMAL RENDER
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);


            Bind(0);

            GL.UseProgram(ShaderManager.GetInstance().GetProgram("coloredShaded"));
            GL.BindTexture(TextureTarget.Texture2D, depthMap);
            systemTransform.PushMatrixDataNoTransform();
            GL.UniformMatrix4(10, false, ref lightSpace);
            var v = _camera.Location;
            x = ModelManager.GetInstance().GetModel(_staticModelComps[0].ModelID).VertexIndices.Length;
            GL.DrawElementsInstanced(PrimitiveType.Triangles, x, DrawElementsType.UnsignedInt, IntPtr.Zero, (int)_world.ObjectMemory - 1);

        }
    }
}
