using DeeSynk.Core.Components.Models;
using DeeSynk.Core.Components.Types.Render;
using OpenTK;
using OpenTK.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;

namespace DeeSynk.Core.Managers
{
    using Model = DeeSynk.Core.Components.Models.Model;

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
        private const string FILE_PATH = @"..\..\Resources\Models\";
        private static ModelManager _modelManager;

        private const int MAX_INDEX_SIZE = 10000;

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
            LoadColladaModel(@"..\..\Resources\Models\Collada\Moo.dae");

            string[] models = Directory.GetFiles(FILE_PATH);
            string[] fileNames = models.Select(Path.GetFileNameWithoutExtension).ToArray();
            string[] fileExtensions = models.Select(Path.GetExtension).ToArray();
            /*
            for (int idx = 0; idx < models.Length; idx++)
            {
                switch (fileExtensions[idx])
                {
                    case (".obj"):
                        ParseObj(models[idx]);
                        break;
                    case (".ply"):
                        TryParsePly(models[idx]);
                        break;

                }
            }*/

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
            switch (modelComp.TemplateID)
            {
                case (ModelTemplates.TemplatePlaneXZ):
                    return Model.CreateTemplatePlaneXZ(ref modelComp);
                case (ModelTemplates.TemplatePlaneXY):
                    return Model.CreateTemplatePlaneXY(ref modelComp);
                case (ModelTemplates.TemplatePlaneYZ):
                    return Model.CreateTemplatePlaneYZ(ref modelComp);
            }

            return null;
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
                    throw new Exception("Woah there cowboy, that's too many object you have there.  You can wrangle that many cows now can you.");
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
            throw new NotImplementedException();
        }

        #endregion

        #region Parsers and File Loading

        public void ParseObj(string filePath)
        {
            var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            using (var StreamReader = new StreamReader(fileStream, Encoding.UTF8))
            {
                Model model = new Model();
                string file = StreamReader.ReadToEnd();

                Vector4[] vertices;

                string rs = @"[v] -?\d\.\d* -?\d.\d* -?\d.\d*";

                Regex regex = new Regex(rs);
                var matches = regex.Matches(file);

                vertices = new Vector4[matches.Count];

                rs = @"-?\d\.\d+(e-?\d+)?";
                regex = new Regex(rs);

                int idx = 0;
                foreach (Match m in matches)
                {
                    string s = m.Value;
                    var components = regex.Matches(s);
                    float x = float.Parse(components[0].ToString());
                    float y = float.Parse(components[1].ToString());
                    float z = float.Parse(components[2].ToString());

                    vertices[idx] = new Vector4(x, y, z, 1);
                    idx++;
                }

                Vector3[] normals;

                rs = @"vn -?\d\.\d* -?\d.\d* -?\d.\d*";

                regex = new Regex(rs);
                matches = regex.Matches(file);

                normals = new Vector3[matches.Count];

                rs = @"-?\d\.\d+(e-?\d+)?";
                regex = new Regex(rs);
                idx = 0;
                foreach (Match m in matches)
                {
                    string s = m.Value;
                    var components = regex.Matches(s);
                    float x = float.Parse(components[0].Value);
                    float y = float.Parse(components[1].Value);
                    float z = float.Parse(components[2].Value);

                    normals[idx] = new Vector3(x, y, z);
                    idx++;
                }

                uint[] vertexIndices, normalIndices;

                rs = @"f \d*//\d* \d*//\d* \d*//\d*";

                regex = new Regex(rs);
                matches = regex.Matches(file);

                vertexIndices = new uint[matches.Count * 3];
                normalIndices = new uint[matches.Count * 3];

                rs = @"\d+";
                regex = new Regex(rs);
                idx = 0;
                foreach (Match m in matches)
                {
                    string s = m.Value;
                    var components = regex.Matches(s);
                    uint v1 = uint.Parse(components[0].Value);
                    uint n1 = uint.Parse(components[1].Value);
                    uint v2 = uint.Parse(components[2].Value);
                    uint n2 = uint.Parse(components[3].Value);
                    uint v3 = uint.Parse(components[4].Value);
                    uint n3 = uint.Parse(components[5].Value);

                    vertexIndices[idx] = v1;
                    normalIndices[idx] = n1;
                    idx++;

                    vertexIndices[idx] = v2;
                    normalIndices[idx] = n2;
                    idx++;

                    vertexIndices[idx] = v3;
                    normalIndices[idx] = n3;
                    idx++;
                }

                model.Vertices = vertices;
                model.Normals = normals;
                model.Elements = vertexIndices;
                model.SetReadOnly();

                _modelLibrary.Add(Path.GetFileNameWithoutExtension(filePath), model);
            }
        }

        public void TryParsePly(string filePath)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            try
            {
                var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                using (var StreamReader = new StreamReader(fileStream, Encoding.UTF8))
                {
                    Model model = new Model();

                    string file = StreamReader.ReadToEnd();
                    Console.WriteLine("Loaded .ply model file at path: {0}", filePath);

                    string endHeaderKeyword = "end_header";
                    int dataIndex = file.IndexOf(endHeaderKeyword) + endHeaderKeyword.Length;
                    string header = file.Substring(0, dataIndex);
                    string data = file.Remove(0, dataIndex);

                    //Format Specification
                    string format = "ascii"; //default
                    string version = "1.0";  //default
                    {
                        string key = @"format\s(ascii|binary)\w*\s(\d+(\.{1}\d+)?)";
                        Regex regex = new Regex(key);
                        var matches = regex.Matches(header);
                        if(matches.Count == 1)
                        {
                            string captureFormat = matches[0].Value;

                            key = @"(ascii|binary)\w*";
                            regex = new Regex(key);
                            matches = regex.Matches(captureFormat);
                            format = matches[0].Value;

                            key = @"(\d+(\.{1}\d+)?)";
                            regex = new Regex(key);
                            matches = regex.Matches(captureFormat);
                            version = matches[0].Value;
                        }
                    }

                    Console.WriteLine("Parsing using format: {0} {1}...", format, version);

                    //Vertex Specification
                    int vertexElementCount = 0;
                    List<PlyPropertySimple> vertexProperties = new List<PlyPropertySimple>(); 
                    {
                        string key = @"element\svertex\s\d+\s*(\nproperty ([^l]\w+)\s(?:property|\w+))+";
                        Regex regex = new Regex(key);
                        var matches = regex.Matches(header);
                        if(matches.Count == 1)
                        {
                            string vertexHeader = matches[0].Value;

                            //Vertex count
                            key = @"(\d+)";
                            regex = new Regex(key);
                            matches = regex.Matches(vertexHeader); //there should be only one match
                            vertexElementCount = int.Parse(matches[0].Value);

                            //Vertex properties
                            key = @"property ([^l]\w+)\s(?:property|\w+)";
                            regex = new Regex(key);
                            matches = regex.Matches(vertexHeader); //retrieving the simple properties from the header

                            if (matches.Count > 0)
                            {
                                key = @"\w+";
                                regex = new Regex(key);            //now that the properties have been found, we will add them to a list
                                foreach (var match in matches)
                                {
                                    matches = regex.Matches(match.ToString());
                                    var dataType = PlyStringToProperty(matches[1].Value);
                                    var dataContent = PlyStringToProperty(matches[2].Value);
                                    vertexProperties.Add(new PlyPropertySimple(
                                                                dataType,
                                                                dataContent,
                                                                GetStride(dataType)));
                                }
                            }
                            else
                                throw new FileLoadException("Invalid .ply file format: could not find any vertex properties");
                        }
                        else
                            throw new FileLoadException("Parsing error: null or duplicate vertex specification");
                    }

                    bool hasColor = vertexProperties.Where((PlyPropertySimple p) => p.DataContent == PlyProperties.R ||
                                                                                    p.DataContent == PlyProperties.G ||
                                                                                    p.DataContent == PlyProperties.B)
                                                                                    .Count() == 3;
                    //Face Specification
                    int faceElementCount = 0;
                    PlyPropertyList faceList = new PlyPropertyList(PlyProperties.UNKNOWN, PlyProperties.UNKNOWN, PlyProperties.UNKNOWN, 0, 0);
                    {
                        string key = @"element\sface\s\d+\s*(\nproperty\slist(\s\w+){3}){1}";
                        Regex regex = new Regex(key);
                        var matches = regex.Matches(header);
                        if (matches.Count == 1)
                        {
                            string faceHeader = matches[0].Value;

                            key = @"\d+";
                            regex = new Regex(key);
                            matches = regex.Matches(faceHeader); //There should only be one match
                            faceElementCount = int.Parse(matches[0].Value);

                            key = @"property\slist(\s\w+){3}";
                            regex = new Regex(key);
                            matches = regex.Matches(faceHeader);
                            if(matches.Count == 1)
                            {
                                string faceListCapture = matches[0].Value;

                                key = @"\w+";
                                regex = new Regex(key);
                                matches = regex.Matches(faceListCapture);
                                if(matches.Count == 5 && matches[0].Value == "property" && matches[1].Value == "list")
                                {
                                    var dataType1 = PlyStringToProperty(matches[2].Value);
                                    var dataType2 = PlyStringToProperty(matches[3].Value);
                                    var dataContent = PlyStringToProperty(matches[4].Value);
                                    faceList = new PlyPropertyList(dataType1,
                                                                   dataType2,
                                                                   dataContent,
                                                                   GetStride(dataType1),
                                                                   GetStride(dataType2));
                                }
                            }
                        }
                        else
                            throw new FileLoadException("Parsing error: null or duplicate face specification");
                    }

                    //moving on to the data section
                    string lastVertexElement = "";


                    Vector4[] vertices = new Vector4[vertexElementCount];
                    Color4[] colors = new Color4[vertexElementCount];
                    uint[] faceIndices = new uint[faceElementCount * 3];
                    if (format.Contains("ascii"))
                    {
                        string key = @"-?\d\.\d+(e-?\d+)?";
                        Regex regex = new Regex(key);
                        var matches = regex.Matches(data);
                        if (matches.Count == vertexElementCount * 3)
                        {
                            for (int idx = 0; idx < vertexElementCount; idx++)
                            {
                                float x = float.Parse(matches[idx * 3].Value);
                                float y = float.Parse(matches[idx * 3 + 1].Value);
                                float z = float.Parse(matches[idx * 3 + 2].Value);

                                vertices[idx] = new Vector4(x, y, z, 1);
                            }

                            lastVertexElement = matches[matches.Count - 1].Value;
                        }
                        else
                            throw new FileLoadException("Invalid .ply file format: incomplete or incorrectly formatted vertex data");

                        int lastElementIndex = data.LastIndexOf(lastVertexElement);
                        data = data.Substring(lastElementIndex + lastVertexElement.Length);

                        key = @"\d+";
                        regex = new Regex(key);
                        matches = regex.Matches(data);

                        if (matches.Count == faceElementCount * 4)
                        {
                            for (int idx = 0; idx < faceElementCount; idx++)
                            {
                                faceIndices[idx * 3] = uint.Parse(matches[idx * 4 + 1].Value);
                                faceIndices[idx * 3 + 1] = uint.Parse(matches[idx * 4 + 2].Value);
                                faceIndices[idx * 3 + 2] = uint.Parse(matches[idx * 4 + 3].Value);
                            }
                        }
                    }
                    else if (format.Contains("binary"))
                    {
                        byte[] bytes = null;
                        FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                        BinaryReader br = new BinaryReader(fs);
                        bytes = br.ReadBytes((int)(new FileInfo(filePath).Length));
                        byte[] endHeader = Encoding.ASCII.GetBytes(endHeaderKeyword);
                        int indexOfEnd = 0;
                        bool works = false;
                        for(int i=0; i<bytes.Length - endHeader.Length; i++)
                        {
                            works = true;
                            for(int j=0; j<endHeader.Length; j++)
                            {
                                if (bytes[i + j] != endHeader[j])
                                {
                                    works = false;
                                    break;
                                }
                            }
                            if (works)
                            {
                                indexOfEnd = i + endHeader.Length + 1;
                                break;
                            }
                        }

                        Console.WriteLine(header);
                        if (BitConverter.IsLittleEndian)
                            bytes.Reverse();
                        bytes.Reverse();
                        int totalStride = vertexProperties.Sum(p => p.Stride);

                        Console.WriteLine(bytes.Length);
                        Console.WriteLine(vertexElementCount);
                        Console.WriteLine(faceElementCount);

                        float[] parsedValues = new float[vertexProperties.Count];

                        int k = indexOfEnd; //offset in byte array
                        for(int i=0; i<vertexElementCount; i++)
                        {
                            for(int j=0; j<parsedValues.Length; j++)
                            {


                                //Console.WriteLine(bytes[k]);
                                //Console.WriteLine(bytes[k + 1]);
                                //Console.WriteLine(bytes[k + 2]);
                                //Console.WriteLine(bytes[k + 3]);
                                //Console.WriteLine(System.BitConverter.ToSingle(bytes, k));

                                int stride = vertexProperties[j].Stride;
                                float parsedValue = 0.0f;
                                switch (vertexProperties[j].DataType)
                                {
                                    case (PlyProperties.CHAR):
                                        parsedValue = System.BitConverter.ToChar(bytes, k);
                                        k += stride;
                                        break;
                                    case (PlyProperties.UCHAR):
                                        parsedValue = System.BitConverter.ToChar(bytes, k);
                                        k += stride;
                                        break;
                                    case (PlyProperties.SHORT):
                                        parsedValue = System.BitConverter.ToInt16(bytes, k);
                                        k += stride;
                                        break;
                                    case (PlyProperties.USHORT):
                                        parsedValue = System.BitConverter.ToUInt16(bytes, k);
                                        k += stride;
                                        break;
                                    case (PlyProperties.INT):
                                        parsedValue = System.BitConverter.ToInt32(bytes, k);
                                        k += stride;
                                        break;
                                    case (PlyProperties.UINT):
                                        parsedValue = System.BitConverter.ToUInt32(bytes, k);
                                        k += stride;
                                        break;
                                    case (PlyProperties.FLOAT):
                                        parsedValue = System.BitConverter.ToSingle(bytes, k);
                                        k += stride;
                                        break;
                                    case (PlyProperties.DOUBLE):
                                        parsedValue = (float)System.BitConverter.ToDouble(bytes, k);
                                        k += stride;
                                        break;
                                }

                                parsedValues[j] = parsedValue;
                            }
                            var value = vertexProperties.Where((PlyPropertySimple p) => p.DataContent == PlyProperties.X);
                            float x = (value.Count() == 1) ? parsedValues[vertexProperties.IndexOf(value.First())] : 0.0f;

                            value = vertexProperties.Where((PlyPropertySimple p) => p.DataContent == PlyProperties.Y);
                            float y = (value.Count() == 1) ? parsedValues[vertexProperties.IndexOf(value.First())] : 0.0f;

                            value = vertexProperties.Where((PlyPropertySimple p) => p.DataContent == PlyProperties.Z);
                            float z = (value.Count() == 1) ? parsedValues[vertexProperties.IndexOf(value.First())] : 0.0f;



                            vertices[i] = new Vector4(x, y, z, 1);

                            if (hasColor)
                            {
                                value = vertexProperties.Where((PlyPropertySimple p) => p.DataContent == PlyProperties.R);
                                float r = (value.Count() == 1) ? parsedValues[vertexProperties.IndexOf(value.First())] : 0.0f;

                                value = vertexProperties.Where((PlyPropertySimple p) => p.DataContent == PlyProperties.G);
                                float g = (value.Count() == 1) ? parsedValues[vertexProperties.IndexOf(value.First())] : 0.0f;

                                value = vertexProperties.Where((PlyPropertySimple p) => p.DataContent == PlyProperties.B);
                                float b = (value.Count() == 1) ? parsedValues[vertexProperties.IndexOf(value.First())] : 0.0f;

                                colors[i] = new Color4(r, g, b, 1);
                            }
                        }

                        int stride1 = faceList.Stride1;
                        int stride2 = faceList.Stride2;
                        for(int i=0; i<faceElementCount; i++)
                        {
                            //Console.WriteLine(bytes[k]);
                            //Console.WriteLine("{0} {1} {2} {3}", bytes[k + 1], bytes[k + 2], bytes[k + 3], bytes[k + 4]);
                            //Console.WriteLine("{0} {1} {2} {3}", bytes[k + 5], bytes[k + 6], bytes[k + 7], bytes[k + 8]);
                            //Console.WriteLine("{0} {1} {2} {3}", bytes[k + 9], bytes[k + 10], bytes[k + 11], bytes[k + 12]);
                            int count = 3; //we assume that everything is triangles here, not necessarily always the case though
                            int counters = System.BitConverter.ToChar(bytes, k);
                            k += stride1;
                            for (int j=0; j<count; j++)
                            {
                                uint index = 0;
                                switch (faceList.DataType2)
                                {
                                    case (PlyProperties.CHAR):
                                        index = System.BitConverter.ToChar(bytes, k);
                                        k += stride2;
                                        break;
                                    case (PlyProperties.UCHAR):
                                        index = System.BitConverter.ToChar(bytes, k);
                                        k += stride2;
                                        break;
                                    case (PlyProperties.SHORT):
                                        index = (uint)System.BitConverter.ToInt16(bytes, k);
                                        k += stride2;
                                        break;
                                    case (PlyProperties.USHORT):
                                        index = System.BitConverter.ToUInt16(bytes, k);
                                        k += stride2;
                                        break;
                                    case (PlyProperties.INT):
                                        index = (uint)System.BitConverter.ToInt32(bytes, k);
                                        k += stride2;
                                        break;
                                    case (PlyProperties.UINT):
                                        index = System.BitConverter.ToUInt32(bytes, k);
                                        k += stride2;
                                        break;
                                    case (PlyProperties.FLOAT):
                                        index = (uint)System.BitConverter.ToSingle(bytes, k);
                                        k += stride2;
                                        break;
                                    case (PlyProperties.DOUBLE):
                                        index = (uint)System.BitConverter.ToDouble(bytes, k);
                                        k += stride2;
                                        break;
                                }
                                if (index > vertexElementCount)
                                    index = 0;
                                faceIndices[i * 3 + j] = index;
                            }
                        }
                    }
                    model.Vertices = vertices;
                    model.Elements = faceIndices;
                    model.SetReadOnly(true, true);

                    _modelLibrary.Add(Path.GetFileNameWithoutExtension(filePath), model);
                    sw.Stop();
                    Console.WriteLine("Loaded model {0}: {1} vertices, {2} faces\nTime: {3} ms", Path.GetFileNameWithoutExtension(filePath), vertexElementCount, faceElementCount, sw.ElapsedMilliseconds);
                }
            }
            catch(Exception e)
            {
                Console.Error.WriteLine(e.ToString());
            }
        }

        private PlyProperties PlyStringToProperty(string s)
        {
            switch (s)
            {
                case ("uchar"):  return PlyProperties.UCHAR;
                case ("char"):   return PlyProperties.CHAR;
                case ("ushort"): return PlyProperties.USHORT;
                case ("short"):  return PlyProperties.SHORT;
                case ("uint"):   return PlyProperties.UINT;
                case ("int"):    return PlyProperties.INT;
                case ("float"):  return PlyProperties.FLOAT;
                case ("double"): return PlyProperties.DOUBLE;

                case ("x"): return PlyProperties.X;
                case ("y"): return PlyProperties.Y;
                case ("z"): return PlyProperties.Z;

                case ("red"): return PlyProperties.R;
                case ("green"): return PlyProperties.G;
                case ("blue"): return PlyProperties.B;

                case ("vertex_indices"): return PlyProperties.VERTEX_INDEX;
                case ("vertex_index"): return PlyProperties.VERTEX_INDEX;

                default: return PlyProperties.UNKNOWN;
            }
        }

        private int GetStride(PlyProperties p)
        {
            switch (p)
            {
                case (PlyProperties.CHAR): return 1;
                case (PlyProperties.UCHAR): return 1;
                case (PlyProperties.SHORT): return 2;
                case (PlyProperties.USHORT): return 2;
                case (PlyProperties.INT): return 4;
                case (PlyProperties.UINT): return 4;
                case (PlyProperties.FLOAT): return 4;
                case (PlyProperties.DOUBLE): return 8;
                default: return 4;
            }
        }

        public void LoadColladaModel(string filePath)
        {
            try
            {
                string xmlns = "{http://wwww.collada.org/2005/11/COLLADASchema}";
                //Load the xml document from a file
                XDocument modelDoc = XDocument.Load(filePath);

                //DEBUG --- VIEWING ALL DATA
                //foreach (XElement element in modelDoc.Root.Elements())
                //{
                //    Console.WriteLine(element.ToString());
                //}
                //END

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

                    vectors = new Vector3[elementCount];
                    for (int i = 0; i < elementCount; i++)
                    {
                        float.TryParse(data[i * stride + 0], out vectors[i].X);
                        float.TryParse(data[i * stride + 1], out vectors[i].Y);
                        float.TryParse(data[i * stride + 2], out vectors[i].Z);
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
                model.SetReadOnly(false, true);
                _modelLibrary.Add("TestCube", model);

            }
            catch(Exception e)
            {
                Console.WriteLine("Error loading '.DAE' file type at {0}", filePath);
                Console.WriteLine(e.ToString());
                return;
            }
        }

        #endregion
    }
}
