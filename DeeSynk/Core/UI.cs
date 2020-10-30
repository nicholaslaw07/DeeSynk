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
        private FBO[] _fbos;
        /// <summary>
        /// Array of frame buffer objects.
        /// </summary>
        public FBO[] FBOs { get => _fbos; }

        public UI(uint objectMemory) : base(objectMemory)
        {
            _fbos = new FBO[FBO_COUNT];
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