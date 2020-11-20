using DeeSynk.Core.Components.Types.Fonts;
using System;
using System.Collections.Generic;
using System.Text;

namespace DeeSynk.Core.Components.Fonts
{
    public static class DataHelper
    {
        public static bool ExistsAtLocation(in byte[] data, int start, int count, int compare) { return GetAtLocationInt(in data, start, count) == compare; }

        public static long GetAtLocationLong(in byte[] data, int start, int count)
        {
            if (count > 8) throw new ArgumentOutOfRangeException("The number of bytes read can only correspond to a 64-bit value.");
            long d = 0;
            for (int idx = 0; idx < count; idx++)
                d += (int)(data[idx + start] << (8 * (count - 1 - idx)));
            return d;
        }

        public static long GetAtLocationLong(in byte[] data, int start, int count, out int newStart)
        {
            if (count > 8) throw new ArgumentOutOfRangeException("The number of bytes read can only correspond to a 64-bit value.");
            long d = 0;
            for (int idx = 0; idx < count; idx++)
                d += (int)(data[idx + start] << (8 * (count - 1 - idx)));
            newStart = start + count;
            return d;
        }

        public static long GetAtLocationLong(in byte[] data, int start) //assumes count is 8
        {
            long d = 0;
            for (int idx = 0; idx < 8; idx++)
                d += (int)(data[idx + start] << (8 * (7 - idx)));
            return d;
        }

        public static long GetAtLocationLong(in byte[] data, int start, out int newStart) //assumes count is 8
        {
            long d = 0;
            for (int idx = 0; idx < 8; idx++)
                d += (int)(data[idx + start] << (8 * (7 - idx)));
            newStart = start + 8;
            return d;
        }

        public static int GetAtLocationInt(in byte[] data, int start, int count)
        {
            if (count > 4) throw new ArgumentOutOfRangeException("The number of bytes read can only correspond to a 32-bit value.");
            int d = 0;
            for (int idx = 0; idx < count; idx++)
                d += (int)(data[idx + start] << (8 * (count - 1 - idx)));
            return d;
        }

        public static int GetAtLocationInt(in byte[] data, int start, int count, out int newStart)
        {
            if (count > 4) throw new ArgumentOutOfRangeException("The number of bytes read can only correspond to a 32-bit value.");
            int d = 0;
            for (int idx = 0; idx < count; idx++)
                d += (int)(data[idx + start] << (8 * (count - 1 - idx)));
            newStart = start + count;
            return d;
        }

        public static int GetAtLocationInt(in byte[] data, int start) //assumes count is 4
        {
            int d = 0;
            for (int idx = 0; idx < 4; idx++)
                d += (int)(data[idx + start] << (8 * (3 - idx)));
            return d;
        }

        public static int GetAtLocationInt(in byte[] data, int start, out int newStart) //assumes count is 4
        {
            int d = 0;
            for (int idx = 0; idx < 4; idx++)
                d += (int)(data[idx + start] << (8 * (3 - idx)));
            newStart = start + 4;
            return d;
        }

        public static short GetAtLocationShort(in byte[] data, int start, int count)
        {
            if (count > 2) throw new ArgumentOutOfRangeException("The number of bytes read can only correspond to a 16-bit value.");
            short d = 0;
            for (int idx = 0; idx < count; idx++)
                d += (short)(data[idx + start] << (8 * (count - 1 - idx)));
            return d;
        }

        public static short GetAtLocationShort(in byte[] data, int start, int count, out int newStart)
        {
            if (count > 2) throw new ArgumentOutOfRangeException("The number of bytes read can only correspond to a 16-bit value.");
            short d = 0;
            for (int idx = 0; idx < count; idx++)
                d += (short)(data[idx + start] << (8 * (count - 1 - idx)));
            newStart = start + count;
            return d;
        }

        public static short GetAtLocationShort(in byte[] data, int start)
        {
            short d = 0;
            for (int idx = 0; idx < 2; idx++)
                d += (short)(data[idx + start] << (8 * (1 - idx)));
            return d;
        }

        public static short GetAtLocationShort(in byte[] data, int start, out int newStart)
        {
            short d = 0;
            for (int idx = 0; idx < 2; idx++)
                d += (short)(data[idx + start] << (8 * (1 - idx)));
            newStart = start + 2;
            return d;
        }

        public static bool HasOTTOHeader(in byte[] data) { return ExistsAtLocation(in data, 0, 4, Font.OTTO); }
    }
}
