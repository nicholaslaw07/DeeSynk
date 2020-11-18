using System;
using System.Collections.Generic;
using System.Text;

namespace DeeSynk.Core.Components.Fonts
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

        public CharStringFunction(CSOperators op, CSOperand[] operands)
        {
            _operator = op;
            _operands = operands;
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
        public CFFCharStringCommands() : base(){}
    }
}
