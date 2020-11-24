using System;
using System.Collections.Generic;
using System.Text;

namespace DeeSynk.Core.Components.Fonts.Tables.CFF
{
    public struct CSOperatorOperandSpec
    {
        //Add a compact but readable way to represent the operand requirements for each operator.
        //This will require require and optional arguments
        //Repeated operators by repeated argument multiples (rrcurveto) or additions beyond a minimum count (hvlineto)
        //Specify if operand additions are additive atomically or in groups
        //Specify location of the operands

        //In the future, add direct method linkages for the operators and C# methods (mostly for cff to raw vector geometry conversion)
        //This will be included in a seperate geometry constructor class.  (static)
    }
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
    }
}
