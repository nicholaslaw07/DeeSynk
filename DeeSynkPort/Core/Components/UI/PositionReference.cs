using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeeSynk.Core.Components.UI
{
    public enum PositionReference : short
    {
        CENTER = 0,
        CORNER_BOTTOM_LEFT = 1,
        CORNER_BOTTOM_RIGHT = 2,
        CORNER_TOP_LEFT = 3,
        CORNER_TOP_RIGHT = 4,
        CENTER_LEFT = 5,
        CENTER_RIGHT = 6,
        TOP_CENTER = 7,
        BOTTOM_CENTER = 8
    }
}
