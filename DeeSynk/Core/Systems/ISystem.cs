using DeeSynk.Core.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeeSynk.Core.Systems
{
    public interface ISystem
    {
        Component MonitoredComponents { get; }
        void Update(float time);
    }
}
