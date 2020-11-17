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
        hflex = 0x0c22,
        flex = 0x0c23,
        hflex1 = 0x0c24,
        flex1 = 0x0c25
    }

    public class CharStringOperation
    {
        private CSOperators _operator;
        public CSOperators Operator { get => _operator; }

        private CSOperand[] _operands;
        public CSOperand[] Operands { get => _operands; }

        public CharStringOperation(CSOperators op, CSOperand[] operands)
        {
            _operator = op;
            _operands = operands;
        }
    }
    public class CFFCharStringCommands : List<CharStringOperation>
    {
        public CFFCharStringCommands() : base(){}
    }
}
