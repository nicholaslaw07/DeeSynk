using DeeSynk.Core.Components.Types.Fonts;
using System;
using System.Collections.Generic;
using System.Text;

namespace DeeSynk.Core.Components.Fonts.Tables
{
    //https://docs.microsoft.com/en-us/typography/opentype/spec/os2#achvendid
    public class OS2
    {
        public const int VERSION_0 = 0x0000;
        public const int VERSION_1 = 0x0001;
        public const int VERSION_2 = 0x0002;
        public const int VERSION_3 = 0x0003;
        public const int VERSION_4 = 0x0004;
        public const int VERSION_5 = 0x0005;

        private int _version;

        private short _xAvgCharWidth;

        private int _usWeightClass, _usWidthClass;

        private int _fsType;

        private short _ySubscriptXSize, _ySubscriptYSize, _ySubscriptXOffset, _ySubscriptYOffset, _ySuperscriptXSize, _ySuperscriptYSize, _ySuperscriptXOffset, _ySuperscriptYOffset, _yStrikeoutSize, _yStrikeoutPosition;

        private short _sFamilyClass;

        private short[] _panose; //must be 10 values long (4xuint32) for v5

        private long _ulUnicodeRange1, _ulUnicodeRange2, _ulUnicodeRange3, _ulUnicodeRange4;

        private short[] _achVendID; //must be 4 long (4xuint8)

        private int _fsSelection;

        private int _usFirstCharIndex, _usLastCharIndex;

        private short _sTypoAscender, _sTypoDescender, _sTypoLineGap;

        private int _usWinAscent, _usWinDescent; /*V0*/

        private long _ulCodePageRange1, _ulCodePageRange2; /*V1*/

        private short _sxHeight, _sCapHeight;

        private int _usDefaultChar, _usBreakChar, _usMaxContext; /*V2 V3 V4*/

        private int _usLowerOpticalPointSize, _usUpperOpticalPointSize; /*V5*/

        public OS2(in byte[] data, FileHeaderEntry entry)
        {
            ParseTableData(in data, entry);
        }

        private void ParseTableData(in byte[] data, FileHeaderEntry entry)
        {
            int index = entry.Offset;
            _version = DataHelper.GetAtLocationInt(in data, index, 2, out index); //uint16

            if (_version > VERSION_5)
                throw new Exception("Invalid table version, must be version 5 or lower.");

            if(_version >= VERSION_0)
            {
                _xAvgCharWidth = DataHelper.GetAtLocationShort(in data, index, out index);  //int16

                _usWeightClass = DataHelper.GetAtLocationInt(in data, index, 2, out index);  //uint16
                _usWidthClass = DataHelper.GetAtLocationInt(in data, index, 2, out index);  //uint16

                _fsType = DataHelper.GetAtLocationInt(in data, index, 2, out index);  //uint16

                _ySubscriptXSize = DataHelper.GetAtLocationShort(in data, index, out index);  //int16
                _ySubscriptYSize = DataHelper.GetAtLocationShort(in data, index, out index);  //int16
                _ySubscriptXOffset = DataHelper.GetAtLocationShort(in data, index, out index);  //int16
                _ySubscriptYOffset = DataHelper.GetAtLocationShort(in data, index, out index);  //int16
                _ySuperscriptXSize = DataHelper.GetAtLocationShort(in data, index, out index);  //int16
                _ySuperscriptYSize = DataHelper.GetAtLocationShort(in data, index, out index);  //int16
                _ySuperscriptXOffset = DataHelper.GetAtLocationShort(in data, index, out index);  //int16
                _ySuperscriptYOffset = DataHelper.GetAtLocationShort(in data, index, out index);  //int16
                _yStrikeoutSize = DataHelper.GetAtLocationShort(in data, index, out index);  //int16
                _yStrikeoutPosition = DataHelper.GetAtLocationShort(in data, index, out index);  //int16

                _sFamilyClass = DataHelper.GetAtLocationShort(in data, index, out index);  //int16

                _panose = new short[10];  //uint8x10
                for (int idx = 0; idx < 10; idx++)
                    _panose[idx] = DataHelper.GetAtLocationShort(in data, index, 1, out index);

                _ulUnicodeRange1 = DataHelper.GetAtLocationLong(in data, index, 4, out index);  //uint32
                _ulUnicodeRange2 = DataHelper.GetAtLocationLong(in data, index, 4, out index);  //uint32
                _ulUnicodeRange3 = DataHelper.GetAtLocationLong(in data, index, 4, out index);  //uint32
                _ulUnicodeRange4 = DataHelper.GetAtLocationLong(in data, index, 4, out index);  //uint32

                _achVendID = new short[4];  //tag uint8x4
                for (int idx = 0; idx < 4; idx++)
                    _achVendID[idx] = DataHelper.GetAtLocationShort(in data, index, 1, out index);

                _fsSelection = DataHelper.GetAtLocationInt(in data, index, 2, out index);  //uint16

                _usFirstCharIndex = DataHelper.GetAtLocationInt(in data, index, 2, out index);  //uint16
                _usLastCharIndex = DataHelper.GetAtLocationInt(in data, index, 2, out index);  //uint16

                _sTypoAscender = DataHelper.GetAtLocationShort(in data, index, out index);  //int16
                _sTypoDescender = DataHelper.GetAtLocationShort(in data, index, out index);  //int16
                _sTypoLineGap = DataHelper.GetAtLocationShort(in data, index, out index);  //int16

                _usWinAscent = DataHelper.GetAtLocationInt(in data, index, 2, out index);  //uint16
                _usWinDescent = DataHelper.GetAtLocationInt(in data, index, 2, out index);  //uint16
            }

            if(_version >= VERSION_1)
            {
                _ulCodePageRange1 = DataHelper.GetAtLocationLong(in data, index, 4, out index);  //uint32
                _ulCodePageRange2 = DataHelper.GetAtLocationLong(in data, index, 4, out index);  //uint32
            }

            if(_version >= VERSION_2)
            {
                _sxHeight = DataHelper.GetAtLocationShort(in data, index, out index);  //int16
                _sCapHeight = DataHelper.GetAtLocationShort(in data, index, out index);  //int16

                _usDefaultChar = DataHelper.GetAtLocationInt(in data, index, 2, out index);  //uint16
                _usBreakChar = DataHelper.GetAtLocationInt(in data, index, 2, out index);  //uint16
                _usMaxContext = DataHelper.GetAtLocationInt(in data, index, 2, out index);  //uint16
            }

            if(_version == VERSION_5)
            {
                _usLowerOpticalPointSize = DataHelper.GetAtLocationInt(in data, index, 2, out index);  //uint16
                _usUpperOpticalPointSize = DataHelper.GetAtLocationInt(in data, index, 2, out index);  //uint16
            }
        }
    }
}
