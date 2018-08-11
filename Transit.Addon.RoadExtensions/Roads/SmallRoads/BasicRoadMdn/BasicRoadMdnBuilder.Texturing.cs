using Transit.Framework;
using Transit.Framework.Texturing;

namespace Transit.Addon.RoadExtensions.Roads.SmallRoads.BasicRoadMdn
{
    public partial class BasicRoadMdnBuilder
    {
        private static void SetupTextures(NetInfo info, NetInfoVersion version)
        {
            switch (version)
            {
                case NetInfoVersion.Ground:
                case NetInfoVersion.GroundGrass:
                case NetInfoVersion.GroundTrees:
                    var suffix = version.ToString().Substring(6).Length > 0 ? "Grass" : "Concrete";
                    for (int i = 0; i < info.m_segments.Length; i++)
                    {
                        if (info.m_segments[i].m_mesh.name.ToLower().Contains("median"))
                        {
                            info.m_segments[i].SetTextures(
                            new TextureSet
                                (@"Roads\SmallRoads\BasicRoadMdn\Textures\Ground_Segment_Median__MainTex.png",
                                $@"Roads\SmallRoads\BasicRoadMdn\Textures\Ground_Segment_Median{suffix}__AlphaMap.png"),
                            new LODTextureSet
                                (@"Roads\SmallRoads\BasicRoadMdn\Textures\Ground_Segment_Median_LOD__MainTex.png",
                                $@"Roads\SmallRoads\BasicRoadMdn\Textures\Ground_Segment_Median{suffix}_LOD__AlphaMap.png",
                                @"Roads\SmallRoads\BasicRoadMdn\Textures\Ground_Segment_Median_LOD__XYSMap.png"));
                        }
                        else
                        {
                            info.m_segments[i].SetTextures(
                                new TextureSet
                                    (@"Roads\SmallHeavyRoads\BasicRoadTL\Textures\Ground_Segment__MainTex.png",
                                    @"Roads\SmallHeavyRoads\BasicRoadTL\Textures\Ground_Segment__AlphaMap.png"),
                                new LODTextureSet
                                    (@"Roads\SmallHeavyRoads\BasicRoadTL\Textures\Ground_SegmentLOD__MainTex.png",
                                    @"Roads\SmallHeavyRoads\BasicRoadTL\Textures\Ground_SegmentLOD__AlphaMap.png",
                                    @"Roads\SmallHeavyRoads\BasicRoadTL\Textures\Ground_SegmentLOD__XYS.png"));
                        }
                    }
                    for (int i = 0; i < info.m_nodes.Length; i++)
                    {
                        if (info.m_nodes[i].m_mesh.name.ToLower().Contains("median"))
                        {
                            info.m_nodes[i].SetTextures(
                                new TextureSet
                                    (@"Roads\SmallRoads\BasicRoadMdn\Textures\Ground_Segment_Median__MainTex.png",
                                    @"Roads\SmallRoads\BasicRoadMdn\Textures\Ground_Segment_MedianConcrete__AlphaMap.png"));
                        }
                    }
                    break;
                case NetInfoVersion.Elevated:
                case NetInfoVersion.Bridge:
                    for (int i = 0; i < info.m_segments.Length; i++)
                    {
                        if (info.m_segments[i].m_mesh.name.ToLower().Contains("median"))
                        {
                            info.m_segments[i].SetTextures(
                            new TextureSet
                                (@"Roads\SmallRoads\BasicRoadMdn\Textures\Ground_Segment_Median__MainTex.png",
                                @"Roads\SmallRoads\BasicRoadMdn\Textures\Ground_Segment_MedianConcrete__AlphaMap.png"),
                            new LODTextureSet
                                (@"Roads\SmallRoads\BasicRoadMdn\Textures\Ground_Segment_Median_LOD__MainTex.png",
                                @"Roads\SmallRoads\BasicRoadMdn\Textures\Ground_Segment_MedianConcrete_LOD__AlphaMap.png",
                                @"Roads\SmallRoads\BasicRoadMdn\Textures\Ground_Segment_Median_LOD__XYSMap.png"));
            }
                        else
                        {
                            info.m_segments[i].SetTextures(
                        new TextureSet
                            (@"Roads\SmallHeavyRoads\BasicRoadTL\Textures\Elevated_MainTex.png",
                            @"Roads\SmallHeavyRoads\BasicRoadTL\Textures\Elevated_Segment__APRMap.png"),
                        new LODTextureSet
                            (@"Roads\SmallHeavyRoads\BasicRoadTL\Textures\Elevated_LOD__MainTex.png",
                            @"Roads\SmallHeavyRoads\BasicRoadTL\Textures\Elevated_SegmentLOD__APRMap.png",
                            @"Roads\SmallHeavyRoads\BasicRoadTL\Textures\Elevated_LOD__XYSMap.png"));
                        }
                    }
                    for (int i = 0; i < info.m_nodes.Length; i++)
                    {
                        if (info.m_nodes[i].m_mesh.name.ToLower().Contains("median"))
                        {
                            info.m_nodes[i].SetTextures(
                            new TextureSet
                                (@"Roads\SmallRoads\BasicRoadMdn\Textures\Ground_Segment_Median__MainTex.png",
                                @"Roads\SmallRoads\BasicRoadMdn\Textures\Ground_Segment_MedianConcrete__AlphaMap.png"));
                        }
                        else
                        {
                            info.m_nodes[i].SetTextures(
                                new TextureSet
                                    (@"Roads\SmallHeavyRoads\BasicRoadTL\Textures\Elevated_MainTex.png",
                                    @"Roads\SmallHeavyRoads\BasicRoadTL\Textures\Elevated_Node__APRMap.png"),
                                new LODTextureSet
                                    (@"Roads\SmallHeavyRoads\BasicRoadTL\Textures\Elevated_LOD__MainTex.png",
                                    @"Roads\SmallHeavyRoads\BasicRoadTL\Textures\Elevated_NodeLOD__APRMap.png",
                                    @"Roads\SmallHeavyRoads\BasicRoadTL\Textures\Elevated_LOD__XYSMap.png"));
                        }
                    }
                    break;
                case NetInfoVersion.Slope:
                    for (int i = 0; i < info.m_segments.Length; i++)
                    {
                        if (info.m_segments[i].m_mesh.name == "Slope")
                        {
                            info.m_segments[i].SetTextures(
                                new TextureSet
                                    (@"Roads\SmallHeavyRoads\BasicRoadTL\Textures\Ground_Segment__MainTex.png",
                                    @"Roads\SmallHeavyRoads\BasicRoadTL\Textures\Ground_Segment__AlphaMap.png"),
                                new LODTextureSet
                                    (@"Roads\SmallHeavyRoads\BasicRoadTL\Textures\Ground_SegmentLOD__MainTex.png",
                                    @"Roads\SmallHeavyRoads\BasicRoadTL\Textures\Ground_SegmentLOD__AlphaMap.png",
                                    @"Roads\SmallHeavyRoads\BasicRoadTL\Textures\Slope_SegmentLOD2__XYSMap.png"));
                        }
                        else if (info.m_segments[i].m_mesh.name.ToLower().Contains("median"))
                        {
                            info.m_segments[i].SetTextures(
                                new TextureSet
                                    (@"Roads\SmallRoads\BasicRoadMdn\Textures\Ground_Segment_Median__MainTex.png",
                                    @"Roads\SmallRoads\BasicRoadMdn\Textures\Ground_Segment_MedianConcrete__AlphaMap.png"),
                                new LODTextureSet
                                    (@"Roads\SmallRoads\BasicRoadMdn\Textures\Ground_Segment_Median_LOD__MainTex.png",
                                    @"Roads\SmallRoads\BasicRoadMdn\Textures\Ground_Segment_MedianConcrete_LOD__AlphaMap.png",
                                    @"Roads\SmallRoads\BasicRoadMdn\Textures\Ground_Segment_Median_LOD__XYSMap.png"));
            }
                        else
                        {
                            info.m_segments[i].SetTextures(
                                new TextureSet
                                   (@"Roads\Highways\Highway4L\Textures\Slope_Segment__MainTex.png",
                                    @"Roads\SmallHeavyRoads\BasicRoadTL\Textures\Slope_Segment__APRMap.png"),
                                new LODTextureSet
                                    (@"Roads\SmallHeavyRoads\BasicRoadTL\Textures\Slope_SegmentLOD__MainTex.png",
                                     @"Roads\SmallHeavyRoads\BasicRoadTL\Textures\Slope_SegmentLOD__APRMap.png",
                                     @"Roads\SmallHeavyRoads\BasicRoadTL\Textures\Slope_SegmentLOD__XYSMap.png"));
                        }
                    }
                    for (int i = 0; i < info.m_nodes.Length; i++)
                    {
                        if (info.m_nodes[i].m_mesh.name == "Slope_Node")
                        {
                            info.m_nodes[i].SetTextures(
                                new TextureSet
                                    (@"Roads\SmallHeavyRoads\BasicRoadTL\Textures\Ground_Segment__MainTex.png",
                                    @"Roads\SmallHeavyRoads\BasicRoadTL\Textures\Ground_Node__AlphaMap.png"),
                                new LODTextureSet
                                    (@"Roads\SmallHeavyRoads\BasicRoadTL\Textures\Ground_SegmentLOD__MainTex.png",
                                    @"Roads\SmallHeavyRoads\BasicRoadTL\Textures\Ground_SegmentLOD__AlphaMap.png",
                                    @"Roads\SmallHeavyRoads\BasicRoadTL\Textures\Ground_SegmentLOD__XYS.png"));
                        }
                        else if (info.m_nodes[i].m_mesh.name.ToLower().Contains("median"))
                        {
                            info.m_nodes[i].SetTextures(
                            new TextureSet
                                (@"Roads\SmallRoads\BasicRoadMdn\Textures\Ground_Segment_Median__MainTex.png",
                                @"Roads\SmallRoads\BasicRoadMdn\Textures\Ground_Segment_MedianConcrete__AlphaMap.png"));
                        }
                        else
                        {
                            info.m_nodes[i].SetTextures(
                                new TextureSet
                                    (@"Roads\SmallHeavyRoads\BasicRoadTL\Textures\Tunnel_Segment__MainTex.png",
                                    @"Roads\SmallHeavyRoads\BasicRoadTL\Textures\Tunnel_Node__APRMap.png"),
                                new LODTextureSet
                                    (@"Roads\SmallHeavyRoads\BasicRoadTL\Textures\Tunnel_NodeLOD__MainTex.png",
                                    @"Roads\SmallHeavyRoads\BasicRoadTL\Textures\Tunnel_LOD__APRMap.png",
                                    @"Roads\SmallHeavyRoads\BasicRoadTL\Textures\Tunnel_LOD__XYSMap.png"));
                        }
                    }
                    break;
                case NetInfoVersion.Tunnel:
                    {
                        for (int i = 0; i < info.m_segments.Length; i++)
                        {
                            if (info.m_segments[i].m_mesh.name.ToLower().Contains("median"))
                            {
                                info.m_segments[i].SetTextures(
                                new TextureSet
                                    (@"Roads\SmallRoads\BasicRoadMdn\Textures\Ground_Segment_Median__MainTex.png",
                                    @"Roads\SmallRoads\BasicRoadMdn\Textures\Ground_Segment_MedianConcrete__AlphaMap.png"),
                                new LODTextureSet
                                    (@"Roads\SmallRoads\BasicRoadMdn\Textures\Ground_Segment_Median_LOD__MainTex.png",
                                    @"Roads\SmallRoads\BasicRoadMdn\Textures\Ground_Segment_MedianConcrete_LOD__AlphaMap.png",
                                    @"Roads\SmallRoads\BasicRoadMdn\Textures\Ground_Segment_Median_LOD__XYSMap.png"));
                            }
                            else
                            {
                                info.m_segments[i].SetTextures(
                                new TextureSet
                                    (@"Roads\SmallHeavyRoads\BasicRoadTL\Textures\Tunnel_Segment__MainTex.png",
                                    @"Roads\SmallHeavyRoads\BasicRoadTL\Textures\Tunnel_Segment__APRMap.png"),
                                new LODTextureSet
                                    (@"Roads\SmallHeavyRoads\BasicRoadTL\Textures\Tunnel_NodeLOD__MainTex.png",
                                    @"Roads\SmallHeavyRoads\BasicRoadTL\Textures\Tunnel_LOD__APRMap.png",
                                    @"Roads\SmallHeavyRoads\BasicRoadTL\Textures\Tunnel_LOD__XYSMap.png"));
                            }
                        }
                        for (int i = 0; i < info.m_nodes.Length; i++)
                        {
                            if (info.m_nodes[i].m_mesh.name.ToLower().Contains("median"))
                            {
                                info.m_nodes[i].SetTextures(
                                new TextureSet
                                    (@"Roads\SmallRoads\BasicRoadMdn\Textures\Ground_Segment_Median__MainTex.png",
                                    @"Roads\SmallRoads\BasicRoadMdn\Textures\Ground_Segment_MedianConcrete__AlphaMap.png"));
                            }
                            else
                            {
                                info.m_nodes[i].SetTextures(
                            new TextureSet
                                (@"Roads\SmallHeavyRoads\BasicRoadTL\Textures\Tunnel_Segment__MainTex.png",
                                @"Roads\SmallHeavyRoads\BasicRoadTL\Textures\Tunnel_Node__APRMap.png"),
                            new LODTextureSet
                                (@"Roads\SmallHeavyRoads\BasicRoadTL\Textures\Tunnel_NodeLOD__MainTex.png",
                                @"Roads\SmallHeavyRoads\BasicRoadTL\Textures\Tunnel_LOD__APRMap.png",
                                @"Roads\SmallHeavyRoads\BasicRoadTL\Textures\Tunnel_LOD__XYSMap.png"));
                            }
                        }
                        break;
                    }
            }
        }
    }
}

