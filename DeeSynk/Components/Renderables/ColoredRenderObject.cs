using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace DeeSynk.Components.Renderables
{
    public abstract class ColoredRenderObject : RenderObject
    {
        private bool _initVAO; // is the VAO initialized
        protected bool InitVAO { get => _initVAO; set => _initVAO = value; }  //inside of the get, add a means of checking if the buffer references have been created

        private int _VAO;  //Vertex Array Object integer reference
        private int _VBO;  //Vertex Buffer Object
        private int _IBO;  //Index Buffer Object

        protected int VAO { get => _VAO; }
        protected int VBO { get => _VBO; }
        protected int IBO { get => _IBO; }

        private ColoredVertex[] _vertices;
        private int _vertexCount;

        private int[] _indices;
        private int _indexCount;

        public ColoredRenderObject(int renderID, int renderLayer) : base(renderID, renderLayer)
        {
            _initVAO = false;
        }

        public ColoredRenderObject(int renderID, int renderLayer, Vector3 position, float rotX, float rotY, float rotZ, Vector3 scale) : base(renderID, renderLayer, position, rotX, rotY, rotZ, scale)
        {
            _initVAO = false;
        }
    }
}
