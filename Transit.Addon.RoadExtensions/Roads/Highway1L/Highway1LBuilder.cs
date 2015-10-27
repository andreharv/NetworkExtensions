using System;
using System.Collections.Generic;
using System.Linq;
using Transit.Framework;
using Transit.Framework.Builders;
using Transit.Framework.Interfaces;
using Transit.Framework.Modularity;
using UnityEngine;

#if DEBUG
using Debug = Transit.Framework.Debug;
#endif

namespace Transit.Addon.RoadExtensions.Roads.Highway1L
{
    public class Highway1LBuilder : Activable, INetInfoBuilderPart
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
            var defaultMaterial = highwayInfo.m_nodes[0].m_material;

            ///////////////////////////
            // 3DModeling            //
            ///////////////////////////
            if (version == NetInfoVersion.Ground)
            {
                var segments0 = info.m_segments[0];
                var nodes0 = info.m_nodes[0];

                segments0.m_backwardForbidden = NetSegment.Flags.None;
                segments0.m_backwardRequired = NetSegment.Flags.None;

                segments0.m_forwardForbidden = NetSegment.Flags.None;
                segments0.m_forwardRequired = NetSegment.Flags.None;

                var nodes1 = nodes0.ShallowClone();

                nodes0.m_flagsForbidden = NetNode.Flags.Transition;
                nodes0.m_flagsRequired = NetNode.Flags.None;

                nodes1.m_flagsForbidden = NetNode.Flags.None;
                nodes1.m_flagsRequired = NetNode.Flags.Transition;

                segments0.SetMeshes
                    (@"Roads\aHighwayTemplates\Meshes\16m\Ground.obj",
                    @"Roads\aHighwayTemplates\Meshes\16m\Ground_LOD.obj");

                nodes0.SetMeshes
                    (@"Roads\aHighwayTemplates\Meshes\16m\Ground_Node.obj",
                    @"Roads\aHighwayTemplates\Meshes\16m\Ground_Node_LOD.obj");

                nodes1.SetMeshes
                    (@"Roads\aHighwayTemplates\Meshes\16m\Ground_Trans.obj",
                    @"Roads\aHighwayTemplates\Meshes\16m\Ground_Trans_LOD.obj");

                info.m_segments = new[] { segments0 };
                info.m_nodes = new[] { nodes0, nodes1 };
            }
            else if (version == NetInfoVersion.Elevated)
            {
                var segments0 = info.m_segments[0];
                var nodes0 = info.m_nodes[0];

                segments0.m_backwardForbidden = NetSegment.Flags.None;
                segments0.m_backwardRequired = NetSegment.Flags.None;

                segments0.m_forwardForbidden = NetSegment.Flags.None;
                segments0.m_forwardRequired = NetSegment.Flags.None;

                var nodes1 = nodes0.ShallowClone();

                nodes0.m_flagsForbidden = NetNode.Flags.Transition;
                nodes0.m_flagsRequired = NetNode.Flags.None;

                nodes1.m_flagsForbidden = NetNode.Flags.None;
                nodes1.m_flagsRequired = NetNode.Flags.Transition;

                segments0.SetMeshes
                    (@"Roads\aHighwayTemplates\Meshes\16m\Elevated.obj",
                     @"Roads\aHighwayTemplates\Meshes\16m\Elevated_LOD.obj");

                nodes0.SetMeshes
                    (@"Roads\aHighwayTemplates\Meshes\16m\Elevated_Node.obj",
                    @"Roads\aHighwayTemplates\Meshes\16m\Elevated_Node_LOD.obj");

                nodes1.SetMeshes
                    (@"Roads\aHighwayTemplates\Meshes\16m\Elevated_Trans.obj",
                    @"Roads\aHighwayTemplates\Meshes\16m\Elevated_Trans_LOD.obj");

                info.m_segments = new[] { segments0 };
                info.m_nodes = new[] { nodes0, nodes1 };
            }
            else if (version == NetInfoVersion.Bridge)
            {
                var segments0 = info.m_segments[0];
                var nodes0 = info.m_nodes[0];

                segments0.m_backwardForbidden = NetSegment.Flags.None;
                segments0.m_backwardRequired = NetSegment.Flags.None;

                segments0.m_forwardForbidden = NetSegment.Flags.None;
                segments0.m_forwardRequired = NetSegment.Flags.None;

                var nodes1 = nodes0.ShallowClone();

                nodes0.m_flagsForbidden = NetNode.Flags.Transition;
                nodes0.m_flagsRequired = NetNode.Flags.None;

                nodes1.m_flagsForbidden = NetNode.Flags.None;
                nodes1.m_flagsRequired = NetNode.Flags.Transition;

                segments0.SetMeshes
                    (@"Roads\aHighwayTemplates\Meshes\16m\Elevated.obj",
                     @"Roads\aHighwayTemplates\Meshes\16m\Elevated_LOD.obj");

                nodes0.SetMeshes
                    (@"Roads\aHighwayTemplates\Meshes\16m\Elevated_Node.obj",
                    @"Roads\aHighwayTemplates\Meshes\16m\Elevated_Node_LOD.obj");

                nodes1.SetMeshes
                    (@"Roads\aHighwayTemplates\Meshes\16m\Elevated_Trans.obj",
                    @"Roads\aHighwayTemplates\Meshes\16m\Elevated_Trans_LOD.obj");

                info.m_segments = new[] { segments0 };
                info.m_nodes = new[] { nodes0, nodes1 };
            }
            else if (version == NetInfoVersion.Slope)
            {
                var segments0 = info.m_segments[0];
                var segments1 = info.m_segments[1];
                var segments2 = segments1.ShallowClone();
                var nodes0 = info.m_nodes[0];
                var nodes1 = info.m_nodes[1];
                var nodes2 = nodes0.ShallowClone();
                var nodes3 = nodes1.ShallowClone();

                segments0.m_backwardForbidden = NetSegment.Flags.None;
                segments0.m_backwardRequired = NetSegment.Flags.None;

                segments0.m_forwardForbidden = NetSegment.Flags.None;
                segments0.m_forwardRequired = NetSegment.Flags.None;

                segments1.m_backwardForbidden = NetSegment.Flags.None;
                segments1.m_backwardRequired = NetSegment.Flags.None;

                segments1.m_forwardForbidden = NetSegment.Flags.None;
                segments1.m_forwardRequired = NetSegment.Flags.None;

                segments2.m_backwardForbidden = NetSegment.Flags.None;
                segments2.m_backwardRequired = NetSegment.Flags.None;

                segments2.m_forwardForbidden = NetSegment.Flags.None;
                segments2.m_forwardRequired = NetSegment.Flags.None;

                nodes0.m_flagsForbidden = NetNode.Flags.None;
                nodes0.m_flagsRequired = NetNode.Flags.Underground;

                nodes1.m_flagsForbidden = NetNode.Flags.UndergroundTransition;
                nodes1.m_flagsRequired = NetNode.Flags.None;

                nodes2.m_flagsForbidden = NetNode.Flags.None;
                nodes2.m_flagsRequired = NetNode.Flags.Underground;

                nodes3.m_flagsForbidden = NetNode.Flags.None;
                nodes3.m_flagsRequired = NetNode.Flags.Transition;

                segments0.SetMeshes
                    (@"Roads\aHighwayTemplates\Meshes\16m\Tunnel_Gray.obj",
                    @"Roads\aHighwayTemplates\Meshes\16m\Ground_LOD.obj");
                segments2.SetMeshes
                    (@"Roads\aHighwayTemplates\Meshes\16m\Slope.obj",
                    @"Roads\aHighwayTemplates\Meshes\16m\Slope_LOD.obj");

                nodes0.SetMeshes
                    (@"Roads\aHighwayTemplates\Meshes\16m\Tunnel_Node_Gray.obj",
                    @"Roads\aHighwayTemplates\Meshes\16m\Ground_Node_LOD.obj");
                nodes1.SetMeshes
                    (@"Roads\aHighwayTemplates\Meshes\16m\Ground_Node.obj",
                    @"Roads\aHighwayTemplates\Meshes\16m\Ground_Node_LOD.obj");
                nodes2.SetMeshes
                    (@"Roads\aHighwayTemplates\Meshes\16m\Slope_U_Node.obj",
                    @"Roads\aHighwayTemplates\Meshes\16m\Slope_U_Node_LOD.obj");
                nodes3.SetMeshes
                    (@"Roads\aHighwayTemplates\Meshes\16m\Ground_Trans.obj",
                    @"Roads\aHighwayTemplates\Meshes\16m\Ground_Trans_LOD.obj");

                nodes2.m_material = defaultMaterial;

                info.m_segments = new[] { segments0, segments1, segments2 };
                info.m_nodes = new[] { nodes0, nodes1, nodes2, nodes3 };
            }
            else if (version == NetInfoVersion.Tunnel)
            {
                var segments0 = info.m_segments[0];
                var segments1 = segments0.ShallowClone();
                var nodes0 = info.m_nodes[0];
                var nodes1 = nodes0.ShallowClone();
                //var nodes2 = nodes1.ShallowClone();

                //segments1.m_backwardForbidden = NetSegment.Flags.None;
                //segments1.m_backwardRequired = NetSegment.Flags.None;

                //segments1.m_forwardForbidden = NetSegment.Flags.None;
                //segments1.m_forwardRequired = NetSegment.Flags.None;
                nodes0.m_flagsForbidden = NetNode.Flags.None;
                nodes0.m_flagsRequired = NetNode.Flags.None;

                nodes1.m_flagsForbidden = NetNode.Flags.None;
                nodes1.m_flagsRequired = NetNode.Flags.None;

                //nodes1.m_flagsForbidden = NetNode.Flags.Transition;
                //nodes1.m_flagsRequired = NetNode.Flags.Underground;

                //nodes2.m_flagsForbidden = NetNode.Flags.None;
                // nodes2.m_flagsRequired = NetNode.Flags.UndergroundTransition;

                segments0.SetMeshes
                    (@"Roads\aHighwayTemplates\Meshes\16m\Tunnel_Gray.obj",
                    @"Roads\aHighwayTemplates\Meshes\16m\Ground_LOD.obj");
                segments1.SetMeshes
                    (@"Roads\aHighwayTemplates\Meshes\16m\Tunnel.obj",
                    @"Roads\aHighwayTemplates\Meshes\16m\Tunnel_LOD.obj");
                nodes0.SetMeshes
                     (@"Roads\aHighwayTemplates\Meshes\16m\Tunnel_Node_Gray.obj",
                    @"Roads\aHighwayTemplates\Meshes\16m\Ground_Node_LOD.obj");
                nodes1.SetMeshes
                    (@"Roads\aHighwayTemplates\Meshes\16m\Tunnel_Node.obj",
                    @"Roads\aHighwayTemplates\Meshes\16m\Tunnel_Node_LOD.obj");

                // nodes2.SetMeshes
                //    (@"Roads\aHighwayTemplates\Meshes\16m\Tunnel.obj",
                //    @"Roads\aHighwayTemplates\Meshes\16m\Ground_LOD.obj");

                segments1.m_material = defaultMaterial;
                nodes1.m_material = defaultMaterial;
                //nodes2.m_material = defaultMaterial;

                segments1.m_surfaceMapping = new UnityEngine.Vector4(0, 0, 0, 0);
                nodes1.m_surfaceMapping = new UnityEngine.Vector4(0, 0, 0, 0);
                //nodes2.m_surfaceMapping = new UnityEngine.Vector4(0, 0, 0, 0);

                info.m_segments = new[] { segments0, segments1 };
                info.m_nodes = new[] { nodes0, nodes1 };
            }

            ///////////////////////////
            // Texturing             //
            ///////////////////////////
            switch (version)
            {
                case NetInfoVersion.Ground:
                    info.SetAllSegmentsTexture(
                       new TexturesSet(
                           @"Roads\Highway1L\Textures\Ground_Elevated_Segment__MainTex.png",
                           @"Roads\Highway1L\Textures\Ground_Segment__APRMap.png"),
                    new LODTexturesSet
                       (@"Roads\Highway1L\Textures\Ground_SegmentLOD__MainTex.png",
                        @"Roads\Highway1L\Textures\Ground_SegmentLOD__APRMap.png",
                        @"Roads\Highway1L\Textures\Ground_LOD__XYSMap.png"));
                    info.SetAllNodesTexture(
                        new TexturesSet
                           (@"Roads\Highway1L\Textures\Ground_Elevated_Node__MainTex.png",
                            @"Roads\Highway1L\Textures\Ground_Node__APRMap.png"),
                        new LODTexturesSet
                           (@"Roads\Highway1L\Textures\Ground_NodeLOD__MainTex.png",
                            @"Roads\Highway1L\Textures\Ground_NodeLOD__APRMap.png",
                            @"Roads\Highway1L\Textures\Ground_LOD__XYSMap.png"));
                    break;
                case NetInfoVersion.Elevated:
                case NetInfoVersion.Bridge:
                    info.SetAllSegmentsTexture(
                    new TexturesSet(
                        @"Roads\Highway1L\Textures\Ground_Elevated_Segment__MainTex.png",
                        @"Roads\Highway1L\Textures\Elevated_Segment__APRMap.png"),
                        new LODTexturesSet
                           (@"Roads\Highway1L\Textures\Elevated_SegmentLOD__MainTex.png",
                            @"Roads\Highway1L\Textures\Elevated_SegmentLOD__APRMap.png",
                            @"Roads\Highway1L\Textures\Elevated_NodeLOD__XYSMap.png"));
                    info.SetAllNodesTexture(
                        new TexturesSet
                           (@"Roads\Highway1L\Textures\Ground_Elevated_Node__MainTex.png",
                            @"Roads\Highway1L\Textures\Elevated_Node__APRMap.png"),
                        new LODTexturesSet
                           (@"Roads\Highway1L\Textures\Elevated_NodeLOD__MainTex.png",
                            @"Roads\Highway1L\Textures\Elevated_NodeLOD__APRMap.png",
                            @"Roads\Highway1L\Textures\Elevated_NodeLOD__XYSMap.png"));
                    break;

                case NetInfoVersion.Slope:
                    info.SetAllSegmentsTexture(
                        new TexturesSet
                           (@"Roads\Highway1L\Textures\Slope_Segment__MainTex.png",
                            @"Roads\Highway1L\Textures\Slope_Segment_Open__APRMap.png"),
                    new LODTexturesSet
                        (@"Roads\Highway1L\Textures\Slope_SegmentLOD__MainTex.png",
                        @"Roads\Highway1L\Textures\Slope_SegmentLOD__APRMap.png",
                        @"Roads\Highway1L\Textures\Slope_SegmentLOD__XYSMap.png"));
                    info.SetAllNodesTexture(
                        new TexturesSet
                           (@"Roads\Highway1L\Textures\Slope_Node__MainTex.png",
                            @"Roads\Highway1L\Textures\Ground_Node__APRMap.png"),
                        new LODTexturesSet
                           (@"Roads\Highway1L\Textures\Ground_NodeLOD__MainTex.png",
                            @"Roads\Highway1L\Textures\Ground_NodeLOD__APRMap.png",
                            @"Roads\Highway1L\Textures\Ground_LOD__XYSMap.png"));
                    break;
                case NetInfoVersion.Tunnel:
                    info.SetAllSegmentsTexture(
                        new TexturesSet
                           (@"Roads\Highway1L\Textures\Tunnel_Segment__MainTex.png",
                            @"Roads\Highway1L\Textures\Tunnel__APRMap.png"),
                    new LODTexturesSet
                       (@"Roads\Highway1L\Textures\Tunnel_SegmentLOD__MainTex.png",
                        @"Roads\Highway1L\Textures\TunnelLOD__APRMap.png",
                        @"Roads\Highway1L\Textures\Slope_NodeLOD__XYSMap.png"));
                    info.SetAllNodesTexture(
                        new TexturesSet
                           (@"Roads\Highway1L\Textures\Tunnel_Node__MainTex.png",
                            @"Roads\Highway1L\Textures\Tunnel__APRMap.png"),
                        new LODTexturesSet
                           (@"Roads\Highway1L\Textures\Tunnel_NodeLOD__MainTex.png",
                            @"Roads\Highway1L\Textures\TunnelLOD__APRMap.png",
                            @"Roads\Highway1L\Textures\Slope_NodeLOD__XYSMap.png"));
                    break;
            }

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
            var vehicleLanes = info.m_lanes
                .Where(l => l.m_laneType != NetInfo.LaneType.None)
                .OrderBy(l => l.m_similarLaneIndex)
                .ToArray();

            var propLanes = info.m_lanes
                .Where(l => l.m_laneType == NetInfo.LaneType.None)
                .OrderBy(l => l.m_similarLaneIndex)
                .ToArray();

            var nbLanes = vehicleLanes.Count();

            const float laneWidth = 4f;
            var positionStart = (laneWidth * ((1f - nbLanes) / 2f));

            for (int i = 0; i < vehicleLanes.Length; i++)
            {
                var l = vehicleLanes[i];
                l.m_allowStop = false;
                l.m_speedLimit = 2f;
                l.m_verticalOffset = 0f;
                l.m_width = laneWidth;
                l.m_position = positionStart + i * laneWidth;
            }
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

            info.SetHighwayProps(highwayInfo);
            info.TrimHighwayProps();

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
                        leftHwLane.m_width = info.m_pavementWidth - 1;
                        leftHwLane.m_position = (info.m_halfWidth * -1) + (leftHwLane.m_width * 0.5f + 1);
                    }
                    else if (info.m_lanes[i].m_laneType == NetInfo.LaneType.None && counter == 1)
                    {
                        rightHwLane = info.m_lanes[i];
                        rightHwLane.m_width = info.m_pavementWidth - 1;
                        rightHwLane.m_position = (info.m_halfWidth) - (rightHwLane.m_width * 0.5f + 1);
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
                    newProps.name = "Highway1L Left Props";

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
                    newProps.name = "Highway1L Right Props";

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

                foreach (var prop in rightHwLane.m_laneProps.m_props)
                {
                    if (prop != null && prop.m_prop != null && prop.m_prop.name != null && prop.m_prop.name.Contains("New Street Light"))
                    {
                        streetLightRight = prop;
                        break;
                    }
                }

                var invertedProps = leftHwProps.Where(lp => lp.m_flagsForbidden == NetLane.Flags.Inverted).ToArray();
                if (invertedProps != null)
                {
                    for (int i = 0; i < invertedProps.Length; i++)
                    {
                        invertedProps[i].m_startFlagsForbidden = NetNode.Flags.None;
                        invertedProps[i].m_startFlagsRequired = NetNode.Flags.None;
                        invertedProps[i].m_endFlagsForbidden = NetNode.Flags.None;
                        invertedProps[i].m_endFlagsRequired = NetNode.Flags.Transition;
                        invertedProps[i].m_angle = 180;
                        invertedProps[i].m_position.z *= -1;
                        invertedProps[i].m_segmentOffset *= -1;
                    }
                }
                if (version == NetInfoVersion.Elevated || version == NetInfoVersion.Bridge)
                {
                    for (int i = 0; i < leftHwProps.Count; i++)
                    {
                        leftHwProps[i].m_position.x = -1.55f;
                    }
                }

                if (version == NetInfoVersion.Elevated || version == NetInfoVersion.Bridge)
                {
                    for (int i = 0; i < rightHwProps.Count; i++)
                    {
                        rightHwProps[i].m_position.x = 1.55f;
                    }
                }

                //Replace 1 way traffic lights with 2 way traffic lights
                foreach (var prop in info.m_lanes.Where(l => l != null && l.m_laneProps != null && l.m_laneProps.name.ToLower().Contains("left")).FirstOrDefault().m_laneProps.m_props.Where(lp => lp.m_prop.name.Contains("Traffic")))
                {
                    leftHwProps.Remove(prop);
                }

                foreach (var prop in info.m_lanes.Where(l => l != null && l.m_laneProps != null && l.m_laneProps.name.ToLower().Contains("right")).FirstOrDefault().m_laneProps.m_props.Where(lp => lp.m_prop.name.Contains("Traffic")))
                {
                    rightHwProps.Remove(prop);
                }

                foreach (var prop in basicRoadInfo.m_lanes.Where(l => l != null && l.m_laneProps != null && l.m_laneProps.name.ToLower().Contains("left")).FirstOrDefault().m_laneProps.m_props.Where(lp => lp.m_prop.name.Contains("Traffic")))
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

                foreach (var prop in basicRoadInfo.m_lanes.Where(l => l != null && l.m_laneProps != null && l.m_laneProps.name.ToLower().Contains("right")).FirstOrDefault().m_laneProps.m_props.Where(lp => lp.m_prop.name.Contains("Traffic")))
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

                if (streetLightRight != null)
                {
                    if (version == NetInfoVersion.Tunnel)
                    {
                        streetLightRight.m_repeatDistance = 40;
                        streetLightRight.m_segmentOffset = 0;
                        streetLightRight.m_position = new UnityEngine.Vector3(3.2f, -4.5f, 0);

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
                        wallLightPropLeft.m_position = new UnityEngine.Vector3(-1, 1.5f, 0);
                        wallLightPropRight.m_position = new UnityEngine.Vector3(1, 1.5f, 0);

                        streetLightRight.m_repeatDistance = 80;
                        streetLightRight.m_segmentOffset = 0;
                        streetLightRight.m_position = new UnityEngine.Vector3(1.75f, -3, 0);

                        leftHwProps.Add(wallLightPropLeft);

                        rightHwProps.Add(streetLightRight);
                        rightHwProps.Add(wallLightPropRight);
                    }
                    else
                    {
                        streetLightRight.m_repeatDistance = 80;

                        if (version == NetInfoVersion.Bridge || version == NetInfoVersion.Elevated)
                        {
                            streetLightRight.m_position = new UnityEngine.Vector3(1.75f, -3, 0);
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
                            prop.m_position = new UnityEngine.Vector3(0, 0, 0);
                        }
                    }
                }

                //foreach (var lane in propLanes)
                //{
                //    if (lane.m_laneProps != null && lane.m_laneProps.m_props.Length > 0)
                //    {
                //        foreach (var prop in lane.m_laneProps.m_props)
                //        {
                //            var propName = prop.m_prop.name;
                //            var positionMultiplier = lane.m_position / Math.Abs(lane.m_position);
                //            if (!propName.Contains(streetLightPropInfo.name) && propName != wallLightPropInfo.name)
                //            {
                //                if (version != NetInfoVersion.Elevated && version != NetInfoVersion.Bridge)
                //                {
                //                    prop.m_position.x = 0;
                //                }
                //            }
                //        }
                //    }
                //}
            }
        }
    }
}
