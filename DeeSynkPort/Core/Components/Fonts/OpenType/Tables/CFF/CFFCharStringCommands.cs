using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace DeeSynk.Core.Components.Fonts.Tables.CFF
{
    public enum CSOperators : short
    {
        hstem = 0x01,
        vstem = 0x03,
        vmoveto = 0x04,
        rlineto = 0x05,
        hlineto = 0x06,
        vlineto = 0x07,
        rrcurveto = 0x08,
        callsubr = 0x0a,
        rturn = 0x0b,
        escape = 0x0c,
        endchar = 0x0e,
        vsindex = 0x0f,
        blend = 0x10,
        hstemhm = 0x12,
        hintmask = 0x13,
        cntrmask = 0x14,
        rmoveto = 0x15,
        hmoveto = 0x16,
        vstemhm = 0x17,
        rcurveline = 0x18,
        rlinecurve = 0x19,
        vvcureto = 0x1a,
        hhcurveto = 0x1b,
        //numbers = 0x1c 3byte unsigned integer
        //note how 0x1d is not available.  this means that 32-bit signed integers cannot be stored / registered
        callgsubr = 0x1d,
        vhcurveto = 0x1e,
        hvcurveto = 0x1f,
        //numbers

        //two-byte
        and = 0x0c03,
        or = 0x0c04,
        not = 0x0c05,
        abs = 0x0c09,
        add = 0x0c0a,
        sub = 0x0c0b,
        div = 0x0c0c,
        neg = 0x0c0e,
        eq = 0x0c0f,
        drop = 0x0c12,
        put = 0x0c14,
        get = 0x0c15,
        ifelse = 0x0c16,
        random = 0x0c17,
        mul = 0x0c18,
        sqrt = 0x0c1a,
        dup = 0x0c1b,
        exch = 0x0c1c,
        index = 0x0c1d,
        roll = 0x0c1e,
        hflex = 0x0c22,
        flex = 0x0c23,
        hflex1 = 0x0c24,
        flex1 = 0x0c25
    }

    public class CharStringFunction
    {

        private CSOperators _operator;
        public CSOperators Operator { get => _operator; }

        private CSOperand[] _operands;
        public CSOperand[] Operands { get => _operands; }

        //this is only ever used for the hintmask and cntrmask operators

        private long _mask1;
        /// <summary>
        /// The first 64 bits of the maximum 96-bit hint mask.
        /// </summary>
        public long Mask1 { get => _mask1; }

        private int _mask2;
        /// <summary>
        /// The last 32 bits of the maximum 96-bit hint mask.
        /// </summary>
        public int Mask2 { get => _mask2; }

        public bool HasValidMask { get => (_operator == CSOperators.hintmask || _operator == CSOperators.cntrmask) && _mask1 != 0 || _mask2 != 0; }

        public CharStringFunction(CSOperand[] operands, CSOperators op, long mask1 = 0, int mask2 = 0)
        {
            _operands = operands;
            _operator = op;
            _mask1 = mask1;
            _mask2 = mask2;
        }

        public override string ToString()
        {
            string outVal = _operator.ToString();
            foreach (CSOperand c in _operands)
                outVal += " " + c.ToString();
            return outVal;
        }
    }
    public class CFFCharStringCommands : List<CharStringFunction>
    {
        private CSOperand _width;
        public CSOperand Width { get => _width; set => _width = value; }

        public bool HasWidthOverride { get => (_width.ShortValue > -1 && _width.NumberType == CSOperandNumberTypes.Short) || (_width.FixedValue > -1 && _width.NumberType == CSOperandNumberTypes.Fixed); }

        public CFFCharStringCommands() : base(){ _width = new CSOperand(CSOperandNumberTypes.Short, -1, -1); }

        public static CFFCharStringCommands[] ParseCharStrings(CFFIndex indexCharStrings, bool ignoreStartConditions, bool ignoreEndConditions)
        {
            CFFCharStringCommands[] commands = new CFFCharStringCommands[indexCharStrings.Count];
            int count = 0;
            for (int idx = 0; idx < indexCharStrings.Count; idx++)
            {
                //must begin with one of the following  width (1-5) hstem (2n) hstemhm (2n) vstem (2n) vstemhm (2n) hmoveto (1) vmoveto (1) rmoveto (2) endchar

                commands[idx] = new CFFCharStringCommands();
                var code = indexCharStrings.GetDataAtIndex(idx);
                if (code.Length == 1 && code[0] == 0x0e) //this is for the space character.  the minimum number of commands in a CharString is 1 and it must be the operand free operator of endchar (0x0e)
                {
                    commands[idx].Add(new CharStringFunction(new CSOperand[0], CSOperators.endchar));
                    continue;
                }
                var dataType = GetNumberType(code[0]);
                int dataSize = indexCharStrings.OffsetGaps[idx];
                int startIndex = 0;
                int newStart = startIndex;

                bool inHintSection = true;
                int hintCount = 0;
                int maskByteCount = 0;

                bool isFirstOperator = !ignoreStartConditions;
                //bool isFirstOperator = true;

                //Investigate if there is a way to add highly parallelized, hinted font rendering for gpu applications.

                while ((newStart - startIndex) < dataSize)
                {
                    var operands = ParseOperands(in code, newStart, out newStart);
                    var op = ParseOperator(in code, newStart, out newStart);

                    bool isHintOperator = op == CSOperators.hstem || op == CSOperators.vstem || op == CSOperators.hstemhm || op == CSOperators.vstemhm;

                    if (inHintSection)
                    {
                        if (isHintOperator)
                            hintCount++;
                        else
                        {
                            inHintSection = false;
                            maskByteCount = (hintCount % 8 != 0) ? hintCount / 8 + 1 : hintCount / 8;
                        }
                    }
                    else if (isHintOperator)
                        throw new Exception("Invalid charstring command stack.  Hint operators can only be declared at the beginning of the sequence.");

                    if (isFirstOperator) //I know that this could be modified and moved outside of the loop, but this isn't a time pressing operation as of now.
                    {
                        bool isValidFirstOperator = isHintOperator || op == CSOperators.hmoveto || op == CSOperators.vmoveto || op == CSOperators.rmoveto || op == CSOperators.endchar;
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
                        }
                        else
                            Debug.WriteLine("{0} {1}", idx, op);
                        //    throw new Exception("Invalid charstring sequence, must begin with a valid width, hint, move, or endchar.");

                        isFirstOperator = false;
                        commands[idx].Add(new CharStringFunction(operands.ToArray(), op));
                    }
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

        private static bool ValidNumberStart(byte b0)
        {
            return (b0 >= 0x20 && b0 <= 0xff) || (b0 == 0x1c);
        }

        private static CSOperandNumberTypes GetNumberType(byte b0)
        {
            switch (b0)
            {
                case byte b when ((b >= 0x20 && b <= 0xfe) || (b == 0x1c)): return CSOperandNumberTypes.Short;
                case (0xff): return CSOperandNumberTypes.Fixed;
                default: return CSOperandNumberTypes.UNDEFINED;
            }
        }

        private static List<CSOperand> ParseOperands(in byte[] data, int startIndex, out int i)
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

        private static CSOperators ParseOperator(in byte[] data, int startIndex, out int newStart)
        {
            newStart = startIndex + ((data[startIndex] == 0xc) ? 2 : 1);
            return (CSOperators)((data[startIndex] == 0xc) ? (short)(data[startIndex] << 8 | data[startIndex + 1]) : data[startIndex]);
        }
    }
}
