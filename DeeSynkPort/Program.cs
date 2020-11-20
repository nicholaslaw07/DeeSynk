using DeeSynk.Core;
using OpenTK.Windowing.Desktop;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// The Program class acts as a driver for the entirety of the game.
/// It instantiates an instance of the MainWindow class (which inherits from the GameWindow class),
/// and then immediately calls the Run method with a specified FPS, which starts the game. 
/// </summary>

namespace DeeSynk
{

    static class Program
    {
        public static MainWindow window;

        [STAThread]
        static void Main()
        {
            GameWindowSettings gs = new GameWindowSettings();
            //gs.IsMultiThreaded = true;
            NativeWindowSettings ns = new NativeWindowSettings();
            //ns.API = OpenTK.Windowing.Common.ContextAPI.OpenGL;
            ns.APIVersion = Version.Parse("4.6");
            ns.AutoLoadBindings = true;
            //ns.IsFullscreen = false;
            window = new MainWindow(gs, ns);
            window.Run();
        }
    }


}
