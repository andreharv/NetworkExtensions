using System.Linq;
using Transit.Addon.RoadExtensions.Roads.Common;
using Transit.Framework;
using Transit.Framework.Builders;
using UnityEngine;

namespace Transit.Addon.RoadExtensions.Roads.Avenues.LargeAvenue6LM
{
    public class LargeAvenue6LMBuilder : Activable, INetInfoBuilderPart
    {
        public int Order { get { return 22; } }
        public int UIOrder { get { return 0; } }

        public string BasedPrefabName { get { return NetInfos.Vanilla.ROAD_4L; } }
        public string Name { get { return "Six-Lane Avenue M"; } }
        public string DisplayName { get { return "Six-Lane Road with Median"; } }
        public string CodeName { get { return "LARGEROAD_6LM"; } }
        public string Description { get { return "A six-lane road. Supports heavy traffic."; } }
        public string ShortDescription { get { return "No parking, zoneable, heavy traffic."; } }
        public string UICategory { get { return "RoadsLarge"; } }

        public string ThumbnailsPath { get { return @"Roads\Avenues\LargeAvenue6LM\thumbnails.png"; } }
        public string InfoTooltipPath { get { return @"Roads\Avenues\LargeAvenue6LM\infotooltip.png"; } }

        public NetInfoVersion SupportedVersions
        {
            get { return NetInfoVersion.Ground; }
        }

        public void BuildUp(NetInfo info, NetInfoVersion version)
        {
            ///////////////////////////
            // Template              //
            ///////////////////////////
            var mediumRoadInfo = Prefabs.Find<NetInfo>(NetInfos.Vanilla.ROAD_4L);

            ///////////////////////////
            // 3DModeling            //
            ///////////////////////////
            if (version == NetInfoVersion.Ground)
            {

                var segments0 = info.m_segments[0];
                //var nodes0 = info.m_nodes[0];

                segments0.SetMeshes
                    (@"Roads\Avenues\LargeAvenue6LM\Meshes\Ground.obj");

                //nodes0.SetMeshes
                //    (@"Roads\LargeAvenue6LM\Meshes\Ground_Node.obj");

                info.m_segments = new[] { segments0 };
                //info.m_nodes = new[] { nodes0 };
            }

            ///////////////////////////
            // Texturing             //
            ///////////////////////////
            switch (version)
            {
                case NetInfoVersion.Ground:
                    info.SetAllSegmentsTexture(
                        new TexturesSet
                           (@"Roads\Avenues\LargeAvenue6LM\Textures\Ground_Segment__MainTex.png",
                            @"Roads\Avenues\LargeAvenue6LM\Textures\Ground_Segment__AlphaMap.png"));
                    break;
            }

            /////////////////////////////
            //// Set up                //
            /////////////////////////////
            info.m_class = mediumRoadInfo.m_class.Clone(NetInfoClasses.NEXT_MEDIUM_ROAD);
            info.m_UnlockMilestone = mediumRoadInfo.m_UnlockMilestone;
            info.m_hasParkingSpaces = false;

            // Setting up traffic lanes
            var vehicleLaneTypes = new[]
            {
                NetInfo.LaneType.Vehicle,
                NetInfo.LaneType.PublicTransport,
                NetInfo.LaneType.CargoVehicle,
                NetInfo.LaneType.TransportVehicle
            };

            var vehicleLanes = info.m_lanes
                .Where(l =>
                    l.m_laneType.HasFlag(NetInfo.LaneType.Parking) ||
                    vehicleLaneTypes.Contains(l.m_laneType))
                .OrderBy(l => l.m_position)
                .ToArray();

            const float outerCarLanePosition = 9.5f;
            const float middleCarLanePosition = 6.5f;
            const float innerCarLanePosition = 3.5f;
            const float laneWidth = 3f;
            const float pedLanePosition = 16f;
            const float pedLaneWidth = 5f;

            for (int i = 0; i < vehicleLanes.Length; i++)
            {
                var lane = vehicleLanes[i];

                if (lane.m_laneType.HasFlag(NetInfo.LaneType.Parking))
                {
                    int closestVehicleLaneId;

                    if (i - 1 >= 0 && vehicleLaneTypes.Contains(vehicleLanes[i - 1].m_laneType))
                    {
                        closestVehicleLaneId = i - 1;
                    }
                    else if (i + 1 < vehicleLanes.Length && vehicleLaneTypes.Contains(vehicleLanes[i + 1].m_laneType))
                    {
                        closestVehicleLaneId = i + 1;
                    }
                    else
                    {
                        continue; // Not supposed to happen
                    }

                    var closestVehicleLane = vehicleLanes[closestVehicleLaneId];

                    SetLane(lane, closestVehicleLane);
                }

                switch (i)
                {
                    case 0: lane.m_position = -outerCarLanePosition; break;
                    case 1: lane.m_position = -middleCarLanePosition; break;
                    case 2: lane.m_position = -innerCarLanePosition; break;
                    case 3: lane.m_position = innerCarLanePosition; break;
                    case 4: lane.m_position = middleCarLanePosition; break;
                    case 5: lane.m_position = outerCarLanePosition; break;
                }
                lane.m_width = laneWidth;
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

            info.SetupNewSpeedLimitProps(60, 50);

            // TODO: Replace leftlane traffic light to median
            // TODO: Use custom mesh (with median)

            if (version == NetInfoVersion.Ground)
            {
                var mrPlayerNetAI = mediumRoadInfo.GetComponent<PlayerNetAI>();
                var playerNetAI = info.GetComponent<PlayerNetAI>();

                if (mrPlayerNetAI != null && playerNetAI != null)
                {
                    playerNetAI.m_constructionCost = mrPlayerNetAI.m_constructionCost * 12 / 10; // 20% increase
                    playerNetAI.m_maintenanceCost = mrPlayerNetAI.m_maintenanceCost * 12 / 10; // 10% increase
                }

                var mrRoadBaseAI = mediumRoadInfo.GetComponent<RoadBaseAI>();
                var roadBaseAI = info.GetComponent<RoadBaseAI>();

                if (mrRoadBaseAI != null && roadBaseAI != null)
                {
                    roadBaseAI.m_noiseAccumulation = mrRoadBaseAI.m_noiseAccumulation;
                    roadBaseAI.m_noiseRadius = mrRoadBaseAI.m_noiseRadius;
                }
            }
        }

        private static void SetLane(NetInfo.Lane newLane, NetInfo.Lane closestLane)
        {
            newLane.m_direction = closestLane.m_direction;
            newLane.m_finalDirection = closestLane.m_finalDirection;
            newLane.m_allowConnect = closestLane.m_allowConnect;
            newLane.m_allowStop = closestLane.m_allowStop;
            if (closestLane.m_allowStop)
            {
                closestLane.m_allowStop = false;
                closestLane.m_stopOffset = 0;
            }
            if (newLane.m_allowStop)
            {
                if (newLane.m_position < 0)
                {
                    newLane.m_stopOffset = -0.3f;
                }
                else
                {
                    newLane.m_stopOffset = 0.3f;
                }
            }

            newLane.m_laneType = closestLane.m_laneType;
            newLane.m_similarLaneCount = closestLane.m_similarLaneCount = closestLane.m_similarLaneCount + 1;
            newLane.m_similarLaneIndex = closestLane.m_similarLaneIndex + 1;
            newLane.m_speedLimit = closestLane.m_speedLimit;
            newLane.m_vehicleType = closestLane.m_vehicleType;
            newLane.m_verticalOffset = closestLane.m_verticalOffset;
            newLane.m_width = closestLane.m_width;

            NetLaneProps templateLaneProps;
            if (closestLane.m_laneProps != null)
            {
                templateLaneProps = closestLane.m_laneProps;
            }
            else
            {
                templateLaneProps = ScriptableObject.CreateInstance<NetLaneProps>();
            }

            if (templateLaneProps.m_props == null)
            {
                templateLaneProps.m_props = new NetLaneProps.Prop[0];
            }

            if (newLane.m_laneProps == null)
            {
                newLane.m_laneProps = ScriptableObject.CreateInstance<NetLaneProps>();
            }

            newLane.m_laneProps.m_props = templateLaneProps
                .m_props
                .Select(p => p.ShallowClone())
                .ToArray();
        }
    }
}
