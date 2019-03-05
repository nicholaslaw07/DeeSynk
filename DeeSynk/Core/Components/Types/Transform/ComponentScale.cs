using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeeSynk.Core.Components.Types.Transform
{
    public class ComponentScale : IComponent
    {
        public int BitMaskID => (int)Component.SCALE;

        private bool _valueUpdated;
        public bool ValueUpdated { get => _valueUpdated; }

        private Vector3 _scale;
        public Vector3 Scale { get => _scale; set => _scale = value; }

        //DEFAULT CONSTRUCTOR
        public ComponentScale()
        {
            _valueUpdated = false;
            _scale = new Vector3(1.0f, 1.0f, 1.0f);
        }

        public ComponentScale(ref Vector3 scale)
        {
            _valueUpdated = true;
            _scale = scale;
        }

        public ComponentScale(float scale, bool scaleOnZAxis)
        {
            if (scaleOnZAxis)
            {
                _valueUpdated = true;
                _scale = new Vector3(scale, scale, scale);
            }
            else
            {
                _valueUpdated = true;
                _scale = new Vector3(scale, scale, 1.0f);
            }
        }

        public ComponentScale(float scaleX, float scaleY)
        {
            _valueUpdated = true;
            _scale = new Vector3(scaleX, scaleY, 1.0f);
        }

        public ComponentScale(float scaleX, float scaleY, float scaleZ)
        {
            _valueUpdated = true;
            _scale = new Vector3(scaleX, scaleY, scaleZ);
        }

        public ref Vector3 GetScaleByRef()
        {
            return ref _scale;
        }

        public void Update(float time)
        {
            _valueUpdated = false;
        }
    }
}
