using Transit.Addon.RoadExtensions.Compatibility;
using Transit.Framework;
using Transit.Framework.Builders;
using Transit.Framework.Network;

namespace Transit.Addon.RoadExtensions.Roads.PedestrianRoads.Legacy.Pavement
{
    public class ZonablePedestrianPavedRoadBuilder : ZonablePedestrianBuilderBase, INetInfoBuilderPart, ITrafficPlusPlusPart
    {
        public int Order { get { return 315; } }
        public int UIOrder { get { return 20; } }

        public string Name { get { return "Zonable Pedestrian Pavement"; } }
        public string DisplayName { get { return "Zonable Pedestrian Paved Road"; } }
        public string Description { get { return "Paved roads are nicer to walk on than gravel."; } }
        public string ShortDescription { get { return "Zoneable, No Passenger Vehicles [Traffic++ V2 required]"; } }

        public string ThumbnailsPath { get { return @"Roads\PedestrianRoads\Legacy\Pavement\thumbnails.png"; } }
        public string InfoTooltipPath { get { return @"Roads\PedestrianRoads\Legacy\Pavement\infotooltip.png"; } }

        public NetInfoVersion SupportedVersions
        {
            get { return NetInfoVersion.Ground; }
        }

        public override void BuildUp(NetInfo info, NetInfoVersion version)
        {
            ///////////////////////////
            // Set up                //
            ///////////////////////////
            info.m_createGravel = false;
            info.m_createPavement = true;

            base.BuildUp(info, version);

            info.m_setVehicleFlags = Vehicle.Flags.None;

            ///////////////////////////
            // AI                    //
            ///////////////////////////
            var pedestrianVanilla = Prefabs.Find<NetInfo>(NetInfos.Vanilla.PED_PAVEMENT);
            var vanillaplayerNetAI = pedestrianVanilla.GetComponent<PlayerNetAI>();
            var playerNetAI = info.GetComponent<PlayerNetAI>();

            if (playerNetAI != null)
            {
                playerNetAI.m_constructionCost = vanillaplayerNetAI.m_constructionCost * 2;
                playerNetAI.m_maintenanceCost = vanillaplayerNetAI.m_maintenanceCost * 2;
            }
        }
    }
}
