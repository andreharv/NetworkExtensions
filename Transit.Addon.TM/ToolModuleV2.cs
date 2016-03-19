using Transit.Addon.TM.PathFinding;
using Transit.Addon.TM.PathFindingFeatures;
using Transit.Framework.ExtensionPoints.PathFinding;
using Transit.Framework.ExtensionPoints.PathFinding.Implementations;
using Transit.Framework.ExtensionPoints.PathFindingFeatures;
using Transit.Framework.Modularity;
using Transit.Framework.Redirection;
using Transit.Framework;

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

            TAMPathFindManager.instance.DefinePathFinding<ImprovedPathFind>();

            // TODO: Convert TM LaneArrows into TPP routes
            TAMPathFindFeatureManager.instance.DefineLaneRoutingManager(TPPLaneRoutingManager.instance);

            // TODO: Merge those 2
            //TAMPathFindFeatureManager.instance.DefineRoadRestrictionManager(TPPRoadRestrictionManager.instance);
            TAMPathFindFeatureManager.instance.DefineRoadRestrictionManager<TMRoadRestrictionManager>();

            // TODO: Convert TPP data into TMRoadSpeedManager
            //TAMPathFindFeatureManager.instance.DefineRoadSpeedManager<TPPRoadSpeedManager>();
            TAMPathFindFeatureManager.instance.DefineRoadSpeedManager<TMRoadSpeedManager>();

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

            TAMPathFindManager.instance.ResetPathFinding<TAMVanillaPathFind>();
            TAMPathFindFeatureManager.instance.ResetLaneRoutingManager<TPPLaneRoutingManager>();
            TAMPathFindFeatureManager.instance.ResetRoadRestrictionManager<TMRoadRestrictionManager>();
            TAMPathFindFeatureManager.instance.ResetRoadSpeedManager<TMRoadSpeedManager>();

            Log.Info("TrafficManagerMod released path finding helper services");
        }
    }
}
