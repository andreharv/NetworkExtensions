using System;
using Transit.Addon.RoadExtensions.Roads.Common;
using Transit.Addon.RoadExtensions.Roads.PedestrianRoads.Common;
using Transit.Addon.RoadExtensions.UI.Toolbar.Roads;
using Transit.Framework;
using Transit.Framework.Builders;
using Transit.Framework.Network;

namespace Transit.Addon.RoadExtensions.Roads.PedestrianRoads.Pavement
{
    public class ZonablePedestrianPavedRoadBuilder : Activable, INetInfoBuilderPart, INetInfoLateBuilder, INetInfoSpecificNameBuilder
    {
        public int Order { get { return 315; } }
        public int UIOrder { get { return 20; } }

        public string BasedPrefabName { get { return NetInfos.Vanilla.ROAD_2L; } }
        public string UICategory { get { return PedestriansRoadsCategoryInfo.NAME; } }

        public string Name { get { return "Zonable Pedestrian Pavement"; } }
        public string DisplayName { get { return "Zonable Pedestrian Paved Road"; } }
        public string Description { get { return "Paved roads are nicer to walk on than gravel."; } }
        public string ShortDescription { get { return "Zoneable, No Passenger Vehicles [Traffic++ V2 required]"; } }

        public string ThumbnailsPath { get { return @"Roads\PedestrianRoads\Pavement\thumbnails.png"; } }
        public string InfoTooltipPath { get { return @"Roads\PedestrianRoads\Pavement\infotooltip.png"; } }

        public NetInfoVersion SupportedVersions
        {
            get { return NetInfoVersion.Ground | NetInfoVersion.Elevated; }
        }

        public string GetSpecificBuiltPrefabName(NetInfoVersion version)
        {
            switch (version)
            {
                case NetInfoVersion.Ground:
                    return "Zonable Pedestrian Pavement";
                case NetInfoVersion.Elevated:
                    return "Zonable Pedestrian Elevated"; // T++ Legacy name
                default:
                    throw new ArgumentOutOfRangeException(nameof(version), version, null);
            }
        }

        public void BuildUp(NetInfo info, NetInfoVersion version)
        {
            ///////////////////////////
            // 3DModeling            //
            ///////////////////////////
            info.Setup8mNoSWMesh(version);

            ///////////////////////////
            // Texturing             //
            ///////////////////////////
            if (version == NetInfoVersion.Ground)
            {
                info.SetupGroundNakedTextures(version);
            }
            else
            {
                info.SetupElevatedPavedTextures(version);
            }

            ///////////////////////////
            // Set up                //
            ///////////////////////////
            info.m_createGravel = false;
            info.m_createPavement = true;
            info.SetupTinyPed(version);

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
                            playerNetAI.m_constructionCost = vanillaplayerNetAI.m_constructionCost * 3 / 2;
                            playerNetAI.m_maintenanceCost = vanillaplayerNetAI.m_maintenanceCost * 3 / 2;
                        }
                    }
                    break;
            }
        }

        public void LateBuildUp(NetInfo info, NetInfoVersion version)
        {
            info.AddRetractBollard(version);
        }
    }
}
