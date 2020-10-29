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
                          UIPositionType positionType, int layer, int globalIndex, string canvasID, string parentPath)

            : base(childElementIDs, elementType, width, height, position,
                   positionType, layer, globalIndex, canvasID, parentPath)
        {

        }

        public UIElementContainer(int childElementCount, UIElementType elementType, int width, int height, Vector2 position,
                                  UIPositionType positionType, int layer, int globalIndex, string canvasID, string parentPath)

            : base(childElementCount, elementType, width, height, position,
                   positionType, layer, globalIndex, canvasID, parentPath)
        {

        }
        public override void AddChild(UIElement e)
        {
            base.AddChild(e);
        }

        public override bool Update(float time)
        {
            throw new NotImplementedException();
        }
    }
}
