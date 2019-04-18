using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeeSynk.Core.Components.Types.Render
{
    public class ComponentCamera : IComponent
    {
        public Component BitMaskID => Component.CAMERA;

        private Camera _camera;
        public Camera Camera { get => _camera; }

        public ComponentCamera(Camera camera)
        {
            _camera = camera;
        }
    }
}
