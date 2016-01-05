using Transit.Framework;

namespace Transit.Addon.RoadExtensions.Roads.Highways.HighwayRamp
{
    public partial class HighwayRampBuilder
    {
        public static void SetupTextures(NetInfo info, NetInfoVersion version)
        {
            switch (version)
            {
                case NetInfoVersion.Slope:
                    info.SetAllSegmentsTexture(
                        new TexturesSet
                           (@"Roads\Highways\HighwayRamp\Textures\Slope_Segment__MainTex.png",
                            @"Roads\Highways\HighwayRamp\Textures\Slope_Segment_Open__APRMap.png"),
                    new LODTexturesSet
                        (@"Roads\Highways\HighwayRamp\Textures\Slope_SegmentLOD__MainTex.png",
                        @"Roads\Highways\HighwayRamp\Textures\Slope_SegmentLOD__APRMap.png",
                        @"Roads\Highways\HighwayRamp\Textures\Slope_SegmentLOD__XYSMap.png"));
                    foreach (var node in info.m_nodes)
                    {
                        if (node.m_mesh.name == "Slope_U_Node" || node.m_mesh.name == "Slope_U_Trans")
                        {
                            node.SetTextures(
                            new TexturesSet
                            (@"Roads\Highways\HighwayRamp\Textures\Slope_Node__MainTex.png",
                             @"Roads\Highways\HighwayRamp\Textures\Slope_Node__APRMap.png"),
                             new LODTexturesSet
                                (@"Roads\Highways\HighwayRamp\Textures\Ground_NodeLOD__MainTex.png",
                                 @"Roads\Highways\HighwayRamp\Textures\Ground_NodeLOD__APRMap.png",
                                 @"Roads\Highways\HighwayRamp\Textures\Ground_NodeLOD__XYSMap.png"));
                        }
                    }
                    break;

                case NetInfoVersion.Tunnel:
                    info.SetAllSegmentsTexture(
                        new TexturesSet
                           (@"Roads\Highways\HighwayRamp\Textures\Tunnel_Segment__MainTex.png",
                            @"Roads\Highways\HighwayRamp\Textures\Tunnel__APRMap.png"),
                    new LODTexturesSet
                       (@"Roads\Highways\HighwayRamp\Textures\Tunnel_SegmentLOD__MainTex.png",
                        @"Roads\Highways\HighwayRamp\Textures\TunnelLOD__APRMap.png",
                        @"Roads\Highways\HighwayRamp\Textures\Slope_NodeLOD__XYSMap.png"));
                    info.SetAllNodesTexture(
                        new TexturesSet
                           (@"Roads\Highways\HighwayRamp\Textures\Tunnel_Node__MainTex.png",
                            @"Roads\Highways\HighwayRamp\Textures\Tunnel__APRMap.png"),
                        new LODTexturesSet
                           (@"Roads\Highways\HighwayRamp\Textures\Tunnel_NodeLOD__MainTex.png",
                            @"Roads\Highways\HighwayRamp\Textures\TunnelLOD__APRMap.png",
                            @"Roads\Highways\HighwayRamp\Textures\Slope_NodeLOD__XYSMap.png"));
                    break;
            }
        }
    }
}