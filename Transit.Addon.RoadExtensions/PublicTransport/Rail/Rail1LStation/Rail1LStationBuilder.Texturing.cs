using Transit.Framework;
using Transit.Framework.Texturing;

namespace Transit.Addon.RoadExtensions.PublicTransport.Rail1LStation
{
    public partial class Rail1LStationBuilder
    {
        private static void SetupTextures(NetInfo info, NetInfoVersion version)
        {
            switch (version)
            {
                case NetInfoVersion.Ground:
                    for (var i = 0; i < info.m_segments.Length; i++)
                    {
                        if (info.m_segments[i].m_mesh.name.Contains("Ground_Pavement"))
                        {
                            info.m_segments[i].SetTextures(
                                new TextureSet
                                    (@"PublicTransport\Rail\Rail1LStation\Textures\Ground_Segment_Pavement__MainTex.png",
                                    @"PublicTransport\Rail\Rail1LStation\Textures\Ground_Segment_Pavement__AlphaMap.png"));
                        }
                    }
                    break;
                //           case NetInfoVersion.Elevated:
                //           case NetInfoVersion.Bridge:
                //               info.SetAllSegmentsTexture(
                //                   new TexturesSet
                //                       (@"Roads\BasicRoadTL\Textures\Elevated_MainTex.png",
                //                       @"Roads\BasicRoadTL\Textures\Elevated_Segment__APRMap.png"),
                //	new LODTexturesSet
                //		(@"Roads\BasicRoadTL\Textures\Elevated_LOD__MainTex.png",
                //		@"Roads\BasicRoadTL\Textures\Elevated_SegmentLOD__APRMap.png",
                //		@"Roads\BasicRoadTL\Textures\Elevated_LOD__XYSMap.png"));
                //info.SetAllNodesTexture(
                //                   new TexturesSet
                //                       (@"Roads\BasicRoadTL\Textures\Elevated_MainTex.png",
                //                       @"Roads\BasicRoadTL\Textures\Elevated_Node__APRMap.png"),
                //                   new LODTexturesSet
                //                       (@"Roads\BasicRoadTL\Textures\Elevated_LOD__MainTex.png",
                //                       @"Roads\BasicRoadTL\Textures\Elevated_NodeLOD__APRMap.png",
                //                       @"Roads\BasicRoadTL\Textures\Elevated_LOD__XYSMap.png"));
                //               break;
                //           case NetInfoVersion.Slope:
                //               for (int i = 0; i < info.m_segments.Length; i++)
                //               {
                //                   if (info.m_segments[i].m_mesh.name == "Slope")
                //                   {
                //                       info.m_segments[i].SetTextures(
                //                           new TexturesSet
                //                               (@"Roads\BasicRoadTL\Textures\Ground_Segment__MainTex.png",
                //                               @"Roads\BasicRoadTL\Textures\Ground_Segment__AlphaMap.png"),
                //                           new LODTexturesSet
                //                               (@"Roads\BasicRoadTL\Textures\Ground_SegmentLOD__MainTex.png",
                //                               @"Roads\BasicRoadTL\Textures\Ground_SegmentLOD__AlphaMap.png",
                //                               @"Roads\BasicRoadTL\Textures\Slope_SegmentLOD2__XYSMap.png"));
                //                   }
                //                   else
                //                   {
                //                       info.m_segments[i].SetTextures(
                //                           new TexturesSet
                //                              (@"Roads\Highway4L\Textures\Slope_Segment__MainTex.png",
                //                               @"Roads\BasicRoadTL\Textures\Slope_Segment__APRMap.png"),
                //                           new LODTexturesSet
                //                               (@"Roads\BasicRoadTL\Textures\Slope_SegmentLOD__MainTex.png",
                //                                @"Roads\BasicRoadTL\Textures\Slope_SegmentLOD__APRMap.png",
                //                                @"Roads\BasicRoadTL\Textures\Slope_SegmentLOD__XYSMap.png"));
                //                   }
                //               }
                //               for (int i = 0; i < info.m_nodes.Length; i++)
                //               {
                //                   if (info.m_nodes[i].m_mesh.name == "Slope_Node")
                //                   {
                //                       info.m_nodes[i].SetTextures(
                //                           new TexturesSet
                //                               (@"Roads\BasicRoadTL\Textures\Ground_Segment__MainTex.png",
                //                               @"Roads\BasicRoadTL\Textures\Ground_Node__AlphaMap.png"),
                //                           new LODTexturesSet
                //                               (@"Roads\BasicRoadTL\Textures\Ground_SegmentLOD__MainTex.png",
                //                               @"Roads\BasicRoadTL\Textures\Ground_SegmentLOD__AlphaMap.png",
                //                               @"Roads\BasicRoadTL\Textures\Ground_SegmentLOD__XYS.png"));
                //                   }
                //                   else
                //                   {
                //                       info.m_nodes[i].SetTextures(
                //                           new TexturesSet
                //                               (@"Roads\BasicRoadTL\Textures\Tunnel_Segment__MainTex.png",
                //                               @"Roads\BasicRoadTL\Textures\Tunnel_Node__APRMap.png"),
                //                           new LODTexturesSet
                //                               (@"Roads\BasicRoadTL\Textures\Tunnel_NodeLOD__MainTex.png",
                //                               @"Roads\BasicRoadTL\Textures\Tunnel_LOD__APRMap.png",
                //                               @"Roads\BasicRoadTL\Textures\Tunnel_LOD__XYSMap.png"));
                //                   }
                //               }
                //                   //info.SetAllNodesTexture(
                //                   //new TexturesSet
                //                   //    (@"Roads\Highway4L\Textures\Slope_Node__MainTex.png",
                //                   //    @"Roads\Highway4L\Textures\Ground_Node__APRMap.png"),
                //                   //new LODTexturesSet
                //                   //    (@"Roads\Highway4L\Textures\Ground_NodeLOD__MainTex.png",
                //                   //    @"Roads\Highway4L\Textures\Ground_NodeLOD__APRMap.png",
                //                   //    @"Roads\Highway4L\Textures\Ground_LOD__XYSMap.png"));
                //               break;
                case NetInfoVersion.Slope:
                    for (var i = 0; i < info.m_segments.Length; i++)
                    {
                        if (info.m_segments[i].m_mesh.name.Contains("slope"))
                        {
                            info.m_segments[i].SetTextures(
                                new TextureSet
                                    (@"PublicTransport\Rail\Rail1LStation\Textures\Slope_Segment__MainTex.png",
                                    @"PublicTransport\Rail\Rail1LStation\Textures\Slope_Segment__AlphaMap.png"),
                                new LODTextureSet
                                    (@"PublicTransport\Rail\Rail1LStation\Textures\Slope_Cover_LOD__MainTex.png",
                                    @"PublicTransport\Rail\Rail1LStation\Textures\Slope_Cover_LOD__AlphaMap.png",
                                    @"PublicTransport\Rail\Rail1LStation\Textures\Slope_Cover_LOD__XYSMap.png"));
                        }
                        else if (info.m_segments[i].m_mesh.name.Contains("Ground_Pavement"))
                        {
                            info.m_segments[i].SetTextures(
                                new TextureSet
                                    (@"PublicTransport\Rail\Rail1LStation\Textures\Ground_Segment_Pavement__MainTex.png",
                                    @"PublicTransport\Rail\Rail1LStation\Textures\Ground_Segment_Pavement__AlphaMap.png",
                                    null));
                        }
                    }
                    break;
                case NetInfoVersion.Tunnel:
                    {
                        info.SetAllSegmentsTexture(
                            new TextureSet
                                (@"PublicTransport\Rail\Rail1LStation\Textures\Tunnel_Segment__MainTex.png",
                                @"PublicTransport\Rail\Rail1LStation\Textures\Tunnel_Segment__AlphaMap.png"));
                        info.SetAllNodesTexture(
                            new TextureSet
                                (@"PublicTransport\Rail\Rail1LStation\Textures\Tunnel_Segment__MainTex.png",
                                @"PublicTransport\Rail\Rail1LStation\Textures\Tunnel_Segment__AlphaMap.png"));
                        break;
                    }
            }
        }
    }
}

