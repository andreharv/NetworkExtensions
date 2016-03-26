using Transit.Addon.TM.PathFinding;
using Transit.Addon.TM.PathFindingFeatures;
using Transit.Framework.ExtensionPoints.PathFinding;
using Transit.Framework.ExtensionPoints.PathFinding.Implementations;
using Transit.Framework.ExtensionPoints.PathFindingFeatures;
using Transit.Framework.Modularity;
using Transit.Framework.Redirection;
using Transit.Framework;
using Transit.Framework.ExtensionPoints.UI;

namespace Transit.Addon.TM
{
    [Module("Transit.Mod.TAM", "Transit.Mod.TrafficManager")]
    public partial class ToolModuleV2 : ModuleBase
    {
        private static bool gameLoaded = false;
        private static bool sm_redirectionInstalled = false;

        public override string Name
        {
            get { return "Tools"; }
        }

        public override int Order
        {
            get { return 10; }
        }

        public static bool IsGameLoaded()
        {
            return gameLoaded;
        }

        public override void OnEnabled()
        {
            Log.Info("TrafficManagerMod Enabled");
            base.OnEnabled();
            
            if (!sm_redirectionInstalled)
            {
                Redirector.PerformRedirections();
                sm_redirectionInstalled = true;
            }

            TAMPathFindManager.instance.DefinePathFinding<TAMImprovedPathFind>();
            TAMPathFindFeatureManager.instance.DefineRestrictionManager(TAMRestrictionManager.instance);
            TAMPathFindFeatureManager.instance.DefineSpeedLimitManager(TAMSpeedLimitManager.instance);
            TAMPathFindFeatureManager.instance.DefineLaneRoutingManager(TAMLaneRoutingManager.instance);

            Log.Info("TrafficManagerMod took over path finding helper services");
        }

        public override void OnDisabled()
        {
            Log.Info("TrafficManagerMod Disabled");
            base.OnDisabled();

            if (sm_redirectionInstalled)
            {
                Redirector.RevertRedirections();
                sm_redirectionInstalled = false;
            }

            TAMPathFindManager.instance.ResetPathFinding<TAMImprovedPathFind>();
            TAMPathFindFeatureManager.instance.ResetRestrictionManager<TAMRestrictionManager>();
            TAMPathFindFeatureManager.instance.ResetSpeedLimitManager<TAMSpeedLimitManager>();
            TAMPathFindFeatureManager.instance.ResetLaneRoutingManager<TAMLaneRoutingManager>();

            Log.Info("TrafficManagerMod released path finding helper services");
        }
    }
}
