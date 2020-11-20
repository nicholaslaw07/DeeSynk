using System;
using System.Collections.Generic;
using System.Text;

namespace DeeSynk.Core.Components.Fonts.Tables.CFF
{
    public enum CSOperandNumberTypes : short
    {
        UNDEFINED = 0,
        Short = 1,  //compact byte values
        Fixed = 2  //nibbling
    }

    public struct CSOperand
    {
        private CSOperandNumberTypes _numberType;
        public CSOperandNumberTypes NumberType { get => _numberType; }

        private short _shortValue;
        public short ShortValue
        {
            get
            {
                //if (_numberType == CSOperandNumberTypes.Short)
                    return _shortValue;
                //else
                //    throw new FieldAccessException("Read operation does not match the value type for this Operand.  Expected a call to RealValue.");
            }
        }

        private float _fixedValue;
        public float FixedValue
        {
            get
            {
                //if (_numberType == CSOperandNumberTypes.Fixed)
                    return _fixedValue;
                //else
                //    throw new FieldAccessException("Read operation does not match the value type for this Operand.  Expected a call to IntegerValue.");
            }
        }

        public CSOperand(CSOperandNumberTypes type, short s, float f)
        {
            _numberType = type;
            _shortValue = s;
            _fixedValue = f;
        }

        public override string ToString()
        {
            return $"[{_numberType}  {_shortValue}  {_fixedValue}]";
        }
    }
}
