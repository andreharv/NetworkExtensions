using System.Linq;
using Transit.Framework;
using Transit.Framework.Builders;
using UnityEngine;

namespace Transit.Addon.RoadExtensions.Roads.Highway1L
{
    public partial class Highway1LBuilder : Activable, INetInfoBuilderPart
    {
        public int Order { get { return 30; } }
        public int UIOrder { get { return 9; } }

        public string BasedPrefabName { get { return NetInfos.Vanilla.ROAD_2L; } }
        public string Name { get { return "Small Rural Highway"; } }
        public string DisplayName { get { return "National Road"; } }
        public string Description { get { return "A two-lane, two-way road suitable for low traffic between areas. National Road does not allow zoning next to it!"; } }
        public string UICategory { get { return "RoadsHighway"; } }

        public string ThumbnailsPath { get { return @"Roads\Highway1L\thumbnails.png"; } }
        public string InfoTooltipPath { get { return @"Roads\Highway1L\infotooltip.png"; } }

        public NetInfoVersion SupportedVersions
        {
            get { return NetInfoVersion.All; }
        }

        public void BuildUp(NetInfo info, NetInfoVersion version)
        {
            ///////////////////////////
            // Template              //
            ///////////////////////////
            var highwayInfo = Prefabs.Find<NetInfo>(NetInfos.Vanilla.HIGHWAY_3L);
            var basicRoadInfo = Prefabs.Find<NetInfo>(NetInfos.Vanilla.ROAD_2L);


            ///////////////////////////
            // 3DModeling            //
            ///////////////////////////
            SetupModels(info, version);
            

            ///////////////////////////
            // Texturing             //
            ///////////////////////////
            SetupTextures(info, version);


            ///////////////////////////
            // Set up                //
            ///////////////////////////
            info.m_availableIn = ItemClass.Availability.All;
            info.m_class = highwayInfo.m_class.Clone(NetInfoClasses.NEXT_HIGHWAY1L);
            info.m_surfaceLevel = 0;
            info.m_createPavement = version != NetInfoVersion.Ground && version != NetInfoVersion.Tunnel;
            info.m_createGravel = version == NetInfoVersion.Ground;
            info.m_averageVehicleLaneSpeed = 2f;
            info.m_hasParkingSpaces = false;
            info.m_hasPedestrianLanes = false;
            info.m_halfWidth = 8;
            info.m_UnlockMilestone = highwayInfo.m_UnlockMilestone;
            info.m_pavementWidth = 2;
            if (version == NetInfoVersion.Tunnel)
            {
                info.m_setVehicleFlags = Vehicle.Flags.Transition;
            }


            ///////////////////////////
            // Set up lanes          //
            ///////////////////////////
            // Disabling Parkings and Peds Lanes
            foreach (var lane in info.m_lanes)
            {
                switch (lane.m_laneType)
                {
                    case NetInfo.LaneType.Parking:
                        lane.m_laneType = NetInfo.LaneType.None;
                        break;
                    case NetInfo.LaneType.Pedestrian:
                        lane.m_laneType = NetInfo.LaneType.None;
                        break;
                }
            }

            // Setting up Lanes
            var vehicleLanes = info.m_lanes
                .Where(l => l.m_laneType != NetInfo.LaneType.None)
                .OrderBy(l => l.m_similarLaneIndex)
                .ToArray();

            var nbLanes = vehicleLanes.Count();

            const float laneWidth = 4f;
            var positionStart = (laneWidth * ((1f - nbLanes) / 2f));

            for (var i = 0; i < vehicleLanes.Length; i++)
            {
                var l = vehicleLanes[i];
                l.m_allowStop = false;
                l.m_speedLimit = 2f;
                l.m_verticalOffset = 0f;
                l.m_width = laneWidth;
                l.m_position = positionStart + i * laneWidth;
            }

            info.SetHighwayProps(highwayInfo);
            info.TrimHighwayProps();


            ///////////////////////////
            // Set up props          //
            ///////////////////////////
            NetInfo.Lane leftHwLane = null;
            NetInfo.Lane rightHwLane = null;
            if (version == NetInfoVersion.Tunnel)
            {
                var counter = 0;
                foreach (var lane in info.m_lanes)
                {
                    if (lane.m_laneType == NetInfo.LaneType.None && counter == 0)
                    {
                        counter++;
                        leftHwLane = lane;
                        leftHwLane.m_width = info.m_pavementWidth - 1;
                        leftHwLane.m_position = (info.m_halfWidth * -1) + (leftHwLane.m_width * 0.5f + 1);
                    }
                    else if (lane.m_laneType == NetInfo.LaneType.None && counter == 1)
                    {
                        rightHwLane = lane;
                        rightHwLane.m_width = info.m_pavementWidth - 1;
                        rightHwLane.m_position = (info.m_halfWidth) - (rightHwLane.m_width * 0.5f + 1);
                    }
                    else
                    {
                        lane.m_laneProps = highwayInfo.FindLane(NetInfo.LaneType.Vehicle).m_laneProps.ShallowClone();
                    }
                }

                if (leftHwLane != null)
                {
                    leftHwLane.m_laneProps = highwayInfo
                        .FindLane(name => name.Contains("left"))
                        .m_laneProps
                        .Clone("Highway1L Left Props");
                }

                if (rightHwLane != null)
                {
                    rightHwLane.m_laneProps = highwayInfo
                        .FindLane(name => name.Contains("right"))
                        .m_laneProps
                        .Clone("Highway1L Right Props");
                }
            }
            else
            {
                leftHwLane = info
                    .FindLane(name => name.Contains("left"))
                    .ShallowClone();

                rightHwLane = info
                    .FindLane(name => name.Contains("right"))
                    .ShallowClone();
            }

            if (leftHwLane != null && rightHwLane != null)
            {
                var leftHwProps = leftHwLane.m_laneProps.m_props.ToList();
                var rightHwProps = rightHwLane.m_laneProps.m_props.ToList();

                var wallLightProp = new NetLaneProps.Prop();
                var wallLightPropInfo = Prefabs.Find<PropInfo>("Wall Light Orange", false);

                foreach (var prop in leftHwProps.Where(lp => lp.m_flagsForbidden == NetLane.Flags.Inverted))
                {
                    prop.m_startFlagsForbidden = NetNode.Flags.None;
                    prop.m_startFlagsRequired = NetNode.Flags.None;
                    prop.m_endFlagsForbidden = NetNode.Flags.None;
                    prop.m_endFlagsRequired = NetNode.Flags.Transition;
                    prop.m_angle = 180;
                    prop.m_position.z *= -1;
                    prop.m_segmentOffset *= -1;
                }

                if (version == NetInfoVersion.Elevated || version == NetInfoVersion.Bridge)
                {
                    foreach (var prop in leftHwProps)
                    {
                        prop.m_position.x = -1.55f;
                    }

                    foreach (var prop in rightHwProps)
                    {
                        prop.m_position.x = 1.55f;
                    }
                }

                //Replace 1 way traffic lights with 2 way traffic lights
                foreach (var prop in leftHwLane.m_laneProps.m_props.Where(lp => lp.m_prop.name.Contains("Traffic")))
                {
                    leftHwProps.Remove(prop);
                }

                foreach (var prop in rightHwLane.m_laneProps.m_props.Where(lp => lp.m_prop.name.Contains("Traffic")))
                {
                    rightHwProps.Remove(prop);
                }

                foreach (var prop in basicRoadInfo.FindLane(name => name.Contains("left")).m_laneProps.m_props.Where(lp => lp.m_prop.name.Contains("Traffic")))
                {
                    var leftProp = prop.ShallowClone();
                    if (version == NetInfoVersion.Elevated || version == NetInfoVersion.Bridge)
                    {
                        leftProp.m_position = new Vector3(-2.75f, 1, 0);
                    }
                    else
                    {
                        leftProp.m_position.x = -1;
                    }
                    leftHwProps.Add(leftProp);
                }

                foreach (var prop in basicRoadInfo.FindLane(name => name.Contains("right")).m_laneProps.m_props.Where(lp => lp.m_prop.name.Contains("Traffic")))
                {
                    var rightProp = prop.ShallowClone();
                    if (version == NetInfoVersion.Elevated || version == NetInfoVersion.Bridge)
                    {
                        rightProp.m_position = new Vector3(2.75f, 1, 0);
                    }
                    else
                    {
                        rightProp.m_position.x = 1;
                    }
                    rightHwProps.Add(rightProp);
                }


                var streetLightRight = rightHwLane
                    .m_laneProps
                    .m_props
                    .FirstOrDefault(prop =>
                        prop != null &&
                        prop.m_prop != null &&
                        prop.m_prop.name != null &&
                        prop.m_prop.name.Contains("New Street Light"));

                if (streetLightRight != null)
                {
                    if (version == NetInfoVersion.Tunnel)
                    {
                        streetLightRight.m_repeatDistance = 40;
                        streetLightRight.m_segmentOffset = 0;
                        streetLightRight.m_position = new Vector3(3.2f, -4.5f, 0);

                        rightHwProps.Add(streetLightRight);
                    }
                    else if (version == NetInfoVersion.Slope)
                    {
                        wallLightProp.m_prop = wallLightPropInfo.ShallowClone();
                        wallLightProp.m_finalProp = wallLightPropInfo.ShallowClone();
                        wallLightProp.m_probability = 100;
                        wallLightProp.m_repeatDistance = 20;
                        wallLightProp.m_segmentOffset = 0;
                        var wallLightPropLeft = wallLightProp.ShallowClone();
                        var wallLightPropRight = wallLightProp.ShallowClone();
                        wallLightPropLeft.m_angle = 270;
                        wallLightPropRight.m_angle = 90;
                        wallLightPropLeft.m_position = new Vector3(-1, 1.5f, 0);
                        wallLightPropRight.m_position = new Vector3(1, 1.5f, 0);

                        streetLightRight.m_repeatDistance = 80;
                        streetLightRight.m_segmentOffset = 0;
                        streetLightRight.m_position = new Vector3(1.75f, -3, 0);

                        leftHwProps.Add(wallLightPropLeft);

                        rightHwProps.Add(streetLightRight);
                        rightHwProps.Add(wallLightPropRight);
                    }
                    else
                    {
                        streetLightRight.m_repeatDistance = 80;

                        if (version == NetInfoVersion.Bridge || version == NetInfoVersion.Elevated)
                        {
                            streetLightRight.m_position = new Vector3(1.75f, -3, 0);
                        }
                        else
                        {
                            streetLightRight.m_probability = 0;
                        }
                    }
                }

                leftHwLane.m_laneProps.m_props = leftHwProps.ToArray();
                rightHwLane.m_laneProps.m_props = rightHwProps.ToArray();

                foreach (var lane in vehicleLanes)
                {
                    if (lane.m_laneProps != null && lane.m_laneProps.m_props.Length > 0)
                    {
                        foreach (var prop in lane.m_laneProps.m_props)
                        {
                            prop.m_position = new Vector3(0, 0, 0);
                        }
                    }
                }
            }


            ///////////////////////////
            // AI                    //
            ///////////////////////////
            var hwPlayerNetAI = highwayInfo.GetComponent<PlayerNetAI>();
            var playerNetAI = info.GetComponent<PlayerNetAI>();

            if (hwPlayerNetAI != null && playerNetAI != null)
            {
                playerNetAI.m_constructionCost = hwPlayerNetAI.m_constructionCost * 2 / 3;
                playerNetAI.m_maintenanceCost = hwPlayerNetAI.m_maintenanceCost * 2 / 3;
            }

            var roadBaseAI = info.GetComponent<RoadBaseAI>();

            if (roadBaseAI != null)
            {
                roadBaseAI.m_highwayRules = true;
                roadBaseAI.m_trafficLights = false;
            }

            var roadAI = info.GetComponent<RoadAI>();

            if (roadAI != null)
            {
                roadAI.m_enableZoning = false;
            }
        }
    }
}
