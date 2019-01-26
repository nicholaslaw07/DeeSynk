using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics.OpenGL4;

namespace DeeSynk
{
    public sealed class MainWindow : GameWindow
    {

        public MainWindow() : base( 500,                                    // initial width
                                    500,                                    // initial height
                                    OpenTK.Graphics.GraphicsMode.Default,
                                    "DeeSynk",                              // initial window title
                                    GameWindowFlags.Default,
                                    DisplayDevice.Default,
                                    4,                                      // OpenGL major version
                                    0,                                      // OpenGL minor version
                                    OpenTK.Graphics.GraphicsContextFlags.ForwardCompatible)
        {
            Title += " | The WIP Student Video Game | OpenGL Version: " + GL.GetString(StringName.Version);
        }
    }
}
