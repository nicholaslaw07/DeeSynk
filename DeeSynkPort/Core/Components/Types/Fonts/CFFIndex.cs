using System;
using System.Collections.Generic;
using System.Text;

namespace DeeSynk.Core.Components.Types.Fonts
{
    public class CFFIndex
    {
        private short _count;
        public short Count { get => _count; set => _count = value; }

        private byte _offset;
        public byte offset { get => _offset; set => _offset = value; }

        private int[] _offsets;
        public int[] Offsets
        {
            get => _offsets;
            set
            {
                if (value.Length == _count + 1)
                    _offsets = value;
                else
                    throw new ArgumentException("Invalid number of offset values.  Number of offsets must be one more than the number of objects.");
            }
        }

        private byte[] _data;
        public byte[] Data { get => _data; set => _data = value; }

        public CFFIndex(short count, byte offset)
        {
            _count = count;
            _offset = offset;
            _offsets = new int[count + 1];
        }
    }
}
