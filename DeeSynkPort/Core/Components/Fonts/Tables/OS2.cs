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
            _version = DataHelper.GetAtLocationInt(in data, index, 2, out index);
            switch (_version)
            {
                case (VERSION_5):
                    //ADD VERSION_5 stuff
                    goto case VERSION_2;
                case (VERSION_4):
                    goto case VERSION_2;
                case (VERSION_3):
                    goto case VERSION_2;
                case (VERSION_2):
                    //ADD VERSION_2 stuff
                    goto case VERSION_1;
                case (VERSION_1):
                    //ADD VERSION_1 stuff
                    goto case VERSION_0;
                case (VERSION_0):
                    _xAvgCharWidth = DataHelper.GetAtLocationShort(in data, index, out index);

                    _usWeightClass = DataHelper.GetAtLocationInt(in data, index, 2, out index);
                    _usWidthClass = DataHelper.GetAtLocationInt(in data, index, 2, out index);

                    _fsType = DataHelper.GetAtLocationInt(in data, index, 2, out index);

                    _ySubscriptXSize = DataHelper.GetAtLocationShort(in data, index, out index);
                    _ySubscriptYSize = DataHelper.GetAtLocationShort(in data, index, out index);
                    _ySubscriptXOffset = DataHelper.GetAtLocationShort(in data, index, out index);
                    _ySubscriptYOffset = DataHelper.GetAtLocationShort(in data, index, out index);
                    _ySuperscriptXSize = DataHelper.GetAtLocationShort(in data, index, out index);
                    _ySuperscriptYSize = DataHelper.GetAtLocationShort(in data, index, out index);
                    _ySuperscriptXOffset = DataHelper.GetAtLocationShort(in data, index, out index);
                    _ySuperscriptYOffset = DataHelper.GetAtLocationShort(in data, index, out index);
                    _yStrikeoutSize = DataHelper.GetAtLocationShort(in data, index, out index);
                    _yStrikeoutPosition = DataHelper.GetAtLocationShort(in data, index, out index);

                    _sFamilyClass = DataHelper.GetAtLocationShort(in data, index, out index);

                    _panose = new short[10];
                    for(int idx = 0; idx < 10; idx++)
                        _panose[idx] = DataHelper.GetAtLocationShort(in data, index, 1, out index);

                    _ulUnicodeRange1 = DataHelper.GetAtLocationLong(in data, index, 4, out index);
                    _ulUnicodeRange2 = DataHelper.GetAtLocationLong(in data, index, 4, out index);
                    _ulUnicodeRange3 = DataHelper.GetAtLocationLong(in data, index, 4, out index);
                    _ulUnicodeRange4 = DataHelper.GetAtLocationLong(in data, index, 4, out index);

                    _achVendID = new short[4];
                    for (int idx = 0; idx < 4; idx++)
                        _achVendID[idx] = DataHelper.GetAtLocationShort(in data, index, 1, out index);

                    _fsSelection = DataHelper.GetAtLocationInt(in data, index, 2, out index);

                    _usFirstCharIndex = DataHelper.GetAtLocationInt(in data, index, 2, out index);
                    _usLastCharIndex = DataHelper.GetAtLocationInt(in data, index, 2, out index);

                    _sTypoAscender = DataHelper.GetAtLocationShort(in data, index, out index);
                    _sTypoDescender = DataHelper.GetAtLocationShort(in data, index, out index);
                    _sTypoLineGap = DataHelper.GetAtLocationShort(in data, index, out index);
                    break;
            }
        }
    }
}
