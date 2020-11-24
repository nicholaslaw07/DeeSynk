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

        public CFFDictionaryIndex(in byte[] data, int startIndex, out int newStart)
        {
            newStart = startIndex;
            ParseHeader(in data, startIndex, out startIndex);
            if (_count > 0)
            {
                ParseOffsets(in data, startIndex, out startIndex);
                ParseData(in data, startIndex, out newStart);
            }
            else if (_count == 0)
                newStart += 2;
        }

        private void ParseHeader(in byte[] data, int startIndex, out int newStart)
        {
            newStart = startIndex;
            _count = DataHelper.GetAtLocationShort(in data, startIndex, 2);
            _offset = (_count > 0) ? data[newStart += 2] : _offset = 0;
            _offsets = new int[_count + ((_count == 0) ? 0 : 1)];
            _offsets = new int[_count + 1];
            _offsetGaps = new int[_count];
            _data = new CFFDictionary[_count];
            newStart += 1;
        }

        private void ParseOffsets(in byte[] data, int startIndex, out int newStart)
        {
            newStart = startIndex - _offset;
            _offsets[0] = DataHelper.GetAtLocationInt(in data, newStart += _offset, _offset) - 1;
            for (int idx = 0; idx < _offsets.Length - 1; idx++)
                _offsetGaps[idx] = (_offsets[idx + 1] = DataHelper.GetAtLocationInt(in data, newStart += _offset, _offset) - 1) - _offsets[idx];
            newStart += _offset;
        }

        private void ParseData(in byte[] data, int startIndex, out int newStart)
        {
            newStart = startIndex;
            for (int idx = 0; idx < _data.Length; idx++)
                _data[idx] = new CFFDictionary(in data, startIndex, _offsetGaps[idx], out newStart);
        }
    }
}
