﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DeeSynk.Core.Components;
using OpenTK;
using OpenTK.Graphics.OpenGL4;

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

        private const string TEXTURE_PATH = @"..\..\Resources\Textures\";
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

        private readonly Point CORNER_00 = new Point(0, 0);
        private readonly Point CORNER_01 = new Point(0, 1);
        private readonly Point CORNER_10 = new Point(1, 0);
        private readonly Point CORNER_11 = new Point(1, 1); 

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

            string[] subFolders = Directory.GetDirectories(TEXTURE_PATH);
            foreach(string folder in subFolders)
            {
                int fileCount = Directory.GetFiles(folder).Count();
                if (Directory.GetDirectories(folder).Count() == 0 && fileCount > 1)
                    InitTextureAtlas(TEXTURE_PATH, folder, fileCount);
            }

        }

        /// <summary>
        /// Loads a texture from a file, binds it to the current GL context, specifies some GL parameters,
        /// generates a mipmap, and adds the generated texture's id to the loadedTextures dictionary.  Outputs the width and height of the image.
        /// </summary>
        private void InitTexture(string folderPath, string fileName, string fileType, out int w, out int h)
        {
            int width, height;
            var data = LoadTexture(folderPath, fileName, fileType, out width, out height);
            int texture = GL.GenTexture();

            GL.BindTexture(TextureTarget.Texture2D, texture);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, width, height, 0, PixelFormat.Bgra, PixelType.Float, IntPtr.Zero);
            GL.TextureSubImage2D(texture, 0, 0, 0, width, height, PixelFormat.Bgra, PixelType.Float, data);
            GL.Enable(EnableCap.Texture2D);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.LinearMipmapLinear); // defines sampling behavior when scaling image down
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear); // defines sampling behavior when scaling image up
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToBorder); // defines border behavior in the x directions
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToBorder); // defines border behavior in the y directions

            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

            w = width;  //not used currently, but may be used to store as object size if needed?
            h = height;

            loadedTextures.Add(fileName, texture);
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
            GL.Enable(EnableCap.Texture2D);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.LinearMipmapLinear); // defines sampling behavior when scaling image down
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear); // defines sampling behavior when scaling image up
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToBorder); // defines border behavior in the x directions
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToBorder); // defines border behavior in the y directions

            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

            Texture tex = new Texture(texture, width, height, 1);

            loadedTextures.Add(fileName, texture);
            _loadedTextures[_loadedTextureCount] = tex;
            _loadedTextureCount++;
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

            return values;
        }

        private void InitTextureAtlas(string folderPath, string groupFolder, int textureCount)
        {
            var filePaths = Directory.GetFiles(folderPath + groupFolder);
            List<Rectangle> imgDims = new List<Rectangle>(textureCount);
            foreach (string path in filePaths)
                imgDims.Add(new Rectangle(new Point(0, 0), GetImageDimensions(path))); //Add rectangle representing image to list
            imgDims.Sort(CompareByArea); //Sort by descending area

            Random r = new Random();
            //TEST START
            imgDims = new List<Rectangle>();
            imgDims.Add(new Rectangle(0, 0, 100, 100));
            imgDims.Add(new Rectangle(0, 0, 100, 100));
            imgDims.Add(new Rectangle(0, 0, 100, 100));
            imgDims.Add(new Rectangle(0, 0, 100, 100));
            //imgDims.Add(new Rectangle(0, 0, r.Next(10, 200), r.Next(10, 200)));
            //imgDims.Add(new Rectangle(0, 0, r.Next(10, 200), r.Next(10, 200)));
            //imgDims.Add(new Rectangle(0, 0, r.Next(10, 200), r.Next(10, 200)));
            //imgDims.Add(new Rectangle(0, 0, r.Next(10, 200), r.Next(10, 200)));
            //imgDims.Add(new Rectangle(0, 0, r.Next(10, 200), r.Next(10, 200)));
            //imgDims.Add(new Rectangle(0, 0, r.Next(10, 200), r.Next(10, 200)));
            //imgDims.Add(new Rectangle(0, 0, r.Next(10, 200), r.Next(10, 200)));
            //imgDims.Add(new Rectangle(0, 0, r.Next(10, 200), r.Next(10, 200)));
            //TEST END

            var rects = new DeeSynk.Core.Algorithms.AlgorithmRectangleCompaction(imgDims).FindBestConfiguration();
             int x = 0;

            int width = rects.Max(rect => rect.C11.X);
            width++;
            int height = rects.Max(rect => rect.C11.Y);
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
            }

            int texture = GL.GenTexture();

            GL.BindTexture(TextureTarget.Texture2D, texture);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, width, height, 0, PixelFormat.Bgra, PixelType.Float, IntPtr.Zero);
            GL.TextureSubImage2D(texture, 0, 0, 0, width, height, PixelFormat.Bgra, PixelType.Float, data);
            GL.Enable(EnableCap.Texture2D);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.LinearMipmapLinear); // defines sampling behavior when scaling image down
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear); // defines sampling behavior when scaling image up
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToBorder); // defines border behavior in the x directions
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToBorder); // defines border behavior in the y directions

            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);


            loadedTextures.Add("testing", texture);
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
            return new Size(width, height);
        }

        private static int CompareByArea(Rectangle a, Rectangle b)
        {
            int areaA = a.Width * a.Height;
            int areaB = b.Width * b.Height;
            if (areaA == areaB)
                return 0;
            else if (areaA > areaB)
                return -1;
            else
                return 1;
        }

        public int GetTexture(string name)
        {
            int textureID = -1;
            if (loadedTextures.TryGetValue(name, out textureID))
                return textureID;
            else
                return textureID;
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
