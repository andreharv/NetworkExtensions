using System.Collections.Generic;
using System.Linq;
using Transit.Addon.RoadExtensions.Menus;
using Transit.Addon.RoadExtensions.Menus.Roads;
using Transit.Addon.RoadExtensions.Roads.Common;
using Transit.Framework;
using Transit.Framework.Builders;
using Transit.Framework.Network;
using UnityEngine;

namespace TransitPlus.Addon.RoadExtensions.Roads.SmallRoads.OneWay1LParkingBicycle
{
    public partial class OneWay1LParkingBicycleBuilder : Activable, INetInfoBuilderPart
    {
        public int Order { get { return 20; } }
        public int UIOrder { get { return 12; } }

        public string BasedPrefabName { get { return NetInfos.Vanilla.ROAD_2L; } }
        public string Name { get { return "One-Lane Oneway with bicycle lanes and parking"; } }
        public string DisplayName { get { return "Small Oneway road with bicycle lanes and parking"; } }
        public string Description { get { return "A one-lane road with parkings spaces and bicycle lanes. Supports low traffic."; } }
        public string ShortDescription { get { return "Zoneable, parking, neighborhood traffic"; } }
        public string UICategory { get { return "RoadsSmall"; } }

        public string ThumbnailsPath    { get { return @"Roads\SmallRoads\OneWay1LParkingBicycle\thumbnails.png"; } }
        public string InfoTooltipPath   { get { return @"Roads\SmallRoads\OneWay1LParkingBicycle\infotooltip.png"; } }

        public NetInfoVersion SupportedVersions
        {
            get { return NetInfoVersion.All; }
        }

        public void BuildUp(NetInfo info, NetInfoVersion version)
        {
            ///////////////////////////
            // Template              //
            ///////////////////////////
            if (version == NetInfoVersion.GroundGrass || version == NetInfoVersion.GroundTrees)
            {
                var roadInfo = Prefabs.Find<NetInfo>(NetInfos.Vanilla.ROAD_2L);
                info.m_segments = roadInfo.m_segments.Select(x => x.ShallowClone()).ToArray();
                info.m_nodes = roadInfo.m_nodes.Select(x => x.ShallowClone()).ToArray();
                info.m_lanes = roadInfo.m_lanes.Select(x => x.ShallowClone()).ToArray();
            }

            ///////////////////////////
            // 3DModeling            //
            ///////////////////////////
            info.Setup16m3mSW3mBikelaneMesh(version, LanesLayoutStyle.AsymL1R3);

            ///////////////////////////
            // Texturing             //
            ///////////////////////////
            SetupTextures(info, version);

            ///////////////////////////
            // Set up                //
            ///////////////////////////
            info.m_hasParkingSpaces = true;

            info.m_pavementWidth = (version != NetInfoVersion.Slope && version != NetInfoVersion.Tunnel ? 2 : 5);
            info.m_halfWidth = (version != NetInfoVersion.Slope && version != NetInfoVersion.Tunnel ? 8 : 11);
            info.m_canCrossLanes = false;
            if (version == NetInfoVersion.Tunnel)
            {
                info.m_setVehicleFlags = Vehicle.Flags.Transition | Vehicle.Flags.Underground;
                info.m_setCitizenFlags = CitizenInstance.Flags.Transition | CitizenInstance.Flags.Underground;
                info.m_class = info.m_class.Clone("NEXTbasicroadmedian" + version.ToString());
                info.m_hasParkingSpaces = false;
            }
            else
            {
                info.m_class = info.m_class.Clone("NEXTbasicroadmedian" + version.ToString());
            }

            // Setting up lanes
            info.SetRoadLanes(version, new LanesConfiguration
            {
                IsTwoWay = false,
                LanesToAdd = 1,
                PedPropOffsetX = 0.5f,
                BusStopOffset = 0f,
                LaneWidth = 3.3f,
                SpeedLimit = 0.8f,

            });
            var leftPedLane = info.GetLeftRoadShoulder();
            var rightPedLane = info.GetRightRoadShoulder();
            var laneWidth = 3;
            var bikeLaneWidth = .8f;
            var vehicleLanes = info.m_lanes.Where(l => l.m_laneType == NetInfo.LaneType.Vehicle).ToList();
            var carLanes = new List<NetInfo.Lane>();
            var bikeLanes = new List<NetInfo.Lane>();
            var parkWidth = 2;
            var parkLanes = info.m_lanes.Where(l => l.m_laneType == NetInfo.LaneType.Parking).ToList();

            bikeLanes.AddRange(vehicleLanes.Take(1));
            carLanes.AddRange(vehicleLanes.Skip(1));
            carLanes[0].m_width = laneWidth;
            carLanes[0].m_position = 0;
            carLanes[0].m_direction = NetInfo.Direction.Forward;
            carLanes[0].m_verticalOffset = -0.2f;
            bikeLanes.AddRange(vehicleLanes.Skip(2));

            for (var i = 0; i < bikeLanes.Count; i++)
            {
                bikeLanes[i].m_vehicleType = VehicleInfo.VehicleType.Bicycle;
                bikeLanes[i].m_position = i == 1 ? 5.2f : -5.2f;
                bikeLanes[i].m_width = bikeLaneWidth;
                bikeLanes[i].m_verticalOffset = 0f;
                bikeLanes[i].m_direction = i == 1 ? NetInfo.Direction.Forward : NetInfo.Direction.Backward;
                bikeLanes[i].m_speedLimit = 0.6f;
                bikeLanes[i].m_verticalOffset = -0.2f;
                bikeLanes[i].m_stopType = VehicleInfo.VehicleType.None;
            
                bikeLanes[i].SetBikeLaneProps();
            }


            for (var i = 0; i < parkLanes.Count; i++)
            {
                parkLanes[i].m_width = parkWidth;
                parkLanes[i].m_verticalOffset = -0.2f;
                parkLanes[i].m_position = i==1? -2.9f:2.9f;
                parkLanes[i].m_direction = NetInfo.Direction.Forward;
                parkLanes[i].m_stopType = VehicleInfo.VehicleType.Car;
            }
            var pedLanes = info.m_lanes.Where(l => l.m_laneType == NetInfo.LaneType.Pedestrian).ToList();
            for (int i = 0; i < pedLanes.Count(); i++)
            {
                pedLanes[i].m_verticalOffset = 0.15f;
                pedLanes[i].m_stopType = VehicleInfo.VehicleType.None;
            }

            //Setting Up Props
            var leftRoadProps = leftPedLane.m_laneProps.m_props.ToList();
            var rightRoadProps = rightPedLane.m_laneProps.m_props.ToList();

            var tempLanes = new List<NetInfo.Lane>();
            tempLanes.AddRange(bikeLanes);
            tempLanes.AddRange(carLanes);
            tempLanes.AddRange(parkLanes);
            tempLanes.AddRange(pedLanes);
            info.m_lanes = tempLanes.ToArray();

            //Setting Up Props

            if (version == NetInfoVersion.Slope)
            {
                leftRoadProps.AddLeftWallLights(info.m_pavementWidth);
                rightRoadProps.AddRightWallLights(info.m_pavementWidth);
            }

            leftPedLane.m_laneProps.m_props = leftRoadProps.ToArray();
            rightPedLane.m_laneProps.m_props = rightRoadProps.ToArray();

            info.TrimAboveGroundProps(version);
            info.SetupNewSpeedLimitProps(50, 40);

            /*
            // AI
            var owPlayerNetAI = owRoadInfo.GetComponent<PlayerNetAI>();
            var playerNetAI = info.GetComponent<PlayerNetAI>();

            if (owPlayerNetAI != null && playerNetAI != null)
            {
                playerNetAI.m_constructionCost = owPlayerNetAI.m_constructionCost * 2; // Charge by the lane?
                playerNetAI.m_maintenanceCost = owPlayerNetAI.m_maintenanceCost * 2; // Charge by the lane?
            }
            */
            // TODO: make it configurable
            var roadBaseAI = info.GetComponent<RoadBaseAI>();
            if (roadBaseAI != null)
            {
                roadBaseAI.m_trafficLights = false;
            }
        }
    }
}
