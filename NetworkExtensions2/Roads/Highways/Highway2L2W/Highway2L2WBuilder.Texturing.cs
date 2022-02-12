using Transit.Addon.RoadExtensions.Compatibility;
using Transit.Framework;
using Transit.Framework.Texturing;

namespace Transit.Addon.RoadExtensions.Roads.Highways.Highway2L2W
{
    public partial class Highway2L2WBuilder
    {
        private static void SetupTextures(NetInfo info, NetInfoVersion version)
        {
            var aprPathPrefix = (RoadColorChanger.IsPluginActive ? RoadColorChanger.GetTexturePrefix() : string.Empty);

            switch (version)
            {
                case NetInfoVersion.Ground:
                    info.SetAllSegmentsTexture(
                        new TextureSet(
                            @"Roads\Highways\Highway2L2W\Textures\Ground_Segment__MainTex.png",
                            aprPathPrefix + @"Roads\Highways\Highway2L2W\Textures\Ground_Segment__APRMap.png"),
                        new LODTextureSet
                           (@"Roads\Highways\Highway2L2W\Textures\Ground_SegmentLOD__MainTex.png",
                            aprPathPrefix + @"Roads\Highways\Highway2L2W\Textures\Ground_SegmentLOD__APRMap.png",
                            @"Roads\Highways\Highway2L2W\Textures\Ground_LOD__XYSMap.png"));
                    info.SetAllNodesTexture(
                        new TextureSet
                           (@"Roads\Highways\Highway2L2W\Textures\Ground_Node__MainTex.png",
                            aprPathPrefix + @"Roads\Highways\Highway2L2W\Textures\Ground_Node__APRMap.png"),
                        new LODTextureSet
                           (@"Roads\Highways\Highway2L2W\Textures\Ground_NodeLOD__MainTex.png",
                            aprPathPrefix + @"Roads\Highways\Highway2L2W\Textures\Ground_NodeLOD__APRMap.png",
                            @"Roads\Highways\Highway2L2W\Textures\Ground_LOD__XYSMap.png"));
                    break;

                case NetInfoVersion.Elevated:
                case NetInfoVersion.Bridge:
                    info.SetAllSegmentsTexture(
                        new TextureSet(
                            @"Roads\Highways\Highway2L2W\Textures\Elevated_Segment__MainTex.png",
                            aprPathPrefix + @"Roads\Highways\Highway2L2W\Textures\Elevated_Segment__APRMap.png"),
                        new LODTextureSet
                           (@"Roads\Highways\Highway2L2W\Textures\Elevated_SegmentLOD__MainTex.png",
                            aprPathPrefix + @"Roads\Highways\Highway2L2W\Textures\Elevated_SegmentLOD__APRMap.png",
                            @"Roads\Highways\Highway2L2W\Textures\Elevated_LOD__XYSMap.png"));
                    info.SetAllNodesTexture(
                        new TextureSet
                           (@"Roads\Highways\Highway2L2W\Textures\Elevated_Node__MainTex.png",
                            aprPathPrefix + @"Roads\Highways\Highway2L2W\Textures\Elevated_Node__APRMap.png"),
                        new LODTextureSet
                           (@"Roads\Highways\Highway2L2W\Textures\Elevated_NodeLOD__MainTex.png",
                            aprPathPrefix + @"Roads\Highways\Highway2L2W\Textures\Elevated_NodeLOD__APRMap.png",
                            @"Roads\Highways\Highway2L2W\Textures\Elevated_LOD__XYSMap.png"));
                    break;

                case NetInfoVersion.Slope:
                    info.SetAllSegmentsTexture(
                        new TextureSet
                           (@"Roads\Highways\Highway2L2W\Textures\Slope_Segment__MainTex.png",
                            aprPathPrefix + @"Roads\Highways\Highway2L2W\Textures\Slope_Segment_Open__APRMap.png"),
                        new LODTextureSet
                            (@"Roads\Highways\Highway2L2W\Textures\Slope_SegmentLOD__MainTex.png",
                             @"Roads\Highways\Highway2L2W\Textures\Slope_SegmentLOD__APRMap.png",
                             @"Roads\Highways\Highway2L2W\Textures\Slope_LOD__XYSMap.png"));
                    info.SetAllNodesTexture(
                        new TextureSet
                           (@"Roads\Highways\Highway2L2W\Textures\Slope_Node__MainTex.png",
                            aprPathPrefix + @"Roads\Highways\Highway2L2W\Textures\Ground_Node__APRMap.png"),
                        new LODTextureSet
                           (@"Roads\Highways\Highway2L2W\Textures\Ground_NodeLOD__MainTex.png",
                            aprPathPrefix + @"Roads\Highways\Highway2L2W\Textures\Ground_NodeLOD__APRMap.png",
                            @"Roads\Highways\Highway2L2W\Textures\Ground_LOD__XYSMap.png"));
                    break;
                case NetInfoVersion.Tunnel:
                    info.SetAllSegmentsTexture(
                        new TextureSet
                           (@"Roads\Highways\Highway2L2W\Textures\Tunnel_Segment__MainTex.png",
                            aprPathPrefix + @"Roads\Highways\Highway2L2W\Textures\Tunnel" + (RoadColorChanger.IsPluginActive ? "_Segment" : "") + "__APRMap.png"),
                        new LODTextureSet
                           (@"Roads\Highways\Highway2L2W\Textures\Tunnel_SegmentLOD__MainTex.png",
                            aprPathPrefix + @"Roads\Highways\Highway2L2W\Textures\TunnelLOD__APRMap.png",
                            @"Roads\Highways\Highway2L2W\Textures\Slope_LOD__XYSMap.png"));
                    info.SetAllNodesTexture(
                        new TextureSet
                           (@"Roads\Highways\Highway2L2W\Textures\Tunnel_Node__MainTex.png",
                            aprPathPrefix + @"Roads\Highways\Highway2L2W\Textures\Tunnel" + (RoadColorChanger.IsPluginActive ? "_Node" : "") + "__APRMap.png"),
                        new LODTextureSet
                           (@"Roads\Highways\Highway2L2W\Textures\Tunnel_NodeLOD__MainTex.png",
                            aprPathPrefix + @"Roads\Highways\Highway2L2W\Textures\TunnelLOD__APRMap.png",
                            @"Roads\Highways\Highway2L2W\Textures\Slope_LOD__XYSMap.png"));
                    break;
            }
        }
    }
}