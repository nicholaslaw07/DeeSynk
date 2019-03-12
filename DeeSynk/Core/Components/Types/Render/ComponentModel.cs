using DeeSynk.Core.Managers;
using DeeSynk.Core.Systems;
using OpenTK;
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
        public string ModelID { get => _modelID; }

        private byte[] _constructionData;
        /// <summary>
        /// The set of parameters, in a byte array, used to manipulate or build the model before uploading it to GPU memory.
        /// </summary>
        public ref byte[] ConstructionData { get => ref _constructionData; }

        private ConstructionParameterFlags _parameterFlags;
        /// <summary>
        /// A bit mask used to indicate which parameters are stored in the byte array.  The order of the parameters corresponds to the order of the enumerated types.
        /// </summary>
        public ConstructionParameterFlags ParameterFlags { get => _parameterFlags; }

        //+++

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

        private int[] _lengthsInMemory;
        /// <summary>
        /// An array of the number of the indices that the data in each of the buffers used by this model occupies (sequentially, of course).
        /// </summary>
        public int[] LengthsInMemory { get => _lengthsInMemory; }

        //ADD ENUMERATED TYPES FOR ALL OF THE DIFFERENT FLAGS

        public ComponentModelStatic(ModelProperties modelProperties, ModelReferenceType modelReferenceType, string modelID,
                                    byte[] constructionData, ConstructionParameterFlags parameterFlags)
        {
            _modelProperties = ModelProperties;
            _modelReferenceType = modelReferenceType;
            _modelID = modelID;
            _constructionData = constructionData;
            _parameterFlags = parameterFlags;

            _isLoadedIntoVAO = false; //Enum of states instead?
        }

        //Most of this data is purely for debugging, or model modification
        public void PushVAOBufferProperties(int[] bufferIDs, Buffers bufferFlags, int[] baseBufferIndices, int[] lengthsInMemory)
        {
            if (!_isLoadedIntoVAO)
            {
                _bufferIDs = bufferIDs;
                _bufferFlags = bufferFlags;
                _baseBufferIndices = baseBufferIndices;
                _lengthsInMemory = lengthsInMemory;

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
