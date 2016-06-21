using System.Linq;
using Transit.Addon.RoadExtensions.Menus;
using Transit.Addon.RoadExtensions.Menus.Roads;
using Transit.Addon.RoadExtensions.Roads.Common;
using Transit.Framework;
using Transit.Framework.Builders;
using Transit.Framework.Network;

namespace Transit.Addon.RoadExtensions.Roads.Avenues.AsymAvenue5L.AsymAvenueL3R2
{
    public partial class AsymAvenueL3R2Builder : Activable, INetInfoBuilderPart
    {
        public int Order { get { return 10; } }
        public int UIOrder { get { return 60; } }

        public string BasedPrefabName { get { return NetInfos.Vanilla.ROAD_6L; } }
        public string Name { get { return "AsymAvenueL3R2"; } }
        public string DisplayName { get { return "Asymmetrical Road: 3 Left / 2 Right"; } }
        public string Description { get { return "An asymmetrical road with three left lane and two right lanes.  Note, dragging this road backwards reverses its orientation."; } }
        public string ShortDescription { get { return "Parking, zoneable, medium to high traffic"; } }
        public string UICategory { get { return "RoadsMedium"; } }

        public string ThumbnailsPath { get { return @"Roads\Avenues\AsymAvenue5L\AsymAvenueL3R2\thumbnails.png"; } }
        public string InfoTooltipPath { get { return @"Roads\Avenues\AsymAvenue5L\AsymAvenueL3R2\infotooltip.png"; } }

        public NetInfoVersion SupportedVersions
        {
            get { return NetInfoVersion.All; }
        }

        public void BuildUp(NetInfo info, NetInfoVersion version)
        {
            ///////////////////////////
            // Template              //
            ///////////////////////////
            var owRoadInfo = Prefabs.Find<NetInfo>(NetInfos.Vanilla.ROAD_6L);
            var owRoadTunnelInfo = Prefabs.Find<NetInfo>(NetInfos.Vanilla.ROAD_6L_TUNNEL);
            var asymLaneType = AsymLaneType.L3R2;
            ///////////////////////////
            // 3DModeling            //
            ///////////////////////////
            info.Setup32m5mSW3mMdn(version, asymLaneType);

            ///////////////////////////
            // Texturing             //
            ///////////////////////////
            info.SetupTextures(version, asymLaneType);

            ///////////////////////////
            // Set up                //
            ///////////////////////////
            info.m_hasParkingSpaces = true;
            info.m_pavementWidth = (version != NetInfoVersion.Slope && version != NetInfoVersion.Tunnel ? 5 : 7);
            info.m_halfWidth = (version != NetInfoVersion.Elevated && version != NetInfoVersion.Bridge ? 16 : 14);
            info.m_canCrossLanes = false;
            if (version == NetInfoVersion.Tunnel)
            {
                info.m_setVehicleFlags = Vehicle.Flags.Transition | Vehicle.Flags.Underground;
                info.m_setCitizenFlags = CitizenInstance.Flags.Transition | CitizenInstance.Flags.Underground;
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
                PedPropOffsetX = 0.5f,
                BusStopOffset = 3,
                AsymLT = asymLaneType
            });
            var keepLanes = info.m_lanes.Where(l => l.m_position != -1.5).ToArray();
            info.m_lanes = keepLanes;
            var leftPedLane = info.GetLeftRoadShoulder();
            var rightPedLane = info.GetRightRoadShoulder();

            // Fix for T++ legacy support
            //var lanes = info.m_lanes.OrderBy(l => l.m_position).ToArray();
            //var lanesLegacyOrder = new[]
            //{
            //    lanes[0],
            //    lanes[5],
            //    lanes[1],
            //    lanes[4],
            //    lanes[2],
            //    lanes[3]
            //};

            //info.m_lanes = lanesLegacyOrder;

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
                playerNetAI.m_constructionCost = owPlayerNetAI.m_constructionCost * 5 / 6; // Charge by the lane?
                playerNetAI.m_maintenanceCost = owPlayerNetAI.m_maintenanceCost * 5 / 6; // Charge by the lane?
            }

            // TODO: make it configurable
            var roadBaseAI = info.GetComponent<RoadBaseAI>();
            if (roadBaseAI != null)
            {
                roadBaseAI.m_trafficLights = true;
            }
        }
    }
}
