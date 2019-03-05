using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeeSynk.Core.Components.Types.Render
{
    public class ComponentRender : IComponent
    {
        public int BitMaskID => throw new NotImplementedException();

        private int _vaoID;
        /// <summary>
        /// The ID of the VAO that this object should be rendered with.
        /// </summary>
        public int VAO_ID { get => _vaoID; }

        private int _shaderID;
        /// <summary>
        /// The ID of the shader that this object should be rendered with.
        /// </summary>
        public int SHADER_ID { get => _shaderID; }



        public void Update(float time)
        {
            throw new NotImplementedException();
        }
    }
}
