using System;
using System.Collections.Generic;
using System.Text;

namespace DeeSynk.Core.Components.Types.Fonts
{
    public struct CFFHeader
    {
        byte _versionMajor; //0-255
        public byte VersionMajor { get => _versionMajor; set => _versionMajor = value; }

        byte _versionMinor; //0-255
        public byte VersionMinor { get => _versionMinor; set => _versionMinor = value; }

        byte _headerSize; //0-255
        public byte HeaderSize { get => _headerSize; set => _headerSize = value; }

        byte _offset; //1-4
        public byte Offset { get => _offset; set => _offset = value; }
    }
    public class CFFTable
    {
        private CFFHeader _cffHeader;
        public CFFHeader CFFHeader { get => _cffHeader; set => _cffHeader = value; }


        public CFFTable() { }
    }
}
