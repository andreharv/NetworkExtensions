using System;
using System.Linq;
using Transit.Framework;
using UnityEngine;
using Transit.Framework.Builders;

namespace Transit.Addon.RoadExtensions.Roads.Highway4L
{
    public partial class Highway4LBuilder : Activable, INetInfoBuilderPart
    {
        public int Order { get { return 49; } }
        public int UIOrder { get { return 14; } }

        public string BasedPrefabName { get { return NetInfos.Vanilla.HIGHWAY_3L; } }
        public string Name { get { return "Four-Lane Highway"; } }
        public string DisplayName { get { return "Four-Lane Highway"; } }
        public string Description { get { return "A four-lane, one-way road suitable for very high and dense traffic between metropolitan areas. Lanes going the opposite direction need to be built separately. Highway does not allow zoning next to it!"; } }
        public string UICategory { get { return "RoadsHighway"; } }

        public string ThumbnailsPath { get { return @"Roads\Highway4L\thumbnails.png"; } }
        public string InfoTooltipPath { get { return @"Roads\Highway4L\infotooltip.png"; } }

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


            ///////////////////////////
            // 3DModeling            //
            ///////////////////////////
            info.Setup24mMesh(version);


            ///////////////////////////
            // Texturing             //
            ///////////////////////////
            SetupTextures(info, version);


            ///////////////////////////
            // Set up                //
            ///////////////////////////
            info.m_availableIn = ItemClass.Availability.All;
            info.m_class = highwayInfo.m_class.Clone(NetInfoClasses.NEXT_HIGHWAY4L);
            info.m_surfaceLevel = 0;
            info.m_createPavement = !(version == NetInfoVersion.Ground || version == NetInfoVersion.Tunnel);
            info.m_createGravel = version == NetInfoVersion.Ground;
            info.m_averageVehicleLaneSpeed = 2f;
            info.m_hasParkingSpaces = false;
            info.m_hasPedestrianLanes = false;
            info.m_UnlockMilestone = highwayInfo.m_UnlockMilestone;
            info.m_halfWidth = (version == NetInfoVersion.Bridge || version == NetInfoVersion.Elevated) ? 11 : 12;
            info.m_pavementWidth = 2;
            if (version == NetInfoVersion.Tunnel)
            {
                info.m_setVehicleFlags = Vehicle.Flags.Transition;
            }
            // Disabling Parkings and Peds
            foreach (var l in info.m_lanes)
            {
                switch (l.m_laneType)
                {
                    case NetInfo.LaneType.Parking:
                        l.m_laneType = NetInfo.LaneType.None;
                        break;
                    case NetInfo.LaneType.Pedestrian:
                        l.m_laneType = NetInfo.LaneType.None;
                        break;
                }
            }
            // Setting up lanes
            var sourceLane = info.m_lanes.Where(l => l.m_laneType == NetInfo.LaneType.Vehicle).First();

            if (sourceLane != null)
            {
                var targetLane = sourceLane.Clone("New Lane");
                var tempLanes = info.m_lanes.ToList();
                tempLanes.Add(targetLane);
                info.m_lanes = tempLanes.ToArray();
            }

            var vehicleLanes = info.m_lanes
                .Where(l => l.m_laneType != NetInfo.LaneType.None)
                .OrderBy(l => l.m_similarLaneIndex)
                .ToList();

            var propLanes = info.m_lanes
                .Where(l => l.m_laneType == NetInfo.LaneType.None)
                .OrderBy(l => l.m_similarLaneIndex)
                .ToArray();

            var nbLanes = vehicleLanes.Count();

            const float laneWidth = 4f;
            var positionStart = (laneWidth * ((1f - nbLanes) / 2f));

            for (int i = 0; i < vehicleLanes.Count; i++)
            {
                var l = vehicleLanes[i];
                l.m_allowStop = false;
                l.m_speedLimit = 2f;
                l.m_direction = NetInfo.Direction.Forward;
                l.m_finalDirection = NetInfo.Direction.Forward;
                l.m_verticalOffset = 0f;
                l.m_width = laneWidth;
                l.m_position = positionStart + i * laneWidth;
            }
            var hwPlayerNetAI = highwayInfo.GetComponent<PlayerNetAI>();
            var playerNetAI = info.GetComponent<PlayerNetAI>();

            if (hwPlayerNetAI != null && playerNetAI != null)
            {
                playerNetAI.m_constructionCost = hwPlayerNetAI.m_constructionCost * 4 / 3;
                playerNetAI.m_maintenanceCost = hwPlayerNetAI.m_maintenanceCost * 4 / 3;
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

            info.SetHighwayProps(highwayInfo);
            info.TrimNonHighwayProps();

            //Setting up props
            NetInfo.Lane leftHwLane = null;
            NetInfo.Lane rightHwLane = null;

            if (version == NetInfoVersion.Tunnel)
            {
                var counter = 0;
                for (var i = 0; i < info.m_lanes.Length; i++)
                {
                    if (info.m_lanes[i].m_laneType == NetInfo.LaneType.None && counter == 0)
                    {
                        counter++;
                        leftHwLane = info.m_lanes[i];
                    }
                    else if (info.m_lanes[i].m_laneType == NetInfo.LaneType.None && counter == 1)
                    {
                        rightHwLane = info.m_lanes[i];
                    }
                    else
                    {
                        info.m_lanes[i].m_laneProps = highwayInfo.m_lanes.Where(l => l.m_laneType == NetInfo.LaneType.Vehicle).First().m_laneProps.ShallowClone();
                    }
                }
                if (leftHwLane != null)
                {
                    leftHwLane.m_laneProps = new NetLaneProps();
                    var newProps = ScriptableObject.CreateInstance<NetLaneProps>();
                    newProps.name = "Highway4L Left Props";

                    newProps.m_props = highwayInfo
                        .m_lanes
                        .Where(l => l != null && l.m_laneProps != null && l.m_laneProps.name != null && l.m_laneProps.m_props != null)
                        .FirstOrDefault(l => l.m_laneProps.name.ToLower().Contains("left"))
                        .m_laneProps
                        .m_props
                        .Select(p => p.ShallowClone())
                        .ToArray();

                    leftHwLane.m_laneProps = newProps;
                }
                if (rightHwLane != null)
                {
                    rightHwLane.m_laneProps = new NetLaneProps();
                    var newProps = ScriptableObject.CreateInstance<NetLaneProps>();
                    newProps.name = "Highway4L Right Props";

                    newProps.m_props = highwayInfo
                        .m_lanes
                        .Where(l => l != null && l.m_laneProps != null && l.m_laneProps.name != null && l.m_laneProps.m_props != null)
                        .FirstOrDefault(l => l.m_laneProps.name.ToLower().Contains("right"))
                        .m_laneProps
                        .m_props
                        .Select(p => p.ShallowClone())
                        .ToArray();

                    rightHwLane.m_laneProps = newProps;
                }
            }
            else
            {
                leftHwLane = info
                   .m_lanes
                   .Where(l => l != null && l.m_laneProps != null && l.m_laneProps.name != null && l.m_laneProps.m_props != null)
                   .FirstOrDefault(l => l.m_laneProps.name.ToLower().Contains("left")).ShallowClone();

                rightHwLane = info
                  .m_lanes
                  .Where(l => l != null && l.m_laneProps != null && l.m_laneProps.name != null && l.m_laneProps.m_props != null)
                  .FirstOrDefault(l => l.m_laneProps.name.ToLower().Contains("right")).ShallowClone();
            }

            if (leftHwLane != null && rightHwLane != null)
            {
                var leftHwProps = leftHwLane.m_laneProps.m_props.ToList();
                var rightHwProps = rightHwLane.m_laneProps.m_props.ToList();

                var wallLightProp = new NetLaneProps.Prop();
                var wallLightPropInfo = Prefabs.Find<PropInfo>("Wall Light Orange", false);
                var streetLightPropInfo = Prefabs.Find<PropInfo>("New Street Light", false);

                NetLaneProps.Prop streetLightRight = null;
                NetLaneProps.Prop motorwaySignLeft = null;
                NetLaneProps.Prop motorwaySignRight = null;
                NetLaneProps.Prop speedLimitSignLeft = null;
                NetLaneProps.Prop speedLimitSignRight = null;

                for (int i = 0; i < rightHwLane.m_laneProps.m_props.Length; i++)
                {
                    var prop = rightHwLane.m_laneProps.m_props[i];
                    if (prop != null && prop.m_prop != null && prop.m_prop.name != null && prop.m_prop.name.Contains("New Street Light"))
                    {
                        streetLightRight = prop;
                    }
                    else if (prop != null && prop.m_prop != null && prop.m_prop.name != null && prop.m_prop.name.ToLower().Contains("motorway"))
                    {
                        motorwaySignRight = prop.ShallowClone();
                        rightHwProps.Remove(prop);
                        if (version == NetInfoVersion.Slope)
                        {
                            motorwaySignRight.m_position = new Vector3(0.4f, 1, 5);
                            motorwaySignRight.m_angle = -30;
                        }
                        rightHwProps.Add(motorwaySignRight);

                    }
                    else if (prop != null && prop.m_prop != null && prop.m_prop.name != null && prop.m_prop.name.Contains("100"))
                    {
                        speedLimitSignRight = prop.ShallowClone();
                        rightHwProps.Remove(prop);
                        if (version == NetInfoVersion.Slope)
                        {
                            speedLimitSignRight.m_position = new Vector3(0.4f, 1, 10);
                            speedLimitSignRight.m_angle = -30;
                        }
                        rightHwProps.Add(speedLimitSignRight);
                    }
                }

                for (int i = 0; i < leftHwLane.m_laneProps.m_props.Length; i++)
                {
                    var prop = leftHwLane.m_laneProps.m_props[i];
                    if (prop != null && prop.m_prop != null && prop.m_prop.name != null && prop.m_prop.name.ToLower().Contains("motorway"))
                    {
                        motorwaySignLeft = prop.ShallowClone();
                        leftHwProps.Remove(prop);
                        if (version == NetInfoVersion.Slope)
                        {
                            motorwaySignLeft.m_position = new Vector3(-0.4f, 1, 5);
                            motorwaySignLeft.m_angle = 30;
                        }
                        leftHwProps.Add(motorwaySignLeft);
                    }
                    else if (prop != null && prop.m_prop != null && prop.m_prop.name != null && prop.m_prop.name.Contains("100"))
                    {
                        speedLimitSignLeft = prop.ShallowClone();
                        leftHwProps.Remove(prop);
                        if (version == NetInfoVersion.Slope)
                        {
                            speedLimitSignLeft.m_position = new Vector3(-0.4f, 1, 10);
                            speedLimitSignLeft.m_angle = 30;
                        }
                        leftHwProps.Add(speedLimitSignLeft);
                    }
                }

                if (streetLightRight != null)
                {
                    if (version == NetInfoVersion.Tunnel)
                    {
                        streetLightRight.m_repeatDistance = 40;
                        streetLightRight.m_segmentOffset = 0;
                        streetLightRight.m_position = new Vector3(1.75f, -4.5f, 0);

                        motorwaySignLeft.m_position.x = 0;
                        motorwaySignRight.m_position.x = 0;
                        speedLimitSignLeft.m_position.x = 0;
                        speedLimitSignRight.m_position.x = 0;

                        rightHwProps.Add(streetLightRight);
                    }
                    else if (version == NetInfoVersion.Slope)
                    {
                        wallLightProp.m_prop = wallLightPropInfo.ShallowClone();
                        wallLightProp.m_finalProp = wallLightPropInfo.ShallowClone();
                        wallLightProp.m_probability = 100;
                        wallLightProp.m_repeatDistance = 10;
                        wallLightProp.m_segmentOffset = 0;
                        wallLightProp.m_prop.m_effects[0].m_direction = new Vector3(0, -90, 25);
                        wallLightProp.m_finalProp.m_effects[0].m_direction = new Vector3(0, -90, 25);
                        var wallLightPropLeft = wallLightProp.ShallowClone();
                        var wallLightPropRight = wallLightProp.ShallowClone();
                        wallLightPropLeft.m_angle = 270;
                        wallLightPropRight.m_angle = 90;
                        wallLightPropLeft.m_position = new Vector3(0.5f, 1.5f, 0);
                        wallLightPropRight.m_position = new Vector3(-0.5f, 1.5f, 0);

                        leftHwProps.Add(wallLightPropLeft);
                        rightHwProps.Add(wallLightPropRight);

                        streetLightRight.m_repeatDistance = 0;
                        streetLightRight.m_segmentOffset = 0;
                        streetLightRight.m_position = new Vector3(0, -4, 0);

                        rightHwProps.Add(streetLightRight);
                    }
                    else
                    {
                        streetLightRight.m_repeatDistance = 40;

                        if (version == NetInfoVersion.Bridge || version == NetInfoVersion.Elevated)
                        {
                            streetLightRight.m_position = new Vector3(1.125f, 0, 0);
                        }
                        else
                        {
                            streetLightRight.m_position = new Vector3(1, 0, 0);
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
                foreach (var lane in propLanes)
                {
                    if (lane.m_laneProps != null && lane.m_laneProps.m_props.Length > 0)
                    {
                        foreach (var prop in lane.m_laneProps.m_props)
                        {
                            var propName = prop.m_prop.name;
                            var positionMultiplier = lane.m_position / Math.Abs(lane.m_position);
                            if (!propName.Contains(streetLightPropInfo.name) && propName != wallLightPropInfo.name && !propName.Contains("100") && !propName.ToLower().Contains("motorway"))
                            {
                                prop.m_position.x = positionMultiplier * 1.2f;
                            }
                        }
                    }
                }
            }
        }
    }
}