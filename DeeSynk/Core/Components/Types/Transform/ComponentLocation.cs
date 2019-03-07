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

        private Vector3 _location;
        public Vector3 Location
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
            _location = new Vector3();
        }

        public ComponentLocation(float X, float Y, float Z)
        {
            _valueUpdated = true;
            _location = new Vector3(X, Y, Z);
        }
        public ComponentLocation(float X, float Y)
        {
            _valueUpdated = true;
            _location = new Vector3(X, Y, 1.0f);
        }

        public ComponentLocation(ref Vector3 l)
        {
            _valueUpdated = true;
            _location = new Vector3(l.X, l.Y, l.Z);
        }
        public ComponentLocation(ref Vector2 l)
        {
            _valueUpdated = true;
            _location = new Vector3(l.X, l.Y, 1.0f);
        }

        public ref Vector3 GetLocationByRef()
        {
            return ref _location;
        }

        public void Update(float time)
        {
        }

        public void CompleteUpdate()
        {
            _valueUpdated = false;
        }
    }
}
