using System;
using System.Collections.Generic;
using System.Text;

namespace DeeSynk.Core.Components.Fonts
{
    public enum CSOperandNumberTypes : short
    {
        UNDEFINED = 0,
        Short = 1,  //compact byte values
        Fixed = 2  //nibbling
    }

    public class CSOperand
    {
        private CSOperandNumberTypes _numberType;
        public CSOperandNumberTypes NumberType { get => _numberType; }

        private int _intValue;
        public int IntegerValue
        {
            get
            {
                if (_numberType == CSOperandNumberTypes.Short)
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
                if (_numberType == CSOperandNumberTypes.Fixed)
                    return _realValue;
                else
                    throw new FieldAccessException("Read operation does not match the value type for this Operand.  Expected a call to IntegerValue.");
            }
        }

        public CSOperand()
        {
            _numberType = CSOperandNumberTypes.UNDEFINED;
        }

        public CSOperand(short val)
        {
            _numberType = CSOperandNumberTypes.Short;
            _intValue = val;
        }

        public CSOperand(float val)
        {
            _numberType = CSOperandNumberTypes.Fixed;
            _realValue = val;
        }
    }
}
