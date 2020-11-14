using DeeSynk.Core.Components.Models;
using DeeSynk.Core.Components.Types.Render;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;

namespace DeeSynk.Core.Managers
{
    using Model = DeeSynk.Core.Components.Models.Model;
    using Templates = DeeSynk.Core.Components.Models.Templates;
    using ModelTemplates = DeeSynk.Core.Components.Models.Templates.ModelTemplates;

    public enum PlyProperties : byte
    {
        UNKNOWN = 0,

        CHAR    = 1,
        UCHAR   = 2,
        SHORT   = 3,
        USHORT  = 4,
        INT     = 5,
        UINT    = 6,
        FLOAT   = 7,
        DOUBLE  = 8,

        X = 10,
        Y = 11,
        Z = 12,

        R = 20,
        G = 21,
        B = 22,

        VERTEX_INDEX = 30
    }

    //LoadColladaModel(@"C:\Users\Nicholas\source\repos\nicholaslaw07\DeeSynk\DeeSynkPort\Resources\Models\Collada\Moo.dae");
    //string[] models = Directory.GetFiles(FILE_PATH);
    //string[] fileNames = models.Select(Path.GetFileNameWithoutExtension).ToArray();
    //string[] fileExtensions = models.Select(Path.GetExtension).ToArray();

    public struct PlyPropertySimple
    {
        public readonly PlyProperties DataType;
        public readonly PlyProperties DataContent;
        public readonly int Stride;

        public PlyPropertySimple(PlyProperties dataType, PlyProperties dataContent, int stride)
        {
            DataType = dataType;
            DataContent = dataContent;
            Stride = stride;
        }
    }

    public struct PlyPropertyList
    {
        public readonly PlyProperties DataType1;
        public readonly PlyProperties DataType2;
        public readonly PlyProperties DataContent;
        public readonly int Stride1;
        public readonly int Stride2;

        public PlyPropertyList(PlyProperties dataType1, PlyProperties dataType2, PlyProperties dataContent, int stride1, int stride2)
        {
            DataType1 = dataType1;
            DataType2 = dataType2;
            DataContent = dataContent;
            Stride1 = stride1;
            Stride2 = stride2;
        }
    }

    public class ModelManager : IManager
    {
        private const string X_UP = "X_UP";
        private const string Y_UP = "Y_UP";
        private const string Z_UP = "Z_UP";

        private Vector3 DefaultOrder { get { return new Vector3(0.0f, 1.0f, 2.0f); } }

        private const string FILE_PATH = @"C:\Users\Nicholas\source\repos\nicholaslaw07\DeeSynk\DeeSynkPort\Resources\Models\";
        private static ModelManager _modelManager;

        private const int MAX_INDEX_SIZE = 10000;

        object _lockObject = new object();
        private Dictionary<string, Model> _modelLibrary;

        private ModelManager()
        {
            _modelLibrary = new Dictionary<string, Model>();
        }

        public static ref ModelManager GetInstance()
        {
            if (_modelManager == null)
            {
                _modelManager = new ModelManager();
            }

            return ref _modelManager;    
        }

        public void Load()
        {
            LoadColladaLibrary();

            //LoadPLYLibrary();
        }

        private void LoadColladaLibrary()
        {
            int count = CountFiles(FILE_PATH + @"Collada\");
            LoadColladaFolder(FILE_PATH + @"Collada\");
            while (_modelLibrary.Keys.Count() < count)
                Thread.Sleep(50);
        }

        private void LoadPLYLibrary()
        {
            string path0 = @"C:\Users\Nicholas\Documents\complete";
            LoadPlyFolder(path0);
        }

        private void LoadColladaFolder(string folderPath)
        {
            var directories = Directory.GetDirectories(folderPath);
            if (directories.Count() == 0)
            {
                var files = Directory.GetFiles(folderPath);

                int fileCount = files.Count();

                foreach (string f in files)
                {
                    string name = Path.GetFileName(f);
                    int idx = name.IndexOf('.');
                    name = name.Substring(0, idx);
                    (new Thread(unused => LoadColladaModel(f, name))).Start();
                }
            }
            else
            {
                foreach (string f in directories)
                {
                    if (f == folderPath + @"Schema")
                        continue;
                    LoadColladaFolder(f);
                }
            }
        }

        private int CountFiles(string folderPath)
        {
            var directories = Directory.GetDirectories(folderPath);
            int count = 0;
            if(directories.Count() == 0)
            {
                var files = Directory.GetFiles(folderPath);

                count += files.Count();
            }
            else
            {
                foreach (string f in directories)
                {
                    if (f == folderPath + @"Schema")
                        continue;
                    count += CountFiles(f);
                }
            }
            return count;
        }

        private void LoadPlyFolder(string folderPath)
        {
            var directories = Directory.GetDirectories(folderPath);
            if(directories.Count() == 0)
            {
                var files = Directory.GetFiles(folderPath);
                
                foreach(string f in files)
                {
                    string name = Path.GetFileName(f);
                    int idx = name.IndexOf('.');
                    name = name.Substring(0, idx);
                    LoadPLY(f, name);
                }
            }
            else
            {
                foreach (string f in directories)
                    LoadPlyFolder(f);
            }
        }

        #region Model Retrieval

        public void InitModel(ref ComponentModelStatic modelComp)
        {
            if (modelComp.ModelReferenceType == ModelReferenceType.TEMPLATE)
            {
                CreateModelFromTemplate(ref modelComp);
            }
        }

        public Model GetModel(string name)
        {
            Model value;
            var success = _modelLibrary.TryGetValue(name, out value);
            if (success)
                return value;
            else
                return null;
        }

        public Model GetModel(ref ComponentModelStatic modelComp)
        {
            if(modelComp.ModelReferenceType == ModelReferenceType.TEMPLATE)
            {
                if (!ModelExists(modelComp.ModelID))
                {
                    CreateModelFromTemplate(ref modelComp);
                } 
            }

            return GetModel(modelComp.ModelID);
        }

        private void CreateModelFromTemplate(ref ComponentModelStatic modelComp)
        {
            Model model = GetModelFromTemplate(ref modelComp);

            string name = GetValidNameForModel(Model.GetTemplateName(modelComp.TemplateID));
            modelComp.ModelID = name;
            if (model != null)
                _modelLibrary.Add(name, model);
        }

        private Model GetModelFromTemplate(ref ComponentModelStatic modelComp)
        {
            return modelComp.TemplateData.ConstructModel(modelComp);
        }

        private string GetValidNameForModel(string keyName)
        {
            string testName = "";
            int superIndex = 0;
            int index = 0;
            do
            {
                if (index >= MAX_INDEX_SIZE)
                {
                    index = 0;
                    superIndex++;
                }

                if (superIndex >= MAX_INDEX_SIZE)
                {
                    throw new Exception("Woahhhh cowboy, that's too many object you have there.  You can't herd that many cows!");
                }
                testName = keyName + ((superIndex > 0) ? "" : ("_" + superIndex.ToString() + "_")) + index.ToString();
                index++;
            } while (_modelLibrary.Keys.Contains(testName));

            return testName;
        }

        public bool ModelExists(string modelID)
        {
            if (_modelLibrary.Keys.Contains(modelID))
                return true;
            else
                return false;
        }

        public void UnLoad()
        {
            foreach (string s in _modelLibrary.Keys)
                _modelLibrary.Remove(s);
        }

        #endregion

        #region Parsers and File Loading

        public void LoadColladaModel(string filePath, string name)
        {
            try
            {
                string xmlns = "{http://wwww.collada.org/2005/11/COLLADASchema}";
                //Load the xml document from a file
                XDocument modelDoc = XDocument.Load(filePath);

                //DEBUG --- VIEWING ALL DATA
                foreach (XElement element in modelDoc.Root.Elements())
                {
                    //Console.WriteLine(element.ToString());
                }
                //END

                //Retrieve preliminary information
                var asset = modelDoc.Root.Elements().Where(a => a.Name.LocalName == "asset").First();
                var upAxis = asset.Elements().Where(a => a.Name.LocalName == "up_axis").First().Value;  //Valid values are X_UP Y_UP Z_UP
                if (upAxis == null)
                    upAxis = Y_UP;

                //Load geometry
                    //Isolate the geometry library 'library_geometries'
                var libraryGeometries = modelDoc.Root
                    .Elements()
                    .Where(e => e.Name.LocalName == "library_geometries")
                    .First();

                    //Isolate the first geomtry element 'geometry'
                var geometryElement = libraryGeometries.Elements()
                    .Where(e => e.Name.LocalName == "geometry")
                    .First();

                    //Isolate the id of the first geometry element
                var geometryAttribute = geometryElement.Attributes().Where(a => a.Name == "id").First();
                string geometryId = geometryAttribute.Value;

                    //Descend to the mesh data
                var meshGeometry = geometryElement.Elements().First().Elements();

                    //Descend to source data
                var sourceElements = meshGeometry.Where(b => b.Attribute(XName.Get("id")) != null);

                //for position and normals
                string FindVectorData(string dataName, out Vector3[] vectors) 
                {
                    //get source position element
                    var sourceElement = sourceElements.Where(a => a.Attribute(XName.Get("id")).Value == geometryId + "-" +dataName);

                    //get accessor element from technique element
                    var accessor = sourceElement.Elements()
                                    .Where(a => a.Name.LocalName == "technique_common")
                                    .First()
                                    .Elements()
                                    .Where(a => a.Name.LocalName == "accessor")
                                    .First();
                    int elementCount = 1;
                    int stride = 3; //default value
                    Int32.TryParse(accessor.Attribute(XName.Get("count")).Value, out elementCount);
                    Int32.TryParse(accessor.Attribute(XName.Get("stride")).Value, out stride); //get stride from accessor

                    //ACCOUNT FOR Z UP!!?!

                    var accessorParameters = accessor.Elements().Distinct();
                    foreach(XElement el in accessorParameters)
                    {
                        if (!("XYZ".Contains(el.Attribute(XName.Get("name")).Value)) ||
                            el.Attribute(XName.Get("type")).Value != "float")
                            throw new Exception("File lacks valid XYZ or float parameters");
                    }

                    //float array element
                    var floatArrayElement = sourceElement.Elements()
                        .Where(a => a.Name.LocalName == "float_array" && 
                               a.Attribute(XName.Get("id")).Value == geometryId + "-" + dataName + "-array")
                        .First();

                    int count = 3;
                    Int32.TryParse(floatArrayElement.Attribute(XName.Get("count")).Value, out count);

                    if (count != elementCount * stride)
                        throw new Exception("Mismatched data in file: array data count does not match what is expected");


                    ParseVectorData(floatArrayElement.Value.Split(' '), stride, elementCount, out vectors);

                    return sourceElement.First().Attribute(XName.Get("id")).Value;
                }

                void ParseVectorData(string[] data, int stride, int elementCount, out Vector3[] vectors)
                {
                    if (data.Length != stride * elementCount)
                        throw new Exception("Mismatched data in file: array data count does not match what is expected");

                    var order = ElementOrder(upAxis);

                    int xOffset = (int)order.X;
                    int yOffset = (int)order.Y;
                    int zOffset = (int)order.Z;

                    bool flipX = (upAxis == X_UP);
                    bool flipZ = (upAxis == Z_UP);

                    vectors = new Vector3[elementCount];
                    for (int i = 0; i < elementCount; i++)
                    {
                        float.TryParse(data[i * stride + xOffset], out vectors[i].X);
                        float.TryParse(data[i * stride + yOffset], out vectors[i].Y);
                        float.TryParse(data[i * stride + zOffset], out vectors[i].Z);
                        if (flipX) vectors[i].X *= -1f;
                        if (flipZ) vectors[i].Z *= -1f;
                    }
                }

                string positionSourceId = FindVectorData("positions", out Vector3[] positions);
                string normalSourceId = FindVectorData("normals", out Vector3[] normals);

                //Parse triangle data


                void ParseTriangleData(out uint[] vertexOrder, out uint[] normalOrder)
                {
                    var triangleElement = meshGeometry.Where(a => a.Name.LocalName == "triangles").First();
                    int triCount = 1;
                    Int32.TryParse(triangleElement.Attribute(XName.Get("count")).Value, out triCount);

                    vertexOrder = new uint[triCount * 3];
                    normalOrder = new uint[triCount * 3];

                    var verticesSemantic = meshGeometry.Where(a => a.Name.LocalName == "vertices").First();
                    string verticesId = verticesSemantic.Attribute(XName.Get("id")).Value;
                    var inputSemantic = verticesSemantic.Elements().First();
                    if (inputSemantic.Attribute(XName.Get("semantic")).Value != "POSITION" || inputSemantic.Attribute(XName.Get("source")).Value != "#" + positionSourceId)
                        throw new Exception("Unknown input semantic for vertex data");

                    //Get the input semantics
                    var inputSemantics = triangleElement.Elements().Where(a => a.Name.LocalName == "input");

                    //Get the vertex and normals semantic
                    var vertexSemantic = inputSemantics.Where(a => a.Attribute(XName.Get("semantic")).Value == "VERTEX").First(); //.Attribute(XName.Get("source")).Value
                    var normalSemantic = inputSemantics.Where(a => a.Attribute(XName.Get("semantic")).Value == "NORMAL").First(); //.Attribute(XName.Get("source")).Value

                    var vertexSource = vertexSemantic.Attribute(XName.Get("source")).Value;
                    var normalSource = normalSemantic.Attribute(XName.Get("source")).Value;

                    if (vertexSource != "#" + verticesId || normalSource != "#" + normalSourceId)
                        throw new Exception("Invalid vertex semantics, could not find vertex source");

                    int vertexOffset = 0;
                    int normalOffset = 1;
                    Int32.TryParse(vertexSemantic.Attribute(XName.Get("offset")).Value, out vertexOffset);
                    Int32.TryParse(normalSemantic.Attribute(XName.Get("offset")).Value, out normalOffset);

                    var data = triangleElement.Elements().Where(a => a.Name.LocalName == "p").First().Value;

                    string[] splitData = data.Split(' ');
                    if (splitData.Length != triCount * 3 * 2)
                        throw new Exception("Number of elements read does not match the number expected");

                    for (int i = 0; i < triCount * 3; i++)
                    {
                        UInt32.TryParse(splitData[2 * i + vertexOffset].ToString(), out vertexOrder[i]);
                        UInt32.TryParse(splitData[2 * i + normalOffset].ToString(), out normalOrder[i]);
                    }
                }

                ParseTriangleData(out uint[] vOrder, out uint[] nOrder);

                Model model = new Model();

                Vector4[] positionsNew = new Vector4[positions.Length];
                for(int i=0; i<positions.Length; i++)
                    positionsNew[i] = new Vector4(positions[i], 1.0f);

                model.Vertices = positionsNew;
                //model.Normals = normals;
                model.Elements = vOrder;
                model.SetReadOnly(true, true);
                lock(_lockObject)
                    _modelLibrary.Add(name, model);

            }
            catch(Exception e)
            {
                throw new Exception($@"Error loading '.DAE' file type at {filePath}", e);
            }
        }

        private Vector3 ElementOrder(string upAxis)
        {
            Vector3 order = DefaultOrder;
            switch (upAxis)
            {
                case (X_UP): order = order.Yxz; return order;
                case (Z_UP): order = order.Xzy; return order;
                default: return order;
            }
        }

        #endregion

        //I am not including this into the other section until this is more compelte.  This is currently only setup for a specific format.
        #region PLY_PARSER
        private void LoadPLY(string path, string name)
        {
            //4 4 4 1 1 1 4 4 4
            //1 4 4 4 1 1 1

            //TRIANGLE FAN NOT STRIP

            byte[] file = File.ReadAllBytes(path);
            StringBuilder sb = new StringBuilder();
            ASCIIEncoding asc = new ASCIIEncoding();
            byte[] lookup;

            lookup = asc.GetBytes("element vertex ");
            int a = FindIndex(in file, in lookup, 0), l, b, vC, fC;
            l = lookup.Length;
            lookup = asc.GetBytes("\n");
            b = FindIndex(in file, in lookup, a + l);
            var cT = asc.GetChars(file, a + l, b - a - l);
            int vb = b - a - l;
            foreach (char ch in cT)
                sb.Append(ch);
            Int32.TryParse(sb.ToString(), out vC);

            sb.Clear();

            lookup = asc.GetBytes("element face ");
            a = FindIndex(in file, in lookup, 0);
            l = lookup.Length;
            lookup = asc.GetBytes("\n");
            b = FindIndex(in file, in lookup, a + l);
            cT = asc.GetChars(file, a + l, b - a - l);
            int fb = b - a - l;
            foreach (char ch in cT)
                sb.Append(ch);
            Int32.TryParse(sb.ToString(), out fC);

            lookup = asc.GetBytes("\nend_header\n");
            a = FindIndex(in file, in lookup, a);
            a += lookup.Length;

            lookup = asc.GetBytes("ply\nformat binary_big_endian 1.0\nelement vertex \nproperty float x\nproperty float y\nproperty float z\nproperty uchar red\nproperty uchar green\nproperty uchar blue\nproperty float nx\nproperty float ny\nproperty float nz\nelement face \nproperty list uchar int vertex_indices\nproperty uchar red\nproperty uchar green\nproperty uchar blue\nend_header\n");
            int len = lookup.Length + vb + fb;
            if (len != a)
                throw new Exception("Fuck");

            Vector4[] vertices = new Vector4[vC];
            Color4[] colors = new Color4[vC];
            Vector3[] normals = new Vector3[vC];

            int filesize = a + vC * 27 + fC * 16;
            int partialSize = a + vC * 27; // 70 21 171 180

            a -= 4;

            if (BitConverter.IsLittleEndian)
            {
                for (int i = 0; i < vC; i++)
                {
                    float x = BitConverter.ToSingle(GetSubsetLE(in file, a += 4, 4), 0);
                    float y = BitConverter.ToSingle(GetSubsetLE(in file, a += 4, 4), 0);
                    float z = BitConverter.ToSingle(GetSubsetLE(in file, a += 4, 4), 0);

                    vertices[i] = new Vector4(x, y, z, 1.0f);

                    byte R = file[++a];
                    byte G = file[++a];
                    byte B = file[++a];

                    colors[i] = new Color4(R, G, B, 255);

                    float nx = BitConverter.ToSingle(GetSubsetLE(in file, a += 4, 4), 0);
                    float ny = BitConverter.ToSingle(GetSubsetLE(in file, a += 4, 4), 0);
                    float nz = BitConverter.ToSingle(GetSubsetLE(in file, a += 4, 4), 0);

                    normals[i] = new Vector3(nx, ny, nz);
                }
            }
            else
            {
                for (int i = 0; i < vC; i++)
                {
                    float x = BitConverter.ToSingle(GetSubsetBE(in file, a += 4, 4), 0);
                    float y = BitConverter.ToSingle(GetSubsetBE(in file, a += 4, 4), 0);
                    float z = BitConverter.ToSingle(GetSubsetBE(in file, a += 4, 4), 0);

                    vertices[i] = new Vector4(x, y, z, 1.0f);

                    byte R = file[++a];
                    byte G = file[++a];
                    byte B = file[++a];

                    colors[i] = new Color4(R, G, B, 255);

                    float nx = BitConverter.ToSingle(GetSubsetBE(in file, a += 4, 4), 0);
                    float ny = BitConverter.ToSingle(GetSubsetBE(in file, a += 4, 4), 0);
                    float nz = BitConverter.ToSingle(GetSubsetBE(in file, a += 4, 4), 0);

                    normals[i] = new Vector3(nx, ny, nz);
                }
            }

            a += 3;
            int ap = a;
            ap++;
            int eCount = 0;

            for(int i=0; i< fC; i++)
            {
                byte A = file[ap];
                ap += 4 * (int)A + 4;
                eCount += (int)A;
            }

            uint[] elements = new uint[eCount];
            Color4[] eColors = new Color4[eCount];

            ap = a;

            if (BitConverter.IsLittleEndian)
            {
                for (int i = 0; i < eCount; i+=0)
                {
                    byte A = file[++ap];
                    if (A != (byte)3)
                        throw new Exception("Uh oh");
                    ap -= 3;
                    for (int j = 0; j < A; j++)
                        elements[i+j] = (uint)BitConverter.ToInt32(GetSubsetLE(in file, ap += 4, 4), 0);
                    byte R = file[ap += 4];
                    byte G = file[++ap];
                    byte B = file[++ap];
                    eColors[i] = new Color4(R, G, B, 255);
                    i += (int)A;
                }
            }
            else
            {
                for (int i = 0; i < fC; i+=0)
                {
                    byte A = file[++ap];
                    if (A != (byte)3)
                        throw new Exception("Uh oh");
                    ap -= 3;
                    for (int j = 0; j < A; j++)
                        elements[i+j] = (uint)BitConverter.ToInt32(GetSubsetBE(in file, ap += 4, 4), 0);
                    byte R = file[ap += 4];
                    byte G = file[++ap];
                    byte B = file[++ap];
                    eColors[i] = new Color4(R, G, B, 255);
                    i += (int)A;
                }
            }

            uint max = 0;
            for (int i = 0; i < elements.Length; i++)
            {
                if (elements[i] > max)
                    max = elements[i];
            }

            Model model = new Model();

            model.Vertices = vertices;
            model.Normals = normals;
            model.Elements = elements;
            model.Colors = colors;
            model.SetReadOnly(false, false);
            lock (_modelLibrary)
            {
                _modelLibrary.Add(_modelLibrary.Count().ToString(), model);
            }
        }
        private int FindIndex(in byte[] arr, in byte[] val, int start)
        {
            for (int i = start; i < arr.Length; i++)
            {
                if (arr[i] == val[0])
                {
                    bool pass = true;
                    for (int j = 1; j < val.Length; j++)
                    {
                        pass &= arr[i + j] == val[j];
                    }
                    if (pass)
                        return i;
                }
            }
            return -1;
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

    }
    #endregion
}
