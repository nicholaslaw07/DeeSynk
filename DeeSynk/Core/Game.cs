using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace DeeSynk.Core
{
    /// <summary>
    /// All objects and mechanic modeling should be housed here.
    /// </summary>
    public class Game
    {
        private World _world;
        public Game()
        {
            Load();
        }

        public void Load()
        {
            Managers.ShaderManager.GetInstance().Load();
            Managers.TextureManager.GetInstance().Load();
            _world = new World();
        }

        public void LoadGameData()
        {
            //TestStart
            //Stopwatch sw = new Stopwatch();
            //sw.Start();
            //for(int i=0; i<10000; i++)
            //{
            //    _world.InitializeComponents(ref _world.CreateGameObject(255), new Vector4(1.0f, 1.0f, 1.0f, 1.0f), new Vector4(1.0f, 1.0f, 1.0f, 1.0f), 1.0f, 1.0f, 1.0f, new Vector3(1.0f, 1.0f, 1.0f));
            //}
            //sw.Stop();
            //Console.WriteLine(sw.ElapsedMilliseconds);
            //TestEnd
        }

        public void Update(float time)
        {
            //TestStart
            //Stopwatch sw = new Stopwatch();
            //sw.Start();
            //_world.Update(time);
            //sw.Stop();
            //Console.WriteLine(sw.ElapsedMilliseconds);
            //TestEnd
            _world.Update(time);
        }

        public void Render()
        {
            _world.Render();
        }
    }
}
