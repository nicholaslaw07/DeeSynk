using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace DeeSynk.Core
{
    /// <summary>
    /// All objects and mechanic modeling should be housed here.
    /// </summary>
    public class Game
    {
        public Game()
        {
            Load();
        }

//........................................._¸„„„„_
//………..................................„--~*'¯…….'\
//……………............................. („-~~--„¸_….,/ì'Ì
//……………....…......................¸„-^"¯:::::¸-¯"¯/'
//………...…....……............¸.„„-^"¯:::::::'\¸„„,-"
//....**¯¯¯'^^~-„„„----~^*'"¯::::::::::¸-"
//......:.:.:.:.„-^":::::::::::::::::„-"
//:.:.:.:.:.:.:.:.:.:.:::::::::: ¸„-^¯
//.::.:.:.:.:.:.:.:.:::::::¸„„-^¯
//....:.'::'\:::::::;¸„„-~"
//:.:.:: :"-„""***/*'ì¸'¯
//...:.':::::"-„:::"\
//.....:.:.:::::"::::\,
//....:.::::::::::::'Ì
//....:::::::,::::::/
//"-„_::::_„-*__„„~"

        public void Load()
        {
            Managers.ShaderManager.GetInstance().Load();
            Managers.TextureManager.GetInstance().Load();
        }

        public void LoadGameData()
        {

        }
    }
}
