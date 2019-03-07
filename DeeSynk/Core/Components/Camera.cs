using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeeSynk.Core.Components
{
    public class Camera
    {
        private Matrix4 _view;
        public ref Matrix4 View { get => ref _view; }

        private bool _viewUpdate;

        private Vector3 _location, _lookAt, _up;
        public Vector3 Location { get => _location; set { _location = value; _viewUpdate = true; } }
        public Vector3 LookAt { get => _lookAt; set { _lookAt = value; _viewUpdate = true; } }
        public Vector3 Up { get => _up; set { _up = value; _viewUpdate = true; } }

        private float _rotX, _rotY;
        public float CameraRotationX { get => _rotX; set { _rotX = value; _viewUpdate = true; } }
        public float CameraRotationY { get => _rotY; set { _rotY = value; _viewUpdate = true; } }

        private Quaternion qX, qY, p, qXi, qYi;

        private Matrix4 _projection;
        public ref Matrix4 Projection { get => ref _projection; }

        private bool _projectionUpdate;

        private float _fov;
        public float FOV { get => _fov; set { _fov = value; _projectionUpdate = true; } }

        private float _width, _height;
        public float Width { get => _width; set { _width = value; _projectionUpdate = true; } }
        public float Height { get => _height; set { _height = value; _projectionUpdate = true; } }

        private float _zNear, _zFar;
        public float ZNear { get => _zNear; set { _zNear = value; _projectionUpdate = true; } }
        public float ZFar { get => _zFar; set { _zFar = value; _projectionUpdate = true; } }

        private Matrix4 _viewProjection;
        public ref Matrix4 ViewProjection { get => ref _viewProjection; }

        public Camera()
        {
            _fov = 0.0f;
            _width = 0.0f;
            _height = 0.0f;
            _zNear = 0.0f;
            _zFar = 0.0f;

            _location = Vector3.Zero;
            _lookAt = Vector3.Zero;
            _up = Vector3.Zero;

            _rotX = 0.0f;
            _rotY = 0.0f;

            _view = Matrix4.Identity;
            _projection = Matrix4.Identity;

            _viewProjection = Matrix4.Identity;

            qX = new Quaternion();
            qY = new Quaternion();
            p = new Quaternion(0.0f, 0.0f, -1.0f, 1.0f);
            qYi = new Quaternion();
            qXi = new Quaternion();
        }

        public Camera(float fov, float width, float height, float zNear, float zFar)
        {
            _fov = fov;
            _width = width;
            _height = height;
            _zNear = zNear;
            _zFar = zFar;

            _location = Vector3.Zero;
            _lookAt = new Vector3(0.0f, 0.0f, -1.0f);
            _up = new Vector3(0.0f, 1.0f, 0.0f);

            _rotX = 0.0f;
            _rotY = 0.0f;

            _view = Matrix4.LookAt(_location, _lookAt, _up);
            _projection = Matrix4.CreatePerspectiveFieldOfView(_fov, _width / _height, _zNear, _zFar);

            _viewProjection = _view * _projection;

            qX = new Quaternion();
            qY = new Quaternion();
            p = new Quaternion(0.0f, 0.0f, -1.0f, 1.0f);
            qYi = new Quaternion();
            qXi = new Quaternion();
        }

        public Camera(float fov, float width, float height, float zNear, float zFar, float homeX, float homeY)
        {
            _fov = fov;
            _width = width;
            _height = height;
            _zNear = zNear;
            _zFar = zFar;

            _location = Vector3.Zero;
            _lookAt = new Vector3(0.0f, 0.0f, -1.0f);
            _up = new Vector3(0.0f, 1.0f, 0.0f);

            _rotX = 0.0f;
            _rotY = 0.0f;

            _view = Matrix4.LookAt(_location, _lookAt, _up);
            _projection = Matrix4.CreatePerspectiveFieldOfView(_fov, _width / _height, _zNear, _zFar);

            _viewProjection = _view * _projection;

            qX = new Quaternion();
            qY = new Quaternion();
            p = new Quaternion(0.0f, 0.0f, -1.0f, 1.0f);
            qYi = new Quaternion();
            qXi = new Quaternion();
        }

        public Camera(float fov, float width, float height, float zNear, float zFar, float homeX, float homeY, ref Vector3 location, ref Vector3 lookAt, ref Vector3 up)
        {
            _fov = fov;
            _width = width;
            _height = height;
            _zNear = zNear;
            _zFar = zFar;

            _location = location;
            _lookAt = lookAt;
            _up = up;

            _rotX = 0.0f;
            _rotY = 0.0f;

            _view = Matrix4.LookAt(_location, _lookAt, _up);
            _projection = Matrix4.CreatePerspectiveFieldOfView(_fov, _width / _height, _zNear, _zFar);

            _viewProjection = _view * _projection;

            qX = new Quaternion();
            qY = new Quaternion();
            p = new Quaternion(0.0f, 0.0f, -1.0f, 1.0f);
            qYi = new Quaternion();
            qXi = new Quaternion();
        }

        public void AddLocation(float dx, float dy, float dz)
        {
            if(dx != 0 || dy != 0 || dz != 0)
            {
                Location += (qY * qX * new Quaternion(dx, dy, dz, 1.0f) * qXi * qYi).Xyz;
            }
        }

        public void AddLocation(ref Vector3 dX)
        {
            if (dX != Vector3.Zero)
            {
                Location += (qY * qX * new Quaternion(dX, 1.0f) * qXi * qYi).Xyz;
            }
        }

        public void AddLocation(ref Vector3 dX, float time)
        {
            if (dX != Vector3.Zero)
            {
                Location += (qY * qX * new Quaternion(dX * time, 1.0f) * qXi * qYi).Xyz;
            }
        }

        public void AddRotation(float rx, float ry)
        {
            if (rx != 0 || ry != 0)
            {
                if (rx != 0)
                {
                    CameraRotationX += rx;
                }
                if (ry != 0)
                {
                    CameraRotationY += ry;
                }
                qX = new Quaternion(_rotX, 0.0f, 0.0f);
                qY = new Quaternion(0.0f, _rotY, 0.0f);
                qXi = qX.Inverted();
                qYi = qY.Inverted();
            }
        }

        public void AddRotation()
        {
            if (_rotX > 1.57f)
                _rotX = 1.57f;
            else if (_rotX < -1.57f)
                _rotX = -1.57f;

            qX = new Quaternion(_rotX, 0.0f, 0.0f);
            qY = new Quaternion(0.0f, _rotY, 0.0f);
            qXi = qX.Inverted();
            qYi = qY.Inverted();
        }

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
                    _projection = Matrix4.CreatePerspectiveFieldOfView(_fov, _width / _height, _zNear, _zFar);
                    _projectionUpdate = false;
                }

                _viewProjection = _view * _projection;
            }
        }
    }
}
