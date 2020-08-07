using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Graphics;
using DeeSynk.Core.Components.Types.Matrices;
using DeeSynk.Core.Components.GraphicsObjects.Shadows;

namespace DeeSynk.Core.Components.GraphicsObjects.Lights
{
    public class SpotLight : Light
    {
        public override Color4 EmissionColor { get => _emissionColor; set => _emissionColor = value; }

        private ViewMatrix _view;
        public ViewMatrix View { get => _view; }
        private PerspectiveMatrix _projection;
        public PerspectiveMatrix Projection { get => _projection; }
        private Matrix4 _viewProjection;
        public ref Matrix4 ViewProjection { get => ref _viewProjection; }

        public override int BufferSize => 16 * 8; //matrix, location, lookAt, color, fov (assume 1.0 aspect ratio)  (rgb, fov)

        public SpotLight(Color4 emissionColor, Vector3 location, Vector3 lookAt, Vector3 up, float fov, float aspect, float zNear, float zFar)
        {
            _emissionColor = emissionColor;

            _view = new ViewMatrix(location, lookAt, up);
            _projection = new PerspectiveMatrix(fov, aspect, zNear, zFar);

            ViewMatrix.GetViewProjectionProduct(_view, _projection, out _viewProjection);
        }

        #region UBO Managment
        public override void AttachUBO(int bindingLocation)
        {
            GL.BindBufferRange(BufferRangeTarget.UniformBuffer, _bindingLocation, _ubo_Id, IntPtr.Add(IntPtr.Zero, 0), BufferSize);
        }

        public override void BuildUBO(int bindingLocation, int numOfVec4s)
        {
            _ubo_Id = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.UniformBuffer, _ubo_Id);
            _bufferData = new Vector4[numOfVec4s];
            _bindingLocation = bindingLocation;
            FillBuffer();
            GL.BufferData(BufferTarget.UniformBuffer, BufferSize, _bufferData, BufferUsageHint.DynamicRead);
            AttachUBO(bindingLocation);

            //GL.GetBufferParameter(BufferTarget.UniformBuffer, BufferParameterName.BufferSize, out int sizeQuery);
            //if (GL.IsBuffer(_ubo_Id) && sizeQuery == BufferSize)
            _initUBO = true;

            GL.BindBuffer(BufferTarget.UniformBuffer, 0);
        }

        public override void BuildUBO(int uboId, int uboSize, int offset, int bindingLocation, int numOfVec4s)
        {
            //automatically grow ubo if buffer is too big? or throw error?
            _ubo_Id = uboId;
            GL.BindBuffer(BufferTarget.UniformBuffer, _ubo_Id);
            _bufferData = new Vector4[numOfVec4s];
            _bindingLocation = bindingLocation;
            _bufferOffset = offset;
            FillBuffer();
            GL.BufferSubData(BufferTarget.UniformBuffer, IntPtr.Add(IntPtr.Zero, _bufferOffset), numOfVec4s * 16, _bufferData);
            AttachUBO(bindingLocation);
            _initUBO = true;

            GL.BindBuffer(BufferTarget.UniformBuffer, 0);
        }

        public override void DetatchUBO()
        {
            GL.BindBufferRange(BufferRangeTarget.UniformBuffer, _bindingLocation, 0, IntPtr.Zero, 0);
        }

        public override void FillBuffer()
        {
            Vector3 lookAtTranslated = (_view.LookAt - _view.Location);
            lookAtTranslated.Normalize();
            _bufferData[0] = _viewProjection.Row0;
            _bufferData[1] = _viewProjection.Row1;
            _bufferData[2] = _viewProjection.Row2;
            _bufferData[3] = _viewProjection.Row3;
            _bufferData[4] = new Vector4(_view.Location, 1.0f);
            _bufferData[5] = new Vector4(lookAtTranslated, 1.0f);  //translated to the location of the light
            _bufferData[6] = new Vector4(_emissionColor.R, _emissionColor.G, _emissionColor.B, 1.0f);
            _bufferData[7] = new Vector4((float)Math.Cos(_projection.FOV / 2.0f), _projection.ZNear, _projection.ZFar, _projection.ZFar - _projection.ZNear); //make fov the alpha value of the color if data size is an issue
            // fov near far near-far
        }

        public override void UpdateUBO()
        {
            GL.BindBuffer(BufferTarget.UniformBuffer, _ubo_Id);
            FillBuffer();
            GL.BufferSubData(BufferTarget.UniformBuffer, IntPtr.Zero, BufferSize, _bufferData);
            GL.BindBuffer(BufferTarget.UniformBuffer, 0);
        }
        #endregion

        public void Update()
        {
            if(_view.ValueModified || _projection.ValueModified)
            {
                _view.Update();
                _projection.Update();

                ViewMatrix.GetViewProjectionProduct(_view, _projection, out _viewProjection);

                if (_initUBO)
                    UpdateUBO();
            }
        }
    }
}
