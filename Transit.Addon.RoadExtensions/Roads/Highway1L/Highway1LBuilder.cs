using System;
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
            info.DisableHighwayParkingsAndPeds();
            var leftHwLane = info.SetHighwayLeftShoulder(highwayInfo);
            var rightHwLane = info.SetHighwayRightShoulder(highwayInfo);

            rightHwLane.m_width = info.m_pavementWidth;
            rightHwLane.m_position = info.m_halfWidth - leftHwLane.m_width;

            leftHwLane.m_width = rightHwLane.m_width;
            leftHwLane.m_position = -rightHwLane.m_position;

            var vehicleLanes = info.m_lanes
                .Where(l => l.m_laneType != NetInfo.LaneType.None)
                .OrderBy(l => l.m_similarLaneIndex)
                .ToArray();

            var nbLanes = vehicleLanes.Count();

            const float laneWidth = 4f;
            var positionStart = laneWidth * ((1f - nbLanes) / 2f);

            for (var i = 0; i < vehicleLanes.Length; i++)
            {
                var l = vehicleLanes[i];
                l.m_allowStop = false;
                l.m_speedLimit = 2f;
                l.m_verticalOffset = 0f;
                l.m_width = laneWidth;
                l.m_position = positionStart + i * laneWidth;
            }


            ///////////////////////////
            // Set up props          //
            ///////////////////////////

            // Shoulder lanes ---------------------------------------------------------------------
            var leftHwLaneProps = leftHwLane.m_laneProps.m_props.ToList();
            var rightHwLaneProps = rightHwLane.m_laneProps.m_props.ToList();

            foreach (var prop in leftHwLaneProps.Where(lp => lp.m_flagsForbidden == NetLane.Flags.Inverted))
            {
                prop.m_startFlagsForbidden = NetNode.Flags.None;
                prop.m_startFlagsRequired = NetNode.Flags.None;
                prop.m_endFlagsForbidden = NetNode.Flags.None;
                prop.m_endFlagsRequired = NetNode.Flags.Transition;
                prop.m_angle = 180;
                prop.m_position.z *= -1;
                prop.m_segmentOffset *= -1;
            }

            switch (version)
            {
                case NetInfoVersion.Ground:
                case NetInfoVersion.Slope:
                case NetInfoVersion.Tunnel:
                    foreach (var prop in leftHwLaneProps)
                    {
                        prop.m_position.x = -1f;
                    }

                    foreach (var prop in rightHwLaneProps)
                    {
                        prop.m_position.x = 1f;
                    }
                    break;

                case NetInfoVersion.Elevated:
                case NetInfoVersion.Bridge:
                    foreach (var prop in leftHwLaneProps)
                    {
                        prop.m_position.x = -0.6f;
                    }

                    foreach (var prop in rightHwLaneProps)
                    {
                        prop.m_position.x = 0.6f;
                    }
                    break;
            }


            // Replacing 1 way traffic lights with 2 way traffic lights
            leftHwLaneProps.Trim(lp => lp.m_prop.name.Contains("Traffic"));
            rightHwLaneProps.Trim(lp => lp.m_prop.name.Contains("Traffic"));

            foreach (var prop in basicRoadInfo.FindLane(name => name.Contains("left")).m_laneProps.m_props.Where(lp => lp.m_prop.name.Contains("Traffic")))
            {
                var leftProp = prop.ShallowClone();
                if (version == NetInfoVersion.Elevated || version == NetInfoVersion.Bridge)
                {
                    leftProp.m_position = new Vector3(-1.75f, 1, 0);
                }
                else
                {
                    leftProp.m_position.x = -1;
                }
                leftHwLaneProps.Add(leftProp);
            }

            foreach (var prop in basicRoadInfo.FindLane(name => name.Contains("right")).m_laneProps.m_props.Where(lp => lp.m_prop.name.Contains("Traffic")))
            {
                var rightProp = prop.ShallowClone();
                if (version == NetInfoVersion.Elevated || version == NetInfoVersion.Bridge)
                {
                    rightProp.m_position = new Vector3(1.75f, 1, 0);
                }
                else
                {
                    rightProp.m_position.x = 1;
                }
                rightHwLaneProps.Add(rightProp);
            }


            // Lightning
            var streetLightPropInfo = Prefabs.Find<PropInfo>("New Street Light Highway", false);
            var streetLightProp = rightHwLaneProps.FirstOrDefault(prop => prop.m_prop == streetLightPropInfo);

            if (streetLightProp != null)
            {
                switch (version)
                {
                    case NetInfoVersion.Elevated:
                    case NetInfoVersion.Bridge:
                        streetLightProp.m_repeatDistance = 80;
                        streetLightProp.m_position = new Vector3(1.75f, -2, 0);
                        break;

                    case NetInfoVersion.Tunnel:
                        streetLightProp.m_repeatDistance = 40;
                        streetLightProp.m_segmentOffset = 0;
                        streetLightProp.m_position = new Vector3(3.2f, -4.5f, 0);
                        break;

                    case NetInfoVersion.Slope:
                        rightHwLaneProps.Trim(p => p.m_prop == streetLightPropInfo);
                        break;
                }
            }

            if (version == NetInfoVersion.Slope)
            {
                var wallLightPropInfo = Prefabs.Find<PropInfo>("Wall Light Orange", false);
                var wallLightProp = new NetLaneProps.Prop();
                wallLightProp.m_prop = wallLightPropInfo.ShallowClone();
                wallLightProp.m_probability = 100;
                wallLightProp.m_repeatDistance = 20;
                wallLightProp.m_segmentOffset = 0;

                var wallLightPropLeft = wallLightProp.ShallowClone();
                wallLightPropLeft.m_angle = 270;
                wallLightPropLeft.m_position = new Vector3(0, 1.5f, 0);
                leftHwLaneProps.Add(wallLightPropLeft);

                var wallLightPropRight = wallLightProp.ShallowClone();
                wallLightPropRight.m_angle = 90;
                wallLightPropRight.m_position = new Vector3(0, 1.5f, 0);
                rightHwLaneProps.Add(wallLightPropRight);
            }

            leftHwLane.m_laneProps.m_props = leftHwLaneProps.ToArray();
            rightHwLane.m_laneProps.m_props = rightHwLaneProps.ToArray();


            // Other lanes ------------------------------------------------------------------------
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

            info.TrimHighwayProps(version == NetInfoVersion.Ground);


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
