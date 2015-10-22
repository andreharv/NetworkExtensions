using System;
using System.Linq;
using Transit.Framework;
using Transit.Framework.Modularity;
using Transit.Addon.RoadExtensions.Menus;
using UnityEngine;

namespace Transit.Addon.RoadExtensions.Roads.SmallAvenue4L
{
    public class SmallAvenue4LBuilder : Activable, INetInfoBuilderPart
    {
        public int Order { get { return 10; } }
        public int UIOrder { get { return 20; } }

        public string BasedPrefabName { get { return NetInfos.Vanilla.ROAD_2L; } }
        public string Name { get { return "Small Avenue"; } }
        public string DisplayName { get { return "Small Four-Lane Road"; } }
        public string Description { get { return "A four-lane road without parkings spaces. Supports medium traffic."; } }
        public string UICategory { get { return AdditionnalMenus.ROADS_SMALL_HV; } }

        public string ThumbnailsPath    { get { return @"Roads\SmallAvenue4L\thumbnails.png"; } }
        public string InfoTooltipPath   { get { return @"Roads\SmallAvenue4L\infotooltip.png"; } }

        public NetInfoVersion SupportedVersions
        {
            get { return NetInfoVersion.Ground; }
        }

        public void BuildUp(NetInfo info, NetInfoVersion version)
        {
            ///////////////////////////
            // Template              //
            ///////////////////////////
            var basicRoadInfo = Prefabs.Find<NetInfo>(NetInfos.Vanilla.ROAD_2L);

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
                           (@"Roads\SmallAvenue4L\Textures\Ground_Segment__MainTex.png",
                            @"Roads\SmallAvenue4L\Textures\Ground_Segment__AlphaMap.png"),
                        new TexturesSet
                           (@"Roads\SmallAvenue4L\Textures\Ground_SegmentLOD__MainTex.png",
                            @"Roads\SmallAvenue4L\Textures\Ground_SegmentLOD__AlphaMap.png",
                            @"Roads\SmallAvenue4L\Textures\Ground_SegmentLOD__XYS.png"));
                    break;
            }


            ///////////////////////////
            // Set up                //
            ///////////////////////////
            info.m_hasParkingSpaces = false;
            info.m_class = basicRoadInfo.m_class.Clone(NetInfoClasses.NEXT_SMALL4L_ROAD);
            info.m_pavementWidth = 2;

            // Setting up lanes
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
            
            const float outerCarLanePosition = 4.4f;
            const float innerCarLanePosition = 1.5f;
            const float pedLanePosition = 8f;
            const float pedLaneWidth = 1.5f;

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
                    case 1: lane.m_position = -innerCarLanePosition; break;
                    case 2: lane.m_position = innerCarLanePosition; break;
                    case 3: lane.m_position = outerCarLanePosition; break;
                }
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


            if (version == NetInfoVersion.Ground)
            {
                var brPlayerNetAI = basicRoadInfo.GetComponent<PlayerNetAI>();
                var playerNetAI = info.GetComponent<PlayerNetAI>();

                if (brPlayerNetAI != null && playerNetAI != null)
                {
                    playerNetAI.m_constructionCost = brPlayerNetAI.m_constructionCost * 125 / 100; // 25% increase
                    playerNetAI.m_maintenanceCost = brPlayerNetAI.m_maintenanceCost * 125 / 100; // 25% increase
                }
            }
            else // Same as the original basic road specs
            {

            } 
            
            // Should we put traffic lights?
            //var roadBaseAI = info.GetComponent<RoadBaseAI>();

            //if (roadBaseAI != null)
            //{
            //    roadBaseAI.m_trafficLights = true;
            //}
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
