using System;
using System.Collections.Generic;
using System.Text;

namespace DeeSynk.Core.Components.Fonts.Tables.CFF
{
    public class CFFCharsets: List<short>
    {
        private byte _format;
        public byte Format { get => _format; }

        public CFFCharsets(in byte[] data, int startIndex, int nGlyphs, out int newStart) : base()
        {
            newStart = startIndex;
            _format = data[startIndex];
            int idx = 0;
            switch (_format)
            {
                case (0):
                    for (newStart = startIndex + 1; newStart < startIndex + 2 * nGlyphs; newStart += 2)
                        Add((short)(data[newStart] << 8 | data[newStart + 1]));
                    break;
                case (1):
                    while (idx < nGlyphs)
                    {
                        short first = (short)(data[newStart] << 8 | data[newStart + 1]);
                        byte nLeft = data[newStart + 2];
                        for (byte jdx = 0; jdx <= nLeft; jdx++)
                            Add((short)(first + jdx));
                        idx += nLeft + 1;
                        newStart += 3;
                    }
                    break;
                case (2):
                    while (idx < nGlyphs)
                    {
                        short first = (short)(data[newStart] << 8 | data[newStart + 1]);
                        short nLeft = (short)(data[newStart + 2] << 8 | data[newStart + 3]);
                        for (short jdx = 0; jdx <= nLeft; jdx++)
                            Add((short)(first + jdx));
                        idx += nLeft + 1;
                        newStart += 4;
                    }
                    break;
            }
        }

        public CFFCharsets(in byte[] data, int startIndex, int nGlyphs) : base()
        {
            int newStart = startIndex;
            _format = data[startIndex];
            int idx = 0;
            switch (_format)
            {
                case (0):
                    for (newStart = startIndex + 1; newStart < startIndex + 2 * nGlyphs; newStart += 2)
                        Add((short)(data[newStart] << 8 | data[newStart + 1]));
                    break;
                case (1):
                    while (idx < nGlyphs)
                    {
                        short first = (short)(data[newStart] << 8 | data[newStart + 1]);
                        byte nLeft = data[newStart + 2];
                        for (byte jdx = 0; jdx <= nLeft; jdx++)
                            Add((short)(first + jdx));
                        idx += nLeft + 1;
                        newStart += 3;
                    }
                    break;
                case (2):
                    while (idx < nGlyphs)
                    {
                        short first = (short)(data[newStart] << 8 | data[newStart + 1]);
                        short nLeft = (short)(data[newStart + 2] << 8 | data[newStart + 3]);
                        for (short jdx = 0; jdx <= nLeft; jdx++)
                            Add((short)(first + jdx));
                        idx += nLeft + 1;
                        newStart += 4;
                    }
                    break;
            }
        }
    }
}
