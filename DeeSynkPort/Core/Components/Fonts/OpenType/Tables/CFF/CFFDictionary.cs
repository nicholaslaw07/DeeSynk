using DeeSynk.Core.Components.Fonts;
using System;
using System.Collections.Generic;
using System.Text;

namespace DeeSynk.Core.Components.Fonts.Tables.CFF
{
    //Pulled from adobe CFF spec
    //https://wwwimages2.adobe.com/content/dam/acom/en/devnet/font/pdfs/5176.CFF.pdf
    
    public enum DataTypes : short
    {
        UNDEFINED = 0x7fff,
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
        UNDEFINED = 0x7fff,
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
        UNDEFINED = DataTypes.UNDEFINED,
        //Standard operators
        version = DataTypes.SID,
        Notice = DataTypes.SID,
        Copyright = DataTypes.SID,
        FullName = DataTypes.SID,
        FamilyName = DataTypes.SID,
        Weight = DataTypes.SID,
        isFixedPitch = DataTypes.Boolean,
        ItalicAngle = DataTypes.Number,
        UnderlinePosition = DataTypes.Number,
        UnderlineThickness = DataTypes.Number,
        PaintType = DataTypes.Number,
        CharstringType = DataTypes.Number,
        FontMatrix = DataTypes.Array,
        UniqueID = DataTypes.Number,
        FontBBox = DataTypes.Array,
        StrokeWidth = DataTypes.Number,
        XUID = DataTypes.Array,
        charset = DataTypes.Number,
        Encoding = DataTypes.Number,
        CharStrings = DataTypes.Number,
        Private = DataTypes.NumberNumber, //(Private DICT)  (size and offset)
        SyntheticBase = DataTypes.Number,
        PostScript = DataTypes.SID,
        BaseFontName = DataTypes.SID,
        BaseFontBlend = DataTypes.Delta,

        //Private DICT Operators
        BlueValues = DataTypes.Delta,
        OtherBlues = DataTypes.Delta,
        FamilyBlues = DataTypes.Delta,
        FamilyOtherBlues = DataTypes.Delta,
        BlueScale = DataTypes.Number,
        BlueShift = DataTypes.Number,
        BlueFuzz = DataTypes.Number,
        StdHW = DataTypes.Number,
        StdVW = DataTypes.Number,
        StemSnapH = DataTypes.Delta,
        StemSnapV = DataTypes.Delta,
        ForceBold = DataTypes.Boolean,
        LanguageGroup = DataTypes.Number,
        ExpansionFactor = DataTypes.Number,
        initialRandomSeed = DataTypes.Number,
        Subrs = DataTypes.Number,
        defaultWidthX = DataTypes.Number,
        nominalWidthX = DataTypes.Number,

        //CIDFont operators
        ROS = DataTypes.SIDSIDNumber,
        CIDFontVersion = DataTypes.Number,
        CIDFontRevision = DataTypes.Number,
        CIDFontType = DataTypes.Number,
        CIDCount = DataTypes.Number,
        UIDBase = DataTypes.Number,
        FDArray = DataTypes.Number,
        FDSelect = DataTypes.Number,
        FontName = DataTypes.SID,
    }

    public class CFFDictionary : Dictionary<Operators, Operand[]>
    {
        public static readonly byte TWO_BYTE_OP_DELIMINATOR = 0xc;

        private int _offset;
        public int Offset { get => _offset; }

        public CFFDictionary(in byte[] data, int startIndex, int dataSize, out int newStart) : base()
        {
            newStart = startIndex;
            _offset = startIndex;
            OperandNumberTypes dataType = GetNumberType(data[newStart]);
            while (dataType != OperandNumberTypes.UNDEFINED && (newStart - startIndex) < dataSize)
            {
                var operands = ParseCFFOperands(in data, newStart, out newStart);
                Add(ParseCFFOperator(in data, newStart, out newStart), operands.ToArray());
            }
        }

        public CFFDictionary(in byte[] data, int startIndex, int dataSize) : base()
        {
            int newStart = startIndex;
            _offset = startIndex;
            OperandNumberTypes dataType = GetNumberType(data[newStart]);
            while (dataType != OperandNumberTypes.UNDEFINED && (newStart - startIndex) < dataSize)
            {
                var operands = ParseCFFOperands(in data, newStart, out newStart);
                Add(ParseCFFOperator(in data, newStart, out newStart), operands.ToArray());
            }
        }

        private OperandNumberTypes GetNumberType(byte b0)
        {
            switch (b0)
            {
                case byte b when ((b >= 0x20 && b <= 0xfe) || (b >= 0x1c && b <= 0x1d)): return OperandNumberTypes.Integer;
                case (0x1e): return OperandNumberTypes.Real;
                default: return OperandNumberTypes.UNDEFINED;
            }
        }

        private Operators ParseCFFOperator(in byte[] data, int startIndex, out int newStart)
        {
            newStart = startIndex + ((data[startIndex] == 0xc) ? 2 : 1);
            return (Operators)((data[startIndex] == 0xc) ? (short)(data[startIndex] << 8 | data[startIndex + 1]) : data[startIndex]);
        }

        private List<Operand> ParseCFFOperands(in byte[] data, int startIndex, out int newStart)
        {
            newStart = startIndex;
            List<Operand> operands = new List<Operand>();
            while (ValidIntegerStart(data[newStart]))
            {
                switch (data[newStart])
                {
                    case byte b when (b >= 0x20 && b <= 0xf6): operands.Add(new Operand(data[newStart] - 139)); newStart += 1; break;
                    case byte b when (b >= 0xf7 && b <= 0xfa): operands.Add(new Operand((data[newStart] - 247) * 256 + data[newStart + 1] + 108)); newStart += 2; break;
                    case byte b when (b >= 0xfb && b <= 0xfe): operands.Add(new Operand(-(data[newStart] - 251) * 256 - data[newStart + 1] - 108)); newStart += 2; break;
                    case (0x1c): operands.Add(new Operand(data[newStart + 1] << 8 | data[newStart + 2])); newStart += 3; break;
                    case (0x1d): operands.Add(new Operand(data[newStart + 1] << 24 | data[newStart + 2] << 16 | data[newStart + 3] << 8 | data[newStart + 4])); newStart += 5; break;
                    case (0x1e): operands.Add(new Operand(ParseCFFReal(in data, startIndex, out newStart))); break;
                }
            }
            return operands;
        }

        private double ParseCFFReal(in byte[] data, int startIndex, out int newStart)
        {
            newStart = startIndex;

            byte[] firstNibble = SplitByte(data[newStart += 1]);
            double beforeDec = ((firstNibble[0] == 0xe) ? -firstNibble[1] : firstNibble[0]);
            double afterDec = 0;
            double exponent = 0;
            int afterDecCount = 0;
            int phase = ((firstNibble[1] == 0xa) ? 1 : 0);  //0 = beforeDec  1 = afterDec  2 = postivie exponent  3 = negative exponent

            bool done = false;

            while (!done)
            {
                byte[] nibble = SplitByte(data[newStart += 1]);
                for (int idx = 0; idx < 2; idx++)
                {
                    switch (nibble[idx])
                    {
                        case (0xa): phase = 1; break;
                        case (0xb): phase = 2; break;
                        case (0xc): phase = 3; break;
                        case (0xd): throw new Exception("Reserved token, invalid nibble format.");
                        case (0xe): if (phase == 0) { phase = 1; } else { throw new Exception("More than one minus token, invalid nibble format."); } break;
                        case (0xf): done = true; break;
                        default:
                            switch (phase)
                            {
                                case (0): beforeDec = beforeDec * 10 + nibble[idx]; break;
                                case (1): afterDec = afterDec * 10 + nibble[idx]; afterDecCount--; break;
                                default: exponent = exponent * 10 + nibble[idx]; break;
                            }
                            break;
                    }
                }
            }

            double outValue = (beforeDec + afterDec * Math.Pow(10, afterDecCount) * ((beforeDec < 0) ? -1 : 1));
            if (phase > 1)
                outValue = outValue * Math.Pow(10, ((phase == 2) ? exponent : -exponent));
            return outValue;
        }

        private byte[] SplitByte(byte value)
        {
            byte[] output = new byte[2];
            output[0] = (byte)(value >> 4);
            output[1] = (byte)(value & 0x0f);
            return output;
        }

        private bool ValidIntegerStart(byte b0)
        {
            return (b0 >= 0x20 && b0 <= 0xfe) || (b0 >= 0x1c && b0 <= 0x1e);
        }

        //Honestly, this can be simplified and the whole intermediate enum of OperatorDataTypes can be eliminated.
        public static OperatorDataTypes GetDataType(Operators op)
        {
            switch (op)
            {
                //Standard operators
                case(Operators.version): return OperatorDataTypes.version;
                case (Operators.Notice): return OperatorDataTypes.Notice;
                case (Operators.Copyright): return OperatorDataTypes.Copyright;
                case (Operators.FullName): return OperatorDataTypes.FullName;
                case (Operators.FamilyName): return OperatorDataTypes.FamilyName;
                case (Operators.Weight): return OperatorDataTypes.Weight;
                case (Operators.isFixedPitch): return OperatorDataTypes.isFixedPitch;
                case (Operators.ItalicAngle): return OperatorDataTypes.ItalicAngle;
                case (Operators.UnderlinePosition): return OperatorDataTypes.UnderlinePosition;
                case (Operators.UnderlineThickness): return OperatorDataTypes.UnderlineThickness;
                case (Operators.PaintType): return OperatorDataTypes.PaintType;
                case (Operators.CharstringType): return OperatorDataTypes.CharstringType;
                case (Operators.FontMatrix): return OperatorDataTypes.FontMatrix;
                case (Operators.UniqueID): return OperatorDataTypes.UniqueID;
                case (Operators.FontBBox): return OperatorDataTypes.FontBBox;
                case (Operators.StrokeWidth): return OperatorDataTypes.StrokeWidth;
                case (Operators.XUID): return OperatorDataTypes.XUID;
                case (Operators.charset): return OperatorDataTypes.charset;
                case (Operators.Encoding): return OperatorDataTypes.Encoding;
                case (Operators.CharStrings): return OperatorDataTypes.CharStrings;
                case (Operators.Private): return OperatorDataTypes.Private;
                case (Operators.SyntheticBase): return OperatorDataTypes.SyntheticBase;
                case (Operators.PostScript): return OperatorDataTypes.PostScript;
                case (Operators.BaseFontName): return OperatorDataTypes.BaseFontName;
                case (Operators.BaseFontBlend): return OperatorDataTypes.BaseFontBlend;

                //Private DICT operators
                case (Operators.BlueValues): return OperatorDataTypes.BlueValues;
                case (Operators.OtherBlues): return OperatorDataTypes.OtherBlues;
                case (Operators.FamilyBlues): return OperatorDataTypes.FamilyBlues;
                case (Operators.FamilyOtherBlues): return OperatorDataTypes.FamilyOtherBlues;
                case (Operators.BlueScale): return OperatorDataTypes.BlueScale;
                case (Operators.BlueShift): return OperatorDataTypes.BlueShift;
                case (Operators.BlueFuzz): return OperatorDataTypes.BlueFuzz;
                case (Operators.StdHW): return OperatorDataTypes.StdHW;
                case (Operators.StdVW): return OperatorDataTypes.StdVW;
                case (Operators.StemSnapH): return OperatorDataTypes.StemSnapH;
                case (Operators.StemSnapV): return OperatorDataTypes.StemSnapV;
                case (Operators.ForceBold): return OperatorDataTypes.ForceBold;
                case (Operators.LanguageGroup): return OperatorDataTypes.LanguageGroup;
                case (Operators.ExpansionFactor): return OperatorDataTypes.ExpansionFactor;
                case (Operators.initialRandomSeed): return OperatorDataTypes.initialRandomSeed;
                case (Operators.Subrs): return OperatorDataTypes.Subrs;
                case (Operators.defaultWidthX): return OperatorDataTypes.defaultWidthX;
                case (Operators.nominalWidthX): return OperatorDataTypes.nominalWidthX;

                //CIDFont operators
                case (Operators.ROS): return OperatorDataTypes.ROS;
                case (Operators.CIDFontVersion): return OperatorDataTypes.CIDFontVersion;
                case (Operators.CIDFontRevision): return OperatorDataTypes.CIDFontRevision;
                case (Operators.CIDFontType): return OperatorDataTypes.CIDFontType;
                case (Operators.CIDCount): return OperatorDataTypes.CIDCount;
                case (Operators.UIDBase): return OperatorDataTypes.UIDBase;
                case (Operators.FDArray): return OperatorDataTypes.FDArray;
                case (Operators.FDSelect): return OperatorDataTypes.FDSelect;
                case (Operators.FontName): return OperatorDataTypes.FontName;

                default: return OperatorDataTypes.UNDEFINED;
            }
        }

        //Get values for each different data type
        //Checks if requested operator 
    }
}
