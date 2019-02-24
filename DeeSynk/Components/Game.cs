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
        private GameObject go;
        private List<GameObject> goList;
        Random r;
        public Game()
        {
            Load();


            int program = Managers.ShaderManager.GetInstance().GetProgram("defaultColored");
            int[] programs = { program };

            Managers.ObjectManager om = Managers.ObjectManager.GetInstance();

            om.CreateRectangle(1, 100, 100, -55, 55, Color4.Red).AddProgramIDs(programs).InitializeVAO();
            om.CreateRectangle(1, 100, 100, -55, -55, Color4.Yellow).AddProgramIDs(programs).InitializeVAO();
            om.CreateRectangle(1, 100, 100, 55, 55, Color4.Green).AddProgramIDs(programs).InitializeVAO();
            om.CreateRectangle(1, 100, 100, 55, -55, Color4.Blue).AddProgramIDs(programs).InitializeVAO();
            

        }

        public void Load()
        {
            LoadShaders();
        }

        public void LoadShaders()
        {
            Managers.ShaderManager.GetInstance().Load();
        }

        public void LoadTextures()
        {

        }

        public void LoadGameData()
        {

        }

        //Test Method
        public void Render(Matrix4 ortho)
        {
            Managers.ObjectManager.GetInstance().Render(ortho);
        }
    }
}
