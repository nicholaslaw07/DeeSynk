using OpenTK;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeeSynk.Core.Components.Types.Matrices
{
    public class PerspectiveMatrix : ProjectionMatrix
    {
        public const float FOV_DEFAULT = 1.0f;
        public const float ASPECT_DEFAULT = 1.0f;

        private float _fov, _aspect;
        public float FOV { get => _fov; set { _fov = value; _valueModified = true; } }
        public float AspectRatio { get => _aspect; set { _aspect = value; _valueModified = true; } }

        public PerspectiveMatrix()
        {
            FOV = FOV_DEFAULT;
            _aspect = ASPECT_DEFAULT;
            _zNear = ZNEAR_DEFAULT;
            _zFar = ZFAR_DEFAULT;
            Update();
        }

        public PerspectiveMatrix(float fov, float aspect, float zNear, float zFar)
        {
            FOV = fov;
            _aspect = aspect;
            _zNear = zNear;
            _zFar = zFar;
            Update();
        }

        public override void Update()
        {
            if (_valueModified)
            {
                Matrix4.CreatePerspectiveFieldOfView(_fov, _aspect, _zNear, _zFar, out _projectionMatrix);
                _valueModified = false;
            }
        }
    }
}
