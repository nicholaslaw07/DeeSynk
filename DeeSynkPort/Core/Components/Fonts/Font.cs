using System;
using System.Collections.Generic;
using System.Text;

namespace DeeSynk.Core.Components.Types.Fonts
{
    public class Font
    {
        #region FILE_HEADER
        public static readonly int OTTO = 0x4f54544f;

        public static readonly int CFF = 0x43464620;
        public static readonly int FFTM = 0x4646544d;
        public static readonly int GDEF = 0x47444546;
        public static readonly int GPOS = 0x47504f53;
        public static readonly int GSUB = 0x47535542;
        public static readonly int OSs2 = 0x4f532f32;
        public static readonly int cmap = 0x636d6170;
        public static readonly int head = 0x68656164;
        public static readonly int hhea = 0x68686561;
        public static readonly int hmtx = 0x686d7478;
        public static readonly int maxp = 0x6d617870;
        public static readonly int name = 0x6e616d65;
        public static readonly int post = 0x706f7374;

        public static readonly int[] headerNames = { CFF, FFTM, GDEF, GPOS, GSUB, OSs2, cmap, head, hhea, hmtx, maxp, name, post };
        #endregion

        private string _path;
        public string Path { get => _path; }

        private string _name;
        public string Name { get => _name; }

        private List<FileHeaderEntry> _headerEntires;
        public List<FileHeaderEntry> HeaderEntires { get => _headerEntires; set => _headerEntires = value; }

        private CFFTable _cffTable;
        public CFFTable CFFTable { get => _cffTable; set => _cffTable = value; }

        public Font(string filePath, string name)
        {
            _name = name;
        }
    }
}
