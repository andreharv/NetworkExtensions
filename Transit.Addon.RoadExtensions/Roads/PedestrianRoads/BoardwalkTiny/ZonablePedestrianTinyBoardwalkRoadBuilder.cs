using System.Collections.Generic;
using Transit.Addon.RoadExtensions.Menus.Roads;
using Transit.Addon.RoadExtensions.Roads.Common;
using Transit.Addon.RoadExtensions.Roads.PedestrianRoads.Common;
using Transit.Framework;
using Transit.Framework.Builders;
using Transit.Framework.Network;

namespace Transit.Addon.RoadExtensions.Roads.PedestrianRoads.BoardwalkTiny
{
    public partial class ZonablePedestrianBoardwalkRoadBuilder : Activable, INetInfoBuilderPart, INetInfoLateBuilder
    {
        public int Order { get { return 300; } }
        public int UIOrder { get { return 5; } }

        public string BasedPrefabName { get { return NetInfos.Vanilla.ROAD_2L; } }
        public string UICategory { get { return RExExtendedMenus.ROADS_PEDESTRIANS; } }

        public const string NAME = "Zonable Pedestrian Boardwalk Tiny";
        public string Name { get { return NAME; } }
        public string DisplayName { get { return "Zonable Pedestrian Boardwalk Tiny Road"; } }
        public string Description { get { return "Boardwalks for mixed traffic.  Great for beaches and shopping districts."; } }
        public string ShortDescription { get { return "Zoneable, No Passenger Vehicles [Traffic++ V2 required]"; } }

        public string ThumbnailsPath { get { return @"Roads\PedestrianRoads\BoardwalkTiny\thumbnails.png"; } }
        public string InfoTooltipPath { get { return @"Roads\PedestrianRoads\GravelTiny\infotooltip.png"; } }

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
            info.SetupBoardWalkTextures(version);

            ///////////////////////////
            // Set up                //
            ///////////////////////////
            info.m_createGravel = false;
            info.m_createPavement = true;
            info.m_connectGroup = NetInfo.ConnectGroup.CenterTram;
            info.m_nodeConnectGroups = NetInfo.ConnectGroup.CenterTram;
            info.m_surfaceLevel = 0.2f;
            info.SetupTinyPed(version);
            var lanes = new List<NetInfo.Lane>();
            lanes.AddRange(info.m_lanes);
            for(int i = 0; i < info.m_lanes.Length; i++)
            {
                lanes[i].m_verticalOffset = 0.2f;
            }
            info.m_lanes = lanes.ToArray();
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