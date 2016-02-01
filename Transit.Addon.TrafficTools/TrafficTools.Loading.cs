using ICities;
using System;
using Transit.Framework.Modularity;
using Transit.Framework.UI;
using Transit.Framework.Unsafe;

namespace Transit.Addon.TrafficTools
{
    public partial class TrafficToolsModule
    {
        public override void OnCreated(ILoading loading)
        {
            RoadCustomizerTool.Init();
        }

        public override void OnLevelLoaded(LoadMode mode)
        {
            Redirector.PerformRedirections();
        }

        public override void OnLevelUnloading()
        {
            Redirector.RevertRedirections();
        }

        // Not sure this is needed
        //public override void OnReleased()
        //{
        //    RoadCustomizerTool.CleanUp();
        //}
    }
}
