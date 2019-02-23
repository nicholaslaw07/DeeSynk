using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeeSynk.Components.Managers
{
    class ObjectManager : IManager
    {
        private static ObjectManager _objectManager;

        private ObjectManager()
        {

        }

        public static ref ObjectManager GetInstance()
        {
            if (_objectManager == null)
                _objectManager = new ObjectManager();

            return ref _objectManager;
        }

        public void Load()
        {

        }
         public void UnLoad()
        {
            
        }
    }
}
