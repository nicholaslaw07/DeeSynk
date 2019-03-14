using DeeSynk.Core.Components.Models;
using DeeSynk.Core.Managers;
using DeeSynk.Core.Systems;
using OpenTK;
using OpenTK.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DeeSynk.Core.Components.Types.Render
{
    [Flags]
    public enum ModelProperties
    {
        VERTICES = 1,
        FACE_ELEMENTS = 1 << 1,

        NORMALS = 1 << 2,
        COLORS  = 1 << 3,
        UVS     = 1 << 4,

        VERTICES_ELEMENTS        = VERTICES | FACE_ELEMENTS,
        VERTICES_COLORS_ELEMENTS = VERTICES_ELEMENTS | COLORS,
        VERTICES_UVS_ELEMENTS    = VERTICES_ELEMENTS | UVS,

        VERTICES_NORMALS          = VERTICES | NORMALS,
        VERTICES_NORMALS_ELEMENTS = VERTICES_NORMALS| FACE_ELEMENTS,

        VERTICES_NORMALS_COLORS          = VERTICES_NORMALS | COLORS,
        VERTICES_NORMALS_COLORS_ELEMENTS = VERTICES_NORMALS_COLORS | FACE_ELEMENTS,

        VERTICES_NORMALS_UVS          = VERTICES_NORMALS | UVS,
        VERTICES_NORMALS_UVS_ELEMENTS = VERTICES_NORMALS_UVS | FACE_ELEMENTS
    }

    public enum ModelReferenceType
    {
        DISCRETE = 0,
        TEMPLATE = 1
    }

    public class ComponentModelStatic : IComponent //, ISerializable
    {
        public int BitMaskID => (int)Component.MODEL_STATIC;

        public const int FLOAT   = 1;
        public const int VECTOR2 = 2;
        public const int VECTOR3 = 3;
        public const int VECTOR4 = 4;
        public const int COLOR3  = 3;
        public const int COLOR4  = 4;

        private ModelProperties _modelProperties;
        /// <summary>
        /// Indicates which types of data this model contains.
        /// </summary>
        public ModelProperties ModelProperties { get => _modelProperties; }

        private ModelReferenceType _modelReferenceType;
        /// <summary>
        /// Indicates whether the Model ID for this ComponentModel points to either a preloaded model or template class inside of ModelManager.
        /// </summary>
        public ModelReferenceType ModelReferenceType { get => _modelReferenceType; }

        private string _modelID;
        /// <summary>
        /// The string ID used by model manager to reference a specific model or template.
        /// </summary>
        public string ModelID { get => _modelID; set => _modelID = value; }

        private ConstructionParameterFlags _parameterFlags;
        /// <summary>
        /// A bit mask used to indicate which parameters are stored in the byte array.  The order of the parameters corresponds to the order of the enumerated types.
        /// </summary>
        public ConstructionParameterFlags ParameterFlags { get => _parameterFlags; }

        private float[] _constructionData;
        /// <summary>
        /// The set of parameters, in a byte array, used to manipulate or build the model before uploading it to GPU memory.
        /// </summary>
        public ref float[] ConstructionData { get => ref _constructionData; }

        private ModelTemplates _templateID;
        public  ModelTemplates TemplateID
        {
            get
            {
                if (ModelReferenceType == ModelReferenceType.TEMPLATE)
                    return _templateID;
                return ModelTemplates.NONE;
            }
            set
            {
                if(ModelReferenceType == ModelReferenceType.TEMPLATE)
                {
                    _templateID = value;
                }
            }
        }

        //+++
        /*
        private bool _isLoadedIntoVAO;
        /// <summary>
        /// Whether or not the model data for this ComponentModel has been loaded into a VAO yet.
        /// </summary>
        public bool IsLoadedIntoVAO { get => _isLoadedIntoVAO; }

        private int[] _bufferIDs;
        /// <summary>
        /// List of buffers used to store this model's data.
        /// </summary>
        public int[] BufferIDs { get => _bufferIDs; }

        private Buffers _bufferFlags;
        /// <summary>
        /// A bit mask used to indicate which buffers the BufferIDs property holds.  Their order corresponds to the order of the enumerated types.
        /// </summary>
        public Buffers BufferFlags { get => _bufferFlags; }

        private int[] _baseBufferIndices;
        /// <summary>
        /// An array of the offset indices that point to the data in each of the buffers used by this model.
        /// </summary>
        public int[] BaseBufferIndices { get => _baseBufferIndices; }

        //private int[] _lengthsInMemory;
        /// <summary>
        /// An array of the number of the indices that the data in each of the buffers used by this model occupies (sequentially, of course).
        /// </summary>
        //public int[] LengthsInMemory { get => _lengthsInMemory; }
        */

        public ComponentModelStatic(ModelProperties modelProperties, ModelReferenceType modelReferenceType, string modelID,
                            ConstructionParameterFlags parameterFlags, params object[] constructionData)
        {
            _modelProperties = modelProperties;
            _modelReferenceType = modelReferenceType;
            _modelID = modelID;
            _parameterFlags = parameterFlags;
            LoadConstructionData(constructionData);

            //_isLoadedIntoVAO = false; //Enum of states instead?
        }

        public ComponentModelStatic(ModelProperties modelProperties, ModelReferenceType modelReferenceType,
                            ConstructionParameterFlags parameterFlags, ModelTemplates template,
                            params object[] constructionData)
        {
            _modelProperties = modelProperties;
            _modelReferenceType = modelReferenceType;
            _modelID = "";
            _parameterFlags = parameterFlags;
            _templateID = template;
            LoadConstructionData(constructionData);
            
        }

        private void LoadConstructionData(object[] constructionData)
        {
            int totalFloats = 0;
            int objectCount = 0;
            if (_parameterFlags.HasFlag(ConstructionParameterFlags.VECTOR3_OFFSET))
            {
                totalFloats += VECTOR3;
                objectCount++;
            }
            if (_parameterFlags.HasFlag(ConstructionParameterFlags.FLOAT_ROTATION_X))
            {
                totalFloats += FLOAT;
                objectCount++;
            }
            if (_parameterFlags.HasFlag(ConstructionParameterFlags.FLOAT_ROTATION_Y))
            {
                totalFloats += FLOAT;
                objectCount++;
            }
            if (_parameterFlags.HasFlag(ConstructionParameterFlags.FLOAT_ROTATION_Z))
            {
                totalFloats += FLOAT;
                objectCount++;
            }
            if (_parameterFlags.HasFlag(ConstructionParameterFlags.VECTOR3_SCALE))
            {
                totalFloats += VECTOR3;
                objectCount++;
            }
            if (_parameterFlags.HasFlag(ConstructionParameterFlags.VECTOR2_DIMENSIONS))
            {
                totalFloats += VECTOR2;
                objectCount++;
            }
            if (_parameterFlags.HasFlag(ConstructionParameterFlags.COLOR4_COLOR))
            {
                totalFloats += COLOR4;
                objectCount++;
            }

            if (objectCount == constructionData.Length)
            {
                _constructionData = new float[totalFloats];
                int offset = 0;
                int objectIndex = 0;
                if (_parameterFlags.HasFlag(ConstructionParameterFlags.VECTOR3_OFFSET))
                {
                    _constructionData[offset] = ((Vector3)constructionData[objectIndex]).X;
                    _constructionData[offset + 1] = ((Vector3)constructionData[objectIndex]).Y;
                    _constructionData[offset + 2] = ((Vector3)constructionData[objectIndex]).Z;
                    objectIndex++;
                    offset += VECTOR3;
                }
                if (_parameterFlags.HasFlag(ConstructionParameterFlags.FLOAT_ROTATION_X))
                {
                    _constructionData[offset] = (float)constructionData[objectIndex];
                    objectIndex++;
                    offset += FLOAT;
                }
                if (_parameterFlags.HasFlag(ConstructionParameterFlags.FLOAT_ROTATION_Y))
                {
                    _constructionData[offset] = (float)constructionData[objectIndex];
                    objectIndex++;
                    offset += FLOAT;
                }
                if (_parameterFlags.HasFlag(ConstructionParameterFlags.FLOAT_ROTATION_Z))
                {
                    _constructionData[offset] = (float)constructionData[objectIndex];
                    objectIndex++;
                    offset += FLOAT;
                }
                if (_parameterFlags.HasFlag(ConstructionParameterFlags.VECTOR3_SCALE))
                {
                    _constructionData[offset] = ((Vector3)constructionData[objectIndex]).X;
                    _constructionData[offset + 1] = ((Vector3)constructionData[objectIndex]).Y;
                    _constructionData[offset + 2] = ((Vector3)constructionData[objectIndex]).Z;
                    objectIndex++;
                    offset += VECTOR3;
                }
                if (_parameterFlags.HasFlag(ConstructionParameterFlags.VECTOR2_DIMENSIONS))
                {
                    _constructionData[offset] = ((Vector2)constructionData[objectIndex]).X;
                    _constructionData[offset + 1] = ((Vector2)constructionData[objectIndex]).Y;
                    objectIndex++;
                    offset += VECTOR2;
                }
                if (_parameterFlags.HasFlag(ConstructionParameterFlags.COLOR4_COLOR))
                {
                    _constructionData[offset] = ((Color4)constructionData[objectIndex]).R;
                    _constructionData[offset + 1] = ((Color4)constructionData[objectIndex]).G;
                    _constructionData[offset + 2] = ((Color4)constructionData[objectIndex]).B;
                    _constructionData[offset + 3] = ((Color4)constructionData[objectIndex]).A;
                    objectIndex++;
                    offset += COLOR4;
                }
            }
            else
            {
                throw new Exception("The construction data passed does not match the ParameterFlags. Invalid ComponentModel construction.");
            }
        }

        public float[] GetConstructionParameter(ConstructionParameterFlags flag)
        {
            int offset = ParameterOffset(flag);
            int size = ParameterSize(flag);

            float[] data = new float[size];

            for (int idx = 0; idx < size; idx++)
                data[idx] = _constructionData[idx + offset];

            return data;
        }

        public int ParameterOffset(ConstructionParameterFlags flag)
        {
            int offset = 0;

            if(flag == ConstructionParameterFlags.VECTOR3_OFFSET) { return offset; }
            offset += (_parameterFlags.HasFlag(ConstructionParameterFlags.VECTOR3_OFFSET)) ? VECTOR3 : 0;

            if (flag == ConstructionParameterFlags.FLOAT_ROTATION_X) { return offset; }
            offset += (_parameterFlags.HasFlag(ConstructionParameterFlags.FLOAT_ROTATION_X)) ? FLOAT : 0;

            if (flag == ConstructionParameterFlags.FLOAT_ROTATION_Y) { return offset; }
            offset += (_parameterFlags.HasFlag(ConstructionParameterFlags.FLOAT_ROTATION_Y)) ? FLOAT : 0;

            if (flag == ConstructionParameterFlags.FLOAT_ROTATION_Z) { return offset; }
            offset += (_parameterFlags.HasFlag(ConstructionParameterFlags.FLOAT_ROTATION_Z)) ? FLOAT : 0;

            if (flag == ConstructionParameterFlags.VECTOR3_SCALE) { return offset; }
            offset += (_parameterFlags.HasFlag(ConstructionParameterFlags.VECTOR3_SCALE)) ? VECTOR3 : 0;

            if (flag == ConstructionParameterFlags.VECTOR2_DIMENSIONS) { return offset; }
            offset += (_parameterFlags.HasFlag(ConstructionParameterFlags.VECTOR2_DIMENSIONS)) ? VECTOR2 : 0;

            //if (flag == ConstructionParameterFlags.COLOR4_COLOR) { return offset; }
            //offset += (_parameterFlags.HasFlag(ConstructionParameterFlags.COLOR4_COLOR)) ? COLOR4: 0;  Don't need to check since this is the last parameter, for now

            return offset;
        }

        public int ParameterSize(ConstructionParameterFlags flag)
        {
            switch (flag)
            {
                case (ConstructionParameterFlags.VECTOR3_OFFSET):
                    return VECTOR3;
                case (ConstructionParameterFlags.FLOAT_ROTATION_X):
                    return FLOAT;
                case (ConstructionParameterFlags.FLOAT_ROTATION_Y):
                    return FLOAT;
                case (ConstructionParameterFlags.FLOAT_ROTATION_Z):
                    return FLOAT;
                case (ConstructionParameterFlags.VECTOR3_SCALE):
                    return VECTOR3;
                case (ConstructionParameterFlags.VECTOR2_DIMENSIONS):
                    return VECTOR2;
                case (ConstructionParameterFlags.COLOR4_COLOR):
                    return COLOR4;
                default:
                    return 0;
            }
        }
        /*
        //Most of this data is purely for debugging, or model modification
        public void PushVAOBufferProperties(int[] bufferIDs, Buffers bufferFlags, int[] baseBufferIndices)
        {
            if (!_isLoadedIntoVAO)
            {
                _bufferIDs = bufferIDs;
                _bufferFlags = bufferFlags;
                _baseBufferIndices = baseBufferIndices;
                //_lengthsInMemory = lengthsInMemory;

                _isLoadedIntoVAO = true;
            }
            else
            {
                Console.WriteLine("Warning: Cannot push buffer properties on a static model since VAO data should also be static");
            }
        }

        //This is a test method because I am lazy
        public void PushVAOBufferProperties(int[] bufferIDs, Buffers bufferFlags)
        {
            if (!_isLoadedIntoVAO)
            {
                _bufferIDs = bufferIDs;
                _bufferFlags = bufferFlags;
                //_baseBufferIndices = baseBufferIndices;
                //_lengthsInMemory = lengthsInMemory;

                _isLoadedIntoVAO = true;
            }
            else
            {
                Console.WriteLine("Warning: Cannot push buffer properties on a static model since VAO data should also be static");
            }
        }
        */
        /*
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            throw new NotImplementedException();
        }
        */
    }

    public class ComponentModelDyanmic : IComponent
    {
        public int BitMaskID => (int)Component.MODEL_DYNAMIC;


        public void Update(float time)
        {
            //throw new NotImplementedException();
        }
    }
}
