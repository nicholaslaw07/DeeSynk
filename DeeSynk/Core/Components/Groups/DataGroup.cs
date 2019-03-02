using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DeeSynk.Core.Components.Types;

namespace DeeSynk.Core.Components.Groups
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
}
