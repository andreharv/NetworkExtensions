using Transit.Framework;
using Transit.Framework.Texturing;

namespace Transit.Addon.RoadExtensions.Roads.SmallRoads.OneWay1LParkingBicycle
{
    public partial class OneWay1LParkingBicycleBuilder
    {
        private static void SetupTextures(NetInfo info, NetInfoVersion version)
        {
            switch (version)
            {
                case NetInfoVersion.Ground:
                case NetInfoVersion.GroundGrass:
                case NetInfoVersion.GroundTrees:

                    info.SetAllSegmentsTexture(
                       new TextureSet
                            (@"Roads\SmallRoads\OneWay1LParkingBicycle\Textures\Ground_Segment__MainTex.png",
                             @"Roads\SmallRoads\OneWay1LParkingBicycle\Textures\Ground_Segment__AlphaMap.png"),
                       new LODTextureSet
                           (@"Roads\SmallRoads\OneWay1LParkingBicycle\Textures\Ground_SegmentLOD__MainTex.png",
                           @"Roads\SmallRoads\OneWay1LParkingBicycle\Textures\Ground_SegmentLOD__AlphaMap.png",
                           @"Roads\SmallRoads\OneWay1LParkingBicycle\Textures\Ground_SegmentLOD__XYS.png"));
                    break;
         
                case NetInfoVersion.Elevated:
                case NetInfoVersion.Bridge:
                    info.SetAllSegmentsTexture(
                        new TextureSet
                            (@"Roads\SmallRoads\OneWay1LParkingBicycle\Textures\Elevated_MainTex.png",
                            @"Roads\SmallRoads\OneWay1LParkingBicycle\Textures\Elevated_Segment__APRMap.png"),
						new LODTextureSet
							(@"Roads\SmallRoads\OneWay1LParkingBicycle\Textures\Elevated_LOD__MainTex.png",
							@"Roads\SmallRoads\OneWay1LParkingBicycle\Textures\Elevated_SegmentLOD__APRMap.png",
							@"Roads\SmallRoads\OneWay1LParkingBicycle\Textures\Elevated_LOD__XYSMap.png"));
					info.SetAllNodesTexture(
                        new TextureSet
                            (@"Roads\SmallRoads\OneWay1LParkingBicycle\Textures\Elevated_MainTex.png",
                            @"Roads\SmallRoads\OneWay1LParkingBicycle\Textures\Elevated_Node__APRMap.png"),
                        new LODTextureSet
                            (@"Roads\SmallRoads\OneWay1LParkingBicycle\Textures\Elevated_LOD__MainTex.png",
                            @"Roads\SmallRoads\OneWay1LParkingBicycle\Textures\Elevated_NodeLOD__APRMap.png",
                            @"Roads\SmallRoads\OneWay1LParkingBicycle\Textures\Elevated_LOD__XYSMap.png"));
                    break;
                case NetInfoVersion.Slope:
                    for (int i = 0; i < info.m_segments.Length; i++)
                    {
                        if (info.m_segments[i].m_mesh.name == "Slope")
                        {
                            info.m_segments[i].SetTextures(
                                new TextureSet
                                    (@"Roads\SmallRoads\OneWay1LParkingBicycle\Textures\Slope_Segment__MainTex.png",
                                    @"Roads\SmallRoads\OneWay1LParkingBicycle\Textures\Ground_Segment__AlphaMap.png"),
                                new LODTextureSet
                                    (@"Roads\SmallRoads\OneWay1LParkingBicycle\Textures\Ground_SegmentLOD__MainTex.png",
                                    @"Roads\SmallRoads\OneWay1LParkingBicycle\Textures\Ground_SegmentLOD__AlphaMap.png",
                                    @"Roads\SmallRoads\OneWay1LParkingBicycle\Textures\Slope_SegmentLOD2__XYSMap.png"));
                        }
                        else
                        {
                            info.m_segments[i].SetTextures(
                                new TextureSet
                                   (@"Roads\Highways\Highway4L\Textures\Slope_Segment__MainTex.png",
                                    @"Roads\SmallHeavyRoads\SmallAvenue4L\Textures\Slope_Segment__APRMap.png"),
                                new LODTextureSet
                                    (@"Roads\SmallHeavyRoads\SmallAvenue4L\Textures\Slope_SegmentLOD__MainTex.png",
                                     @"Roads\SmallHeavyRoads\SmallAvenue4L\Textures\Slope_SegmentLOD__APRMap.png",
                                     @"Roads\SmallHeavyRoads\SmallAvenue4L\Textures\Slope_SegmentLOD__XYSMap.png"));
                        }
                    }
                    for (int i = 0; i < info.m_nodes.Length; i++)
                    {
                        if (info.m_nodes[i].m_mesh.name == "Slope_Node")
                        {
                            info.m_nodes[i].SetTextures(
                                new TextureSet
                                    (@"Roads\SmallRoads\OneWay1LParkingBicycle\Textures\Slope_Node__MainTex.png",
                                    @"Roads\SmallHeavyRoads\SmallAvenue4L\Textures\Ground_Node__AlphaMap.png"),
                                new LODTextureSet
                                    (@"Roads\SmallHeavyRoads\SmallAvenue4L\Textures\Ground_SegmentLOD__MainTex.png",
                                    @"Roads\SmallHeavyRoads\SmallAvenue4L\Textures\Ground_SegmentLOD__AlphaMap.png",
                                    @"Roads\SmallHeavyRoads\SmallAvenue4L\Textures\Ground_SegmentLOD__XYS.png"));
                        }
                        else
                        {
                            info.m_nodes[i].SetTextures(
                                new TextureSet
                                    (@"Roads\SmallHeavyRoads\SmallAvenue4L\Textures\Tunnel_Segment__MainTex.png",
                                    @"Roads\SmallHeavyRoads\SmallAvenue4L\Textures\Tunnel_Node__APRMap.png"),
                                new LODTextureSet
                                    (@"Roads\SmallHeavyRoads\SmallAvenue4L\Textures\Tunnel_NodeLOD__MainTex.png",
                                    @"Roads\SmallHeavyRoads\SmallAvenue4L\Textures\Tunnel_LOD__APRMap.png",
                                    @"Roads\SmallHeavyRoads\SmallAvenue4L\Textures\Tunnel_LOD__XYSMap.png"));
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

                /*

            case NetInfoVersion.Slope:
                for (int i = 0; i < info.m_segments.Length; i++)
                {
                    if (info.m_segments[i].m_mesh.name == "Slope")
                    {
                        info.m_segments[i].SetTextures(
                            new TextureSet
                                (@"Roads\SmallRoads\OneWay1LParkingBicycle\Textures\Slope_Segment__MainTex.png",
                                @"Roads\SmallRoads\OneWay1LParkingBicycle\Textures\Ground_Segment__AlphaMap.png"),
                            new LODTextureSet
                                (@"Roads\SmallRoads\OneWay1LParkingBicycle\Textures\Slope_SegmentLOD__MainTex.png",
                                @"Roads\SmallRoads\OneWay1LParkingBicycle\Textures\Ground_SegmentLOD__AlphaMap.png",
                                @"Roads\SmallRoads\OneWay1LParkingBicycle\Textures\Slope_SegmentLOD2__XYSMap.png"));
                    }
                    else
                    {
                        info.m_segments[i].SetTextures(
                            new TextureSet
                               (@"Roads\SmallRoads\OneWay1LParkingBicycle\Textures\Slope_Segment__MainTex.png",
                                @"Roads\SmallRoads\OneWay1LParkingBicycle\Textures\Slope_Segment__APRMap.png"),
                            new LODTextureSet
                                (@"Roads\SmallRoads\OneWay1LParkingBicycle\Textures\Slope_SegmentLOD__MainTex.png",
                                 @"Roads\SmallRoads\OneWay1LParkingBicycle\Textures\Slope_SegmentLOD__APRMap.png",
                                 @"Roads\SmallRoads\OneWay1LParkingBicycle\Textures\Slope_SegmentLOD__XYSMap.png"));
                    }
                }
                for (int i = 0; i < info.m_nodes.Length; i++)
                {
                    if (info.m_nodes[i].m_mesh.name == "Slope_Node")
                    {
                        info.m_nodes[i].SetTextures(
                            new TextureSet
                                (@"Roads\SmallRoads\OneWay1LParkingBicycle\Textures\Slope_Segment__MainTex.png",
                                @"Roads\SmallRoads\OneWay1LParkingBicycle\Textures\Ground_Node__AlphaMap.png"),
                            new LODTextureSet
                                (@"Roads\SmallRoads\OneWay1LParkingBicycle\Textures\Ground_SegmentLOD__MainTex.png",
                                @"Roads\SmallRoads\OneWay1LParkingBicycle\Textures\Ground_SegmentLOD__AlphaMap.png",
                                @"Roads\SmallRoads\OneWay1LParkingBicycle\Textures\Ground_SegmentLOD__XYS.png"));
                    }
                    else
                    {
                        info.m_nodes[i].SetTextures(
                            new TextureSet
                                (@"Roads\SmallRoads\OneWay1LParkingBicycle\Textures\Tunnel_Segment__MainTex.png",
                                @"Roads\SmallRoads\OneWay1LParkingBicycle\Textures\Tunnel_Node__APRMap.png"),
                            new LODTextureSet
                                (@"Roads\SmallRoads\OneWay1LParkingBicycle\Textures\Tunnel_NodeLOD__MainTex.png",
                                @"Roads\SmallRoads\OneWay1LParkingBicycle\Textures\Tunnel_LOD__APRMap.png",
                                @"Roads\SmallRoads\OneWay1LParkingBicycle\Textures\Tunnel_LOD__XYSMap.png"));
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
                break;*/
                case NetInfoVersion.Tunnel:
                     {
                         info.SetAllSegmentsTexture(
                             new TextureSet
                                 (@"Roads\SmallRoads\OneWay1LParkingBicycle\Textures\Tunnel_Segment__MainTex.png",
                                 @"Roads\SmallRoads\OneWay1LParkingBicycle\Textures\Tunnel_Segment__APRMap.png"),
                             new LODTextureSet
                                 (@"Roads\SmallRoads\OneWay1LParkingBicycle\Textures\Tunnel_NodeLOD__MainTex.png",
                                 @"Roads\SmallRoads\OneWay1LParkingBicycle\Textures\Tunnel_LOD__APRMap.png",
                                 @"Roads\SmallRoads\OneWay1LParkingBicycle\Textures\Tunnel_LOD__XYSMap.png"));
                         info.SetAllNodesTexture(
                             new TextureSet
                                 (@"Roads\SmallRoads\OneWay1LParkingBicycle\Textures\Tunnel_Segment__MainTex.png",
                                 @"Roads\SmallRoads\OneWay1LParkingBicycle\Textures\Tunnel_Node__APRMap.png"),
                             new LODTextureSet
                                 (@"Roads\SmallRoads\OneWay1LParkingBicycle\Textures\Tunnel_NodeLOD__MainTex.png",
                                 @"Roads\SmallRoads\OneWay1LParkingBicycle\Textures\Tunnel_LOD__APRMap.png",
                                 @"Roads\SmallRoads\OneWay1LParkingBicycle\Textures\Tunnel_LOD__XYSMap.png"));
                         break;
                     }
                    /*  case NetInfoVersion.Tunnel:
                         {
                             info.SetAllSegmentsTexture(
                                 new TextureSet
                                     (@"Roads\SmallHeavyRoads\SmallAvenue4L\Textures\Tunnel_Segment__MainTex.png",
                                     @"Roads\SmallHeavyRoads\SmallAvenue4L\Textures\Tunnel_Segment__APRMap.png"),
                                 new LODTextureSet
                                     (@"Roads\SmallHeavyRoads\SmallAvenue4L\Textures\Tunnel_NodeLOD__MainTex.png",
                                     @"Roads\SmallHeavyRoads\SmallAvenue4L\Textures\Tunnel_LOD__APRMap.png",
                                     @"Roads\SmallHeavyRoads\SmallAvenue4L\Textures\Tunnel_LOD__XYSMap.png"));
                             info.SetAllNodesTexture(
                                 new TextureSet
                                     (@"Roads\SmallHeavyRoads\SmallAvenue4L\Textures\Tunnel_Segment__MainTex.png",
                                     @"Roads\SmallHeavyRoads\SmallAvenue4L\Textures\Tunnel_Node__APRMap.png"),
                                 new LODTextureSet
                                     (@"Roads\SmallHeavyRoads\SmallAvenue4L\Textures\Tunnel_NodeLOD__MainTex.png",
                                     @"Roads\SmallHeavyRoads\SmallAvenue4L\Textures\Tunnel_LOD__APRMap.png",
                                     @"Roads\SmallHeavyRoads\SmallAvenue4L\Textures\Tunnel_LOD__XYSMap.png"));
                             break;
                         }*/
            }
        }
    }
}

