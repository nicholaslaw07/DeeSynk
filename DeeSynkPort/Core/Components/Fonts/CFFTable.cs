using DeeSynk.Core.Components.Fonts;
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
        private int _startIndex;
        public int StartIndex { get => _startIndex; }

        private CFFHeader _header;
        public CFFHeader Header { get => _header; set => _header = value; }

        private CFFIndex _indexName;
        public CFFIndex IndexName { get => _indexName; set => _indexName = value; }

        private CFFDictionaryIndex _topDictIndex;
        public CFFDictionaryIndex TopDictionaryIndex { get => _topDictIndex; set => _topDictIndex = value; }

        private CFFIndex _indexString;
        public CFFIndex IndexString { get => _indexString; set => _indexString = value; }

        private CFFIndex _indexGlobalSubr;
        public CFFIndex IndexGlobalSubr { get => _indexGlobalSubr; set => _indexGlobalSubr = value; }

        private CFFIndex _indexLocalSubr;
        public CFFIndex IndexLocalSubr { get => _indexLocalSubr; set => _indexLocalSubr = value; }

        private CFFIndex _indexCharStrings;
        public CFFIndex IndexCharStrings { get => _indexCharStrings; set => _indexCharStrings = value; }

        private CFFCharStringCommands[] _charStringCommands;
        public CFFCharStringCommands[] CharStringCommands { get => _charStringCommands; set => _charStringCommands = value; }

        private CFFCharStringCommands[] _localSubrCommands;
        public CFFCharStringCommands[] LocalSubrCommands { get => _localSubrCommands; set => _localSubrCommands = value; }

        private CFFDictionary _privateDict;
        public CFFDictionary PrivateDictionary { get => _privateDict; set => _privateDict = value; }

        private CFFCharsets _charsets;
        public CFFCharsets Charsets { get => _charsets; set => _charsets = value; }

        public CFFTable(int startIndex) { _startIndex = startIndex; }
    }
}
