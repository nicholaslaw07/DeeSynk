using System;
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
        CAMERA        = 1 << 11,
        LIGHT         = 1 << 12,
        UI_CANVAS    = 1 << 13,

        UI_STANDARD   = UI_CANVAS | RENDER | MODEL_STATIC
    }
}
