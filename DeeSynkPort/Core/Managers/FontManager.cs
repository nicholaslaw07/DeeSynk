using DeeSynk.Core.Components.Fonts;
using DeeSynk.Core.Components.Types.Fonts;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace DeeSynk.Core.Managers
{
    public class FontManager : IManager
    {

        private static FontManager _fontManager;

        private Dictionary<string, Font> _fonts;
        public Dictionary<string, Font> Fonts { get => _fonts; }

        #region STANDARD_VARS
        private readonly int _off1 = 1;
        private readonly int _off2 = 2;
        private readonly int _off4 = 4;
        #endregion

        private FontManager()
        {
            _fonts = new Dictionary<string, Font>();
        }

        public static ref FontManager GetInstance()
        {
            if (_fontManager == null)
                _fontManager = new FontManager();

            return ref _fontManager;
        }

        public void Load()
        {
            //LoadFontGeometry(@"C:\Users\Nicholas\source\repos\nicholaslaw07\DeeSynk\DeeSynkPort\Resources\Fonts\OfficeCodePro-Light\OfficeCodePro-Light.otf", "OfficeCodePro-Light.otf");
            LoadFontGeometry(@"C:\Users\Nicholas\source\repos\nicholaslaw07\DeeSynk\DeeSynkPort\Resources\Fonts\Mechanical\Mechanical-g5Y5.otf", "Mechanical-g5Y5");
        }

        public void UnLoad()
        {
            throw new NotImplementedException();
        }

        #region Parser
        //Documentation references:
        //https://wwwimages2.adobe.com/content/dam/acom/en/devnet/font/pdfs/5176.CFF.pdf
        //https://simoncozens.github.io/fonts-and-layout/opentype.html
        //https://www.adobe.com/content/dam/acom/en/devnet/font/pdfs/5177.Type2.pdf
        private void LoadFontGeometry(string path, string name)
        {
            byte[] file = File.ReadAllBytes(path);

            Font font = new Font(name);

            if (!HasOTTOHeader(in file))
                throw new Exception("File does not contain valid header for OpenType format");

            {
                GetFileHeaderEntries(in file, _off4 * 3, out List<FileHeaderEntry> mainHeaderList);
                font.HeaderEntires = mainHeaderList;
            }

            foreach(FileHeaderEntry entry in font.HeaderEntires)
            {
                switch (entry.Table)
                {
                    case (0x43464620 /*CFF*/): font.CFFTable = ParseCFFTable(in file, entry); break;
                    default: break;
                }
            }

            _fonts.Add(name, font);
        }
        private bool ExistsAtLocation(in byte[] data, int start, int count, int compare) { return GetAtLocation4(in data, start, count) == compare; }

        private int GetAtLocation4(in byte[] data, int start, int count)
        {
            if (count > 4) throw new ArgumentOutOfRangeException("The number of bytes read can only correspond to a 32-bit value.");
            int d = 0;
            for (int idx = 0; idx < count; idx++)
                d += (int)(data[idx + start] << (8 * (count - 1 - idx)));
            return d;
        }

        private short GetAtLocation2(in byte[] data, int start, int count)
        {
            if (count > 2) throw new ArgumentOutOfRangeException("The number of bytes read can only correspond to a 16-bit value.");
            short d = 0;
            for (int idx = 0; idx < count; idx++)
                d += (short)(data[idx + start] << (8 * (count - 1 - idx)));
            return d;
        }

        private bool HasOTTOHeader(in byte[] data) { return ExistsAtLocation(in data, 0, 4, Font.OTTO); }

        private void GetFileHeaderEntries(in byte[] data, int start, out List<FileHeaderEntry> headerEntries)
        {
            int offset = start - _off4;
            bool stillChecking = true;
            headerEntries = new List<FileHeaderEntry>();
            List<int> hNames = new List<int>(Font.headerNames);
            do
            {
                int testVal = GetAtLocation4(in data, offset += _off4, _off4);
                stillChecking = hNames.Contains(testVal);
                if (stillChecking)
                {
                    headerEntries.Add(new FileHeaderEntry(
                        testVal, 
                        GetAtLocation4(in data, offset += _off4, _off4), 
                        GetAtLocation4(in data, offset += _off4, _off4),
                        GetAtLocation4(in data, offset += _off4, _off4)));
                }
            } while (stillChecking);
        }

        //OpenType/CFF Stuff

        private CFFTable ParseCFFTable(in byte[] data, FileHeaderEntry entry)
        {
            int startIndex = entry.Offset;
            int newStart = startIndex;
            CFFTable table = new CFFTable(startIndex);
            table.Header = new CFFHeader(data[startIndex + 0], data[startIndex + 1], data[startIndex + 2], data[startIndex + 3]);
            startIndex += table.Header.HeaderSize;
            //NAME Index  //Should be limited to 127 characters with no special characteres (33- 126)
            table.IndexName = ParseCFFIndex(in data, startIndex, out startIndex);
            table.TopDictionaryIndex = ParseCFFDictionaryIndex(in data, startIndex, out startIndex);
            table.IndexString = ParseCFFIndex(in data, startIndex, out startIndex);  //values are accessed at index of idx+390
            table.IndexGlobalSubr = ParseCFFIndex(in data, startIndex, out startIndex);
            if (table.TopDictionaryIndex.Data[0].TryGetValue(Operators.CharStrings, out Operand[] charStringIdx))
            {
                if (charStringIdx.Length == 1)
                    table.IndexCharStrings = ParseCFFIndex(in data, table.StartIndex + charStringIdx[0].IntegerValue, out newStart);
            }
            table.CharStringCommands = ParseCFFCharStrings(table.IndexCharStrings);
            if (table.TopDictionaryIndex.Data[0].TryGetValue(Operators.Private, out Operand[] privateOperands))
            {
                if (privateOperands.Length == 2)
                    table.PrivateDictionary = ParseCFFDictionary(in data, table.StartIndex + privateOperands[1].IntegerValue, privateOperands[0].IntegerValue, out newStart);
            }
            if (table.TopDictionaryIndex.Data[0].TryGetValue(Operators.charset, out Operand[] charsetOperands))
            {
                if (charsetOperands.Length == 1)
                    table.Charsets = ParseCFFCharsets(in data, table.CharStringCommands.Length, table.StartIndex + charsetOperands[0].IntegerValue, out newStart);
            }
            //var gsub19 = table.IndexCharStrings.GetDataAtIndex(19);
            int x = 1;
            return table;
        }

        //add parsers in the constructors of the classes that are being used

        //Add real number support
        //Add fixed value support
        //Link charsets to charstrings
        //Decipher the mess that is poscript code
        //Understand where the subroutines are stored, if anywhere

        private void ParseCFFIndexHeader(in byte[] data, int startIndex, out int newStart, out short count, out byte offset)
        {
            newStart = startIndex;
            count = GetAtLocation2(in data, startIndex, 2);
            //if(count == 0)
            offset = data[newStart += _off2];
            newStart += _off1;
        }

        private void ParseCFFIndexOffsets(in byte[] data, int startIndex, byte offset, ref int[] offsets, ref int[] offsetGaps, out int newStart)
        {
            newStart = startIndex - offset;
            offsets[0] = GetAtLocation4(in data, newStart += offset, offset) - 1;
            for (int idx = 0; idx < offsets.Length - 1; idx++)
                offsetGaps[idx] = (offsets[idx + 1] = GetAtLocation4(in data, newStart += offset, offset) - 1) - offsets[idx];
            newStart += offset;
        }

        private CFFIndex ParseCFFIndex(in byte[] data, int startIndex, out int newStart)
        {
            ParseCFFIndexHeader(in data, startIndex, out startIndex, out short count, out byte offset);
            CFFIndex index = new CFFIndex(count, offset);
            ParseCFFIndexOffsets(in data, startIndex, index.Offset, ref index.OffsetsRef, ref index.OffsetGapsRef, out startIndex);
            int size = index.DataSize;
            index.Data = new byte[size];
            for(int idx = 0; idx < index.Data.Length; idx++)
                index.Data[idx] = data[startIndex + idx];
            newStart = startIndex + size;

            return index;
        }

        private CFFDictionaryIndex ParseCFFDictionaryIndex(in byte[] data, int startIndex, out int newStart)
        {
            newStart = startIndex;
            ParseCFFIndexHeader(in data, newStart, out newStart, out short count, out byte offset);
            CFFDictionaryIndex dictIndex = new CFFDictionaryIndex(count, offset);
            ParseCFFIndexOffsets(in data, newStart, dictIndex.Offset, ref dictIndex.OffsetsRef, ref dictIndex.OffsetGapsRef, out newStart);
            for(int idx = 0; idx < dictIndex.Data.Length; idx++)
                dictIndex.Data[idx] = ParseCFFDictionary(in data, newStart, dictIndex.OffsetGaps[idx], out newStart);
            return dictIndex;
        }

        private CFFDictionary ParseCFFDictionary(in byte[] data, int startIndex, int dataSize, out int newStart)
        {
            newStart = startIndex;
            var dict = new CFFDictionary(startIndex);
            OperandNumberTypes dataType = GetNumberType(data[newStart]);
            while (dataType != OperandNumberTypes.UNDEFINED && (newStart - startIndex) < dataSize)
            {
                var operands = ParseCFFOperands(in data, newStart, out newStart);
                dict.Add(ParseCFFOperator(in data, newStart, out newStart), operands.ToArray());
            }

            return dict;
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
                    case byte b when (b >= 0xf7 && b <= 0xfa): operands.Add(new Operand((data[newStart] - 247) * 256 + data[newStart+1] + 108)); newStart += 2; break;
                    case byte b when (b >= 0xfb && b <= 0xfe): operands.Add(new Operand(-(data[newStart] - 251) * 256 - data[newStart + 1] - 108)); newStart += 2; break;
                    case (0x1c): operands.Add(new Operand(data[newStart+1]<<8 | data[newStart+2])); newStart += 3; break;
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
                for(int idx = 0; idx < 2; idx++)
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

        private Operators ParseCFFOperator(in byte[] data, int startIndex, out int newStart)
        {
            newStart = startIndex + ((data[startIndex] == 0xc) ? 2 : 1);
            return (Operators)((data[startIndex] == 0xc) ? (short)(data[startIndex] << 8 | data[startIndex + 1]) : data[startIndex]);
        }

        private bool ValidIntegerStart(byte b0)
        {
            return (b0 >= 0x20 && b0 <= 0xfe) || (b0 >= 0x1c && b0 <= 0x1e);
        }

        private bool ValidNumberStart(byte b0)
        {
            return (b0 >= 0x20 && b0 <= 0xff) || (b0 == 0x1c);
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

        private CSOperandNumberTypes GetCSNumberType(byte b0)
        {
            switch (b0)
            {
                case byte b when ((b >= 0x20 && b <= 0xfe) || (b == 0x1c)): return CSOperandNumberTypes.Short;
                case (0xff): return CSOperandNumberTypes.Fixed;
                default: return CSOperandNumberTypes.UNDEFINED;
            }
        }

        private CFFCharStringCommands[] ParseCFFCharStrings(CFFIndex indexCharStrings)
        {
            CFFCharStringCommands[] commands = new CFFCharStringCommands[indexCharStrings.Count];
            int count = 0;
            for(int idx = 0; idx < indexCharStrings.Count; idx++)
            {
                commands[idx] = new CFFCharStringCommands();
                var code = indexCharStrings.GetDataAtIndex(idx);
                if(code.Length == 1 && code[0] == 0x0e) //this is for the space character.  the minimum number of commands in a CharString is 1 and it must be the operand free operator of endchar (0x0e)
                {
                    commands[idx].Add(new CharStringFunction(CSOperators.endchar, new CSOperand[0]));
                    continue;
                }
                var dataType = GetCSNumberType(code[0]);
                int dataSize = indexCharStrings.OffsetGaps[idx];
                int startIndex = 0;
                int newStart = startIndex;

                while ((newStart - startIndex) < dataSize)
                {
                    var operands = ParseCFFOperandsCS(in code, newStart, out newStart);
                    commands[idx].Add(new CharStringFunction(ParseCFFCSOperator(in code, newStart, out newStart), operands.ToArray()));
                }

                //TEST
                if (commands[idx].Count > 0)
                {
                    var com = commands[idx].ToArray();
                    if (com[commands[idx].Count - 1].Operator != CSOperators.endchar && 
                        com[commands[idx].Count - 1].Operator != CSOperators.callgsubr &&
                        com[commands[idx].Count - 1].Operator != CSOperators.callsubr)
                    {
                        count++;
                        Debug.WriteLine("This is not okay.  Count: {0}   Index: {1}", count, idx);
                    }
                }
                else
                {
                    Debug.WriteLine("This still isn't okay.  Index: {0}", idx);
                }
                //ENDTEST

            }
            return commands;
        }

        private List<CSOperand> ParseCFFOperandsCS(in byte[] data, int startIndex, out int i)
        {
            i = startIndex;
            List<CSOperand> operands = new List<CSOperand>();
            while (ValidNumberStart(data[i]))
            {
                switch (data[i])  //as specified in the adobe charstring spec (src 3) type 2 values beginning with 255 are fixed values.  we interpret this as standard fixed-point arithmetic (A16/B16)
                {
                    case byte b when (b >= 0x20 && b <= 0xf6): operands.Add(new CSOperand(CSOperandNumberTypes.Short, (short)(data[i] - 139), 0.0f)); i += 1; break;
                    case byte b when (b >= 0xf7 && b <= 0xfa): operands.Add(new CSOperand(CSOperandNumberTypes.Short, (short)((data[i] - 247) * 256 + data[i + 1] + 108), 0.0f)); i += 2; break;
                    case byte b when (b >= 0xfb && b <= 0xfe): operands.Add(new CSOperand(CSOperandNumberTypes.Short, (short)(-(data[i] - 251) * 256 - data[i + 1] - 108), 0.0f)); i += 2; break;
                    case (0x1c): operands.Add(new CSOperand(CSOperandNumberTypes.Short, (short)(data[i + 1] << 8 | data[i + 2]), 0.0f)); i += 3; break;
                    case (0xff): operands.Add(new CSOperand(CSOperandNumberTypes.Fixed, 0, (short)(data[i + 1] << 8 | data[i + 2]) + (data[i + 3] << 8 | data[i + 4]) * (float)1.5258789E-05)); i += 5; break;
                }
            }
            return operands;
        }
        private CSOperators ParseCFFCSOperator(in byte[] data, int startIndex, out int newStart)
        {
            newStart = startIndex + ((data[startIndex] == 0xc) ? 2 : 1);
            return (CSOperators)((data[startIndex] == 0xc) ? (short)(data[startIndex] << 8 | data[startIndex + 1]) : data[startIndex]);
        }

        private CFFCharsets ParseCFFCharsets(in byte[] data, int nGlyphs, int startIndex, out int newStart)
        {
            newStart = startIndex;
            byte format = data[startIndex];
            CFFCharsets charset = new CFFCharsets(format);
            int idx = 0;
            switch (format)
            {
                case (0):
                    for (newStart = startIndex + 1; newStart < startIndex + 2 * nGlyphs; newStart+=2)
                        charset.Add((short)(data[newStart] << 8 | data[newStart + 1]));
                    break;
                case (1):
                    while(idx < nGlyphs)
                    {
                        short first = (short)(data[newStart] << 8 | data[newStart + 1]);
                        byte nLeft = data[newStart + 2];
                        for (byte jdx = 0; jdx <= nLeft; jdx++)
                            charset.Add((short)(first + jdx));
                        idx += nLeft + 1;
                        newStart += 3;
                    }
                    break;
                case (2):
                    while (idx < nGlyphs)
                    {
                        short first = (short)(data[newStart] << 8 | data[newStart + 1]);
                        short nLeft = (short)(data[newStart + 2] << 8 | data[newStart + 3]);
                        for (short jdx = 0; jdx <= nLeft; jdx++)
                            charset.Add((short)(first + jdx));
                        idx += nLeft + 1;
                        newStart += 4;
                    }
                    break;
            }
            return charset;
        }



        private byte[] GetSubsetLE(in byte[] data, int offset, int count)
        {
            byte[] val = new byte[count];
            for (int i = 0; i < count; i++)
                val[i] = data[offset + count - 1 - i];
            return val;
        }
        private byte[] GetSubsetBE(in byte[] data, int offset, int count)
        {
            byte[] val = new byte[count];
            for (int i = 0; i < count; i++)
                val[i] = data[offset + i];
            return val;
        }
        private void GetSubsetLE(in byte[] data, int offset, int count, out byte[] val)
        {
            val = new byte[count];
            for (int i = 0; i < count; i++)
                val[i] = data[offset + count - 1 - i];
        }
        private void GetSubsetBE(in byte[] data, int offset, int count, out byte[] val)
        {
            val = new byte[count];
            for (int i = 0; i < count; i++)
                val[i] = data[offset + i];
        }
        #endregion
    }
}
