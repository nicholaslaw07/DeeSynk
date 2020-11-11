using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DeeSynk.Core.Components.Types;

namespace DeeSynk.Core.Collections
{
    public abstract class DataGroup
    {
        protected int _dataCount;
        public int DATA_COUNT { get => _dataCount; }

        public abstract IComponent DataAtIndexByType<T>(int index) where T : IComponent;
        public abstract IComponent DataAtIndexByArray(int index, int arrayID);
    }
    public class DataGroup<T0> : DataGroup
                                                        where T0 : IComponent
    {
        public const int ARRAY_COUNT = 1;

        private T0[] _componentGroup0;

        public DataGroup(int dataCount)
        {
            _dataCount = dataCount;

            _componentGroup0 = new T0[_dataCount];
        }

        public override IComponent DataAtIndexByType<T>(int index)
        {
            if (index >= 0 && index < _dataCount)
            {
                Type typeT = typeof(T);

                if (typeT == typeof(T0)) return _componentGroup0[index];
            }
            return null;
        }

        public override IComponent DataAtIndexByArray(int index, int arrayID)
        {
            if (index >= 0 && index < _dataCount)
            {
                if (arrayID >= 0 && arrayID < ARRAY_COUNT)
                {
                    switch (arrayID)
                    {
                        case (0): return _componentGroup0[index];

                        default: return null;
                    }
                }
            }

            return null;
        }
    }

    public class DataGroup<T0, T1> : DataGroup
                                                        where T0 : IComponent
                                                        where T1 : IComponent
    {
        public const int ARRAY_COUNT = 2;

        private T0[] _componentGroup0;
        private T1[] _componentGroup1;

        public DataGroup(int dataCount)
        {
            _dataCount = dataCount;

            _componentGroup0 = new T0[_dataCount];
            _componentGroup1 = new T1[_dataCount];
        }

        public override IComponent DataAtIndexByType<T>(int index)
        {
            if (index >= 0 && index < _dataCount)
            {
                Type typeT = typeof(T);

                if (typeT == typeof(T0)) return _componentGroup0[index];
                else if (typeT == typeof(T1)) return _componentGroup1[index];
            }
            return null;
        }

        public override IComponent DataAtIndexByArray(int index, int arrayID)
        {
            if (index >= 0 && index < _dataCount)
            {
                if (arrayID >= 0 && arrayID < ARRAY_COUNT)
                {
                    switch (arrayID)
                    {
                        case (0): return _componentGroup0[index];
                        case (1): return _componentGroup1[index];

                        default: return null;
                    }
                }
            }

            return null;
        }
    }

    public class DataGroup<T0, T1, T2> : DataGroup
                                                        where T0 : IComponent
                                                        where T1 : IComponent
                                                        where T2 : IComponent
    {
        public const int ARRAY_COUNT = 3;

        private T0[] _componentGroup0;
        private T1[] _componentGroup1;
        private T2[] _componentGroup2;

        public DataGroup(int dataCount)
        {
            _dataCount = dataCount;

            _componentGroup0 = new T0[_dataCount];
            _componentGroup1 = new T1[_dataCount];
            _componentGroup2 = new T2[_dataCount];
        }

        public override IComponent DataAtIndexByType<T>(int index)
        {
            if (index >= 0 && index < _dataCount)
            {
                Type typeT = typeof(T);

                if (typeT == typeof(T0)) return _componentGroup0[index];
                else if (typeT == typeof(T1)) return _componentGroup1[index];
                else if (typeT == typeof(T2)) return _componentGroup2[index];
            }
            return null;
        }

        public override IComponent DataAtIndexByArray(int index, int arrayID)
        {
            if (index >= 0 && index < _dataCount)
            {
                if (arrayID >= 0 && arrayID < ARRAY_COUNT)
                {
                    switch (arrayID)
                    {
                        case (0): return _componentGroup0[index];
                        case (1): return _componentGroup1[index];
                        case (2): return _componentGroup2[index];

                        default: return null;
                    }
                }
            }

            return null;
        }
    }

    public class DataGroup<T0, T1, T2, T3> : DataGroup
                                                        where T0 : IComponent
                                                        where T1 : IComponent
                                                        where T2 : IComponent
                                                        where T3 : IComponent
    {
        public const int ARRAY_COUNT = 4;

        private T0[] _componentGroup0;
        private T1[] _componentGroup1;
        private T2[] _componentGroup2;
        private T3[] _componentGroup3;

        public DataGroup(int dataCount)
        {
            _dataCount = dataCount;

            _componentGroup0 = new T0[_dataCount];
            _componentGroup1 = new T1[_dataCount];
            _componentGroup2 = new T2[_dataCount];
            _componentGroup3 = new T3[_dataCount];

        }

        public override IComponent DataAtIndexByType<T>(int index)
        {
            if (index >= 0 && index < _dataCount)
            {
                Type typeT = typeof(T);

                if (typeT == typeof(T0)) return _componentGroup0[index];
                else if (typeT == typeof(T1)) return _componentGroup1[index];
                else if (typeT == typeof(T2)) return _componentGroup2[index];
                else if (typeT == typeof(T3)) return _componentGroup3[index];
            }
            return null;
        }

        public override IComponent DataAtIndexByArray(int index, int arrayID)
        {
            if (index >= 0 && index < _dataCount)
            {
                if (arrayID >= 0 && arrayID < ARRAY_COUNT)
                {
                    switch (arrayID)
                    {
                        case (0): return _componentGroup0[index];
                        case (1): return _componentGroup1[index];
                        case (2): return _componentGroup2[index];
                        case (3): return _componentGroup3[index];

                        default: return null;
                    }
                }
            }

            return null;
        }
    }

    public class DataGroup<T0, T1, T2, T3, T4> : DataGroup
                                                        where T0 : IComponent
                                                        where T1 : IComponent
                                                        where T2 : IComponent
                                                        where T3 : IComponent
                                                        where T4 : IComponent
    {
        public const int ARRAY_COUNT = 5;

        private T0[] _componentGroup0;
        private T1[] _componentGroup1;
        private T2[] _componentGroup2;
        private T3[] _componentGroup3;
        private T4[] _componentGroup4;

        public DataGroup(int dataCount)
        {
            _dataCount = dataCount;

            _componentGroup0 = new T0[_dataCount];
            _componentGroup1 = new T1[_dataCount];
            _componentGroup2 = new T2[_dataCount];
            _componentGroup3 = new T3[_dataCount];
            _componentGroup4 = new T4[_dataCount];
        }

        public override IComponent DataAtIndexByType<T>(int index)
        {
            if (index >= 0 && index < _dataCount)
            {
                Type typeT = typeof(T);

                if (typeT == typeof(T0)) return _componentGroup0[index];
                else if (typeT == typeof(T1)) return _componentGroup1[index];
                else if (typeT == typeof(T2)) return _componentGroup2[index];
                else if (typeT == typeof(T3)) return _componentGroup3[index];
                else if (typeT == typeof(T4)) return _componentGroup4[index];
            }
            return null;
        }

        public override IComponent DataAtIndexByArray(int index, int arrayID)
        {
            if (index >= 0 && index < _dataCount)
            {
                if (arrayID >= 0 && arrayID < ARRAY_COUNT)
                {
                    switch (arrayID)
                    {
                        case (0): return _componentGroup0[index];
                        case (1): return _componentGroup1[index];
                        case (2): return _componentGroup2[index];
                        case (3): return _componentGroup3[index];
                        case (4): return _componentGroup4[index];

                        default: return null;
                    }
                }
            }

            return null;
        }
    }

    public class DataGroup<T0, T1, T2, T3, T4, T5> : DataGroup
                                                        where T0 : IComponent
                                                        where T1 : IComponent
                                                        where T2 : IComponent
                                                        where T3 : IComponent
                                                        where T4 : IComponent
                                                        where T5 : IComponent
    {
        public const int ARRAY_COUNT = 6;

        private T0[] _componentGroup0;
        private T1[] _componentGroup1;
        private T2[] _componentGroup2;
        private T3[] _componentGroup3;
        private T4[] _componentGroup4;
        private T5[] _componentGroup5;

        public DataGroup(int dataCount)
        {
            _dataCount = dataCount;

            _componentGroup0 = new T0[_dataCount];
            _componentGroup1 = new T1[_dataCount];
            _componentGroup2 = new T2[_dataCount];
            _componentGroup3 = new T3[_dataCount];
            _componentGroup4 = new T4[_dataCount];
            _componentGroup5 = new T5[_dataCount];
        }

        public override IComponent DataAtIndexByType<T>(int index)
        {
            if (index >= 0 && index < _dataCount)
            {
                Type typeT = typeof(T);

                if (typeT == typeof(T0)) return _componentGroup0[index];
                else if (typeT == typeof(T1)) return _componentGroup1[index];
                else if (typeT == typeof(T2)) return _componentGroup2[index];
                else if (typeT == typeof(T3)) return _componentGroup3[index];
                else if (typeT == typeof(T4)) return _componentGroup4[index];
                else if (typeT == typeof(T5)) return _componentGroup5[index];
            }
            return null;
        }

        public override IComponent DataAtIndexByArray(int index, int arrayID)
        {
            if (index >= 0 && index < _dataCount)
            {
                if (arrayID >= 0 && arrayID < ARRAY_COUNT)
                {
                    switch (arrayID)
                    {
                        case (0): return _componentGroup0[index];
                        case (1): return _componentGroup1[index];
                        case (2): return _componentGroup2[index];
                        case (3): return _componentGroup3[index];
                        case (4): return _componentGroup4[index];
                        case (5): return _componentGroup5[index];

                        default: return null;
                    }
                }
            }

            return null;
        }
    }

    public class DataGroup<T0, T1, T2, T3, T4, T5, T6> : DataGroup
                                                        where T0 : IComponent
                                                        where T1 : IComponent
                                                        where T2 : IComponent
                                                        where T3 : IComponent
                                                        where T4 : IComponent
                                                        where T5 : IComponent
                                                        where T6 : IComponent
    {
        public const int ARRAY_COUNT = 7;

        private T0[] _componentGroup0;
        private T1[] _componentGroup1;
        private T2[] _componentGroup2;
        private T3[] _componentGroup3;
        private T4[] _componentGroup4;
        private T5[] _componentGroup5;
        private T6[] _componentGroup6;

        public DataGroup(int dataCount)
        {
            _dataCount = dataCount;

            _componentGroup0 = new T0[_dataCount];
            _componentGroup1 = new T1[_dataCount];
            _componentGroup2 = new T2[_dataCount];
            _componentGroup3 = new T3[_dataCount];
            _componentGroup4 = new T4[_dataCount];
            _componentGroup5 = new T5[_dataCount];
            _componentGroup6 = new T6[_dataCount];
        }

        public override IComponent DataAtIndexByType<T>(int index)
        {
            if (index >= 0 && index < _dataCount)
            {
                Type typeT = typeof(T);

                if (typeT == typeof(T0)) return _componentGroup0[index];
                else if (typeT == typeof(T1)) return _componentGroup1[index];
                else if (typeT == typeof(T2)) return _componentGroup2[index];
                else if (typeT == typeof(T3)) return _componentGroup3[index];
                else if (typeT == typeof(T4)) return _componentGroup4[index];
                else if (typeT == typeof(T5)) return _componentGroup5[index];
                else if (typeT == typeof(T6)) return _componentGroup6[index];
            }
            return null;
        }

        public override IComponent DataAtIndexByArray(int index, int arrayID)
        {
            if (index >= 0 && index < _dataCount)
            {
                if (arrayID >= 0 && arrayID < ARRAY_COUNT)
                {
                    switch (arrayID)
                    {
                        case (0): return _componentGroup0[index];
                        case (1): return _componentGroup1[index];
                        case (2): return _componentGroup2[index];
                        case (3): return _componentGroup3[index];
                        case (4): return _componentGroup4[index];
                        case (5): return _componentGroup5[index];
                        case (6): return _componentGroup6[index];

                        default: return null;
                    }
                }
            }

            return null;
        }
    }

    public class DataGroup<T0, T1, T2, T3, T4, T5, T6, T7>  : DataGroup
                                                            where T0 : IComponent
                                                            where T1 : IComponent 
                                                            where T2 : IComponent
                                                            where T3 : IComponent
                                                            where T4 : IComponent
                                                            where T5 : IComponent
                                                            where T6 : IComponent
                                                            where T7 : IComponent
    {
        public const int ARRAY_COUNT = 8;

        private T0[] _componentGroup0;
        private T1[] _componentGroup1;
        private T2[] _componentGroup2;
        private T3[] _componentGroup3;
        private T4[] _componentGroup4;
        private T5[] _componentGroup5;
        private T6[] _componentGroup6;
        private T7[] _componentGroup7;

        public DataGroup(int dataCount)
        {
            _dataCount = dataCount;

            _componentGroup0 = new T0[_dataCount];
            _componentGroup1 = new T1[_dataCount];
            _componentGroup2 = new T2[_dataCount];
            _componentGroup3 = new T3[_dataCount];
            _componentGroup4 = new T4[_dataCount];
            _componentGroup5 = new T5[_dataCount];
            _componentGroup6 = new T6[_dataCount];
            _componentGroup7 = new T7[_dataCount];
        }

        public override IComponent DataAtIndexByType<T>(int index)
        {
            if(index >= 0 && index < _dataCount)
            {
                Type typeT = typeof(T);

                     if (typeT == typeof(T0)) return _componentGroup0[index];
                else if (typeT == typeof(T1)) return _componentGroup1[index];
                else if (typeT == typeof(T2)) return _componentGroup2[index];
                else if (typeT == typeof(T3)) return _componentGroup3[index];
                else if (typeT == typeof(T4)) return _componentGroup4[index];
                else if (typeT == typeof(T5)) return _componentGroup5[index];
                else if (typeT == typeof(T6)) return _componentGroup6[index];
                else if (typeT == typeof(T7)) return _componentGroup7[index];
            }
            return null;
        }

        public override IComponent DataAtIndexByArray(int index, int arrayID)
        {
            if (index >= 0 && index < _dataCount)
            {
                if(arrayID >= 0 && arrayID < ARRAY_COUNT)
                {
                    switch (arrayID)
                    {
                        case (0): return _componentGroup0[index];
                        case (1): return _componentGroup1[index];
                        case (2): return _componentGroup2[index];
                        case (3): return _componentGroup3[index];
                        case (4): return _componentGroup4[index];
                        case (5): return _componentGroup5[index];
                        case (6): return _componentGroup6[index];
                        case (7): return _componentGroup7[index];

                        default:  return null;
                    }
                }
            }

            return null;
        }
    }

    public class DataGroup<T0, T1, T2, T3, T4, T5, T6, T7, T8> : DataGroup
                                                                where T0 : IComponent
                                                                where T1 : IComponent
                                                                where T2 : IComponent
                                                                where T3 : IComponent
                                                                where T4 : IComponent
                                                                where T5 : IComponent
                                                                where T6 : IComponent
                                                                where T7 : IComponent
                                                                where T8 : IComponent
    {
        public const int ARRAY_COUNT = 9;

        private T0[] _componentGroup0;
        private T1[] _componentGroup1;
        private T2[] _componentGroup2;
        private T3[] _componentGroup3;
        private T4[] _componentGroup4;
        private T5[] _componentGroup5;
        private T6[] _componentGroup6;
        private T7[] _componentGroup7;
        private T8[] _componentGroup8;

        public DataGroup(int dataCount)
        {
            _dataCount = dataCount;

            _componentGroup0 = new T0[_dataCount];
            _componentGroup1 = new T1[_dataCount];
            _componentGroup2 = new T2[_dataCount];
            _componentGroup3 = new T3[_dataCount];
            _componentGroup4 = new T4[_dataCount];
            _componentGroup5 = new T5[_dataCount];
            _componentGroup6 = new T6[_dataCount];
            _componentGroup7 = new T7[_dataCount];
            _componentGroup8 = new T8[_dataCount];
        }

        public override IComponent DataAtIndexByType<T>(int index)
        {
            if (index >= 0 && index < _dataCount)
            {
                Type typeT = typeof(T);

                if (typeT == typeof(T0)) return _componentGroup0[index];
                else if (typeT == typeof(T1)) return _componentGroup1[index];
                else if (typeT == typeof(T2)) return _componentGroup2[index];
                else if (typeT == typeof(T3)) return _componentGroup3[index];
                else if (typeT == typeof(T4)) return _componentGroup4[index];
                else if (typeT == typeof(T5)) return _componentGroup5[index];
                else if (typeT == typeof(T6)) return _componentGroup6[index];
                else if (typeT == typeof(T7)) return _componentGroup7[index];
                else if (typeT == typeof(T8)) return _componentGroup8[index];
            }
            return null;
        }

        public override IComponent DataAtIndexByArray(int index, int arrayID)
        {
            if (index >= 0 && index < _dataCount)
            {
                if (arrayID >= 0 && arrayID < ARRAY_COUNT)
                {
                    switch (arrayID)
                    {
                        case (0): return _componentGroup0[index];
                        case (1): return _componentGroup1[index];
                        case (2): return _componentGroup2[index];
                        case (3): return _componentGroup3[index];
                        case (4): return _componentGroup4[index];
                        case (5): return _componentGroup5[index];
                        case (6): return _componentGroup6[index];
                        case (7): return _componentGroup7[index];
                        case (8): return _componentGroup8[index];

                        default: return null;
                    }
                }
            }

            return null;
        }
    }

    public class DataGroup<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9> : DataGroup
                                                                where T0 : IComponent
                                                                where T1 : IComponent
                                                                where T2 : IComponent
                                                                where T3 : IComponent
                                                                where T4 : IComponent
                                                                where T5 : IComponent
                                                                where T6 : IComponent
                                                                where T7 : IComponent
                                                                where T8 : IComponent
                                                                where T9 : IComponent
    {
        public const int ARRAY_COUNT = 10;

        private T0[] _componentGroup0;
        private T1[] _componentGroup1;
        private T2[] _componentGroup2;
        private T3[] _componentGroup3;
        private T4[] _componentGroup4;
        private T5[] _componentGroup5;
        private T6[] _componentGroup6;
        private T7[] _componentGroup7;
        private T8[] _componentGroup8;
        private T9[] _componentGroup9;

        public DataGroup(int dataCount)
        {
            _dataCount = dataCount;

            _componentGroup0 = new T0[_dataCount];
            _componentGroup1 = new T1[_dataCount];
            _componentGroup2 = new T2[_dataCount];
            _componentGroup3 = new T3[_dataCount];
            _componentGroup4 = new T4[_dataCount];
            _componentGroup5 = new T5[_dataCount];
            _componentGroup6 = new T6[_dataCount];
            _componentGroup7 = new T7[_dataCount];
            _componentGroup8 = new T8[_dataCount];
            _componentGroup9 = new T9[_dataCount];
        }

        public override IComponent DataAtIndexByType<T>(int index)
        {
            if (index >= 0 && index < _dataCount)
            {
                Type typeT = typeof(T);

                if (typeT == typeof(T0)) return _componentGroup0[index];
                else if (typeT == typeof(T1)) return _componentGroup1[index];
                else if (typeT == typeof(T2)) return _componentGroup2[index];
                else if (typeT == typeof(T3)) return _componentGroup3[index];
                else if (typeT == typeof(T4)) return _componentGroup4[index];
                else if (typeT == typeof(T5)) return _componentGroup5[index];
                else if (typeT == typeof(T6)) return _componentGroup6[index];
                else if (typeT == typeof(T7)) return _componentGroup7[index];
                else if (typeT == typeof(T8)) return _componentGroup8[index];
                else if (typeT == typeof(T9)) return _componentGroup9[index];
            }
            return null;
        }

        public override IComponent DataAtIndexByArray(int index, int arrayID)
        {
            if (index >= 0 && index < _dataCount)
            {
                if (arrayID >= 0 && arrayID < ARRAY_COUNT)
                {
                    switch (arrayID)
                    {
                        case (0): return _componentGroup0[index];
                        case (1): return _componentGroup1[index];
                        case (2): return _componentGroup2[index];
                        case (3): return _componentGroup3[index];
                        case (4): return _componentGroup4[index];
                        case (5): return _componentGroup5[index];
                        case (6): return _componentGroup6[index];
                        case (7): return _componentGroup7[index];
                        case (8): return _componentGroup8[index];
                        case (9): return _componentGroup9[index];

                        default: return null;
                    }
                }
            }

            return null;
        }
    }

    public class DataGroup<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> : DataGroup
                                                                where T0 : IComponent
                                                                where T1 : IComponent
                                                                where T2 : IComponent
                                                                where T3 : IComponent
                                                                where T4 : IComponent
                                                                where T5 : IComponent
                                                                where T6 : IComponent
                                                                where T7 : IComponent
                                                                where T8 : IComponent
                                                                where T9 : IComponent
                                                                where T10 : IComponent
    {
        public const int ARRAY_COUNT = 11;

        private T0[] _componentGroup0;
        private T1[] _componentGroup1;
        private T2[] _componentGroup2;
        private T3[] _componentGroup3;
        private T4[] _componentGroup4;
        private T5[] _componentGroup5;
        private T6[] _componentGroup6;
        private T7[] _componentGroup7;
        private T8[] _componentGroup8;
        private T9[] _componentGroup9;
        private T10[] _componentGroup10;

        public DataGroup(int dataCount)
        {
            _dataCount = dataCount;

            _componentGroup0 = new T0[_dataCount];
            _componentGroup1 = new T1[_dataCount];
            _componentGroup2 = new T2[_dataCount];
            _componentGroup3 = new T3[_dataCount];
            _componentGroup4 = new T4[_dataCount];
            _componentGroup5 = new T5[_dataCount];
            _componentGroup6 = new T6[_dataCount];
            _componentGroup7 = new T7[_dataCount];
            _componentGroup8 = new T8[_dataCount];
            _componentGroup9 = new T9[_dataCount];
            _componentGroup10 = new T10[_dataCount];
        }

        public override IComponent DataAtIndexByType<T>(int index)
        {
            if (index >= 0 && index < _dataCount)
            {
                Type typeT = typeof(T);

                if (typeT == typeof(T0)) return _componentGroup0[index];
                else if (typeT == typeof(T1)) return _componentGroup1[index];
                else if (typeT == typeof(T2)) return _componentGroup2[index];
                else if (typeT == typeof(T3)) return _componentGroup3[index];
                else if (typeT == typeof(T4)) return _componentGroup4[index];
                else if (typeT == typeof(T5)) return _componentGroup5[index];
                else if (typeT == typeof(T6)) return _componentGroup6[index];
                else if (typeT == typeof(T7)) return _componentGroup7[index];
                else if (typeT == typeof(T8)) return _componentGroup8[index];
                else if (typeT == typeof(T9)) return _componentGroup9[index];
                else if (typeT == typeof(T10)) return _componentGroup10[index];
            }
            return null;
        }

        public override IComponent DataAtIndexByArray(int index, int arrayID)
        {
            if (index >= 0 && index < _dataCount)
            {
                if (arrayID >= 0 && arrayID < ARRAY_COUNT)
                {
                    switch (arrayID)
                    {
                        case (0): return _componentGroup0[index];
                        case (1): return _componentGroup1[index];
                        case (2): return _componentGroup2[index];
                        case (3): return _componentGroup3[index];
                        case (4): return _componentGroup4[index];
                        case (5): return _componentGroup5[index];
                        case (6): return _componentGroup6[index];
                        case (7): return _componentGroup7[index];
                        case (8): return _componentGroup8[index];
                        case (9): return _componentGroup9[index];
                        case (10): return _componentGroup10[index];

                        default: return null;
                    }
                }
            }

            return null;
        }
    }

    public class DataGroup<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> : DataGroup
                                                                where T0 : IComponent
                                                                where T1 : IComponent
                                                                where T2 : IComponent
                                                                where T3 : IComponent
                                                                where T4 : IComponent
                                                                where T5 : IComponent
                                                                where T6 : IComponent
                                                                where T7 : IComponent
                                                                where T8 : IComponent
                                                                where T9 : IComponent
                                                                where T10 : IComponent
                                                                where T11 : IComponent
    {
        public const int ARRAY_COUNT = 12;

        private T0[] _componentGroup0;
        private T1[] _componentGroup1;
        private T2[] _componentGroup2;
        private T3[] _componentGroup3;
        private T4[] _componentGroup4;
        private T5[] _componentGroup5;
        private T6[] _componentGroup6;
        private T7[] _componentGroup7;
        private T8[] _componentGroup8;
        private T9[] _componentGroup9;
        private T10[] _componentGroup10;
        private T11[] _componentGroup11;

        public DataGroup(int dataCount)
        {
            _dataCount = dataCount;

            _componentGroup0 = new T0[_dataCount];
            _componentGroup1 = new T1[_dataCount];
            _componentGroup2 = new T2[_dataCount];
            _componentGroup3 = new T3[_dataCount];
            _componentGroup4 = new T4[_dataCount];
            _componentGroup5 = new T5[_dataCount];
            _componentGroup6 = new T6[_dataCount];
            _componentGroup7 = new T7[_dataCount];
            _componentGroup8 = new T8[_dataCount];
            _componentGroup9 = new T9[_dataCount];
            _componentGroup10 = new T10[_dataCount];
            _componentGroup11 = new T11[_dataCount];
        }

        public override IComponent DataAtIndexByType<T>(int index)
        {
            if (index >= 0 && index < _dataCount)
            {
                Type typeT = typeof(T);

                if (typeT == typeof(T0)) return _componentGroup0[index];
                else if (typeT == typeof(T1)) return _componentGroup1[index];
                else if (typeT == typeof(T2)) return _componentGroup2[index];
                else if (typeT == typeof(T3)) return _componentGroup3[index];
                else if (typeT == typeof(T4)) return _componentGroup4[index];
                else if (typeT == typeof(T5)) return _componentGroup5[index];
                else if (typeT == typeof(T6)) return _componentGroup6[index];
                else if (typeT == typeof(T7)) return _componentGroup7[index];
                else if (typeT == typeof(T8)) return _componentGroup8[index];
                else if (typeT == typeof(T9)) return _componentGroup9[index];
                else if (typeT == typeof(T10)) return _componentGroup10[index];
                else if (typeT == typeof(T11)) return _componentGroup11[index];
            }
            return null;
        }

        public override IComponent DataAtIndexByArray(int index, int arrayID)
        {
            if (index >= 0 && index < _dataCount)
            {
                if (arrayID >= 0 && arrayID < ARRAY_COUNT)
                {
                    switch (arrayID)
                    {
                        case (0): return _componentGroup0[index];
                        case (1): return _componentGroup1[index];
                        case (2): return _componentGroup2[index];
                        case (3): return _componentGroup3[index];
                        case (4): return _componentGroup4[index];
                        case (5): return _componentGroup5[index];
                        case (6): return _componentGroup6[index];
                        case (7): return _componentGroup7[index];
                        case (8): return _componentGroup8[index];
                        case (9): return _componentGroup9[index];
                        case (10): return _componentGroup10[index];
                        case (11): return _componentGroup11[index];

                        default: return null;
                    }
                }
            }

            return null;
        }
    }

    public class DataGroup<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> : DataGroup
                                                                where T0 : IComponent
                                                                where T1 : IComponent
                                                                where T2 : IComponent
                                                                where T3 : IComponent
                                                                where T4 : IComponent
                                                                where T5 : IComponent
                                                                where T6 : IComponent
                                                                where T7 : IComponent
                                                                where T8 : IComponent
                                                                where T9 : IComponent
                                                                where T10 : IComponent
                                                                where T11 : IComponent
                                                                where T12 : IComponent
    {
        public const int ARRAY_COUNT = 13;

        private T0[] _componentGroup0;
        private T1[] _componentGroup1;
        private T2[] _componentGroup2;
        private T3[] _componentGroup3;
        private T4[] _componentGroup4;
        private T5[] _componentGroup5;
        private T6[] _componentGroup6;
        private T7[] _componentGroup7;
        private T8[] _componentGroup8;
        private T9[] _componentGroup9;
        private T10[] _componentGroup10;
        private T11[] _componentGroup11;
        private T12[] _componentGroup12;

        public DataGroup(int dataCount)
        {
            _dataCount = dataCount;

            _componentGroup0 = new T0[_dataCount];
            _componentGroup1 = new T1[_dataCount];
            _componentGroup2 = new T2[_dataCount];
            _componentGroup3 = new T3[_dataCount];
            _componentGroup4 = new T4[_dataCount];
            _componentGroup5 = new T5[_dataCount];
            _componentGroup6 = new T6[_dataCount];
            _componentGroup7 = new T7[_dataCount];
            _componentGroup8 = new T8[_dataCount];
            _componentGroup9 = new T9[_dataCount];
            _componentGroup10 = new T10[_dataCount];
            _componentGroup11 = new T11[_dataCount];
            _componentGroup12 = new T12[_dataCount];
        }

        public override IComponent DataAtIndexByType<T>(int index)
        {
            if (index >= 0 && index < _dataCount)
            {
                Type typeT = typeof(T);

                if (typeT == typeof(T0)) return _componentGroup0[index];
                else if (typeT == typeof(T1)) return _componentGroup1[index];
                else if (typeT == typeof(T2)) return _componentGroup2[index];
                else if (typeT == typeof(T3)) return _componentGroup3[index];
                else if (typeT == typeof(T4)) return _componentGroup4[index];
                else if (typeT == typeof(T5)) return _componentGroup5[index];
                else if (typeT == typeof(T6)) return _componentGroup6[index];
                else if (typeT == typeof(T7)) return _componentGroup7[index];
                else if (typeT == typeof(T8)) return _componentGroup8[index];
                else if (typeT == typeof(T9)) return _componentGroup9[index];
                else if (typeT == typeof(T10)) return _componentGroup10[index];
                else if (typeT == typeof(T11)) return _componentGroup11[index];
                else if (typeT == typeof(T12)) return _componentGroup12[index];
            }
            return null;
        }

        public override IComponent DataAtIndexByArray(int index, int arrayID)
        {
            if (index >= 0 && index < _dataCount)
            {
                if (arrayID >= 0 && arrayID < ARRAY_COUNT)
                {
                    switch (arrayID)
                    {
                        case (0): return _componentGroup0[index];
                        case (1): return _componentGroup1[index];
                        case (2): return _componentGroup2[index];
                        case (3): return _componentGroup3[index];
                        case (4): return _componentGroup4[index];
                        case (5): return _componentGroup5[index];
                        case (6): return _componentGroup6[index];
                        case (7): return _componentGroup7[index];
                        case (8): return _componentGroup8[index];
                        case (9): return _componentGroup9[index];
                        case (10): return _componentGroup10[index];
                        case (11): return _componentGroup11[index];
                        case (12): return _componentGroup12[index];

                        default: return null;
                    }
                }
            }

            return null;
        }
    }

    public class DataGroup<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> : DataGroup
                                                                where T0 : IComponent
                                                                where T1 : IComponent
                                                                where T2 : IComponent
                                                                where T3 : IComponent
                                                                where T4 : IComponent
                                                                where T5 : IComponent
                                                                where T6 : IComponent
                                                                where T7 : IComponent
                                                                where T8 : IComponent
                                                                where T9 : IComponent
                                                                where T10 : IComponent
                                                                where T11 : IComponent
                                                                where T12 : IComponent
                                                                where T13 : IComponent
    {
        public const int ARRAY_COUNT = 14;

        private T0[] _componentGroup0;
        private T1[] _componentGroup1;
        private T2[] _componentGroup2;
        private T3[] _componentGroup3;
        private T4[] _componentGroup4;
        private T5[] _componentGroup5;
        private T6[] _componentGroup6;
        private T7[] _componentGroup7;
        private T8[] _componentGroup8;
        private T9[] _componentGroup9;
        private T10[] _componentGroup10;
        private T11[] _componentGroup11;
        private T12[] _componentGroup12;
        private T13[] _componentGroup13;

        public DataGroup(int dataCount)
        {
            _dataCount = dataCount;

            _componentGroup0 = new T0[_dataCount];
            _componentGroup1 = new T1[_dataCount];
            _componentGroup2 = new T2[_dataCount];
            _componentGroup3 = new T3[_dataCount];
            _componentGroup4 = new T4[_dataCount];
            _componentGroup5 = new T5[_dataCount];
            _componentGroup6 = new T6[_dataCount];
            _componentGroup7 = new T7[_dataCount];
            _componentGroup8 = new T8[_dataCount];
            _componentGroup9 = new T9[_dataCount];
            _componentGroup10 = new T10[_dataCount];
            _componentGroup11 = new T11[_dataCount];
            _componentGroup12 = new T12[_dataCount];
            _componentGroup13 = new T13[_dataCount];
        }

        public override IComponent DataAtIndexByType<T>(int index)
        {
            if (index >= 0 && index < _dataCount)
            {
                Type typeT = typeof(T);

                if (typeT == typeof(T0)) return _componentGroup0[index];
                else if (typeT == typeof(T1)) return _componentGroup1[index];
                else if (typeT == typeof(T2)) return _componentGroup2[index];
                else if (typeT == typeof(T3)) return _componentGroup3[index];
                else if (typeT == typeof(T4)) return _componentGroup4[index];
                else if (typeT == typeof(T5)) return _componentGroup5[index];
                else if (typeT == typeof(T6)) return _componentGroup6[index];
                else if (typeT == typeof(T7)) return _componentGroup7[index];
                else if (typeT == typeof(T8)) return _componentGroup8[index];
                else if (typeT == typeof(T9)) return _componentGroup9[index];
                else if (typeT == typeof(T10)) return _componentGroup10[index];
                else if (typeT == typeof(T11)) return _componentGroup11[index];
                else if (typeT == typeof(T12)) return _componentGroup12[index];
                else if (typeT == typeof(T13)) return _componentGroup13[index];
            }
            return null;
        }

        public override IComponent DataAtIndexByArray(int index, int arrayID)
        {
            if (index >= 0 && index < _dataCount)
            {
                if (arrayID >= 0 && arrayID < ARRAY_COUNT)
                {
                    switch (arrayID)
                    {
                        case (0): return _componentGroup0[index];
                        case (1): return _componentGroup1[index];
                        case (2): return _componentGroup2[index];
                        case (3): return _componentGroup3[index];
                        case (4): return _componentGroup4[index];
                        case (5): return _componentGroup5[index];
                        case (6): return _componentGroup6[index];
                        case (7): return _componentGroup7[index];
                        case (8): return _componentGroup8[index];
                        case (9): return _componentGroup9[index];
                        case (10): return _componentGroup10[index];
                        case (11): return _componentGroup11[index];
                        case (12): return _componentGroup12[index];
                        case (13): return _componentGroup13[index];

                        default: return null;
                    }
                }
            }

            return null;
        }
    }

    public class DataGroup<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> : DataGroup
                                                                where T0 : IComponent
                                                                where T1 : IComponent
                                                                where T2 : IComponent
                                                                where T3 : IComponent
                                                                where T4 : IComponent
                                                                where T5 : IComponent
                                                                where T6 : IComponent
                                                                where T7 : IComponent
                                                                where T8 : IComponent
                                                                where T9 : IComponent
                                                                where T10 : IComponent
                                                                where T11 : IComponent
                                                                where T12 : IComponent
                                                                where T13 : IComponent
                                                                where T14 : IComponent
    {
        public const int ARRAY_COUNT = 15;

        private T0[] _componentGroup0;
        private T1[] _componentGroup1;
        private T2[] _componentGroup2;
        private T3[] _componentGroup3;
        private T4[] _componentGroup4;
        private T5[] _componentGroup5;
        private T6[] _componentGroup6;
        private T7[] _componentGroup7;
        private T8[] _componentGroup8;
        private T9[] _componentGroup9;
        private T10[] _componentGroup10;
        private T11[] _componentGroup11;
        private T12[] _componentGroup12;
        private T13[] _componentGroup13;
        private T14[] _componentGroup14;

        public DataGroup(int dataCount)
        {
            _dataCount = dataCount;

            _componentGroup0 = new T0[_dataCount];
            _componentGroup1 = new T1[_dataCount];
            _componentGroup2 = new T2[_dataCount];
            _componentGroup3 = new T3[_dataCount];
            _componentGroup4 = new T4[_dataCount];
            _componentGroup5 = new T5[_dataCount];
            _componentGroup6 = new T6[_dataCount];
            _componentGroup7 = new T7[_dataCount];
            _componentGroup8 = new T8[_dataCount];
            _componentGroup9 = new T9[_dataCount];
            _componentGroup10 = new T10[_dataCount];
            _componentGroup11 = new T11[_dataCount];
            _componentGroup12 = new T12[_dataCount];
            _componentGroup13 = new T13[_dataCount];
            _componentGroup14 = new T14[_dataCount];
        }

        public override IComponent DataAtIndexByType<T>(int index)
        {
            if (index >= 0 && index < _dataCount)
            {
                Type typeT = typeof(T);

                if (typeT == typeof(T0)) return _componentGroup0[index];
                else if (typeT == typeof(T1)) return _componentGroup1[index];
                else if (typeT == typeof(T2)) return _componentGroup2[index];
                else if (typeT == typeof(T3)) return _componentGroup3[index];
                else if (typeT == typeof(T4)) return _componentGroup4[index];
                else if (typeT == typeof(T5)) return _componentGroup5[index];
                else if (typeT == typeof(T6)) return _componentGroup6[index];
                else if (typeT == typeof(T7)) return _componentGroup7[index];
                else if (typeT == typeof(T8)) return _componentGroup8[index];
                else if (typeT == typeof(T9)) return _componentGroup9[index];
                else if (typeT == typeof(T10)) return _componentGroup10[index];
                else if (typeT == typeof(T11)) return _componentGroup11[index];
                else if (typeT == typeof(T12)) return _componentGroup12[index];
                else if (typeT == typeof(T13)) return _componentGroup13[index];
                else if (typeT == typeof(T14)) return _componentGroup14[index];
            }
            return null;
        }

        public override IComponent DataAtIndexByArray(int index, int arrayID)
        {
            if (index >= 0 && index < _dataCount)
            {
                if (arrayID >= 0 && arrayID < ARRAY_COUNT)
                {
                    switch (arrayID)
                    {
                        case (0): return _componentGroup0[index];
                        case (1): return _componentGroup1[index];
                        case (2): return _componentGroup2[index];
                        case (3): return _componentGroup3[index];
                        case (4): return _componentGroup4[index];
                        case (5): return _componentGroup5[index];
                        case (6): return _componentGroup6[index];
                        case (7): return _componentGroup7[index];
                        case (8): return _componentGroup8[index];
                        case (9): return _componentGroup9[index];
                        case (10): return _componentGroup10[index];
                        case (11): return _componentGroup11[index];
                        case (12): return _componentGroup12[index];
                        case (13): return _componentGroup13[index];
                        case (14): return _componentGroup14[index];

                        default: return null;
                    }
                }
            }

            return null;
        }
    }

    public class DataGroup<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> : DataGroup
                                                                where T0 : IComponent
                                                                where T1 : IComponent
                                                                where T2 : IComponent
                                                                where T3 : IComponent
                                                                where T4 : IComponent
                                                                where T5 : IComponent
                                                                where T6 : IComponent
                                                                where T7 : IComponent
                                                                where T8 : IComponent
                                                                where T9 : IComponent
                                                                where T10 : IComponent
                                                                where T11 : IComponent
                                                                where T12 : IComponent
                                                                where T13 : IComponent
                                                                where T14 : IComponent
                                                                where T15 : IComponent
    {
        public const int ARRAY_COUNT = 16;

        private T0[] _componentGroup0;
        private T1[] _componentGroup1;
        private T2[] _componentGroup2;
        private T3[] _componentGroup3;
        private T4[] _componentGroup4;
        private T5[] _componentGroup5;
        private T6[] _componentGroup6;
        private T7[] _componentGroup7;
        private T8[] _componentGroup8;
        private T9[] _componentGroup9;
        private T10[] _componentGroup10;
        private T11[] _componentGroup11;
        private T12[] _componentGroup12;
        private T13[] _componentGroup13;
        private T14[] _componentGroup14;
        private T15[] _componentGroup15;

        public DataGroup(int dataCount)
        {
            _dataCount = dataCount;

            _componentGroup0 = new T0[_dataCount];
            _componentGroup1 = new T1[_dataCount];
            _componentGroup2 = new T2[_dataCount];
            _componentGroup3 = new T3[_dataCount];
            _componentGroup4 = new T4[_dataCount];
            _componentGroup5 = new T5[_dataCount];
            _componentGroup6 = new T6[_dataCount];
            _componentGroup7 = new T7[_dataCount];
            _componentGroup8 = new T8[_dataCount];
            _componentGroup9 = new T9[_dataCount];
            _componentGroup10 = new T10[_dataCount];
            _componentGroup11 = new T11[_dataCount];
            _componentGroup12 = new T12[_dataCount];
            _componentGroup13 = new T13[_dataCount];
            _componentGroup14 = new T14[_dataCount];
            _componentGroup15 = new T15[_dataCount];
        }

        public override IComponent DataAtIndexByType<T>(int index)
        {
            if (index >= 0 && index < _dataCount)
            {
                Type typeT = typeof(T);

                if (typeT == typeof(T0)) return _componentGroup0[index];
                else if (typeT == typeof(T1)) return _componentGroup1[index];
                else if (typeT == typeof(T2)) return _componentGroup2[index];
                else if (typeT == typeof(T3)) return _componentGroup3[index];
                else if (typeT == typeof(T4)) return _componentGroup4[index];
                else if (typeT == typeof(T5)) return _componentGroup5[index];
                else if (typeT == typeof(T6)) return _componentGroup6[index];
                else if (typeT == typeof(T7)) return _componentGroup7[index];
                else if (typeT == typeof(T8)) return _componentGroup8[index];
                else if (typeT == typeof(T9)) return _componentGroup9[index];
                else if (typeT == typeof(T10)) return _componentGroup10[index];
                else if (typeT == typeof(T11)) return _componentGroup11[index];
                else if (typeT == typeof(T12)) return _componentGroup12[index];
                else if (typeT == typeof(T13)) return _componentGroup13[index];
                else if (typeT == typeof(T14)) return _componentGroup14[index];
                else if (typeT == typeof(T15)) return _componentGroup15[index];
            }
            return null;
        }

        public override IComponent DataAtIndexByArray(int index, int arrayID)
        {
            if (index >= 0 && index < _dataCount)
            {
                if (arrayID >= 0 && arrayID < ARRAY_COUNT)
                {
                    switch (arrayID)
                    {
                        case (0): return _componentGroup0[index];
                        case (1): return _componentGroup1[index];
                        case (2): return _componentGroup2[index];
                        case (3): return _componentGroup3[index];
                        case (4): return _componentGroup4[index];
                        case (5): return _componentGroup5[index];
                        case (6): return _componentGroup6[index];
                        case (7): return _componentGroup7[index];
                        case (8): return _componentGroup8[index];
                        case (9): return _componentGroup9[index];
                        case (10): return _componentGroup10[index];
                        case (11): return _componentGroup11[index];
                        case (12): return _componentGroup12[index];
                        case (13): return _componentGroup13[index];
                        case (14): return _componentGroup14[index];
                        case (15): return _componentGroup15[index];

                        default: return null;
                    }
                }
            }

            return null;
        }
    }

    public class DataGroup<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16> : DataGroup
                                                                where T0 : IComponent
                                                                where T1 : IComponent
                                                                where T2 : IComponent
                                                                where T3 : IComponent
                                                                where T4 : IComponent
                                                                where T5 : IComponent
                                                                where T6 : IComponent
                                                                where T7 : IComponent
                                                                where T8 : IComponent
                                                                where T9 : IComponent
                                                                where T10 : IComponent
                                                                where T11 : IComponent
                                                                where T12 : IComponent
                                                                where T13 : IComponent
                                                                where T14 : IComponent
                                                                where T15 : IComponent
                                                                where T16 : IComponent
    {
        public const int ARRAY_COUNT = 17;

        private T0[] _componentGroup0;
        private T1[] _componentGroup1;
        private T2[] _componentGroup2;
        private T3[] _componentGroup3;
        private T4[] _componentGroup4;
        private T5[] _componentGroup5;
        private T6[] _componentGroup6;
        private T7[] _componentGroup7;
        private T8[] _componentGroup8;
        private T9[] _componentGroup9;
        private T10[] _componentGroup10;
        private T11[] _componentGroup11;
        private T12[] _componentGroup12;
        private T13[] _componentGroup13;
        private T14[] _componentGroup14;
        private T15[] _componentGroup15;
        private T16[] _componentGroup16;

        public DataGroup(int dataCount)
        {
            _dataCount = dataCount;

            _componentGroup0 = new T0[_dataCount];
            _componentGroup1 = new T1[_dataCount];
            _componentGroup2 = new T2[_dataCount];
            _componentGroup3 = new T3[_dataCount];
            _componentGroup4 = new T4[_dataCount];
            _componentGroup5 = new T5[_dataCount];
            _componentGroup6 = new T6[_dataCount];
            _componentGroup7 = new T7[_dataCount];
            _componentGroup8 = new T8[_dataCount];
            _componentGroup9 = new T9[_dataCount];
            _componentGroup10 = new T10[_dataCount];
            _componentGroup11 = new T11[_dataCount];
            _componentGroup12 = new T12[_dataCount];
            _componentGroup13 = new T13[_dataCount];
            _componentGroup14 = new T14[_dataCount];
            _componentGroup15 = new T15[_dataCount];
            _componentGroup16 = new T16[_dataCount];
        }

        public override IComponent DataAtIndexByType<T>(int index)
        {
            if (index >= 0 && index < _dataCount)
            {
                Type typeT = typeof(T);

                if (typeT == typeof(T0)) return _componentGroup0[index];
                else if (typeT == typeof(T1)) return _componentGroup1[index];
                else if (typeT == typeof(T2)) return _componentGroup2[index];
                else if (typeT == typeof(T3)) return _componentGroup3[index];
                else if (typeT == typeof(T4)) return _componentGroup4[index];
                else if (typeT == typeof(T5)) return _componentGroup5[index];
                else if (typeT == typeof(T6)) return _componentGroup6[index];
                else if (typeT == typeof(T7)) return _componentGroup7[index];
                else if (typeT == typeof(T8)) return _componentGroup8[index];
                else if (typeT == typeof(T9)) return _componentGroup9[index];
                else if (typeT == typeof(T10)) return _componentGroup10[index];
                else if (typeT == typeof(T11)) return _componentGroup11[index];
                else if (typeT == typeof(T12)) return _componentGroup12[index];
                else if (typeT == typeof(T13)) return _componentGroup13[index];
                else if (typeT == typeof(T14)) return _componentGroup14[index];
                else if (typeT == typeof(T15)) return _componentGroup15[index];
                else if (typeT == typeof(T16)) return _componentGroup16[index];
            }
            return null;
        }

        public override IComponent DataAtIndexByArray(int index, int arrayID)
        {
            if (index >= 0 && index < _dataCount)
            {
                if (arrayID >= 0 && arrayID < ARRAY_COUNT)
                {
                    switch (arrayID)
                    {
                        case (0): return _componentGroup0[index];
                        case (1): return _componentGroup1[index];
                        case (2): return _componentGroup2[index];
                        case (3): return _componentGroup3[index];
                        case (4): return _componentGroup4[index];
                        case (5): return _componentGroup5[index];
                        case (6): return _componentGroup6[index];
                        case (7): return _componentGroup7[index];
                        case (8): return _componentGroup8[index];
                        case (9): return _componentGroup9[index];
                        case (10): return _componentGroup10[index];
                        case (11): return _componentGroup11[index];
                        case (12): return _componentGroup12[index];
                        case (13): return _componentGroup13[index];
                        case (14): return _componentGroup14[index];
                        case (15): return _componentGroup15[index];
                        case (16): return _componentGroup16[index];

                        default: return null;
                    }
                }
            }

            return null;
        }
    }

    public class DataGroup<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17> : DataGroup
                                                                where T0 : IComponent
                                                                where T1 : IComponent
                                                                where T2 : IComponent
                                                                where T3 : IComponent
                                                                where T4 : IComponent
                                                                where T5 : IComponent
                                                                where T6 : IComponent
                                                                where T7 : IComponent
                                                                where T8 : IComponent
                                                                where T9 : IComponent
                                                                where T10 : IComponent
                                                                where T11 : IComponent
                                                                where T12 : IComponent
                                                                where T13 : IComponent
                                                                where T14 : IComponent
                                                                where T15 : IComponent
                                                                where T16 : IComponent
                                                                where T17 : IComponent
    {
        public const int ARRAY_COUNT = 18;

        private T0[] _componentGroup0;
        private T1[] _componentGroup1;
        private T2[] _componentGroup2;
        private T3[] _componentGroup3;
        private T4[] _componentGroup4;
        private T5[] _componentGroup5;
        private T6[] _componentGroup6;
        private T7[] _componentGroup7;
        private T8[] _componentGroup8;
        private T9[] _componentGroup9;
        private T10[] _componentGroup10;
        private T11[] _componentGroup11;
        private T12[] _componentGroup12;
        private T13[] _componentGroup13;
        private T14[] _componentGroup14;
        private T15[] _componentGroup15;
        private T16[] _componentGroup16;
        private T17[] _componentGroup17;

        public DataGroup(int dataCount)
        {
            _dataCount = dataCount;

            _componentGroup0 = new T0[_dataCount];
            _componentGroup1 = new T1[_dataCount];
            _componentGroup2 = new T2[_dataCount];
            _componentGroup3 = new T3[_dataCount];
            _componentGroup4 = new T4[_dataCount];
            _componentGroup5 = new T5[_dataCount];
            _componentGroup6 = new T6[_dataCount];
            _componentGroup7 = new T7[_dataCount];
            _componentGroup8 = new T8[_dataCount];
            _componentGroup9 = new T9[_dataCount];
            _componentGroup10 = new T10[_dataCount];
            _componentGroup11 = new T11[_dataCount];
            _componentGroup12 = new T12[_dataCount];
            _componentGroup13 = new T13[_dataCount];
            _componentGroup14 = new T14[_dataCount];
            _componentGroup15 = new T15[_dataCount];
            _componentGroup16 = new T16[_dataCount];
            _componentGroup17 = new T17[_dataCount];
        }

        public override IComponent DataAtIndexByType<T>(int index)
        {
            if (index >= 0 && index < _dataCount)
            {
                Type typeT = typeof(T);

                if (typeT == typeof(T0)) return _componentGroup0[index];
                else if (typeT == typeof(T1)) return _componentGroup1[index];
                else if (typeT == typeof(T2)) return _componentGroup2[index];
                else if (typeT == typeof(T3)) return _componentGroup3[index];
                else if (typeT == typeof(T4)) return _componentGroup4[index];
                else if (typeT == typeof(T5)) return _componentGroup5[index];
                else if (typeT == typeof(T6)) return _componentGroup6[index];
                else if (typeT == typeof(T7)) return _componentGroup7[index];
                else if (typeT == typeof(T8)) return _componentGroup8[index];
                else if (typeT == typeof(T9)) return _componentGroup9[index];
                else if (typeT == typeof(T10)) return _componentGroup10[index];
                else if (typeT == typeof(T11)) return _componentGroup11[index];
                else if (typeT == typeof(T12)) return _componentGroup12[index];
                else if (typeT == typeof(T13)) return _componentGroup13[index];
                else if (typeT == typeof(T14)) return _componentGroup14[index];
                else if (typeT == typeof(T15)) return _componentGroup15[index];
                else if (typeT == typeof(T16)) return _componentGroup16[index];
                else if (typeT == typeof(T17)) return _componentGroup17[index];
            }
            return null;
        }

        public override IComponent DataAtIndexByArray(int index, int arrayID)
        {
            if (index >= 0 && index < _dataCount)
            {
                if (arrayID >= 0 && arrayID < ARRAY_COUNT)
                {
                    switch (arrayID)
                    {
                        case (0): return _componentGroup0[index];
                        case (1): return _componentGroup1[index];
                        case (2): return _componentGroup2[index];
                        case (3): return _componentGroup3[index];
                        case (4): return _componentGroup4[index];
                        case (5): return _componentGroup5[index];
                        case (6): return _componentGroup6[index];
                        case (7): return _componentGroup7[index];
                        case (8): return _componentGroup8[index];
                        case (9): return _componentGroup9[index];
                        case (10): return _componentGroup10[index];
                        case (11): return _componentGroup11[index];
                        case (12): return _componentGroup12[index];
                        case (13): return _componentGroup13[index];
                        case (14): return _componentGroup14[index];
                        case (15): return _componentGroup15[index];
                        case (16): return _componentGroup16[index];
                        case (17): return _componentGroup17[index];

                        default: return null;
                    }
                }
            }

            return null;
        }
    }

    public class DataGroup<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18> : DataGroup
                                                                where T0 : IComponent
                                                                where T1 : IComponent
                                                                where T2 : IComponent
                                                                where T3 : IComponent
                                                                where T4 : IComponent
                                                                where T5 : IComponent
                                                                where T6 : IComponent
                                                                where T7 : IComponent
                                                                where T8 : IComponent
                                                                where T9 : IComponent
                                                                where T10 : IComponent
                                                                where T11 : IComponent
                                                                where T12 : IComponent
                                                                where T13 : IComponent
                                                                where T14 : IComponent
                                                                where T15 : IComponent
                                                                where T16 : IComponent
                                                                where T17 : IComponent
                                                                where T18 : IComponent
    {
        public const int ARRAY_COUNT = 19;

        private T0[] _componentGroup0;
        private T1[] _componentGroup1;
        private T2[] _componentGroup2;
        private T3[] _componentGroup3;
        private T4[] _componentGroup4;
        private T5[] _componentGroup5;
        private T6[] _componentGroup6;
        private T7[] _componentGroup7;
        private T8[] _componentGroup8;
        private T9[] _componentGroup9;
        private T10[] _componentGroup10;
        private T11[] _componentGroup11;
        private T12[] _componentGroup12;
        private T13[] _componentGroup13;
        private T14[] _componentGroup14;
        private T15[] _componentGroup15;
        private T16[] _componentGroup16;
        private T17[] _componentGroup17;
        private T18[] _componentGroup18;

        public DataGroup(int dataCount)
        {
            _dataCount = dataCount;

            _componentGroup0 = new T0[_dataCount];
            _componentGroup1 = new T1[_dataCount];
            _componentGroup2 = new T2[_dataCount];
            _componentGroup3 = new T3[_dataCount];
            _componentGroup4 = new T4[_dataCount];
            _componentGroup5 = new T5[_dataCount];
            _componentGroup6 = new T6[_dataCount];
            _componentGroup7 = new T7[_dataCount];
            _componentGroup8 = new T8[_dataCount];
            _componentGroup9 = new T9[_dataCount];
            _componentGroup10 = new T10[_dataCount];
            _componentGroup11 = new T11[_dataCount];
            _componentGroup12 = new T12[_dataCount];
            _componentGroup13 = new T13[_dataCount];
            _componentGroup14 = new T14[_dataCount];
            _componentGroup15 = new T15[_dataCount];
            _componentGroup16 = new T16[_dataCount];
            _componentGroup17 = new T17[_dataCount];
            _componentGroup18 = new T18[_dataCount];
        }

        public override IComponent DataAtIndexByType<T>(int index)
        {
            if (index >= 0 && index < _dataCount)
            {
                Type typeT = typeof(T);

                if (typeT == typeof(T0)) return _componentGroup0[index];
                else if (typeT == typeof(T1)) return _componentGroup1[index];
                else if (typeT == typeof(T2)) return _componentGroup2[index];
                else if (typeT == typeof(T3)) return _componentGroup3[index];
                else if (typeT == typeof(T4)) return _componentGroup4[index];
                else if (typeT == typeof(T5)) return _componentGroup5[index];
                else if (typeT == typeof(T6)) return _componentGroup6[index];
                else if (typeT == typeof(T7)) return _componentGroup7[index];
                else if (typeT == typeof(T8)) return _componentGroup8[index];
                else if (typeT == typeof(T9)) return _componentGroup9[index];
                else if (typeT == typeof(T10)) return _componentGroup10[index];
                else if (typeT == typeof(T11)) return _componentGroup11[index];
                else if (typeT == typeof(T12)) return _componentGroup12[index];
                else if (typeT == typeof(T13)) return _componentGroup13[index];
                else if (typeT == typeof(T14)) return _componentGroup14[index];
                else if (typeT == typeof(T15)) return _componentGroup15[index];
                else if (typeT == typeof(T16)) return _componentGroup16[index];
                else if (typeT == typeof(T17)) return _componentGroup17[index];
                else if (typeT == typeof(T18)) return _componentGroup18[index];
            }
            return null;
        }

        public override IComponent DataAtIndexByArray(int index, int arrayID)
        {
            if (index >= 0 && index < _dataCount)
            {
                if (arrayID >= 0 && arrayID < ARRAY_COUNT)
                {
                    switch (arrayID)
                    {
                        case (0): return _componentGroup0[index];
                        case (1): return _componentGroup1[index];
                        case (2): return _componentGroup2[index];
                        case (3): return _componentGroup3[index];
                        case (4): return _componentGroup4[index];
                        case (5): return _componentGroup5[index];
                        case (6): return _componentGroup6[index];
                        case (7): return _componentGroup7[index];
                        case (8): return _componentGroup8[index];
                        case (9): return _componentGroup9[index];
                        case (10): return _componentGroup10[index];
                        case (11): return _componentGroup11[index];
                        case (12): return _componentGroup12[index];
                        case (13): return _componentGroup13[index];
                        case (14): return _componentGroup14[index];
                        case (15): return _componentGroup15[index];
                        case (16): return _componentGroup16[index];
                        case (17): return _componentGroup17[index];
                        case (18): return _componentGroup18[index];

                        default: return null;
                    }
                }
            }

            return null;
        }
    }

    public class DataGroup<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19> : DataGroup
                                                                where T0 : IComponent
                                                                where T1 : IComponent
                                                                where T2 : IComponent
                                                                where T3 : IComponent
                                                                where T4 : IComponent
                                                                where T5 : IComponent
                                                                where T6 : IComponent
                                                                where T7 : IComponent
                                                                where T8 : IComponent
                                                                where T9 : IComponent
                                                                where T10 : IComponent
                                                                where T11 : IComponent
                                                                where T12 : IComponent
                                                                where T13 : IComponent
                                                                where T14 : IComponent
                                                                where T15 : IComponent
                                                                where T16 : IComponent
                                                                where T17 : IComponent
                                                                where T18 : IComponent
                                                                where T19 : IComponent
    {
        public const int ARRAY_COUNT = 20;

        private T0[] _componentGroup0;
        private T1[] _componentGroup1;
        private T2[] _componentGroup2;
        private T3[] _componentGroup3;
        private T4[] _componentGroup4;
        private T5[] _componentGroup5;
        private T6[] _componentGroup6;
        private T7[] _componentGroup7;
        private T8[] _componentGroup8;
        private T9[] _componentGroup9;
        private T10[] _componentGroup10;
        private T11[] _componentGroup11;
        private T12[] _componentGroup12;
        private T13[] _componentGroup13;
        private T14[] _componentGroup14;
        private T15[] _componentGroup15;
        private T16[] _componentGroup16;
        private T17[] _componentGroup17;
        private T18[] _componentGroup18;
        private T19[] _componentGroup19;

        public DataGroup(int dataCount)
        {
            _dataCount = dataCount;

            _componentGroup0 = new T0[_dataCount];
            _componentGroup1 = new T1[_dataCount];
            _componentGroup2 = new T2[_dataCount];
            _componentGroup3 = new T3[_dataCount];
            _componentGroup4 = new T4[_dataCount];
            _componentGroup5 = new T5[_dataCount];
            _componentGroup6 = new T6[_dataCount];
            _componentGroup7 = new T7[_dataCount];
            _componentGroup8 = new T8[_dataCount];
            _componentGroup9 = new T9[_dataCount];
            _componentGroup10 = new T10[_dataCount];
            _componentGroup11 = new T11[_dataCount];
            _componentGroup12 = new T12[_dataCount];
            _componentGroup13 = new T13[_dataCount];
            _componentGroup14 = new T14[_dataCount];
            _componentGroup15 = new T15[_dataCount];
            _componentGroup16 = new T16[_dataCount];
            _componentGroup17 = new T17[_dataCount];
            _componentGroup18 = new T18[_dataCount];
            _componentGroup19 = new T19[_dataCount];
        }

        public override IComponent DataAtIndexByType<T>(int index)
        {
            if (index >= 0 && index < _dataCount)
            {
                Type typeT = typeof(T);

                if (typeT == typeof(T0)) return _componentGroup0[index];
                else if (typeT == typeof(T1)) return _componentGroup1[index];
                else if (typeT == typeof(T2)) return _componentGroup2[index];
                else if (typeT == typeof(T3)) return _componentGroup3[index];
                else if (typeT == typeof(T4)) return _componentGroup4[index];
                else if (typeT == typeof(T5)) return _componentGroup5[index];
                else if (typeT == typeof(T6)) return _componentGroup6[index];
                else if (typeT == typeof(T7)) return _componentGroup7[index];
                else if (typeT == typeof(T8)) return _componentGroup8[index];
                else if (typeT == typeof(T9)) return _componentGroup9[index];
                else if (typeT == typeof(T10)) return _componentGroup10[index];
                else if (typeT == typeof(T11)) return _componentGroup11[index];
                else if (typeT == typeof(T12)) return _componentGroup12[index];
                else if (typeT == typeof(T13)) return _componentGroup13[index];
                else if (typeT == typeof(T14)) return _componentGroup14[index];
                else if (typeT == typeof(T15)) return _componentGroup15[index];
                else if (typeT == typeof(T16)) return _componentGroup16[index];
                else if (typeT == typeof(T17)) return _componentGroup17[index];
                else if (typeT == typeof(T18)) return _componentGroup18[index];
                else if (typeT == typeof(T19)) return _componentGroup19[index];
            }
            return null;
        }

        public override IComponent DataAtIndexByArray(int index, int arrayID)
        {
            if (index >= 0 && index < _dataCount)
            {
                if (arrayID >= 0 && arrayID < ARRAY_COUNT)
                {
                    switch (arrayID)
                    {
                        case (0): return _componentGroup0[index];
                        case (1): return _componentGroup1[index];
                        case (2): return _componentGroup2[index];
                        case (3): return _componentGroup3[index];
                        case (4): return _componentGroup4[index];
                        case (5): return _componentGroup5[index];
                        case (6): return _componentGroup6[index];
                        case (7): return _componentGroup7[index];
                        case (8): return _componentGroup8[index];
                        case (9): return _componentGroup9[index];
                        case (10): return _componentGroup10[index];
                        case (11): return _componentGroup11[index];
                        case (12): return _componentGroup12[index];
                        case (13): return _componentGroup13[index];
                        case (14): return _componentGroup14[index];
                        case (15): return _componentGroup15[index];
                        case (16): return _componentGroup16[index];
                        case (17): return _componentGroup17[index];
                        case (18): return _componentGroup18[index];
                        case (19): return _componentGroup19[index];

                        default: return null;
                    }
                }
            }

            return null;
        }
    }

    public class DataGroup<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20> : DataGroup
                                                                where T0 : IComponent
                                                                where T1 : IComponent
                                                                where T2 : IComponent
                                                                where T3 : IComponent
                                                                where T4 : IComponent
                                                                where T5 : IComponent
                                                                where T6 : IComponent
                                                                where T7 : IComponent
                                                                where T8 : IComponent
                                                                where T9 : IComponent
                                                                where T10 : IComponent
                                                                where T11 : IComponent
                                                                where T12 : IComponent
                                                                where T13 : IComponent
                                                                where T14 : IComponent
                                                                where T15 : IComponent
                                                                where T16 : IComponent
                                                                where T17 : IComponent
                                                                where T18 : IComponent
                                                                where T19 : IComponent
                                                                where T20 : IComponent
    {
        public const int ARRAY_COUNT = 21;

        private T0[] _componentGroup0;
        private T1[] _componentGroup1;
        private T2[] _componentGroup2;
        private T3[] _componentGroup3;
        private T4[] _componentGroup4;
        private T5[] _componentGroup5;
        private T6[] _componentGroup6;
        private T7[] _componentGroup7;
        private T8[] _componentGroup8;
        private T9[] _componentGroup9;
        private T10[] _componentGroup10;
        private T11[] _componentGroup11;
        private T12[] _componentGroup12;
        private T13[] _componentGroup13;
        private T14[] _componentGroup14;
        private T15[] _componentGroup15;
        private T16[] _componentGroup16;
        private T17[] _componentGroup17;
        private T18[] _componentGroup18;
        private T19[] _componentGroup19;
        private T20[] _componentGroup20;

        public DataGroup(int dataCount)
        {
            _dataCount = dataCount;

            _componentGroup0 = new T0[_dataCount];
            _componentGroup1 = new T1[_dataCount];
            _componentGroup2 = new T2[_dataCount];
            _componentGroup3 = new T3[_dataCount];
            _componentGroup4 = new T4[_dataCount];
            _componentGroup5 = new T5[_dataCount];
            _componentGroup6 = new T6[_dataCount];
            _componentGroup7 = new T7[_dataCount];
            _componentGroup8 = new T8[_dataCount];
            _componentGroup9 = new T9[_dataCount];
            _componentGroup10 = new T10[_dataCount];
            _componentGroup11 = new T11[_dataCount];
            _componentGroup12 = new T12[_dataCount];
            _componentGroup13 = new T13[_dataCount];
            _componentGroup14 = new T14[_dataCount];
            _componentGroup15 = new T15[_dataCount];
            _componentGroup16 = new T16[_dataCount];
            _componentGroup17 = new T17[_dataCount];
            _componentGroup18 = new T18[_dataCount];
            _componentGroup19 = new T19[_dataCount];
            _componentGroup20 = new T20[_dataCount];
        }

        public override IComponent DataAtIndexByType<T>(int index)
        {
            if (index >= 0 && index < _dataCount)
            {
                Type typeT = typeof(T);

                if (typeT == typeof(T0)) return _componentGroup0[index];
                else if (typeT == typeof(T1)) return _componentGroup1[index];
                else if (typeT == typeof(T2)) return _componentGroup2[index];
                else if (typeT == typeof(T3)) return _componentGroup3[index];
                else if (typeT == typeof(T4)) return _componentGroup4[index];
                else if (typeT == typeof(T5)) return _componentGroup5[index];
                else if (typeT == typeof(T6)) return _componentGroup6[index];
                else if (typeT == typeof(T7)) return _componentGroup7[index];
                else if (typeT == typeof(T8)) return _componentGroup8[index];
                else if (typeT == typeof(T9)) return _componentGroup9[index];
                else if (typeT == typeof(T10)) return _componentGroup10[index];
                else if (typeT == typeof(T11)) return _componentGroup11[index];
                else if (typeT == typeof(T12)) return _componentGroup12[index];
                else if (typeT == typeof(T13)) return _componentGroup13[index];
                else if (typeT == typeof(T14)) return _componentGroup14[index];
                else if (typeT == typeof(T15)) return _componentGroup15[index];
                else if (typeT == typeof(T16)) return _componentGroup16[index];
                else if (typeT == typeof(T17)) return _componentGroup17[index];
                else if (typeT == typeof(T18)) return _componentGroup18[index];
                else if (typeT == typeof(T19)) return _componentGroup19[index];
                else if (typeT == typeof(T20)) return _componentGroup20[index];
            }
            return null;
        }

        public override IComponent DataAtIndexByArray(int index, int arrayID)
        {
            if (index >= 0 && index < _dataCount)
            {
                if (arrayID >= 0 && arrayID < ARRAY_COUNT)
                {
                    switch (arrayID)
                    {
                        case (0): return _componentGroup0[index];
                        case (1): return _componentGroup1[index];
                        case (2): return _componentGroup2[index];
                        case (3): return _componentGroup3[index];
                        case (4): return _componentGroup4[index];
                        case (5): return _componentGroup5[index];
                        case (6): return _componentGroup6[index];
                        case (7): return _componentGroup7[index];
                        case (8): return _componentGroup8[index];
                        case (9): return _componentGroup9[index];
                        case (10): return _componentGroup10[index];
                        case (11): return _componentGroup11[index];
                        case (12): return _componentGroup12[index];
                        case (13): return _componentGroup13[index];
                        case (14): return _componentGroup14[index];
                        case (15): return _componentGroup15[index];
                        case (16): return _componentGroup16[index];
                        case (17): return _componentGroup17[index];
                        case (18): return _componentGroup18[index];
                        case (19): return _componentGroup19[index];
                        case (20): return _componentGroup20[index];

                        default: return null;
                    }
                }
            }

            return null;
        }
    }

    public class DataGroup<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21> : DataGroup
                                                                where T0 : IComponent
                                                                where T1 : IComponent
                                                                where T2 : IComponent
                                                                where T3 : IComponent
                                                                where T4 : IComponent
                                                                where T5 : IComponent
                                                                where T6 : IComponent
                                                                where T7 : IComponent
                                                                where T8 : IComponent
                                                                where T9 : IComponent
                                                                where T10 : IComponent
                                                                where T11 : IComponent
                                                                where T12 : IComponent
                                                                where T13 : IComponent
                                                                where T14 : IComponent
                                                                where T15 : IComponent
                                                                where T16 : IComponent
                                                                where T17 : IComponent
                                                                where T18 : IComponent
                                                                where T19 : IComponent
                                                                where T20 : IComponent
                                                                where T21 : IComponent
    {
        public const int ARRAY_COUNT = 22;

        private T0[] _componentGroup0;
        private T1[] _componentGroup1;
        private T2[] _componentGroup2;
        private T3[] _componentGroup3;
        private T4[] _componentGroup4;
        private T5[] _componentGroup5;
        private T6[] _componentGroup6;
        private T7[] _componentGroup7;
        private T8[] _componentGroup8;
        private T9[] _componentGroup9;
        private T10[] _componentGroup10;
        private T11[] _componentGroup11;
        private T12[] _componentGroup12;
        private T13[] _componentGroup13;
        private T14[] _componentGroup14;
        private T15[] _componentGroup15;
        private T16[] _componentGroup16;
        private T17[] _componentGroup17;
        private T18[] _componentGroup18;
        private T19[] _componentGroup19;
        private T20[] _componentGroup20;
        private T21[] _componentGroup21;

        public DataGroup(int dataCount)
        {
            _dataCount = dataCount;

            _componentGroup0 = new T0[_dataCount];
            _componentGroup1 = new T1[_dataCount];
            _componentGroup2 = new T2[_dataCount];
            _componentGroup3 = new T3[_dataCount];
            _componentGroup4 = new T4[_dataCount];
            _componentGroup5 = new T5[_dataCount];
            _componentGroup6 = new T6[_dataCount];
            _componentGroup7 = new T7[_dataCount];
            _componentGroup8 = new T8[_dataCount];
            _componentGroup9 = new T9[_dataCount];
            _componentGroup10 = new T10[_dataCount];
            _componentGroup11 = new T11[_dataCount];
            _componentGroup12 = new T12[_dataCount];
            _componentGroup13 = new T13[_dataCount];
            _componentGroup14 = new T14[_dataCount];
            _componentGroup15 = new T15[_dataCount];
            _componentGroup16 = new T16[_dataCount];
            _componentGroup17 = new T17[_dataCount];
            _componentGroup18 = new T18[_dataCount];
            _componentGroup19 = new T19[_dataCount];
            _componentGroup20 = new T20[_dataCount];
            _componentGroup21 = new T21[_dataCount];
        }

        public override IComponent DataAtIndexByType<T>(int index)
        {
            if (index >= 0 && index < _dataCount)
            {
                Type typeT = typeof(T);

                if (typeT == typeof(T0)) return _componentGroup0[index];
                else if (typeT == typeof(T1)) return _componentGroup1[index];
                else if (typeT == typeof(T2)) return _componentGroup2[index];
                else if (typeT == typeof(T3)) return _componentGroup3[index];
                else if (typeT == typeof(T4)) return _componentGroup4[index];
                else if (typeT == typeof(T5)) return _componentGroup5[index];
                else if (typeT == typeof(T6)) return _componentGroup6[index];
                else if (typeT == typeof(T7)) return _componentGroup7[index];
                else if (typeT == typeof(T8)) return _componentGroup8[index];
                else if (typeT == typeof(T9)) return _componentGroup9[index];
                else if (typeT == typeof(T10)) return _componentGroup10[index];
                else if (typeT == typeof(T11)) return _componentGroup11[index];
                else if (typeT == typeof(T12)) return _componentGroup12[index];
                else if (typeT == typeof(T13)) return _componentGroup13[index];
                else if (typeT == typeof(T14)) return _componentGroup14[index];
                else if (typeT == typeof(T15)) return _componentGroup15[index];
                else if (typeT == typeof(T16)) return _componentGroup16[index];
                else if (typeT == typeof(T17)) return _componentGroup17[index];
                else if (typeT == typeof(T18)) return _componentGroup18[index];
                else if (typeT == typeof(T19)) return _componentGroup19[index];
                else if (typeT == typeof(T20)) return _componentGroup20[index];
                else if (typeT == typeof(T21)) return _componentGroup21[index];
            }
            return null;
        }

        public override IComponent DataAtIndexByArray(int index, int arrayID)
        {
            if (index >= 0 && index < _dataCount)
            {
                if (arrayID >= 0 && arrayID < ARRAY_COUNT)
                {
                    switch (arrayID)
                    {
                        case (0): return _componentGroup0[index];
                        case (1): return _componentGroup1[index];
                        case (2): return _componentGroup2[index];
                        case (3): return _componentGroup3[index];
                        case (4): return _componentGroup4[index];
                        case (5): return _componentGroup5[index];
                        case (6): return _componentGroup6[index];
                        case (7): return _componentGroup7[index];
                        case (8): return _componentGroup8[index];
                        case (9): return _componentGroup9[index];
                        case (10): return _componentGroup10[index];
                        case (11): return _componentGroup11[index];
                        case (12): return _componentGroup12[index];
                        case (13): return _componentGroup13[index];
                        case (14): return _componentGroup14[index];
                        case (15): return _componentGroup15[index];
                        case (16): return _componentGroup16[index];
                        case (17): return _componentGroup17[index];
                        case (18): return _componentGroup18[index];
                        case (19): return _componentGroup19[index];
                        case (20): return _componentGroup20[index];
                        case (21): return _componentGroup21[index];

                        default: return null;
                    }
                }
            }

            return null;
        }
    }

    public class DataGroup<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22> : DataGroup
                                                                where T0 : IComponent
                                                                where T1 : IComponent
                                                                where T2 : IComponent
                                                                where T3 : IComponent
                                                                where T4 : IComponent
                                                                where T5 : IComponent
                                                                where T6 : IComponent
                                                                where T7 : IComponent
                                                                where T8 : IComponent
                                                                where T9 : IComponent
                                                                where T10 : IComponent
                                                                where T11 : IComponent
                                                                where T12 : IComponent
                                                                where T13 : IComponent
                                                                where T14 : IComponent
                                                                where T15 : IComponent
                                                                where T16 : IComponent
                                                                where T17 : IComponent
                                                                where T18 : IComponent
                                                                where T19 : IComponent
                                                                where T20 : IComponent
                                                                where T21 : IComponent
                                                                where T22 : IComponent
    {
        public const int ARRAY_COUNT = 23;

        private T0[] _componentGroup0;
        private T1[] _componentGroup1;
        private T2[] _componentGroup2;
        private T3[] _componentGroup3;
        private T4[] _componentGroup4;
        private T5[] _componentGroup5;
        private T6[] _componentGroup6;
        private T7[] _componentGroup7;
        private T8[] _componentGroup8;
        private T9[] _componentGroup9;
        private T10[] _componentGroup10;
        private T11[] _componentGroup11;
        private T12[] _componentGroup12;
        private T13[] _componentGroup13;
        private T14[] _componentGroup14;
        private T15[] _componentGroup15;
        private T16[] _componentGroup16;
        private T17[] _componentGroup17;
        private T18[] _componentGroup18;
        private T19[] _componentGroup19;
        private T20[] _componentGroup20;
        private T21[] _componentGroup21;
        private T22[] _componentGroup22;

        public DataGroup(int dataCount)
        {
            _dataCount = dataCount;

            _componentGroup0 = new T0[_dataCount];
            _componentGroup1 = new T1[_dataCount];
            _componentGroup2 = new T2[_dataCount];
            _componentGroup3 = new T3[_dataCount];
            _componentGroup4 = new T4[_dataCount];
            _componentGroup5 = new T5[_dataCount];
            _componentGroup6 = new T6[_dataCount];
            _componentGroup7 = new T7[_dataCount];
            _componentGroup8 = new T8[_dataCount];
            _componentGroup9 = new T9[_dataCount];
            _componentGroup10 = new T10[_dataCount];
            _componentGroup11 = new T11[_dataCount];
            _componentGroup12 = new T12[_dataCount];
            _componentGroup13 = new T13[_dataCount];
            _componentGroup14 = new T14[_dataCount];
            _componentGroup15 = new T15[_dataCount];
            _componentGroup16 = new T16[_dataCount];
            _componentGroup17 = new T17[_dataCount];
            _componentGroup18 = new T18[_dataCount];
            _componentGroup19 = new T19[_dataCount];
            _componentGroup20 = new T20[_dataCount];
            _componentGroup21 = new T21[_dataCount];
            _componentGroup22 = new T22[_dataCount];
        }

        public override IComponent DataAtIndexByType<T>(int index)
        {
            if (index >= 0 && index < _dataCount)
            {
                Type typeT = typeof(T);

                if (typeT == typeof(T0)) return _componentGroup0[index];
                else if (typeT == typeof(T1)) return _componentGroup1[index];
                else if (typeT == typeof(T2)) return _componentGroup2[index];
                else if (typeT == typeof(T3)) return _componentGroup3[index];
                else if (typeT == typeof(T4)) return _componentGroup4[index];
                else if (typeT == typeof(T5)) return _componentGroup5[index];
                else if (typeT == typeof(T6)) return _componentGroup6[index];
                else if (typeT == typeof(T7)) return _componentGroup7[index];
                else if (typeT == typeof(T8)) return _componentGroup8[index];
                else if (typeT == typeof(T9)) return _componentGroup9[index];
                else if (typeT == typeof(T10)) return _componentGroup10[index];
                else if (typeT == typeof(T11)) return _componentGroup11[index];
                else if (typeT == typeof(T12)) return _componentGroup12[index];
                else if (typeT == typeof(T13)) return _componentGroup13[index];
                else if (typeT == typeof(T14)) return _componentGroup14[index];
                else if (typeT == typeof(T15)) return _componentGroup15[index];
                else if (typeT == typeof(T16)) return _componentGroup16[index];
                else if (typeT == typeof(T17)) return _componentGroup17[index];
                else if (typeT == typeof(T18)) return _componentGroup18[index];
                else if (typeT == typeof(T19)) return _componentGroup19[index];
                else if (typeT == typeof(T20)) return _componentGroup20[index];
                else if (typeT == typeof(T21)) return _componentGroup21[index];
                else if (typeT == typeof(T22)) return _componentGroup22[index];
            }
            return null;
        }

        public override IComponent DataAtIndexByArray(int index, int arrayID)
        {
            if (index >= 0 && index < _dataCount)
            {
                if (arrayID >= 0 && arrayID < ARRAY_COUNT)
                {
                    switch (arrayID)
                    {
                        case (0): return _componentGroup0[index];
                        case (1): return _componentGroup1[index];
                        case (2): return _componentGroup2[index];
                        case (3): return _componentGroup3[index];
                        case (4): return _componentGroup4[index];
                        case (5): return _componentGroup5[index];
                        case (6): return _componentGroup6[index];
                        case (7): return _componentGroup7[index];
                        case (8): return _componentGroup8[index];
                        case (9): return _componentGroup9[index];
                        case (10): return _componentGroup10[index];
                        case (11): return _componentGroup11[index];
                        case (12): return _componentGroup12[index];
                        case (13): return _componentGroup13[index];
                        case (14): return _componentGroup14[index];
                        case (15): return _componentGroup15[index];
                        case (16): return _componentGroup16[index];
                        case (17): return _componentGroup17[index];
                        case (18): return _componentGroup18[index];
                        case (19): return _componentGroup19[index];
                        case (20): return _componentGroup20[index];
                        case (21): return _componentGroup21[index];
                        case (22): return _componentGroup22[index];

                        default: return null;
                    }
                }
            }

            return null;
        }
    }

    public class DataGroup<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23> : DataGroup
                                                                where T0 : IComponent
                                                                where T1 : IComponent
                                                                where T2 : IComponent
                                                                where T3 : IComponent
                                                                where T4 : IComponent
                                                                where T5 : IComponent
                                                                where T6 : IComponent
                                                                where T7 : IComponent
                                                                where T8 : IComponent
                                                                where T9 : IComponent
                                                                where T10 : IComponent
                                                                where T11 : IComponent
                                                                where T12 : IComponent
                                                                where T13 : IComponent
                                                                where T14 : IComponent
                                                                where T15 : IComponent
                                                                where T16 : IComponent
                                                                where T17 : IComponent
                                                                where T18 : IComponent
                                                                where T19 : IComponent
                                                                where T20 : IComponent
                                                                where T21 : IComponent
                                                                where T22 : IComponent
                                                                where T23 : IComponent
    {
        public const int ARRAY_COUNT = 24;

        private T0[] _componentGroup0;
        private T1[] _componentGroup1;
        private T2[] _componentGroup2;
        private T3[] _componentGroup3;
        private T4[] _componentGroup4;
        private T5[] _componentGroup5;
        private T6[] _componentGroup6;
        private T7[] _componentGroup7;
        private T8[] _componentGroup8;
        private T9[] _componentGroup9;
        private T10[] _componentGroup10;
        private T11[] _componentGroup11;
        private T12[] _componentGroup12;
        private T13[] _componentGroup13;
        private T14[] _componentGroup14;
        private T15[] _componentGroup15;
        private T16[] _componentGroup16;
        private T17[] _componentGroup17;
        private T18[] _componentGroup18;
        private T19[] _componentGroup19;
        private T20[] _componentGroup20;
        private T21[] _componentGroup21;
        private T22[] _componentGroup22;
        private T23[] _componentGroup23;

        public DataGroup(int dataCount)
        {
            _dataCount = dataCount;

            _componentGroup0 = new T0[_dataCount];
            _componentGroup1 = new T1[_dataCount];
            _componentGroup2 = new T2[_dataCount];
            _componentGroup3 = new T3[_dataCount];
            _componentGroup4 = new T4[_dataCount];
            _componentGroup5 = new T5[_dataCount];
            _componentGroup6 = new T6[_dataCount];
            _componentGroup7 = new T7[_dataCount];
            _componentGroup8 = new T8[_dataCount];
            _componentGroup9 = new T9[_dataCount];
            _componentGroup10 = new T10[_dataCount];
            _componentGroup11 = new T11[_dataCount];
            _componentGroup12 = new T12[_dataCount];
            _componentGroup13 = new T13[_dataCount];
            _componentGroup14 = new T14[_dataCount];
            _componentGroup15 = new T15[_dataCount];
            _componentGroup16 = new T16[_dataCount];
            _componentGroup17 = new T17[_dataCount];
            _componentGroup18 = new T18[_dataCount];
            _componentGroup19 = new T19[_dataCount];
            _componentGroup20 = new T20[_dataCount];
            _componentGroup21 = new T21[_dataCount];
            _componentGroup22 = new T22[_dataCount];
            _componentGroup23 = new T23[_dataCount];
        }

        public override IComponent DataAtIndexByType<T>(int index)
        {
            if (index >= 0 && index < _dataCount)
            {
                Type typeT = typeof(T);

                if (typeT == typeof(T0)) return _componentGroup0[index];
                else if (typeT == typeof(T1)) return _componentGroup1[index];
                else if (typeT == typeof(T2)) return _componentGroup2[index];
                else if (typeT == typeof(T3)) return _componentGroup3[index];
                else if (typeT == typeof(T4)) return _componentGroup4[index];
                else if (typeT == typeof(T5)) return _componentGroup5[index];
                else if (typeT == typeof(T6)) return _componentGroup6[index];
                else if (typeT == typeof(T7)) return _componentGroup7[index];
                else if (typeT == typeof(T8)) return _componentGroup8[index];
                else if (typeT == typeof(T9)) return _componentGroup9[index];
                else if (typeT == typeof(T10)) return _componentGroup10[index];
                else if (typeT == typeof(T11)) return _componentGroup11[index];
                else if (typeT == typeof(T12)) return _componentGroup12[index];
                else if (typeT == typeof(T13)) return _componentGroup13[index];
                else if (typeT == typeof(T14)) return _componentGroup14[index];
                else if (typeT == typeof(T15)) return _componentGroup15[index];
                else if (typeT == typeof(T16)) return _componentGroup16[index];
                else if (typeT == typeof(T17)) return _componentGroup17[index];
                else if (typeT == typeof(T18)) return _componentGroup18[index];
                else if (typeT == typeof(T19)) return _componentGroup19[index];
                else if (typeT == typeof(T20)) return _componentGroup20[index];
                else if (typeT == typeof(T21)) return _componentGroup21[index];
                else if (typeT == typeof(T22)) return _componentGroup22[index];
                else if (typeT == typeof(T23)) return _componentGroup23[index];
            }
            return null;
        }

        public override IComponent DataAtIndexByArray(int index, int arrayID)
        {
            if (index >= 0 && index < _dataCount)
            {
                if (arrayID >= 0 && arrayID < ARRAY_COUNT)
                {
                    switch (arrayID)
                    {
                        case (0): return _componentGroup0[index];
                        case (1): return _componentGroup1[index];
                        case (2): return _componentGroup2[index];
                        case (3): return _componentGroup3[index];
                        case (4): return _componentGroup4[index];
                        case (5): return _componentGroup5[index];
                        case (6): return _componentGroup6[index];
                        case (7): return _componentGroup7[index];
                        case (8): return _componentGroup8[index];
                        case (9): return _componentGroup9[index];
                        case (10): return _componentGroup10[index];
                        case (11): return _componentGroup11[index];
                        case (12): return _componentGroup12[index];
                        case (13): return _componentGroup13[index];
                        case (14): return _componentGroup14[index];
                        case (15): return _componentGroup15[index];
                        case (16): return _componentGroup16[index];
                        case (17): return _componentGroup17[index];
                        case (18): return _componentGroup18[index];
                        case (19): return _componentGroup19[index];
                        case (20): return _componentGroup20[index];
                        case (21): return _componentGroup21[index];
                        case (22): return _componentGroup22[index];
                        case (23): return _componentGroup23[index];

                        default: return null;
                    }
                }
            }

            return null;
        }
    }

    public class DataGroup<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24> : DataGroup
                                                                where T0 : IComponent
                                                                where T1 : IComponent
                                                                where T2 : IComponent
                                                                where T3 : IComponent
                                                                where T4 : IComponent
                                                                where T5 : IComponent
                                                                where T6 : IComponent
                                                                where T7 : IComponent
                                                                where T8 : IComponent
                                                                where T9 : IComponent
                                                                where T10 : IComponent
                                                                where T11 : IComponent
                                                                where T12 : IComponent
                                                                where T13 : IComponent
                                                                where T14 : IComponent
                                                                where T15 : IComponent
                                                                where T16 : IComponent
                                                                where T17 : IComponent
                                                                where T18 : IComponent
                                                                where T19 : IComponent
                                                                where T20 : IComponent
                                                                where T21 : IComponent
                                                                where T22 : IComponent
                                                                where T23 : IComponent
                                                                where T24 : IComponent
    {
        public const int ARRAY_COUNT = 25;

        private T0[] _componentGroup0;
        private T1[] _componentGroup1;
        private T2[] _componentGroup2;
        private T3[] _componentGroup3;
        private T4[] _componentGroup4;
        private T5[] _componentGroup5;
        private T6[] _componentGroup6;
        private T7[] _componentGroup7;
        private T8[] _componentGroup8;
        private T9[] _componentGroup9;
        private T10[] _componentGroup10;
        private T11[] _componentGroup11;
        private T12[] _componentGroup12;
        private T13[] _componentGroup13;
        private T14[] _componentGroup14;
        private T15[] _componentGroup15;
        private T16[] _componentGroup16;
        private T17[] _componentGroup17;
        private T18[] _componentGroup18;
        private T19[] _componentGroup19;
        private T20[] _componentGroup20;
        private T21[] _componentGroup21;
        private T22[] _componentGroup22;
        private T23[] _componentGroup23;
        private T24[] _componentGroup24;

        public DataGroup(int dataCount)
        {
            _dataCount = dataCount;

            _componentGroup0 = new T0[_dataCount];
            _componentGroup1 = new T1[_dataCount];
            _componentGroup2 = new T2[_dataCount];
            _componentGroup3 = new T3[_dataCount];
            _componentGroup4 = new T4[_dataCount];
            _componentGroup5 = new T5[_dataCount];
            _componentGroup6 = new T6[_dataCount];
            _componentGroup7 = new T7[_dataCount];
            _componentGroup8 = new T8[_dataCount];
            _componentGroup9 = new T9[_dataCount];
            _componentGroup10 = new T10[_dataCount];
            _componentGroup11 = new T11[_dataCount];
            _componentGroup12 = new T12[_dataCount];
            _componentGroup13 = new T13[_dataCount];
            _componentGroup14 = new T14[_dataCount];
            _componentGroup15 = new T15[_dataCount];
            _componentGroup16 = new T16[_dataCount];
            _componentGroup17 = new T17[_dataCount];
            _componentGroup18 = new T18[_dataCount];
            _componentGroup19 = new T19[_dataCount];
            _componentGroup20 = new T20[_dataCount];
            _componentGroup21 = new T21[_dataCount];
            _componentGroup22 = new T22[_dataCount];
            _componentGroup23 = new T23[_dataCount];
            _componentGroup24 = new T24[_dataCount];
        }

        public override IComponent DataAtIndexByType<T>(int index)
        {
            if (index >= 0 && index < _dataCount)
            {
                Type typeT = typeof(T);

                if (typeT == typeof(T0)) return _componentGroup0[index];
                else if (typeT == typeof(T1)) return _componentGroup1[index];
                else if (typeT == typeof(T2)) return _componentGroup2[index];
                else if (typeT == typeof(T3)) return _componentGroup3[index];
                else if (typeT == typeof(T4)) return _componentGroup4[index];
                else if (typeT == typeof(T5)) return _componentGroup5[index];
                else if (typeT == typeof(T6)) return _componentGroup6[index];
                else if (typeT == typeof(T7)) return _componentGroup7[index];
                else if (typeT == typeof(T8)) return _componentGroup8[index];
                else if (typeT == typeof(T9)) return _componentGroup9[index];
                else if (typeT == typeof(T10)) return _componentGroup10[index];
                else if (typeT == typeof(T11)) return _componentGroup11[index];
                else if (typeT == typeof(T12)) return _componentGroup12[index];
                else if (typeT == typeof(T13)) return _componentGroup13[index];
                else if (typeT == typeof(T14)) return _componentGroup14[index];
                else if (typeT == typeof(T15)) return _componentGroup15[index];
                else if (typeT == typeof(T16)) return _componentGroup16[index];
                else if (typeT == typeof(T17)) return _componentGroup17[index];
                else if (typeT == typeof(T18)) return _componentGroup18[index];
                else if (typeT == typeof(T19)) return _componentGroup19[index];
                else if (typeT == typeof(T20)) return _componentGroup20[index];
                else if (typeT == typeof(T21)) return _componentGroup21[index];
                else if (typeT == typeof(T22)) return _componentGroup22[index];
                else if (typeT == typeof(T23)) return _componentGroup23[index];
                else if (typeT == typeof(T24)) return _componentGroup24[index];
            }
            return null;
        }

        public override IComponent DataAtIndexByArray(int index, int arrayID)
        {
            if (index >= 0 && index < _dataCount)
            {
                if (arrayID >= 0 && arrayID < ARRAY_COUNT)
                {
                    switch (arrayID)
                    {
                        case (0): return _componentGroup0[index];
                        case (1): return _componentGroup1[index];
                        case (2): return _componentGroup2[index];
                        case (3): return _componentGroup3[index];
                        case (4): return _componentGroup4[index];
                        case (5): return _componentGroup5[index];
                        case (6): return _componentGroup6[index];
                        case (7): return _componentGroup7[index];
                        case (8): return _componentGroup8[index];
                        case (9): return _componentGroup9[index];
                        case (10): return _componentGroup10[index];
                        case (11): return _componentGroup11[index];
                        case (12): return _componentGroup12[index];
                        case (13): return _componentGroup13[index];
                        case (14): return _componentGroup14[index];
                        case (15): return _componentGroup15[index];
                        case (16): return _componentGroup16[index];
                        case (17): return _componentGroup17[index];
                        case (18): return _componentGroup18[index];
                        case (19): return _componentGroup19[index];
                        case (20): return _componentGroup20[index];
                        case (21): return _componentGroup21[index];
                        case (22): return _componentGroup22[index];
                        case (23): return _componentGroup23[index];
                        case (24): return _componentGroup24[index];

                        default: return null;
                    }
                }
            }

            return null;
        }
    }

    public class DataGroup<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25> : DataGroup
                                                                where T0 : IComponent
                                                                where T1 : IComponent
                                                                where T2 : IComponent
                                                                where T3 : IComponent
                                                                where T4 : IComponent
                                                                where T5 : IComponent
                                                                where T6 : IComponent
                                                                where T7 : IComponent
                                                                where T8 : IComponent
                                                                where T9 : IComponent
                                                                where T10 : IComponent
                                                                where T11 : IComponent
                                                                where T12 : IComponent
                                                                where T13 : IComponent
                                                                where T14 : IComponent
                                                                where T15 : IComponent
                                                                where T16 : IComponent
                                                                where T17 : IComponent
                                                                where T18 : IComponent
                                                                where T19 : IComponent
                                                                where T20 : IComponent
                                                                where T21 : IComponent
                                                                where T22 : IComponent
                                                                where T23 : IComponent
                                                                where T24 : IComponent
                                                                where T25 : IComponent
    {
        public const int ARRAY_COUNT = 26;

        private T0[] _componentGroup0;
        private T1[] _componentGroup1;
        private T2[] _componentGroup2;
        private T3[] _componentGroup3;
        private T4[] _componentGroup4;
        private T5[] _componentGroup5;
        private T6[] _componentGroup6;
        private T7[] _componentGroup7;
        private T8[] _componentGroup8;
        private T9[] _componentGroup9;
        private T10[] _componentGroup10;
        private T11[] _componentGroup11;
        private T12[] _componentGroup12;
        private T13[] _componentGroup13;
        private T14[] _componentGroup14;
        private T15[] _componentGroup15;
        private T16[] _componentGroup16;
        private T17[] _componentGroup17;
        private T18[] _componentGroup18;
        private T19[] _componentGroup19;
        private T20[] _componentGroup20;
        private T21[] _componentGroup21;
        private T22[] _componentGroup22;
        private T23[] _componentGroup23;
        private T24[] _componentGroup24;
        private T25[] _componentGroup25;

        public DataGroup(int dataCount)
        {
            _dataCount = dataCount;

            _componentGroup0 = new T0[_dataCount];
            _componentGroup1 = new T1[_dataCount];
            _componentGroup2 = new T2[_dataCount];
            _componentGroup3 = new T3[_dataCount];
            _componentGroup4 = new T4[_dataCount];
            _componentGroup5 = new T5[_dataCount];
            _componentGroup6 = new T6[_dataCount];
            _componentGroup7 = new T7[_dataCount];
            _componentGroup8 = new T8[_dataCount];
            _componentGroup9 = new T9[_dataCount];
            _componentGroup10 = new T10[_dataCount];
            _componentGroup11 = new T11[_dataCount];
            _componentGroup12 = new T12[_dataCount];
            _componentGroup13 = new T13[_dataCount];
            _componentGroup14 = new T14[_dataCount];
            _componentGroup15 = new T15[_dataCount];
            _componentGroup16 = new T16[_dataCount];
            _componentGroup17 = new T17[_dataCount];
            _componentGroup18 = new T18[_dataCount];
            _componentGroup19 = new T19[_dataCount];
            _componentGroup20 = new T20[_dataCount];
            _componentGroup21 = new T21[_dataCount];
            _componentGroup22 = new T22[_dataCount];
            _componentGroup23 = new T23[_dataCount];
            _componentGroup24 = new T24[_dataCount];
            _componentGroup25 = new T25[_dataCount];
        }

        public override IComponent DataAtIndexByType<T>(int index)
        {
            if (index >= 0 && index < _dataCount)
            {
                Type typeT = typeof(T);

                if (typeT == typeof(T0)) return _componentGroup0[index];
                else if (typeT == typeof(T1)) return _componentGroup1[index];
                else if (typeT == typeof(T2)) return _componentGroup2[index];
                else if (typeT == typeof(T3)) return _componentGroup3[index];
                else if (typeT == typeof(T4)) return _componentGroup4[index];
                else if (typeT == typeof(T5)) return _componentGroup5[index];
                else if (typeT == typeof(T6)) return _componentGroup6[index];
                else if (typeT == typeof(T7)) return _componentGroup7[index];
                else if (typeT == typeof(T8)) return _componentGroup8[index];
                else if (typeT == typeof(T9)) return _componentGroup9[index];
                else if (typeT == typeof(T10)) return _componentGroup10[index];
                else if (typeT == typeof(T11)) return _componentGroup11[index];
                else if (typeT == typeof(T12)) return _componentGroup12[index];
                else if (typeT == typeof(T13)) return _componentGroup13[index];
                else if (typeT == typeof(T14)) return _componentGroup14[index];
                else if (typeT == typeof(T15)) return _componentGroup15[index];
                else if (typeT == typeof(T16)) return _componentGroup16[index];
                else if (typeT == typeof(T17)) return _componentGroup17[index];
                else if (typeT == typeof(T18)) return _componentGroup18[index];
                else if (typeT == typeof(T19)) return _componentGroup19[index];
                else if (typeT == typeof(T20)) return _componentGroup20[index];
                else if (typeT == typeof(T21)) return _componentGroup21[index];
                else if (typeT == typeof(T22)) return _componentGroup22[index];
                else if (typeT == typeof(T23)) return _componentGroup23[index];
                else if (typeT == typeof(T24)) return _componentGroup24[index];
                else if (typeT == typeof(T25)) return _componentGroup25[index];
            }
            return null;
        }

        public override IComponent DataAtIndexByArray(int index, int arrayID)
        {
            if (index >= 0 && index < _dataCount)
            {
                if (arrayID >= 0 && arrayID < ARRAY_COUNT)
                {
                    switch (arrayID)
                    {
                        case (0): return _componentGroup0[index];
                        case (1): return _componentGroup1[index];
                        case (2): return _componentGroup2[index];
                        case (3): return _componentGroup3[index];
                        case (4): return _componentGroup4[index];
                        case (5): return _componentGroup5[index];
                        case (6): return _componentGroup6[index];
                        case (7): return _componentGroup7[index];
                        case (8): return _componentGroup8[index];
                        case (9): return _componentGroup9[index];
                        case (10): return _componentGroup10[index];
                        case (11): return _componentGroup11[index];
                        case (12): return _componentGroup12[index];
                        case (13): return _componentGroup13[index];
                        case (14): return _componentGroup14[index];
                        case (15): return _componentGroup15[index];
                        case (16): return _componentGroup16[index];
                        case (17): return _componentGroup17[index];
                        case (18): return _componentGroup18[index];
                        case (19): return _componentGroup19[index];
                        case (20): return _componentGroup20[index];
                        case (21): return _componentGroup21[index];
                        case (22): return _componentGroup22[index];
                        case (23): return _componentGroup23[index];
                        case (24): return _componentGroup24[index];
                        case (25): return _componentGroup25[index];

                        default: return null;
                    }
                }
            }

            return null;
        }
    }

    public class DataGroup<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26> : DataGroup
                                                                where T0 : IComponent
                                                                where T1 : IComponent
                                                                where T2 : IComponent
                                                                where T3 : IComponent
                                                                where T4 : IComponent
                                                                where T5 : IComponent
                                                                where T6 : IComponent
                                                                where T7 : IComponent
                                                                where T8 : IComponent
                                                                where T9 : IComponent
                                                                where T10 : IComponent
                                                                where T11 : IComponent
                                                                where T12 : IComponent
                                                                where T13 : IComponent
                                                                where T14 : IComponent
                                                                where T15 : IComponent
                                                                where T16 : IComponent
                                                                where T17 : IComponent
                                                                where T18 : IComponent
                                                                where T19 : IComponent
                                                                where T20 : IComponent
                                                                where T21 : IComponent
                                                                where T22 : IComponent
                                                                where T23 : IComponent
                                                                where T24 : IComponent
                                                                where T25 : IComponent
                                                                where T26 : IComponent
    {
        public const int ARRAY_COUNT = 27;

        private T0[] _componentGroup0;
        private T1[] _componentGroup1;
        private T2[] _componentGroup2;
        private T3[] _componentGroup3;
        private T4[] _componentGroup4;
        private T5[] _componentGroup5;
        private T6[] _componentGroup6;
        private T7[] _componentGroup7;
        private T8[] _componentGroup8;
        private T9[] _componentGroup9;
        private T10[] _componentGroup10;
        private T11[] _componentGroup11;
        private T12[] _componentGroup12;
        private T13[] _componentGroup13;
        private T14[] _componentGroup14;
        private T15[] _componentGroup15;
        private T16[] _componentGroup16;
        private T17[] _componentGroup17;
        private T18[] _componentGroup18;
        private T19[] _componentGroup19;
        private T20[] _componentGroup20;
        private T21[] _componentGroup21;
        private T22[] _componentGroup22;
        private T23[] _componentGroup23;
        private T24[] _componentGroup24;
        private T25[] _componentGroup25;
        private T26[] _componentGroup26;

        public DataGroup(int dataCount)
        {
            _dataCount = dataCount;

            _componentGroup0 = new T0[_dataCount];
            _componentGroup1 = new T1[_dataCount];
            _componentGroup2 = new T2[_dataCount];
            _componentGroup3 = new T3[_dataCount];
            _componentGroup4 = new T4[_dataCount];
            _componentGroup5 = new T5[_dataCount];
            _componentGroup6 = new T6[_dataCount];
            _componentGroup7 = new T7[_dataCount];
            _componentGroup8 = new T8[_dataCount];
            _componentGroup9 = new T9[_dataCount];
            _componentGroup10 = new T10[_dataCount];
            _componentGroup11 = new T11[_dataCount];
            _componentGroup12 = new T12[_dataCount];
            _componentGroup13 = new T13[_dataCount];
            _componentGroup14 = new T14[_dataCount];
            _componentGroup15 = new T15[_dataCount];
            _componentGroup16 = new T16[_dataCount];
            _componentGroup17 = new T17[_dataCount];
            _componentGroup18 = new T18[_dataCount];
            _componentGroup19 = new T19[_dataCount];
            _componentGroup20 = new T20[_dataCount];
            _componentGroup21 = new T21[_dataCount];
            _componentGroup22 = new T22[_dataCount];
            _componentGroup23 = new T23[_dataCount];
            _componentGroup24 = new T24[_dataCount];
            _componentGroup25 = new T25[_dataCount];
            _componentGroup26 = new T26[_dataCount];
        }

        public override IComponent DataAtIndexByType<T>(int index)
        {
            if (index >= 0 && index < _dataCount)
            {
                Type typeT = typeof(T);

                if (typeT == typeof(T0)) return _componentGroup0[index];
                else if (typeT == typeof(T1)) return _componentGroup1[index];
                else if (typeT == typeof(T2)) return _componentGroup2[index];
                else if (typeT == typeof(T3)) return _componentGroup3[index];
                else if (typeT == typeof(T4)) return _componentGroup4[index];
                else if (typeT == typeof(T5)) return _componentGroup5[index];
                else if (typeT == typeof(T6)) return _componentGroup6[index];
                else if (typeT == typeof(T7)) return _componentGroup7[index];
                else if (typeT == typeof(T8)) return _componentGroup8[index];
                else if (typeT == typeof(T9)) return _componentGroup9[index];
                else if (typeT == typeof(T10)) return _componentGroup10[index];
                else if (typeT == typeof(T11)) return _componentGroup11[index];
                else if (typeT == typeof(T12)) return _componentGroup12[index];
                else if (typeT == typeof(T13)) return _componentGroup13[index];
                else if (typeT == typeof(T14)) return _componentGroup14[index];
                else if (typeT == typeof(T15)) return _componentGroup15[index];
                else if (typeT == typeof(T16)) return _componentGroup16[index];
                else if (typeT == typeof(T17)) return _componentGroup17[index];
                else if (typeT == typeof(T18)) return _componentGroup18[index];
                else if (typeT == typeof(T19)) return _componentGroup19[index];
                else if (typeT == typeof(T20)) return _componentGroup20[index];
                else if (typeT == typeof(T21)) return _componentGroup21[index];
                else if (typeT == typeof(T22)) return _componentGroup22[index];
                else if (typeT == typeof(T23)) return _componentGroup23[index];
                else if (typeT == typeof(T24)) return _componentGroup24[index];
                else if (typeT == typeof(T25)) return _componentGroup25[index];
                else if (typeT == typeof(T26)) return _componentGroup26[index];
            }
            return null;
        }

        public override IComponent DataAtIndexByArray(int index, int arrayID)
        {
            if (index >= 0 && index < _dataCount)
            {
                if (arrayID >= 0 && arrayID < ARRAY_COUNT)
                {
                    switch (arrayID)
                    {
                        case (0): return _componentGroup0[index];
                        case (1): return _componentGroup1[index];
                        case (2): return _componentGroup2[index];
                        case (3): return _componentGroup3[index];
                        case (4): return _componentGroup4[index];
                        case (5): return _componentGroup5[index];
                        case (6): return _componentGroup6[index];
                        case (7): return _componentGroup7[index];
                        case (8): return _componentGroup8[index];
                        case (9): return _componentGroup9[index];
                        case (10): return _componentGroup10[index];
                        case (11): return _componentGroup11[index];
                        case (12): return _componentGroup12[index];
                        case (13): return _componentGroup13[index];
                        case (14): return _componentGroup14[index];
                        case (15): return _componentGroup15[index];
                        case (16): return _componentGroup16[index];
                        case (17): return _componentGroup17[index];
                        case (18): return _componentGroup18[index];
                        case (19): return _componentGroup19[index];
                        case (20): return _componentGroup20[index];
                        case (21): return _componentGroup21[index];
                        case (22): return _componentGroup22[index];
                        case (23): return _componentGroup23[index];
                        case (24): return _componentGroup24[index];
                        case (25): return _componentGroup25[index];
                        case (26): return _componentGroup26[index];

                        default: return null;
                    }
                }
            }

            return null;
        }
    }

    public class DataGroup<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27> : DataGroup
                                                                where T0 : IComponent
                                                                where T1 : IComponent
                                                                where T2 : IComponent
                                                                where T3 : IComponent
                                                                where T4 : IComponent
                                                                where T5 : IComponent
                                                                where T6 : IComponent
                                                                where T7 : IComponent
                                                                where T8 : IComponent
                                                                where T9 : IComponent
                                                                where T10 : IComponent
                                                                where T11 : IComponent
                                                                where T12 : IComponent
                                                                where T13 : IComponent
                                                                where T14 : IComponent
                                                                where T15 : IComponent
                                                                where T16 : IComponent
                                                                where T17 : IComponent
                                                                where T18 : IComponent
                                                                where T19 : IComponent
                                                                where T20 : IComponent
                                                                where T21 : IComponent
                                                                where T22 : IComponent
                                                                where T23 : IComponent
                                                                where T24 : IComponent
                                                                where T25 : IComponent
                                                                where T26 : IComponent
                                                                where T27 : IComponent
    {
        public const int ARRAY_COUNT = 28;

        private T0[] _componentGroup0;
        private T1[] _componentGroup1;
        private T2[] _componentGroup2;
        private T3[] _componentGroup3;
        private T4[] _componentGroup4;
        private T5[] _componentGroup5;
        private T6[] _componentGroup6;
        private T7[] _componentGroup7;
        private T8[] _componentGroup8;
        private T9[] _componentGroup9;
        private T10[] _componentGroup10;
        private T11[] _componentGroup11;
        private T12[] _componentGroup12;
        private T13[] _componentGroup13;
        private T14[] _componentGroup14;
        private T15[] _componentGroup15;
        private T16[] _componentGroup16;
        private T17[] _componentGroup17;
        private T18[] _componentGroup18;
        private T19[] _componentGroup19;
        private T20[] _componentGroup20;
        private T21[] _componentGroup21;
        private T22[] _componentGroup22;
        private T23[] _componentGroup23;
        private T24[] _componentGroup24;
        private T25[] _componentGroup25;
        private T26[] _componentGroup26;
        private T27[] _componentGroup27;

        public DataGroup(int dataCount)
        {
            _dataCount = dataCount;

            _componentGroup0 = new T0[_dataCount];
            _componentGroup1 = new T1[_dataCount];
            _componentGroup2 = new T2[_dataCount];
            _componentGroup3 = new T3[_dataCount];
            _componentGroup4 = new T4[_dataCount];
            _componentGroup5 = new T5[_dataCount];
            _componentGroup6 = new T6[_dataCount];
            _componentGroup7 = new T7[_dataCount];
            _componentGroup8 = new T8[_dataCount];
            _componentGroup9 = new T9[_dataCount];
            _componentGroup10 = new T10[_dataCount];
            _componentGroup11 = new T11[_dataCount];
            _componentGroup12 = new T12[_dataCount];
            _componentGroup13 = new T13[_dataCount];
            _componentGroup14 = new T14[_dataCount];
            _componentGroup15 = new T15[_dataCount];
            _componentGroup16 = new T16[_dataCount];
            _componentGroup17 = new T17[_dataCount];
            _componentGroup18 = new T18[_dataCount];
            _componentGroup19 = new T19[_dataCount];
            _componentGroup20 = new T20[_dataCount];
            _componentGroup21 = new T21[_dataCount];
            _componentGroup22 = new T22[_dataCount];
            _componentGroup23 = new T23[_dataCount];
            _componentGroup24 = new T24[_dataCount];
            _componentGroup25 = new T25[_dataCount];
            _componentGroup26 = new T26[_dataCount];
            _componentGroup27 = new T27[_dataCount];
        }

        public override IComponent DataAtIndexByType<T>(int index)
        {
            if (index >= 0 && index < _dataCount)
            {
                Type typeT = typeof(T);

                if (typeT == typeof(T0)) return _componentGroup0[index];
                else if (typeT == typeof(T1)) return _componentGroup1[index];
                else if (typeT == typeof(T2)) return _componentGroup2[index];
                else if (typeT == typeof(T3)) return _componentGroup3[index];
                else if (typeT == typeof(T4)) return _componentGroup4[index];
                else if (typeT == typeof(T5)) return _componentGroup5[index];
                else if (typeT == typeof(T6)) return _componentGroup6[index];
                else if (typeT == typeof(T7)) return _componentGroup7[index];
                else if (typeT == typeof(T8)) return _componentGroup8[index];
                else if (typeT == typeof(T9)) return _componentGroup9[index];
                else if (typeT == typeof(T10)) return _componentGroup10[index];
                else if (typeT == typeof(T11)) return _componentGroup11[index];
                else if (typeT == typeof(T12)) return _componentGroup12[index];
                else if (typeT == typeof(T13)) return _componentGroup13[index];
                else if (typeT == typeof(T14)) return _componentGroup14[index];
                else if (typeT == typeof(T15)) return _componentGroup15[index];
                else if (typeT == typeof(T16)) return _componentGroup16[index];
                else if (typeT == typeof(T17)) return _componentGroup17[index];
                else if (typeT == typeof(T18)) return _componentGroup18[index];
                else if (typeT == typeof(T19)) return _componentGroup19[index];
                else if (typeT == typeof(T20)) return _componentGroup20[index];
                else if (typeT == typeof(T21)) return _componentGroup21[index];
                else if (typeT == typeof(T22)) return _componentGroup22[index];
                else if (typeT == typeof(T23)) return _componentGroup23[index];
                else if (typeT == typeof(T24)) return _componentGroup24[index];
                else if (typeT == typeof(T25)) return _componentGroup25[index];
                else if (typeT == typeof(T26)) return _componentGroup26[index];
                else if (typeT == typeof(T27)) return _componentGroup27[index];
            }
            return null;
        }

        public override IComponent DataAtIndexByArray(int index, int arrayID)
        {
            if (index >= 0 && index < _dataCount)
            {
                if (arrayID >= 0 && arrayID < ARRAY_COUNT)
                {
                    switch (arrayID)
                    {
                        case (0): return _componentGroup0[index];
                        case (1): return _componentGroup1[index];
                        case (2): return _componentGroup2[index];
                        case (3): return _componentGroup3[index];
                        case (4): return _componentGroup4[index];
                        case (5): return _componentGroup5[index];
                        case (6): return _componentGroup6[index];
                        case (7): return _componentGroup7[index];
                        case (8): return _componentGroup8[index];
                        case (9): return _componentGroup9[index];
                        case (10): return _componentGroup10[index];
                        case (11): return _componentGroup11[index];
                        case (12): return _componentGroup12[index];
                        case (13): return _componentGroup13[index];
                        case (14): return _componentGroup14[index];
                        case (15): return _componentGroup15[index];
                        case (16): return _componentGroup16[index];
                        case (17): return _componentGroup17[index];
                        case (18): return _componentGroup18[index];
                        case (19): return _componentGroup19[index];
                        case (20): return _componentGroup20[index];
                        case (21): return _componentGroup21[index];
                        case (22): return _componentGroup22[index];
                        case (23): return _componentGroup23[index];
                        case (24): return _componentGroup24[index];
                        case (25): return _componentGroup25[index];
                        case (26): return _componentGroup26[index];
                        case (27): return _componentGroup27[index];

                        default: return null;
                    }
                }
            }

            return null;
        }
    }

    public class DataGroup<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28> : DataGroup
                                                                where T0 : IComponent
                                                                where T1 : IComponent
                                                                where T2 : IComponent
                                                                where T3 : IComponent
                                                                where T4 : IComponent
                                                                where T5 : IComponent
                                                                where T6 : IComponent
                                                                where T7 : IComponent
                                                                where T8 : IComponent
                                                                where T9 : IComponent
                                                                where T10 : IComponent
                                                                where T11 : IComponent
                                                                where T12 : IComponent
                                                                where T13 : IComponent
                                                                where T14 : IComponent
                                                                where T15 : IComponent
                                                                where T16 : IComponent
                                                                where T17 : IComponent
                                                                where T18 : IComponent
                                                                where T19 : IComponent
                                                                where T20 : IComponent
                                                                where T21 : IComponent
                                                                where T22 : IComponent
                                                                where T23 : IComponent
                                                                where T24 : IComponent
                                                                where T25 : IComponent
                                                                where T26 : IComponent
                                                                where T27 : IComponent
                                                                where T28 : IComponent
    {
        public const int ARRAY_COUNT = 29;

        private T0[] _componentGroup0;
        private T1[] _componentGroup1;
        private T2[] _componentGroup2;
        private T3[] _componentGroup3;
        private T4[] _componentGroup4;
        private T5[] _componentGroup5;
        private T6[] _componentGroup6;
        private T7[] _componentGroup7;
        private T8[] _componentGroup8;
        private T9[] _componentGroup9;
        private T10[] _componentGroup10;
        private T11[] _componentGroup11;
        private T12[] _componentGroup12;
        private T13[] _componentGroup13;
        private T14[] _componentGroup14;
        private T15[] _componentGroup15;
        private T16[] _componentGroup16;
        private T17[] _componentGroup17;
        private T18[] _componentGroup18;
        private T19[] _componentGroup19;
        private T20[] _componentGroup20;
        private T21[] _componentGroup21;
        private T22[] _componentGroup22;
        private T23[] _componentGroup23;
        private T24[] _componentGroup24;
        private T25[] _componentGroup25;
        private T26[] _componentGroup26;
        private T27[] _componentGroup27;
        private T28[] _componentGroup28;

        public DataGroup(int dataCount)
        {
            _dataCount = dataCount;

            _componentGroup0 = new T0[_dataCount];
            _componentGroup1 = new T1[_dataCount];
            _componentGroup2 = new T2[_dataCount];
            _componentGroup3 = new T3[_dataCount];
            _componentGroup4 = new T4[_dataCount];
            _componentGroup5 = new T5[_dataCount];
            _componentGroup6 = new T6[_dataCount];
            _componentGroup7 = new T7[_dataCount];
            _componentGroup8 = new T8[_dataCount];
            _componentGroup9 = new T9[_dataCount];
            _componentGroup10 = new T10[_dataCount];
            _componentGroup11 = new T11[_dataCount];
            _componentGroup12 = new T12[_dataCount];
            _componentGroup13 = new T13[_dataCount];
            _componentGroup14 = new T14[_dataCount];
            _componentGroup15 = new T15[_dataCount];
            _componentGroup16 = new T16[_dataCount];
            _componentGroup17 = new T17[_dataCount];
            _componentGroup18 = new T18[_dataCount];
            _componentGroup19 = new T19[_dataCount];
            _componentGroup20 = new T20[_dataCount];
            _componentGroup21 = new T21[_dataCount];
            _componentGroup22 = new T22[_dataCount];
            _componentGroup23 = new T23[_dataCount];
            _componentGroup24 = new T24[_dataCount];
            _componentGroup25 = new T25[_dataCount];
            _componentGroup26 = new T26[_dataCount];
            _componentGroup27 = new T27[_dataCount];
            _componentGroup28 = new T28[_dataCount];
        }

        public override IComponent DataAtIndexByType<T>(int index)
        {
            if (index >= 0 && index < _dataCount)
            {
                Type typeT = typeof(T);

                if (typeT == typeof(T0)) return _componentGroup0[index];
                else if (typeT == typeof(T1)) return _componentGroup1[index];
                else if (typeT == typeof(T2)) return _componentGroup2[index];
                else if (typeT == typeof(T3)) return _componentGroup3[index];
                else if (typeT == typeof(T4)) return _componentGroup4[index];
                else if (typeT == typeof(T5)) return _componentGroup5[index];
                else if (typeT == typeof(T6)) return _componentGroup6[index];
                else if (typeT == typeof(T7)) return _componentGroup7[index];
                else if (typeT == typeof(T8)) return _componentGroup8[index];
                else if (typeT == typeof(T9)) return _componentGroup9[index];
                else if (typeT == typeof(T10)) return _componentGroup10[index];
                else if (typeT == typeof(T11)) return _componentGroup11[index];
                else if (typeT == typeof(T12)) return _componentGroup12[index];
                else if (typeT == typeof(T13)) return _componentGroup13[index];
                else if (typeT == typeof(T14)) return _componentGroup14[index];
                else if (typeT == typeof(T15)) return _componentGroup15[index];
                else if (typeT == typeof(T16)) return _componentGroup16[index];
                else if (typeT == typeof(T17)) return _componentGroup17[index];
                else if (typeT == typeof(T18)) return _componentGroup18[index];
                else if (typeT == typeof(T19)) return _componentGroup19[index];
                else if (typeT == typeof(T20)) return _componentGroup20[index];
                else if (typeT == typeof(T21)) return _componentGroup21[index];
                else if (typeT == typeof(T22)) return _componentGroup22[index];
                else if (typeT == typeof(T23)) return _componentGroup23[index];
                else if (typeT == typeof(T24)) return _componentGroup24[index];
                else if (typeT == typeof(T25)) return _componentGroup25[index];
                else if (typeT == typeof(T26)) return _componentGroup26[index];
                else if (typeT == typeof(T27)) return _componentGroup27[index];
                else if (typeT == typeof(T28)) return _componentGroup28[index];
            }
            return null;
        }

        public override IComponent DataAtIndexByArray(int index, int arrayID)
        {
            if (index >= 0 && index < _dataCount)
            {
                if (arrayID >= 0 && arrayID < ARRAY_COUNT)
                {
                    switch (arrayID)
                    {
                        case (0): return _componentGroup0[index];
                        case (1): return _componentGroup1[index];
                        case (2): return _componentGroup2[index];
                        case (3): return _componentGroup3[index];
                        case (4): return _componentGroup4[index];
                        case (5): return _componentGroup5[index];
                        case (6): return _componentGroup6[index];
                        case (7): return _componentGroup7[index];
                        case (8): return _componentGroup8[index];
                        case (9): return _componentGroup9[index];
                        case (10): return _componentGroup10[index];
                        case (11): return _componentGroup11[index];
                        case (12): return _componentGroup12[index];
                        case (13): return _componentGroup13[index];
                        case (14): return _componentGroup14[index];
                        case (15): return _componentGroup15[index];
                        case (16): return _componentGroup16[index];
                        case (17): return _componentGroup17[index];
                        case (18): return _componentGroup18[index];
                        case (19): return _componentGroup19[index];
                        case (20): return _componentGroup20[index];
                        case (21): return _componentGroup21[index];
                        case (22): return _componentGroup22[index];
                        case (23): return _componentGroup23[index];
                        case (24): return _componentGroup24[index];
                        case (25): return _componentGroup25[index];
                        case (26): return _componentGroup26[index];
                        case (27): return _componentGroup27[index];
                        case (28): return _componentGroup28[index];

                        default: return null;
                    }
                }
            }

            return null;
        }
    }

    public class DataGroup<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29> : DataGroup
                                                                where T0 : IComponent
                                                                where T1 : IComponent
                                                                where T2 : IComponent
                                                                where T3 : IComponent
                                                                where T4 : IComponent
                                                                where T5 : IComponent
                                                                where T6 : IComponent
                                                                where T7 : IComponent
                                                                where T8 : IComponent
                                                                where T9 : IComponent
                                                                where T10 : IComponent
                                                                where T11 : IComponent
                                                                where T12 : IComponent
                                                                where T13 : IComponent
                                                                where T14 : IComponent
                                                                where T15 : IComponent
                                                                where T16 : IComponent
                                                                where T17 : IComponent
                                                                where T18 : IComponent
                                                                where T19 : IComponent
                                                                where T20 : IComponent
                                                                where T21 : IComponent
                                                                where T22 : IComponent
                                                                where T23 : IComponent
                                                                where T24 : IComponent
                                                                where T25 : IComponent
                                                                where T26 : IComponent
                                                                where T27 : IComponent
                                                                where T28 : IComponent
                                                                where T29 : IComponent
    {
        public const int ARRAY_COUNT = 30;

        private T0[] _componentGroup0;
        private T1[] _componentGroup1;
        private T2[] _componentGroup2;
        private T3[] _componentGroup3;
        private T4[] _componentGroup4;
        private T5[] _componentGroup5;
        private T6[] _componentGroup6;
        private T7[] _componentGroup7;
        private T8[] _componentGroup8;
        private T9[] _componentGroup9;
        private T10[] _componentGroup10;
        private T11[] _componentGroup11;
        private T12[] _componentGroup12;
        private T13[] _componentGroup13;
        private T14[] _componentGroup14;
        private T15[] _componentGroup15;
        private T16[] _componentGroup16;
        private T17[] _componentGroup17;
        private T18[] _componentGroup18;
        private T19[] _componentGroup19;
        private T20[] _componentGroup20;
        private T21[] _componentGroup21;
        private T22[] _componentGroup22;
        private T23[] _componentGroup23;
        private T24[] _componentGroup24;
        private T25[] _componentGroup25;
        private T26[] _componentGroup26;
        private T27[] _componentGroup27;
        private T28[] _componentGroup28;
        private T29[] _componentGroup29;

        public DataGroup(int dataCount)
        {
            _dataCount = dataCount;

            _componentGroup0 = new T0[_dataCount];
            _componentGroup1 = new T1[_dataCount];
            _componentGroup2 = new T2[_dataCount];
            _componentGroup3 = new T3[_dataCount];
            _componentGroup4 = new T4[_dataCount];
            _componentGroup5 = new T5[_dataCount];
            _componentGroup6 = new T6[_dataCount];
            _componentGroup7 = new T7[_dataCount];
            _componentGroup8 = new T8[_dataCount];
            _componentGroup9 = new T9[_dataCount];
            _componentGroup10 = new T10[_dataCount];
            _componentGroup11 = new T11[_dataCount];
            _componentGroup12 = new T12[_dataCount];
            _componentGroup13 = new T13[_dataCount];
            _componentGroup14 = new T14[_dataCount];
            _componentGroup15 = new T15[_dataCount];
            _componentGroup16 = new T16[_dataCount];
            _componentGroup17 = new T17[_dataCount];
            _componentGroup18 = new T18[_dataCount];
            _componentGroup19 = new T19[_dataCount];
            _componentGroup20 = new T20[_dataCount];
            _componentGroup21 = new T21[_dataCount];
            _componentGroup22 = new T22[_dataCount];
            _componentGroup23 = new T23[_dataCount];
            _componentGroup24 = new T24[_dataCount];
            _componentGroup25 = new T25[_dataCount];
            _componentGroup26 = new T26[_dataCount];
            _componentGroup27 = new T27[_dataCount];
            _componentGroup28 = new T28[_dataCount];
            _componentGroup29 = new T29[_dataCount];
        }

        public override IComponent DataAtIndexByType<T>(int index)
        {
            if (index >= 0 && index < _dataCount)
            {
                Type typeT = typeof(T);

                if (typeT == typeof(T0)) return _componentGroup0[index];
                else if (typeT == typeof(T1)) return _componentGroup1[index];
                else if (typeT == typeof(T2)) return _componentGroup2[index];
                else if (typeT == typeof(T3)) return _componentGroup3[index];
                else if (typeT == typeof(T4)) return _componentGroup4[index];
                else if (typeT == typeof(T5)) return _componentGroup5[index];
                else if (typeT == typeof(T6)) return _componentGroup6[index];
                else if (typeT == typeof(T7)) return _componentGroup7[index];
                else if (typeT == typeof(T8)) return _componentGroup8[index];
                else if (typeT == typeof(T9)) return _componentGroup9[index];
                else if (typeT == typeof(T10)) return _componentGroup10[index];
                else if (typeT == typeof(T11)) return _componentGroup11[index];
                else if (typeT == typeof(T12)) return _componentGroup12[index];
                else if (typeT == typeof(T13)) return _componentGroup13[index];
                else if (typeT == typeof(T14)) return _componentGroup14[index];
                else if (typeT == typeof(T15)) return _componentGroup15[index];
                else if (typeT == typeof(T16)) return _componentGroup16[index];
                else if (typeT == typeof(T17)) return _componentGroup17[index];
                else if (typeT == typeof(T18)) return _componentGroup18[index];
                else if (typeT == typeof(T19)) return _componentGroup19[index];
                else if (typeT == typeof(T20)) return _componentGroup20[index];
                else if (typeT == typeof(T21)) return _componentGroup21[index];
                else if (typeT == typeof(T22)) return _componentGroup22[index];
                else if (typeT == typeof(T23)) return _componentGroup23[index];
                else if (typeT == typeof(T24)) return _componentGroup24[index];
                else if (typeT == typeof(T25)) return _componentGroup25[index];
                else if (typeT == typeof(T26)) return _componentGroup26[index];
                else if (typeT == typeof(T27)) return _componentGroup27[index];
                else if (typeT == typeof(T28)) return _componentGroup28[index];
                else if (typeT == typeof(T29)) return _componentGroup29[index];
            }
            return null;
        }

        public override IComponent DataAtIndexByArray(int index, int arrayID)
        {
            if (index >= 0 && index < _dataCount)
            {
                if (arrayID >= 0 && arrayID < ARRAY_COUNT)
                {
                    switch (arrayID)
                    {
                        case (0): return _componentGroup0[index];
                        case (1): return _componentGroup1[index];
                        case (2): return _componentGroup2[index];
                        case (3): return _componentGroup3[index];
                        case (4): return _componentGroup4[index];
                        case (5): return _componentGroup5[index];
                        case (6): return _componentGroup6[index];
                        case (7): return _componentGroup7[index];
                        case (8): return _componentGroup8[index];
                        case (9): return _componentGroup9[index];
                        case (10): return _componentGroup10[index];
                        case (11): return _componentGroup11[index];
                        case (12): return _componentGroup12[index];
                        case (13): return _componentGroup13[index];
                        case (14): return _componentGroup14[index];
                        case (15): return _componentGroup15[index];
                        case (16): return _componentGroup16[index];
                        case (17): return _componentGroup17[index];
                        case (18): return _componentGroup18[index];
                        case (19): return _componentGroup19[index];
                        case (20): return _componentGroup20[index];
                        case (21): return _componentGroup21[index];
                        case (22): return _componentGroup22[index];
                        case (23): return _componentGroup23[index];
                        case (24): return _componentGroup24[index];
                        case (25): return _componentGroup25[index];
                        case (26): return _componentGroup26[index];
                        case (27): return _componentGroup27[index];
                        case (28): return _componentGroup28[index];
                        case (29): return _componentGroup29[index];

                        default: return null;
                    }
                }
            }

            return null;
        }
    }

    public class DataGroup<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30> : DataGroup
                                                                where T0 : IComponent
                                                                where T1 : IComponent
                                                                where T2 : IComponent
                                                                where T3 : IComponent
                                                                where T4 : IComponent
                                                                where T5 : IComponent
                                                                where T6 : IComponent
                                                                where T7 : IComponent
                                                                where T8 : IComponent
                                                                where T9 : IComponent
                                                                where T10 : IComponent
                                                                where T11 : IComponent
                                                                where T12 : IComponent
                                                                where T13 : IComponent
                                                                where T14 : IComponent
                                                                where T15 : IComponent
                                                                where T16 : IComponent
                                                                where T17 : IComponent
                                                                where T18 : IComponent
                                                                where T19 : IComponent
                                                                where T20 : IComponent
                                                                where T21 : IComponent
                                                                where T22 : IComponent
                                                                where T23 : IComponent
                                                                where T24 : IComponent
                                                                where T25 : IComponent
                                                                where T26 : IComponent
                                                                where T27 : IComponent
                                                                where T28 : IComponent
                                                                where T29 : IComponent
                                                                where T30 : IComponent
    {
        public const int ARRAY_COUNT = 31;

        private T0[] _componentGroup0;
        private T1[] _componentGroup1;
        private T2[] _componentGroup2;
        private T3[] _componentGroup3;
        private T4[] _componentGroup4;
        private T5[] _componentGroup5;
        private T6[] _componentGroup6;
        private T7[] _componentGroup7;
        private T8[] _componentGroup8;
        private T9[] _componentGroup9;
        private T10[] _componentGroup10;
        private T11[] _componentGroup11;
        private T12[] _componentGroup12;
        private T13[] _componentGroup13;
        private T14[] _componentGroup14;
        private T15[] _componentGroup15;
        private T16[] _componentGroup16;
        private T17[] _componentGroup17;
        private T18[] _componentGroup18;
        private T19[] _componentGroup19;
        private T20[] _componentGroup20;
        private T21[] _componentGroup21;
        private T22[] _componentGroup22;
        private T23[] _componentGroup23;
        private T24[] _componentGroup24;
        private T25[] _componentGroup25;
        private T26[] _componentGroup26;
        private T27[] _componentGroup27;
        private T28[] _componentGroup28;
        private T29[] _componentGroup29;
        private T30[] _componentGroup30;

        public DataGroup(int dataCount)
        {
            _dataCount = dataCount;

            _componentGroup0 = new T0[_dataCount];
            _componentGroup1 = new T1[_dataCount];
            _componentGroup2 = new T2[_dataCount];
            _componentGroup3 = new T3[_dataCount];
            _componentGroup4 = new T4[_dataCount];
            _componentGroup5 = new T5[_dataCount];
            _componentGroup6 = new T6[_dataCount];
            _componentGroup7 = new T7[_dataCount];
            _componentGroup8 = new T8[_dataCount];
            _componentGroup9 = new T9[_dataCount];
            _componentGroup10 = new T10[_dataCount];
            _componentGroup11 = new T11[_dataCount];
            _componentGroup12 = new T12[_dataCount];
            _componentGroup13 = new T13[_dataCount];
            _componentGroup14 = new T14[_dataCount];
            _componentGroup15 = new T15[_dataCount];
            _componentGroup16 = new T16[_dataCount];
            _componentGroup17 = new T17[_dataCount];
            _componentGroup18 = new T18[_dataCount];
            _componentGroup19 = new T19[_dataCount];
            _componentGroup20 = new T20[_dataCount];
            _componentGroup21 = new T21[_dataCount];
            _componentGroup22 = new T22[_dataCount];
            _componentGroup23 = new T23[_dataCount];
            _componentGroup24 = new T24[_dataCount];
            _componentGroup25 = new T25[_dataCount];
            _componentGroup26 = new T26[_dataCount];
            _componentGroup27 = new T27[_dataCount];
            _componentGroup28 = new T28[_dataCount];
            _componentGroup29 = new T29[_dataCount];
            _componentGroup30 = new T30[_dataCount];
        }

        public override IComponent DataAtIndexByType<T>(int index)
        {
            if (index >= 0 && index < _dataCount)
            {
                Type typeT = typeof(T);

                if (typeT == typeof(T0)) return _componentGroup0[index];
                else if (typeT == typeof(T1)) return _componentGroup1[index];
                else if (typeT == typeof(T2)) return _componentGroup2[index];
                else if (typeT == typeof(T3)) return _componentGroup3[index];
                else if (typeT == typeof(T4)) return _componentGroup4[index];
                else if (typeT == typeof(T5)) return _componentGroup5[index];
                else if (typeT == typeof(T6)) return _componentGroup6[index];
                else if (typeT == typeof(T7)) return _componentGroup7[index];
                else if (typeT == typeof(T8)) return _componentGroup8[index];
                else if (typeT == typeof(T9)) return _componentGroup9[index];
                else if (typeT == typeof(T10)) return _componentGroup10[index];
                else if (typeT == typeof(T11)) return _componentGroup11[index];
                else if (typeT == typeof(T12)) return _componentGroup12[index];
                else if (typeT == typeof(T13)) return _componentGroup13[index];
                else if (typeT == typeof(T14)) return _componentGroup14[index];
                else if (typeT == typeof(T15)) return _componentGroup15[index];
                else if (typeT == typeof(T16)) return _componentGroup16[index];
                else if (typeT == typeof(T17)) return _componentGroup17[index];
                else if (typeT == typeof(T18)) return _componentGroup18[index];
                else if (typeT == typeof(T19)) return _componentGroup19[index];
                else if (typeT == typeof(T20)) return _componentGroup20[index];
                else if (typeT == typeof(T21)) return _componentGroup21[index];
                else if (typeT == typeof(T22)) return _componentGroup22[index];
                else if (typeT == typeof(T23)) return _componentGroup23[index];
                else if (typeT == typeof(T24)) return _componentGroup24[index];
                else if (typeT == typeof(T25)) return _componentGroup25[index];
                else if (typeT == typeof(T26)) return _componentGroup26[index];
                else if (typeT == typeof(T27)) return _componentGroup27[index];
                else if (typeT == typeof(T28)) return _componentGroup28[index];
                else if (typeT == typeof(T29)) return _componentGroup29[index];
                else if (typeT == typeof(T30)) return _componentGroup30[index];
            }
            return null;
        }

        public override IComponent DataAtIndexByArray(int index, int arrayID)
        {
            if (index >= 0 && index < _dataCount)
            {
                if (arrayID >= 0 && arrayID < ARRAY_COUNT)
                {
                    switch (arrayID)
                    {
                        case (0): return _componentGroup0[index];
                        case (1): return _componentGroup1[index];
                        case (2): return _componentGroup2[index];
                        case (3): return _componentGroup3[index];
                        case (4): return _componentGroup4[index];
                        case (5): return _componentGroup5[index];
                        case (6): return _componentGroup6[index];
                        case (7): return _componentGroup7[index];
                        case (8): return _componentGroup8[index];
                        case (9): return _componentGroup9[index];
                        case (10): return _componentGroup10[index];
                        case (11): return _componentGroup11[index];
                        case (12): return _componentGroup12[index];
                        case (13): return _componentGroup13[index];
                        case (14): return _componentGroup14[index];
                        case (15): return _componentGroup15[index];
                        case (16): return _componentGroup16[index];
                        case (17): return _componentGroup17[index];
                        case (18): return _componentGroup18[index];
                        case (19): return _componentGroup19[index];
                        case (20): return _componentGroup20[index];
                        case (21): return _componentGroup21[index];
                        case (22): return _componentGroup22[index];
                        case (23): return _componentGroup23[index];
                        case (24): return _componentGroup24[index];
                        case (25): return _componentGroup25[index];
                        case (26): return _componentGroup26[index];
                        case (27): return _componentGroup27[index];
                        case (28): return _componentGroup28[index];
                        case (29): return _componentGroup29[index];
                        case (30): return _componentGroup30[index];

                        default: return null;
                    }
                }
            }

            return null;
        }
    }

    public class DataGroup<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31> : DataGroup
                                                                where T0 : IComponent
                                                                where T1 : IComponent
                                                                where T2 : IComponent
                                                                where T3 : IComponent
                                                                where T4 : IComponent
                                                                where T5 : IComponent
                                                                where T6 : IComponent
                                                                where T7 : IComponent
                                                                where T8 : IComponent
                                                                where T9 : IComponent
                                                                where T10 : IComponent
                                                                where T11 : IComponent
                                                                where T12 : IComponent
                                                                where T13 : IComponent
                                                                where T14 : IComponent
                                                                where T15 : IComponent
                                                                where T16 : IComponent
                                                                where T17 : IComponent
                                                                where T18 : IComponent
                                                                where T19 : IComponent
                                                                where T20 : IComponent
                                                                where T21 : IComponent
                                                                where T22 : IComponent
                                                                where T23 : IComponent
                                                                where T24 : IComponent
                                                                where T25 : IComponent
                                                                where T26 : IComponent
                                                                where T27 : IComponent
                                                                where T28 : IComponent
                                                                where T29 : IComponent
                                                                where T30 : IComponent
                                                                where T31 : IComponent
    {
        public const int ARRAY_COUNT = 32;

        private T0[] _componentGroup0;
        private T1[] _componentGroup1;
        private T2[] _componentGroup2;
        private T3[] _componentGroup3;
        private T4[] _componentGroup4;
        private T5[] _componentGroup5;
        private T6[] _componentGroup6;
        private T7[] _componentGroup7;
        private T8[] _componentGroup8;
        private T9[] _componentGroup9;
        private T10[] _componentGroup10;
        private T11[] _componentGroup11;
        private T12[] _componentGroup12;
        private T13[] _componentGroup13;
        private T14[] _componentGroup14;
        private T15[] _componentGroup15;
        private T16[] _componentGroup16;
        private T17[] _componentGroup17;
        private T18[] _componentGroup18;
        private T19[] _componentGroup19;
        private T20[] _componentGroup20;
        private T21[] _componentGroup21;
        private T22[] _componentGroup22;
        private T23[] _componentGroup23;
        private T24[] _componentGroup24;
        private T25[] _componentGroup25;
        private T26[] _componentGroup26;
        private T27[] _componentGroup27;
        private T28[] _componentGroup28;
        private T29[] _componentGroup29;
        private T30[] _componentGroup30;
        private T31[] _componentGroup31;

        public DataGroup(int dataCount)
        {
            _dataCount = dataCount;

            _componentGroup0 = new T0[_dataCount];
            _componentGroup1 = new T1[_dataCount];
            _componentGroup2 = new T2[_dataCount];
            _componentGroup3 = new T3[_dataCount];
            _componentGroup4 = new T4[_dataCount];
            _componentGroup5 = new T5[_dataCount];
            _componentGroup6 = new T6[_dataCount];
            _componentGroup7 = new T7[_dataCount];
            _componentGroup8 = new T8[_dataCount];
            _componentGroup9 = new T9[_dataCount];
            _componentGroup10 = new T10[_dataCount];
            _componentGroup11 = new T11[_dataCount];
            _componentGroup12 = new T12[_dataCount];
            _componentGroup13 = new T13[_dataCount];
            _componentGroup14 = new T14[_dataCount];
            _componentGroup15 = new T15[_dataCount];
            _componentGroup16 = new T16[_dataCount];
            _componentGroup17 = new T17[_dataCount];
            _componentGroup18 = new T18[_dataCount];
            _componentGroup19 = new T19[_dataCount];
            _componentGroup20 = new T20[_dataCount];
            _componentGroup21 = new T21[_dataCount];
            _componentGroup22 = new T22[_dataCount];
            _componentGroup23 = new T23[_dataCount];
            _componentGroup24 = new T24[_dataCount];
            _componentGroup25 = new T25[_dataCount];
            _componentGroup26 = new T26[_dataCount];
            _componentGroup27 = new T27[_dataCount];
            _componentGroup28 = new T28[_dataCount];
            _componentGroup29 = new T29[_dataCount];
            _componentGroup30 = new T30[_dataCount];
            _componentGroup31 = new T31[_dataCount];
        }

        public override IComponent DataAtIndexByType<T>(int index)
        {
            if (index >= 0 && index < _dataCount)
            {
                Type typeT = typeof(T);

                if (typeT == typeof(T0)) return _componentGroup0[index];
                else if (typeT == typeof(T1)) return _componentGroup1[index];
                else if (typeT == typeof(T2)) return _componentGroup2[index];
                else if (typeT == typeof(T3)) return _componentGroup3[index];
                else if (typeT == typeof(T4)) return _componentGroup4[index];
                else if (typeT == typeof(T5)) return _componentGroup5[index];
                else if (typeT == typeof(T6)) return _componentGroup6[index];
                else if (typeT == typeof(T7)) return _componentGroup7[index];
                else if (typeT == typeof(T8)) return _componentGroup8[index];
                else if (typeT == typeof(T9)) return _componentGroup9[index];
                else if (typeT == typeof(T10)) return _componentGroup10[index];
                else if (typeT == typeof(T11)) return _componentGroup11[index];
                else if (typeT == typeof(T12)) return _componentGroup12[index];
                else if (typeT == typeof(T13)) return _componentGroup13[index];
                else if (typeT == typeof(T14)) return _componentGroup14[index];
                else if (typeT == typeof(T15)) return _componentGroup15[index];
                else if (typeT == typeof(T16)) return _componentGroup16[index];
                else if (typeT == typeof(T17)) return _componentGroup17[index];
                else if (typeT == typeof(T18)) return _componentGroup18[index];
                else if (typeT == typeof(T19)) return _componentGroup19[index];
                else if (typeT == typeof(T20)) return _componentGroup20[index];
                else if (typeT == typeof(T21)) return _componentGroup21[index];
                else if (typeT == typeof(T22)) return _componentGroup22[index];
                else if (typeT == typeof(T23)) return _componentGroup23[index];
                else if (typeT == typeof(T24)) return _componentGroup24[index];
                else if (typeT == typeof(T25)) return _componentGroup25[index];
                else if (typeT == typeof(T26)) return _componentGroup26[index];
                else if (typeT == typeof(T27)) return _componentGroup27[index];
                else if (typeT == typeof(T28)) return _componentGroup28[index];
                else if (typeT == typeof(T29)) return _componentGroup29[index];
                else if (typeT == typeof(T30)) return _componentGroup30[index];
                else if (typeT == typeof(T31)) return _componentGroup31[index];
            }
            return null;
        }

        public override IComponent DataAtIndexByArray(int index, int arrayID)
        {
            if (index >= 0 && index < _dataCount)
            {
                if (arrayID >= 0 && arrayID < ARRAY_COUNT)
                {
                    switch (arrayID)
                    {
                        case (0): return _componentGroup0[index];
                        case (1): return _componentGroup1[index];
                        case (2): return _componentGroup2[index];
                        case (3): return _componentGroup3[index];
                        case (4): return _componentGroup4[index];
                        case (5): return _componentGroup5[index];
                        case (6): return _componentGroup6[index];
                        case (7): return _componentGroup7[index];
                        case (8): return _componentGroup8[index];
                        case (9): return _componentGroup9[index];
                        case (10): return _componentGroup10[index];
                        case (11): return _componentGroup11[index];
                        case (12): return _componentGroup12[index];
                        case (13): return _componentGroup13[index];
                        case (14): return _componentGroup14[index];
                        case (15): return _componentGroup15[index];
                        case (16): return _componentGroup16[index];
                        case (17): return _componentGroup17[index];
                        case (18): return _componentGroup18[index];
                        case (19): return _componentGroup19[index];
                        case (20): return _componentGroup20[index];
                        case (21): return _componentGroup21[index];
                        case (22): return _componentGroup22[index];
                        case (23): return _componentGroup23[index];
                        case (24): return _componentGroup24[index];
                        case (25): return _componentGroup25[index];
                        case (26): return _componentGroup26[index];
                        case (27): return _componentGroup27[index];
                        case (28): return _componentGroup28[index];
                        case (29): return _componentGroup29[index];
                        case (30): return _componentGroup30[index];
                        case (31): return _componentGroup31[index];

                        default: return null;
                    }
                }
            }

            return null;
        }
    }
}
