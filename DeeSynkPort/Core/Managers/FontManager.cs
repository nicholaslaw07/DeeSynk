using DeeSynk.Core.Components.Fonts;
using DeeSynk.Core.Components.Fonts.Tables;
using DeeSynk.Core.Components.Fonts.Tables.CFF;
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

        private Dictionary<string, Font> _fonts;
        public Dictionary<string, Font> Fonts { get => _fonts; }

        #region STANDARD_VARS
        private readonly int _off1 = 1;
        private readonly int _off2 = 2;
        private readonly int _off4 = 4;
        #endregion

        private FontManager()
        {
            _fonts = new Dictionary<string, Font>();
        }

        public static ref FontManager GetInstance()
        {
            if (_fontManager == null)
                _fontManager = new FontManager();

            return ref _fontManager;
        }

        public void Load()
        {
            //LoadFontGeometry(@"C:\Users\Nicholas\source\repos\nicholaslaw07\DeeSynk\DeeSynkPort\Resources\Fonts\OfficeCodePro-Light\OfficeCodePro-Light.otf", "OfficeCodePro-Light.otf");
            //LoadFontGeometry(@"C:\Users\Chuck\source\repos\nicholaslaw07\DeeSynk\DeeSynkPort\Resources\Fonts\OfficeCodePro-Light\OfficeCodePro-Light.otf", "OfficeCodePro-Light.otf");
            //LoadFontGeometry(@"C:\Users\Nicholas\source\repos\nicholaslaw07\DeeSynk\DeeSynkPort\Resources\Fonts\Mechanical\Mechanical-g5Y5.otf", "Mechanical-g5Y5");
            LoadFontGeometry(@"C:\Users\Chuck\source\repos\nicholaslaw07\DeeSynk\DeeSynkPort\Resources\Fonts\Mechanical\Mechanical-g5Y5.otf", "Mechanical-g5Y5");
        }

        public void UnLoad()
        {
            throw new NotImplementedException();
        }

        //Documentation references:
        //https://wwwimages2.adobe.com/content/dam/acom/en/devnet/font/pdfs/5176.CFF.pdf
        //https://simoncozens.github.io/fonts-and-layout/opentype.html
        //https://www.adobe.com/content/dam/acom/en/devnet/font/pdfs/5177.Type2.pdf
        private void LoadFontGeometry(string path, string name)
        {
            byte[] file = File.ReadAllBytes(path);

            Font font = new Font(path, name);

            //try
            {
                if (!DataHelper.HasOTTOHeader(in file))
                    throw new Exception("File does not contain valid header for OpenType format");

                {
                    GetFileHeaderEntries(in file, _off4 * 3, out List<FileHeaderEntry> mainHeaderList);
                    font.HeaderEntires = mainHeaderList;
                }

                foreach (FileHeaderEntry entry in font.HeaderEntires)
                {
                    switch (entry.Table)
                    {
                        case (0x43464620 /*CFF */): font.CFFTable = new CFFTable(in file, entry); break;
                        case (0x6d617870 /*maxp*/): font.MaxP = new MaximumProfile(in file, entry); break;
                        case (0x4f532f32 /*OS/2*/): font.OS2 = new OS2(in file, entry); break;
                        default: Debug.WriteLine("Unknown or unsupported table."); break;
                    }
                }

                _fonts.Add(name, font);
            }
            //catch(Exception e)
            //{
            //    Debug.WriteLine($"Exception while parsing the font {name} at {path}.\n{e}");
            //}
        }

        private void GetFileHeaderEntries(in byte[] data, int start, out List<FileHeaderEntry> headerEntries)
        {
            int offset = start - _off4;
            bool stillChecking = true;
            headerEntries = new List<FileHeaderEntry>();
            List<int> hNames = new List<int>(Font.headerNames);
            do
            {
                int testVal = DataHelper.GetAtLocationInt(in data, offset += _off4, _off4);
                stillChecking = hNames.Contains(testVal);
                if (stillChecking)
                {
                    headerEntries.Add(new FileHeaderEntry(
                        testVal,
                        DataHelper.GetAtLocationInt(in data, offset += _off4, _off4),
                        DataHelper.GetAtLocationInt(in data, offset += _off4, _off4),
                        DataHelper.GetAtLocationInt(in data, offset += _off4, _off4)));
                }
            } while (stillChecking);
        }

        //=====================================//
        //        -Point of Uncertainy-        //
        // There some mysterious behavior in   //
        // the global subroutines.  There is a //
        // mysterious global subroutine that   //
        // begins WITH an operator and is then //
        // followed by normal operands and     //
        // operators.  This is not the case    //
        // for all global subroutines.  My     //
        // hypothesis is that it may have      //
        // something to do with how they are   //
        // added to the stack of commands.     //
        // More specifically, these may be     //
        // added to the stack in such a way    //
        // that operands are passed from the   //
        // primary charstring command list or  //
        // the subroutine is appended directly //
        // after the raw binary of the         //
        // charstring commands.  If true, then //
        // the methods of parsing charstring   //
        // commands will need to be different. //
        // It may become the case that global  //
        // subroutines are parsed into the     //
        // charstrings upon the initial parse  //
        // so as to avoid subroutines at all.  //
        // If this were the case, then this    //
        // should not cause issues since we    //
        // not drawing fonts directly from the //
        // code and most vector font and bmp   //
        // font data will be baked.            //
        //=====================================//

        //====================================//
        //             -Addendum-             //
        // We need to add Geometry to VAO     //
        // functionality.  This will require  //
        // interpreting the functions and     //
        // in the case of baked geometry,     //
        // creating it on the CPU.  When we   //
        // have baked textures, we bake on    //
        // the CPU and render on the GPU      //
        // When we have adaptive font vectors //
        // we need to upload custom VAO data  //
        // to the GPU and have it utilize     //
        // custom tessellation shaders to add //
        // just the right level of detail     //
        // for the expected size on the       //
        // display.                           //
        //====================================//
    }
}
