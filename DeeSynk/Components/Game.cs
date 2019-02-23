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
        public Game()
        {
            Load();

            ColoredVertex[] vertices = {new ColoredVertex(new Vector4(-1.5f, -1.5f, 1.0f, 1.0f), Color4.BlanchedAlmond),
                                        new ColoredVertex(new Vector4(1.5f, -1.5f, 1.0f, 1.0f), Color4.Black),
                                        new ColoredVertex(new Vector4(1.5f, 1.5f, 1.0f, 1.0f), Color4.BlanchedAlmond),
                                        new ColoredVertex(new Vector4(-1.5f, 1.5f, 1.0f, 1.0f), Color4.BlanchedAlmond)};

            //ColoredVertex[] vertices = {new ColoredVertex(new Vector4(-1f, -1f, 1.0f, 1.0f), Color4.Red),
                                       // new ColoredVertex(new Vector4(1f, -1f, 1.0f, 1.0f), Color4.Green),
                                       // new ColoredVertex(new Vector4(1f, 1f, 1.0f, 1.0f), Color4.Blue)};

            uint[] indices = { 0, 1, 2, 0, 2, 3};

            int program = Managers.ShaderManager.GetInstance().GetProgram("defaultColored");
            int[] programs = { program };

            go = new GameObject(1, 1, vertices, vertices.Length, indices, false)
                                .AddProgramIDs(programs)
                                .InitializeVAO();
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
            go.Render(ortho);
        }
    }
}
