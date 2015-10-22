using System;
using System.Linq;
using Transit.Framework;
using Transit.Framework.Modularity;

namespace Transit.Addon.RoadExtensions.Roads.Highway6L
{
    public class Highway6LBuilder : Activable, INetInfoBuilderPart, INetInfoModifier
    {
        public int Order { get { return 50; } }
        public int UIOrder { get { return 14; } }

        public string BasedPrefabName  { get { return NetInfos.Vanilla.ONEWAY_6L; } }
        public string Name        { get { return "Large Highway"; } }
        public string DisplayName { get { return "Six-Lane Highway"; } }
        public string Description { get { return "A six-lane, one-way road suitable for very high and dense traffic between metropolitan areas. Lanes going the opposite direction need to be built separately. Highway does not allow zoning next to it!"; } }
        public string UICategory  { get { return "RoadsHighway"; } }

        public string ThumbnailsPath  { get { return @"Roads\Highway6L\thumbnails.png"; } }
        public string InfoTooltipPath { get { return @"Roads\Highway6L\infotooltip.png"; } }

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
                    (@"Roads\Highway6L\Meshes\Ground.obj",
                     @"Roads\Highway6L\Meshes\Ground_LOD.obj");

                nodes0.SetMeshes
                    (@"Roads\Highway6L\Meshes\Ground.obj",
                     @"Roads\Highway6L\Meshes\Ground_LOD.obj");

                nodes1.SetMeshes
                    (@"Roads\Highway6L\Meshes\Ground_Trans.obj",
                     @"Roads\Highway6L\Meshes\Ground_Trans_LOD.obj");

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
                        new TexturesSet(
                            @"Roads\Highway6L\Textures\Ground_Segment__MainTex.png",
                            @"Roads\Highway6L\Textures\Ground_Segment__APRMap.png"),
                        new TexturesSet
                           (@"Roads\Highway6L\Textures\Ground_SegmentLOD__MainTex.png",
                            @"Roads\Highway6L\Textures\Ground_SegmentLOD__APRMap.png",
                            @"Roads\Highway6L\Textures\Ground_SegmentLOD__XYSMap.png"));
                    info.SetAllNodesTexture(
                        new TexturesSet
                           (@"Roads\Highway6L\Textures\Ground_Node__MainTex.png",
                            @"Roads\Highway6L\Textures\Ground_Node__APRMap.png"),
                        new TexturesSet
                           (@"Roads\Highway6L\Textures\Ground_NodeLOD__MainTex.png",
                            @"Roads\Highway6L\Textures\Ground_NodeLOD__APRMap.png",
                            @"Roads\Highway6L\Textures\Ground_NodeLOD__XYSMap.png"));
                    break;

                case NetInfoVersion.Elevated:
                case NetInfoVersion.Bridge:
                    info.SetAllNodesTexture(
                        new TexturesSet
                           (@"Roads\Highway6L\Textures\Elevated_Node__MainTex.png",
                            @"Roads\Highway6L\Textures\Elevated_Node__APRMap.png"));
                        // Lets leave the crossings there until we have a fix
                        //new TexturesSet
                        //   (@"Roads\Highway6L\Textures\Elevated_NodeLOD__MainTex.png",
                        //    @"Roads\Highway6L\Textures\Elevated_NodeLOD__APRMap.png",
                        //    @"Roads\Highway6L\Textures\Elevated_NodeLOD__XYSMap.png"));
                    break;

                case NetInfoVersion.Slope:
                    info.SetAllNodesTexture(
                        new TexturesSet
                           (@"Roads\Highway6L\Textures\Slope_Node__MainTex.png",
                            @"Roads\Highway6L\Textures\Slope_Node__APRMap.png"),
                        new TexturesSet
                           (@"Roads\Highway6L\Textures\Slope_NodeLOD__MainTex.png",
                            @"Roads\Highway6L\Textures\Slope_NodeLOD__APRMap.png",
                            @"Roads\Highway6L\Textures\Slope_NodeLOD__XYSMap.png"));
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
            var nbLanes = vehiculeLanes.Count(); // Supposed to be 6

            const float laneWidth = 2f; // TODO: Make it 2.5 with new texture
            const float laneWidthPad = 1f;
            const float laneWidthTotal = laneWidth + laneWidthPad;
            var positionStart = (laneWidthTotal * ((1f - nbLanes) / 2f));

            for (int i = 0; i < vehiculeLanes.Length; i++)
            {
                var l = vehiculeLanes[i];
                l.m_allowStop = false;
                l.m_speedLimit = 2f;

                if (version == NetInfoVersion.Ground)
                {
                    l.m_verticalOffset = 0f;
                }

                l.m_width = laneWidthTotal;
                l.m_position = positionStart + i * laneWidthTotal;
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
        }

        public void ModifyExistingNetInfo()
        {
            var highwayRampInfo = Prefabs.Find<NetInfo>("HighwayRamp");
            highwayRampInfo.m_UIPriority = highwayRampInfo.m_UIPriority + 1;
        }
    }
}
