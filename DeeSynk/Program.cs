using DeeSynk.Core;
using System;
using System.Collections.Generic;
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
        [STAThread]
        static void Main()
        {
            new MainWindow().Run(60);
        }
    }
}
