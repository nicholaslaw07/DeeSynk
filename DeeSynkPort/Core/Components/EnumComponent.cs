﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeeSynk.Core.Components
{
    [Flags]
    public enum Component : int
    {
        NONE        = 0,

        LOCATION    = 1,
        VELOCITY    = 1 << 1,
        GRAVITY     = 1 << 2,
        ROTATION_X  = 1 << 3,
        ROTATION_Y  = 1 << 4,
        ROTATION_Z	= 1 << 5,
        SCALE       = 1 << 6,
        TRANSFORM   = 1 << 7,

        ROTATION_XY  = ROTATION_X | ROTATION_Y,
        ROTATION_XZ  = ROTATION_X              | ROTATION_Z,
        ROTATION_YZ  =              ROTATION_Y | ROTATION_Z,
        ROTATION_XYZ = ROTATION_X | ROTATION_Y | ROTATION_Z,

        RENDER        = 1 << 8,
        MODEL_STATIC  = 1 << 9,
        TEXTURE       = 1 << 10,
        MATERIAL      = 1 << 11,
        CAMERA        = 1 << 12,
        LIGHT         = 1 << 13,
        UI_CANVAS     = 1 << 14,
        UI_ELEMENT    = 1 << 15,

        UI_STANDARD   = UI_ELEMENT | RENDER | MODEL_STATIC | TRANSFORM
    }
}
