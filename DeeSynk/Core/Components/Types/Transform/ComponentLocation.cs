using OpenTK;

namespace DeeSynk.Core.Components.Types.Transform
{
    public class ComponentLocation : IComponent
    {
        public int BitMaskID => (int)Component.LOCATION;

        //ADD VALUE UPDATED BOOL

        private bool _valueUpdated;
        public bool ValueUpdated { get => _valueUpdated; }

        public bool SetValueUpdateComplete { set => _valueUpdated = false; }

        private Vector4 _location;
        public Vector4 Location
        {
            get => _location;
            set
            {
                _location = value;
                if (!_valueUpdated)
                    _valueUpdated = true;
            }
        }

        public ComponentLocation()
        {
            _valueUpdated = false;
            _location = new Vector4();
        }

        public ComponentLocation(float X, float Y, float Z, float W)
        {
            _valueUpdated = true;
            _location = new Vector4(X, Y, Z, W);
        }
        public ComponentLocation(float X, float Y, float Z)
        {
            _valueUpdated = true;
            _location = new Vector4(X, Y, Z, 1.0f);
        }
        public ComponentLocation(float X, float Y)
        {
            _valueUpdated = true;
            _location = new Vector4(X, Y, 1.0f, 1.0f);
        }

        public ComponentLocation(ref Vector4 l)
        {
            _valueUpdated = true;
            _location = new Vector4(l.X, l.Y, l.Z, l.W);
        }
        public ComponentLocation(ref Vector3 l)
        {
            _valueUpdated = true;
            _location = new Vector4(l.X, l.Y, l.Z, 1.0f);
        }
        public ComponentLocation(ref Vector2 l)
        {
            _valueUpdated = true;
            _location = new Vector4(l.X, l.Y, 1.0f, 1.0f);
        }

        public void Update(float time)
        {
            throw new System.NotImplementedException();
        }
    }
}
