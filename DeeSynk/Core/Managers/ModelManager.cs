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
            throw new NotImplementedException();
        }

        #endregion

        #region Parsers and File Loading

        public void LoadColladaModel(string filePath)
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
                _modelLibrary.Add("TestCube", model);

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
    }
}
