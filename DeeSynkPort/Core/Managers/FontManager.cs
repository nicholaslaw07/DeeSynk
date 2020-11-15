using DeeSynk.Core.Components.Types.Fonts;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace DeeSynk.Core.Managers
{
    public class FontManager : IManager
    {

        private static FontManager _fontManager;

        #region STANDARD_VARS
        private readonly int _off1 = 1;
        private readonly int _off2 = 2;
        private readonly int _off4 = 4;
        #endregion

        private FontManager()
        {

        }

        public static ref FontManager GetInstance()
        {
            if (_fontManager == null)
                _fontManager = new FontManager();

            return ref _fontManager;
        }

        public void Load()
        {
            LoadFontGeometry(@"C:\Users\Nicholas\source\repos\nicholaslaw07\DeeSynk\DeeSynkPort\Resources\Fonts\OfficeCodePro-Light\OfficeCodePro-Light.otf", "OfficeCodePro-Light.otf");
        }

        public void UnLoad()
        {
            throw new NotImplementedException();
        }

        #region Parser
        private void LoadFontGeometry(string path, string name)
        {
            byte[] file = File.ReadAllBytes(path);

            Font font = new Font(name);

            if (!HasOTTOHeader(in file))
                throw new Exception("File does not contain valid header for OpenType format");

            {
                GetFileHeaderEntries(in file, _off4 * 3, out List<FileHeaderEntry> mainHeaderList);
                font.HeaderEntires = mainHeaderList;
            }

            foreach(FileHeaderEntry entry in font.HeaderEntires)
            {
                switch (entry.Table)
                {
                    case (0x43464620 /*CFF*/): font.CFFTable = ParseCFFTable(in file, entry); break;
                    default: break;
                }
            }
        }

        private bool ExistsAtLocation(in byte[] data, int start, int count, int compare) { return GetAtLocation4(in data, start, count) == compare; }

        private int GetAtLocation4(in byte[] data, int start, int count)
        {
            if (count > 4) throw new ArgumentOutOfRangeException("The number of bytes read can only correspond to a 32-bit value.");
            int d = 0;
            for (int idx = 0; idx < count; idx++)
                d += (int)(data[idx + start] << (8 * (count - 1 - idx)));
            return d;
        }

        private short GetAtLocation2(in byte[] data, int start, int count)
        {
            if (count > 2) throw new ArgumentOutOfRangeException("The number of bytes read can only correspond to a 16-bit value.");
            short d = 0;
            for (int idx = 0; idx < count; idx++)
                d += (short)(data[idx + start] << (8 * (count - 1 - idx)));
            return d;
        }

        private bool HasOTTOHeader(in byte[] data) { return ExistsAtLocation(in data, 0, 4, Font.OTTO); }

        private void GetFileHeaderEntries(in byte[] data, int start, out List<FileHeaderEntry> headerEntries)
        {
            int offset = start - _off4;
            bool stillChecking = true;
            headerEntries = new List<FileHeaderEntry>();
            List<int> hNames = new List<int>(Font.headerNames);
            do
            {
                int testVal = GetAtLocation4(in data, offset += _off4, _off4);
                stillChecking = hNames.Contains(testVal);
                if (stillChecking)
                {
                    int table = testVal;
                    int checkSum = GetAtLocation4(in data, offset += _off4, _off4);
                    int off = GetAtLocation4(in data, offset += _off4, _off4);
                    int size = GetAtLocation4(in data, offset += _off4, _off4);
                    headerEntries.Add(new FileHeaderEntry(table, checkSum, off, size));
                }
            } while (stillChecking);
        }

        private CFFTable ParseCFFTable(in byte[] data, FileHeaderEntry entry)
        {
            int startIndex = entry.Offset;
            CFFTable table = new CFFTable();
            table.CFFHeader = new CFFHeader(data[startIndex + 0], data[startIndex + 1], data[startIndex + 2], data[startIndex + 3]);
            startIndex += table.CFFHeader.HeaderSize;
            table.IndexName = ParseCFFIndex(in data, startIndex, out startIndex);
            return table;
        }

        private CFFIndex ParseCFFIndex(in byte[] data, int startIndex, out int newStart)
        {
            newStart = startIndex;
            short count = GetAtLocation2(in data, startIndex, 2);
            byte offset = data[newStart += _off2];
            newStart += _off1;
            CFFIndex index = new CFFIndex(count, offset);
            int dataSize = 0;
            for(int idx = 0; idx < index.Offsets.Length; idx++)
            {
                int value = GetAtLocation4(in data, newStart, offset)-1;
                index.Offsets[idx] = value;
                newStart += offset;
                dataSize += value;
            }
            index.Data = new byte[dataSize];
            for(int idx = 0; idx < index.Data.Length; idx++)
                index.Data[idx] = data[newStart + idx];

            return index;
        }

        private byte[] GetSubsetLE(in byte[] data, int offset, int count)
        {
            byte[] val = new byte[count];
            for (int i = 0; i < count; i++)
                val[i] = data[offset + count - 1 - i];
            return val;
        }
        private byte[] GetSubsetBE(in byte[] data, int offset, int count)
        {
            byte[] val = new byte[count];
            for (int i = 0; i < count; i++)
                val[i] = data[offset + i];
            return val;
        }
        private void GetSubsetLE(in byte[] data, int offset, int count, out byte[] val)
        {
            val = new byte[count];
            for (int i = 0; i < count; i++)
                val[i] = data[offset + count - 1 - i];
        }
        private void GetSubsetBE(in byte[] data, int offset, int count, out byte[] val)
        {
            val = new byte[count];
            for (int i = 0; i < count; i++)
                val[i] = data[offset + i];
        }
        #endregion
    }
}
