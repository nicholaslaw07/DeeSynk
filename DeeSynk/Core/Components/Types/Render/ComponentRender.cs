using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DeeSynk.Core.Systems;
using OpenTK.Graphics.OpenGL4;

namespace DeeSynk.Core.Components.Types.Render
{
    public class ComponentRender : IComponent
    {
        public Component BitMaskID => Component.RENDER;

        private bool _init;
        public bool Initialized { get => _init; }

        private int _vaoID;
        /// <summary>
        /// The ID of the VAO that this object should be rendered with.
        /// </summary>
        public int VAO_ID {
            get => _vaoID;
            set
            {
                if (GL.IsVertexArray(value))
                    _vaoID = value;
                else
                    _vaoID = 0;
            }
        }

        private int _iboID;
        /// <summary>
        /// The ID of the IBO that this object should be rendered with.
        /// </summary>
        public int IBO_ID {
            get => _iboID;
            set
            {
                if (_bufferFlags.HasFlag(Buffers.FACE_ELEMENTS))
                {
                    if (GL.IsBuffer(value))
                        _iboID = value;
                    else
                        _iboID = 0;
                }
            }
        }

        private int _programID;
        /// <summary>
        /// The ID of the shader that this object should be rendered with.
        /// </summary>
        public int PROGRAM_ID {
            get => _programID;
            set
            {
                if (GL.IsProgram(value))
                    _programID = value;
                else
                    _programID = 0;
            }
        }

        private const int BUFF_MIN_MAG = 0;
        private const int BUFF_MAX_MAG = 5;

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

        //Render Layer
        //Render method (2D or 3D)
        //Is simple sprite?  Idk
        //Has unique VAO
        //Position in vao if not unique
        //Must also store cbo id somewhere

        private int _bufferCount;
        private int BufferCount
        {
            get
            {
                if(_bufferFlags != Buffers.NONE && _bufferCount == 0)
                {
                    int count = 0;
                    for (int i = BUFF_MIN_MAG; i <= BUFF_MAX_MAG; i++)
                        count += (1 << i & (int)_bufferFlags) >> i;
                    _bufferCount = count;
                }

                return _bufferCount;
            }
        }

        public ComponentRender(Buffers bufferFlags)
        {
            _bufferFlags = bufferFlags;

            _bufferIDs = new int[BufferCount];

            _init = false;
        }

        public void AddVAOData(int vaoID, int iboID, int programID)
        {
            VAO_ID = vaoID;
            IBO_ID = iboID;
            PROGRAM_ID = programID;
        }

        public void AddBufferData()
        {
            VAO_ID = GL.GetInteger(GetPName.VertexArrayBinding);
            IBO_ID = GL.GetInteger(GetPName.ElementArrayBufferBinding);
            PROGRAM_ID = GL.GetInteger(GetPName.CurrentProgram);
        }

        /// <summary>
        /// Used to ensure that all of the data stored inside of here is indeed valid data that can be used without issue.
        /// </summary>
        /// <returns></returns>
        public bool ValidateData()
        {
            if(GL.IsVertexArray(_vaoID) && GL.IsProgram(_programID)) //(_bufferFlags.HasFlag(Buffers.COLORS) ^ _bufferFlags.HasFlag(Buffers.UVS))
            {
                if(_bufferFlags.HasFlag(Buffers.FACE_ELEMENTS))
                {
                    if(GL.IsBuffer(_iboID))
                    {
                        _init = true;
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    _init = true;
                    return true;
                }
            }

            return false;
        }

        public void BindData()
        {
            if (_init)
            {
                int data = 0;
                GL.GetInteger(GetPName.VertexArray, out data);
                if (data != _vaoID)
                    GL.BindVertexArray(_vaoID);
                GL.BindVertexArray(_vaoID);  //binds this object's VAO
                if (_bufferFlags.HasFlag(Buffers.FACE_ELEMENTS))
                {
                    data = 0;
                    GL.GetInteger(GetPName.ElementArrayBufferBinding, out data);
                    if(data != _iboID)
                        GL.BindBuffer(BufferTarget.ElementArrayBuffer, _iboID);
                } //binds this object's elements buffer

                data = 0;
                GL.GetInteger(GetPName.CurrentProgram, out data);  //gets the currently active program id
                if (data != _programID)  //if program id == the program that is to be used by this object then continue
                    GL.UseProgram(_programID); //use the program as determined by this class
            }
        }

        public void BindDataTest()
        {
            GL.BindVertexArray(_vaoID);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _iboID);
            GL.UseProgram(_programID);
        }

        public void Update(float time)
        {
            throw new NotImplementedException();
        }
    }
}
