using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DeeSynk.Core.Components.GraphicsObjects.Shadows;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;

namespace DeeSynk.Core.Components.GraphicsObjects.Lights
{
    public abstract class Light : IUBO
    {
        public static readonly Color4 DEFAULT_EMISSION_COLOR = Color4.White;

        protected Color4 _emissionColor;
        public abstract Color4 EmissionColor { get; set; }

        #region Shadow Mapping
        protected ShadowMap _shadowMap;
        public ShadowMap ShadowMap { get => _shadowMap; set => _shadowMap = value; }
        public bool HasShadowMap => _shadowMap != null && _shadowMap.Init;
        #endregion

        #region UBO
        protected bool _initUBO;
        public bool InitUBO => _initUBO;
        protected int _ubo_Id;
        public int UBO_Id => _ubo_Id;
        protected int _bindingLocation;
        public int BindingLocation => _bindingLocation;
        protected Vector4[] _bufferData;
        public Vector4[] BufferData => _bufferData;
        public abstract int BufferSize { get; }
        protected int _bufferOffset;
        public int BufferOffset { get => _bufferOffset; }

        public abstract void BuildUBO(int bindingLocation, int numOfVec4s);
        public abstract void BuildUBO(int uboId, int uboSize, int offset, int bindingLocation, int numOfVec4s);
        public abstract void AttachUBO(int bindingLocation);
        public abstract void DetatchUBO();
        public abstract void UpdateUBO();
        public abstract void FillBuffer();
        #endregion
    }
}
