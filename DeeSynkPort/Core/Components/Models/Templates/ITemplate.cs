using DeeSynk.Core.Components.Types.Render;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeeSynk.Core.Components.Models.Templates
{
    public interface ITemplate
    {
        Model ConstructModel(ComponentModelStatic modelComp);
    }
}
