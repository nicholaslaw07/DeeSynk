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

            Random rand = new Random();

            Managers.ObjectManager om = Managers.ObjectManager.GetInstance();
            for(int i=0; i < 10000; i++)
            {
                om.CreateCircle(1, rand.Next(-500, 500), rand.Next(-500, 500), 10, Color4.Blue).AddProgramIDs(programs).InitializeVAO();
            }
            Console.WriteLine("CREATION 1");
            for(int i=0; i < 10000; i++)
            {
                om.DeleteGameObject(i);
            }
            Console.WriteLine("DELETION 1");
            for(int i=0; i < 10000; i++)
            {
                om.CreateCircle(1, rand.Next(-500, 500), rand.Next(-500, 500), 10, Color4.Red).AddProgramIDs(programs).InitializeVAO();
            }
            Console.WriteLine("CREATION 2");
            for(int i=4000; i<5000; i++)
            {
                om.DeleteGameObject(i);
            }
            Console.WriteLine("DELETION 2");
            for(int i=0; i < 1000; i++)
            {
                om.CreateCircle(1, rand.Next(-500, 500), rand.Next(-500, 500), 10, Color4.Green).AddProgramIDs(programs).InitializeVAO();
            }
            Console.WriteLine("CREATION 3");
            //om.CreateRectangle(1, 100, 100, 55, -55, Color4.Blue).AddProgramIDs(programs).InitializeVAO();


            /* END TESTING */

        }

        public void Load()
        {
            Managers.ShaderManager.GetInstance().Load();
            Managers.ObjectManager.GetInstance().Load();
            Managers.TextureManager.GetInstance().Load();
            Console.WriteLine(GL.GetString(StringName.Renderer));
        }

        public void LoadGameData()
        {

        }
    }
}
