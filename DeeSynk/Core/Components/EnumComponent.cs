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
        TRANSFORM   = 1 << 1,
        VELOCITY    = 1 << 2,
        GRAVITY     = 1 << 3,
        ROTATION_X  = 1 << 4,
        ROTATION_Y  = 1 << 5,
        ROTATION_Z	= 1 << 6,
        SCALE       = 1 << 7
    }
}
