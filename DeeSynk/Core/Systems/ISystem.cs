﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeeSynk.Core.Systems
{
    public interface ISystem
    {
        int MonitoredComponents { get; }
        void Update(float time);
    }
}
