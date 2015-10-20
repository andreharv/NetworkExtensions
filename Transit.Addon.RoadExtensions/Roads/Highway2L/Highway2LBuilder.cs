using System;
using System.Linq;
using Transit.Framework;
using Transit.Framework.Modularity;

namespace Transit.Addon.RoadExtensions.Roads.Highway2L
{
    public class Highway2LBuilder : Activable, INetInfoBuilder
    {
        public int Order { get { return 40; } }
        public int UIOrder { get { return 10; } }

        public string TemplateName { get { return NetInfos.Vanilla.ONEWAY_2L; } }
        public string Name { get { return "Rural Highway"; } }
        public string DisplayName { get { return "Two-Lane Highway"; } }
        public string Description { get { return "A two-lane, one-way road suitable for low to medium traffic between areas. Lanes going the opposite direction need to be built separately. Highway does not allow zoning next to it!"; } }
        public string UICategory { get { return "RoadsHighway"; } }

        public string ThumbnailsPath { get { return @"Roads\Highway2L\thumbnails.png"; } }
        public string InfoTooltipPath { get { return @"Roads\Highway2L\infotooltip.png"; } }

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


            ///////////////////////////
            // 3DModeling            //
            ///////////////////////////
            if (version == NetInfoVersion.Ground)
            {
                info.m_surfaceLevel = 0;
                info.m_class = highwayInfo.m_class.Clone(NetInfoClasses.NEXT_HIGHWAY);

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
                    (@"Roads\Highway2L\Meshes\Ground.obj",
                     @"Roads\Highway2L\Meshes\Ground_LOD.obj");

                nodes0.SetMeshes
                    (@"Roads\Highway2L\Meshes\Ground.obj",
                     @"Roads\Highway2L\Meshes\Ground_Node_LOD.obj");

                nodes1.SetMeshes
                    (@"Roads\Highway2L\Meshes\Ground_Trans.obj",
                     @"Roads\Highway2L\Meshes\Ground_Trans_LOD.obj");

                info.m_segments = new[] { segments0 };
                info.m_nodes = new[] { nodes0, nodes1 };
            }


            ///////////////////////////
            // Texturing             //
            ///////////////////////////
            switch (version)
            {
                case NetInfoVersion.Ground:
                    info.SetAllSegmentsTexture(
                        new TexturesSet
                           (@"Roads\Highway2L\Textures\Ground_Segment__MainTex.png",
                            @"Roads\Highway2L\Textures\Ground_Segment__AlphaMap.png"),
                        new TexturesSet
                           (@"Roads\Highway2L\Textures\Ground_SegmentLOD__MainTex.png",
                            @"Roads\Highway2L\Textures\Ground_SegmentLOD__AlphaMap.png",
                            @"Roads\Highway2L\Textures\Ground_SegmentLOD__XYSMap.png"));
                    info.SetAllNodesTexture(
                        new TexturesSet
                           (@"Roads\Highway2L\Textures\Ground_Node__MainTex.png",
                            @"Roads\Highway2L\Textures\Ground_Node__AlphaMap.png"),
                        new TexturesSet
                           (@"Roads\Highway2L\Textures\Ground_NodeLOD__MainTex.png",
                            @"Roads\Highway2L\Textures\Ground_NodeLOD__AlphaMap.png",
                            @"Roads\Highway2L\Textures\Ground_NodeLOD__XYSMap.png"));
                    break;

                case NetInfoVersion.Elevated:
                case NetInfoVersion.Bridge:
                    info.SetAllNodesTexture(
                        new TexturesSet
                           (@"Roads\Highway2L\Textures\Elevated_Node__MainTex.png",
                            @"Roads\Highway2L\Textures\Elevated_Node__AlphaMap.png"));
                    // Lets leave the crossings there until we have a fix
                    //new TexturesSet
                    //   (@"Roads\Highway2L\Textures\Elevated_NodeLOD__MainTex.png",
                    //    @"Roads\Highway2L\Textures\Elevated_NodeLOD__AlphaMap.png",
                    //    @"Roads\Highway2L\Textures\Elevated_NodeLOD__XYSMap.png"));
                    break;

                case NetInfoVersion.Slope:
                    info.SetAllNodesTexture(
                        new TexturesSet
                           (@"Roads\Highway2L\Textures\Slope_Node__MainTex.png",
                            @"Roads\Highway2L\Textures\Slope_Node__AlphaMap.png"),
                        new TexturesSet
                           (@"Roads\Highway2L\Textures\Slope_NodeLOD__MainTex.png",
                            @"Roads\Highway2L\Textures\Slope_NodeLOD__AlphaMap.png",
                            @"Roads\Highway2L\Textures\Slope_NodeLOD__XYSMap.png"));
                    break;

                case NetInfoVersion.Tunnel:
                    break;
            }


            ///////////////////////////
            // Set up                //
            ///////////////////////////
            info.m_availableIn = ItemClass.Availability.All;
            info.m_createPavement = (version == NetInfoVersion.Slope);
            info.m_createGravel = (version == NetInfoVersion.Ground);
            info.m_averageVehicleLaneSpeed = 2f;
            info.m_hasParkingSpaces = false;
            info.m_hasPedestrianLanes = false;

            info.m_UnlockMilestone = highwayInfo.m_UnlockMilestone;


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

            for (int i = 0; i < vehiculeLanes.Length; i++)
            {
                var l = vehiculeLanes[i];
                l.m_allowStop = false;
                l.m_speedLimit = 2f;

                if (version == NetInfoVersion.Ground)
                {
                    l.m_verticalOffset = 0f;
                }
            }


            if (version == NetInfoVersion.Ground)
            {
                var hwPlayerNetAI = highwayInfo.GetComponent<PlayerNetAI>();
                var playerNetAI = info.GetComponent<PlayerNetAI>();

                if (hwPlayerNetAI != null && playerNetAI != null)
                {
                    playerNetAI.m_constructionCost = hwPlayerNetAI.m_constructionCost * 2 / 3;
                    playerNetAI.m_maintenanceCost = hwPlayerNetAI.m_maintenanceCost * 2 / 3;
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
        }
    }
}
