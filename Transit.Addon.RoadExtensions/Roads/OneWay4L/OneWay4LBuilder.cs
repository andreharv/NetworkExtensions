using System;
using System.Collections.Generic;
using System.Linq;
using Transit.Framework;
using Transit.Framework.Modularity;
using Transit.Addon.RoadExtensions.Menus;

namespace Transit.Addon.RoadExtensions.Roads.OneWay4L
{
    public class OneWay4LBuilder : Activable, INetInfoBuilder
    {
        public int Order { get { return 9; } }
        public int UIOrder { get { return 30; } }

        public string TemplateName { get { return NetInfos.Vanilla.ONEWAY_2L; } }
        public string Name { get { return "Oneway4L"; } }
        public string DisplayName { get { return "Small Four-Lane Oneway"; } }
        public string Description { get { return "A four-lane one-way road without parkings spaces. Supports medium traffic."; } }
        public string UICategory { get { return AdditionnalMenus.ROADS_SMALL_HV; } }

        public string ThumbnailsPath    { get { return @"Roads\OneWay4L\thumbnails.png"; } }
        public string InfoTooltipPath   { get { return @"Roads\OneWay4L\infotooltip.png"; } }

        public NetInfoVersion SupportedVersions
        {
            get { return NetInfoVersion.Ground; }
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
            if (version == NetInfoVersion.Ground)
            {
                var segments0 = info.m_segments[0];
                var nodes0 = info.m_nodes[0];

                segments0.m_forwardRequired = NetSegment.Flags.None;
                segments0.m_forwardForbidden = NetSegment.Flags.None;
                segments0.m_backwardRequired = NetSegment.Flags.None;
                segments0.m_backwardForbidden = NetSegment.Flags.None;
                segments0.SetMeshes
                    (@"Roads\SmallHeavyRoads\Meshes\Ground.obj",
                     @"Roads\SmallHeavyRoads\Meshes\Ground_LOD.obj");

                nodes0.SetMeshes
                    (@"Roads\SmallHeavyRoads\Meshes\Ground.obj",
                     @"Roads\SmallHeavyRoads\Meshes\Ground_Node_LOD.obj");

                info.m_segments = new[] { segments0 };
                info.m_nodes = new[] { nodes0 };
            }

            ///////////////////////////
            // Texturing             //
            ///////////////////////////
            switch (version)
            {
                case NetInfoVersion.Ground:
                    info.SetAllSegmentsTexture(
                        new TexturesSet
                           (@"Roads\OneWay4L\Textures\Ground_Segment__MainTex.png",
                            @"Roads\OneWay4L\Textures\Ground_Segment__AlphaMap.png"),
                        new TexturesSet
                           (@"Roads\OneWay4L\Textures\Ground_SegmentLOD__MainTex.png",
                            @"Roads\OneWay4L\Textures\Ground_SegmentLOD__AlphaMap.png",
                            @"Roads\OneWay4L\Textures\Ground_SegmentLOD__XYS.png"));
                    break;
            }


            ///////////////////////////
            // Set up                //
            ///////////////////////////
            info.m_hasParkingSpaces = false;
            info.m_class = owRoadInfo.m_class.Clone(NetInfoClasses.NEXT_SMALL4L_ROAD);
            info.m_pavementWidth = 2;

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
            const float outerCarLanePosition = 4.4f;
            const float innerCarLanePosition = 1.5f;
            const float pedLanePosition = 8f;
            const float pedLaneWidth = 1.5f;

            for (int i = 0; i < 4; i++)
            {
                var lane = templateLane.Clone(string.Format("Carlane {0}", i + 1));
                lane.m_similarLaneIndex = i;
                lane.m_similarLaneCount = 4;

                switch (i)
                {
                    case 0: lane.m_position = -outerCarLanePosition; break;
                    case 1: lane.m_position = -innerCarLanePosition; break;
                    case 2: lane.m_position = innerCarLanePosition; break;
                    case 3: lane.m_position = outerCarLanePosition; break;
                }

                if (i == 3)
                {
                    lane.m_allowStop = true;
                    lane.m_stopOffset = 0.3f;
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


            if (version == NetInfoVersion.Ground)
            {
                var owPlayerNetAI = owRoadInfo.GetComponent<PlayerNetAI>();
                var playerNetAI = info.GetComponent<PlayerNetAI>();

                if (owPlayerNetAI != null && playerNetAI != null)
                {
                    playerNetAI.m_constructionCost = owPlayerNetAI.m_constructionCost * 125 / 100; // 25% increase
                    playerNetAI.m_maintenanceCost = owPlayerNetAI.m_maintenanceCost * 125 / 100; // 25% increase
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