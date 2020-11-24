using DeeSynk.Core.Components.Fonts;
using DeeSynk.Core.Components.Fonts.Tables;
using DeeSynk.Core.Components.Fonts.Tables.CFF;
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
            //LoadFontGeometry(@"C:\Users\Chuck\source\repos\nicholaslaw07\DeeSynk\DeeSynkPort\Resources\Fonts\OfficeCodePro-Light\OfficeCodePro-Light.otf", "OfficeCodePro-Light.otf");
            //LoadFontGeometry(@"C:\Users\Nicholas\source\repos\nicholaslaw07\DeeSynk\DeeSynkPort\Resources\Fonts\Mechanical\Mechanical-g5Y5.otf", "Mechanical-g5Y5");
            LoadFontGeometry(@"C:\Users\Chuck\source\repos\nicholaslaw07\DeeSynk\DeeSynkPort\Resources\Fonts\Mechanical\Mechanical-g5Y5.otf", "Mechanical-g5Y5");
        }

        public void UnLoad()
        {
            throw new NotImplementedException();
        }

        //Documentation references:
        //https://wwwimages2.adobe.com/content/dam/acom/en/devnet/font/pdfs/5176.CFF.pdf
        //https://simoncozens.github.io/fonts-and-layout/opentype.html
        //https://www.adobe.com/content/dam/acom/en/devnet/font/pdfs/5177.Type2.pdf
        private void LoadFontGeometry(string path, string name)
        {
            byte[] file = File.ReadAllBytes(path);

            Font font = new Font(path, name);

            try
            {
                if (!DataHelper.HasOTTOHeader(in file))
                    throw new Exception("File does not contain valid header for OpenType format");

                {
                    GetFileHeaderEntries(in file, _off4 * 3, out List<FileHeaderEntry> mainHeaderList);
                    font.HeaderEntires = mainHeaderList;
                }

                foreach (FileHeaderEntry entry in font.HeaderEntires)
                {
                    switch (entry.Table)
                    {
                        case (0x43464620 /*CFF */): font.CFFTable = ParseCFFTable(in file, entry); break;
                        case (0x6d617870 /*maxp*/): font.MaxP = new MaximumProfile(in file, entry); break;
                        case (0x4f532f32 /*OS/2*/): font.OS2 = new OS2(in file, entry); break;
                        default: Debug.WriteLine("Unknown or unsupported table."); break;
                    }
                }

                _fonts.Add(name, font);
            }
            catch(Exception e)
            {
                Debug.WriteLine($"Exception while parsing the font {name} at {path}.\n{e}");
            }
        }

        private void GetFileHeaderEntries(in byte[] data, int start, out List<FileHeaderEntry> headerEntries)
        {
            int offset = start - _off4;
            bool stillChecking = true;
            headerEntries = new List<FileHeaderEntry>();
            List<int> hNames = new List<int>(Font.headerNames);
            do
            {
                int testVal = DataHelper.GetAtLocationInt(in data, offset += _off4, _off4);
                stillChecking = hNames.Contains(testVal);
                if (stillChecking)
                {
                    headerEntries.Add(new FileHeaderEntry(
                        testVal,
                        DataHelper.GetAtLocationInt(in data, offset += _off4, _off4),
                        DataHelper.GetAtLocationInt(in data, offset += _off4, _off4),
                        DataHelper.GetAtLocationInt(in data, offset += _off4, _off4)));
                }
            } while (stillChecking);
        }
        //OpenType7/CFF Stuff

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
            if (!table.IndexGlobalSubr.IsBlank)
                table.GlobalSubrCommands = ParseCFFCharStrings(table.IndexGlobalSubr, true);

            if (table.TopDictionaryIndex.Data[0].TryGetValue(Operators.CharStrings, out Operand[] charStringIdx))
            {
                if (charStringIdx.Length == 1)
                    table.IndexCharStrings = ParseCFFIndex(in data, table.StartIndex + charStringIdx[0].IntegerValue);
            }

            table.CharStringCommands = ParseCFFCharStrings(table.IndexCharStrings, false);

            if (table.TopDictionaryIndex.Data[0].TryGetValue(Operators.Private, out Operand[] privateOperands))
            {
                if (privateOperands.Length == 2)
                {
                    table.PrivateDictionary = ParseCFFDictionary(in data, table.StartIndex + privateOperands[1].IntegerValue, privateOperands[0].IntegerValue);
                    if(table.PrivateDictionary.TryGetValue(Operators.Subrs, out Operand[] subrsOperands))
                    {
                        if(subrsOperands.Length == 1)
                        {
                            table.IndexLocalSubr = ParseCFFIndex(in data, table.StartIndex + privateOperands[1].IntegerValue + subrsOperands[0].IntegerValue);
                            table.LocalSubrCommands = ParseCFFCharStrings(table.IndexLocalSubr, true);
                        }
                    }
                }
            }
            if (table.TopDictionaryIndex.Data[0].TryGetValue(Operators.charset, out Operand[] charsetOperands))
            {
                if (charsetOperands.Length == 1)
                    table.Charsets = ParseCFFCharsets(in data, table.CharStringCommands.Length, table.StartIndex + charsetOperands[0].IntegerValue, out newStart);
            }
            var value = table.IndexCharStrings.GetDataAtIndex(1);
            int x = 1;
            return table;
        }

        private void ParseCFFIndexHeader(in byte[] data, int startIndex, out int newStart, out short count, out byte offset)
        {
            newStart = startIndex;
            count = DataHelper.GetAtLocationShort(in data, startIndex, 2);
            if(count > 0)
                offset = data[newStart += _off2];
            else
                offset = 0;

            newStart += _off1;
        }

        private void ParseCFFIndexOffsets(in byte[] data, int startIndex, byte offset, ref int[] offsets, ref int[] offsetGaps, out int newStart)
        {
            newStart = startIndex - offset;
            offsets[0] = DataHelper.GetAtLocationInt(in data, newStart += offset, offset) - 1;
            for (int idx = 0; idx < offsets.Length - 1; idx++)
                offsetGaps[idx] = (offsets[idx + 1] = DataHelper.GetAtLocationInt(in data, newStart += offset, offset) - 1) - offsets[idx];
            newStart += offset;
        }

        private CFFIndex ParseCFFIndex(in byte[] data, int startIndex, out int newStart)
        {
            ParseCFFIndexHeader(in data, startIndex, out startIndex, out short count, out byte offset);
            if (count == 0){
                newStart = startIndex + _off2;
                return new CFFIndex();
            }
            CFFIndex index = new CFFIndex(count, offset);
            ParseCFFIndexOffsets(in data, startIndex, index.Offset, ref index.OffsetsRef, ref index.OffsetGapsRef, out startIndex);
            int size = index.DataSize;
            index.Data = new byte[size];
            for(int idx = 0; idx < index.Data.Length; idx++)
                index.Data[idx] = data[startIndex + idx];
            newStart = startIndex + size;

            return index;
        }

        private CFFIndex ParseCFFIndex(in byte[] data, int startIndex)
        {
            ParseCFFIndexHeader(in data, startIndex, out startIndex, out short count, out byte offset);
            if (count == 0)
                return new CFFIndex();
            CFFIndex index = new CFFIndex(count, offset);
            ParseCFFIndexOffsets(in data, startIndex, index.Offset, ref index.OffsetsRef, ref index.OffsetGapsRef, out startIndex);
            int size = index.DataSize;
            index.Data = new byte[size];
            for (int idx = 0; idx < index.Data.Length; idx++)
                index.Data[idx] = data[startIndex + idx];

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

        private CFFDictionary ParseCFFDictionary(in byte[] data, int startIndex, int dataSize)
        {
            int newStart = startIndex;
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

        private CFFCharStringCommands[] ParseCFFCharStrings(CFFIndex indexCharStrings, bool ignoreEndConditions)
        {
            CFFCharStringCommands[] commands = new CFFCharStringCommands[indexCharStrings.Count];
            int count = 0;
            for(int idx = 0; idx < indexCharStrings.Count; idx++)
            {
                //must begin with one of the following  width (1-5) hstem (2n) hstemhm (2n) vstem (2n) vstemhm (2n) hmoveto (1) vmoveto (1) rrmoveto (2) endchar

                commands[idx] = new CFFCharStringCommands();
                var code = indexCharStrings.GetDataAtIndex(idx);
                if(code.Length == 1 && code[0] == 0x0e) //this is for the space character.  the minimum number of commands in a CharString is 1 and it must be the operand free operator of endchar (0x0e)
                {
                    commands[idx].Add(new CharStringFunction(new CSOperand[0], CSOperators.endchar));
                    continue;
                }
                var dataType = GetCSNumberType(code[0]);
                int dataSize = indexCharStrings.OffsetGaps[idx];
                int startIndex = 0;
                int newStart = startIndex;

                bool inHintSection = true;
                int hintCount = 0;
                int maskByteCount = 0;

                bool isFirstOperator = true;

                //Investigate if there is a way to add highly parallelized, hinted font rendering for gpu applications.

                while ((newStart - startIndex) < dataSize)
                {
                    var operands = ParseCFFOperandsCS(in code, newStart, out newStart);
                    var op = ParseCFFCSOperator(in code, newStart, out newStart);

                    bool isHintOperator = op == CSOperators.hstem || op == CSOperators.vstem || op == CSOperators.hstemhm || op == CSOperators.vstemhm;

                    //REMOVE
                    if (isFirstOperator) //I know that this could be modified and moved outside of the loop, but this isn't a time pressing operation as of now.
                    {
                        bool isValidFirstOperator = isHintOperator || op == CSOperators.hmoveto || op == CSOperators.vmoveto || op == CSOperators.rrcurveto || op == CSOperators.endchar;
                        if (isValidFirstOperator)
                        {
                            CSOperand[] tempOperands = operands.ToArray();
                            switch (op)
                            {
                                case (CSOperators.hstem):
                                    if (operands.Count % 2 == 1)
                                    {
                                        commands[idx].Width = tempOperands[0];
                                        operands.RemoveAt(0);
                                    }
                                    break;
                                case (CSOperators.hstemhm):
                                    if (operands.Count % 2 == 1)
                                    {
                                        commands[idx].Width = tempOperands[0];
                                        operands.RemoveAt(0);
                                    }
                                    break;
                                case (CSOperators.vstem):
                                    if (operands.Count % 2 == 1)
                                    {
                                        commands[idx].Width = tempOperands[0];
                                        operands.RemoveAt(0);
                                    }
                                    break;
                                case (CSOperators.vstemhm):
                                    if (operands.Count % 2 == 1)
                                    {
                                        commands[idx].Width = tempOperands[0];
                                        operands.RemoveAt(0);
                                    }
                                    break;
                                case (CSOperators.hmoveto):
                                    if (operands.Count == 2)
                                    {
                                        commands[idx].Width = tempOperands[0];
                                        operands.RemoveAt(0);
                                    }
                                    break;
                                case (CSOperators.vmoveto):
                                    if (operands.Count == 2)
                                    {
                                        commands[idx].Width = tempOperands[0];
                                        operands.RemoveAt(0);
                                    }
                                    break;
                                case (CSOperators.rmoveto):
                                    if (operands.Count == 3)
                                    {
                                        commands[idx].Width = tempOperands[0];
                                        operands.RemoveAt(0);
                                    }
                                    break;
                                case (CSOperators.endchar):
                                    if (operands.Count == 1)
                                    {
                                        commands[idx].Width = tempOperands[0];
                                        operands.RemoveAt(0);
                                    }
                                    break;
                            }
                            isFirstOperator = false;
                        }
                        else
                            Debug.WriteLine("{0} {1}", idx, op);
                            //throw new Exception("Invalid charstring sequence, must begin with a valid width, hint, move, or endchar.");
                    }
                    //END-REMOVE

                    if (inHintSection)
                    {
                        if (isHintOperator)
                            hintCount++;
                        else
                        {
                            inHintSection = false;
                            maskByteCount = (hintCount % 8 != 0) ? hintCount / 8 + 1: hintCount / 8;
                        }
                    }

                    if (isHintOperator && !inHintSection)
                        throw new Exception("Invalid charstring command stack.  Hint operators can only be declared at the beginning of the sequence.");

                    else if (op == CSOperators.hintmask || op == CSOperators.cntrmask)
                    {
                        long mask1 = DataHelper.GetAtLocationLong(in code, newStart, (maskByteCount < 8) ? maskByteCount : 8, out newStart);
                        int mask2 = DataHelper.GetAtLocationInt(in code, newStart, (maskByteCount > 8) ? maskByteCount - 8 : 0, out newStart);
                        commands[idx].Add(new CharStringFunction(operands.ToArray(), op, mask1, mask2));
                    }
                    else
                        commands[idx].Add(new CharStringFunction(operands.ToArray(), op));
                }

                foreach (CharStringFunction f in commands[idx])
                {
                    if (f.Operator == CSOperators.rturn && commands[idx].ToArray()[commands[idx].ToArray().Length - 1].Operator != CSOperators.rturn)
                        Debug.WriteLine("This is not supposed to happen {0} {1}", idx, ++count);
                }

                //TEST
                if (!ignoreEndConditions)
                {
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

        private CFFCharsets ParseCFFCharsets(in byte[] data, int nGlyphs, int startIndex)
        {
            int newStart = startIndex;
            byte format = data[startIndex];
            CFFCharsets charset = new CFFCharsets(format);
            int idx = 0;
            switch (format)
            {
                case (0):
                    for (newStart = startIndex + 1; newStart < startIndex + 2 * nGlyphs; newStart += 2)
                        charset.Add((short)(data[newStart] << 8 | data[newStart + 1]));
                    break;
                case (1):
                    while (idx < nGlyphs)
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

        //=====================================//
        //           -Feature note-            //
        // Remove all of the parser stuff from //
        // here and add the respective methods //
        // inside of the classes where they    //
        // belong.  For example, ParseCFFIndex //
        // should go inside of the CFFIndex    //
        // class as we can easily pass file    //
        // data around.  It does not have to   //
        // be stored, only referenced (in).    //
        // Since we're going for a an approach //
        // of strongly partitioned methods and //
        // heavily divided classes (as one     //
        // should), we will keep FontManager   //
        // small in size and focused in its    //
        // functions.  This same philsophy     //
        // should be applied in our other      //
        // parsers.                            //
        // Additionally, moving all of these   //
        // functions out of here will make     //
        // future TrueType implementation much //
        // easier and visually more appealing  //
        // to look at.                         //
        // Keep FontManager at simply loading  //
        // and storing fonts.  It initiates    //
        // the parsing routines by creating a  //
        // new Font object with the specified  //
        // directory.  The Font object handles //
        // the parsing operations.             //
        // For now, we will keep CFF functions //
        // here purely for debugging purposes. //
        //=====================================//

        //=====================================//
        //        -Point of Uncertainy-        //
        // There some mysterious behavior in   //
        // the global subroutines.  There is a //
        // mysterious global subroutine that   //
        // begins WITH an operator and is then //
        // followed by normal operands and     //
        // operators.  This is not the case    //
        // for all global subroutines.  My     //
        // hypothesis is that it may have      //
        // something to do with how they are   //
        // added to the stack of commands.     //
        // More specifically, these may be     //
        // added to the stack in such a way    //
        // that operands are passed from the   //
        // primary charstring command list or  //
        // the subroutine is appended directly //
        // after the raw binary of the         //
        // charstring commands.  If true, then //
        // the methods of parsing charstring   //
        // commands will need to be different. //
        // It may become the case that global  //
        // subroutines are parsed into the     //
        // charstrings upon the initial parse  //
        // so as to avoid subroutines at all.  //
        // If this were the case, then this    //
        // should not cause issues since we    //
        // not drawing fonts directly from the //
        // code and most vector font and bmp   //
        // font data will be baked.            //
        //=====================================//

        //====================================//
        //             -Addendum-             //
        // We need to add Geometry to VAO     //
        // functionality.  This will require  //
        // interpreting the functions and     //
        // in the case of baked geometry,     //
        // creating it on the CPU.  When we   //
        // have baked textures, we bake on    //
        // the CPU and render on the GPU      //
        // When we have adaptive font vectors //
        // we need to upload custom VAO data  //
        // to the GPU and have it utilize     //
        // custom tessellation shaders to add //
        // just the right level of detail     //
        // for the expected size on the       //
        // display.                           //
        //====================================//

        //===================================//
        //             -Hinting-             //
        // The hinting given by the hintmask //
        // operation seems to be followed by //
        // a bit mask.  We neeed to parse    //
        // this bit mask as it seems to be   //
        // causing rogue return and zero     //
        // functions that directly follow    //
        // hintmask.  This should be easily  //
        // fixable.      Done.               //
        //===================================//
    }
}
