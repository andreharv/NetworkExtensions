using System.Collections.Generic;
using System.Linq;
using Transit.Framework;
using Transit.Framework.Modularity;

namespace Transit.Addon.RoadExtensions.Roads.Highway6L
{
    public class Highway6LBuilder : NetInfoBuilderBase, INetInfoBuilder, INetInfoModifier
    {
        public int Order { get { return 50; } }
        public int Priority { get { return 14; } }

        public string TemplatePrefabName { get { return NetInfos.Vanilla.ONEWAY_6L; } }
        public string Name { get { return "Large Highway"; } }
        public string DisplayName { get { return "Six-Lane Highway"; } }
        public string CodeName { get { return "HIGHWAY_6L"; } }
        public string Description { get { return "A six-lane, one-way road suitable for very high and dense traffic between metropolitan areas. Lanes going the opposite direction need to be built separately. Highway does not allow zoning next to it!"; } }
        public string UICategory { get { return "RoadsHighway"; } }

        public string ThumbnailsPath { get { return @"NewNetwork\Highway6L\thumbnails.png"; } }
        public string InfoTooltipPath { get { return @"NewNetwork\Highway6L\infotooltip.png"; } }

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
                info.m_surfaceLevel = 0;
                info.m_class = highwayInfo.m_class.Clone("NExtHighway");

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
                    (@"NewNetwork\Highway6L\Meshes\Ground.obj",
                     @"NewNetwork\Highway6L\Meshes\Ground_LOD.obj");

                nodes0.SetMeshes
                    (@"NewNetwork\Highway6L\Meshes\Ground.obj",
                     @"NewNetwork\Highway6L\Meshes\Ground_LOD.obj");

                nodes1.SetMeshes
                    (@"NewNetwork\Highway6L\Meshes\Ground_Trans.obj",
                     @"NewNetwork\Highway6L\Meshes\Ground_Trans_LOD.obj");

                info.m_segments = new[] { segments0 };
                info.m_nodes = new[] { nodes0, nodes1 };
            }
            else if (version == NetInfoVersion.Elevated)
            {
                info.m_surfaceLevel = 0;
                info.m_class = highwayInfo.m_class.Clone("NExtHighway");

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
                    (@"NewNetwork\Highway6L\Meshes\Elevated.obj");

                nodes0.SetMeshes
                    (@"NewNetwork\Highway6L\Meshes\Elevated.obj");

                nodes1.SetMeshes
                    (@"NewNetwork\Highway6L\Meshes\Elevated_Trans.obj");

                info.m_segments = new[] { segments0 };
                info.m_nodes = new[] { nodes0, nodes1 };
            }
            else if (version == NetInfoVersion.Slope)
            {
                info.m_surfaceLevel = 0;
                info.m_class = highwayInfo.m_class.Clone("NExtHighway");

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

                nodes0.m_flagsForbidden = NetNode.Flags.Transition;
                nodes0.m_flagsRequired = NetNode.Flags.Underground;

                nodes1.m_flagsForbidden = NetNode.Flags.UndergroundTransition;
                nodes1.m_flagsRequired = NetNode.Flags.None;

                nodes2.m_flagsForbidden = NetNode.Flags.None;
                nodes2.m_flagsRequired = NetNode.Flags.UndergroundTransition;

                nodes3.m_flagsForbidden = NetNode.Flags.Underground;
                nodes3.m_flagsRequired = NetNode.Flags.Transition;

                segments0.SetMeshes
                    (@"NewNetwork\Highway6L\Meshes\Slope.obj",
                     @"NewNetwork\Highway6L\Meshes\Ground_LOD.obj");
                segments2.SetMeshes
                    (@"NewNetwork\Highway6L\Meshes\Slope.obj",
                     @"NewNetwork\Highway6L\Meshes\Ground_LOD.obj");

                nodes0.SetMeshes
                    (@"NewNetwork\Highway6L\Meshes\Slope_U_Node.obj",
                     @"NewNetwork\Highway6L\Meshes\Ground_LOD.obj");
                nodes1.SetMeshes
                    (@"NewNetwork\Highway6L\Meshes\Ground.obj");
                nodes2.SetMeshes
                    (@"NewNetwork\Highway6L\Meshes\Slope_U_Node.obj");
                nodes3.SetMeshes
                    (@"NewNetwork\Highway6L\Meshes\Ground_Trans.obj",
                     @"NewNetwork\Highway6L\Meshes\Ground_Trans_LOD.obj");

                nodes0.m_material = defaultMaterial;
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
                var nodes2 = nodes1.ShallowClone();

                segments1.m_backwardForbidden = NetSegment.Flags.None;
                segments1.m_backwardRequired = NetSegment.Flags.None;

                segments1.m_forwardForbidden = NetSegment.Flags.None;
                segments1.m_forwardRequired = NetSegment.Flags.None;

                nodes1.m_flagsForbidden = NetNode.Flags.Transition;
                nodes1.m_flagsRequired = NetNode.Flags.Underground;

                nodes2.m_flagsForbidden = NetNode.Flags.None;
                nodes2.m_flagsRequired = NetNode.Flags.UndergroundTransition;

                segments0.SetMeshes
                    (@"NewNetwork\Highway6L\Meshes\Tunnel.obj",
                     @"NewNetwork\Highway6L\Meshes\Ground_LOD.obj");
                segments1.SetMeshes
                    (@"NewNetwork\Highway6L\Meshes\Tunnel.obj",
                     @"NewNetwork\Highway6L\Meshes\Ground_LOD.obj");
                nodes0.SetMeshes
                     (@"NewNetwork\Highway6L\Meshes\Tunnel_Node.obj",
                     @"NewNetwork\Highway6L\Meshes\Ground_LOD.obj");
                nodes1.SetMeshes
                    (@"NewNetwork\Highway6L\Meshes\Tunnel_Node.obj",
                     @"NewNetwork\Highway6L\Meshes\Ground_LOD.obj");
                nodes2.SetMeshes
                    (@"NewNetwork\Highway6L\Meshes\Tunnel_Node.obj",
                     @"NewNetwork\Highway6L\Meshes\Ground_LOD.obj");

                segments1.m_material = defaultMaterial;
                nodes1.m_material = defaultMaterial;
                nodes2.m_material = defaultMaterial;

                segments1.m_surfaceMapping = new UnityEngine.Vector4(0, 0, 0, 0);
                nodes1.m_surfaceMapping = new UnityEngine.Vector4(0, 0, 0, 0);
                nodes2.m_surfaceMapping = new UnityEngine.Vector4(0, 0, 0, 0);

                info.m_segments = new[] { segments0, segments1 };
                info.m_nodes = new[] { nodes0, nodes1, nodes2 };
            }

            ///////////////////////////
            // Texturing             //
            ///////////////////////////
            switch (version)
            {
                case NetInfoVersion.Ground:
                    info.SetAllSegmentsTexture(
                        new TexturesSet(
                            @"NewNetwork\Highway6L\Textures\Ground_Elevated_Segment__MainTex.png",
                            @"NewNetwork\Highway6L\Textures\Ground_Elevated_Segment__APRMap.png"),
                        new TexturesSet
                           (@"NewNetwork\Highway6L\Textures\Ground_SegmentLOD__MainTex.png",
                            @"NewNetwork\Highway6L\Textures\Ground_SegmentLOD__APRMap.png",
                            @"NewNetwork\Highway6L\Textures\Ground_SegmentLOD__XYSMap.png"));
                    info.SetAllNodesTexture(
                        new TexturesSet
                           (@"NewNetwork\Highway6L\Textures\Ground_Elevated_Node__MainTex.png",
                            @"NewNetwork\Highway6L\Textures\Ground_Elevated_Node__APRMap.png"),
                        new TexturesSet
                           (@"NewNetwork\Highway6L\Textures\Ground_NodeLOD__MainTex.png",
                            @"NewNetwork\Highway6L\Textures\Ground_NodeLOD__APRMap.png",
                            @"NewNetwork\Highway6L\Textures\Ground_NodeLOD__XYSMap.png"));
                    break;

                case NetInfoVersion.Elevated:
                case NetInfoVersion.Bridge:
                    info.SetAllSegmentsTexture(
                        new TexturesSet(
                            @"NewNetwork\Highway6L\Textures\Ground_Elevated_Segment__MainTex.png",
                            @"NewNetwork\Highway6L\Textures\Ground_Elevated_Segment__APRMap.png"));
                    info.SetAllNodesTexture(
                        new TexturesSet
                           (@"NewNetwork\Highway6L\Textures\Ground_Elevated_Node__MainTex.png",
                            @"NewNetwork\Highway6L\Textures\Ground_Elevated_Node__APRMap.png"));
                    // Lets leave the crossings there until we have a fix
                    //new TexturesSet
                    //   (@"NewNetwork\Highway6L\Textures\Elevated_NodeLOD__MainTex.png",
                    //    @"NewNetwork\Highway6L\Textures\Elevated_NodeLOD__APRMap.png",
                    //    @"NewNetwork\Highway6L\Textures\Elevated_NodeLOD__XYSMap.png"));
                    break;

                case NetInfoVersion.Slope:
                    info.SetAllSegmentsTexture(
                        new TexturesSet
                           (@"NewNetwork\Highway6L\Textures\Slope_Tunnel_Segment__MainTex.png",
                            @"NewNetwork\Highway6L\Textures\Slope_Tunnel_Segment_Open__APRMap.png"));
                    info.SetAllNodesTexture(
                        new TexturesSet
                           (@"NewNetwork\Highway6L\Textures\Slope_Tunnel_Node__MainTex.png",
                            @"NewNetwork\Highway6L\Textures\Slope_Tunnel_Node__APRMap.png"));
                    break;
                case NetInfoVersion.Tunnel:
                    info.SetAllSegmentsTexture(
                        new TexturesSet
                           (@"NewNetwork\Highway6L\Textures\Tunnel_Segment__MainTex.png",
                            @"NewNetwork\Highway6L\Textures\Tunnel_Segment__APRMap.png"));
                    info.SetAllNodesTexture(
                        new TexturesSet
                           (@"NewNetwork\Highway6L\Textures\Tunnel_Node__MainTex.png",
                            @"NewNetwork\Highway6L\Textures\Tunnel_Segment__APRMap.png"));
                    break;
            }


            ///////////////////////////
            // Set up                //
            ///////////////////////////
            info.m_setVehicleFlags = Vehicle.Flags.None;
            info.m_availableIn = ItemClass.Availability.All;
            info.m_surfaceLevel = (version == NetInfoVersion.Slope) ? 0.1f : 0;
            info.m_createPavement = false; //(version == NetInfoVersion.Slope);
            info.m_createGravel = !(version == NetInfoVersion.Tunnel);
            info.m_averageVehicleLaneSpeed = 2f;
            info.m_hasParkingSpaces = false;
            info.m_hasPedestrianLanes = false;
            info.m_halfWidth = 16f;
            info.m_UnlockMilestone = highwayInfo.m_UnlockMilestone;
            info.m_pavementWidth = 2f;
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
            var vehiculeLanes = info.m_lanes
                .Where(l => l.m_laneType != NetInfo.LaneType.None)
                .OrderBy(l => l.m_similarLaneIndex)
                .ToArray();
            var nbLanes = vehiculeLanes.Count();

            const float laneWidth = 4f;
            var positionStart = (laneWidth * ((1f - nbLanes) / 2f));

            for (int i = 0; i < vehiculeLanes.Length; i++)
            {
                var l = vehiculeLanes[i];
                l.m_allowStop = false;
                l.m_speedLimit = 2f;

                if (version == NetInfoVersion.Ground)
                {
                    l.m_verticalOffset = 0f;
                }

                l.m_width = laneWidth;
                l.m_position = positionStart + i * laneWidth;
            }

            if (version == NetInfoVersion.Ground)
            {
                var hwPlayerNetAI = highwayInfo.GetComponent<PlayerNetAI>();
                var playerNetAI = info.GetComponent<PlayerNetAI>();

                if (hwPlayerNetAI != null && playerNetAI != null)
                {
                    playerNetAI.m_constructionCost = hwPlayerNetAI.m_constructionCost * 2;
                    playerNetAI.m_maintenanceCost = hwPlayerNetAI.m_maintenanceCost * 2;
                }
            }
            else // Same as the original oneway
            {

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

            var leftHwLane = info
                .m_lanes
                .Where(l => l != null && l.m_laneProps != null && l.m_laneProps.name != null && l.m_laneProps.m_props != null)
                .FirstOrDefault(l => l.m_laneProps.name.ToLower().Contains("left"));

            var rightHwLane = info
                .m_lanes
                .Where(l => l != null && l.m_laneProps != null && l.m_laneProps.name != null && l.m_laneProps.m_props != null)
                .FirstOrDefault(l => l.m_laneProps.name.ToLower().Contains("right"));

            var leftHwProps = new List<NetLaneProps.Prop>();
            var rightHwProps = new List<NetLaneProps.Prop>();

            if (leftHwLane != null && rightHwLane != null)
            {
                leftHwProps = leftHwLane.m_laneProps.m_props.ToList();

                NetLaneProps.Prop streetLightLeft = null;
                NetLaneProps.Prop streetLightRight = null;

                foreach (var prop in rightHwLane.m_laneProps.m_props)
                {
                    if (prop != null && prop.m_prop != null && prop.m_prop.name != null && prop.m_prop.name.Contains("New Street Light"))
                    {
                        streetLightLeft = prop.ShallowClone();
                        streetLightRight = prop.ShallowClone();
                        break;
                    }
                }

                if (streetLightLeft != null && version != NetInfoVersion.Tunnel)
                {
                    streetLightRight.m_repeatDistance = 80;
                    streetLightLeft.m_repeatDistance = 80;

                    streetLightLeft.m_angle = 180;
                    streetLightLeft.m_segmentOffset = 40;

                    if (version == NetInfoVersion.Bridge || version == NetInfoVersion.Elevated)
                    {
                        streetLightLeft.m_position = new UnityEngine.Vector3(-3.6f, 1, 0);
                        streetLightRight.m_position = new UnityEngine.Vector3(3.6f, 1, 0);
                    }
                    else
                    {
                        streetLightLeft.m_position = new UnityEngine.Vector3(-1, 0, 0);
                        streetLightRight.m_position = new UnityEngine.Vector3(1, 0, 0); ;
                    }

                    leftHwProps.Add(streetLightLeft);


                    rightHwProps.Add(streetLightRight);

                }
                //if (version == NetInfoVersion.Slope || version == NetInfoVersion.Tunnel)
                //{
                //    var wallLightPropInfo = ToolsCSL.FindPrefab<PropInfo>("Flood Light Down White");
                //    var wallLightProp = new NetLaneProps.Prop();
                //    var floodLightEffectInfo = ToolsCSL.FindEffect<EffectInfo>("Flood Light White");

                //    wallLightProp.m_prop = wallLightPropInfo;
                //    wallLightProp.m_finalProp = wallLightPropInfo;

                //    wallLightProp.m_probability = 100;
                //    wallLightProp.m_repeatDistance = 40;
                //    wallLightProp.m_segmentOffset = 20;
                //    var wallLightPropLeft = wallLightProp.ShallowClone();
                //    var wallLightPropRight = wallLightProp.ShallowClone();

                //    wallLightPropLeft.m_position = new UnityEngine.Vector3(-1, 6.9f, 0);
                //    wallLightPropRight.m_position = new UnityEngine.Vector3(1, 6.9f, 0);
                //    leftHwProps.Add(wallLightPropLeft);
                //    rightHwProps.Add(wallLightPropRight);
                //}
                //leftHwLane.m_laneProps.m_props = leftHwProps.ToArray();
                //rightHwLane.m_laneProps.m_props = rightHwProps.ToArray();
            }
        }

        public void ModifyExistingNetInfo()
        {
            var highwayRampInfo = Prefabs.Find<NetInfo>("HighwayRamp");
            highwayRampInfo.m_UIPriority = highwayRampInfo.m_UIPriority + 1;
        }
    }
}
