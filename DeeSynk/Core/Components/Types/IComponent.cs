using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeeSynk.Core.Components.Types
{
    public interface IComponent
    {
        Component BitMaskID { get; }
        void Update(float time);
    }
}
