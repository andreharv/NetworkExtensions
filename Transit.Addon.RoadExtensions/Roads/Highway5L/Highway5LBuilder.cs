using System;
using System.Linq;
using Transit.Framework;
using UnityEngine;
using Transit.Framework.Builders;

namespace Transit.Addon.RoadExtensions.Roads.Highway5L
{
    public class Highway5LBuilder : Activable, INetInfoBuilderPart
    {
        public int Order { get { return 50; } }
        public int UIOrder { get { return 15; } }

        public string BasedPrefabName { get { return NetInfos.Vanilla.ROAD_6L; } }
        public string Name { get { return "Five-Lane Highway"; } }
        public string DisplayName { get { return "Five-Lane Highway"; } }
        public string Description { get { return "A five-lane, one-way road suitable for very high and dense traffic between metropolitan areas. Lanes going the opposite direction need to be built separately. Highway does not allow zoning next to it!"; } }
        public string UICategory { get { return "RoadsHighway"; } }
        public string ThumbnailsPath { get { return @"Roads\Highway5L\thumbnails.png"; } }
        public string InfoTooltipPath { get { return @"Roads\Highway5L\infotooltip.png"; } }
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
                    (@"Roads\aHighwayTemplates\Meshes\28m\Ground.obj",
                     @"Roads\aHighwayTemplates\Meshes\28m\Ground_LOD.obj");

                nodes0.SetMeshes
                    (@"Roads\aHighwayTemplates\Meshes\28m\Ground_Node.obj",
                     @"Roads\aHighwayTemplates\Meshes\28m\Ground_Node_LOD.obj");

                nodes1.SetMeshes
                    (@"Roads\aHighwayTemplates\Meshes\28m\Ground_Trans.obj",
                     @"Roads\aHighwayTemplates\Meshes\28m\Ground_Trans_LOD.obj");

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
                    (@"Roads\aHighwayTemplates\Meshes\28m\Elevated.obj",
                    @"Roads\aHighwayTemplates\Meshes\28m\Elevated_LOD.obj");

                nodes0.SetMeshes
                    (@"Roads\aHighwayTemplates\Meshes\28m\Elevated_Node.obj",
                    @"Roads\aHighwayTemplates\Meshes\28m\Elevated_Node_LOD.obj");

                nodes1.SetMeshes
                    (@"Roads\aHighwayTemplates\Meshes\28m\Elevated_Trans.obj",
                    @"Roads\aHighwayTemplates\Meshes\28m\Elevated_Trans_LOD.obj");

                info.m_segments = new[] { segments0 };
                info.m_nodes = new[] { nodes0, nodes1 };
            }
            else if (version == NetInfoVersion.Bridge)
            {
                var segments0 = info.m_segments[0];
                var segments1 = info.m_segments[1];
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
                    (@"Roads\aHighwayTemplates\Meshes\28m\Elevated.obj",
                    @"Roads\aHighwayTemplates\Meshes\28m\Elevated_LOD.obj");

                nodes0.SetMeshes
                    (@"Roads\aHighwayTemplates\Meshes\28m\Elevated_Node.obj",
                    @"Roads\aHighwayTemplates\Meshes\28m\Elevated_Node_LOD.obj");

                nodes1.SetMeshes
                    (@"Roads\aHighwayTemplates\Meshes\28m\Elevated_Trans.obj",
                    @"Roads\aHighwayTemplates\Meshes\28m\Elevated_Trans_LOD.obj");

                info.m_segments = new[] { segments0, segments1 };
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

                //nodes0.m_flagsForbidden = NetNode.Flags.Transition;
                //nodes0.m_flagsRequired = NetNode.Flags.Underground;

                nodes1.m_flagsForbidden = NetNode.Flags.UndergroundTransition;
                nodes1.m_flagsRequired = NetNode.Flags.None;

                //nodes2.m_flagsForbidden = NetNode.Flags.None;
                //nodes2.m_flagsRequired = NetNode.Flags.UndergroundTransition;

                //nodes3.m_flagsForbidden = NetNode.Flags.Underground;
                //nodes3.m_flagsRequired = NetNode.Flags.Transition;

                segments0.SetMeshes
                    (@"Roads\aHighwayTemplates\Meshes\32m\Tunnel_Gray.obj",
                     @"Roads\aHighwayTemplates\Meshes\32m\Ground_LOD.obj");
                segments2.SetMeshes
                    (@"Roads\aHighwayTemplates\Meshes\32m\Tunnel_Node_Gray.obj",
                     @"Roads\aHighwayTemplates\Meshes\32m\Slope_LOD.obj");

                nodes0.SetMeshes
                    (@"Roads\aHighwayTemplates\Meshes\32m\Tunnel_Node_Gray.obj",
                     @"Roads\aHighwayTemplates\Meshes\32m\Ground_Node_LOD.obj");
                nodes1.SetMeshes
                    (@"Roads\aHighwayTemplates\Meshes\32m\Ground_Node.obj",
                    @"Roads\aHighwayTemplates\Meshes\32m\Ground_Node_LOD.obj");
                nodes2.SetMeshes
                    (@"Roads\aHighwayTemplates\Meshes\32m\Slope_U_Node.obj");
                nodes3.SetMeshes
                    (@"Roads\aHighwayTemplates\Meshes\32m\Ground_Trans.obj",
                     @"Roads\aHighwayTemplates\Meshes\32m\Ground_Trans_LOD.obj");

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

                //nodes2.m_flagsForbidden = NetNode.Flags.None;
                // nodes2.m_flagsRequired = NetNode.Flags.UndergroundTransition;

                segments0.SetMeshes
                    (@"Roads\aHighwayTemplates\Meshes\28m\Tunnel_Gray.obj",
                    @"Roads\aHighwayTemplates\Meshes\28m\Ground_LOD.obj");
                segments1.SetMeshes
                    (@"Roads\aHighwayTemplates\Meshes\28m\Tunnel.obj",
                    @"Roads\aHighwayTemplates\Meshes\28m\Tunnel_LOD.obj");
                nodes0.SetMeshes
                     (@"Roads\aHighwayTemplates\Meshes\28m\Tunnel_Node_Gray.obj",
                    @"Roads\aHighwayTemplates\Meshes\28m\Ground_Node_LOD.obj");
                nodes1.SetMeshes
                    (@"Roads\aHighwayTemplates\Meshes\28m\Tunnel_Node.obj",
                     @"Roads\aHighwayTemplates\Meshes\28m\Tunnel_Node_LOD.obj");
                // nodes2.SetMeshes
                //    (@"Roads\aHighwayTemplates\Meshes\28m\Tunnel.obj",
                //    @"Roads\aHighwayTemplates\Meshes\28m\Ground_LOD.obj");

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
                            @"Roads\Highway5L\Textures\Ground\Segment__MainTex.png",
                            @"Roads\Highway5L\Textures\Ground\Segment__APRMap.png"),
                        new LODTexturesSet
                           (@"Roads\Highway5L\Textures\Ground\SegmentLOD__MainTex.png",
                            @"Roads\Highway5L\Textures\Ground\SegmentLOD__APRMap.png",
                            @"Roads\Highway5L\Textures\Ground\SegmentLOD__XYSMap.png"));
                    info.SetAllNodesTexture(
                        new TexturesSet
                           (@"Roads\Highway5L\Textures\Ground\Node__MainTex.png",
                            @"Roads\Highway5L\Textures\Ground\Node__APRMap.png"),
                        new LODTexturesSet
                           (@"Roads\Highway5L\Textures\Ground\NodeLOD__MainTex.png",
                            @"Roads\Highway5L\Textures\Ground\NodeLOD__APRMap.png",
                            @"Roads\Highway5L\Textures\Ground\NodeLOD__XYSMap.png"));
                    break;

                case NetInfoVersion.Elevated:
                case NetInfoVersion.Bridge:
                    info.SetAllSegmentsTexture(
                        new TexturesSet(
                            @"Roads\Highway5L\Textures\ElevatedBridge\Segment__MainTex.png",
                            @"Roads\Highway5L\Textures\ElevatedBridge\Segment__APRMap.png"),
                        new LODTexturesSet
                           (@"Roads\Highway5L\Textures\ElevatedBridge\SegmentLOD__MainTex.png",
                            @"Roads\Highway5L\Textures\ElevatedBridge\SegmentLOD__APRMap.png",
                            @"Roads\Highway5L\Textures\ElevatedBridge\LOD__XYSMap.png"));
                    info.SetAllNodesTexture(
                        new TexturesSet
                           (@"Roads\Highway5L\Textures\ElevatedBridge\Node__MainTex.png",
                            @"Roads\Highway5L\Textures\ElevatedBridge\Node__APRMap.png"),
                        new LODTexturesSet
                           (@"Roads\Highway5L\Textures\ElevatedBridge\NodeLOD__MainTex.png",
                            @"Roads\Highway5L\Textures\ElevatedBridge\NodeLOD__APRMap.png",
                            @"Roads\Highway5L\Textures\ElevatedBridge\LOD__XYSMap.png"));
                    break;

                case NetInfoVersion.Slope:
                    info.SetAllSegmentsTexture(
                        new TexturesSet
                           (@"Roads\Highway5L\Textures\Slope\Segment__MainTex.png",
                            @"Roads\Highway5L\Textures\Slope\Segment__APRMap.png"),
                        new LODTexturesSet
                            (@"Roads\Highway5L\Textures\Slope\SegmentLOD__MainTex.png",
                            @"Roads\Highway5L\Textures\Slope\SegmentLOD__APRMap.png",
                            @"Roads\Highway5L\Textures\Slope\SegmentLOD__XYSMap.png"));
                    info.SetAllNodesTexture(
                        new TexturesSet
                           (@"Roads\Highway6L\Textures\Tunnel\Node__MainTex.png",
                            @"Roads\Highway6L\Textures\Ground\Node__APRMap.png"),
                        new LODTexturesSet
                           (@"Roads\Highway6L\Textures\Ground\NodeLOD__MainTex.png",
                            @"Roads\Highway6L\Textures\Ground\NodeLOD__APRMap.png",
                            @"Roads\Highway6L\Textures\Ground\NodeLOD__XYSMap.png"));
                    break;
                case NetInfoVersion.Tunnel:
                    info.SetAllSegmentsTexture(
                        new TexturesSet
                           (@"Roads\Highway5L\Textures\Tunnel\Segment__MainTex.png",
                            @"Roads\Highway5L\Textures\Tunnel\Segment__APRMap.png"));
                    //new LODTexturesSet
                    //   (@"Roads\Highway5L\Textures\Tunnel\SegmentLOD__MainTex.png",
                    //    @"Roads\Highway5L\Textures\Tunnel\SegmentLOD__APRMap.png",
                    //    @"Roads\Highway5L\Textures\Slope\NodeLOD__XYSMap.png"));
                    info.SetAllNodesTexture(
                        new TexturesSet
                           (@"Roads\Highway5L\Textures\Tunnel\Node__MainTex.png",
                            @"Roads\Highway5L\Textures\Tunnel\Segment__APRMap.png"));
                    //new LODTexturesSet
                    //   (@"Roads\Highway5L\Textures\Tunnel\NodeLOD__MainTex.png",
                    //    @"Roads\Highway5L\Textures\Tunnel\SegmentLOD__APRMap.png",
                    //    @"Roads\Highway5L\Textures\Slope\NodeLOD__XYSMap.png"));
                    break;
            }

            ///////////////////////////
            // Set up                //
            ///////////////////////////
            info.m_availableIn = ItemClass.Availability.All;
            info.m_class = highwayInfo.m_class.Clone(NetInfoClasses.NEXT_HIGHWAY5L);
            info.m_surfaceLevel = 0;
            info.m_createPavement = !(version == NetInfoVersion.Ground || version == NetInfoVersion.Tunnel);
            info.m_createGravel = version == NetInfoVersion.Ground;
            info.m_averageVehicleLaneSpeed = 2f;
            info.m_hasParkingSpaces = false;
            info.m_hasPedestrianLanes = false;
            info.m_halfWidth = (version == NetInfoVersion.Slope ? 16 : 14);
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
            var tempLanes = info.m_lanes.ToList();
            var vehicleLaneToRemove = tempLanes.Where(tl => tl.m_laneType == NetInfo.LaneType.Vehicle).First();
            tempLanes.Remove(vehicleLaneToRemove);
            info.m_lanes = tempLanes.ToArray();

            var vehicleLanes = info.m_lanes
                .Where(l => l.m_laneType != NetInfo.LaneType.None)
                .OrderBy(l => l.m_similarLaneIndex)
                .ToArray();

            var propLanes = info.m_lanes
                .Where(l => l.m_laneType != NetInfo.LaneType.Vehicle)
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
                playerNetAI.m_constructionCost = hwPlayerNetAI.m_constructionCost * 5 / 3;
                playerNetAI.m_maintenanceCost = hwPlayerNetAI.m_maintenanceCost * 5 / 3;
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
                    newProps.name = "Highway5L Left Props";

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
                    newProps.name = "Highway5L Right Props";

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

            foreach (var lane in propLanes)
            {
                if (lane.m_laneProps != null && lane.m_laneProps.m_props.Length > 0)
                {
                    foreach (var prop in lane.m_laneProps.m_props)
                    {
                        var propName = prop.m_prop.name;
                        var positionMultiplier = lane.m_position / Math.Abs(lane.m_position);
                        prop.m_position.x = 0;
                    }
                }
            }
            if (leftHwLane != null && rightHwLane != null)
            {
                var leftHwProps = leftHwLane.m_laneProps.m_props.ToList();
                var rightHwProps = rightHwLane.m_laneProps.m_props.ToList();

                var wallLightProp = new NetLaneProps.Prop();
                var wallLightPropInfo = Prefabs.Find<PropInfo>("Wall Light Orange", false);
                var streetLightPropInfo = Prefabs.Find<PropInfo>("New Street Light", false);

                NetLaneProps.Prop streetLightLeft = null;
                NetLaneProps.Prop streetLightRight = null;

                foreach (var prop in rightHwLane.m_laneProps.m_props)
                {
                    if (prop != null && prop.m_prop != null && prop.m_prop.name != null && prop.m_prop.name.Contains("New Street Light"))
                    {
                        streetLightRight = prop;
                        streetLightLeft = prop.ShallowClone();
                        break;
                    }
                }

                if (streetLightLeft != null && streetLightRight != null)
                {
                    streetLightLeft.m_angle = 180;
                    if (version == NetInfoVersion.Tunnel)
                    {
                        streetLightLeft.m_repeatDistance = 40;
                        streetLightRight.m_repeatDistance = 40;

                        streetLightLeft.m_segmentOffset = 0;
                        streetLightRight.m_segmentOffset = 0;

                        streetLightLeft.m_position = new UnityEngine.Vector3(-3.2f, -4.5f, 20);
                        streetLightRight.m_position = new UnityEngine.Vector3(3.2f, -4.5f, 0);

                        //extra strength lighting (x2)
                        leftHwProps.Add(streetLightLeft);
                        leftHwProps.Add(streetLightLeft);

                        rightHwProps.Add(streetLightRight);
                    }
                    else if (version == NetInfoVersion.Slope)
                    {
                        wallLightProp.m_prop = wallLightPropInfo.ShallowClone();
                        wallLightProp.m_finalProp = wallLightPropInfo.ShallowClone();
                        wallLightProp.m_probability = 100;
                        wallLightProp.m_repeatDistance = 10;
                        wallLightProp.m_segmentOffset = 0;
                        wallLightProp.m_prop.m_effects[0].m_direction = new UnityEngine.Vector3(0, -90, 25);
                        wallLightProp.m_finalProp.m_effects[0].m_direction = new UnityEngine.Vector3(0, -90, 25);
                        var wallLightPropLeft = wallLightProp.ShallowClone();
                        var wallLightPropRight = wallLightProp.ShallowClone();
                        wallLightPropLeft.m_angle = 270;
                        wallLightPropRight.m_angle = 90;
                        wallLightPropLeft.m_position = new UnityEngine.Vector3(-2, 1.5f, 0);
                        wallLightPropRight.m_position = new UnityEngine.Vector3(2, 1.5f, 0);

                        streetLightLeft.m_repeatDistance = 80;
                        streetLightRight.m_repeatDistance = 80;
                        streetLightLeft.m_segmentOffset = 0;
                        streetLightRight.m_segmentOffset = 0;
                        streetLightLeft.m_position = new UnityEngine.Vector3(-3, -3, 0);
                        streetLightRight.m_position = new UnityEngine.Vector3(3, -3, 0);


                        leftHwProps.Add(streetLightLeft);
                        leftHwProps.Add(streetLightLeft);
                        leftHwProps.Add(wallLightPropLeft);

                        rightHwProps.Add(streetLightRight);
                        rightHwProps.Add(wallLightPropRight);
                    }
                    else
                    {
                        streetLightRight.m_repeatDistance = 80;
                        streetLightLeft.m_repeatDistance = 80;

                        streetLightLeft.m_segmentOffset = 39;
                        streetLightLeft.m_endFlagsForbidden = NetNode.Flags.TrafficLights;

                        if (version == NetInfoVersion.Bridge || version == NetInfoVersion.Elevated)
                        {
                            streetLightLeft.m_position = new UnityEngine.Vector3(-1.75f, 0, 0);
                            streetLightRight.m_position = new UnityEngine.Vector3(1.75f, 0, 0);
                        }
                        else
                        {
                            streetLightLeft.m_position = new UnityEngine.Vector3(1, 0, 0);
                            streetLightRight.m_position = new UnityEngine.Vector3(-1, 0, 0);
                        }

                        leftHwProps.Add(streetLightLeft);
                    }
                }

                leftHwLane.m_laneProps.m_props = leftHwProps.ToArray();
                rightHwLane.m_laneProps.m_props = rightHwProps.ToArray();
            }
        }
    }
}
