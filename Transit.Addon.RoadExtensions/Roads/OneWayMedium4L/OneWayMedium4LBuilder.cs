using System;
using System.Linq;
using Transit.Framework;
using Transit.Framework.Modularity;

namespace Transit.Addon.RoadExtensions.Roads.OneWayMedium4L
{
    public class OneWayMedium4LBuilder : NetInfoBuilderBase, INetInfoBuilder
    {
        public int Order { get { return 10; } }
        public int Priority { get { return 15; } }

        public string TemplatePrefabName { get { return NetInfos.Vanilla.ONEWAY_6L; } }
        public string Name { get { return "Medium Oneway"; } }
        public string DisplayName { get { return "Four-Lane Oneway"; } }
        public string CodeName { get { return "ONEWAY_4L"; } }
        public string Description { get { return "A four-lane, one-way road suitable for medium traffic moving in one direction. This road is zonable."; } }
        public string UICategory { get { return "RoadsMedium"; } }

        public string ThumbnailsPath { get { return @"Roads\OneWayMedium4L\thumbnails.png"; } }
        public string InfoTooltipPath { get { return @"Roads\OneWayMedium4L\infotooltip.png"; } }


        public NetInfoVersion SupportedVersions
        {
            get { return NetInfoVersion.Ground; }
        }

        public void BuildUp(NetInfo info, NetInfoVersion version)
        {
            ///////////////////////////
            // Texturing             //
            ///////////////////////////
            switch (version)
            {
                case NetInfoVersion.Ground:
                    info.SetAllSegmentsTexture(
                        new TexturesSet
                           (@"Roads\OneWayMedium4L\Textures\Ground_Segment__MainTex.png",
                            @"Roads\OneWayMedium4L\Textures\Ground_Segment__AlphaMap.png"));
                    info.SetAllNodesTexture(
                        new TexturesSet
                            (@"Roads\OneWayMedium4L\Textures\Ground_Node__MainTex.png",
                             @"Roads\OneWayMedium4L\Textures\Ground_Node__AlphaMap.png"));
                    break;
            }

            ///////////////////////////
            // 3DModeling            //
            ///////////////////////////
            if (version == NetInfoVersion.Ground)
            {

                var segments0 = info.m_segments[0];
                var nodes0 = info.m_nodes[0];

                segments0.SetMeshes
                    (@"Roads\OneWayMedium4L\Meshes\Ground.obj",
                     @"Roads\OneWayMedium4L\Meshes\Ground_LOD.obj");

                nodes0.SetMeshes
                    (@"Roads\OneWayMedium4L\Meshes\Ground.obj",
                     @"Roads\OneWayMedium4L\Meshes\Ground_Node_LOD.obj");

                info.m_segments = new[] { segments0 };
                info.m_nodes = new[] { nodes0};
            }

            ///////////////////////////
            // Set up                //
            ///////////////////////////
            var vehicleLaneWidth = 3f;
            var pedWidth = 4f;
            var roadHalfWidth = 8f;
            var parkingLaneWidth = 2f;
            var vehicleLanesToTake = 4;
            info.m_halfWidth = 12.0f;
            info.m_pavementWidth = pedWidth;
            // Disabling Parkings and Peds
            //foreach (var l in info.m_lanes)
            //{
            //    if (l.m_laneType == NetInfo.LaneType.Parking)
            //    {
            //        l.m_laneType = NetInfo.LaneType.None;
            //    }
            //}

            // Setting up lanes

            var lanes = info.m_lanes;
            
            var parkingLanes = lanes
                .Where(l => l.m_laneType == NetInfo.LaneType.Parking)
                .ToList();

            var vehicleLanes = lanes
                .Where(l => l.m_laneType != NetInfo.LaneType.None)
                .Where(l => l.m_laneType != NetInfo.LaneType.Pedestrian)
                .Where(l => l.m_laneType != NetInfo.LaneType.Parking)
                .Take(vehicleLanesToTake).ToList();

            var pedestrianLanes = lanes
                .Where(l => l.m_laneType == NetInfo.LaneType.Pedestrian)
                .OrderBy(l => l.m_similarLaneIndex)
                .ToList();

            for (var i = 0; i < pedestrianLanes.Count; i++)
            {
                var multiplier = pedestrianLanes[i].m_position / Math.Abs(pedestrianLanes[i].m_position);
                pedestrianLanes[i].m_width = pedWidth;
                pedestrianLanes[i].m_position = multiplier * (roadHalfWidth + (.5f * pedWidth));

                foreach (var prop in pedestrianLanes[i].m_laneProps.m_props)
                {
                    prop.m_position.x += multiplier * 1.5f;
                }
            }

            for (var i = 0; i < vehicleLanes.Count; i++)
            {
                vehicleLanes[i].m_similarLaneCount = vehicleLanes.Count();
                vehicleLanes[i].m_similarLaneIndex = i;
                vehicleLanes[i].m_width = vehicleLaneWidth;
                vehicleLanes[i].m_position = (-1 * ((vehicleLanes.Count / 2f) - .5f) + i) * vehicleLaneWidth;
            }

            for (var i = 0; i < parkingLanes.Count; i++)
            {
                var multiplier = parkingLanes[i].m_position / Math.Abs(parkingLanes[i].m_position);
                parkingLanes[i].m_width = parkingLaneWidth;
                parkingLanes[i].m_position = multiplier * (roadHalfWidth - (parkingLaneWidth / 2));
            }

            var onewayRoadInfo = Prefabs.Find<NetInfo>(NetInfos.Vanilla.ONEWAY_2L);

            if (version == NetInfoVersion.Ground)
            {
                var playerNetAI = info.GetComponent<PlayerNetAI>();
                var orPlayerNetAI = onewayRoadInfo.GetComponent<PlayerNetAI>();
                if (playerNetAI != null)
                {
                    playerNetAI.m_constructionCost = orPlayerNetAI.m_constructionCost * 2;
                    playerNetAI.m_maintenanceCost = orPlayerNetAI.m_maintenanceCost * 2;
                }
            }
            else // Same as the original oneway
            {

            }
        }
    }
}
