using OpenTK;

namespace DeeSynk.Core.Components.Types
{
    public class ComponentLocation : IComponent
    {
        public Component BitMaskID => Component.LOCATION;

        private Vector4 _location;
        public Vector4 Location { get => _location; set => _location = value; }
    }
}
