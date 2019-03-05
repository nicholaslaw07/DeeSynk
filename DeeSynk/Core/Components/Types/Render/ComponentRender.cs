using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;

namespace DeeSynk.Core.Components.Types.Render
{
    public class ComponentRender : IComponent
    {
        public int BitMaskID => (int)Component.RENDER;

        private int _vaoID;
        /// <summary>
        /// The ID of the VAO that this object should be rendered with.
        /// </summary>
        public int VAO_ID { get => _vaoID; }

        private int _iboID;
        /// <summary>
        /// The ID of the IBO that this object should be rendered with.
        /// </summary>
        public int IBO_ID { get => _iboID; }

        private int _programID;
        /// <summary>
        /// The ID of the shader that this object should be rendered with.
        /// </summary>
        public int PROGRAM_ID { get => _programID; }

        //Render Layer
        //Render method (2D or 3D)
        //Is simple sprite?  Idk
        //Has unique VAO
        //Position in vao if not unique
        //Must also store cbo id somewhere

        public ComponentRender(int vaoID, int iboID, int programID)
        {
            _vaoID = vaoID;
            _iboID = iboID;
            _programID = programID;
        }

        public void BindData()
        {
            GL.BindVertexArray(_vaoID);  //binds this object's VAO
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _iboID);  //binds this object's elements buffer

            int data = 0;
            GL.GetInteger(GetPName.CurrentProgram, out data);  //gets the currently active program id
            if (data != _programID)  //if program id == the program that is to be used by this object then continue
                GL.UseProgram(_programID); //use the program as determined by this class


        }

        public void Update(float time)
        {
            //throw new NotImplementedException();
        }
    }
}
