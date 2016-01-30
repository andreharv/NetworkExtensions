using Transit.Framework;
using Transit.Framework.Texturing;
using Transit.Addon.RoadExtensions.Compatibility;

namespace Transit.Addon.RoadExtensions.Roads.Highways.Highway2L
{
    public partial class Highway2LBuilder
    {
        private static void SetupTextures(NetInfo info, NetInfoVersion version)
        {
            var aprPathPrefix = (RoadColorChanger.IsPluginActive ? RoadColorChanger.GetTexturePrefix() : string.Empty);

            switch (version)
            {
                case NetInfoVersion.Ground:
                    info.SetAllSegmentsTexture(
                        new TextureSet(
                            @"Roads\Highways\Highway2L\Textures\Ground_Elevated_Segment__MainTex.png",
                            aprPathPrefix + @"Roads\Highways\Highway2L\Textures\Ground_Segment__APRMap.png"),
                        new LODTextureSet
                           (@"Roads\Highways\Highway2L\Textures\Ground_SegmentLOD__MainTex.png",
                            aprPathPrefix + @"Roads\Highways\Highway2L\Textures\Ground_SegmentLOD__APRMap.png",
                            @"Roads\Highways\Highway2L\Textures\Ground_LOD__XYSMap.png"));
                    info.SetAllNodesTexture(
                        new TextureSet
                           (@"Roads\Highways\Highway2L\Textures\Ground_Elevated_Node__MainTex.png",
                            aprPathPrefix + @"Roads\Highways\Highway2L\Textures\Ground_Node__APRMap.png"),
                        new LODTextureSet
                           (@"Roads\Highways\Highway2L\Textures\Ground_NodeLOD__MainTex.png",
                            aprPathPrefix + @"Roads\Highways\Highway2L\Textures\Ground_NodeLOD__APRMap.png",
                            @"Roads\Highways\Highway2L\Textures\Ground_LOD__XYSMap.png"));
                    break;

                case NetInfoVersion.Elevated:
                case NetInfoVersion.Bridge:
                    info.SetAllSegmentsTexture(
                        new TextureSet(
                            @"Roads\Highways\Highway2L\Textures\Ground_Elevated_Segment__MainTex.png",
                            aprPathPrefix + @"Roads\Highways\Highway2L\Textures\Elevated_Segment__APRMap.png"),
                        new LODTextureSet
                           (@"Roads\Highways\Highway2L\Textures\Elevated_SegmentLOD__MainTex.png",
                            aprPathPrefix + @"Roads\Highways\Highway2L\Textures\Elevated_SegmentLOD__APRMap.png",
                            @"Roads\Highways\Highway2L\Textures\Elevated_NodeLOD__XYSMap.png"));
                    info.SetAllNodesTexture(
                        new TextureSet
                           (@"Roads\Highways\Highway2L\Textures\Ground_Elevated_Node__MainTex.png",
                            aprPathPrefix + @"Roads\Highways\Highway2L\Textures\Elevated_Node__APRMap.png"),
                        new LODTextureSet
                           (@"Roads\Highways\Highway2L\Textures\Elevated_NodeLOD__MainTex.png",
                            aprPathPrefix + @"Roads\Highways\Highway2L\Textures\Elevated_NodeLOD__APRMap.png",
                            @"Roads\Highways\Highway2L\Textures\Elevated_NodeLOD__XYSMap.png"));
                    break;

                case NetInfoVersion.Slope:
                    info.SetAllSegmentsTexture(
                        new TextureSet
                           (@"Roads\Highways\Highway2L\Textures\Slope_Segment__MainTex.png",
                            aprPathPrefix + @"Roads\Highways\Highway2L\Textures\Slope_Segment_Open__APRMap.png"),
                        new LODTextureSet
                            (@"Roads\Highways\Highway2L\Textures\Slope_SegmentLOD__MainTex.png",
                            aprPathPrefix + @"Roads\Highways\Highway2L\Textures\Slope_SegmentLOD__APRMap.png",
                            @"Roads\Highways\Highway2L\Textures\Slope_SegmentLOD__XYSMap.png"));
                    info.SetAllNodesTexture(
                        new TextureSet
                           (@"Roads\Highways\Highway2L\Textures\Slope_Node__MainTex.png",
                            aprPathPrefix + @"Roads\Highways\Highway2L\Textures\Ground_Node__APRMap.png"),
                        new LODTextureSet
                           (@"Roads\Highways\Highway2L\Textures\Ground_NodeLOD__MainTex.png",
                            aprPathPrefix + @"Roads\Highways\Highway2L\Textures\Ground_NodeLOD__APRMap.png",
                            @"Roads\Highways\Highway2L\Textures\Ground_LOD__XYSMap.png"));
                    break;

                case NetInfoVersion.Tunnel:
                    info.SetAllSegmentsTexture(
                        new TextureSet
                           (@"Roads\Highways\Highway2L\Textures\Tunnel_Segment__MainTex.png",
                            aprPathPrefix + @"Roads\Highways\Highway2L\Textures\Tunnel" + (RoadColorChanger.IsPluginActive ? "_Segment" : "") + "__APRMap.png"),
                        new LODTextureSet
                           (@"Roads\Highways\Highway2L\Textures\Tunnel_SegmentLOD__MainTex.png",
                            aprPathPrefix + @"Roads\Highways\Highway2L\Textures\TunnelLOD__APRMap.png",
                            @"Roads\Highways\Highway2L\Textures\Slope_NodeLOD__XYSMap.png"));
                    info.SetAllNodesTexture(
                        new TextureSet
                           (@"Roads\Highways\Highway2L\Textures\Tunnel_Node__MainTex.png",
                            aprPathPrefix + @"Roads\Highways\Highway2L\Textures\Tunnel" + (RoadColorChanger.IsPluginActive ? "_Node" : "") + "__APRMap.png"),
                        new LODTextureSet
                           (@"Roads\Highways\Highway2L\Textures\Tunnel_NodeLOD__MainTex.png",
                            aprPathPrefix + @"Roads\Highways\Highway2L\Textures\TunnelLOD__APRMap.png",
                            @"Roads\Highways\Highway2L\Textures\Slope_NodeLOD__XYSMap.png"));
                    break;
            }
        }
    }
}
