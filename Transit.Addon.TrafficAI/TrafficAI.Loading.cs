using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICities;
using Transit.Framework.Modularity;
using Transit.Framework.Unsafe;

namespace Transit.Addon.TrafficAI
{
    public partial class TrafficAIModule
    {
        public override void OnLevelLoaded(LoadMode mode)
        {
            if (mode == LoadMode.NewGame || mode == LoadMode.LoadGame)
            {
                Redirector.PerformRedirections();
            }
        }

        public override void OnLevelUnloading()
        {
            Redirector.RevertRedirections();
        }
    }
}
