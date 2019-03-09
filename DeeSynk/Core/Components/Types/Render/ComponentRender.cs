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
        public int BitMaskID => (int)Component.RENDER;

        private int _vaoBitMask;
        public int VAOBitMask { get => _vaoBitMask; }

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
                if((_vaoBitMask & (int)VAOTypes.Indexed) != 0)
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

        private int _objectCount;
        /// <summary>
        /// The number of objects that this VAO contains (not necessarily the number of triangles).
        /// </summary>
        public int OBJECT_COUNT
        {
            get => _objectCount;
            set
            {
                if(value >= 0)
                {
                    _objectCount = value;
                }
                else
                {
                    _objectCount = 0;
                }
            }
        }

        //Render Layer
        //Render method (2D or 3D)
        //Is simple sprite?  Idk
        //Has unique VAO
        //Position in vao if not unique
        //Must also store cbo id somewhere

        public ComponentRender(int vaoBitMask)
        {
            _vaoBitMask = vaoBitMask;

            _vaoID = 0;
            _iboID = 0;
            _programID = 0;

            _init = false;
        }

        public void AddVAOData(int vaoID, int iboID, int programID, int objectCount)
        {
            VAO_ID = vaoID;
            IBO_ID = iboID;
            PROGRAM_ID = programID;
            OBJECT_COUNT = objectCount;
        }

        /// <summary>
        /// Used to ensure that all of the data stored inside of here is indeed valid data that can be used without issue.
        /// </summary>
        /// <returns></returns>
        public bool ValidateData()
        {
            if(GL.IsVertexArray(_vaoID) && 
               GL.IsProgram(_programID) &&
               (_vaoBitMask & (int)VAOTypes.Colored) != (_vaoBitMask & (int)VAOTypes.Textured))
            {
                if((_vaoBitMask & (int)VAOTypes.Indexed) != 0)
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
                if ((VAOBitMask & (int)VAOTypes.Instanced) != 0)
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
