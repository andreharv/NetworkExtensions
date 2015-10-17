using Transit.Framework;
using Transit.Framework.Modularity;

namespace Transit.Addon.RoadExtensions.Roads.Busway6L
{
    public class Busway6LGrassBuilder : Busway6LBuilderBase, INetInfoBuilder
    {
        public int Order { get { return 180; } }
        public int Priority { get { return 27; } }

        public string TemplatePrefabName { get { return NetInfos.Vanilla.ROAD_6L_GRASS; } }
        public string Name { get { return "Large Road Decoration Grass With Bus Lanes"; } }
        public string DisplayName { get { return "Six-Lane Road with Grass and Bus Lanes"; } }
        public string CodeName { get { return "BUSWAY_6L_GRASS"; } }
        public string Description { get { return "A six-lane, two-way road with decorative grass suitable for cars with dedicated bus lanes."; } }

        public string ThumbnailsPath { get { return @"Roads\Busway6L\thumbnails_grass.png"; } }
        public string InfoTooltipPath { get { return @"Roads\Busway6L\infotooltip_grass.png"; } }

        public override void BuildUp(NetInfo info, NetInfoVersion version)
        {
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
                                           (@"Roads\Busway6L\Textures_Grass\Ground_Segment__MainTex.png",
                                            @"Roads\Busway6L\Textures_Grass\Ground_Segment__AlphaMap.png"),
                                        new TexturesSet
                                           (@"Roads\Busway6L\Textures\Ground_SegmentLOD_Bus__MainTex.png",
                                            @"Roads\Busway6L\Textures\Ground_SegmentLOD_Bus__AlphaMap.png",
                                            @"Roads\Busway6L\Textures\Ground_SegmentLOD_Bus__XYSMap.png"));
                                    break;

                                case NetSegment.Flags.StopBoth:
                                    segment.SetTextures(
                                        new TexturesSet
                                           (@"Roads\Busway6L\Textures_Grass\Ground_Segment_Bus__MainTex.png",
                                            @"Roads\Busway6L\Textures_Grass\Ground_Segment_Bus__AlphaMap.png"),
                                        new TexturesSet
                                           (@"Roads\Busway6L\Textures\Ground_SegmentLOD_BusBoth__MainTex.png",
                                            @"Roads\Busway6L\Textures\Ground_SegmentLOD_BusBoth__AlphaMap.png",
                                            @"Roads\Busway6L\Textures\Ground_SegmentLOD_BusBoth__XYSMap.png"));
                                    break;

                                default:
                                    segment.SetTextures(
                                        new TexturesSet
                                           (@"Roads\Busway6L\Textures_Grass\Ground_Segment_BusBoth__MainTex.png",
                                            @"Roads\Busway6L\Textures_Grass\Ground_Segment_BusBoth__AlphaMap.png"),
                                        new TexturesSet
                                           (@"Roads\Busway6L\Textures\Ground_SegmentLOD__MainTex.png",
                                            @"Roads\Busway6L\Textures\Ground_SegmentLOD__AlphaMap.png",
                                            @"Roads\Busway6L\Textures\Ground_SegmentLOD__XYSMap.png"));
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
                                    (@"Roads\Busway6L\Textures\Elevated_Segment__MainTex.png",
                                     @"Roads\Busway6L\Textures\Elevated_Segment__AlphaMap.png"),
                                new TexturesSet
                                    (@"Roads\Busway6L\Textures\Elevated_SegmentLOD__MainTex.png",
                                     @"Roads\Busway6L\Textures\Elevated_SegmentLOD__AlphaMap.png",
                                     @"Roads\Busway6L\Textures\Elevated_SegmentLOD__XYSMap.png"));
                        }
                    }
                    break;
                case NetInfoVersion.Slope:
                    {
                        foreach (var segment in info.m_segments)
                        {
                            segment.SetTextures(
                                new TexturesSet
                                    (@"Roads\Busway6L\Textures\Slope_Segment__MainTex.png",
                                     @"Roads\Busway6L\Textures\Slope_Segment__AlphaMap.png"),
                                new TexturesSet
                                    (@"Roads\Busway6L\Textures\Slope_SegmentLOD__MainTex.png",
                                     @"Roads\Busway6L\Textures\Slope_SegmentLOD__AlphaMap.png",
                                     @"Roads\Busway6L\Textures\Slope_SegmentLOD__XYS.png"));
                        }
                    }
                    break;
                case NetInfoVersion.Tunnel:
                    break;
            }

            base.BuildUp(info, version);
        }
    }
}
