using System.Linq;
using System.Collections.Generic;
using Transit.Addon.RoadExtensions.Menus;
using Transit.Addon.RoadExtensions.Menus.Roads;
using Transit.Addon.RoadExtensions.Roads.Common;
using Transit.Framework;
using Transit.Framework.Builders;
using Transit.Framework.Network;

namespace Transit.Addon.RoadExtensions.Roads.SmallHeavyRoads.OneWay3LParking
{
    public partial class OneWay3LParkingBuilder : Activable, INetInfoBuilderPart
    {
        public int Order { get { return 20; } }
        public int UIOrder { get { return 20; } }

        public string BasedPrefabName { get { return NetInfos.Vanilla.ONEWAY_2L; } }
        public string Name { get { return "OneWay3LParking"; } }
        public string DisplayName { get { return "Three-Lane Oneway with Parking"; } }
        public string Description { get { return "A three-lane one-way road with parkings spaces. Supports medium traffic."; } }
        public string ShortDescription { get { return "Zoneable, medium traffic"; } }
        public string UICategory { get { return RExExtendedMenus.ROADS_SMALL_HV; } }

        public string ThumbnailsPath    { get { return @"Roads\SmallHeavyRoads\OneWay3LParking\thumbnails.png"; } }
        public string InfoTooltipPath   { get { return @"Roads\SmallHeavyRoads\OneWay3LParking\infotooltip.png"; } }

        public NetInfoVersion SupportedVersions
        {
            get { return NetInfoVersion.All; }
        }

        public void BuildUp(NetInfo info, NetInfoVersion version)
        {
            ///////////////////////////
            // Template              //
            ///////////////////////////
            var owRoadInfo = Prefabs.Find<NetInfo>(NetInfos.Vanilla.ONEWAY_2L);
            var owRoadTunnelInfo = Prefabs.Find<NetInfo>(NetInfos.Vanilla.ONEWAY_2L_TUNNEL);

            ///////////////////////////
            // 3DModeling            //
            ///////////////////////////

            if (version == NetInfoVersion.Ground)
            {
                info.Setup16m2mSWMesh(version, LanesLayoutStyle.AsymL1R2);
            }else
            {
                info.Setup16m3mSWMesh(version);
            }
            ///////////////////////////
            // Texturing             //
            ///////////////////////////
            SetupTextures(info, version);

            ///////////////////////////
            // Set up                //
            ///////////////////////////
            info.m_hasParkingSpaces = true;
            if (version == NetInfoVersion.Ground)
            {
                info.m_pavementWidth = (version != NetInfoVersion.Slope && version != NetInfoVersion.Tunnel ? 2 : 5);
                info.m_halfWidth = (version != NetInfoVersion.Slope && version != NetInfoVersion.Tunnel ? 8 : 11);
            }
            else
            {
                info.m_pavementWidth = (version != NetInfoVersion.Slope && version != NetInfoVersion.Tunnel ? 3 : 6);
                info.m_halfWidth = (version != NetInfoVersion.Slope && version != NetInfoVersion.Tunnel ? 8 : 11);
            }
            if (version == NetInfoVersion.Tunnel)
            {
                info.m_setVehicleFlags = Vehicle.Flags.Transition | Vehicle.Flags.Underground;
                info.m_setCitizenFlags = CitizenInstance.Flags.Transition | CitizenInstance.Flags.Underground;
                info.m_class = owRoadTunnelInfo.m_class.Clone(NetInfoClasses.NEXT_SMALL3L_ROAD_TUNNEL);
            }
            else
            {
                info.m_class = owRoadInfo.m_class.Clone(NetInfoClasses.NEXT_SMALL4L_ROAD);
            }
            var LaneWidth = 3.0f;
            // Setting up lanes
            info.SetRoadLanes(version, new LanesConfiguration
            {
                IsTwoWay = false,
                LanesToAdd = 1,
                PedPropOffsetX = 0.5f,
                BusStopOffset = 3f,
                SpeedLimit = 1.2f,
                LaneWidth = LaneWidth
            });
            var carLanes = info.m_lanes.Where(l => l.m_laneType == NetInfo.LaneType.Vehicle).ToList();
            var pedkLanes = info.m_lanes.Where(l => l.m_laneType == NetInfo.LaneType.Pedestrian).ToList();
            var parkLane =  info.m_lanes.Where(l => l.m_laneType == NetInfo.LaneType.Parking).ToList();

            if (version == NetInfoVersion.Ground)
            {
                for (var i = 0; i < carLanes.Count; i++)
                {
                    carLanes[i].m_position = i * LaneWidth - LaneWidth * 1.5f;
                    carLanes[1].m_stopType = VehicleInfo.VehicleType.None;
                  
                }
                for (var i = 0; i < parkLane.Count; i++)
                {
                    parkLane[i].m_stopType = VehicleInfo.VehicleType.None;
                }
           
                    pedkLanes[1].m_stopType = VehicleInfo.VehicleType.Car;
              
            }
            var tempLanes = new List<NetInfo.Lane>();
            tempLanes.AddRange(pedkLanes);
            tempLanes.AddRange(carLanes);
            tempLanes.AddRange(parkLane.Skip(1).Take(1));
            info.m_lanes = tempLanes.ToArray();

            var leftPedLane = info.GetLeftRoadShoulder();
            var rightPedLane = info.GetRightRoadShoulder();

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
            info.SetupNewSpeedLimitProps(60, 40);


            // AI
            var owPlayerNetAI = owRoadInfo.GetComponent<PlayerNetAI>();
            var playerNetAI = info.GetComponent<PlayerNetAI>();

            if (owPlayerNetAI != null && playerNetAI != null)
            {
                playerNetAI.m_constructionCost = owPlayerNetAI.m_constructionCost * 2; // Charge by the lane?
                playerNetAI.m_maintenanceCost = owPlayerNetAI.m_maintenanceCost * 2; // Charge by the lane?
            }

            var roadBaseAI = info.GetComponent<RoadBaseAI>();

            if (roadBaseAI != null)
            {
                roadBaseAI.m_trafficLights = true;
            }
        }
    }
}
