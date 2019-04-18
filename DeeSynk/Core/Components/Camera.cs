using OpenTK;
using OpenTK.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;

namespace DeeSynk.Core.Components
{
    public enum CameraMode
    {
        PERSPECTIVE = 0,
        ORTHOGRAPHIC = 1,
        PERSPECTIVE_OFF_CENTER = 2,
        ORTHOGRAPHIC_OFF_CENTER = 3
    }
    public class Camera
    {
        public static readonly int DEFAULT_BINDING = 2;

        #region ViewProperties
        public static readonly Vector3 DEFAULT_LOCATION = Vector3.Zero;
        public static readonly Vector3 DEFAULT_LOOK = new Vector3(0.0f, 0.0f, -1.0f);
        public static readonly Vector3 DEFAULT_UP = new Vector3(0.0f, 1.0f, 0.0f);

        private Matrix4 _view;
        /// <summary>
        /// The matrix that represents the lookAt matrix for the camera.
        /// </summary>
        public ref Matrix4 View { get => ref _view; }

        private bool _viewUpdate;

        private Vector3 _location, _lookAt, _up;
        /// <summary>
        /// Location of the camera in world space.
        /// </summary>
        public Vector3 Location { get => _location; set { _location = value; _viewUpdate = true; } }
        /// <summary>
        /// Location where the camera is looking at (can also be interpetreted as the position on the surface of sphere post transformation in world space)
        /// </summary>
        public Vector3 LookAt { get => _lookAt; set { _lookAt = value; _viewUpdate = true; } }
        /// <summary>
        /// Determines which direction is up relatve to the camera.
        /// </summary>
        public Vector3 Up { get => _up; set { _up = value; _viewUpdate = true; } }
        #endregion

        #region RotationProperties
        private bool _rotationUpdate;

        private float _rotX, _rotY;
        /// <summary>
        /// Rotation of the camera about the x-axis (mouse down and up).
        /// </summary>
        public float CameraRotationX { get => _rotX; set { _rotX = value; _viewUpdate = true; _rotationUpdate = true; } }
        /// <summary>
        /// Rotation of the camera about the y-axis (mouse right and left).
        /// </summary>
        public float CameraRotationY { get => _rotY; set { _rotY = value; _viewUpdate = true; _rotationUpdate = true; } }

        private Quaternion qX, qY, p, qXi, qYi;
        #endregion

        #region ProjectionProperties
        private Matrix4 _projection;
        /// <summary>
        /// The projection matrix for this camera: either perspective or orthographic.
        /// </summary>
        public ref Matrix4 Projection { get => ref _projection; }

        private CameraMode _cameraMode;
        /// <summary>
        /// States which type of projection matrix this camera is using.
        /// </summary>
        public CameraMode Mode { get => _cameraMode; }
        private bool _projectionUpdate;

        public static readonly float DEFAULT_FOV = 1.3f;

        private float _fov;
        public float FOV { get => _fov; set { _fov = value; _projectionUpdate = true; } }

        private float _width, _height, _aspect;
        public float Width { get => _width; set { _width = value; if (_cameraMode == CameraMode.PERSPECTIVE) { _aspect = _width / _height; } _projectionUpdate = true; } }
        public float Height { get => _height; set { _height = value; if (_cameraMode == CameraMode.PERSPECTIVE) { _aspect = _width / _height; } _projectionUpdate = true; } }
        public float Aspect { get => _aspect; }

        private float _left, _right, _bottom, _top;
        public float Left { get => _left; set { _left = value; _projectionUpdate = true; } }
        public float Right { get => _right; set { _right = value; _projectionUpdate = true; } }

        public float Bottom { get => _bottom; set { _bottom = value; _projectionUpdate = true; } }
        public float Top { get => _top; set { _top = value; _projectionUpdate = true; } }

        private float _zNear, _zFar;
        public float ZNear { get => _zNear; set { _zNear = value; _projectionUpdate = true; } }
        public float ZFar { get => _zFar; set { _zFar = value; _projectionUpdate = true; } }
        #endregion

        #region UBOProperties
        private bool _initUBO;
        public bool InitUBO { get => _initUBO; }

        private int _ubo;
        public int UBO { get => _ubo; }

        private int _bindingLocation;

        private Vector4[] vec4s;
        #endregion

        private Matrix4 _viewProjection;
        public ref Matrix4 ViewProjection { get => ref _viewProjection; }

        public Camera()
        {
            SetView(DEFAULT_LOCATION, DEFAULT_LOOK, DEFAULT_UP);
            SetOrthographic(1, 1, -1, 1);
            BuildQuarternionBlock();
            UpdateMatrices();
        }
        public Camera(float fov, float width, float height, float zNear, float zFar)
        {
            SetView(Vector3.Zero, new Vector3(0.0f, 0.0f, -1.0f), new Vector3(0.0f, 1.0f, 0.0f));
            SetPerspectiveFOV(fov, width, height, zNear, zFar);
            BuildQuarternionBlock();
            UpdateMatrices();

        }
        public Camera(float fov, float width, float height, float zNear, float zFar, Vector3 location, Vector3 lookAt, Vector3 up)
        {
            SetView(location, lookAt, up);
            SetPerspectiveFOV(fov, width, height, zNear, zFar);
            BuildQuarternionBlock();
            UpdateMatrices();
        }
        public Camera(CameraMode cameraMode, float width, float height, float zNear, float zFar)
        {
            if(cameraMode == CameraMode.ORTHOGRAPHIC)
            {
                SetView(DEFAULT_LOCATION, DEFAULT_LOOK, DEFAULT_UP);
                SetOrthographic(width, height, zNear, zFar);
                BuildQuarternionBlock();
                UpdateMatrices();
            }
            else if(cameraMode == CameraMode.PERSPECTIVE)
            {
                SetView(DEFAULT_LOCATION, DEFAULT_LOOK, DEFAULT_UP);
                SetPerspectiveFOV(DEFAULT_FOV, width, height, zNear, zFar);
                BuildQuarternionBlock();
                UpdateMatrices();
            }
        }
        public Camera(CameraMode cameraMode, float width, float height, float zNear, float zFar, Vector3 location, Vector3 lookAt, Vector3 up)
        {
            if (cameraMode == CameraMode.ORTHOGRAPHIC)
            {
                SetView(location, lookAt, up);
                SetOrthographic(width, height, zNear, zFar);
                BuildQuarternionBlock();
                UpdateMatrices();
            }
            else if (cameraMode == CameraMode.PERSPECTIVE)
            {
                SetView(location, lookAt, up);
                SetPerspectiveFOV(DEFAULT_FOV, width, height, zNear, zFar);
                BuildQuarternionBlock();
                UpdateMatrices();
            }
        }
        public Camera(CameraMode cameraMode, float left, float right, float bottom, float top, float zNear, float zFar)
        {
            if(cameraMode == CameraMode.ORTHOGRAPHIC_OFF_CENTER)
            {
                SetView(DEFAULT_LOCATION, DEFAULT_LOOK, DEFAULT_UP);
                SetOrthographicOffCenter(left, right, bottom, top, zNear, zFar);
                BuildQuarternionBlock();
                UpdateMatrices();
            }
            else if(cameraMode == CameraMode.PERSPECTIVE_OFF_CENTER)
            {
                SetView(DEFAULT_LOCATION, DEFAULT_LOOK, DEFAULT_UP);
                SetPerspectiveOffCenter(left, right, bottom, top, zNear, zFar);
                BuildQuarternionBlock();
                UpdateMatrices();
            }
        }
        public Camera(CameraMode cameraMode, float left, float right, float bottom, float top, float zNear, float zFar, Vector3 location, Vector3 lookAt, Vector3 up)
        {
            if (cameraMode == CameraMode.ORTHOGRAPHIC_OFF_CENTER)
            {
                SetView(location, lookAt, up);
                SetOrthographicOffCenter(left, right, bottom, top, zNear, zFar);
                BuildQuarternionBlock();
                UpdateMatrices();
            }
            else if (cameraMode == CameraMode.PERSPECTIVE_OFF_CENTER)
            {
                SetView(location, lookAt, up);
                SetPerspectiveOffCenter(left, right, bottom, top, zNear, zFar);
                BuildQuarternionBlock();
                UpdateMatrices();
            }
        }

        private void BuildQuarternionBlock()
        {
            qX = new Quaternion(0.0f, 0.0f, 0.0f);
            qY = new Quaternion(0.0f, 0.0f, 0.0f);
            p = new Quaternion(0.0f, 0.0f, -1.0f, 1.0f);
            qYi = qY.Inverted();
            qXi = qX.Inverted();
        }

        #region CameraLook
        public void SetView(Vector3 location, Vector3 lookAt, Vector3 up)
        {
            Location = location;
            _lookAt = lookAt;
            _up = up;
        }
        #endregion

        #region ProjectionModes
        public void SetPerspectiveFOV(float fov, float width, float height, float zNear, float zFar)
        {
            _cameraMode = CameraMode.PERSPECTIVE;

            _fov = fov;
            _height = height;
            Width = width;
            _zNear = zNear;
            _zFar = zFar;
        }
        public void SetPerspectiveOffCenter(float left, float right, float bottom, float top, float zNear, float zFar)
        {
            _cameraMode = CameraMode.PERSPECTIVE_OFF_CENTER;

            Left = left;
            _right = right;
            _bottom = bottom;
            _top = top;
            _zNear = zNear;
            _zFar = zFar;
        }

        public void SetOrthographic(float width, float height, float zNear, float zFar)
        {
            _cameraMode = CameraMode.ORTHOGRAPHIC;

            Width = width;
            _height = height;
            _zNear = zNear;
            _zFar = zFar;
        }
        public void SetOrthographicOffCenter(float left, float right, float bottom, float top, float zNear, float zFar)
        {
            _cameraMode = CameraMode.ORTHOGRAPHIC_OFF_CENTER;

            Left = left;
            _right = right;
            _bottom = bottom;
            _top = top;
            _zNear = zNear;
            _zFar = zFar;
        }
        #endregion

        #region UBO
        public void BuildUBO(int bindingLocation)
        {
            _ubo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.UniformBuffer, _ubo);
            vec4s = new Vector4[5];
            FillVectorBuffer();
            GL.BufferData(BufferTarget.UniformBuffer, 80, vec4s, BufferUsageHint.DynamicRead);
            GL.BindBuffer(BufferTarget.UniformBuffer, 0);
            AttachUBO(bindingLocation);

            if (_ubo != 0)
                _initUBO = true;
        }

        public void AttachUBO(int bindingLocation)
        {
            _bindingLocation = bindingLocation;
            GL.BindBufferRange(BufferRangeTarget.UniformBuffer, _bindingLocation, _ubo, IntPtr.Zero, 80);
        }
        public void DetatchUBO()
        {
            GL.BindBufferRange(BufferRangeTarget.UniformBuffer, _bindingLocation, 0, IntPtr.Zero, 0);
        }

        private void UpdateUBO()
        {
            GL.BindBuffer(BufferTarget.UniformBuffer, _ubo);
            FillVectorBuffer();
            GL.BufferSubData(BufferTarget.UniformBuffer, IntPtr.Zero, 80, vec4s);
            GL.BindBuffer(BufferTarget.UniformBuffer, 0);
        }
        private void FillVectorBuffer()
        {
            vec4s[0] = _viewProjection.Row0;
            vec4s[1] = _viewProjection.Row1;
            vec4s[2] = _viewProjection.Row2;
            vec4s[3] = _viewProjection.Row3;
            vec4s[4] = new Vector4(_location);
        }
        #endregion

        #region CameraTransform
        public void AddLocation(float dx, float dy, float dz)
        {
            if(dx != 0 || dy != 0 || dz != 0)
                Location += (qY * qX * new Quaternion(dx, dy, dz, 1.0f) * qXi * qYi).Xyz;
        }
        public void AddLocation(ref Vector3 dX)
        {
            if (dX != Vector3.Zero)
                Location += (qY * qX * new Quaternion(dX, 1.0f) * qXi * qYi).Xyz;
        }
        public void AddLocation(ref Vector3 dX, float time)
        {
            if (dX != Vector3.Zero)
                Location += (qY * qX * new Quaternion(dX * time, 1.0f) * qXi * qYi).Xyz;
        }

        public void AddRotation(float rx, float ry)
        {
            if (rx != 0) CameraRotationX += rx;
            if (ry != 0) CameraRotationY += ry;
        }

        public void UpdateRotation()
        {
            if (_rotationUpdate)
            {
                if (_rotX > 1.57f) _rotX = 1.57f;
                else if (_rotX < -1.57f) _rotX = -1.57f;

                if (_rotY > 6.283185f || _rotY < -6.283185f) _rotY %= 6.283185f;

                qX = new Quaternion(_rotX, 0.0f, 0.0f);
                qY = new Quaternion(0.0f, _rotY, 0.0f);
                qXi = qX.Inverted();
                qYi = qY.Inverted();

                _rotationUpdate = false;
            }
        }
        #endregion

        public void UpdateMatrices()
        {
            if(_viewUpdate || _projectionUpdate)
            {
                if (_viewUpdate)
                {
                    _lookAt = _location + (qY * qX * p * qXi * qYi).Xyz;
                    _view = Matrix4.LookAt(_location, _lookAt, _up);
                    _viewUpdate = false;
                }
                if (_projectionUpdate)
                {
                    switch (_cameraMode)
                    {
                        case (CameraMode.ORTHOGRAPHIC): Matrix4.CreateOrthographic(_width, _height, _zNear, _zFar, out _projection); break;
                        case (CameraMode.ORTHOGRAPHIC_OFF_CENTER): Matrix4.CreateOrthographicOffCenter(_left, _right, _bottom, _top, _zNear, _zFar, out _projection); break;
                        case (CameraMode.PERSPECTIVE): Matrix4.CreatePerspectiveFieldOfView(_fov, _aspect, _zNear, _zFar, out _projection); break;
                        case (CameraMode.PERSPECTIVE_OFF_CENTER): Matrix4.CreatePerspectiveOffCenter(_left, _right, _bottom, _top, _zNear, _zFar, out _projection); break;
                    }
                    _projectionUpdate = false;
                }

                Matrix4.Mult(ref _view, ref _projection, out _viewProjection);

                if(_initUBO)
                    UpdateUBO();
            }
        }
    }
}
