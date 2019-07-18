using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeeSynk.Core.Components.Types.Matrices
{
    public class PerspectiveMatrix : ProjectionMatrix
    {
        private float _fov, _aspect;
        public float FOV { get => _fov; set { _fov = value; _updated = true; } }
        public float AspectRatio { get => _aspect; set { _aspect = value; _updated = true; } }

        public PerspectiveMatrix()
        {
            _fov = 1.0f;
            _aspect = 1.0f;
        }

        public override void Update()
        {
            throw new NotImplementedException();
        }
    }
}
