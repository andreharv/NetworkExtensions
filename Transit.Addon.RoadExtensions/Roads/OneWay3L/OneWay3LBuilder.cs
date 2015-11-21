using System.Collections.Generic;
using System.Linq;
using Transit.Addon.RoadExtensions.Menus;
using Transit.Framework;
using Transit.Framework.Builders;

namespace Transit.Addon.RoadExtensions.Roads.OneWay3L
{
    public class OneWay3LBuilder : Activable, INetInfoBuilderPart
    {
        public int Order { get { return 8; } }
        public int UIOrder { get { return 10; } }

        public string BasedPrefabName { get { return NetInfos.Vanilla.ONEWAY_2L; } }
        public string Name { get { return "Oneway3L"; } }
        public string DisplayName { get { return "Three-Lane Oneway"; } }
        public string Description { get { return "A three-lane one-way road without parkings spaces. Supports medium traffic."; } }
        public string ShortDescription { get { return "No parking, zoneable, medium traffic"; } }
        public string UICategory { get { return AdditionnalMenus.ROADS_SMALL_HV; } }

        public string ThumbnailsPath    { get { return @"Roads\OneWay3L\thumbnails.png"; } }
        public string InfoTooltipPath   { get { return @"Roads\OneWay3L\infotooltip.png"; } }

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
                           (@"Roads\OneWay3L\Textures\Ground_Segment__MainTex.png",
                            @"Roads\OneWay3L\Textures\Ground_Segment__AlphaMap.png"),
                        new LODTexturesSet
                           (@"Roads\OneWay3L\Textures\Ground_SegmentLOD__MainTex.png",
                            @"Roads\OneWay3L\Textures\Ground_SegmentLOD__AlphaMap.png",
                            @"Roads\OneWay3L\Textures\Ground_SegmentLOD__XYS.png"));
                    break;
            }


            ///////////////////////////
            // Set up                //
            ///////////////////////////
            info.m_hasParkingSpaces = false;
            info.m_class = owRoadInfo.m_class.Clone(NetInfoClasses.NEXT_SMALL3L_ROAD);
            info.m_pavementWidth = 2;
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
            const float outerCarLanePosition = 4.0f;
            const float pedLanePosition = 8f;
            const float pedLaneWidth = 1.5f;

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
