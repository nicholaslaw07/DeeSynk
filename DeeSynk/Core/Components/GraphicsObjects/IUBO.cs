using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace DeeSynk.Core.Components.GraphicsObjects
{
    public interface IUBO
    {
        bool InitUBO { get; }
        int UBO_Id { get; }
        int BindingLocation { get; }
        Vector4[] BufferData { get; }
        int BufferSize { get; }

        void BuildUBO(int bindingLocation, int numOfVec4s);
        void AttachUBO(int bindingLocation);
        void DetatchUBO();
        void UpdateUBO();
        void FillBuffer();
    }
}
