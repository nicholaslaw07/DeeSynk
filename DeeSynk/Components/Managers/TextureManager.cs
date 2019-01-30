using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeeSynk.Components.Managers
{
    class TextureManager : IManager
    {
        private static TextureManager _textureManager;

        private TextureManager()
        {

        }

        public static ref TextureManager GetInstance()
        {
            if (_textureManager == null)
                _textureManager = new TextureManager();

            return ref _textureManager;
        }

        public void Load()
        {

        }

        public void UnLoad()
        {

        }
    }
}
