using DeeSynk.Core.Components.Types.Fonts;
using System;
using System.Collections.Generic;
using System.Text;

namespace DeeSynk.Core.Components.Fonts.Tables
{
    public class MaximumProfile
    {
        public const int VERSION_0_5 = 0x00005000;
        public const int VERSION_1_0 = 0x00010000;

        //Required components (0.5 & 1.0)
        private int _version, _numGlyphs;
        public int Version { get => _version; }
        public int NumberOfGlyphs { get => _numGlyphs; }

        //Version 1.0 only
        private int _maxPoints, _maxContours;
        public int MaxPoints { get => _maxPoints; }
        public int MaxContours { get => _maxContours; }

        private int _maxCompositePoints, _maxCompositeContours;
        public int MaxCompositePoints { get => _maxCompositePoints; }
        public int MaxCompositeContours { get => _maxCompositeContours; }

        private int _maxZones, _maxTwilightPoints, _maxStorage;
        public int MaxZones { get => _maxZones; }
        public int MaxTwilightPoints { get => _maxTwilightPoints; }
        public int MaxStorage { get => _maxStorage; }

        private int _maxFunctionDefs, _maxInstructionDefs;
        public int MaxFunctionDefs { get => _maxFunctionDefs; }
        public int MaxInstructionDefs { get => _maxInstructionDefs; }

        private int _maxStackElements, _maxSizeOfInstructions;
        public int MaxStackElements { get => _maxStackElements; }
        public int MaxSizeOfInstructions { get => _maxSizeOfInstructions; }

        private int _maxComponentElements, _maxComponentDepth;
        public int MaxComponentElements { get => _maxComponentElements; }
        public int MaxComponentDepth { get => _maxComponentDepth; }

        public MaximumProfile(in byte[] data, FileHeaderEntry entry)
        {
            ParseTableData(in data, entry);
        }

        private void ParseTableData(in byte[] data, FileHeaderEntry entry)
        {
            int index = entry.Offset;

            _version = DataHelper.GetAtLocation4(in data, index, out index);
            _numGlyphs = DataHelper.GetAtLocation4(in data, index, 2, out index);
            switch (_version)
            {
                case (VERSION_0_5): return;
                case (VERSION_1_0): ParseVersion1Data(in data, index); return;
                default: throw new ArgumentException($"Unknown version {_version}, cannot parse Maximum Profile table.");
            }
        }

        private void ParseVersion1Data(in byte[] data, int index)
        {
            _maxPoints = DataHelper.GetAtLocation4(in data, index, 2, out index);
            _maxContours = DataHelper.GetAtLocation4(in data, index, 2, out index);

            _maxCompositePoints = DataHelper.GetAtLocation4(in data, index, 2, out index);
            _maxCompositeContours = DataHelper.GetAtLocation4(in data, index, 2, out index);

            _maxZones = DataHelper.GetAtLocation4(in data, index, 2, out index);
            _maxTwilightPoints = DataHelper.GetAtLocation4(in data, index, 2, out index);
            _maxStorage = DataHelper.GetAtLocation4(in data, index, 2, out index);

            _maxFunctionDefs = DataHelper.GetAtLocation4(in data, index, 2, out index);
            _maxInstructionDefs = DataHelper.GetAtLocation4(in data, index, 2, out index);

            _maxStackElements = DataHelper.GetAtLocation4(in data, index, 2, out index);
            _maxSizeOfInstructions = DataHelper.GetAtLocation4(in data, index, 2, out index);

            _maxComponentElements = DataHelper.GetAtLocation4(in data, index, 2, out index);
            _maxComponentDepth = DataHelper.GetAtLocation4(in data, index, 2, out index);
        }
    }
}
