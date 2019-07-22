using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeeSynk.Core.Components.Types.Matrices
{
    public class OrthographicMatrix : ProjectionMatrix
    {
        public const float WIDTH_DEFAULT = 1.0f;
        public const float HEIGHT_DEFAULT = 1.0f;

        private float _width, _height;
        public float Width { get => _width; set { _width = value; _valueModified = true; } }
        public float Height { get => _height; set { _height = value; _valueModified = true; } }

        public OrthographicMatrix()
        {
            Width = WIDTH_DEFAULT;
            _height = HEIGHT_DEFAULT;
            _zNear = ZNEAR_DEFAULT;
            _zFar = ZFAR_DEFAULT;
            Update();
        }

        public OrthographicMatrix(float width, float height, float zNear, float zFar)
        {
            Width = width;
            _height = height;
            _zNear = zNear;
            _zFar = zFar;
            Update();
        }
        public override void Update()
        {
            if (_valueModified)
            {
                Matrix4.CreateOrthographic(_width, _height, _zNear, _zFar, out _projectionMatrix);
                _valueModified = false;
            }
        }
    }
}
