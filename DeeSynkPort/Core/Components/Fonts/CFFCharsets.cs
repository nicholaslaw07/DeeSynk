using System;
using System.Collections.Generic;
using System.Text;

namespace DeeSynk.Core.Components.Fonts
{
    public class CFFCharsets: List<short>
    {
        private byte _format;
        public byte Format { get => _format; }
        public CFFCharsets(byte format) : base()
        {
            _format = format;
        }
    }
}
