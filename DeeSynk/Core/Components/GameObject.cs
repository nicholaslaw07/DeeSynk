using System;
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
        public int Components;

        public GameObject(int id, int comps)
        {
            ID = id;
            Components = comps;
        }

        public bool HasComponent(Component c)
        {
            if ((ID & (int)c) == (int)c)
                return true;
            return false;
        }

        public bool HasComponent(int c)
        {
            if ((ID & c) == c)
                return true;
            return false;
        }

        public bool HasComponents(int cs)
        {
            return HasComponent(cs);
        }
    }

}



