using System.Linq;
using Transit.Addon.RoadExtensions.Menus;
using Transit.Addon.RoadExtensions.Roads.Common;
using Transit.Addon.RoadExtensions.SmallHeavyRoads.Common;
using Transit.Framework;
using Transit.Framework.Builders;

namespace Transit.Addon.RoadExtensions.SmallHeavyRoads.BasicRoadTL
{
    public partial class BasicRoadTLBuilder : Activable, INetInfoBuilderPart
    {
        public int Order { get { return 7; } }
        public int UIOrder { get { return 9; } }

        public string BasedPrefabName { get { return NetInfos.Vanilla.ROAD_2L; } }
        public string Name { get { return "BasicRoadTL"; } }
        public string DisplayName { get { return "Basic Road with Turning Lane"; } }
        public string Description { get { return "A basic two lane road with an additional center turning lane and no parkings spaces. Supports medium and local traffic. Note: The turning lane goes in both direction so collisions may occur!"; } }
        public string ShortDescription { get { return "No parking, zoneable, low traffic"; } }
        public string UICategory { get { return AdditionnalMenus.ROADS_SMALL_HV; } }

        public string ThumbnailsPath { get { return @"SmallHeavyRoads\BasicRoadTL\thumbnails.png"; } }
        public string InfoTooltipPath { get { return @"SmallHeavyRoads\BasicRoadTL\infotooltip.png"; } }

        public NetInfoVersion SupportedVersions
        {
            get { return NetInfoVersion.All; }
        }

        public void BuildUp(NetInfo info, NetInfoVersion version)
        {
            ///////////////////////////
            // Template              //
            ///////////////////////////
            var roadInfo = Prefabs.Find<NetInfo>(NetInfos.Vanilla.ROAD_2L);
            var owRoadTunnelInfo = Prefabs.Find<NetInfo>(NetInfos.Vanilla.ONEWAY_2L_TUNNEL);

            ///////////////////////////
            // 3DModeling            //
            ///////////////////////////
            info.Setup16m3mSWMesh(version);

            ///////////////////////////
            // Texturing             //
            ///////////////////////////
            SetupTextures(info, version);
            
            ///////////////////////////
            // Set up                //
            ///////////////////////////
            info.m_hasParkingSpaces = false;
            info.m_class = roadInfo.m_class.Clone(NetInfoClasses.NEXT_SMALL3L_ROAD);
            info.m_pavementWidth = (version != NetInfoVersion.Slope && version != NetInfoVersion.Tunnel ? 3 : 6);
            info.m_halfWidth = (version != NetInfoVersion.Slope && version != NetInfoVersion.Tunnel ? 8 : 11);

            if (version == NetInfoVersion.Tunnel)
            {
                info.m_setVehicleFlags = Vehicle.Flags.Transition;
                info.m_setCitizenFlags = CitizenInstance.Flags.Transition;
                info.m_class = owRoadTunnelInfo.m_class.Clone(NetInfoClasses.NEXT_SMALL3L_ROAD_TUNNEL);
            }
            else
            {
                info.m_class = roadInfo.m_class.Clone(NetInfoClasses.NEXT_SMALL3L_ROAD);
            }

            // Setting up lanes
            info.SetRoadLanes(version, 2, 0, 1.2f, true, true, 1.5f);
            var leftPedLane = info.GetLeftRoadShoulder(roadInfo, version);
            var rightPedLane = info.GetRightRoadShoulder(roadInfo, version);
            //Setting Up Props
            var leftRoadProps = leftPedLane.m_laneProps.m_props.ToList();
            var rightRoadProps = rightPedLane.m_laneProps.m_props.ToList();

            if (version == NetInfoVersion.Slope)
            {
                leftRoadProps.AddLeftWallLights(info.m_pavementWidth);
                rightRoadProps.AddRightWallLights(info.m_pavementWidth);
            }
            
            leftPedLane.m_laneProps.m_props = leftRoadProps.ToArray();
            rightPedLane.m_laneProps.m_props = rightRoadProps.ToArray();

            info.TrimAboveGroundProps(version);
            info.SetupNewSpeedLimitProps(50, 40);

            
            //var propLanes = info.m_lanes.Where(l => l.m_laneProps != null && (l.m_laneProps.name.ToLower().Contains("left") || l.m_laneProps.name.ToLower().Contains("right"))).ToList();

            var owPlayerNetAI = roadInfo.GetComponent<PlayerNetAI>();
            var playerNetAI = info.GetComponent<PlayerNetAI>();

            if (owPlayerNetAI != null && playerNetAI != null)
            {
                playerNetAI.m_constructionCost = owPlayerNetAI.m_constructionCost * 3 / 2; // Charge by the lane?
                playerNetAI.m_maintenanceCost = owPlayerNetAI.m_maintenanceCost * 3 / 2; // Charge by the lane?
            }

            var roadBaseAI = info.GetComponent<RoadBaseAI>();
            if (roadBaseAI != null)
            {
                roadBaseAI.m_trafficLights = false;
            }
        }
    }
}
