using DeeSynk.Core.Algorithms;
using DeeSynk.Core.Components;
using DeeSynk.Core.Components.Models;
using DeeSynk.Core.Components.Types.Render;
using DeeSynk.Core.Managers;
using OpenTK;
using OpenTK.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeeSynk.Core.Systems
{
    using ModelTemplates = DeeSynk.Core.Components.Models.Templates.ModelTemplates;
    public class SystemModel : ISystem
    {
        public Component MonitoredComponents => Component.MODEL_STATIC;

        private World _world;
        private UI _ui;

        private bool[] _monitoredGameObjects_W;
        private bool[] _monitoredGameObjects_U;

        private ComponentModelStatic[] _staticModelComps_W;
        private ComponentModelStatic[] _staticModelComps_U;

        public SystemModel(World world, UI ui)
        {
            _world = world;
            _ui = ui;

            _monitoredGameObjects_W = new bool[_world.ObjectMemory];
            _monitoredGameObjects_U = new bool[_ui.ObjectMemory];

            _staticModelComps_W = _world.StaticModelComps;
            _staticModelComps_U = _ui.StaticModelComps;
        }

        public void UpdateMonitoredGameObjects()
        {
            //UpdateMonitoredGameObjects(_world, _monitoredGameObjects_W);
            //UpdateMonitoredGameObjects(_ui, _monitoredGameObjects_U);
        }

        private void UpdateMonitoredGameObjects(GameObjectContainer c, bool[] monitor)
        {
            for (int i = 0; i < c.ObjectMemory; i++)
            {
                if (c.ExistingGameObjects[i])
                {
                    if (c.GameObjects[i].Components.HasFlag(MonitoredComponents))
                    {
                        monitor[i] = true;
                    }
                }
            }
        }

        //TEST START
        public void InitModel()
        {
            //CreateModels();
            //LinkModels(_world);
            //LinkModels(_ui);
        }

        /// <summary>
        /// Creates a ComponentModelStatic for each model that is to be registered.
        /// </summary>
        private void CreateModels()
        {

        }

        /// <summary>
        /// Links models from ModelManager, either prexisting or registered from template, to each ComponentModelStatic based on the specifications stored in CreateModels.
        /// </summary>
        public void LinkModels(GameObjectContainer c)
        {
            var modelManager = ModelManager.GetInstance();
            for (int idx = 0; idx < c.ObjectMemory; idx++)
            {
                if (c.GameObjects[idx].Components.HasFlag(Component.MODEL_STATIC))
                    modelManager.InitModel(ref c.StaticModelComps[idx]);
            }
        }

        //TEST END

        public void Update(float time)
        {
            throw new NotImplementedException();
        }
    }
}
