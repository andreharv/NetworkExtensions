using Transit.Addon.RoadExtensions.Roads.Common;
using Transit.Addon.RoadExtensions.Roads.PedestrianRoads.Common;
using Transit.Addon.RoadExtensions.UI.Toolbar.Roads;
using Transit.Framework;
using Transit.Framework.Builders;
using Transit.Framework.Network;

namespace Transit.Addon.RoadExtensions.Roads.PedestrianRoads.Gravel
{
    public class ZonablePedestrianGravelRoadBuilder : Activable, INetInfoBuilderPart, INetInfoLateBuilder
    {
        public int Order { get { return 305; } }
        public int UIOrder { get { return 10; } }

        public string BasedPrefabName { get { return NetInfos.Vanilla.ROAD_2L; } }
        public string UICategory { get { return PedestriansRoadsCategoryInfo.NAME; } }

        public string Name { get { return "Zonable Pedestrian Gravel"; } }
        public string DisplayName { get { return "Zonable Pedestrian Gravel Road"; } }
        public string Description { get { return "Gravel roads allow pedestrians to walk fast and easy."; } }
        public string ShortDescription { get { return "Zoneable, No Passenger Vehicles [Traffic++ V2 required]"; } }

        public string ThumbnailsPath { get { return @"Roads\PedestrianRoads\Gravel\thumbnails.png"; } }
        public string InfoTooltipPath { get { return @"Roads\PedestrianRoads\Gravel\infotooltip.png"; } }

        public NetInfoVersion SupportedVersions
        {
            get { return NetInfoVersion.Ground | NetInfoVersion.Elevated; }
        }

        public void BuildUp(NetInfo info, NetInfoVersion version)
        {
            ///////////////////////////
            // 3DModeling            //
            ///////////////////////////
            info.Setup8mNoSwWoodMesh(version);

            ///////////////////////////
            // Texturing             //
            ///////////////////////////
            if (version == NetInfoVersion.Ground)
            {
                info.SetupGroundNakedTextures(version);
            }
            else
            {
                info.SetupElevatedBoardWalkTextures(version);
            }

            info.m_createGravel = true;
            info.m_createPavement = false;
            info.SetupTinyPed(version);

            if (version == NetInfoVersion.Ground)
            {
                info.m_setVehicleFlags = Vehicle.Flags.OnGravel;
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
                        playerNetAI.m_constructionCost = vanillaplayerNetAI.m_constructionCost;
                        playerNetAI.m_maintenanceCost = vanillaplayerNetAI.m_maintenanceCost;
                    }
                }
                break;
            }
        }

        public void LateBuildUp(NetInfo info, NetInfoVersion version)
        {
            info.AddWoodBollards(version);
        }
    }
}
