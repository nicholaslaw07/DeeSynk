using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeeSynk.Core.Managers
{
    /// <summary>
    /// Should be applied to all managers controlled by Game. Will ensure (to some degree) that all
    /// resources are responsibly managed.
    /// </summary>
    interface IManager
    {
        void Load();
        void UnLoad();
    }
}
