using Transit.Framework;
using Transit.Framework.Texturing;

namespace Transit.Addon.RoadExtensions.Roads.Avenues.MediumAvenue4LTL
{
    public partial class MediumAvenue4LTLBuilder
    {
        private static void SetupTextures(NetInfo info, NetInfoVersion version)
        {
            switch (version)
            {
                case NetInfoVersion.Ground:
                    info.SetAllSegmentsTexture(
                        new TextureSet
                            (@"Roads\Avenues\MediumAvenue4LTL\Textures\Ground_Segment__MainTex.png",
                            @"Roads\Avenues\MediumAvenue4LTL\Textures\Ground_Segment__APRMap.png"),
                        new LODTextureSet
                            (@"Roads\Avenues\MediumAvenue4LTL\Textures\Ground_SegmentLOD__MainTex.png",
                            @"Roads\Avenues\MediumAvenue4LTL\Textures\Ground_SegmentLOD__APRMap.png",
                            @"Roads\Avenues\MediumAvenue4LTL\Textures\Ground_SegmentLOD__XYSMap.png"));
                    break;
                case NetInfoVersion.Elevated:
                case NetInfoVersion.Bridge:
                    info.SetAllSegmentsTexture(
                        new TextureSet
                            (@"Roads\Avenues\MediumAvenue4LTL\Textures\Elevated_Segment__MainTex.png",
                            @"Roads\Avenues\MediumAvenue4LTL\Textures\Elevated_Segment__APRMap.png"),
                        new LODTextureSet
                            (@"Roads\Avenues\MediumAvenue4LTL\Textures\Elevated_SegmentLOD__MainTex.png",
                            @"Roads\Avenues\MediumAvenue4LTL\Textures\Elevated_SegmentLOD__APRMap.png",
                            @"Roads\Avenues\MediumAvenue4LTL\Textures\Elevated_SegmentLOD__XYSMap.png"));
                    break;
                    //info.SetAllNodesTexture(
                    //    new TextureSet
                    //        (@"Roads\Avenues\MediumAvenue4LTL\Textures\Elevated_Segment__MainTex.png",
                    //        @"Roads\Avenues\MediumAvenue4LTL\Textures\Elevated_Node__APRMap.png"),
                    //    new LODTextureSet
                    //        (@"Roads\Avenues\MediumAvenue4LTL\Textures\Elevated_SegmentLOD__MainTex.png",
                    //        @"Roads\Avenues\MediumAvenue4LTL\Textures\Elevated_NodeLOD__APRMap.png",
                    //        @"Roads\Avenues\MediumAvenue4LTL\Textures\Elevated_SegmentLOD__XYSMap.png"));

                case NetInfoVersion.Slope:
                    info.SetAllSegmentsTexture(
                        new TextureSet
                            (@"Roads\Avenues\MediumAvenue4LTL\Textures\Slope_Segment__MainTex.png",
                            @"Roads\Avenues\MediumAvenue4LTL\Textures\Slope_Segment__APRMap.png"),
                        new LODTextureSet
                            (@"Roads\Avenues\MediumAvenue4LTL\Textures\Slope_SegmentLOD__MainTex.png",
                            @"Roads\Avenues\MediumAvenue4LTL\Textures\Slope_SegmentLOD__APRMap.png",
                            @"Roads\Avenues\MediumAvenue4LTL\Textures\Slope_SegmentLOD__XYSMap.png"));
                    foreach(var node in info.m_nodes)
                    {
                        if (node.m_mesh.name == "Slope_U_Node")
                        {
                            node.SetTextures(
                                new TextureSet
                                    (@"Roads\Avenues\MediumAvenue4LTL\Textures\Tunnel_Segment__MainTex.png",
                                    @"Roads\Avenues\MediumAvenue4LTL\Textures\Tunnel_Node__APRMap.png"),
                                new LODTextureSet
                                    (@"Roads\Avenues\MediumAvenue4LTL\Textures\Tunnel_SegmentLOD__MainTex.png",
                                    @"Roads\Avenues\MediumAvenue4LTL\Textures\Tunnel_SegmentLOD__APRMap.png",
                                    @"Roads\Avenues\MediumAvenue4LTL\Textures\Tunnel_SegmentLOD__XYSMap.png"));
                        }
                    }
                    //info.SetAllNodesTexture(
                    //new TextureSet
                    //    (@"Roads\Highways\Highway4L\Textures\Slope_Node__MainTex.png",
                    //    @"Roads\Highways\Highway4L\Textures\Ground_Node__APRMap.png"),
                    //new LODTextureSet
                    //    (@"Roads\Highways\Highway4L\Textures\Ground_NodeLOD__MainTex.png",
                    //    @"Roads\Highways\Highway4L\Textures\Ground_NodeLOD__APRMap.png",
                    //    @"Roads\Highways\Highway4L\Textures\Ground_LOD__XYSMap.png"));
                    break;
                case NetInfoVersion.Tunnel:
                    {
                        info.SetAllSegmentsTexture(
                            new TextureSet
                                (@"Roads\Avenues\MediumAvenue4LTL\Textures\Tunnel_Segment__MainTex.png",
                                @"Roads\Avenues\MediumAvenue4LTL\Textures\Tunnel_Segment__APRMap.png"),
                            new LODTextureSet
                                (@"Roads\Avenues\MediumAvenue4LTL\Textures\Tunnel_SegmentLOD__MainTex.png",
                                @"Roads\Avenues\MediumAvenue4LTL\Textures\Tunnel_SegmentLOD__APRMap.png",
                                @"Roads\Avenues\MediumAvenue4LTL\Textures\Tunnel_SegmentLOD__XYSMap.png"));
                        info.SetAllNodesTexture(
                            new TextureSet
                                (@"Roads\Avenues\MediumAvenue4LTL\Textures\Tunnel_Segment__MainTex.png",
                                @"Roads\Avenues\MediumAvenue4LTL\Textures\Tunnel_Node__APRMap.png"),
                            new LODTextureSet
                                (@"Roads\Avenues\MediumAvenue4LTL\Textures\Tunnel_SegmentLOD__MainTex.png",
                                @"Roads\Avenues\MediumAvenue4LTL\Textures\Tunnel_SegmentLOD__APRMap.png",
                                @"Roads\Avenues\MediumAvenue4LTL\Textures\Tunnel_SegmentLOD__XYSMap.png"));
                        break;
                    }
            }
        }
    }
}

