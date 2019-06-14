using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics;

namespace DeeSynk.Core.Components.GraphicsObjects.Lights
{
    public abstract class Light : IUBO
    {
        public static readonly Color4 DEFAULT_EMISSION_COLOR = Color4.White;

        protected Color4 _emissionColor;
        public abstract Color4 EmissionColor { get; set; }

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

        //This is temporary stuff.  There  will be a new graphics object that will gauge the shadow method which is where all stuff regarding depth maps (or other) will be stored.
        public abstract bool HasShadowMap { get; }

        public abstract int MapResolutionX { get; }
        public abstract int MapResolutionY { get; }

        protected int _fbo, _depthMap;
        public int FBO => _fbo;
        public int DepthMap => _depthMap;

        public abstract void BuildUBO(int bindingLocation, int numOfVec4s);
        public abstract void AttachUBO(int bindingLocation);
        public abstract void DetatchUBO();
        public abstract void UpdateUBO();
        public abstract void FillBuffer();

        public abstract void AddShadowMap(int width, int height);
        public abstract void BindShadowMap();
        public abstract void UnbindShadowMap();
        #endregion
    }
}
