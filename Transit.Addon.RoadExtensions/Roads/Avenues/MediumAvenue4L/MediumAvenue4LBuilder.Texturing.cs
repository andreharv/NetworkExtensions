using Transit.Framework;
using Transit.Framework.Texturing;

namespace Transit.Addon.RoadExtensions.Roads.Avenues.MediumAvenue4L
{
    public partial class MediumAvenue4LBuilder
    {
        private static void SetupTextures(NetInfo info, NetInfoVersion version)
        {
            switch (version)
            {
                case NetInfoVersion.Ground:
                    for (int i = 0; i < info.m_segments.Length; i++)
                    {
                        if (info.m_segments[i].m_mesh.name.ToLower().Contains("ground_p"))
                        {
                            info.m_segments[i].SetTextures(
                            new TextureSet
                                (@"Roads\SmallRoads\BasicRoadMdn\Textures\Ground_Segment_Median__MainTex.png",
                                $@"Roads\SmallRoads\BasicRoadMdn\Textures\Ground_Segment_MedianConcrete2__AlphaMap.png"),
                            new LODTextureSet
                                (@"Roads\SmallRoads\BasicRoadMdn\Textures\Ground_Segment_Median_LOD__MainTex.png",
                                $@"Roads\SmallRoads\BasicRoadMdn\Textures\Ground_Segment_MedianConcrete2_LOD__AlphaMap.png",
                                @"Roads\SmallRoads\BasicRoadMdn\Textures\Ground_Segment_Median_LOD__XYSMap.png"));
                        }
                        else
                        {
                            info.m_segments[i].SetTextures(
                        new TextureSet
                            (@"Roads\Avenues\MediumAvenue4L\Textures\Ground_Segment__MainTex.png",
                            @"Roads\Avenues\MediumAvenue4L\Textures\Ground_Segment__APRMap.png"),
                        new LODTextureSet
                            (@"Roads\Avenues\MediumAvenue4L\Textures\Ground_SegmentLOD__MainTex.png",
                            @"Roads\Avenues\MediumAvenue4L\Textures\Ground_SegmentLOD__APRMap.png",
                            @"Roads\Avenues\MediumAvenue4L\Textures\Ground_SegmentLOD__XYSMap.png"));
                        }
                    }
                    for (int i = 0; i < info.m_nodes.Length; i++)
                    {
                        if (info.m_nodes[i].m_mesh.name.ToLower().Contains("ground_p") || info.m_nodes[i].m_mesh.name.Contains("MedCon"))
                        {
                            info.m_nodes[i].SetTextures(
                            new TextureSet
                                (@"Roads\SmallRoads\BasicRoadMdn\Textures\Ground_Segment_Median__MainTex.png",
                                $@"Roads\SmallRoads\BasicRoadMdn\Textures\Ground_Segment_MedianConcrete2__AlphaMap.png"),
                            new LODTextureSet
                                (@"Roads\SmallRoads\BasicRoadMdn\Textures\Ground_Segment_Median_LOD__MainTex.png",
                                $@"Roads\SmallRoads\BasicRoadMdn\Textures\Ground_Segment_MedianConcrete2_LOD__AlphaMap.png",
                                @"Roads\SmallRoads\BasicRoadMdn\Textures\Ground_Segment_Median_LOD__XYSMap.png"));
                        }
                    }
                    break;
                case NetInfoVersion.GroundGrass:
                case NetInfoVersion.GroundTrees:
                case NetInfoVersion.GroundPavement:
                    var suffix = version == NetInfoVersion.GroundPavement ? "Concrete2" : "Grass";
                    for (int i = 0; i < info.m_segments.Length; i++)
                    {
                        if (info.m_segments[i].m_mesh.name.ToLower().Contains("ground_p"))
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
                    }
                    for (int i = 0; i < info.m_nodes.Length; i++)
                    {
                        if (info.m_nodes[i].m_mesh.name.ToLower().Contains("ground_p") || info.m_nodes[i].m_mesh.name.Contains("MedCon"))
                        {
                            info.m_nodes[i].SetTextures(
                            new TextureSet
                                (@"Roads\SmallRoads\BasicRoadMdn\Textures\Ground_Segment_Median__MainTex.png",
                                $@"Roads\SmallRoads\BasicRoadMdn\Textures\Ground_Segment_MedianConcrete2__AlphaMap.png"),
                            new LODTextureSet
                                (@"Roads\SmallRoads\BasicRoadMdn\Textures\Ground_Segment_Median_LOD__MainTex.png",
                                $@"Roads\SmallRoads\BasicRoadMdn\Textures\Ground_Segment_MedianConcrete2_LOD__AlphaMap.png",
                                @"Roads\SmallRoads\BasicRoadMdn\Textures\Ground_Segment_Median_LOD__XYSMap.png"));
                        }
                    }
                    break;
                case NetInfoVersion.Elevated:
                case NetInfoVersion.Bridge:
                    info.SetAllSegmentsTexture(
                        new TextureSet
                            (@"Roads\Avenues\MediumAvenue4L\Textures\Elevated_Segment__MainTex.png",
                            @"Roads\Avenues\MediumAvenue4L\Textures\Elevated_Segment__APRMap.png"),
                        new LODTextureSet
                            (@"Roads\Avenues\MediumAvenue4L\Textures\Elevated_SegmentLOD__MainTex.png",
                            @"Roads\Avenues\MediumAvenue4L\Textures\Elevated_SegmentLOD__APRMap.png",
                            @"Roads\Avenues\MediumAvenue4L\Textures\Elevated_SegmentLOD__XYSMap.png"));
                    break;
                //info.SetAllNodesTexture(
                //    new TextureSet
                //        (@"Roads\Avenues\MediumAvenue4L\Textures\Elevated_Segment__MainTex.png",
                //        @"Roads\Avenues\MediumAvenue4L\Textures\Elevated_Node__APRMap.png"),
                //    new LODTextureSet
                //        (@"Roads\Avenues\MediumAvenue4L\Textures\Elevated_SegmentLOD__MainTex.png",
                //        @"Roads\Avenues\MediumAvenue4L\Textures\Elevated_NodeLOD__APRMap.png",
                //        @"Roads\Avenues\MediumAvenue4L\Textures\Elevated_SegmentLOD__XYSMap.png"));

                case NetInfoVersion.Slope:
                    info.SetAllSegmentsTexture(
                        new TextureSet
                            (@"Roads\Avenues\MediumAvenue4L\Textures\Slope_Segment__MainTex.png",
                            @"Roads\Avenues\MediumAvenue4L\Textures\Slope_Segment__APRMap.png"),
                        new LODTextureSet
                            (@"Roads\Avenues\MediumAvenue4L\Textures\Slope_SegmentLOD__MainTex.png",
                            @"Roads\Avenues\MediumAvenue4L\Textures\Slope_SegmentLOD__APRMap.png",
                            @"Roads\Avenues\MediumAvenue4L\Textures\Slope_SegmentLOD__XYSMap.png"));
                    foreach (var node in info.m_nodes)
                    {
                        if (node.m_mesh.name == "Slope_U_Node")
                        {
                            node.SetTextures(
                                new TextureSet
                                    (@"Roads\Avenues\MediumAvenue4L\Textures\Tunnel_Segment__MainTex.png",
                                    @"Roads\Avenues\MediumAvenue4L\Textures\Tunnel_Node__APRMap.png"),
                                new LODTextureSet
                                    (@"Roads\Avenues\MediumAvenue4L\Textures\Tunnel_SegmentLOD__MainTex.png",
                                    @"Roads\Avenues\MediumAvenue4L\Textures\Tunnel_SegmentLOD__APRMap.png",
                                    @"Roads\Avenues\MediumAvenue4L\Textures\Tunnel_SegmentLOD__XYSMap.png"));
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
                                (@"Roads\Avenues\MediumAvenue4L\Textures\Tunnel_Segment__MainTex.png",
                                @"Roads\Avenues\MediumAvenue4L\Textures\Tunnel_Segment__APRMap.png"),
                            new LODTextureSet
                                (@"Roads\Avenues\MediumAvenue4L\Textures\Tunnel_SegmentLOD__MainTex.png",
                                @"Roads\Avenues\MediumAvenue4L\Textures\Tunnel_SegmentLOD__APRMap.png",
                                @"Roads\Avenues\MediumAvenue4L\Textures\Tunnel_SegmentLOD__XYSMap.png"));
                        info.SetAllNodesTexture(
                            new TextureSet
                                (@"Roads\Avenues\MediumAvenue4L\Textures\Tunnel_Segment__MainTex.png",
                                @"Roads\Avenues\MediumAvenue4L\Textures\Tunnel_Node__APRMap.png"),
                            new LODTextureSet
                                (@"Roads\Avenues\MediumAvenue4L\Textures\Tunnel_SegmentLOD__MainTex.png",
                                @"Roads\Avenues\MediumAvenue4L\Textures\Tunnel_SegmentLOD__APRMap.png",
                                @"Roads\Avenues\MediumAvenue4L\Textures\Tunnel_SegmentLOD__XYSMap.png"));
                        break;
                    }
            }
        }
    }
}

