﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DeeSynk.Core.Components;
using DeeSynk.Core.Systems;
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
        private SystemInput _systemInput;
        public SystemInput SystemInput { get => _systemInput; }

        public Game()
        {
            Load();
        }

        public void Load()
        {
            Managers.ShaderManager.GetInstance().Load();
            Managers.TextureManager.GetInstance().Load();
            Managers.ModelManager.GetInstance().Load();
            _world = new World();
            _systemInput = new SystemInput();
        }

        public void PushCameraRef(ref Camera camera)
        {
            _world.PushCameraRef(ref camera);
        }

        public void LoadGameData()
        {

        }

        public void Update(float time)
        {
            _world.Update(time);
        }

        public void Render()
        {
            _world.Render();
        }
    }
}
