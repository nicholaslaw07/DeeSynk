using System;
using System.Collections.Generic;
using System.Text;

namespace DeeSynk.Core.Components.Fonts.Tables.CFF
{
    public enum OperandNumberTypes : short
    {
        UNDEFINED = 0,
        Integer = 1,  //compact byte values
        Real = 2  //nibbling
    }

    public class Operand
    {
        private OperandNumberTypes _numberType;
        public OperandNumberTypes NumberType { get => _numberType; }

        private int _intValue;
        public int IntegerValue
        {
            get
            {
                if (_numberType == OperandNumberTypes.Integer)
                    return _intValue;
                else
                    throw new FieldAccessException("Read operation does not match the value type for this Operand.  Expected a call to RealValue.");
            }
        }

        private double _realValue;
        public double RealValue
        {
            get
            {
                if (_numberType == OperandNumberTypes.Real)
                    return _realValue;
                else
                    throw new FieldAccessException("Read operation does not match the value type for this Operand.  Expected a call to IntegerValue.");
            }
        }

        public Operand()
        {
            _numberType = OperandNumberTypes.UNDEFINED;
        }

        public Operand(int val)
        {
            _numberType = OperandNumberTypes.Integer;
            _intValue = val;
        }

        public Operand(double val)
        {
            _numberType = OperandNumberTypes.Real;
            _realValue = val;
        }
    }
}
