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
        ELEMENTS = 1 << 1,

        NORMALS = 1 << 2,
        COLORS  = 1 << 3,
        UVS     = 1 << 4,

        VERTICES_ELEMENTS        = VERTICES | ELEMENTS,
        VERTICES_COLORS_ELEMENTS = VERTICES_ELEMENTS | COLORS,
        VERTICES_UVS_ELEMENTS    = VERTICES_ELEMENTS | UVS,

        VERTICES_NORMALS          = VERTICES | NORMALS,
        VERTICES_NORMALS_ELEMENTS = VERTICES_NORMALS| ELEMENTS,

        VERTICES_NORMALS_COLORS          = VERTICES_NORMALS | COLORS,
        VERTICES_NORMALS_COLORS_ELEMENTS = VERTICES_NORMALS_COLORS | ELEMENTS,

        VERTICES_NORMALS_UVS          = VERTICES_NORMALS | UVS,
        VERTICES_NORMALS_UVS_ELEMENTS = VERTICES_NORMALS_UVS | ELEMENTS
    }

    [Flags]
    public enum ConstructionFlags
    {
        NONE = 0,

        VECTOR3_OFFSET   = 1,
        FLOAT_ROTATION_X = 1 << 1,
        FLOAT_ROTATION_Y = 1 << 2,
        FLOAT_ROTATION_Z = 1 << 3,
        VECTOR3_SCALE    = 1 << 4,

        VECTOR3_DIMENSIONS = 1 << 5,

        COLOR4_COLOR = 1 << 6,

        VECTOR2_UV_OFFSET  = 1 << 7,
        VECTOR2_UV_SCALE   = 1 << 8
    }

    public enum ModelReferenceType
    {
        DISCRETE = 0,
        TEMPLATE = 1
    }

    public class ComponentModelStatic : IComponent
    {
        public Component BitMaskID => Component.MODEL_STATIC;

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

        private ConstructionFlags _constructionFlags;
        /// <summary>
        /// A bit mask used to indicate which parameters are stored in the byte array.  The order of the parameters corresponds to the order of the enumerated types.
        /// </summary>
        public ConstructionFlags ConstructionFlags { get => _constructionFlags; }

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

        public ComponentModelStatic(ModelProperties modelProperties, ModelReferenceType modelReferenceType, string modelID,
                            ConstructionFlags parameterFlags, params object[] constructionData)
        {
            _modelProperties = modelProperties;
            _modelReferenceType = modelReferenceType;
            _modelID = modelID;
            _constructionFlags = parameterFlags;
            LoadConstructionData(constructionData);

            //_isLoadedIntoVAO = false; //Enum of states instead?
        }

        public ComponentModelStatic(ModelProperties modelProperties, ModelReferenceType modelReferenceType,
            ModelTemplates template, ConstructionFlags parameterFlags,
            params object[] constructionData)
        {
            _modelProperties = modelProperties;
            _modelReferenceType = modelReferenceType;
            _modelID = "";
            _constructionFlags = parameterFlags;
            _templateID = template;
            LoadConstructionData(constructionData);
            
        }

        private void LoadConstructionData(object[] constructionData)
        {
            int totalFloats = 0;
            int objectCount = 0;
            if (_constructionFlags.HasFlag(ConstructionFlags.VECTOR3_OFFSET))
            {
                totalFloats += VECTOR3;
                objectCount++;
            }
            if (_constructionFlags.HasFlag(ConstructionFlags.FLOAT_ROTATION_X))
            {
                totalFloats += FLOAT;
                objectCount++;
            }
            if (_constructionFlags.HasFlag(ConstructionFlags.FLOAT_ROTATION_Y))
            {
                totalFloats += FLOAT;
                objectCount++;
            }
            if (_constructionFlags.HasFlag(ConstructionFlags.FLOAT_ROTATION_Z))
            {
                totalFloats += FLOAT;
                objectCount++;
            }
            if (_constructionFlags.HasFlag(ConstructionFlags.VECTOR3_SCALE))
            {
                totalFloats += VECTOR3;
                objectCount++;
            }
            if (_constructionFlags.HasFlag(ConstructionFlags.VECTOR3_DIMENSIONS))
            {
                totalFloats += VECTOR3;
                objectCount++;
            }
            if (_constructionFlags.HasFlag(ConstructionFlags.COLOR4_COLOR))
            {
                totalFloats += COLOR4;
                objectCount++;
            }
            if (_constructionFlags.HasFlag(ConstructionFlags.VECTOR2_UV_OFFSET))
            {
                totalFloats += VECTOR2;
                objectCount++;
            }
            if (_constructionFlags.HasFlag(ConstructionFlags.VECTOR2_UV_SCALE))
            {
                totalFloats += VECTOR2;
                objectCount++;
            }

            if (objectCount == constructionData.Length)
            {
                _constructionData = new float[totalFloats];
                int offset = 0;
                int objectIndex = 0;
                if (_constructionFlags.HasFlag(ConstructionFlags.VECTOR3_OFFSET))
                {
                    _constructionData[offset] = ((Vector3)constructionData[objectIndex]).X;
                    _constructionData[offset + 1] = ((Vector3)constructionData[objectIndex]).Y;
                    _constructionData[offset + 2] = ((Vector3)constructionData[objectIndex]).Z;
                    objectIndex++;
                    offset += VECTOR3;
                }
                if (_constructionFlags.HasFlag(ConstructionFlags.FLOAT_ROTATION_X))
                {
                    _constructionData[offset] = (float)constructionData[objectIndex];
                    objectIndex++;
                    offset += FLOAT;
                }
                if (_constructionFlags.HasFlag(ConstructionFlags.FLOAT_ROTATION_Y))
                {
                    _constructionData[offset] = (float)constructionData[objectIndex];
                    objectIndex++;
                    offset += FLOAT;
                }
                if (_constructionFlags.HasFlag(ConstructionFlags.FLOAT_ROTATION_Z))
                {
                    _constructionData[offset] = (float)constructionData[objectIndex];
                    objectIndex++;
                    offset += FLOAT;
                }
                if (_constructionFlags.HasFlag(ConstructionFlags.VECTOR3_SCALE))
                {
                    _constructionData[offset] = ((Vector3)constructionData[objectIndex]).X;
                    _constructionData[offset + 1] = ((Vector3)constructionData[objectIndex]).Y;
                    _constructionData[offset + 2] = ((Vector3)constructionData[objectIndex]).Z;
                    objectIndex++;
                    offset += VECTOR3;
                }
                if (_constructionFlags.HasFlag(ConstructionFlags.VECTOR3_DIMENSIONS))
                {
                    _constructionData[offset] = ((Vector3)constructionData[objectIndex]).X;
                    _constructionData[offset + 1] = ((Vector3)constructionData[objectIndex]).Y;
                    _constructionData[offset + 2] = ((Vector3)constructionData[objectIndex]).Z;
                    objectIndex++;
                    offset += VECTOR3;
                }
                if (_constructionFlags.HasFlag(ConstructionFlags.COLOR4_COLOR))
                {
                    _constructionData[offset] = ((Color4)constructionData[objectIndex]).R;
                    _constructionData[offset + 1] = ((Color4)constructionData[objectIndex]).G;
                    _constructionData[offset + 2] = ((Color4)constructionData[objectIndex]).B;
                    _constructionData[offset + 3] = ((Color4)constructionData[objectIndex]).A;
                    objectIndex++;
                    offset += COLOR4;
                }
                if (_constructionFlags.HasFlag(ConstructionFlags.VECTOR2_UV_OFFSET))
                {
                    _constructionData[offset] = ((Vector2)constructionData[objectIndex]).X;
                    _constructionData[offset + 1] = ((Vector2)constructionData[objectIndex]).Y;
                    objectIndex++;
                    offset += VECTOR2;
                }
                if (_constructionFlags.HasFlag(ConstructionFlags.VECTOR2_UV_SCALE))
                {
                    _constructionData[offset] = ((Vector2)constructionData[objectIndex]).X;
                    _constructionData[offset + 1] = ((Vector2)constructionData[objectIndex]).Y;
                    objectIndex++;
                    offset += VECTOR2;
                }
            }
            else
            {
                throw new Exception("The construction data passed does not match the ParameterFlags. Invalid ComponentModel construction.");
            }
        }

        /// <summary>
        /// Returns an array containing the data associated with the specified ConstructionFlag
        /// </summary>
        /// <param name="flag"></param>
        /// <returns></returns>
        public float[] GetConstructionParameter(ConstructionFlags flag) //needs error checking or catching mechanism
        {
            int offset = ParameterOffset(flag);
            int size = ParameterSize(flag);

            float[] data = new float[size];

            for (int idx = 0; idx < size; idx++)
                data[idx] = _constructionData[idx + offset];

            return data;
        }

        public void GetConstructionParameter(ConstructionFlags flag, out float[] data) //needs error checking or catching mechanism
        {
            int offset = ParameterOffset(flag);
            int size = ParameterSize(flag);

            data = new float[size];

            for (int idx = 0; idx < size; idx++)
                data[idx] = _constructionData[idx + offset];
        }

        public int ParameterOffset(ConstructionFlags flag)
        {
            int offset = 0;

            if(flag == ConstructionFlags.VECTOR3_OFFSET) { return offset; }
            offset += (_constructionFlags.HasFlag(ConstructionFlags.VECTOR3_OFFSET)) ? VECTOR3 : 0;

            if (flag == ConstructionFlags.FLOAT_ROTATION_X) { return offset; }
            offset += (_constructionFlags.HasFlag(ConstructionFlags.FLOAT_ROTATION_X)) ? FLOAT : 0;

            if (flag == ConstructionFlags.FLOAT_ROTATION_Y) { return offset; }
            offset += (_constructionFlags.HasFlag(ConstructionFlags.FLOAT_ROTATION_Y)) ? FLOAT : 0;

            if (flag == ConstructionFlags.FLOAT_ROTATION_Z) { return offset; }
            offset += (_constructionFlags.HasFlag(ConstructionFlags.FLOAT_ROTATION_Z)) ? FLOAT : 0;

            if (flag == ConstructionFlags.VECTOR3_SCALE) { return offset; }
            offset += (_constructionFlags.HasFlag(ConstructionFlags.VECTOR3_SCALE)) ? VECTOR3 : 0;

            if (flag == ConstructionFlags.VECTOR3_DIMENSIONS) { return offset; }
            offset += (_constructionFlags.HasFlag(ConstructionFlags.VECTOR3_DIMENSIONS)) ? VECTOR3 : 0;

            if (flag == ConstructionFlags.COLOR4_COLOR) { return offset; }
            offset += (_constructionFlags.HasFlag(ConstructionFlags.COLOR4_COLOR)) ? COLOR4: 0;

            if (flag == ConstructionFlags.VECTOR2_UV_OFFSET) { return offset; }
            offset += (_constructionFlags.HasFlag(ConstructionFlags.VECTOR2_UV_SCALE)) ? VECTOR2 : 0;

            if (flag == ConstructionFlags.VECTOR2_UV_SCALE) { return offset; }
            offset += (_constructionFlags.HasFlag(ConstructionFlags.VECTOR2_UV_SCALE)) ? VECTOR2: 0;

            return offset; //Don't need to check since this is the last parameter, for now
        }

        public int ParameterSize(ConstructionFlags flag)
        {
            switch (flag)
            {
                case (ConstructionFlags.VECTOR3_OFFSET):
                    return VECTOR3;
                case (ConstructionFlags.FLOAT_ROTATION_X):
                    return FLOAT;
                case (ConstructionFlags.FLOAT_ROTATION_Y):
                    return FLOAT;
                case (ConstructionFlags.FLOAT_ROTATION_Z):
                    return FLOAT;
                case (ConstructionFlags.VECTOR3_SCALE):
                    return VECTOR3;
                case (ConstructionFlags.VECTOR3_DIMENSIONS):
                    return VECTOR3;
                case (ConstructionFlags.COLOR4_COLOR):
                    return COLOR4;
                case (ConstructionFlags.VECTOR2_UV_OFFSET):
                    return VECTOR2;
                case (ConstructionFlags.VECTOR2_UV_SCALE):
                    return VECTOR2;
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
}
