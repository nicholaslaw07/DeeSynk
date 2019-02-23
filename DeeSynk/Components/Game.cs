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
            r = new Random();
            Load();

            goList = new List<GameObject>();

            ColoredVertex[] vertices = {new ColoredVertex(new Vector4(-10f, -10f, 1.0f, 1.0f), Color4.Red),
                                        new ColoredVertex(new Vector4(10f, -10f, 1.0f, 1.0f), Color4.Green),
                                        new ColoredVertex(new Vector4(10f, 10f, 1.0f, 1.0f), Color4.Blue),
                                        new ColoredVertex(new Vector4(-10f, 10f, 1.0f, 1.0f), Color4.Yellow)};

            //ColoredVertex[] vertices = {new ColoredVertex(new Vector4(-1f, -1f, 1.0f, 1.0f), Color4.Red),
                                       // new ColoredVertex(new Vector4(1f, -1f, 1.0f, 1.0f), Color4.Green),
                                       // new ColoredVertex(new Vector4(1f, 1f, 1.0f, 1.0f), Color4.Blue)};

            uint[] indices = { 0, 1, 2, 0, 2, 3};

            int program = Managers.ShaderManager.GetInstance().GetProgram("defaultColored");
            int[] programs = { program };

            for (int i = 0;i<10000; i++)
            {
                goList.Add(new GameObject(1, 1, vertices, vertices.Length, indices, false,
                                            new Vector3((float)(r.NextDouble() * 1400d - 700d), (float)(r.NextDouble() * 1000d - 500d), 0.0f), 0.0f, 0.0f ,0.0f, new Vector3((float)(r.NextDouble()) + 0.2f, (float)(r.NextDouble()) + 0.2f, 0.0f))
                                .AddProgramIDs(programs)
                                .InitializeVAO());
            }

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
            //go.Render(ortho);

            foreach(GameObject g in goList)
            {
                g.Render(ortho);
            }
        }
    }
}
