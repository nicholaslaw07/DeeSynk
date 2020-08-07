using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeeSynk.Core
{
    using UICanvas = Components.Types.UI.UICanvas;
    using IUIElement = Components.Types.UI.IUIElement;

    public class UI
    {
        private const uint OBJECT_MEMORY = 4;
        /// <summary>
        /// Maximum number of canvases that can be stored and switched between
        /// </summary>
        public uint ObjectMemory { get => OBJECT_MEMORY; }

        private UICanvas[] _uiCanvases;
        /// <summary>
        /// Array containing all useable canvases.
        /// </summary>
        public UICanvas[] UICanvases { get => _uiCanvases; }

        private bool[] _existingCanvases;
        /// <summary>
        /// Specifies if a canvas in the UICanvases array exists in the current context.
        /// </summary>
        public bool[] ExistingCanvases { get => _existingCanvases; }
    }
}
