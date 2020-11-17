using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;

namespace DeeSynk.Core.Components.Types.Matrices
{
    public abstract class ProjectionMatrix
    {
        public const float ZNEAR_DEFAULT = 1.0f;
        public const float ZFAR_DEFAULT = 10.0f;

        protected bool _valueModified;
        /// <summary>
        /// Flips on whenever a value for the matrix (not the matrix itself) is modified
        /// </summary>
        public bool ValueModified { get => _valueModified; }

        protected Matrix4 _projectionMatrix;
        public Matrix4 Matrix {get => _projectionMatrix; }
        public ref Matrix4 MatrixRef { get => ref _projectionMatrix; }

        protected float _zNear, _zFar;
        /// <summary>
        /// The nearest clipping boundary of the rendering region
        /// </summary>
        public float ZNear { get => _zNear; set { _zNear = value; _valueModified = true; } }
        /// <summary>
        /// The furthest clipping boundary of the rendering region
        /// </summary>
        public float ZFar { get => _zFar; set { _zFar = value; _valueModified = true; } }
        /// <summary>
        /// Updates the matrix values if there has been any modifications to the data
        /// </summary>
        public abstract void Update();
    }
}
