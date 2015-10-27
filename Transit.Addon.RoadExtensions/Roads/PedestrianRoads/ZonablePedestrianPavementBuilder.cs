using Transit.Framework;
using Transit.Framework.Builders;

namespace Transit.Addon.RoadExtensions.Roads.PedestrianRoads
{
    public class ZonablePedestrianPavementBuilder : ZonablePedestrianBuilderBase, INetInfoBuilderPart
    {
        public int Order { get { return 310; } }
        public int UIOrder { get { return 20; } }

        public string Name { get { return "Zonable Pedestrian Pavement"; } }
        public string DisplayName { get { return "Zonable Pedestrian Pavement"; } }
        public string Description { get { return "Paved roads are nicer to walk on than gravel."; } }

        public string ThumbnailsPath { get { return @"Roads\PedestrianRoads\thumbnails_pavement.png"; } }
        public string InfoTooltipPath { get { return @"Roads\PedestrianRoads\infotooltip.png"; } }

        public NetInfoVersion SupportedVersions
        {
            get { return NetInfoVersion.Ground | NetInfoVersion.Elevated; }
        }

        public override void BuildUp(NetInfo info, NetInfoVersion version)
        {
            ///////////////////////////
            // Set up                //
            ///////////////////////////
            info.m_createGravel = false;
            info.m_createPavement = true;

            base.BuildUp(info, version); 
            
            switch (version)
            {
                case NetInfoVersion.Ground:
                    {
                        info.m_setVehicleFlags = Vehicle.Flags.None;
                    }
                    break;
            }

            ///////////////////////////
            // AI                    //
            ///////////////////////////
            var pedestrianVanilla = Prefabs.Find<NetInfo>(NetInfos.Vanilla.PED_PAVEMENT);

            switch (version)
            {
                case NetInfoVersion.Ground:
                    {
                        var vanillaplayerNetAI = pedestrianVanilla.GetComponent<PlayerNetAI>();
                        var playerNetAI = info.GetComponent<PlayerNetAI>();

                        if (playerNetAI != null)
                        {
                            playerNetAI.m_constructionCost = vanillaplayerNetAI.m_constructionCost * 2;
                            playerNetAI.m_maintenanceCost = vanillaplayerNetAI.m_maintenanceCost * 2;
                        }
                    }
                    break;
            }
        }
    }
}
