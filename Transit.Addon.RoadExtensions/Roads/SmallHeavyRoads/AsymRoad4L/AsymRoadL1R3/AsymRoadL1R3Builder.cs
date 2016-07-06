using System.Linq;
using Transit.Addon.RoadExtensions.Menus;
using Transit.Addon.RoadExtensions.Menus.Roads;
using Transit.Addon.RoadExtensions.Roads.Common;
using Transit.Framework;
using Transit.Framework.Builders;
using Transit.Framework.Network;

namespace Transit.Addon.RoadExtensions.Roads.SmallHeavyRoads.AsymRoad4L.AsymRoadL1R3
{
    public partial class AsymRoadL1R3Builder : Activable, INetInfoBuilderPart
    {
        public int Order { get { return 10; } }
        public int UIOrder { get { return 60; } }

        public string BasedPrefabName { get { return NetInfos.Vanilla.ROAD_2L; } }
        public string Name { get { return "AsymRoadL1R3"; } }
        public string DisplayName { get { return "Asymmetrical Road: 1 Left / 3 Right"; } }
        public string Description { get { return "An asymmetrical road with one left lane and three right lanes.  Note, dragging this road backwards reverses its orientation."; } }
        public string ShortDescription { get { return "No parking, zoneable, low to medium traffic"; } }
        public string UICategory { get { return RExExtendedMenus.ROADS_SMALL_HV; } }

        public string ThumbnailsPath { get { return @"Roads\SmallHeavyRoads\AsymRoad4L\AsymRoadL1R3\thumbnails.png"; } }
        public string InfoTooltipPath { get { return @"Roads\SmallHeavyRoads\AsymRoad4L\AsymRoadL1R3\infotooltip.png"; } }

        public NetInfoVersion SupportedVersions
        {
            get { return NetInfoVersion.All; }
        }

        public void BuildUp(NetInfo info, NetInfoVersion version)
        {
            ///////////////////////////
            // Template              //
            ///////////////////////////
            var owRoadInfo = Prefabs.Find<NetInfo>(NetInfos.Vanilla.ROAD_2L);
            var owRoadTunnelInfo = Prefabs.Find<NetInfo>(NetInfos.Vanilla.ONEWAY_2L_TUNNEL);
            var asymLaneType = LanesLayoutStyle.AsymL1R3;
            ///////////////////////////
            // 3DModeling            //
            ///////////////////////////
            info.Setup16m2mSWMesh(version, asymLaneType);

            ///////////////////////////
            // Texturing             //
            ///////////////////////////
            info.SetupTextures(version, asymLaneType);

            ///////////////////////////
            // Set up                //
            ///////////////////////////
            info.m_hasParkingSpaces = false;
            info.m_pavementWidth = (version != NetInfoVersion.Slope && version != NetInfoVersion.Tunnel ? 2 : 5);
            info.m_halfWidth = (version != NetInfoVersion.Slope && version != NetInfoVersion.Tunnel ? 8 : 11);

            if (version == NetInfoVersion.Tunnel)
            {
                info.m_setVehicleFlags = Vehicle.Flags.Transition;
                info.m_setCitizenFlags = CitizenInstance.Flags.Transition;
                info.m_class = owRoadTunnelInfo.m_class.Clone(NetInfoClasses.NEXT_SMALL4L_ROAD_TUNNEL);
            }
            else
            {
                info.m_class = owRoadInfo.m_class.Clone(NetInfoClasses.NEXT_SMALL4L_ROAD);
            }

            // Setting up lanes
            info.SetRoadLanes(version, new LanesConfiguration
            {
                IsTwoWay = true,
                LanesToAdd = 2,
                PedPropOffsetX = 0.5f,
                BusStopOffset = 0f,
                SpeedLimit = 1.0f,
                LayoutStyle = asymLaneType
            });
            var leftPedLane = info.GetLeftRoadShoulder();
            var rightPedLane = info.GetRightRoadShoulder();

            // Fix for T++ legacy support
            var lanes = info.m_lanes.OrderBy(l => l.m_position).ToArray();
            var lanesLegacyOrder = new[]
            {
                lanes[0],
                lanes[5],
                lanes[1],
                lanes[4],
                lanes[2],
                lanes[3]
            };

            info.m_lanes = lanesLegacyOrder;

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

            // AI
            var owPlayerNetAI = owRoadInfo.GetComponent<PlayerNetAI>();
            var playerNetAI = info.GetComponent<PlayerNetAI>();

            if (owPlayerNetAI != null && playerNetAI != null)
            {
                playerNetAI.m_constructionCost = owPlayerNetAI.m_constructionCost * 2; // Charge by the lane?
                playerNetAI.m_maintenanceCost = owPlayerNetAI.m_maintenanceCost * 2; // Charge by the lane?
            }

            // TODO: make it configurable
            var roadBaseAI = info.GetComponent<RoadBaseAI>();
            if (roadBaseAI != null)
            {
                roadBaseAI.m_trafficLights = false;
            }
        }
    }
}
