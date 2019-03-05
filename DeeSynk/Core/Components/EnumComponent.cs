using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeeSynk.Core.Components
{
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

        RENDER      = 1 << 7,
        MODEL       = 1 << 8,
        TEXTURE     = 1 << 9,
        COLOR       = 1 << 10
    }
}
