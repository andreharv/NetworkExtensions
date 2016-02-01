using System.Collections.Generic;
using Transit.Framework;

namespace Transit.Addon.RoadExtensions.Roads.DivHighways.DivHighway4L
{
    public partial class DivHighway4LBuilder
    {
        private static void SetupTextures(NetInfo info, NetInfoVersion version)
        {
            switch (version)
            {
                case NetInfoVersion.Ground:
                    info.SetAllSegmentsTexture(
                        new TexturesSet(
                            @"Roads\DivHighways\DivHighway4L\Textures\Ground_Elevated_Segment__MainTex.png",
                            @"Roads\DivHighways\DivHighway4L\Textures\Ground_Elevated_Segment__APRMap.png"),
                        new LODTexturesSet
                            (@"Roads\DivHighways\DivHighway4L\Textures\Ground_SegmentLOD__MainTex.png",
                            @"Roads\DivHighways\DivHighway4L\Textures\Ground_SegmentLOD__APRMap.png",
                            @"Roads\DivHighways\DivHighway4L\Textures\Ground_SegmentLOD__XYSMap.png"));
                    for (int i = 0; i < info.m_nodes.Length; i++)
                    {
                        if (info.m_nodes[i].m_mesh.name.Contains("Ground_Node_Grass"))
                        {
                            info.m_nodes[i].SetTextures(
                                new TexturesSet(
                                    @"Roads\DivHighways\DivHighway4L\Textures\Ground_Elevated_Segment__MainTex.png",
                                    @"Roads\DivHighways\DivHighway4L\Textures\Ground_Elevated_Segment__APRMap.png"),
                                new LODTexturesSet
                                    (@"Roads\DivHighways\DivHighway4L\Textures\Ground_SegmentLOD__MainTex.png",
                                    @"Roads\DivHighways\DivHighway4L\Textures\Ground_SegmentLOD__APRMap.png",
                                    @"Roads\DivHighways\DivHighway4L\Textures\Ground_SegmentLOD__XYSMap.png"));
                        }
                        else
                        {
                            info.m_nodes[i].SetTextures(
                                new TexturesSet
                                    (@"Roads\DivHighways\DivHighway4L\Textures\Ground_Node__MainTex.png",
                                    @"Roads\DivHighways\DivHighway4L\Textures\Ground_Node__APRMap.png"),
                                new LODTexturesSet
                                    (@"Roads\DivHighways\DivHighway4L\Textures\Ground_NodeLOD__MainTex.png",
                                    @"Roads\DivHighways\DivHighway4L\Textures\Ground_NodeLOD__APRMap.png",
                                    @"Roads\DivHighways\DivHighway4L\Textures\Ground_NodeLOD__XYSMap.png"));
                        }
                    }

                    break;

                    //case NetInfoVersion.Elevated:
                    //case NetInfoVersion.Bridge:
                    //    info.SetAllSegmentsTexture(
                    //        new TexturesSet(
                    //            @"Roads\Highways\Highway1L\Textures\Ground_Segment__MainTex.png",
                    //            @"Roads\Highways\Highway1L\Textures\Ground_Segment__APRMap.png"),
                    //        new LODTexturesSet
                    //           (@"Roads\Highways\Highway1L\Textures\Elevated_SegmentLOD__MainTex.png",
                    //            @"Roads\Highways\Highway1L\Textures\Elevated_SegmentLOD__APRMap.png",
                    //            @"Roads\Highways\Highway1L\Textures\Elevated_NodeLOD__XYSMap.png"));
                    //    info.SetAllNodesTexture(
                    //        new TexturesSet
                    //           (@"Roads\Highways\Highway1L\Textures\Ground_Segment__MainTex.png",
                    //            @"Roads\Highways\Highway1L\Textures\Ground_Node__APRMap.png"),
                    //        new LODTexturesSet
                    //           (@"Roads\Highways\Highway1L\Textures\Elevated_NodeLOD__MainTex.png",
                    //            @"Roads\Highways\Highway1L\Textures\Elevated_NodeLOD__APRMap.png",
                    //            @"Roads\Highways\Highway1L\Textures\Elevated_NodeLOD__XYSMap.png"));
                    //    break;

                    //case NetInfoVersion.Slope:
                    //    info.SetAllSegmentsTexture(
                    //        new TexturesSet
                    //           (@"Roads\Highways\Highway1L\Textures\Slope_Segment__MainTex.png",
                    //            @"Roads\Highways\Highway1L\Textures\Slope_Segment_Open__APRMap.png"),
                    //        new LODTexturesSet
                    //           (@"Roads\Highways\Highway1L\Textures\Slope_SegmentLOD__MainTex.png",
                    //            @"Roads\Highways\Highway1L\Textures\Slope_SegmentLOD__APRMap.png",
                    //            @"Roads\Highways\Highway1L\Textures\Slope_SegmentLOD__XYSMap.png"));
                    //    info.SetAllNodesTexture(
                    //        new TexturesSet
                    //           (@"Roads\Highways\Highway1L\Textures\Slope_Segment__MainTex.png",
                    //            @"Roads\Highways\Highway1L\Textures\Ground_Node__APRMap.png"),
                    //        new LODTexturesSet
                    //           (@"Roads\Highways\Highway1L\Textures\Ground_NodeLOD__MainTex.png",
                    //            @"Roads\Highways\Highway1L\Textures\Ground_NodeLOD__APRMap.png",
                    //            @"Roads\Highways\Highway1L\Textures\Ground_LOD__XYSMap.png"));
                    //    break;
                    //case NetInfoVersion.Tunnel:
                    //    info.SetAllSegmentsTexture(
                    //        new TexturesSet
                    //           (@"Roads\Highways\Highway1L\Textures\Tunnel_Segment__MainTex.png",
                    //            @"Roads\Highways\Highway1L\Textures\Tunnel_Segment__APRMap.png"),
                    //        new LODTexturesSet
                    //           (@"Roads\Highways\Highway1L\Textures\Tunnel_SegmentLOD__MainTex.png",
                    //            @"Roads\Highways\Highway1L\Textures\TunnelLOD__APRMap.png",
                    //            @"Roads\Highways\Highway1L\Textures\Slope_NodeLOD__XYSMap.png"));
                    //    info.SetAllNodesTexture(
                    //        new TexturesSet
                    //           (@"Roads\Highways\Highway1L\Textures\Tunnel_Segment__MainTex.png",
                    //            @"Roads\Highways\Highway1L\Textures\Tunnel_Node__APRMap.png"),
                    //        new LODTexturesSet
                    //           (@"Roads\Highways\Highway1L\Textures\Tunnel_NodeLOD__MainTex.png",
                    //            @"Roads\Highways\Highway1L\Textures\TunnelLOD__APRMap.png",
                    //            @"Roads\Highways\Highway1L\Textures\Slope_NodeLOD__XYSMap.png"));
                    //    break;
            }
        }
    }
}