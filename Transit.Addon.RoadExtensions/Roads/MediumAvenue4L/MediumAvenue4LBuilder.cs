﻿using System.Linq;
using Transit.Addon.RoadExtensions.Roads.Common;
using Transit.Framework;
using Transit.Framework.Builders;
using UnityEngine;

namespace Transit.Addon.RoadExtensions.Roads.MediumAvenue4L
{
    public partial class MediumAvenue4LBuilder : Activable, INetInfoBuilderPart
    {
        public int Order { get { return 21; } }
        public int UIOrder { get { return 5; } }

        public string BasedPrefabName { get { return NetInfos.Vanilla.ROAD_6L; } }
        public string Name { get { return "Medium Avenue"; } }
        public string DisplayName { get { return "Four-Lane Road with painted median"; } }
        public string Description { get { return "A four-lane road with painted median and parking spaces. Supports medium traffic."; } }
        public string ShortDescription { get { return "Parkings, zoneable, medium traffic"; } }
        public string UICategory { get { return "RoadsMedium"; } }

        public string ThumbnailsPath { get { return @"Roads\MediumAvenue4L\thumbnails.png"; } }
        public string InfoTooltipPath { get { return @"Roads\MediumAvenue4L\infotooltip.png"; } }

        public NetInfoVersion SupportedVersions
        {
            get { return NetInfoVersion.All; }
        }

        public void BuildUp(NetInfo info, NetInfoVersion version)
        {
            ///////////////////////////
            // Template              //
            ///////////////////////////
            //var highwayInfo = Prefabs.Find<NetInfo>(NetInfos.Vanilla.HIGHWAY_3L_SLOPE);
            var roadInfo = Prefabs.Find<NetInfo>(NetInfos.Vanilla.ROAD_6L);
            var roadTunnelInfo = Prefabs.Find<NetInfo>(NetInfos.Vanilla.ROAD_6L_TUNNEL);
            var bridgeInfo = Prefabs.Find<NetInfo>(NetInfos.Vanilla.AVENUE_4L_BRIDGE).Clone("temp");

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
                info.m_setVehicleFlags = Vehicle.Flags.Transition;
                info.m_setCitizenFlags = CitizenInstance.Flags.Transition;
                info.m_class = roadTunnelInfo.m_class.Clone(NetInfoClasses.NEXT_MEDIUM_ROAD_TUNNEL);
            }
            else if (version == NetInfoVersion.Bridge)
            {
                info.m_class = bridgeInfo.m_class.Clone(NetInfoClasses.NEXT_MEDIUM_ROAD);
            }
            else
            {
                info.m_class = roadInfo.m_class.Clone(NetInfoClasses.NEXT_MEDIUM_ROAD);
            }

            // Setting up lanes
            var rpHelper = new RoadPropertyHelper()
            {
                IsTwoWay = true,
                LanesToAdd = -2,
                LaneWidth = 3.8f,
                CLVersion = CenterLaneVersion.Median
            };

            info.SetRoadLanes(version, rpHelper);
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


            //var propLanes = info.m_lanes.Where(l => l.m_laneProps != null && (l.m_laneProps.name.ToLower().Contains("left") || l.m_laneProps.name.ToLower().Contains("right"))).ToList();
            var owPlayerNetAI = roadInfo.GetComponent<PlayerNetAI>();
            var playerNetAI = info.GetComponent<PlayerNetAI>();

            if (owPlayerNetAI != null && playerNetAI != null)
            {
                playerNetAI.m_constructionCost = owPlayerNetAI.m_constructionCost * 2 / 3; // Charge by the lane?
                playerNetAI.m_maintenanceCost = owPlayerNetAI.m_maintenanceCost * 2 / 3; // Charge by the lane?
            }

            var roadBaseAI = info.GetComponent<RoadBaseAI>();

            if (roadBaseAI != null)
            {
                roadBaseAI.m_trafficLights = true;
            }
        }
    }
}
