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

        private int _xAvgCharWidth;

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
            _version = DataHelper.GetAtLocation4(in data, index, out index);
        }
    }
}
