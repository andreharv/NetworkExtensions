using System.Collections.Generic;
using ICities;
using Transit.Framework.Modularity;

namespace Transit.Addon.RealisticSpeed
{
    public partial class RealisticSpeedModule : ModuleBase
    {
        public override void OnLevelLoaded(LoadMode mode)
        {
            base.OnLevelLoaded(mode);

            if (mode == LoadMode.NewGame || mode == LoadMode.LoadGame)
            {
                if ((ActiveOptions & Options.UseRealisticSpeeds) == Options.UseRealisticSpeeds)
                    RealisticSpeedManager.Activate();
            }
        }

        public override void OnLevelUnloading()
        {
            base.OnLevelUnloading();

            if ((ActiveOptions & Options.UseRealisticSpeeds) == Options.UseRealisticSpeeds)
                RealisticSpeedManager.Deactivate();
        }
    }
}
