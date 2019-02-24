using DeeSynk.Components.Renderables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace DeeSynk.Components
{
    /// <summary>
    /// All objects and mechanic modeling should be housed here.
    /// </summary>
    public class Game
    {
        public Game()
        {
            Load();

            /* TESTING */

            // not sure how to handle these programs yet, I suspect a component based approach for
            // renderability is the answer.
            int program = Managers.ShaderManager.GetInstance().GetProgram("defaultColored");
            int[] programs = { program };

            Managers.ObjectManager om = Managers.ObjectManager.GetInstance();

            om.CreateCircle(1, 0, 0, 160, Color4.Black).AddProgramIDs(programs).InitializeVAO();

            om.CreateRectangle(1, 100, 100, -55, 55, Color4.Red).AddProgramIDs(programs).InitializeVAO();
            om.CreateRectangle(1, 100, 100, -55, -55, Color4.Yellow).AddProgramIDs(programs).InitializeVAO();
            om.CreateRectangle(1, 100, 100, 55, 55, Color4.Green).AddProgramIDs(programs).InitializeVAO();
            om.CreateRectangle(1, 100, 100, 55, -55, Color4.Blue).AddProgramIDs(programs).InitializeVAO();

            
            /* END TESTING */

        }

        public void Load()
        {
            Managers.ShaderManager.GetInstance().Load();
            Managers.ObjectManager.GetInstance().Load();
            Managers.TextureManager.GetInstance().Load();
        }

        public void LoadGameData()
        {

        }
    }
}
