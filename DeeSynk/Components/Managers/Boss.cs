using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeeSynk.Components.Managers
{
    /// <summary>
    /// Runs in parallel to the MainWindow class, managing resources and the other manager classes.
    /// </summary>
    public class Boss
    {

        private ShaderManager _shaderManager;

        public Boss()
        {
            _shaderManager = new ShaderManager();
        }
    }
}
