using Transit.Addon.RoadExtensions.Compatibility;
using Transit.Framework;
using Transit.Framework.Texturing;

namespace Transit.Addon.RoadExtensions.Roads.Highways.Highway3L
{
    public partial class Highway3LBuilder
    {
        public static void SetupTextures(NetInfo info, NetInfoVersion version)
        {
            var aprPathPrefix = (RoadColorChanger.IsPluginActive ? RoadColorChanger.GetTexturePrefix() : string.Empty);

            switch (version)
            {
                case NetInfoVersion.Slope:
                    info.SetAllSegmentsTexture(
                        new TextureSet
                           (@"Roads\Highways\Highway3L\Textures\Slope_Segment__MainTex.png",
                            aprPathPrefix + @"Roads\Highways\Highway3L\Textures\Slope_Segment_Open__APRMap.png"),
                        new LODTextureSet
                            (@"Roads\Highways\Highway3L\Textures\Slope_SegmentLOD__MainTex.png",
                             @"Roads\Highways\Highway3L\Textures\Slope_SegmentLOD__APRMap.png",
                             @"Roads\Highways\Highway3L\Textures\Slope_LOD__XYSMap.png"));
                    info.SetAllNodesTexture(
                        new TextureSet
                           (@"Roads\Highways\Highway3L\Textures\Slope_Node__MainTex.png",
                            aprPathPrefix + @"Roads\Highways\Highway3L\Textures\Ground_Node__APRMap.png"),
                        new LODTextureSet
                           (@"Roads\Highways\Highway3L\Textures\Ground_NodeLOD__MainTex.png",
                            aprPathPrefix + @"Roads\Highways\Highway3L\Textures\Ground_NodeLOD__APRMap.png",
                            @"Roads\Highways\Highway3L\Textures\Ground_LOD__XYSMap.png"));
                    break;
                case NetInfoVersion.Tunnel:
                    info.SetAllSegmentsTexture(
                        new TextureSet
                           (@"Roads\Highways\Highway3L\Textures\Tunnel_Segment__MainTex.png",
                            aprPathPrefix + @"Roads\Highways\Highway3L\Textures\Tunnel" + (RoadColorChanger.IsPluginActive ? "_Segment" : "") + "__APRMap.png"),
                        new LODTextureSet
                           (@"Roads\Highways\Highway3L\Textures\Tunnel_SegmentLOD__MainTex.png",
                            aprPathPrefix + @"Roads\Highways\Highway3L\Textures\TunnelLOD__APRMap.png",
                            @"Roads\Highways\Highway3L\Textures\Slope_LOD__XYSMap.png"));
                    info.SetAllNodesTexture(
                        new TextureSet
                           (@"Roads\Highways\Highway3L\Textures\Tunnel_Node__MainTex.png",
                            aprPathPrefix + @"Roads\Highways\Highway3L\Textures\Tunnel" + (RoadColorChanger.IsPluginActive ? "_Node" : "") + "__APRMap.png"),
                        new LODTextureSet
                           (@"Roads\Highways\Highway3L\Textures\Tunnel_NodeLOD__MainTex.png",
                            aprPathPrefix + @"Roads\Highways\Highway3L\Textures\TunnelLOD__APRMap.png",
                            @"Roads\Highways\Highway3L\Textures\Slope_LOD__XYSMap.png"));
                    break;
            }
        }
    }
}