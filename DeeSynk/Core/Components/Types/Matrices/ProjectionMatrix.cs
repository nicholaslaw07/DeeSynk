using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeeSynk.Core.Components.Types.Matrices
{
    public abstract class ProjectionMatrix
    {
        protected bool _updated;
        /// <summary>
        /// Flips on whenever a value for the matrix (not the matrix itself) is modified
        /// </summary>
        public bool Updated { get => _updated; }

        protected Matrix4 _projectionMatrix;
        public Matrix4 Matrix {get => _projectionMatrix; }

        private float _zNear, _zFar;
        /// <summary>
        /// The nearest clipping boundary of the rendering region
        /// </summary>
        public float ZNear { get => _zNear; set { _zNear = value; _updated = true; } }
        /// <summary>
        /// The furthest clipping boundary of the rendering region
        /// </summary>
        public float ZFar { get => _zFar; set { _zFar = value; _updated = true; } }

        public abstract void Update();
    }
}
