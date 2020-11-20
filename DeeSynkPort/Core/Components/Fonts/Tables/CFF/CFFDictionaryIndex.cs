using DeeSynk.Core.Components.Types.Fonts;
using System;
using System.Collections.Generic;
using System.Text;

namespace DeeSynk.Core.Components.Fonts.Tables.CFF
{
    public class CFFDictionaryIndex
    {
        private short _count;
        public short Count { get => _count; set => _count = value; }

        private byte _offset;
        public byte Offset { get => _offset; set => _offset = value; }

        private int[] _offsets;
        public ref int[] OffsetsRef { get => ref _offsets; }
        public int[] Offsets { get => _offsets; }

        private int[] _offsetGaps;
        public ref int[] OffsetGapsRef { get => ref _offsetGaps; }
        public int[] OffsetGaps { get => _offsetGaps; }

        private CFFDictionary[] _data;
        public CFFDictionary[] Data { get => _data; set => _data = value; }

        public CFFDictionaryIndex(short count, byte offset)
        {
            _count = count;
            _offset = offset;
            _offsets = new int[_count + 1];
            _offsetGaps = new int[_count];
            _data = new CFFDictionary[_count];
        }
    }
}
