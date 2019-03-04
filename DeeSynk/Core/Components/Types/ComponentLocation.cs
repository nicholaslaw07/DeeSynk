using OpenTK;

namespace DeeSynk.Core.Components.Types
{
    public class ComponentLocation : IComponent
    {
        public Component BitMaskID => Component.LOCATION;
        
        //ADD VALUE UPDATED BOOL

        private Vector4 _location;
        public Vector4 Location { get => _location; set => _location = value; }

        public ComponentLocation()
        {
            _location = new Vector4();
        }

        public ComponentLocation(float X, float Y, float Z, float W)
        {
            _location = new Vector4(X, Y, Z, W);
        }
        public ComponentLocation(float X, float Y, float Z)
        {
            _location = new Vector4(X, Y, Z, 1.0f);
        }
        public ComponentLocation(float X, float Y)
        {
            _location = new Vector4(X, Y, 1.0f, 1.0f);
        }

        public ComponentLocation(ref Vector4 l)
        {
            _location = new Vector4(l.X, l.Y, l.Z, l.W);
        }
        public ComponentLocation(ref Vector3 l)
        {
            _location = new Vector4(l.X, l.Y, l.Z, 1.0f);
        }
        public ComponentLocation(ref Vector2 l)
        {
            _location = new Vector4(l.X, l.Y, 1.0f, 1.0f);
        }

        public void Update(float time)
        {
            throw new System.NotImplementedException();
        }
    }
}
