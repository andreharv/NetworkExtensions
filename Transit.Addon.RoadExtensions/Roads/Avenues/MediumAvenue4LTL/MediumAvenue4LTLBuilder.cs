using System.Linq;
using Transit.Addon.RoadExtensions.Props;
using Transit.Addon.RoadExtensions.Roads.Common;
using Transit.Framework;
using Transit.Framework.Builders;
using Transit.Framework.Network;

namespace Transit.Addon.RoadExtensions.Roads.Avenues.MediumAvenue4LTL
{
    public partial class MediumAvenue4LTLBuilder : Activable, INetInfoBuilderPart
    {
        public int Order { get { return 21; } }
        public int UIOrder { get { return 5; } }

        public string BasedPrefabName { get { return NetInfos.Vanilla.ROAD_6L; } }
        public string Name { get { return "Medium Avenue TL"; } }
        public string DisplayName { get { return "Four-Lane Road with Turning Lane"; } }
        public string Description { get { return "A four-lane road with turning lanes and parking spaces. Supports medium traffic. Note: The turning lane goes in both directions so collisions may occur!"; } }
        public string ShortDescription { get { return "Parkings, zoneable, medium traffic; turning lane works both ways and could cause collisions"; } }
        public string UICategory { get { return "RoadsMedium"; } }

        public string ThumbnailsPath { get { return @"Roads\Avenues\MediumAvenue4LTL\thumbnails.png"; } }
        public string InfoTooltipPath { get { return @"Roads\Avenues\MediumAvenue4LTL\infotooltip.png"; } }

        public NetInfoVersion SupportedVersions
        {
            get { return NetInfoVersion.All; }
        }

        public void BuildUp(NetInfo info, NetInfoVersion version)
        {
            ///////////////////////////
            // Template              //
            ///////////////////////////
            var roadInfo = Prefabs.Find<NetInfo>(NetInfos.Vanilla.ROAD_6L);
            var roadTunnelInfo = Prefabs.Find<NetInfo>(NetInfos.Vanilla.ROAD_6L_TUNNEL);
            var bridgeInfo = Prefabs.Find<NetInfo>(NetInfos.Vanilla.ROAD_4L_BRIDGE).Clone("temp");

            ///////////////////////////
            // 3DModeling            //
            ///////////////////////////
            info.Setup32m5mSWMesh(version);

            ///////////////////////////
            // Texturing             //
            ///////////////////////////
            SetupTextures(info, version);

            ///////////////////////////
            // Set up                //
            ///////////////////////////
            info.m_hasParkingSpaces = true;
            info.m_pavementWidth = (version != NetInfoVersion.Slope && version != NetInfoVersion.Tunnel ? 5 : 7);
            info.m_halfWidth = (version == NetInfoVersion.Bridge || version == NetInfoVersion.Elevated ? 14 : 16);

            if (version == NetInfoVersion.Tunnel)
            {
                info.m_setVehicleFlags = Vehicle.Flags.Transition | Vehicle.Flags.Underground;
                info.m_setCitizenFlags = CitizenInstance.Flags.Transition | CitizenInstance.Flags.Underground;
                info.m_class = roadTunnelInfo.m_class.Clone(NetInfoClasses.NEXT_MEDIUM_ROAD_TL_TUNNEL);
            }
            else if (version == NetInfoVersion.Bridge)
            {
                info.m_class = bridgeInfo.m_class.Clone(NetInfoClasses.NEXT_MEDIUM_ROAD_TL);
            }
            else
            {
                info.m_class = roadInfo.m_class.Clone(NetInfoClasses.NEXT_MEDIUM_ROAD_TL);
            }
			
            // Setting up lanes
            info.SetRoadLanes(version, new LanesConfiguration
            {
                IsTwoWay = true,
                LaneWidth = 3.8f,
                BusStopOffset = 2.9f,
                CenterLane = CenterLaneType.TurningLane,
                CenterLaneWidth = 3.8f
            });
            var leftPedLane = info.GetLeftRoadShoulder();
            var rightPedLane = info.GetRightRoadShoulder();

            // Fix for T++ legacy support
            if (version == NetInfoVersion.Ground)
            {
                var lanes = info.m_lanes.OrderBy(l => l.m_position).ToArray();
                var lanesLegacyOrder = new[]
                {
                    lanes[0],
                    lanes[9],
                    lanes[1],
                    lanes[8],
                    lanes[2],
                    lanes[7],
                    lanes[3],
                    lanes[6],
                    lanes[4],
                    lanes[5]
                };

                info.m_lanes = lanesLegacyOrder;
            }

            //Setting Up Props
            var leftRoadProps = leftPedLane.m_laneProps.m_props.ToList();
            var rightRoadProps = rightPedLane.m_laneProps.m_props.ToList();

            if (version != NetInfoVersion.Tunnel)
            {
                var leftStreetLight = leftRoadProps.FirstOrDefault(p => p.m_prop.name.ToLower().Contains("new street light"));
                if (leftStreetLight != null)
                {
                    leftStreetLight.m_finalProp =
                    leftStreetLight.m_prop = Prefabs.Find<PropInfo>(MediumAvenueSideLightBuilder.NAME);
                }

                var rightStreetLight = rightRoadProps.FirstOrDefault(p => p.m_prop.name.ToLower().Contains("new street light"));
                if (rightStreetLight != null)
                {
                    rightStreetLight.m_finalProp =
                    rightStreetLight.m_prop = Prefabs.Find<PropInfo>(MediumAvenueSideLightBuilder.NAME);
                }
            }

            if (version == NetInfoVersion.Slope)
            {
                leftRoadProps.AddLeftWallLights(info.m_pavementWidth);
                rightRoadProps.AddRightWallLights(info.m_pavementWidth);
            }

            leftPedLane.m_laneProps.m_props = leftRoadProps.ToArray();
            rightPedLane.m_laneProps.m_props = rightRoadProps.ToArray();

            info.TrimAboveGroundProps(version);
            info.SetupNewSpeedLimitProps(50, 60);


            // AI
            var owPlayerNetAI = roadInfo.GetComponent<PlayerNetAI>();
            var playerNetAI = info.GetComponent<PlayerNetAI>();

            if (owPlayerNetAI != null && playerNetAI != null)
            {
                playerNetAI.m_constructionCost = owPlayerNetAI.m_constructionCost * 5 / 6; // Charge by the lane?
                playerNetAI.m_maintenanceCost = owPlayerNetAI.m_maintenanceCost * 5 / 6; // Charge by the lane?
            }

            var roadBaseAI = info.GetComponent<RoadBaseAI>();
            if (roadBaseAI != null)
            {
                roadBaseAI.m_trafficLights = false;
            }
        }
    }
}
