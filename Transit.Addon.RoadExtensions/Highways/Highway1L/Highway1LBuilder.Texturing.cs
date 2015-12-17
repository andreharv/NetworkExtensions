using Transit.Framework;
using Transit.Framework.Texturing;

namespace Transit.Addon.RoadExtensions.Highways.Highway1L
{
    public partial class Highway1LBuilder
    {
        private static void SetupTextures(NetInfo info, NetInfoVersion version)
        {
            switch (version)
            {
                case NetInfoVersion.Ground:
                    info.SetAllSegmentsTexture(
                        new TexturesSet(
                            @"Highways\Highway1L\Textures\Ground_Segment__MainTex.png",
                            @"Highways\Highway1L\Textures\Ground_Segment__APRMap.png"),
                        new LODTexturesSet
                           (@"Highways\Highway1L\Textures\Ground_SegmentLOD__MainTex.png",
                            @"Highways\Highway1L\Textures\Ground_SegmentLOD__APRMap.png",
                            @"Highways\Highway1L\Textures\Ground_LOD__XYSMap.png"));
                    info.SetAllNodesTexture(
                        new TexturesSet
                           (@"Highways\Highway1L\Textures\Ground_Segment__MainTex.png",
                            @"Highways\Highway1L\Textures\Ground_Node__APRMap.png"),
                        new LODTexturesSet
                           (@"Highways\Highway1L\Textures\Ground_NodeLOD__MainTex.png",
                            @"Highways\Highway1L\Textures\Ground_NodeLOD__APRMap.png",
                            @"Highways\Highway1L\Textures\Ground_LOD__XYSMap.png"));
                    break;

                case NetInfoVersion.Elevated:
                case NetInfoVersion.Bridge:
                    info.SetAllSegmentsTexture(
                        new TexturesSet(
                            @"Highways\Highway1L\Textures\Ground_Segment__MainTex.png",
                            @"Highways\Highway1L\Textures\Ground_Segment__APRMap.png"),
                        new LODTexturesSet
                           (@"Highways\Highway1L\Textures\Elevated_SegmentLOD__MainTex.png",
                            @"Highways\Highway1L\Textures\Elevated_SegmentLOD__APRMap.png",
                            @"Highways\Highway1L\Textures\Elevated_NodeLOD__XYSMap.png"));
                    info.SetAllNodesTexture(
                        new TexturesSet
                           (@"Highways\Highway1L\Textures\Ground_Segment__MainTex.png",
                            @"Highways\Highway1L\Textures\Ground_Node__APRMap.png"),
                        new LODTexturesSet
                           (@"Highways\Highway1L\Textures\Elevated_NodeLOD__MainTex.png",
                            @"Highways\Highway1L\Textures\Elevated_NodeLOD__APRMap.png",
                            @"Highways\Highway1L\Textures\Elevated_NodeLOD__XYSMap.png"));
                    break;

                case NetInfoVersion.Slope:
                    info.SetAllSegmentsTexture(
                        new TexturesSet
                           (@"Highways\Highway1L\Textures\Slope_Segment__MainTex.png",
                            @"Highways\Highway1L\Textures\Slope_Segment_Open__APRMap.png"),
                        new LODTexturesSet
                           (@"Highways\Highway1L\Textures\Slope_SegmentLOD__MainTex.png",
                            @"Highways\Highway1L\Textures\Slope_SegmentLOD__APRMap.png",
                            @"Highways\Highway1L\Textures\Slope_SegmentLOD__XYSMap.png"));
                    info.SetAllNodesTexture(
                        new TexturesSet
                           (@"Highways\Highway1L\Textures\Slope_Segment__MainTex.png",
                            @"Highways\Highway1L\Textures\Ground_Node__APRMap.png"),
                        new LODTexturesSet
                           (@"Highways\Highway1L\Textures\Ground_NodeLOD__MainTex.png",
                            @"Highways\Highway1L\Textures\Ground_NodeLOD__APRMap.png",
                            @"Highways\Highway1L\Textures\Ground_LOD__XYSMap.png"));
                    break;
                case NetInfoVersion.Tunnel:
                    info.SetAllSegmentsTexture(
                        new TexturesSet
                           (@"Highways\Highway1L\Textures\Tunnel_Segment__MainTex.png",
                            @"Highways\Highway1L\Textures\Tunnel_Segment__APRMap.png"),
                        new LODTexturesSet
                           (@"Highways\Highway1L\Textures\Tunnel_SegmentLOD__MainTex.png",
                            @"Highways\Highway1L\Textures\TunnelLOD__APRMap.png",
                            @"Highways\Highway1L\Textures\Slope_NodeLOD__XYSMap.png"));
                    info.SetAllNodesTexture(
                        new TexturesSet
                           (@"Highways\Highway1L\Textures\Tunnel_Segment__MainTex.png",
                            @"Highways\Highway1L\Textures\Tunnel_Node__APRMap.png"),
                        new LODTexturesSet
                           (@"Highways\Highway1L\Textures\Tunnel_NodeLOD__MainTex.png",
                            @"Highways\Highway1L\Textures\TunnelLOD__APRMap.png",
                            @"Highways\Highway1L\Textures\Slope_NodeLOD__XYSMap.png"));
                    break;
            }
        }
    }
}
