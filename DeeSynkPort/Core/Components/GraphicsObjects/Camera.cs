using OpenTK;
using OpenTK.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;
using DeeSynk.Core.Components.GraphicsObjects;
using DeeSynk.Core.Components.Models;
using OpenTK.Mathematics;

namespace DeeSynk.Core.Components
{
    public enum CameraMode
    {
        PERSPECTIVE = 0,
        ORTHOGRAPHIC = 1,
        PERSPECTIVE_OFF_CENTER = 2,
        ORTHOGRAPHIC_OFF_CENTER = 3
    }
    public class Camera : IUBO
    {
        public static readonly int DEFAULT_BINDING = 2;

        #region ViewProperties
        public static readonly Vector3 DEFAULT_LOCATION = Vector3.Zero;
        public static readonly Vector3 DEFAULT_LOOK = new Vector3(0.0f, 0.0f, -1.0f);
        public static readonly Vector3 DEFAULT_UP = new Vector3(0.0f, 1.0f, 0.0f);

        private bool _overrideLookAtVector;
        public bool OverrideLookAtVector { get => _overrideLookAtVector; set => _overrideLookAtVector = value; }

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

                //***NOTE***
                //Add z rotation as an optional thing
                //***ENDNOTE***

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

        private float _width, _height, _aspect, _diameter;
        public float Width { get => _width; set { _width = value; if (_cameraMode == CameraMode.PERSPECTIVE) { _aspect = _width / _height; _diameter = (float)Math.Sqrt(_aspect * _aspect + 1); } _projectionUpdate = true; } }
        public float Height { get => _height; set { _height = value; if (_cameraMode == CameraMode.PERSPECTIVE) { _aspect = _width / _height; _diameter = (float)Math.Sqrt(_aspect * _aspect + 1); } _projectionUpdate = true; } }
        public float Aspect { get => _aspect; }
        public float AspectDiameter { get => _diameter; }



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
        public int UBO_Id { get => _ubo; }

        private int _bindingLocation;
        public int BindingLocation { get => _bindingLocation; }


        private Vector4[] _vec4s;
        public Vector4[] BufferData { get => _vec4s; }

        private int _bufferSize;
        public int BufferSize { get => _bufferSize; }

        #endregion

        private Matrix4 _viewProjection;
        public ref Matrix4 ViewProjection { get => ref _viewProjection; }

        public int BufferOffset => throw new NotImplementedException();

        public int VectorCount { get => 6; }

        /// <summary>
        /// Default constructor, sets to an orthographic view with a space of unit size.  ZFar extends equally far behind as in front.
        /// </summary>
        public Camera()
        {
            SetView(DEFAULT_LOCATION, DEFAULT_LOOK, DEFAULT_UP);
            SetOrthographic(1, 1, -1, 1);
            BuildQuarternionBlock();
            UpdateMatrices();
        }

        /// <summary>
        /// Creates a centered perspective view with default view parameters.
        /// </summary>
        /// <param name="fov">Field of view in radians.</param>
        /// <param name="width">Width of the camera sensor.</param>
        /// <param name="height">Height of the camera sensor.</param>
        /// <param name="zNear">Distance from the camera to begin near-field clipping in world units.</param>
        /// <param name="zFar">Distance from the camera to begin far-field clipping in world units.</param>
        public Camera(float fov, float width, float height, float zNear, float zFar)
        {
            SetView(Vector3.Zero, new Vector3(0.0f, 0.0f, -1.0f), new Vector3(0.0f, 1.0f, 0.0f));
            SetPerspectiveFOV(fov, width, height, zNear, zFar);
            BuildQuarternionBlock();
            UpdateMatrices();

        }

        /// <summary>
        /// Creates a centered perspective view with specified view parameters.
        /// </summary>
        /// <param name="fov">Field of view in radians.</param>
        /// <param name="width">Width of the camera sensor.  Mathematically is a ratio, not absolute size.</param>
        /// <param name="height">Height of the camera sensor.  Mathematically is a ratio, not absolute size.</param>
        /// <param name="zNear">Distance from the camera to begin near-field clipping in world units.</param>
        /// <param name="zFar">Distance from the camera to begin far-field clipping in world units.</param>
        /// <param name="location">Position of the camera in world units.</param>
        /// <param name="lookAt">The location vector (may be normalized to a rotation) that the camera looks in the direction of.</param>
        /// <param name="up">The direction that is considered to be up, towards the sky, or always above the camera.</param>
        public Camera(float fov, float width, float height, float zNear, float zFar, Vector3 location, Vector3 lookAt, Vector3 up)
        {
            SetView(location, lookAt, up);
            SetPerspectiveFOV(fov, width, height, zNear, zFar);
            BuildQuarternionBlock();
            UpdateMatrices();
        }

        /// <summary>
        /// Creates a camera with a specified centered view matrix type with default FOV (if applicable) and view parameters.
        /// </summary>
        /// <param name="cameraMode">The type of view matrix used by this camera.</param>
        /// <param name="width">Width of the camera sensor.  Measured in world units for orthographic and unitless for perspective.</param>
        /// <param name="height">Height of the camera sensor.  Measured in world units for orthographic and unitless for perspective.</param>
        /// <param name="zNear">Distance from the camera to begin near-field clipping in world units.</param>
        /// <param name="zFar">Distance from the camera to begin far-field clipping in world units.</param>
        public Camera(CameraMode cameraMode, float width, float height, float zNear, float zFar)  //perspective should rarely be used here since a specified FOV is much more useful.
        {
            switch(cameraMode)
            {
                case (CameraMode.ORTHOGRAPHIC):
                    SetView(DEFAULT_LOCATION, DEFAULT_LOOK, DEFAULT_UP);
                    SetOrthographic(width, height, zNear, zFar);
                    BuildQuarternionBlock();
                    UpdateMatrices();
                    return;

                case (CameraMode.PERSPECTIVE):
                    SetView(DEFAULT_LOCATION, DEFAULT_LOOK, DEFAULT_UP);
                    SetPerspectiveFOV(DEFAULT_FOV, width, height, zNear, zFar);
                    BuildQuarternionBlock();
                    UpdateMatrices();
                    return;

                default:
                    throw new Exception($"Invalid CameraMode for constructor: {cameraMode}.");
            }
        }

        /// <summary>
        /// Creates a camera with a specified centered view matrix type and view parameters with the default FOV (if applicable).
        /// </summary>
        /// <param name="cameraMode">The type of view matrix used by this camera.</param>
        /// <param name="width">Width of the camera sensor.  Measured in world units for orthographic and unitless for perspective.</param>
        /// <param name="height">Height of the camera sensor.  Measured in world units for orthographic and unitless for perspective.</param>
        /// <param name="zNear">Distance from the camera to begin near-field clipping in world units.</param>
        /// <param name="zFar">Distance from the camera to begin far-field clipping in world units.</param>
        /// <param name="location">Position of the camera in world units.</param>
        /// <param name="lookAt">The location vector (may be normalized to a rotation) that the camera looks in the direction of.</param>
        /// <param name="up">The direction that is considered to be up, towards the sky, or always above the camera.</param>
        public Camera(CameraMode cameraMode, float width, float height, float zNear, float zFar, Vector3 location, Vector3 lookAt, Vector3 up)
        {
            switch (cameraMode)
            {
                case (CameraMode.ORTHOGRAPHIC):
                    SetView(location, lookAt, up);
                    SetOrthographic(width, height, zNear, zFar);
                    BuildQuarternionBlock();
                    UpdateMatrices();
                    return;

                case (CameraMode.PERSPECTIVE):
                    SetView(location, lookAt, up);
                    SetPerspectiveFOV(DEFAULT_FOV, width, height, zNear, zFar);
                    BuildQuarternionBlock();
                    UpdateMatrices();
                    return;

                default:
                    throw new Exception($"Invalid CameraMode for constructor: {cameraMode}");
            }
        }

        /// <summary>
        /// Creates a camera with a specified off-center view matrix type and default view parameters.
        /// </summary>
        /// <param name="cameraMode">The type of view matrix used by this camera.</param>
        /// <param name="left">The distance to the left edge of the view frustrum.</param>
        /// <param name="right">The distance to the right edge of the view frustrum.</param>
        /// <param name="bottom">The distance to the bottom edge of the view frustrum.</param>
        /// <param name="top">The distance to the top edge of the view frustrum.</param>
        /// <param name="zNear">Distance from the camera to begin near-field clipping in world units.</param>
        /// <param name="zFar">Distance from the camera to begin far-field clipping in world units.</param>
        public Camera(CameraMode cameraMode, float left, float right, float bottom, float top, float zNear, float zFar)
        {
            switch(cameraMode)
            {
                case (CameraMode.ORTHOGRAPHIC_OFF_CENTER):
                    SetView(DEFAULT_LOCATION, DEFAULT_LOOK, DEFAULT_UP);
                    SetOrthographicOffCenter(left, right, bottom, top, zNear, zFar);
                    BuildQuarternionBlock();
                    UpdateMatrices();
                    return;

                case (CameraMode.PERSPECTIVE_OFF_CENTER):
                    SetView(DEFAULT_LOCATION, DEFAULT_LOOK, DEFAULT_UP);
                    SetPerspectiveOffCenter(left, right, bottom, top, zNear, zFar);
                    BuildQuarternionBlock();
                    UpdateMatrices();
                    return;

                default:
                    throw new Exception($"Invalid CameraMode for constructor: {cameraMode}.");
            }
        }

        /// <summary>
        /// Creates a camera with a specified off-center view matrix type and view parameters.
        /// </summary>
        /// <param name="cameraMode">The type of view matrix used by this camera.</param>
        /// <param name="left">The distance to the left edge of the view frustrum.</param>
        /// <param name="right">The distance to the right edge of the view frustrum.</param>
        /// <param name="bottom">The distance to the bottom edge of the view frustrum.</param>
        /// <param name="top">The distance to the top edge of the view frustrum.</param>
        /// <param name="zNear">Distance from the camera to begin near-field clipping in world units.</param>
        /// <param name="zFar">Distance from the camera to begin far-field clipping in world units.</param>
        /// <param name="location">Position of the camera in world units.</param>
        /// <param name="lookAt">The location vector (may be normalized to a rotation) that the camera looks in the direction of.</param>
        /// <param name="up">The direction that is considered to be up, towards the sky, or always above the camera.</param>
        public Camera(CameraMode cameraMode, float left, float right, float bottom, float top, float zNear, float zFar, Vector3 location, Vector3 lookAt, Vector3 up)
        {
            switch (cameraMode)
            {
                case (CameraMode.ORTHOGRAPHIC_OFF_CENTER):
                    SetView(location, lookAt, up);
                    SetOrthographicOffCenter(left, right, bottom, top, zNear, zFar);
                    BuildQuarternionBlock();
                    UpdateMatrices();
                    return;

                case (CameraMode.PERSPECTIVE_OFF_CENTER):
                    SetView(location, lookAt, up);
                    SetPerspectiveOffCenter(left, right, bottom, top, zNear, zFar);
                    BuildQuarternionBlock();
                    UpdateMatrices();
                    return;

                default:
                    throw new Exception($"Invalid CameraMode for constructor: {cameraMode}.");
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
        public void BuildUBO(int bindingLocation, int numOfVec4s)
        {
            _ubo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.UniformBuffer, _ubo);
            _vec4s = new Vector4[numOfVec4s];
            _bufferSize = numOfVec4s << 4;
            FillBuffer();
            GL.BufferData(BufferTarget.UniformBuffer, _bufferSize, _vec4s, BufferUsageHint.DynamicRead);
            GL.BindBuffer(BufferTarget.UniformBuffer, 0);
            AttachUBO(bindingLocation);

            if (GL.IsBuffer(_ubo))
                _initUBO = true;
        }

        public void AttachUBO(int bindingLocation)
        {
            _bindingLocation = bindingLocation;
            GL.BindBufferRange(BufferRangeTarget.UniformBuffer, _bindingLocation, _ubo, IntPtr.Zero, _bufferSize);
        }
        public void DetatchUBO()
        {
            GL.BindBufferRange(BufferRangeTarget.UniformBuffer, _bindingLocation, 0, IntPtr.Zero, 0);
        }

        public void UpdateUBO()
        {
            GL.BindBuffer(BufferTarget.UniformBuffer, _ubo);
            FillBuffer();
            GL.BufferSubData(BufferTarget.UniformBuffer, IntPtr.Zero, _bufferSize, _vec4s);
            GL.BindBuffer(BufferTarget.UniformBuffer, 0);
        }
        public void FillBuffer()  //Maybe: add a flag system to control what is put in the UBO
        {
            _vec4s[0] = _viewProjection.Row0;
            _vec4s[1] = _viewProjection.Row1;
            _vec4s[2] = _viewProjection.Row2;
            _vec4s[3] = _viewProjection.Row3;
            _vec4s[4] = new Vector4(_location, 1.0f);
            _vec4s[5] = new Vector4(_fov, _width, _height, _aspect);
            _vec4s[6] = new Vector4(_lookAt, 1.0f);
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

        public void AddLocation(Vector3 dX)
        {
            if (dX != Vector3.Zero)
                Location += (qY * qX * new Quaternion(dX, 1.0f) * qXi * qYi).Xyz;
        }
        public void AddLocation(Vector3 dX, float time)
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
                    if(_overrideLookAtVector)
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

                Matrix4.Mult(in _view, in _projection, out _viewProjection);

                if(_initUBO)
                    UpdateUBO();
            }
        }

        public void BuildUBO(int uboId, int uboSize, int offset, int bindingLocation, int numOfVec4s)
        {
            throw new NotImplementedException();
        }
    }
}
