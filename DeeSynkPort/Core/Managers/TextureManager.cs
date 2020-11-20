using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DeeSynk.Core.Components;
using OpenTK;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace DeeSynk.Core.Managers
{
    /// <summary>
    /// Manages all texture resources. Is responsible for loading at the start of the game and unloading at
    /// the end. Keeps track of all textures generated within the GL context, in the loadedTextures dictionary.
    /// </summary>
    class TextureManager : IManager
    {
        // _B stands for the variable representing the number of bytes for said variable (name before)
        private const int MAX_TEXTURE_COUNT = 2048;

        private static int callCount = 0;

        //private const string TEXTURE_PATH = @"C:\Users\Nicholas\source\repos\nicholaslaw07\DeeSynk\DeeSynkPort\Resources\Textures\Single_Textures\";  //Location of individual texture files
        //private const string ATLAS_PATH = @"C:\Users\Nicholas\source\repos\nicholaslaw07\DeeSynk\DeeSynkPort\Resources\Textures\Atlases\"; //Location of atlas group folders
        private const string TEXTURE_PATH = @"C:\Users\Chuck\source\repos\nicholaslaw07\DeeSynk\DeeSynkPort\Resources\Textures\Single_Textures\";  //Location of individual texture files
        private const string ATLAS_PATH = @"C:\Users\Chuck\source\repos\nicholaslaw07\DeeSynk\DeeSynkPort\Resources\Textures\Atlases\"; //Location of atlas group folders
        private const string FILE_TYPE = ".bmp";

        #region BitmapHeaderOffsets

        private const int B_INDEX = 0;
        private const int M_INDEX = 1;

        private const int PIXEL_ARRAY = 10;
        private const int PIXEL_ARRAY_B = 4;

        private const int IMAGE_WIDTH = 18;
        private const int IMAGE_WIDTH_B = 4;

        private const int IMAGE_HEIGHT = 22;
        private const int IMAGE_HEIGHT_B = 4;

        private const int COLOR_PLANES = 26;
        private const int COLOR_PLANES_B = 2;

        private const int BITS_PER_PIXEL = 28;
        private const int BITS_PER_PIXEL_B = 2;

        #endregion

        private static TextureManager _textureManager;

        private Dictionary<string, int> loadedTextures;
        private Texture[] _loadedTextures;
        private int _loadedTextureCount;

        /// <summary>
        /// Constructor instantiating the dictionary where loaded textures will be stored.
        /// </summary>
        private TextureManager()
        {
            loadedTextures = new Dictionary<string, int>();
            _loadedTextures = new Texture[MAX_TEXTURE_COUNT];
            _loadedTextureCount = 0;
        }
        
        /// <summary>
        /// Returns the only instance of this class, controlled in a singleton format. If an instance exists
        /// already, it is returned, otherwise it creates a new instance.
        /// </summary>
        public static ref TextureManager GetInstance()
        {
            if (_textureManager == null)
                _textureManager = new TextureManager();

            return ref _textureManager;
        }
        
        /// <summary>
        /// Loads all initial texture resources, generates them within the GL context, and stores them in
        /// the loadedTextures dictionary.
        /// </summary>
        public void Load()
        {
            string[] fileNames = Directory.GetFiles(TEXTURE_PATH)
                         .Select(Path.GetFileNameWithoutExtension)
                         .ToArray();
            foreach(string fileName in fileNames)
            {
                InitTexture(TEXTURE_PATH, fileName, FILE_TYPE);
            }

            string[] subFolders = Directory.GetDirectories(ATLAS_PATH).Select(s => s + "\\").ToArray();

            foreach(string folder in subFolders)
            {
                int fileCount = Directory.GetFiles(folder).Count();
                if (Directory.GetDirectories(folder).Count() == 0 && fileCount > 1)
                    InitTextureAtlas(folder, fileCount);
            }

            //CreateFontBitmap();
        }

        /// <summary>
        /// Loads a texture from a file, binds it to the current GL context, specifies some GL parameters,
        /// generates a mipmap, and adds the generated texture's id to the loadedTextures dictionary.  Does not output the width and height of the image.
        /// </summary>
        private void InitTexture(string folderPath, string fileName, string fileType)
        {
            int width, height;
            var data = LoadTexture(folderPath, fileName, fileType, out width, out height);
            int texture = GL.GenTexture();

            GL.BindTexture(TextureTarget.Texture2D, texture);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, width, height, 0, PixelFormat.Bgra, PixelType.Float, IntPtr.Zero);

            GL.TextureSubImage2D(texture, 0, 0, 0, width, height, PixelFormat.Bgra, PixelType.Float, data);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.LinearMipmapLinear); // defines sampling behavior when scaling image down
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear); // defines sampling behavior when scaling image up
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToBorder); // defines border behavior in the x directions
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToBorder); // defines border behavior in the y directions

            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

            Texture tex = new Texture(texture, width, height, 1);
            tex.AddSubTextureLocation(new SubTextureLocation(new Vector2(0f, 0f), new Vector2(1f, 1f)));
            loadedTextures.Add(fileName, _loadedTextureCount);

            _loadedTextures[_loadedTextureCount] = tex;
            _loadedTextureCount++;
        }

        private void CreateFontBitmap()
        {
            int width, height;
            var data = LoadTexture("arial", 15, Color.White, out width, out height);
            int texture = GL.GenTexture();

            GL.BindTexture(TextureTarget.Texture2D, texture);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, width, height, 0, PixelFormat.Bgra, PixelType.Float, IntPtr.Zero);

            GL.TextureSubImage2D(texture, 0, 0, 0, width, height, PixelFormat.Bgra, PixelType.Float, data);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.LinearMipmapLinear); // defines sampling behavior when scaling image down
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear); // defines sampling behavior when scaling image up
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToBorder); // defines border behavior in the x directions
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToBorder); // defines border behavior in the y directions

            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

            Texture tex = new Texture(texture, width, height, 1);
            tex.AddSubTextureLocation(new SubTextureLocation(new Vector2(0f, 0f), new Vector2(1f, 1f)));
            loadedTextures.Add("bitmap_0", _loadedTextureCount);

            _loadedTextures[_loadedTextureCount] = tex;
            _loadedTextureCount++;
        }

        //cannot subdivide folders, is it necessary too though... it depends on the size of textures we want to work with
        private void InitTextureAtlas(string folderPath, int textureCount)
        {
            var filePaths = Directory.GetFiles(folderPath);
            var fileName = Directory.GetFiles(folderPath).Select(Path.GetFileNameWithoutExtension);
            List<Tuple<int, Rectangle>> imgDims = new List<Tuple<int, Rectangle>>(filePaths.Count());

            int folderStartIdx = folderPath.IndexOf("Atlas_");
            int folderEndIdx = folderPath.LastIndexOf("\\");
            string groupFolder = folderPath.Substring(folderStartIdx, folderEndIdx - folderStartIdx);

            for (int idx = 0; idx < textureCount; idx++)
                imgDims.Add(new Tuple<int, Rectangle>(idx, new Rectangle(new Point(0, 0), GetImageDimensions(filePaths[idx])))); //Add rectangle representing image to list
            imgDims.Sort(CompareByArea); //Sort by descending area

            var indices = imgDims.Select(t => t.Item1).ToArray();
            var rects = new Algorithms.AlgorithmRectangleCompaction(imgDims.Select(t => t.Item2).ToList()).FindBestConfiguration();

            int width = rects.Max(r => r.C11.X) + 1;
            int height = rects.Max(r => r.C11.Y) + 1;

            int texture = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, texture);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, width, height, 0, PixelFormat.Bgra, PixelType.Float, IntPtr.Zero);
            loadedTextures.Add(groupFolder, _loadedTextureCount);
            _loadedTextures[_loadedTextureCount] = new Texture(texture, width, height, textureCount);

            for(int idx = 0; idx < textureCount; idx++)
            {
                var data = LoadTexture(folderPath, Path.GetFileNameWithoutExtension(filePaths[indices[idx]]), ".bmp", out int subW, out int subH);
                int offsetX = rects[idx].X0;
                int offsetY = rects[idx].Y0;
                GL.TextureSubImage2D(texture, 0, offsetX, offsetY, subW, subH, PixelFormat.Bgra, PixelType.Float, data);

                Vector2 uvOffset = new Vector2(((float)offsetX) / ((float)width), ((float)offsetY) / ((float)height));
                Vector2 uvScale = new Vector2((subW) / (float)width, (subH) / (float)height);
                if(!_loadedTextures[_loadedTextureCount].AddSubTextureLocation(new SubTextureLocation(uvOffset, uvScale)))
                    throw new ArgumentException("Invalid texture atlas.");
            }

            _loadedTextureCount++;

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.NearestMipmapNearest); // defines sampling behavior when scaling image down
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest); // defines sampling behavior when scaling image up
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToBorder); // defines border behavior in the x directions
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToBorder); // defines border behavior in the y directions

            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
        }

        /// <summary>
        /// Retrieves the specified files and pulls all of the pixel values, converting them to
        /// and returning them as an array of floats.
        /// </summary>
        private float[] LoadTexture(string folderPath, string fileName, string fileType, out int width, out int height)
        {
            float[] values = new float[0];

            using (var image = Image.FromFile(folderPath + fileName + fileType))
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    image.Save(ms, image.RawFormat);
                    var outArr = ms.ToArray();

                    if (outArr[0] != (byte)67 && outArr[1] != (byte)77)                                                //ensuring the first two characters are 'B' and 'M'  (66 & 77, respectively)
                        throw new Exception("Invalid file header format - 'B' and 'M' expected.");

                    width = 0;
                    height = 0;

                    int pixelDataOffset = 0;
                    int colorPlanes     = 0;
                    int bitsPerPixel    = 0;
                    int bytesPerPixel   = 0;

                    for (int i = 0; i <= PIXEL_ARRAY_B - 1; i++)    { pixelDataOffset += outArr[i + PIXEL_ARRAY] << (i * 8); }      //gets the offset of where the pixel data of stored, consists of four bytes starting at index 10

                    for (int i = 0; i <= IMAGE_WIDTH_B - 1; i++)    { width  += (int)(outArr[i + IMAGE_WIDTH]  << (i * 8)); }         //retrieves width
                    for (int i = 0; i <= IMAGE_HEIGHT_B - 1; i++)   { height += (int)(outArr[i + IMAGE_HEIGHT] << (i * 8)); }      //retrieves height
                                                                               
                    for (int i = 0; i <= COLOR_PLANES_B - 1; i++)   { colorPlanes += (int)(outArr[i + COLOR_PLANES] << (i * 8)); } //retrieves value for number of color planes and checks its value (should always be one)
                    if (colorPlanes != 1)
                        throw new Exception("Invalid number of colors planes in DIB - a value of 1 is expected.");

                    for (int i = 0; i <= BITS_PER_PIXEL_B - 1; i++) { bitsPerPixel += (int)(outArr[i + BITS_PER_PIXEL] << (i * 8)); }  //retrieves the number of bits per pixel within the image
                    bytesPerPixel = bitsPerPixel / 8;                                                                                  //takes bpp and converts it to bytes to determine the size of one pixel in the array

                    int totalValues = width * height * bytesPerPixel;   //total number of bytes in the image data

                    values = new float[totalValues];

                    for (int i = 0; i < totalValues; i++)               //reads the array sequentially and outputs to the image array
                        values[i] = outArr[i + pixelDataOffset] / 255f;
                }
            }
            //Debug.WriteLine(GL.GetError());
            return values;
        }

        private float[] LoadTexture(string font, int size, Color color, out int width, out int height)
        {
            float[] values = new float[0];
            float[] values2 = new float[0];

            using (var bitmap = new Bitmap(8000, 8000))
            {
                var b = new SolidBrush(Color.White);
                var g = Graphics.FromImage(bitmap);
                Font f = new Font(font, size);
                int s = size;

                for(int idx = 0; idx < 256; idx++)
                {
                    for(int jdx = 0; jdx < 256; jdx++)
                    {
                        g.DrawString(Convert.ToChar(idx * 128 + jdx).ToString(), f, b, new PointF(jdx * s + s * jdx, idx * s + s * idx * 1.25f));
                    }
                }

                //g.DrawString("B", f, b, new PointF(size * (1.0f + 0.1f) + s * 0, 0.0f));
                //g.DrawString("AB", f, b, new PointF(size * (1.0f + 0.2f) + s * 1, 0.0f));
                //g.DrawString("WARi iiiiA", f, b, new PointF(size * (1.0f + 0.2f) + s * 1, 0.0f));

                g.SmoothingMode = SmoothingMode.HighQuality;
                using (MemoryStream ms = new MemoryStream())
                {
                    bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
                    var outArr = ms.ToArray();

                    if (outArr[0] != (byte)67 && outArr[1] != (byte)77)                                                //ensuring the first two characters are 'B' and 'M'  (66 & 77, respectively)
                        throw new Exception("Invalid file header format - 'B' and 'M' expected.");

                    width = 0;
                    height = 0;

                    int pixelDataOffset = 0;
                    int colorPlanes = 0;
                    int bitsPerPixel = 0;
                    int bytesPerPixel = 0;

                    for (int i = 0; i <= PIXEL_ARRAY_B - 1; i++) { pixelDataOffset += outArr[i + PIXEL_ARRAY] << (i * 8); }      //gets the offset of where the pixel data of stored, consists of four bytes starting at index 10

                    for (int i = 0; i <= IMAGE_WIDTH_B - 1; i++) { width += (int)(outArr[i + IMAGE_WIDTH] << (i * 8)); }         //retrieves width
                    for (int i = 0; i <= IMAGE_HEIGHT_B - 1; i++) { height += (int)(outArr[i + IMAGE_HEIGHT] << (i * 8)); }      //retrieves height

                    for (int i = 0; i <= COLOR_PLANES_B - 1; i++) { colorPlanes += (int)(outArr[i + COLOR_PLANES] << (i * 8)); } //retrieves value for number of color planes and checks its value (should always be one)
                    if (colorPlanes != 1)
                        throw new Exception("Invalid number of colors planes in DIB - a value of 1 is expected.");

                    for (int i = 0; i <= BITS_PER_PIXEL_B - 1; i++) { bitsPerPixel += (int)(outArr[i + BITS_PER_PIXEL] << (i * 8)); }  //retrieves the number of bits per pixel within the image
                    bytesPerPixel = bitsPerPixel / 8;                                                                                  //takes bpp and converts it to bytes to determine the size of one pixel in the array

                    int totalValues = width * height * bytesPerPixel;   //total number of bytes in the image data

                    values = new float[totalValues];

                    for (int i = 0; i < totalValues; i++)               //reads the array sequentially and outputs to the image array
                        values[i] = outArr[i + pixelDataOffset] / 255f;

                    values2 = new float[values.Length];
                    for(int idx = 0; idx < bitmap.Height; idx++)
                    {
                        for(int jdx = 0; jdx < bitmap.Width; jdx++)
                        {
                            values2[idx * bitmap.Width * 4 + (bitmap.Width - jdx - 1) * 4 + 0] = values[idx * bitmap.Width * 4 + jdx * 4 + 0];
                            values2[idx * bitmap.Width * 4 + (bitmap.Width - jdx - 1) * 4 + 1] = values[idx * bitmap.Width * 4 + jdx * 4 + 1];
                            values2[idx * bitmap.Width * 4 + (bitmap.Width - jdx - 1) * 4 + 2] = values[idx * bitmap.Width * 4 + jdx * 4 + 2];
                            values2[idx * bitmap.Width * 4 + (bitmap.Width - jdx - 1) * 4 + 3] = values[idx * bitmap.Width * 4 + jdx * 4 + 3];
                        }
                    }
                }
            }

            return values2;
        }

        private Size GetImageDimensions(string filePath)
        {
            int width = 0;
            int height = 0;
            using (var image = Image.FromFile(filePath))
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    image.Save(ms, image.RawFormat);
                    var outArr = ms.ToArray();
                    for (int i = 0; i < IMAGE_WIDTH_B; i++) { width += outArr[i + IMAGE_WIDTH] << (i * 8); }
                    for (int i = 0; i < IMAGE_HEIGHT_B; i++) { height += outArr[i + IMAGE_HEIGHT] << (i * 8); }
                }
            }
            //Debug.WriteLine(GL.GetError());
            return new Size(width, height);
        }

        private static int CompareByArea(Tuple<int, Rectangle> a, Tuple<int, Rectangle> b)
        {
            int areaA = a.Item2.Width * a.Item2.Height;
            int areaB = b.Item2.Width * b.Item2.Height;
            if (areaA == areaB)
                return 0;
            else if (areaA > areaB)
                return -1;
            else
                return 1;
        }

        public Texture GetTexture(string name)
        {
            int textureID = -1;
            if (loadedTextures.TryGetValue(name, out textureID))
                return _loadedTextures[textureID];
            else
                return null;
        }

        public Texture GetTexture(int id)
        {
            if(_loadedTextures[id] != null)
                return _loadedTextures[id];
            return null;
        }


        /// <summary>
        /// Deletes all loaded textures from the GL context.
        /// </summary>
        public void UnLoad()
        {
            foreach (int texture in loadedTextures.Values)
            {
                GL.DeleteTexture(texture);
            }
        }
    }
}

//THIS IS OLD TEST CODE

//TEST START
/*
Random rand = new Random();
imgDims = new List<Rectangle>();
for (int idx = 0; idx < 32; idx++)
    imgDims.Add(new Rectangle(0, 0, rand.Next(16, 256), rand.Next(16, 256)));

imgDims.Sort(CompareByArea); //Sort by descending area
Stopwatch sw = new Stopwatch();
sw.Start();
var rects = new DeeSynk.Core.Algorithms.AlgorithmRectangleCompaction(imgDims).FindBestConfiguration();
sw.Stop();
Console.WriteLine(sw.ElapsedMilliseconds / 1000f);
imgDims = new List<Rectangle>();
imgDims.Add(new Rectangle(0, 0, 100, 100));
imgDims.Add(new Rectangle(0, 0, 100, 100));
imgDims.Add(new Rectangle(0, 0, 150, 200));
imgDims.Add(new Rectangle(0, 0, 210, 15));
imgDims.Add(new Rectangle(0, 0, 100, 100));
imgDims.Add(new Rectangle(0, 0, 150, 90));
imgDims.Add(new Rectangle(0, 0, 50, 50));
imgDims.Add(new Rectangle(0, 0, 120, 100));
imgDims.Add(new Rectangle(0, 0, 30, 100));
imgDims.Add(new Rectangle(0, 0, 21, 21));
imgDims.Add(new Rectangle(0, 0, 170, 140));
imgDims.Add(new Rectangle(0, 0, 300, 100));
imgDims.Add(new Rectangle(0, 0, 90, 40));
imgDims.Add(new Rectangle(0, 0, 70, 60));
imgDims.Add(new Rectangle(0, 0, 30, 100));
imgDims.Add(new Rectangle(0, 0, 100, 100));
//imgDims.Add(new Rectangle(0, 0, 100, 100));
imgDims.Sort(CompareByArea); //Sort by descending area

//TEST END
Stopwatch sw = new Stopwatch();
//sw.Start();
//List<Rectangle> bestArrangment = FindBestArrangment(imgDims);
//sw.Stop();
//Console.WriteLine(sw.ElapsedMilliseconds / 1000f);
//sw.Reset();
sw.Start();
var rects1 = new DeeSynk.Core.Algorithms.AlgorithmRectangleCompaction(imgDims.GetRange(0, 4)).FindBestConfiguration();
var rects2 = new DeeSynk.Core.Algorithms.AlgorithmRectangleCompaction(imgDims.GetRange(4, 4)).FindBestConfiguration();
var rects3 = new DeeSynk.Core.Algorithms.AlgorithmRectangleCompaction(imgDims.GetRange(8, 4)).FindBestConfiguration();
var rects4 = new DeeSynk.Core.Algorithms.AlgorithmRectangleCompaction(imgDims.GetRange(12, 4)).FindBestConfiguration();
imgDims.Clear();
imgDims.Add(new Rectangle(0, 0, rects1.Max(rc => rc.X1 + 1), rects1.Max(rc => rc.Y1 + 1)));
imgDims.Add(new Rectangle(0, 0, rects2.Max(rc => rc.X1 + 1), rects2.Max(rc => rc.Y1 + 1)));
imgDims.Add(new Rectangle(0, 0, rects3.Max(rc => rc.X1 + 1), rects3.Max(rc => rc.Y1 + 1)));
imgDims.Add(new Rectangle(0, 0, rects4.Max(rc => rc.X1 + 1), rects4.Max(rc => rc.Y1 + 1)));
var rectsF = new DeeSynk.Core.Algorithms.AlgorithmRectangleCompaction(imgDims.GetRange(0, 4)).FindBestConfiguration();

DeeSynk.Core.Algorithms.Rectangle[] rects = new DeeSynk.Core.Algorithms.Rectangle[rects1.Length + rects2.Length + rects3.Length + rects4.Length];
for(int idx = 0; idx < 4; idx++)
    rects[idx] = rects1[idx];
for (int idx = 4; idx < 8; idx++)
    rects[idx] = rects2[idx - 4];
for (int idx = 8; idx < 12; idx++)
    rects[idx] = rects3[idx - 8];
for (int idx = 12; idx < 16; idx++)
    rects[idx] = rects4[idx - 12];

for(int idx = 0; idx < 4; idx++)
{
    var p = rectsF[idx].C00;
    rects[4 * idx + 0] = rects[4 * idx + 0] + p;
    rects[4 * idx + 1] = rects[4 * idx + 1] + p;
    rects[4 * idx + 2] = rects[4 * idx + 2] + p;
    rects[4 * idx + 3] = rects[4 * idx + 3] + p;
}

sw.Stop();
Console.WriteLine(sw.ElapsedMilliseconds / 1000f);


int width = rects.Max(r => r.C11.X);
width++;
int height = rects.Max(r => r.C11.Y);
height++;

var data = new float[4 * width * height];

for(int jdx = 0; jdx < height; jdx++)
{
    for(int idx = 0; idx < width; idx++)
    {
        data[jdx * width * 4 + idx * 4 + 0] = 1.0f;
        data[jdx * width * 4 + idx * 4 + 1] = 1.0f;
        data[jdx * width * 4 + idx * 4 + 2] = 1.0f;
        data[jdx * width * 4 + idx * 4 + 3] = 1.0f;

        for(int kdx = 0; kdx < rects.Length; kdx++)
        {
            if(rects[kdx].ContainsPoint(new Algorithms.Point(idx, jdx)))
            {
                if(rects[kdx].IsEdgePoint(new Algorithms.Point(idx, jdx)))
                {
                    data[jdx * width * 4 + idx * 4 + 0] = 0.0f;
                    data[jdx * width * 4 + idx * 4 + 1] = 1.0f;
                    data[jdx * width * 4 + idx * 4 + 2] = 0.0f;
                    data[jdx * width * 4 + idx * 4 + 3] = 1.0f;
                }
                else
                {
                    data[jdx * width * 4 + idx * 4 + 0] = 1.0f;
                    data[jdx * width * 4 + idx * 4 + 1] = 0.0f;
                    data[jdx * width * 4 + idx * 4 + 2] = 1.0f;
                    data[jdx * width * 4 + idx * 4 + 3] = 1.0f;
                }
            }
        }
    }
}*/
