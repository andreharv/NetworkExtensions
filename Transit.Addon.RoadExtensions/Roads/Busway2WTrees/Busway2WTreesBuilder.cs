using Transit.Framework;
using Transit.Framework.Modularity;

namespace Transit.Addon.RoadExtensions.Roads.Busway2WTrees
{
    public class Busway2WTreesBuilder : NetInfoBuilderBase, INetInfoBuilder
    {
        public int Order { get { return 130; } }
        public int Priority { get { return 21; } }

        public string TemplatePrefabName { get { return NetInfos.Vanilla.ROAD_2L_TREES; } }
        public string Name { get { return "Small Busway Trees"; } } // TODO: Make sur the name fit with the existing T++ name
        public string DisplayName { get { return "Busway with Trees"; } }
        public string CodeName { get { return "BUSWAY_2W_TREES"; } }
        public string Description { get { return "A two-lane, two-way road suitable for buses only. Busway does not allow zoning next to it!"; } }
        public string UICategory { get { return "RoadsSmall"; } }

        public string ThumbnailsPath { get { return @"Roads\Busway2WTrees\thumbnails.png"; } }
        public string InfoTooltipPath { get { return @"Roads\Busway2WTrees\infotooltip.png"; } }

        public NetInfoVersion SupportedVersions
        {
            get { return NetInfoVersion.All; }
        }

        public void BuildUp(NetInfo info, NetInfoVersion version)
        {
            ///////////////////////////
            // Templates             //
            ///////////////////////////
            var highwayInfo = Prefabs.Find<NetInfo>(NetInfos.Vanilla.HIGHWAY_3L);

            ///////////////////////////
            // Texturing             //
            ///////////////////////////
            switch (version)
            {
                case NetInfoVersion.Ground:
                    {
                        foreach (var segment in info.m_segments)
                        {
                            switch (segment.m_forwardRequired)
                            {
                                case NetSegment.Flags.StopLeft:
                                case NetSegment.Flags.StopRight:
                                    segment.SetTextures(
                                        new TexturesSet
                                           (@"Roads\Busway2WGrass\Textures\Ground_Segment__MainTex.png",
                                            @"Roads\Busway2WGrass\Textures\Ground_Segment_Bus__AlphaMap.png"));
                                    break;

                                case NetSegment.Flags.StopBoth:
                                    segment.SetTextures(
                                        new TexturesSet
                                           (@"Roads\Busway2WGrass\Textures\Ground_Segment__MainTex.png",
                                            @"Roads\Busway2WGrass\Textures\Ground_Segment_BusBoth__AlphaMap.png"));
                                    break;

                                default:
                                    segment.SetTextures(
                                        new TexturesSet
                                           (@"Roads\Busway2WGrass\Textures\Ground_Segment__MainTex.png",
                                            @"Roads\Busway2WGrass\Textures\Ground_Segment__AlphaMap.png"));
                                    break;
                            }
                        }
                    }
                    break;

                case NetInfoVersion.Bridge:
                case NetInfoVersion.Elevated:
                    {
                        foreach (var segment in info.m_segments)
                        {
                            segment.SetTextures(
                                new TexturesSet
                                    (@"Roads\Busway2W\Textures\Elevated_Segment__MainTex.png",
                                     @"Roads\Busway2W\Textures\Elevated_Segment__AlphaMap.png"));
                        }
                    }
                    break;
                case NetInfoVersion.Slope:
                    {
                        foreach (var segment in info.m_segments)
                        {
                            segment.SetTextures(
                                new TexturesSet
                                    (@"Roads\Busway2W\Textures\Slope_Segment__MainTex.png",
                                     @"Roads\Busway2W\Textures\Slope_Segment__AlphaMap.png"));
                        }
                    }
                    break;
                case NetInfoVersion.Tunnel:
                    break;
            }

            ///////////////////////////
            // Set up                //
            ///////////////////////////
            info.m_UnlockMilestone = highwayInfo.m_UnlockMilestone;

            foreach (var lane in info.m_lanes)
            {
                if (lane.m_laneType == NetInfo.LaneType.Vehicle)
                {
                    lane.m_speedLimit = 1.6f;
                    lane.m_laneType = NetInfo.LaneType.TransportVehicle;
                }
            }

            var roadBaseAI = info.GetComponent<RoadBaseAI>();

            if (roadBaseAI != null)
            {
            }

            var roadAI = info.GetComponent<RoadAI>();

            if (roadAI != null)
            {
                roadAI.m_enableZoning = false;
            }
        }
    }
}
