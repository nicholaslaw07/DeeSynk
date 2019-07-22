using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeeSynk.Core.Components.Types.Matrices
{
    public class ViewMatrix
    {
        private bool _valueModified;
        /// <summary>
        /// Flips on whenever a value for the matrix (not the matrix itself) is modified
        /// </summary>
        public bool ValueModified { get => _valueModified; }

        private Matrix4 _viewMatrix;
        /// <summary>
        /// Transformation to the viewer space
        /// </summary>
        public Matrix4 Matrix { get => _viewMatrix; }
        public ref Matrix4 MatrixRef { get => ref _viewMatrix; }

        private Vector3 _location, _lookAt, _up;
        /// <summary>
        /// Location of the viewer (eye)
        /// </summary>
        public Vector3 Location { get => _location; set { _location = value; _valueModified = true; } }
        /// <summary>
        /// Direction, location, or target that the viewer is facing
        /// </summary>
        public Vector3 LookAt   { get => _lookAt; set { _lookAt = value; _valueModified = true; } }
        /// <summary>
        /// Vector pointing in the up direction for the viewer
        /// </summary>
        public Vector3 Up       { get => _up; set { _up = value; _valueModified = true; } }

        public ViewMatrix()
        {
            Location = Vector3.Zero;
            LookAt = new Vector3(0.0f, 0.0f, 1.0f);
            Up = new Vector3(0.0f, 1.0f, 0.0f);
            Update();
        }

        public ViewMatrix(Vector3 location, Vector3 lookAt, Vector3 up)
        {
            Location = location;
            LookAt = lookAt;
            Up = up;
            Update();
        }

        public void Update()
        {
            if (_valueModified)
            {
                _viewMatrix = Matrix4.LookAt(_location, _lookAt, _up);
                _valueModified = false;
            }
        }

        public static void GetViewProjectionProduct(ViewMatrix view, ProjectionMatrix projection, out Matrix4 product)
        {
            Matrix4.Mult(ref view.MatrixRef, ref projection.MatrixRef, out product);
        }
    }
}
