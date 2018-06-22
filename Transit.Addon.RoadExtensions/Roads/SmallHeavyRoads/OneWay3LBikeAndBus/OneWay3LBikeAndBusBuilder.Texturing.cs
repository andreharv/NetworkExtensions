using Transit.Framework;
using Transit.Framework.Texturing;

namespace TransitPlus.Addon.RoadExtensions.Roads.SmallHeavyRoads.OneWay3LBikeAndBusAndBus
{
    public partial class OneWay3LBikeAndBusBuilder
    {
        private static void SetupTextures(NetInfo info, NetInfoVersion version)
        {
            switch (version)
            {
                case NetInfoVersion.Ground:
                    for (int i = 0; i < info.m_segments.Length; i++)
                    {
                       
                        var inverted = string.Empty;
                        if ((info.m_segments[i].m_backwardForbidden & NetSegment.Flags.Invert) != 0)
                        {
                            inverted = "_Inverted";
                        }
                            info.m_segments[i].SetTextures(
                            new TextureSet(
                                string.Format(@"Roads\SmallHeavyRoads\OneWay3LBikeAndBus\Textures\Ground_Segment{0}__MainTex.png", inverted),
                                string.Format(@"Roads\SmallHeavyRoads\OneWay3LBikeAndBus\Textures\Ground_Segment{0}__AlphaMap.png", inverted)),
                            new LODTextureSet(
                                string.Format(@"Roads\SmallHeavyRoads\OneWay3LBikeAndBus\Textures\Ground_SegmentLOD{0}__MainTex.png", inverted),
                                string.Format(@"Roads\SmallHeavyRoads\OneWay3LBikeAndBus\Textures\Ground_SegmentLOD{0}__AlphaMap.png", inverted),
                                @"Roads\SmallHeavyRoads\OneWay3LBikeAndBus\Textures\Ground_SegmentLOD__XYS.png"));
                    }
                    break;
                case NetInfoVersion.Elevated:
                case NetInfoVersion.Bridge:
                    info.SetAllSegmentsTexture(
                        new TextureSet
                            (@"Roads\SmallHeavyRoads\OneWay3LBikeAndBus\Textures\Elevated_MainTex.png",
                            @"Roads\SmallHeavyRoads\OneWay3LBikeAndBus\Textures\Elevated_Segment__APRMap.png"),
                        new LODTextureSet
                            (@"Roads\SmallHeavyRoads\OneWay3LBikeAndBus\Textures\Elevated_LOD__MainTex.png",
                            @"Roads\SmallHeavyRoads\OneWay3LBikeAndBus\Textures\Elevated_SegmentLOD__APRMap.png",
                            @"Roads\SmallHeavyRoads\OneWay3LBikeAndBus\Textures\Elevated_LOD__XYSMap.png"));
                    info.SetAllNodesTexture(
                        new TextureSet
                            (@"Roads\SmallHeavyRoads\OneWay3LBikeAndBus\Textures\Elevated_MainTex.png",
                            @"Roads\SmallHeavyRoads\OneWay3LBikeAndBus\Textures\Elevated_Node__APRMap.png"),
                        new LODTextureSet
                            (@"Roads\SmallHeavyRoads\OneWay3LBikeAndBus\Textures\Elevated_LOD__MainTex.png",
                            @"Roads\SmallHeavyRoads\OneWay3LBikeAndBus\Textures\Elevated_NodeLOD__APRMap.png",
                            @"Roads\SmallHeavyRoads\OneWay3LBikeAndBus\Textures\Elevated_LOD__XYSMap.png"));
                    break;
                case NetInfoVersion.Slope:
                    for (int i = 0; i < info.m_segments.Length; i++)
                    {
                        if (info.m_segments[i].m_mesh.name == "Slope")
                        {
                            info.m_segments[i].SetTextures(
                                new TextureSet
                                    (@"Roads\SmallHeavyRoads\OneWay3LBikeAndBus\Textures\Ground_Segment__MainTex.png",
                                    @"Roads\SmallHeavyRoads\OneWay3LBikeAndBus\Textures\Ground_Segment__AlphaMap.png"),
                                new LODTextureSet
                                    (@"Roads\SmallHeavyRoads\OneWay3LBikeAndBus\Textures\Ground_SegmentLOD__MainTex.png",
                                    @"Roads\SmallHeavyRoads\OneWay3LBikeAndBus\Textures\Ground_SegmentLOD__AlphaMap.png",
                                    @"Roads\SmallHeavyRoads\OneWay3LBikeAndBus\Textures\Slope_SegmentLOD2__XYSMap.png"));
                        }
                        else
                        {
                            info.m_segments[i].SetTextures(
                                new TextureSet
                                   (@"Roads\Highways\Highway4L\Textures\Slope_Segment__MainTex.png",
                                    @"Roads\SmallHeavyRoads\OneWay3LBikeAndBus\Textures\Slope_Segment__APRMap.png"),
                                new LODTextureSet
                                    (@"Roads\SmallHeavyRoads\OneWay3LBikeAndBus\Textures\Slope_SegmentLOD__MainTex.png",
                                     @"Roads\SmallHeavyRoads\OneWay3LBikeAndBus\Textures\Slope_SegmentLOD__APRMap.png",
                                     @"Roads\SmallHeavyRoads\OneWay3LBikeAndBus\Textures\Slope_SegmentLOD__XYSMap.png"));
                        }
                    }
                    for (int i = 0; i < info.m_nodes.Length; i++)
                    {
                        if (info.m_nodes[i].m_mesh.name == "Slope_Node")
                        {
                            info.m_nodes[i].SetTextures(
                                new TextureSet
                                    (@"Roads\SmallHeavyRoads\OneWay3LBikeAndBus\Textures\Ground_Segment__MainTex.png",
                                    @"Roads\SmallHeavyRoads\OneWay3LBikeAndBus\Textures\Ground_Node__AlphaMap.png"),
                                new LODTextureSet
                                    (@"Roads\SmallHeavyRoads\OneWay3LBikeAndBus\Textures\Ground_SegmentLOD__MainTex.png",
                                    @"Roads\SmallHeavyRoads\OneWay3LBikeAndBus\Textures\Ground_SegmentLOD__AlphaMap.png",
                                    @"Roads\SmallHeavyRoads\OneWay3LBikeAndBus\Textures\Ground_SegmentLOD__XYS.png"));
                        }
                        else
                        {
                            info.m_nodes[i].SetTextures(
                                new TextureSet
                                    (@"Roads\SmallHeavyRoads\OneWay3LBikeAndBus\Textures\Tunnel_Segment__MainTex.png",
                                    @"Roads\SmallHeavyRoads\OneWay3LBikeAndBus\Textures\Tunnel_Node__APRMap.png"),
                                new LODTextureSet
                                    (@"Roads\SmallHeavyRoads\OneWay3LBikeAndBus\Textures\Tunnel_NodeLOD__MainTex.png",
                                    @"Roads\SmallHeavyRoads\OneWay3LBikeAndBus\Textures\Tunnel_LOD__APRMap.png",
                                    @"Roads\SmallHeavyRoads\OneWay3LBikeAndBus\Textures\Tunnel_LOD__XYSMap.png"));
                        }
                    }
            
                    break;
                case NetInfoVersion.Tunnel:
                    {
                        info.SetAllSegmentsTexture(
                            new TextureSet
                                (@"Roads\SmallHeavyRoads\OneWay3LBikeAndBus\Textures\Tunnel_Segment__MainTex.png",
                                @"Roads\SmallHeavyRoads\OneWay3LBikeAndBus\Textures\Tunnel_Segment__APRMap.png"),
                            new LODTextureSet
                                (@"Roads\SmallHeavyRoads\OneWay3LBikeAndBus\Textures\Tunnel_NodeLOD__MainTex.png",
                                @"Roads\SmallHeavyRoads\OneWay3LBikeAndBus\Textures\Tunnel_LOD__APRMap.png",
                                @"Roads\SmallHeavyRoads\OneWay3LBikeAndBus\Textures\Tunnel_LOD__XYSMap.png"));
                        info.SetAllNodesTexture(
                            new TextureSet
                                (@"Roads\SmallHeavyRoads\OneWay3LBikeAndBus\Textures\Tunnel_Segment__MainTex.png",
                                @"Roads\SmallHeavyRoads\OneWay3LBikeAndBus\Textures\Tunnel_Node__APRMap.png"),
                            new LODTextureSet
                                (@"Roads\SmallHeavyRoads\OneWay3LBikeAndBus\Textures\Tunnel_NodeLOD__MainTex.png",
                                @"Roads\SmallHeavyRoads\OneWay3LBikeAndBus\Textures\Tunnel_LOD__APRMap.png",
                                @"Roads\SmallHeavyRoads\OneWay3LBikeAndBus\Textures\Tunnel_LOD__XYSMap.png"));
                        break;
                    }
            }
        }
    }
}

