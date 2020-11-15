using System;
using System.Collections.Generic;
using System.Text;

namespace DeeSynk.Core.Components.Types.Fonts
{
    public struct CFFHeader
    {
        byte _versionMajor; //0-255
        public byte VersionMajor { get => _versionMajor; }

        byte _versionMinor; //0-255
        public byte VersionMinor { get => _versionMinor; }

        byte _headerSize; //0-255
        public byte HeaderSize { get => _headerSize; }

        byte _offset; //1-4
        public byte Offset { get => _offset; }

        public CFFHeader(byte versionMajor, byte versionMinor, byte headerSize, byte offset)
        {
            _versionMajor = versionMajor;
            _versionMinor = versionMinor;
            _headerSize = headerSize;
            _offset = offset;
        }
    }
    public class CFFTable
    {
        private CFFHeader _cffHeader;
        public CFFHeader CFFHeader { get => _cffHeader; set => _cffHeader = value; }

        private CFFIndex _indexName;
        public CFFIndex IndexName { get => _indexName; set => _indexName = value; }

        public CFFTable() { }
    }
}
