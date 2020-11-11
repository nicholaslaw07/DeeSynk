using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeeSynk.Core.Components.Types.UI
{
    public class ComponentText : IComponent
    {
        public Component BitMaskID => throw new NotImplementedException();

        private string _text;
        public string Text { get => _text; }

        //This will require a font system since we need knowledge of the size of the text in pixels depending on the font size.
        //Some options for this may include whether or not to wrap with the text box.  Maybe this will need to be paired with a text box.
        //Maybe this will instead be replaced with a text region...
        //Actually... a text window would make more sense.  It could also be a UIElement instead.  Or no.
        //This just holds the text that is in the box but the text in the UIElement uses the text from this component?  Maybe not. Idk.

        public ComponentText(string text)
        {
            _text = text;
        }
    }
}
