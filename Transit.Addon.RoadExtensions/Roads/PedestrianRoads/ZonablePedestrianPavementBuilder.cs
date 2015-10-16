using System;
using Transit.Addon.RoadExtensions.Menus;
using Transit.Framework;
using Transit.Framework.Modularity;

namespace Transit.Addon.RoadExtensions.Roads.PedestrianRoads
{
    public class ZonablePedestrianPavementBuilder : NetInfoBuilderBase, INetInfoBuilder
    {
        public int Order { get { return 200; } }
        public int Priority { get { return 10; } }

        public string TemplatePrefabName { get { return NetInfos.Vanilla.PED_GRAVEL; } }
        public string Name { get { return "Zonable Pedestrian Pavement"; } }
        public string DisplayName { get { return "Zonable Pedestrian Road with Pavement"; } }
        public string CodeName { get { return "Z_PED_PAVEMENT"; } }
        public string Description { get { return "TODO."; } }
        public string UICategory { get { return AdditionnalMenus.ROADS_PEDESTRIANS; } }

        public string ThumbnailsPath    { get { return @"Roads\PedestrianRoads\thumbnails.png"; } }
        public string InfoTooltipPath   { get { return @"Roads\PedestrianRoads\infotooltip.png"; } }

        public NetInfoVersion SupportedVersions
        {
            get { return NetInfoVersion.Ground | NetInfoVersion.Elevated; }
        }

        public void BuildUp(NetInfo info, NetInfoVersion version)
        {
            ///////////////////////////
            // Templates             //
            ///////////////////////////
            var onewayInfo = Prefabs.Find<NetInfo>(NetInfos.Vanilla.ONEWAY_2L);
            var pedestrianPavement = Prefabs.Find<NetInfo>(NetInfos.Vanilla.PED_PAVEMENT);


            ///////////////////////////
            // Texturing             //
            ///////////////////////////


            ///////////////////////////
            // Set up                //
            ///////////////////////////
            info.m_createGravel = false;
            info.m_createPavement = version == NetInfoVersion.Ground;
            info.m_setVehicleFlags = Vehicle.Flags.None;
            info.m_UnlockMilestone = onewayInfo.m_UnlockMilestone;

            switch (version)
            {
                case NetInfoVersion.Ground:
                    {
                        var vanillaplayerNetAI = pedestrianPavement.GetComponent<PlayerNetAI>();
                        var playerNetAI = info.GetComponent<PlayerNetAI>();

                        if (playerNetAI != null)
                        {
                            playerNetAI.m_constructionCost = vanillaplayerNetAI.m_constructionCost * 2;
                            playerNetAI.m_maintenanceCost = vanillaplayerNetAI.m_maintenanceCost * 2;
                        }
                    }
                    break;
                case NetInfoVersion.Elevated:
                case NetInfoVersion.Bridge:
                    {
                    
                    }
                    break;
            }
        }
    }
}
