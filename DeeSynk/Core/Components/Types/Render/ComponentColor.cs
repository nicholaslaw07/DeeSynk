using OpenTK.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeeSynk.Core.Components.Types.Render
{
    public class ComponentColor : IComponent
    {
        public int BitMaskID => (int)Component.COLOR;

        private bool _hasColorData;
        public bool HasColorData { get => _hasColorData; }

        private Color4[] _colors;  //should match the same number of vertices
        public Color4[] Colors { get => _colors; }
        public ref Color4[] ColorsByRef { get => ref _colors; }

        private int _colorCount;
        public int ColorCount { get => _colorCount; }

        public ComponentColor()
        {
            _hasColorData = false;
            _colorCount = 0;
        }

        public ComponentColor(Color4[] colors)
        {
            _colors = colors;
            _colorCount = _colors.Length;
        }

        public void Update(float time)
        {
            //throw new NotImplementedException();
        }
    }
}
