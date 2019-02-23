using DeeSynk.Components.Renderables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics;

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

            ColoredVertex[] vertices = {new ColoredVertex(new Vector4(100f, 100f, 1.0f, 1.0f), Color4.Black),
                                        new ColoredVertex(new Vector4(300f, 100f, 1.0f, 1.0f), Color4.Black),
                                        new ColoredVertex(new Vector4(300f, 300f, 1.0f, 1.0f), Color4.Black),
                                        new ColoredVertex(new Vector4(100f, 300f, 1.0f, 1.0f), Color4.Black)};

            int[] indices = { 0, 1, 2, 2, 1, 3 };

            int program = Managers.ShaderManager.GetInstance().GetProgram("defaultColored");
            int[] programs = { program };

            go = new GameObject(1, 1, vertices, vertices.Length, indices, false)
                                .AddProgramIDs(programs)
                                .InitializeVAO(vertices);
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
