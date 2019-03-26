using System;
using System.Collections.Generic;
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
    public enum Axis
    {
        X = 0,
        Y = 1
    }

    public enum Corners : int
    {
        C00 = 0,
        C01 = 1,
        C10 = 2,
        C11 = 3
    }

    public enum EdgeNormals : int
    {
        UP = 0,
        DOWN = 1,
        LEFT = 2,
        RIGHT = 3
    }

    [Flags]
    public enum AttachmentLocks
    {
        ALL_OPEN = 0,
        UP_L = 1,
        UP_R = 2,
        DOWN_L = 4,
        DOWN_R = 8,
        ALL_LOCKED = 15
    }

    public class Edge
    {

        private Point _p1;
        public Point P1
        {
            get => _p1;
            set
            {
                if ((value.X != _p2.X && value.Y != _p2.Y) || (value.X == _p2.X && value.Y == _p2.Y))
                    throw new ArgumentException("Points must share one, and only one, coordinate on either the X or Y axes.");
                else if (value.X < 0 || value.Y < 0)
                    throw new ArgumentException("Point must contain only zero or positive coordinates.");
                else
                {
                    if (value.X == _p2.X)
                        _sharedAxis = Axis.X;
                    else
                        _sharedAxis = Axis.Y;

                    _p1 = new Point(value.X, value.Y);
                }
            }
        }

        private Point _p2;
        public Point P2
        {
            get => _p2;
            set
            {
                if ((_p1.X != value.X && _p1.Y != value.Y) || (_p1.X == value.X && _p1.Y == value.Y))
                    throw new ArgumentException("Points must share one, and only one, coordinate on either the X or Y axes.");
                else if (value.X < 0 || value.Y < 0)
                    throw new ArgumentException("Point must contain only zero or positive coordinates.");
                else
                {
                    if (value.X == _p2.X)
                        _sharedAxis = Axis.X;
                    else
                        _sharedAxis = Axis.Y;

                    _p1 = new Point(value.X, value.Y);
                }
            }
        }

        public int Length { get => (_sharedAxis == Axis.X) ? _p1.Y - _p2.Y : _p1.X - _p2.X; }

        private int SharedCoordinate
        {
            get
            {
                if (_sharedAxis == Axis.X)
                    return _p1.X;
                else
                    return _p1.Y;
            }
        }
        
        public bool IsAgainstAxis { get => SharedCoordinate == 0; }

        private Axis _sharedAxis;
        public Axis Axis { get => _sharedAxis; }

        private EdgeNormals _edgeDirection;
        public EdgeNormals EdgeDirection { get => _edgeDirection; }

        public Edge(Point p1, Point p2, EdgeNormals edgeNormal)
        {
            if ((p1.X != p2.X && p1.Y != p2.Y) || (p1.X == p2.X && p1.Y == p2.Y))
                throw new ArgumentException("Points must share one, and only one, coordinate on either the X or Y axes.");

            if (p1.X == p2.X)
            {
                _sharedAxis = Axis.X;
                if(p1.X < p2.X)
                {
                    _p1 = p1;
                    _p2 = p2;
                }
                else
                {
                    _p1 = p2;
                    _p2 = p1;
                }

                _edgeDirection = edgeNormal;
            }
            else
            {
                _sharedAxis = Axis.Y;
                if (p1.Y < p2.Y)
                {
                    _p1 = p1;
                    _p2 = p2;
                }
                else
                {
                    _p1 = p2;
                    _p2 = p1;
                }
            }
        }

        public bool ContainsPoint(Point p, bool excludeEndpoint)
        {
            if(_sharedAxis == Axis.X)
            {
                if(p.X == SharedCoordinate)
                {
                    if (excludeEndpoint)
                    {
                        if ((_p1.Y < p.Y && _p2.Y > p.Y))
                            return true;
                    }
                    else
                    {
                        if ((_p1.Y <= p.Y && _p2.Y >= p.Y))
                            return true;
                    }
                }
            }
            else
            {
                if(p.Y == SharedCoordinate)
                {
                    if (excludeEndpoint)
                    {
                        if ((_p1.X <= p.X && _p2.X >= p.X))
                            return true;
                    }
                    else
                    {
                        if ((_p1.X < p.X && _p2.X > p.X))
                            return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Returns true if the union of the points between the two edges has a non-zero size.
        /// </summary>
        /// <param name="edge"></param>
        /// <returns></returns>
        public bool SharesEdge(Edge edge)
        {
            if(_sharedAxis == edge._sharedAxis)
            {
                if ((edge.ContainsPoint(_p1, false) && edge.ContainsPoint(_p2, false)) ||
                    (this.ContainsPoint(edge._p1, false) && this.ContainsPoint(edge._p2, false)))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Returns true only if the points of the passed in edge are a perfect subset this edge.
        /// </summary>
        /// <param name="edge"></param>
        /// <returns></returns>
        public bool ContainsEdge(Edge edge)
        {
            if(_sharedAxis == edge._sharedAxis)
            {
                if (this.ContainsPoint(edge._p1, false) && this.ContainsPoint(edge._p2, false))
                    return true;
            }
            return false;
        }

        public bool Intersects(Edge edge)
        {
            if(_sharedAxis != edge._sharedAxis)
            {
                if(_sharedAxis == Axis.X)
                {
                    if ((edge._p1.X <= SharedCoordinate && edge._p2.X >= SharedCoordinate) &&
                       (edge._p1.Y >= _p1.Y && edge._p2.Y >= _p1.Y) && (edge._p1.Y <= _p2.Y && edge._p2.Y <= _p2.Y))
                        return true;
                }
                else
                {
                    if ((edge._p1.Y <= SharedCoordinate && edge._p2.Y >= SharedCoordinate) &&
                       (edge._p1.X >= _p1.X && edge._p2.X >= _p1.X) && (edge._p1.X <= _p2.X && edge._p2.X <= _p2.X))
                        return true;
                }
            }
            return false;
        }

        public override bool Equals(object obj)
        {
            if (_p1 == ((Edge)obj)._p1 && _p2 == ((Edge)obj)._p2)
                return true;
            return false;
        }

        public override string ToString()
        {
            return _p1.ToString() + " " + _p2.ToString();
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static bool operator ==(Edge edgeA, Edge edgeB)
        {
            if (edgeA.Equals(edgeB))
                return true;
            return false;
        }

        public static bool operator !=(Edge edgeA, Edge edgeB)
        {
            if (!edgeA.Equals(edgeB))
                return true;
            return false;
        }
    }

    public class AttachmentPoint
    {
        private Point _point;
        public Point Point { get => _point; }

        private AttachmentLocks _attachmentLocks;
        public AttachmentLocks AttachmentLocks { get => _attachmentLocks; set => _attachmentLocks = value; }

        public AttachmentPoint(Point p, AttachmentLocks directions)
        {
            if (p.X < 0 || p.Y < 0)
                throw new ArgumentException("Coordinates must be non-negative integers.");
            _point = p;
            _attachmentLocks = directions;
        }
    }

    /// <summary>
    /// Manages all texture resources. Is responsible for loading at the start of the game and unloading at
    /// the end. Keeps track of all textures generated within the GL context, in the loadedTextures dictionary.
    /// </summary>
    class TextureManager : IManager
    {
        // _B stands for the variable representing the number of bytes for said variable (name before)
        private const int MAX_TEXTURE_COUNT = 2048;

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
            /*
            string[] fileNames = Directory.GetFiles(TEXTURE_PATH)
                         .Select(Path.GetFileNameWithoutExtension)
                         .ToArray();
            foreach(string fileName in fileNames)
            {
                InitTexture(TEXTURE_PATH, fileName, FILE_TYPE);
            }*/

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

            //TEST START
            imgDims = new List<Rectangle>();
            imgDims.Add(new Rectangle(0, 0, 100, 100));
            imgDims.Add(new Rectangle(0, 0, 100, 100));
            imgDims.Add(new Rectangle(0, 0, 150, 200));
            //TEST END

            List<Rectangle> bestArrangment = FindBestArrangment(imgDims);
            int x = 0;
        }

        private List<Rectangle> FindBestArrangment(List<Rectangle> rectangles)
        {
            return FindBestArrangment(rectangles, new List<Rectangle>());
        }

        private List<Rectangle> FindBestArrangment(List<Rectangle> rectangles, List<Rectangle> fillArray)
        {
            List<Rectangle> bestArrangment = new List<Rectangle>();
            float bestScore = 0f;

            List<AttachmentPoint> attachmentPoints = new List<AttachmentPoint>();
            if (fillArray.Count() > 0)
                attachmentPoints = GetValidAttachmentPoints(fillArray, rectangles);

            var rC = new Rectangle[rectangles.Count()];
            var fAC = new Rectangle[fillArray.Count()];

            rectangles.CopyTo(rC);
            fillArray.CopyTo(fAC);

            if (rectangles.Count() == 0)
                return fillArray;

            for (int i = 0; i < rectangles.Count(); i++)
            {

                if(attachmentPoints.Count() == 0)
                {
                    var rectanglesCopy = rC.ToList();
                    var fillArrayCopy = fAC.ToList();

                    fillArrayCopy.Add(rectanglesCopy[i]);
                    rectanglesCopy.RemoveAt(i);
                    var arrangment = FindBestArrangment(rectanglesCopy, fillArrayCopy);
                    float score = Score(arrangment);
                    if (score > bestScore)
                    {
                        bestArrangment = arrangment;
                        bestScore = score;
                    }
                }
                else
                {
                    foreach (AttachmentPoint aP in attachmentPoints)
                    {
                        if (!aP.AttachmentLocks.HasFlag(AttachmentLocks.UP_L))
                        {
                            var rectanglesCopy = rC.ToList();
                            var fillArrayCopy = fAC.ToList();

                            var currentRectangle = rectanglesCopy[i];

                            var stapledRectangle = StapleToPoint(currentRectangle, Corners.C11, aP.Point);
                            if (IsValidPositionForRectangle(fillArrayCopy, stapledRectangle))
                            {
                                fillArrayCopy.Add(stapledRectangle);
                                rectanglesCopy.RemoveAt(i);
                                var arrangment = FindBestArrangment(rectanglesCopy, fillArrayCopy);
                                float score = Score(arrangment);
                                if (score > bestScore)
                                {
                                    bestArrangment = arrangment;
                                    bestScore = score;
                                }
                            }
                        }

                        if (!aP.AttachmentLocks.HasFlag(AttachmentLocks.UP_R))
                        {
                            var rectanglesCopy = rC.ToList();
                            var fillArrayCopy = fAC.ToList();

                            var currentRectangle = rectanglesCopy[i];

                            var stapledRectangle = StapleToPoint(currentRectangle, Corners.C01, aP.Point);
                            if (IsValidPositionForRectangle(fillArrayCopy, stapledRectangle))
                            {
                                fillArrayCopy.Add(stapledRectangle);
                                rectanglesCopy.RemoveAt(i);
                                var arrangment = FindBestArrangment(rectanglesCopy, fillArrayCopy);
                                float score = Score(arrangment);
                                if (score > bestScore)
                                {
                                    bestArrangment = arrangment;
                                    bestScore = score;
                                }
                            }
                        }

                        if (!aP.AttachmentLocks.HasFlag(AttachmentLocks.DOWN_L))
                        {
                            var rectanglesCopy = rC.ToList();
                            var fillArrayCopy = fAC.ToList();

                            var currentRectangle = rectanglesCopy[i];

                            var stapledRectangle = StapleToPoint(currentRectangle, Corners.C10, aP.Point);
                            if (IsValidPositionForRectangle(fillArrayCopy, stapledRectangle))
                            {
                                fillArrayCopy.Add(stapledRectangle);
                                rectanglesCopy.RemoveAt(i);
                                var arrangment = FindBestArrangment(rectanglesCopy, fillArrayCopy);
                                float score = Score(arrangment);
                                if (score > bestScore)
                                {
                                    bestArrangment = arrangment;
                                    bestScore = score;
                                }
                            }
                        }

                        if (!aP.AttachmentLocks.HasFlag(AttachmentLocks.DOWN_R))
                        {
                            var rectanglesCopy = rC.ToList();
                            var fillArrayCopy = fAC.ToList();

                            var currentRectangle = rectanglesCopy[i];

                            var stapledRectangle = StapleToPoint(currentRectangle, Corners.C00, aP.Point);
                            if (IsValidPositionForRectangle(fillArrayCopy, stapledRectangle))
                            {
                                fillArrayCopy.Add(stapledRectangle);
                                rectanglesCopy.RemoveAt(i);
                                var arrangment = FindBestArrangment(rectanglesCopy, fillArrayCopy);
                                float score = Score(arrangment);
                                if (score > bestScore)
                                {
                                    bestArrangment = arrangment;
                                    bestScore = score;
                                }
                            }
                        }
                    }
                }



            }

            return bestArrangment;
        }

        private float Score(List<Rectangle> rectangles)
        {
            float maxX = 0f, maxY = 0f;
            float usedArea = 0f;
            foreach(Rectangle r in rectangles)
            {
                usedArea += r.Width * r.Height;

                if (r.X + r.Width > maxX)
                    maxX = r.X + r.Width;
                if (r.Y + r.Height > maxY)
                    maxY = r.Y + r.Height;
            }

            float totalArea = maxX * maxY;

            return usedArea / totalArea;
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

        /// <summary>
        /// Transforms the rectangle (Point a) to touch the corner of another rectangle (Point b).  Returns an offset coordinate.
        /// </summary>
        /// <param name="a">The rectangle to be transformed</param>
        /// <param name="b">The rectangle which holds the corners that the other rectangle will attach to</param>
        /// <param name="aCorner">A corner on the transformed rectangle represented in normalized Coordinates</param>
        /// <param name="bCorner">A corner on the non-transformed rectangle represented in normalized Coordinates</param>
        private Rectangle StapleToCorner(Rectangle a, Rectangle b, Point aCorner, Point bCorner)
        {
            return new Rectangle(b.Width * bCorner.X - a.Width * aCorner.X, b.Height * bCorner.Y - a.Height * aCorner.Y, a.Width, a.Height);
        }

        private Rectangle StapleToPoint(Rectangle a, Corners corner, Point p)
        {
            var cornerPoint = GetCorner(a, corner, false);
            return new Rectangle(p.X - cornerPoint.X, p.Y - cornerPoint.Y, a.Width, a.Height);
        }

        //    C00     UP     C10
        //       0----------1
        //  LEFT |          | RIGHT
        //       2----------3
        //    C01    DOWN    C11

        private List<AttachmentPoint> GetAttachmentPoints(List<Rectangle> rectangles)
        {
            var points = GetCorners(rectangles).Distinct();
            List<AttachmentPoint> attachmentPoints = new List<AttachmentPoint>();
            foreach(Point p in points)
            {
                AttachmentLocks aLocks = AttachmentLocks.ALL_OPEN;
                foreach(Rectangle r in rectangles)
                {
                    //checks if two points share the same position and applies the appropriate lock flag
                    bool foundMatch = false;
                    var corners = GetCorners(r);
                    for(int idx = 0; idx < 4; idx++)
                    {
                        if(corners[idx] == p)
                        {
                            switch (idx)
                            {
                                case (0): aLocks |= AttachmentLocks.DOWN_R;   foundMatch = true; break;
                                case (1): aLocks |= AttachmentLocks.DOWN_L;   foundMatch = true; break;
                                case (2): aLocks |= AttachmentLocks.UP_R; foundMatch = true; break;
                                case (3): aLocks |= AttachmentLocks.UP_L; foundMatch = true; break;
                            }

                            break;
                        }
                    }
                    if (foundMatch)
                        continue;
                    else
                    {
                        //checks if this point shares a value with an edge and locks placements if it does
                        var edges = GetEdges(r);
                        for(int idx = 0; idx < 4; idx++)
                        {
                            if(edges[idx].ContainsPoint(p, true))
                            {
                                aLocks |= GetLocksFromEdge(r, edges[idx]);
                            }
                        }
                    }
                }

                //checking if this point is against an axis boundary, which would lead to an invalid position
                if(p.X == 0)
                    aLocks |= AttachmentLocks.UP_L | AttachmentLocks.DOWN_L;
                if (p.Y == 0)
                    aLocks |= AttachmentLocks.UP_L | AttachmentLocks.UP_R;

                if (aLocks != AttachmentLocks.ALL_LOCKED)
                    attachmentPoints.Add(new AttachmentPoint(p, aLocks));
            }

            return attachmentPoints;
        }

        private List<AttachmentPoint> GetValidAttachmentPoints(List<Rectangle> rectangles, List<Rectangle> testRectangles, List<AttachmentPoint> attachmentPoints)
        {
            foreach(AttachmentPoint aP in attachmentPoints)
            {
                AttachmentLocks locks = aP.AttachmentLocks;
                bool isValidPoint = true;
                int validRectangles = testRectangles.Count();

                if (!locks.HasFlag(AttachmentLocks.UP_L))
                {
                    int valids = validRectangles;
                    foreach(Rectangle r in testRectangles)
                    {
                        if (!IsValidPositionForRectangle(rectangles, StapleToPoint(r, Corners.C11, aP.Point)))
                        {
                            valids--;
                        }
                    }

                    if (valids == 0)
                        locks |= AttachmentLocks.UP_L;
                }

                if (!locks.HasFlag(AttachmentLocks.UP_R))
                {
                    int valids = validRectangles;
                    foreach (Rectangle r in testRectangles)
                    {
                        if (!IsValidPositionForRectangle(rectangles, StapleToPoint(r, Corners.C01, aP.Point)))
                        {
                            valids--;
                        }
                    }

                    if (valids == 0)
                        locks |= AttachmentLocks.UP_R;
                }

                if (!locks.HasFlag(AttachmentLocks.DOWN_L))
                {
                    int valids = validRectangles;
                    foreach (Rectangle r in testRectangles)
                    {
                        if (!IsValidPositionForRectangle(rectangles, StapleToPoint(r, Corners.C10, aP.Point)))
                        {
                            valids--;
                        }
                    }

                    if (valids == 0)
                        locks |= AttachmentLocks.DOWN_L;
                }

                if (!locks.HasFlag(AttachmentLocks.DOWN_R))
                {
                    int valids = validRectangles;
                    foreach (Rectangle r in testRectangles)
                    {
                        if (!IsValidPositionForRectangle(rectangles, StapleToPoint(r, Corners.C00, aP.Point)))
                        {
                            valids--;
                        }
                    }

                    if (valids == 0)
                        locks |= AttachmentLocks.DOWN_R;
                }

                if (locks == AttachmentLocks.ALL_LOCKED)
                    aP.AttachmentLocks = locks;
            }
            return attachmentPoints.Where(aP => aP.AttachmentLocks != AttachmentLocks.ALL_LOCKED).ToList();
        }

        private List<AttachmentPoint> GetValidAttachmentPoints(List<Rectangle> rectangles, List<Rectangle> testRectangles)
        {
            var attachmentPoints = GetAttachmentPoints(rectangles);
            return GetValidAttachmentPoints(rectangles, testRectangles, attachmentPoints);
        }

        /// <summary>
        /// Determines if a given rectangle is valid at it's currently location (i.e. the union of the space of the rectangle with any other, disregarding edges, is a null set)
        /// </summary>
        /// <param name="rectangles">The set of rectangles to test against (fillArray)</param>
        /// <param name="testR">The stapled rectangle that is being tested</param>
        /// <returns></returns>
        private bool IsValidPositionForRectangle(List<Rectangle> rectangles, Rectangle testR)
        {
            var staticPoints = GetCorners(testR); //The points of the rectangle that has been stapled
            var staticEdges = GetEdges(testR); //The edges of the rectangles that has been stapled

            //checks for negative coordinates
            foreach(Point p in staticPoints)
            {
                if (p.X < 0 || p.Y < 0)
                    return false;
            }

            //checks against all rectangles currently in position
            foreach(Rectangle r in rectangles)
            {
                var testCorners = GetCorners(r); //not to be confused with staticPoints, this is the set of points for an existing rectangle in fillArray

                //test if any points of testR lie in a stapled rectangle, non-inclusive of edges
                foreach(Point p in staticPoints)
                {
                    if (RectangleContainsPoint(r, p))
                        return false;
                }

                //test if any points of a stapled rectangle lie inside of testR
                foreach(Point p in testCorners)
                {
                    if (RectangleContainsPoint(testR, p))
                        return false;
                }

                //tests for overlapping rectangles via edge sharing, point inclusion, and intersections
                var testEdges = GetEdges(r);
                for(int idx = 0; idx < testEdges.Count(); idx++)
                {
                    for(int jdx = 0; jdx < staticEdges.Count(); jdx++)
                    {
                        // +------O-------+        +----O---------+ 
                        // |      I       |        |    I         |
                        // |      I       |   or   |    O         |
                        // |      I       |        |              |
                        // +------O-------+        +--------------+

                        if (testEdges[idx].ContainsPoint(staticEdges[jdx].P1, true))
                        {
                            if (testEdges[(idx + 2) % 4].ContainsPoint(staticEdges[jdx].P2, true) || RectangleContainsPoint(r, staticEdges[jdx].P2))
                                return false;
                        }
                        else if(testEdges[idx].ContainsPoint(staticEdges[jdx].P2, true))
                        {
                            if (testEdges[(idx + 2) % 4].ContainsPoint(staticEdges[jdx].P1, true) || RectangleContainsPoint(r, staticEdges[jdx].P1))
                                return false;
                        }
                        //        +
                        // +------O---+
                        //        |
                        //        |
                        //        +
                        else if (testEdges[idx].Intersects(staticEdges[jdx]))
                            return false;
                        // [==========A========]
                        // [=====B=====]
                        // +-----------+-------+
                        // |           |       |
                        // |           |       |
                        // |           |       |
                        // +-----------+-------+
                        else if (testEdges[idx].SharesEdge(staticEdges[jdx]) && testEdges[idx].EdgeDirection == staticEdges[jdx].EdgeDirection)
                            return false;

                    }
                }
            }

            return true;
        }

        private bool RectangleContainsPoint(Rectangle r, Point p)
        {
            if ((r.X < p.X) && (r.Y < p.Y) && (p.X < r.X + r.Width) && (p.Y < r.Y + r.Height))
                return true;
            return false;
        }

        private AttachmentLocks GetLocksFromEdge(Rectangle r, Edge e)
        {
            return GetLocksFromPoint(r, e.P1) | GetLocksFromPoint(r, e.P2);
        }

        //    C00     UP     C10
        //       0----------1
        //  LEFT |          | RIGHT
        //       2----------3
        //    C01    DOWN    C11

        private AttachmentLocks GetLocksFromPoint(Rectangle r, Point p)
        {
            var corners = GetCorners(r);
            if (corners[0] == p)
                return AttachmentLocks.UP_L;
            else if (corners[1] == p)
                return AttachmentLocks.UP_R;
            else if (corners[2] == p)
                return AttachmentLocks.DOWN_L;
            else if (corners[3] == p)
                return AttachmentLocks.DOWN_R;
            else
                return AttachmentLocks.ALL_OPEN;
        }

        private Point GetCorner(Rectangle rectangle, Corners corner, bool includeLocation)
        {
            if (includeLocation)
            {
                switch (corner)
                {
                    case (Corners.C00): return new Point(rectangle.X                  , rectangle.Y                   );
                    case (Corners.C10): return new Point(rectangle.X + rectangle.Width, rectangle.Y                   );
                    case (Corners.C01): return new Point(rectangle.X                  , rectangle.Y + rectangle.Height);
                    case (Corners.C11): return new Point(rectangle.X + rectangle.Width, rectangle.Y + rectangle.Height);
                }
            }
            else
            {
                switch (corner)
                {
                    case (Corners.C00): return new Point(0              , 0               );
                    case (Corners.C10): return new Point(rectangle.Width, 0               );
                    case (Corners.C01): return new Point(0              , rectangle.Height);
                    case (Corners.C11): return new Point(rectangle.Width, rectangle.Height);
                }
            }

            return new Point(0, 0);
        }

        private List<Point> GetCorners(Rectangle rectangle)
        {
            List<Point> corners = new List<Point>(4);

            var l= rectangle.Location;
            var s = new Point(rectangle.Size);
            corners.Add(new Point(l.X      , l.Y      )); //Corners.C00  //UpLeft
            corners.Add(new Point(l.X + s.X, l.Y      )); //Corners.C10  //UpRight
            corners.Add(new Point(l.X      , l.Y + s.Y)); //Corners.C01  //DownLeft
            corners.Add(new Point(l.X + s.X, l.Y + s.Y)); //Corners.C11  //DownRight

            return corners;
        }

        private List<Point> GetCorners(List<Rectangle> rectangles)
        {
            List<Point> corners = new List<Point>();

            foreach(Rectangle r in rectangles)
            {
                var l = r.Location;
                var s = new Point(r.Size);
                corners.Add(new Point(l.X      , l.Y      )); //Corners.C00  //UpLeft
                corners.Add(new Point(l.X + s.X, l.Y      )); //Corners.C10  //UpRight
                corners.Add(new Point(l.X      , l.Y + s.Y)); //Corners.C01  //DownLeft
                corners.Add(new Point(l.X + s.X, l.Y + s.Y)); //Corners.C11  //DownRight
            }

            return corners;

        }

        private List<Edge> GetEdges(Rectangle rectangle)
        {
            List<Edge> edges = new List<Edge>(4);
            var corners = GetCorners(rectangle);
            edges.Add(new Edge(corners[0], corners[1], EdgeNormals.UP));
            edges.Add(new Edge(corners[0], corners[2], EdgeNormals.LEFT));
            edges.Add(new Edge(corners[1], corners[3], EdgeNormals.RIGHT));
            edges.Add(new Edge(corners[2], corners[3], EdgeNormals.DOWN));

            return edges;
        }

        private List<Edge> GetEdges(List<Rectangle> rectangles)
        {
            List<Edge> edges = new List<Edge>();
            foreach(Rectangle r in rectangles)
            {
                var corners = GetCorners(r);
                edges.Add(new Edge(corners[0], corners[1], EdgeNormals.UP));
                edges.Add(new Edge(corners[0], corners[2], EdgeNormals.LEFT));
                edges.Add(new Edge(corners[1], corners[3], EdgeNormals.RIGHT));
                edges.Add(new Edge(corners[2], corners[3], EdgeNormals.DOWN));
            }

            return edges;
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
