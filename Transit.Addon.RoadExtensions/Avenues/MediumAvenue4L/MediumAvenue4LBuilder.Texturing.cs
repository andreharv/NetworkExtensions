using Transit.Framework;

namespace Transit.Addon.RoadExtensions.Avenues.MediumAvenue4L
{
    public partial class MediumAvenue4LBuilder
    {
        private static void SetupTextures(NetInfo info, NetInfoVersion version)
        {
            switch (version)
            {
                case NetInfoVersion.Ground:
                    info.SetAllSegmentsTexture(
                        new TexturesSet
                            (@"Avenues\MediumAvenue4L\Textures\Ground_Segment__MainTex.png",
                            @"Avenues\MediumAvenue4L\Textures\Ground_Segment__APRMap.png"),
                        new LODTexturesSet
                            (@"Avenues\MediumAvenue4L\Textures\Ground_SegmentLOD__MainTex.png",
                            @"Avenues\MediumAvenue4L\Textures\Ground_SegmentLOD__APRMap.png",
                            @"Avenues\MediumAvenue4L\Textures\Ground_SegmentLOD__XYSMap.png"));
                    break;
                case NetInfoVersion.Elevated:
                case NetInfoVersion.Bridge:
                    info.SetAllSegmentsTexture(
                        new TexturesSet
                            (@"Avenues\MediumAvenue4L\Textures\Elevated_Segment__MainTex.png",
                            @"Avenues\MediumAvenue4L\Textures\Elevated_Segment__APRMap.png"),
                        new LODTexturesSet
                            (@"Avenues\MediumAvenue4L\Textures\Elevated_SegmentLOD__MainTex.png",
                            @"Avenues\MediumAvenue4L\Textures\Elevated_SegmentLOD__APRMap.png",
                            @"Avenues\MediumAvenue4L\Textures\Elevated_SegmentLOD__XYSMap.png"));
                    break;
                    //info.SetAllNodesTexture(
                    //    new TexturesSet
                    //        (@"Avenues\MediumAvenue4L\Textures\Elevated_Segment__MainTex.png",
                    //        @"Avenues\MediumAvenue4L\Textures\Elevated_Node__APRMap.png"),
                    //    new LODTexturesSet
                    //        (@"Avenues\MediumAvenue4L\Textures\Elevated_SegmentLOD__MainTex.png",
                    //        @"Avenues\MediumAvenue4L\Textures\Elevated_NodeLOD__APRMap.png",
                    //        @"Avenues\MediumAvenue4L\Textures\Elevated_SegmentLOD__XYSMap.png"));

                case NetInfoVersion.Slope:
                    info.SetAllSegmentsTexture(
                        new TexturesSet
                            (@"Avenues\MediumAvenue4L\Textures\Slope_Segment__MainTex.png",
                            @"Avenues\MediumAvenue4L\Textures\Slope_Segment__APRMap.png"),
                        new LODTexturesSet
                            (@"Avenues\MediumAvenue4L\Textures\Slope_SegmentLOD__MainTex.png",
                            @"Avenues\MediumAvenue4L\Textures\Slope_SegmentLOD__APRMap.png",
                            @"Avenues\MediumAvenue4L\Textures\Slope_SegmentLOD__XYSMap.png"));
                    foreach(var node in info.m_nodes)
                    {
                        if (node.m_mesh.name == "Slope_U_Node")
                        {
                            node.SetTextures(
                                new TexturesSet
                                    (@"Avenues\MediumAvenue4L\Textures\Tunnel_Segment__MainTex.png",
                                    @"Avenues\MediumAvenue4L\Textures\Tunnel_Node__APRMap.png"),
                                new LODTexturesSet
                                    (@"Avenues\MediumAvenue4L\Textures\Tunnel_SegmentLOD__MainTex.png",
                                    @"Avenues\MediumAvenue4L\Textures\Tunnel_SegmentLOD__APRMap.png",
                                    @"Avenues\MediumAvenue4L\Textures\Tunnel_SegmentLOD__XYSMap.png"));
                        }
                    }
                    //info.SetAllNodesTexture(
                    //new TexturesSet
                    //    (@"Highways\Highway4L\Textures\Slope_Node__MainTex.png",
                    //    @"Highways\Highway4L\Textures\Ground_Node__APRMap.png"),
                    //new LODTexturesSet
                    //    (@"Highways\Highway4L\Textures\Ground_NodeLOD__MainTex.png",
                    //    @"Highways\Highway4L\Textures\Ground_NodeLOD__APRMap.png",
                    //    @"Highways\Highway4L\Textures\Ground_LOD__XYSMap.png"));
                    break;
                case NetInfoVersion.Tunnel:
                    {
                        info.SetAllSegmentsTexture(
                            new TexturesSet
                                (@"Avenues\MediumAvenue4L\Textures\Tunnel_Segment__MainTex.png",
                                @"Avenues\MediumAvenue4L\Textures\Tunnel_Segment__APRMap.png"),
                            new LODTexturesSet
                                (@"Avenues\MediumAvenue4L\Textures\Tunnel_SegmentLOD__MainTex.png",
                                @"Avenues\MediumAvenue4L\Textures\Tunnel_SegmentLOD__APRMap.png",
                                @"Avenues\MediumAvenue4L\Textures\Tunnel_SegmentLOD__XYSMap.png"));
                        info.SetAllNodesTexture(
                            new TexturesSet
                                (@"Avenues\MediumAvenue4L\Textures\Tunnel_Segment__MainTex.png",
                                @"Avenues\MediumAvenue4L\Textures\Tunnel_Node__APRMap.png"),
                            new LODTexturesSet
                                (@"Avenues\MediumAvenue4L\Textures\Tunnel_SegmentLOD__MainTex.png",
                                @"Avenues\MediumAvenue4L\Textures\Tunnel_SegmentLOD__APRMap.png",
                                @"Avenues\MediumAvenue4L\Textures\Tunnel_SegmentLOD__XYSMap.png"));
                        break;
                    }
            }
        }
    }
}

