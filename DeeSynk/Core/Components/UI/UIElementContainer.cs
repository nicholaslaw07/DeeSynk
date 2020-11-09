using DeeSynk.Core.Components.Input;
using DeeSynk.Core.Components.Types.UI;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeeSynk.Core.Components.UI
{
    public class UIElementContainer : UIElement
    {
        public override string ID_GLOBAL_DEFAULT => "CONTAINER";

        public UIElementContainer(int[] childElementIDs, UIElementType elementType, int width, int height, Vector2 position,
                          PositionType positionType, PositionReference reference, int layer, int globalIndex)

            : base(childElementIDs, elementType, width, height, position,
                   positionType, reference, layer, globalIndex)
        {

        }

        public UIElementContainer(int childElementCount, UIElementType elementType, int width, int height, Vector2 position,
                                  PositionType positionType, PositionReference reference, int layer, int globalIndex)

            : base(childElementCount, elementType, width, height, position,
                   positionType, reference, layer, globalIndex)
        {

        }
        public override void AddChild(UIElement e)
        {
            base.AddChild(e);
        }

        public override void ClickAt(float time, MousePosition mouseClick, MouseMove mouseMove)
        {
            //ADD CLICK ACTION HERE
        }

        public override bool Update(float time)
        {
            throw new NotImplementedException();
        }
    }
}
