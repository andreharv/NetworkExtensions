using System.Linq;
using Transit.Addon.RoadExtensions.Menus;
using Transit.Framework;
using Transit.Framework.Modularity;

namespace Transit.Addon.RoadExtensions.Roads.Busway6L
{
    public class Busway6LBuilder : Busway6LBuilderBase, INetInfoBuilderPart
    {
        public int Order { get { return 170; } }
        public int UIOrder { get { return 26; } }

        public string BasedPrefabName { get { return NetInfos.Vanilla.ROAD_6L; } }
        public string Name { get { return "Large Road With Bus Lanes"; } }
        public string DisplayName { get { return "Six-Lane Road with Bus Lanes"; } }
        public string Description { get { return "A six-lane, two-way road suitable for cars with dedicated bus lanes."; } }

        public string ThumbnailsPath { get { return @"Roads\Busway6L\thumbnails.png"; } }
        public string InfoTooltipPath { get { return @"Roads\Busway6L\infotooltip.png"; } }

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
                                           (@"Roads\Busway6L\Textures\Ground_Segment__MainTex.png",
                                            @"Roads\Busway6L\Textures\Ground_Segment__AlphaMap.png"),
                                        new TexturesSet
                                           (@"Roads\Busway6L\Textures\Ground_SegmentLOD_Bus__MainTex.png",
                                            @"Roads\Busway6L\Textures\Ground_SegmentLOD_Bus__AlphaMap.png",
                                            @"Roads\Busway6L\Textures\Ground_SegmentLOD_Bus__XYSMap.png"));
                                    break;

                                case NetSegment.Flags.StopBoth:
                                    segment.SetTextures(
                                        new TexturesSet
                                           (@"Roads\Busway6L\Textures\Ground_Segment__MainTex.png",
                                            @"Roads\Busway6L\Textures\Ground_Segment__AlphaMap.png"),
                                        new TexturesSet
                                           (@"Roads\Busway6L\Textures\Ground_SegmentLOD_BusBoth__MainTex.png",
                                            @"Roads\Busway6L\Textures\Ground_SegmentLOD_BusBoth__AlphaMap.png",
                                            @"Roads\Busway6L\Textures\Ground_SegmentLOD_BusBoth__XYSMap.png"));
                                    break;

                                default:
                                    segment.SetTextures(
                                        new TexturesSet
                                           (@"Roads\Busway6L\Textures\Ground_Segment__MainTex.png",
                                            @"Roads\Busway6L\Textures\Ground_Segment__AlphaMap.png"),
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
