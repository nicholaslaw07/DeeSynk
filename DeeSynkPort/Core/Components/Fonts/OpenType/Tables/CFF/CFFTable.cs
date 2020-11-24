using DeeSynk.Core.Components.Fonts;
using DeeSynk.Core.Components.Fonts.Tables.CFF;
using DeeSynk.Core.Components.Types.Fonts;
using System;
using System.Collections.Generic;
using System.Text;

namespace DeeSynk.Core.Components.Fonts.Tables.CFF
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
        public CFFHeader Header { get => _header; }

        private CFFIndex _indexName;
        public CFFIndex IndexName { get => _indexName; }

        private CFFDictionaryIndex _topDictIndex;
        public CFFDictionaryIndex TopDictionaryIndex { get => _topDictIndex; }

        private CFFIndex _indexString;
        public CFFIndex IndexString { get => _indexString; }

        private CFFIndex _indexGlobalSubr;
        public CFFIndex IndexGlobalSubr { get => _indexGlobalSubr; }

        private CFFIndex _indexLocalSubr;
        public CFFIndex IndexLocalSubr { get => _indexLocalSubr; }

        private CFFIndex _indexCharStrings;
        public CFFIndex IndexCharStrings { get => _indexCharStrings; }

        private CFFCharStringCommands[] _charStringCommands;
        public CFFCharStringCommands[] CharStringCommands { get => _charStringCommands; }

        private CFFCharStringCommands[] _globalSubrCommands;
        public CFFCharStringCommands[] GlobalSubrCommands { get => _globalSubrCommands; }

        private CFFCharStringCommands[] _localSubrCommands;
        public CFFCharStringCommands[] LocalSubrCommands { get => _localSubrCommands; }

        private CFFDictionary _privateDict;
        public CFFDictionary PrivateDictionary { get => _privateDict; }

        private CFFCharsets _charsets;
        public CFFCharsets Charsets { get => _charsets; }

        public CFFTable(in byte[] data, FileHeaderEntry entry)
        {
            int startIndex = entry.Offset;
            _startIndex = startIndex;
            int newStart = startIndex;
            _header = new CFFHeader(data[startIndex + 0], data[startIndex + 1], data[startIndex + 2], data[startIndex + 3]);
            startIndex += Header.HeaderSize;
            //NAME Index  //Should be limited to 127 characters with no special characteres (33- 126)
            _indexName = new CFFIndex(in data, startIndex, out startIndex);
            _topDictIndex = new CFFDictionaryIndex(in data, startIndex, out startIndex);
            _indexString = new CFFIndex(in data, startIndex, out startIndex);  //values are accessed at index of idx+390
            _indexGlobalSubr = new CFFIndex(in data, startIndex, out startIndex);
            if (!IndexGlobalSubr.IsBlank)
                _globalSubrCommands = CFFCharStringCommands.ParseCharStrings(_indexGlobalSubr, true, true);

            if (_topDictIndex.Data[0].TryGetValue(Operators.CharStrings, out Operand[] charStringIdx))
            {
                if (charStringIdx.Length == 1)
                    _indexCharStrings = new CFFIndex(in data, StartIndex + charStringIdx[0].IntegerValue);
            }

            _charStringCommands = CFFCharStringCommands.ParseCharStrings(_indexCharStrings, false, false);

            if (TopDictionaryIndex.Data[0].TryGetValue(Operators.Private, out Operand[] privateOperands))
            {
                if (privateOperands.Length == 2)
                {
                    _privateDict = new CFFDictionary(in data, StartIndex + privateOperands[1].IntegerValue, privateOperands[0].IntegerValue);
                    if(PrivateDictionary.TryGetValue(Operators.Subrs, out Operand[] subrsOperands))
                    {
                        if(subrsOperands.Length == 1)
                        {
                            _indexLocalSubr = new CFFIndex(in data, StartIndex + privateOperands[1].IntegerValue + subrsOperands[0].IntegerValue);
                            _localSubrCommands  = CFFCharStringCommands.ParseCharStrings(_indexLocalSubr, true, true);
                        }
                    }
                }
            }
            if (TopDictionaryIndex.Data[0].TryGetValue(Operators.charset, out Operand[] charsetOperands))
            {
                if (charsetOperands.Length == 1)
                    _charsets = new CFFCharsets(in data, _charStringCommands.Length, StartIndex + charsetOperands[0].IntegerValue, out newStart);
            }
            var value = IndexCharStrings.GetDataAtIndex(1);
            int x = 1;
        }
    }
}
