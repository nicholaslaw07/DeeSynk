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
    public class SunLamp : Light
    {
        //TODO add attenuation option for sunlamp (boolean)

        public override Color4 EmissionColor { get => _emissionColor; set => _emissionColor = value; }

        private ViewMatrix _view;
        private OrthographicMatrix _projection;
        private Matrix4 _viewProjection;

        public override int BufferSize => 16 * 8; //matrix, location, lookAt, color, fov (assume 1.0 aspect ratio)  (rgb, fov)

        public SunLamp(Color4 emissionColor, Vector3 location, Vector3 lookAt, Vector3 up, float width, float height, float zNear, float zFar)
        {
            _emissionColor = emissionColor;

            _view = new ViewMatrix(location, lookAt, up);
            _projection = new OrthographicMatrix(width, height, zNear, zFar);

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

            _initUBO = true;

            GL.BindBuffer(BufferTarget.UniformBuffer, 0);
        }

        public override void BuildUBO(int uboId, int uboSize, int offset, int bindingLocation, int numOfVec4s)
        {
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
            _bufferData[5] = new Vector4(lookAtTranslated, 1.0f);
            _bufferData[6] = new Vector4(_emissionColor.R, _emissionColor.G, _emissionColor.B, 1.0f);
            _bufferData[7] = new Vector4(_projection.Width, _projection.Height, _projection.ZNear, _projection.ZFar);
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
            if (_view.ValueModified || _projection.ValueModified)
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
