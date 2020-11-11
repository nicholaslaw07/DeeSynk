using OpenTK;
using OpenTK.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;
using DeeSynk.Core.Components.Models;
using DeeSynk.Core.Components.GraphicsObjects.Lights;
using OpenTK.Mathematics;

namespace DeeSynk.Core.Components.Types.Render
{
    public enum LightType
    {
        SPOTLIGHT = 1,
        SUN = 2,
        POINT = 3
    }

    public class ComponentLight : IComponent
    {
        public static readonly Color4 DEFAULT_COLOR = Color4.White;
        public static readonly TextureUnit DEFAULT_SHADOW_BINDING = TextureUnit.Texture1;
        public static readonly int DEFAULT_BINDING_BASE = 3; //will have multiple bindings for diferent types of lights

        public Component BitMaskID => throw new NotImplementedException();

        private LightType _lightType;
        public LightType LightType { get => _lightType; }

        private Light _light;
        public Light LightObject => _light;

        public ComponentLight(LightType lightType, Light light)
        {
            _lightType = lightType;
            _light = light;
        }

        //*************NOTE*************//
        // DETERMINE A WAY TO ADD ALL   //
        // OBJECTS THAT WILL BE STORED  //
        // IN A UBO UNDER ONE MAIN UBO  //
        // IF POSSIBLE. SIZE IS LIMITED //
        // TO 16KB, SO IT IS POSSIBLE   //
        // THAT MULTIPLE WILL BE NEEDED //
        // THIS WILL SAVE THE ISSUE OF  //
        // CONSTANTLY CHANGING BUFFERS  //
        // AND INSTEAD DETERMINE WHICH  //
        // BUFFER LOCATIONS GO WHERE    //
        // *****************************//

        //*************NOTE*************//
        // ADD BAKED SHADOWS.  THAT IS, //
        // ONLY UPDATE SHADOWS FOR ANY  //
        // OBJECTS THAT MOVE, THERE CAN //
        // BE SEPERATE SHADOWS MAPS FOR //
        // OBJECTS THAT DON"T MOVE, EVEN//
        // IF THEY OCCUPY THE SAME FIELD//
        // AS ONES THAT ARE MOVING      //
        //******************************//
    }
}
