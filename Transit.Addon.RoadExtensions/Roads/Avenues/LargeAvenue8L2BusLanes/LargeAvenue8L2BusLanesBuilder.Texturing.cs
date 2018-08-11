using Transit.Framework;
using Transit.Framework.Texturing;

namespace Transit.Addon.RoadExtensions.Roads.Avenues.LargeAvenue8L2BusLanes
{
    public partial class LargeAvenue8L2BusLanesBuilder
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
                            var isPavement = (info.m_segments[i].m_mesh.name.Contains("Pavement") && info.m_segments[i].m_mesh.name.Contains("Bus") == false) ? "_Pavement" : "";
                            info.m_segments[i].SetTextures(
                                new TextureSet
                                    ($@"Roads\Avenues\LargeAvenue8L2BusLanes\Textures\Ground_Segment{isPavement}__MainTex.png",
                                    $@"Roads\Avenues\LargeAvenue8L2BusLanes\Textures\Ground_Segment{isPavement}__APRMap.png"),
                                new LODTextureSet
                                    (@"Roads\Avenues\LargeAvenue8L2BusLanes\Textures\Ground_Segment_LOD__MainTex.png",
                                    @"Roads\Avenues\LargeAvenue8L2BusLanes\Textures\Ground_Segment_LOD__APRMap.png",
                                    @"Roads\Avenues\LargeAvenue8L2BusLanes\Textures\Ground_Segment_LOD__XYSMap.png"));
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
                                (@"Roads\Avenues\LargeAvenue8L2BusLanes\Textures\Ground_Node__MainTex.png",
                                @"Roads\Avenues\LargeAvenue8L2BusLanes\Textures\Ground_Node__APRMap.png"),
                            new LODTextureSet
                                (@"Roads\Avenues\LargeAvenue8L2BusLanes\Textures\Ground_Node_LOD__MainTex.png",
                                @"Roads\Avenues\LargeAvenue8L2BusLanes\Textures\Ground_Node_LOD__APRMap.png",
                                @"Roads\Avenues\LargeAvenue8L2BusLanes\Textures\Ground_Segment_LOD__XYSMap.png"));
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
                            (@"Roads\Avenues\LargeAvenue8L2BusLanes\Textures\Elevated_Segment__MainTex.png",
                            @"Roads\Avenues\LargeAvenue8L2BusLanes\Textures\Elevated_Segment__APRMap.png"),
                        new LODTextureSet
                            (@"Roads\Avenues\LargeAvenue8L2BusLanes\Textures\Elevated_Segment_LOD__MainTex.png",
                            @"Roads\Avenues\LargeAvenue8L2BusLanes\Textures\Elevated_Segment_LOD__APRMap.png",
                            @"Roads\Avenues\LargeAvenue8L2BusLanes\Textures\Elevated_Segment_LOD__XYSMap.png"));
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
                                    (@"Roads\Avenues\LargeAvenue8L2BusLanes\Textures\Elevated_Node__MainTex.png",
                                    @"Roads\Avenues\LargeAvenue8L2BusLanes\Textures\Elevated_Node__APRMap.png"),
                                new LODTextureSet
                                    (@"Roads\Avenues\LargeAvenue8L2BusLanes\Textures\Elevated_Segment_LOD__MainTex.png",
                                    @"Roads\Avenues\LargeAvenue8L2BusLanes\Textures\Elevated_Node_LOD__APRMap.png",
                                    @"Roads\Avenues\LargeAvenue8L2BusLanes\Textures\Elevated_Segment_LOD__XYSMap.png"));
                        }
                    }
                    break;
                case NetInfoVersion.Slope:
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
                        else if (info.m_segments[i].m_mesh.name.Contains("tunnel-slope"))
                        {
                            info.m_segments[i].SetTextures(
                        new TextureSet
                            (@"Roads\Avenues\LargeAvenue8L2BusLanes\Textures\Slope_Segment__MainTex.png",
                            @"Roads\Avenues\LargeAvenue8L2BusLanes\Textures\Slope_Segment_Cover__APRMap.png"),
                        new LODTextureSet
                            (@"Roads\Avenues\LargeAvenue8L2BusLanes\Textures\Slope_Segment_LOD__MainTex.png",
                            @"Roads\Avenues\LargeAvenue8L2BusLanes\Textures\Slope_Segment_LOD__APRMap.png",
                            @"Roads\Avenues\LargeAvenue8L2BusLanes\Textures\Slope_Segment_LOD__XYSMap.png"));
                        }
                        else
                        {
                            info.m_segments[i].SetTextures(
                                new TextureSet
                                    (@"Roads\Avenues\LargeAvenue8L2BusLanes\Textures\Slope_Segment__MainTex.png",
                                    @"Roads\Avenues\LargeAvenue8L2BusLanes\Textures\Slope_Segment__APRMap.png"),
                                new LODTextureSet
                                    (@"Roads\Avenues\LargeAvenue8L2BusLanes\Textures\Slope_Segment_LOD__MainTex.png",
                                    @"Roads\Avenues\LargeAvenue8L2BusLanes\Textures\Slope_Segment_LOD__APRMap.png",
                                    @"Roads\Avenues\LargeAvenue8L2BusLanes\Textures\Slope_Segment_LOD__XYSMap.png"));
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
                        else if (info.m_nodes[i].m_mesh.name == "Slope_U_Node")
                        {
                            info.m_nodes[i].SetTextures(
                                new TextureSet
                                    (@"Roads\Avenues\LargeAvenue8L2BusLanes\Textures\Slope_Segment__MainTex.png",
                                    @"Roads\Avenues\LargeAvenue8L2BusLanes\Textures\Tunnel_Node__APRMap.png"),
                                new LODTextureSet
                                    (@"Roads\Avenues\LargeAvenue8L2BusLanes\Textures\Slope_Segment_LOD__MainTex.png",
                                    @"Roads\Avenues\LargeAvenue8L2BusLanes\Textures\Slope_Segment_LOD__APRMap.png",
                                    @"Roads\Avenues\LargeAvenue8L2BusLanes\Textures\Slope_Segment_LOD__XYSMap.png"));
                        }
                        else
                        {
                            info.m_nodes[i].SetTextures(
                                new TextureSet
                                    (@"Roads\Avenues\LargeAvenue8L2BusLanes\Textures\Ground_Node__MainTex.png",
                                    @"Roads\Avenues\LargeAvenue8L2BusLanes\Textures\Ground_Node__APRMap.png"),
                                new LODTextureSet
                                    (@"Roads\Avenues\LargeAvenue8L2BusLanes\Textures\Ground_Node_LOD__MainTex.png",
                                    @"Roads\Avenues\LargeAvenue8L2BusLanes\Textures\Ground_Node_LOD__APRMap.png",
                                    @"Roads\Avenues\LargeAvenue8L2BusLanes\Textures\Slope_Segment_LOD__XYSMap.png"));
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
                                    @"Roads\SmallRoads\BasicRoadMdn\Textures\Ground_Segment_MedianConcrete__AlphaMap.png"));
                            }
                            else
                            {
                                info.m_segments[i].SetTextures(
                                new TextureSet
                                    (@"Roads\Avenues\LargeAvenue8L2BusLanes\Textures\Tunnel_Segment__MainTex.png",
                                    @"Roads\Avenues\LargeAvenue8L2BusLanes\Textures\Tunnel_Segment__APRMap.png"),
                                new LODTextureSet
                                    (@"Roads\Avenues\LargeAvenue8L2BusLanes\Textures\Tunnel_Segment_LOD__MainTex.png",
                                    @"Roads\Avenues\LargeAvenue8L2BusLanes\Textures\Tunnel_Segment_LOD__APRMap.png",
                                    @"Roads\Avenues\LargeAvenue8L2BusLanes\Textures\Tunnel_Segment_LOD__XYSMap.png"));
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
                                (@"Roads\Avenues\LargeAvenue8L2BusLanes\Textures\Tunnel_Segment__MainTex.png",
                                @"Roads\Avenues\LargeAvenue8L2BusLanes\Textures\Tunnel_Node__APRMap.png"),
                            new LODTextureSet
                                (@"Roads\Avenues\LargeAvenue8L2BusLanes\Textures\Tunnel_Segment_LOD__MainTex.png",
                                @"Roads\Avenues\LargeAvenue8L2BusLanes\Textures\Tunnel_Segment_LOD__APRMap.png",
                                @"Roads\Avenues\LargeAvenue8L2BusLanes\Textures\Tunnel_Segment_LOD__XYSMap.png"));
                            }
                        }


                        break;
                    }
            }
        }
    }
}

