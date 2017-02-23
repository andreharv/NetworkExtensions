using Transit.Addon.RoadExtensions.Compatibility;
using Transit.Framework;
using Transit.Framework.Texturing;

namespace Transit.Addon.RoadExtensions.Roads.Highways.HighwayL1R2
{
    public partial class HighwayL1R2Builder
    {
        private static void SetupTextures(NetInfo info, NetInfoVersion version)
        {
            var aprPathPrefix = (RoadColorChanger.IsPluginActive ? RoadColorChanger.GetTexturePrefix() : string.Empty);

            switch (version)
            {
                case NetInfoVersion.Ground:
                    info.SetAllSegmentsTexture(
                        new TextureSet(
                            @"Roads\Highways\HighwayL1R2\Textures\Ground_Segment__MainTex.png",
                            aprPathPrefix + @"Roads\Highways\HighwayL1R2\Textures\Ground_Segment__APRMap.png"),
                        new LODTextureSet
                           (@"Roads\Highways\HighwayL1R2\Textures\Ground_SegmentLOD__MainTex.png",
                            aprPathPrefix + @"Roads\Highways\HighwayL1R2\Textures\Ground_SegmentLOD__APRMap.png",
                            @"Roads\Highways\HighwayL1R2\Textures\Ground_LOD__XYSMap.png"));
                    info.SetAllNodesTexture(
                        new TextureSet
                           (@"Roads\Highways\HighwayL1R2\Textures\Ground_Node__MainTex.png",
                            aprPathPrefix + @"Roads\Highways\HighwayL1R2\Textures\Ground_Node__APRMap.png"),
                        new LODTextureSet
                           (@"Roads\Highways\HighwayL1R2\Textures\Ground_NodeLOD__MainTex.png",
                            aprPathPrefix + @"Roads\Highways\HighwayL1R2\Textures\Ground_NodeLOD__APRMap.png",
                            @"Roads\Highways\HighwayL1R2\Textures\Ground_LOD__XYSMap.png"));
                    break;

                case NetInfoVersion.Elevated:
                case NetInfoVersion.Bridge:
                    info.SetAllSegmentsTexture(
                        new TextureSet(
                            @"Roads\Highways\HighwayL1R2\Textures\Elevated_Segment__MainTex.png",
                            aprPathPrefix + @"Roads\Highways\HighwayL1R2\Textures\Elevated_Segment__APRMap.png"),
                        new LODTextureSet
                           (@"Roads\Highways\HighwayL1R2\Textures\Elevated_SegmentLOD__MainTex.png",
                            aprPathPrefix + @"Roads\Highways\HighwayL1R2\Textures\Elevated_SegmentLOD__APRMap.png",
                            @"Roads\Highways\HighwayL1R2\Textures\Elevated_LOD__XYSMap.png"));
                    info.SetAllNodesTexture(
                        new TextureSet
                           (@"Roads\Highways\HighwayL1R2\Textures\Elevated_Node__MainTex.png",
                            aprPathPrefix + @"Roads\Highways\HighwayL1R2\Textures\Elevated_Node__APRMap.png"),
                        new LODTextureSet
                           (@"Roads\Highways\HighwayL1R2\Textures\Elevated_NodeLOD__MainTex.png",
                            aprPathPrefix + @"Roads\Highways\HighwayL1R2\Textures\Elevated_NodeLOD__APRMap.png",
                            @"Roads\Highways\HighwayL1R2\Textures\Elevated_LOD__XYSMap.png"));
                    break;

                case NetInfoVersion.Slope:
                    info.SetAllSegmentsTexture(
                        new TextureSet
                           (@"Roads\Highways\HighwayL1R2\Textures\Slope_Segment__MainTex.png",
                            aprPathPrefix + @"Roads\Highways\HighwayL1R2\Textures\Slope_Segment_Open__APRMap.png"),
                        new LODTextureSet
                            (@"Roads\Highways\HighwayL1R2\Textures\Slope_SegmentLOD__MainTex.png",
                             @"Roads\Highways\HighwayL1R2\Textures\Slope_SegmentLOD__APRMap.png",
                             @"Roads\Highways\HighwayL1R2\Textures\Slope_LOD__XYSMap.png"));
                    info.SetAllNodesTexture(
                        new TextureSet
                           (@"Roads\Highways\HighwayL1R2\Textures\Slope_Node__MainTex.png",
                            aprPathPrefix + @"Roads\Highways\HighwayL1R2\Textures\Ground_Node__APRMap.png"),
                        new LODTextureSet
                           (@"Roads\Highways\HighwayL1R2\Textures\Ground_NodeLOD__MainTex.png",
                            aprPathPrefix + @"Roads\Highways\HighwayL1R2\Textures\Ground_NodeLOD__APRMap.png",
                            @"Roads\Highways\HighwayL1R2\Textures\Ground_LOD__XYSMap.png"));
                    break;
                case NetInfoVersion.Tunnel:
                    info.SetAllSegmentsTexture(
                        new TextureSet
                           (@"Roads\Highways\HighwayL1R2\Textures\Tunnel_Segment__MainTex.png",
                            aprPathPrefix + @"Roads\Highways\HighwayL1R2\Textures\Tunnel" + (RoadColorChanger.IsPluginActive ? "_Segment" : "") + "__APRMap.png"),
                        new LODTextureSet
                           (@"Roads\Highways\HighwayL1R2\Textures\Tunnel_SegmentLOD__MainTex.png",
                            aprPathPrefix + @"Roads\Highways\HighwayL1R2\Textures\TunnelLOD__APRMap.png",
                            @"Roads\Highways\HighwayL1R2\Textures\Slope_LOD__XYSMap.png"));
                    info.SetAllNodesTexture(
                        new TextureSet
                           (@"Roads\Highways\HighwayL1R2\Textures\Tunnel_Node__MainTex.png",
                            aprPathPrefix + @"Roads\Highways\HighwayL1R2\Textures\Tunnel" + (RoadColorChanger.IsPluginActive ? "_Node" : "") + "__APRMap.png"),
                        new LODTextureSet
                           (@"Roads\Highways\HighwayL1R2\Textures\Tunnel_NodeLOD__MainTex.png",
                            aprPathPrefix + @"Roads\Highways\HighwayL1R2\Textures\TunnelLOD__APRMap.png",
                            @"Roads\Highways\HighwayL1R2\Textures\Slope_LOD__XYSMap.png"));
                    break;
            }
        }
    }
}