using Transit.Addon.TM.AI;
using Transit.Addon.TM.Data;
using Transit.Addon.TM.Traffic;

namespace Transit.Addon.TM.PathFindingFeatures
{
    public static partial class TMDataManager
    {
        public static void Init(TMConfigurationV2 configuration, byte[] options)
        {
            Log.Info("Initializing flags");
            Flags.OnBeforeLoadData();
            Log.Info("Initializing segment geometries");
            CustomRoadAI.OnBeforeLoadData();
            Log.Info("Initialization done. Loading mod data now.");

            ApplyConfiguration(configuration);

            Flags.clearHighwayLaneArrows();
            Flags.applyAllFlags();
            TrafficPriority.HandleAllVehicles();

            ApplyOptions(options);
        }
    }
}
