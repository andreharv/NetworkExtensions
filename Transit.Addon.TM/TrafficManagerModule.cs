using ICities;
using Transit.Addon.TM;
using Transit.Addon.TM.PathFinding;
using Transit.Addon.TM.PathFindingFeatures;
using Transit.Addon.TM.State;
using Transit.Framework.ExtensionPoints.PathFinding;
using Transit.Framework.ExtensionPoints.PathFindingFeatures;
using Transit.Framework.Modularity;
using Transit.Framework.Redirection;
using UnityEngine;

namespace Transit.Addon.TM
{
    [Module("Transit.Addon.Mod")]
    public partial class TrafficManagerModule : ModuleBase
    {
        public override string Name => "Traffic Manager";

        public override int Order => 20;

        private static bool sm_redirectionInstalled = false;

        public override void OnEnabled()
        {
            Log._Debug("TrafficManagerMod Enabled");
            base.OnEnabled();

            if (!sm_redirectionInstalled)
            {
                Redirector.PerformRedirections();
                sm_redirectionInstalled = true;
            }

            TAMPathFindManager.instance.DefinePathFinding<ImprovedPathFind>();
            //TAMPathFindFeatureManager.instance.DefineLaneRoutingManager<TMLaneRoutingManager>();
            TAMPathFindFeatureManager.instance.DefineRoadRestrictionManager<TMRoadRestrictionManager>();
            TAMPathFindFeatureManager.instance.DefineRoadSpeedManager<TMRoadSpeedManager>();
        }

        public override void OnDisabled()
        {
            Log._Debug("TrafficManagerMod Disabled");
            base.OnDisabled();

            if (sm_redirectionInstalled)
            {
                Redirector.RevertRedirections();
                sm_redirectionInstalled = false;
            }

            TAMPathFindManager.instance.ResetPathFinding<ImprovedPathFind>();
            //TAMPathFindFeatureManager.instance.ResetLaneRoutingManager<TMLaneRoutingManager>();
            TAMPathFindFeatureManager.instance.ResetRoadRestrictionManager<TMRoadRestrictionManager>();
            TAMPathFindFeatureManager.instance.ResetRoadSpeedManager<TMRoadSpeedManager>();
        }

        public override void OnSettingsUI(UIHelperBase helper)
        {
            Options.makeSettings(helper);
        }
    }
}
