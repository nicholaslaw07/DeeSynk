using System;
using System.Collections.Generic;
using System.Text;

namespace DeeSynk.Core.Components.Fonts.Tables.CFF
{
    /*set
{
    if (value.Length == _count + 1)
        _offsets = value;
    else
        throw new ArgumentException("Invalid number of offset values.  Number of offsets must be one more than the number of objects.");
}*/
    public class CFFIndex
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

        private byte[] _data;
        public ref byte[] DataRef { get => ref _data; }
        public byte[] Data { get => _data; set => _data = value; }

        public int DataSize { get => (IsBlank) ? 0 : _offsets[_offsets.Length - 1] - _offsets[0]; }

        public bool IsBlank { get => _count == 0 && _offset == 0 && _offsets.Length == 0 && _offsetGaps.Length == 0; }

        public CFFIndex(in byte[] data, int startIndex, out int newStart)
        {
            newStart = startIndex;
            ParseHeader(in data, startIndex, out startIndex);
            if(_count > 0)
            {
                ParseOffsets(in data, startIndex, out startIndex);
                ParseData(in data, startIndex, out newStart);
            }
            else if(_count == 0)
            {
                newStart += 2;
                _data = new byte[0];
            }
        }

        public CFFIndex(in byte[] data, int startIndex)
        {
            ParseHeader(in data, startIndex, out startIndex);
            if (_count > 0)
            {
                ParseOffsets(in data, startIndex, out startIndex);
                ParseData(in data, startIndex, out startIndex);
            }
            else
                _data = new byte[0];
        }

        private void ParseHeader(in byte[] data, int startIndex, out int newStart)
        {
            newStart = startIndex;
            _count = DataHelper.GetAtLocationShort(in data, startIndex, 2);
            _offset = (_count > 0) ? data[newStart += 2] : _offset = 0;
            _offsets = new int[_count + ((_count == 0) ? 0 : 1)];
            _offsets = new int[_count + 1];
            _offsetGaps = new int[_count];
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
            _data = new byte[DataSize];
            for (int idx = 0; idx < _data.Length; idx++)
                _data[idx] = data[startIndex + idx];
            newStart = startIndex + DataSize;
        }

        public Span<byte> GetDataSpanAtIndex(int offsetIndex)
        {
            return new Span<byte>(_data, _offsets[offsetIndex], _offsets[offsetIndex + 1] - _offsets[offsetIndex]);
        }

        public void GetDataAtIndex(int dataIndex, out byte[] subset)
        {
            int startIndex = _offsets[dataIndex];
            int length = _offsetGaps[dataIndex];
            subset = new byte[length];
            for (int idx = 0; idx < length; idx++)
                subset[idx] = _data[startIndex + idx];
        }

        public byte[] GetDataAtIndex(int dataIndex)
        {
            int startIndex = _offsets[dataIndex];
            int length = _offsetGaps[dataIndex];
            byte[] subset = new byte[length];
            for (int idx = 0; idx < length; idx++)
                subset[idx] = _data[startIndex + idx];
            return subset;
        }
    }
}
