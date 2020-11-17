using System;
using System.Collections.Generic;
using System.Text;

namespace DeeSynk.Core.Components.Types.Fonts
{
    public struct FileHeaderEntry
    {
        int _table;
        public int Table { get => _table; }

        int _checkSum;
        public int CheckSum { get => _checkSum; }

        int _offset;
        public int Offset { get => _offset; }

        int _size;
        public int Size { get => _size; }

        public FileHeaderEntry(int table, int checkSum, int offset, int size)
        {
            _table = table;
            _checkSum = checkSum;
            _offset = offset;
            _size = size;
        }
    }
}
