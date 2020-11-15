using System;
using System.Collections.Generic;
using System.Text;

namespace DeeSynk.Core.Components.Types.Fonts
{
    //Pulled from adobe CFF spec
    //https://wwwimages2.adobe.com/content/dam/acom/en/devnet/font/pdfs/5176.CFF.pdf
    
    public enum OperandTypes : short
    {
        Boolean = 0,
        Number = 1,
        Delta = 2,
        SID = 3,
        Array = 4,
        NumberNumber = 11,
        SIDSIDNumber = 331
    }

    public enum Operators : short
    {
        //Standard operators
        version = 0, //SID
        Notice = 1,  //SID
        Copyright = 0xc00,  //SID
        FullName = 2,  //SID
        FamilyName = 3,  //SID
        Weight = 4,  //SID
        isFixedPitch = 0xc01,  //Bool
        ItalicAngle = 0xc02,  //Num
        UnderlinePosition = 0xc03,  //Num
        UnderlineThickness = 0xc04,  //Num
        PaintType = 0xc05,  //Num
        CharstringType = 0xc06,  //Num
        FontMatrix = 0xc07,  //Array
        UniqueID = 13,  //Num
        FontBBox = 5,  //Array
        StrokeWidth = 0xc08, //Num
        XUID = 14,  //Array
        charset = 15,  //Num
        Encoding = 16,  //Num
        CharStrings = 17,  //Num
        Private = 18,  //Num Num (Private DICT)  (size and offset)
        SyntheticBase = 0xc14,  //Num
        PostScript = 0xc15,  //SID
        BaseFontName = 0xc16,  //SID
        BaseFontBlend = 0xc17,  //Delta

        //Private DICT Operators
        BlueValues = 6,  //Delta
        OtherBlues = 7,  //Delta
        FamilyBlues = 8,  //Delta
        FamilyOtherBlues = 9,  //Delta
        BlueScale = 0xc09,  //Num
        BlueShift = 0xc0a,  //Num
        BlueFuzz = 0xc0b,  //Num
        StdHW = 10,  //Num
        StdVW = 11,  //Num
        StemSnapH = 0xc0c,  //Delta
        StemSnapV = 0xc0d,  //Delta
        ForceBold = 0xc0e,  //Bool
        LanguageGroup = 0xc11,  //Num
        ExpansionFactor = 0xc12,  //Num
        initialRandomSeed = 0xc13,  //Num
        Subrs = 19,  //Num
        defaultWidthX = 20,  //Num
        nominalWidthX = 21,  //Num

        //CIDFont operators
        ROS = 0xc1e,  //SID  SID  Num
        CIDFontVersion = 0xc1f,  //Num
        CIDFontRevision = 0xc20,  //Num
        CIDFontType = 0xc21,  //Num
        CIDCount = 0xc22,  //Num
        UIDBase = 0xc23,  //Num
        FDArray = 0xc24,  //Num
        FDSelect = 0xc25,  //Num
        FontName = 0xc26  //SID
    }

    public enum OperatorDataTypes : short
    {
        //Standard operators
        version = OperandTypes.SID,
        Notice = OperandTypes.SID,
        Copyright = OperandTypes.SID,
        FullName = OperandTypes.SID,
        FamilyName = OperandTypes.SID,
        Weight = OperandTypes.SID,
        isFixedPitch = OperandTypes.Boolean,
        ItalicAngle = OperandTypes.Number,
        UnderlinePosition = OperandTypes.Number,
        UnderlineThickness = OperandTypes.Number,
        PaintType = OperandTypes.Number,
        CharstringType = OperandTypes.Number,
        FontMatrix = OperandTypes.Array,
        UniqueID = OperandTypes.Number,
        FontBBox = OperandTypes.Array,
        StrokeWidth = OperandTypes.Number,
        XUID = OperandTypes.Array,
        charset = OperandTypes.Number,
        Encoding = OperandTypes.Number,
        CharStrings = OperandTypes.Number,
        Private = OperandTypes.NumberNumber, //(Private DICT)  (size and offset)
        SyntheticBase = OperandTypes.Number,
        PostScript = OperandTypes.SID,
        BaseFontName = OperandTypes.SID,
        BaseFontBlend = OperandTypes.Delta,

        //Private DICT Operators
        BlueValues = OperandTypes.Delta,
        OtherBlues = OperandTypes.Delta,
        FamilyBlues = OperandTypes.Delta,
        FamilyOtherBlues = OperandTypes.Delta,
        BlueScale = OperandTypes.Number,
        BlueShift = OperandTypes.Number,
        BlueFuzz = OperandTypes.Number,
        StdHW = OperandTypes.Number,
        StdVW = OperandTypes.Number,
        StemSnapH = OperandTypes.Delta,
        StemSnapV = OperandTypes.Delta,
        ForceBold = OperandTypes.Boolean,
        LanguageGroup = OperandTypes.Number,
        ExpansionFactor = OperandTypes.Number,
        initialRandomSeed = OperandTypes.Number,
        Subrs = OperandTypes.Number,
        defaultWidthX = OperandTypes.Number,
        nominalWidthX = OperandTypes.Number,

        //CIDFont operators
        ROS = OperandTypes.SIDSIDNumber,
        CIDFontVersion = OperandTypes.Number,
        CIDFontRevision = OperandTypes.Number,
        CIDFontType = OperandTypes.Number,
        CIDCount = OperandTypes.Number,
        UIDBase = OperandTypes.Number,
        FDArray = OperandTypes.Number,
        FDSelect = OperandTypes.Number,
        FontName = OperandTypes.SID,
    }
    public class CFFDictionary
    {
        public static readonly byte TWO_BYTE_OP_DELIMINATOR = 0xc;

        private short _count;
        public short Count { get => _count; set => _count = value; }

        private byte _offset;
        public byte offset { get => _offset; set => _offset = value; }

        private int[] _offsets;
        public int[] Offsets
        {
            get => _offsets;
            set
            {
                if (value.Length == _count + 1)
                    _offsets = value;
                else
                    throw new ArgumentException("Invalid number of offset values.  Number of offsets must be one more than the number of objects.");
            }
        }

        public CFFDictionary(short count, byte offset)
        {
            _count = count;
            _offset = offset;
            _offsets = new int[count + 1];
        }
    }
}
