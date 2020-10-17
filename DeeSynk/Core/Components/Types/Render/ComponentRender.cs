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

        private VAO _vao;
        public VAO VAO
        {
            get => _vao;
            set => _vao = value;
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


        private int _indexCount;
        public int IndexCount { get => _indexCount; }

        private bool _isFinalRenderPlane;
        public bool IsFinalRenderPlane { get => _isFinalRenderPlane; set => _isFinalRenderPlane = value; }

        //Render Layer
        //Render method (2D or 3D)
        //Is simple sprite?  Idk
        //Has unique VAO
        //Position in vao if not unique
        //Must also store cbo id somewhere

        public ComponentRender(Buffers bufferFlags)
        {
            _bufferFlags = bufferFlags;

            _init = false;
        }

        public void AddVAOData(int vaoID, int iboID, int programID)
        {
            PROGRAM_ID = programID;
        }

        public void AddBufferData()
        {
            PROGRAM_ID = GL.GetInteger(GetPName.CurrentProgram);

            GL.GetBufferParameter(BufferTarget.ElementArrayBuffer, BufferParameterName.BufferSize, out int size);
            _indexCount = size / 4;
        }

        /// <summary>
        /// Used to ensure that all of the data stored inside of here is indeed valid data that can be used without issue.
        /// </summary>
        /// <returns></returns>
        public bool ValidateData()
        {
            if(GL.IsVertexArray(_vao.Id) && GL.IsProgram(_programID)) //(_bufferFlags.HasFlag(Buffers.COLORS) ^ _bufferFlags.HasFlag(Buffers.UVS))
            {
                if(_bufferFlags.HasFlag(Buffers.FACE_ELEMENTS))
                {
                    if(GL.IsBuffer(_vao.Buffers[VAO.IndexData]))
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
                _vao.Bind(); //binds the buffers associated with this object if they aren't already bound
                GL.GetInteger(GetPName.CurrentProgram, out int data);  //gets the currently active program id
                if (data != _programID)
                    GL.UseProgram(_programID); //binds this objects program if it isn't currently bound
            }
        }

        public void BindDataNoShader()
        {
            if (_init)
            {
                _vao.Bind();
            }
        }

        public void Update(float time)
        {
            throw new NotImplementedException();
        }
    }
}
