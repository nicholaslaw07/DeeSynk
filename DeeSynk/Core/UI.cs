using DeeSynk.Core.Components;
using DeeSynk.Core.Components.GraphicsObjects;
using DeeSynk.Core.Components.Types.Render;
using DeeSynk.Core.Components.Types.Transform;
using DeeSynk.Core.Components.Types.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeeSynk.Core
{
    /// <summary>
    /// Acts as a container for components related to the UI.  This is entirely distinct from World in that render components stored here are not visible to World and vice-versa.
    /// </summary>
    public class UI : GameObjectContainer
    {
        private const uint FBO_COUNT = 4;
        /// <summary>
        /// Total number of FBO objects available.
        /// </summary>
        public uint FBO_Count { get => FBO_COUNT; }

        //CURRENTLY NO LIGHT COMPS, NO OBVIOUS PURPOSE YET
        #region COMPONENT_ARRAYS
        private ComponentTransform[] _transComps;
        public ComponentTransform[] TransComps { get => _transComps; }
        private ComponentRender[] _renderComps;
        public ComponentRender[] RenderComps { get => _renderComps; }
        private ComponentModelStatic[] _staticModelComps;
        public ComponentModelStatic[] StaticModelComps { get => _staticModelComps; }
        private ComponentTexture[] _textureComps;
        public ComponentTexture[] TextureComps { get => _textureComps; }
        private ComponentCamera[] _cameraComps;
        public ComponentCamera[] CameraComps { get => _cameraComps; }
        private ComponentLight[] _lightComps;
        public ComponentLight[] LightComps { get => _lightComps; }
        private ComponentCanvas[] _canvasComps;
        public ComponentCanvas[] CanvasComps { get => _canvasComps; }
        private ComponentElement[] _elementComps;
        public ComponentElement[] ElementComps { get => _elementComps; }
        private ComponentText[] _textComps;
        public ComponentText[] TextComps { get => _textComps; }
        #endregion

        private FBO[] _fbos;
        /// <summary>
        /// Array of frame buffer objects.
        /// </summary>
        public FBO[] FBOs { get => _fbos; }

        public UI(uint objectMemory) : base(objectMemory)
        {
            _existingGameObjects = new bool[OBJECT_MEMORY];
            _gameObjects = new GameObject[OBJECT_MEMORY];

            _transComps = new ComponentTransform[OBJECT_MEMORY];
            _renderComps = new ComponentRender[OBJECT_MEMORY];
            _staticModelComps = new ComponentModelStatic[OBJECT_MEMORY];
            _textureComps = new ComponentTexture[OBJECT_MEMORY];
            _cameraComps = new ComponentCamera[OBJECT_MEMORY];
            _canvasComps = new ComponentCanvas[OBJECT_MEMORY];
            _elementComps = new ComponentElement[OBJECT_MEMORY];
            _textComps = new ComponentText[OBJECT_MEMORY];

            _fbos = new FBO[FBO_COUNT];

            _compIdx = 0;
        }

        public void AddCanvas(UICanvas canvas, int idx)
        {

        }

        public override void InitData()
        {
            throw new NotImplementedException();
        }

        public override void Update(float time)
        {
            throw new NotImplementedException();
        }
    }
}


/*        private UICanvas[] _uiCanvases;
        /// <summary>
        /// Array containing all useable canvases.
        /// </summary>
        public UICanvas[] UICanvases { get => _uiCanvases; }

        private bool[] _existingCanvases;
        /// <summary>
        /// Specifies if a canvas in the UICanvases array exists in the current context.
        /// </summary>
        public bool[] ExistingCanvases { get => _existingCanvases; }
*/