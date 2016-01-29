using Transit.Addon.RoadExtensions.Compatibility;
using Transit.Framework;
using Transit.Framework.Texturing;

namespace Transit.Addon.RoadExtensions.Roads.Highways.Highway6L
{
    public partial class Highway6LBuilder
    {
        private static void SetupTextures(NetInfo info, NetInfoVersion version)
        {
            RoadColorChanger rcc = new RoadColorChanger();
            var aprMapPath = (rcc.IsPluginActive ? rcc.GetTexturePrefix() : @"Roads\");

            switch (version)
            {
                case NetInfoVersion.Ground:
                    info.SetAllSegmentsTexture(
                        new TexturesSet(
                            @"Roads\Highways\Highway6L\Textures\Ground_Segment__MainTex.png",
                            aprMapPath + @"Highways\Highway6L\Textures\Ground_Segment__APRMap.png"),
                        new LODTexturesSet
                           (@"Roads\Highways\Highway6L\Textures\Ground_SegmentLOD__MainTex.png",
                            aprMapPath + @"Highways\Highway6L\Textures\Ground_SegmentLOD__APRMap.png",
                            @"Roads\Highways\Highway6L\Textures\Ground_SegmentLOD__XYSMap.png"));
                    info.SetAllNodesTexture(
                        new TexturesSet
                           (@"Roads\Highways\Highway6L\Textures\Ground_Node__MainTex.png",
                            aprMapPath + @"Highways\Highway6L\Textures\Ground_Node__APRMap.png"),
                        new LODTexturesSet
                           (@"Roads\Highways\Highway6L\Textures\Ground_NodeLOD__MainTex.png",
                            aprMapPath + @"Highways\Highway6L\Textures\Ground_NodeLOD__APRMap.png",
                            @"Roads\Highways\Highway6L\Textures\Ground_NodeLOD__XYSMap.png"));
                    break;

                case NetInfoVersion.Elevated:
                case NetInfoVersion.Bridge:
                    info.SetAllSegmentsTexture(
                        new TexturesSet(
                            @"Roads\Highways\Highway6L\Textures\Elevated_Segment__MainTex.png",
                            aprMapPath + @"Highways\Highway6L\Textures\Elevated_Segment__APRMap.png"),
                        new LODTexturesSet
                           (@"Roads\Highways\Highway6L\Textures\Elevated_SegmentLOD__MainTex.png",
                            aprMapPath + @"Highways\Highway6L\Textures\Elevated_SegmentLOD__APRMap.png",
                            @"Roads\Highways\Highway6L\Textures\Elevated_LOD__XYSMap.png"));
                    info.SetAllNodesTexture(
                        new TexturesSet
                           (@"Roads\Highways\Highway6L\Textures\Elevated_Node__MainTex.png",
                            aprMapPath + @"Highways\Highway6L\Textures\Elevated_Node__APRMap.png"),
                        new LODTexturesSet
                           (@"Roads\Highways\Highway6L\Textures\Elevated_NodeLOD__MainTex.png",
                            aprMapPath + @"Highways\Highway6L\Textures\Elevated_NodeLOD__APRMap.png",
                            @"Roads\Highways\Highway6L\Textures\Elevated_LOD__XYSMap.png"));
                    break;

                case NetInfoVersion.Slope:
                    info.SetAllSegmentsTexture(
                        new TexturesSet
                           (@"Roads\Highways\Highway6L\Textures\Slope_Segment__MainTex.png",
                            aprMapPath + @"Highways\Highway6L\Textures\Slope_Segment__APRMap.png"),
                        new LODTexturesSet
                            (@"Roads\Highways\Highway6L\Textures\Slope_SegmentLOD__MainTex.png",
                            aprMapPath + @"Highways\Highway6L\Textures\Slope_SegmentLOD__APRMap.png",
                            @"Roads\Highways\Highway6L\Textures\Slope_SegmentLOD__XYSMap.png"));
                    info.SetAllNodesTexture(
                        new TexturesSet
                           (@"Roads\Highways\Highway6L\Textures\Tunnel_Node__MainTex.png",
                            aprMapPath + @"Highways\Highway6L\Textures\Ground_Node__APRMap.png"),
                        new LODTexturesSet
                           (@"Roads\Highways\Highway6L\Textures\Ground_NodeLOD__MainTex.png",
                            aprMapPath + @"Highways\Highway6L\Textures\Ground_NodeLOD__APRMap.png",
                            @"Roads\Highways\Highway6L\Textures\Ground_NodeLOD__XYSMap.png"));
                    break;
                case NetInfoVersion.Tunnel:
                    info.SetAllSegmentsTexture(
                        new TexturesSet
                           (@"Roads\Highways\Highway6L\Textures\Tunnel_Segment__MainTex.png",
                            aprMapPath + @"Highways\Highway6L\Textures\Tunnel_Segment__APRMap.png"),
                        new LODTexturesSet
                           (@"Roads\Highways\Highway6L\Textures\Tunnel_SegmentLOD__MainTex.png",
                            aprMapPath + @"Highways\Highway6L\Textures\Tunnel_SegmentLOD__APRMap.png",
                            @"Roads\Highways\Highway6L\Textures\Tunnel_NodeLOD__XYSMap.png"));
                    info.SetAllNodesTexture(
                        new TexturesSet
                           (@"Roads\Highways\Highway6L\Textures\Tunnel_Node__MainTex.png",
                            aprMapPath + @"Highways\Highway6L\Textures\Tunnel_" + (rcc.IsPluginActive ? "Node" : "Segment") + "__APRMap.png"),
                        new LODTexturesSet
                           (@"Roads\Highways\Highway6L\Textures\Tunnel_NodeLOD__MainTex.png",
                            aprMapPath + @"Highways\Highway6L\Textures\Tunnel_SegmentLOD__APRMap.png",
                            @"Roads\Highways\Highway6L\Textures\Tunnel_NodeLOD__XYSMap.png"));
                    break;
            }
        }
    }
}