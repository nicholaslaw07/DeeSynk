using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace DeeSynk.Core.Components.Types.Transform
{
    public class ComponentVelocity : IComponent
    {
        public int BitMaskID => (int)Component.VELOCITY;

        private bool _valueUpdated;
        public bool ValueUpdated { get => _valueUpdated; }

        public bool SetValueUpdateComplete { set => _valueUpdated = false; }

        private Vector4 _velocity;
        public Vector4 Velocity
        {
            get => _velocity;
            set
            {
                _velocity = value;
                if (!_valueUpdated)
                    _valueUpdated = true;
            }
        }

        public ComponentVelocity()  //default constructor
        {
            _velocity = new Vector4();
        }

        public ComponentVelocity(float vX, float vY, float vZ, float vW)
        {
            _velocity = new Vector4(vX, vY, vZ, vW);
        }  // 4 float components (xyzw)
        public ComponentVelocity(float vX, float vY, float vZ)
        {
            _velocity = new Vector4(vX, vY, vZ, 0.0f);
        }            // 3 float components (xyz)
        public ComponentVelocity(float vX, float vY)
        {
            _velocity = new Vector4(vX, vY, 0.0f, 0.0f);
        }                      // 2 float components (xy)

        public ComponentVelocity(ref Vector4 v)
        {
            _velocity = new Vector4(v.X, v.Y, v.Z, v.W);
        }   //same but use an actual vector struct
        public ComponentVelocity(ref Vector3 v)
        {
            _velocity = new Vector4(v.X, v.Y, v.Z, 0.0f);
        }   
        public ComponentVelocity(ref Vector2 v)
        {
            _velocity = new Vector4(v.X, v.Y, 0.0f, 0.0f);
        }

        public void Update(float time)
        {
            _valueUpdated = false;
        }
    }
}
