using System;
using System.Collections.Generic;
using System.Linq;
using Transit.Framework;
using Transit.Framework.Builders;
using Transit.Framework.Interfaces;
using Transit.Framework.Modularity;
using Transit.Addon.RoadExtensions.Menus;

namespace Transit.Addon.RoadExtensions.Roads.OneWay3L
{
    public partial class OneWay3LBuilder : Activable, INetInfoBuilderPart
    {
        public int Order { get { return 8; } }
        public int UIOrder { get { return 10; } }

        public string BasedPrefabName { get { return NetInfos.Vanilla.ONEWAY_2L; } }
        public string Name { get { return "Oneway3L"; } }
        public string DisplayName { get { return "Three-Lane Oneway"; } }
        public string Description { get { return "A three-lane one-way road without parkings spaces. Supports medium traffic."; } }
        public string UICategory { get { return AdditionnalMenus.ROADS_SMALL_HV; } }

        public string ThumbnailsPath { get { return @"Roads\OneWay3L\thumbnails.png"; } }
        public string InfoTooltipPath { get { return @"Roads\OneWay3L\infotooltip.png"; } }

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
            info.m_hasParkingSpaces = false;
            info.m_class = owRoadInfo.m_class.Clone(NetInfoClasses.NEXT_SMALL3L_ROAD);
            info.m_pavementWidth = 3;
            info.m_halfWidth = 8;
            info.m_class.m_level = ItemClass.Level.Level3; // To make sure they dont fit with the 4L Small Roads

            // Setting up lanes
            var vehicleLaneTypes = new[]
            {
                NetInfo.LaneType.Vehicle,
                NetInfo.LaneType.PublicTransport,
                NetInfo.LaneType.CargoVehicle,
                NetInfo.LaneType.TransportVehicle
            };

            var templateLane = info.m_lanes
                .Where(l =>
                    vehicleLaneTypes.Contains(l.m_laneType))
                .OrderBy(l => l.m_position)
                .First();

            var vehicleLanes = new List<NetInfo.Lane>();
            const float outerCarLanePosition = 3.0f;
            const float pedLanePosition = 5.5f;
            const float pedLaneWidth = 2.5f;

            for (int i = 0; i < 3; i++)
            {
                var lane = templateLane.Clone(string.Format("Carlane {0}", i + 1));
                lane.m_similarLaneIndex = i;
                lane.m_similarLaneCount = 3;

                switch (i)
                {
                    case 0: lane.m_position = -outerCarLanePosition; break;
                    case 1: lane.m_position = 0f; break;
                    case 2: lane.m_position = outerCarLanePosition; break;
                }

                if (i == 2)
                {
                    lane.m_allowStop = true;
                    lane.m_stopOffset = 0.7f;
                }

                vehicleLanes.Add(lane);
            }

            var pedestrianLanes = info.m_lanes
                .Where(l => l.m_laneType == NetInfo.LaneType.Pedestrian)
                .OrderBy(l => l.m_position)
                .ToArray();

            foreach (var lane in pedestrianLanes)
            {
                if (lane.m_position < 0)
                {
                    lane.m_position = -pedLanePosition;
                }
                else
                {
                    lane.m_position = pedLanePosition;
                }

                lane.m_width = pedLaneWidth;
            }

            var allLanes = new List<NetInfo.Lane>();
            allLanes.AddRange(vehicleLanes);
            allLanes.AddRange(pedestrianLanes);

            info.m_lanes = allLanes
                .OrderBy(l => l.m_position)
                .ToArray();

            //Setting Up Props

            var propLanes = info.m_lanes.Where(l => l.m_laneProps != null && (l.m_laneProps.name.ToLower().Contains("left") || l.m_laneProps.name.ToLower().Contains("right"))).ToList();

            if (propLanes.Count > 0)
            {
                for (int i = 0; i < propLanes.Count(); i++)
                {
                    var positionMultiplier = Math.Abs(propLanes[i].m_position) / propLanes[i].m_position;
                    propLanes[i].m_position += positionMultiplier;
                }
            }

            if (version == NetInfoVersion.Ground)
            {
                var owPlayerNetAI = owRoadInfo.GetComponent<PlayerNetAI>();
                var playerNetAI = info.GetComponent<PlayerNetAI>();

                if (owPlayerNetAI != null && playerNetAI != null)
                {
                    playerNetAI.m_constructionCost = owPlayerNetAI.m_constructionCost * 11 / 10; // 10% increase
                    playerNetAI.m_maintenanceCost = owPlayerNetAI.m_maintenanceCost * 11 / 10; // 10% increase
                }
            }
            else // Same as the original basic road specs
            {

            }

            var roadBaseAI = info.GetComponent<RoadBaseAI>();

            if (roadBaseAI != null)
            {
                roadBaseAI.m_trafficLights = true;
            }
        }
    }
}
