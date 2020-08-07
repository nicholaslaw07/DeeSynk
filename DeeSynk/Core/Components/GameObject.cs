﻿using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;

namespace DeeSynk.Core.Components
{

    public struct GameObject
    {
        public int ID;
        public Component Components;

        public GameObject(int id, Component comps)
        {
            ID = id;
            Components = comps;
        }
    }

}



