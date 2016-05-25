using Transit.Framework;
using Transit.Framework.Texturing;

namespace Transit.Addon.RoadExtensions.Roads.PedestrianRoads.Stone
{
    public partial class ZonablePedestrianStoneRoadBuilder
    {
        private static void SetupTextures(NetInfo info, NetInfoVersion version)
        {
            switch (version)
            {
                case NetInfoVersion.Ground:
                    info.SetAllSegmentsTexture(
                        new TextureSet
                           (@"Roads\PedestrianRoads\Stone\Textures\Ground_Segment__MainTex.png",
                            @"Roads\PedestrianRoads\Stone\Textures\Ground_Segment__AlphaMap.png",
                            @"Roads\PedestrianRoads\Stone\Textures\Ground_Segment__XYSMap.png"),
                        new LODTextureSet
                            (@"Roads\PedestrianRoads\Stone\Textures\Ground_Segment_LOD__MainTex.png",
                            @"Roads\PedestrianRoads\Stone\Textures\Ground_Segment_LOD__AlphaMap.png",
                            @"Roads\PedestrianRoads\Stone\Textures\Ground_Segment_LOD__XYSMap.png"));

                    for (int i = 0; i < info.m_nodes.Length; i++)
                    {
                        if (info.m_nodes[i].m_flagsRequired == NetNode.Flags.Transition)
                        {
                            info.m_nodes[i].SetTextures(
                                new TextureSet
                                    (@"Roads\PedestrianRoads\Stone\Textures\Ground_Trans__MainTex.png",
                                     @"Roads\PedestrianRoads\Stone\Textures\Ground_Trans__AlphaMap.png",
                                     @"Roads\PedestrianRoads\Stone\Textures\Ground_Node__XYSMap.png"),
                                new LODTextureSet
                                    (@"Roads\PedestrianRoads\Stone\Textures\Ground_Node_LOD__MainTex.png",
                                    @"Roads\PedestrianRoads\Stone\Textures\Ground_Trans_LOD__AlphaMap.png",
                                    @"Roads\PedestrianRoads\Common\Textures\Ground_Node_LOD__XYSMap.png"));
                        }
                        else
                        {
                            info.m_nodes[i].SetTextures(
                                new TextureSet
                                    (@"Roads\PedestrianRoads\Stone\Textures\Ground_Node__MainTex.png",
                                    @"Roads\PedestrianRoads\Stone\Textures\Ground_Node__AlphaMap.png",
                                    @"Roads\PedestrianRoads\Stone\Textures\Ground_Node__XYSMap.png"),
                                new LODTextureSet
                                    (@"Roads\PedestrianRoads\Stone\Textures\Ground_Node_LOD__MainTex.png",
                                    @"Roads\PedestrianRoads\Stone\Textures\Ground_Node_LOD__AlphaMap.png",
                                    @"Roads\PedestrianRoads\Common\Textures\Ground_Node_LOD__XYSMap.png"));
                        }
                    }
                    break;
                case NetInfoVersion.Elevated:
                    info.SetAllSegmentsTexture(
                        new TextureSet
                           (@"Roads\PedestrianRoads\Stone\Textures\Elevated_Segment__MainTex.png",
                            @"Roads\PedestrianRoads\Stone\Textures\Elevated_Segment__AlphaMap.png",
                            @"Roads\PedestrianRoads\Stone\Textures\Elevated_Segment__XYSMap.png"),
                        new LODTextureSet
                            (@"Roads\PedestrianRoads\Stone\Textures\Elevated_Segment_LOD__MainTex.png",
                            @"Roads\PedestrianRoads\Stone\Textures\Elevated_Segment_LOD__AlphaMap.png",
                            @"Roads\PedestrianRoads\Stone\Textures\Elevated_Segment_LOD__XYSMap.png"));

                    for (int i = 0; i < info.m_nodes.Length; i++)
                    {
                        if (info.m_nodes[i].m_mesh.name.ToLower().Contains("trans"))
                        {
                            info.m_nodes[i].SetTextures(
                                new TextureSet
                                    (@"Roads\PedestrianRoads\Stone\Textures\Elevated_Trans__MainTex.png",
                                     @"Roads\PedestrianRoads\Stone\Textures\Elevated_Trans__AlphaMap.png"),
                                new LODTextureSet
                                    (@"Roads\PedestrianRoads\Stone\Textures\Elevated_Trans_LOD__MainTex.png",
                                    @"Roads\PedestrianRoads\Stone\Textures\Elevated_Trans_LOD__AlphaMap.png",
                                    @"Roads\PedestrianRoads\Stone\Textures\Elevated_Node_LOD__XYSMap.png"));
                        }
                        else
                        {
                            info.m_nodes[i].SetTextures(
                                new TextureSet
                                    (@"Roads\PedestrianRoads\Stone\Textures\Elevated_Node__MainTex.png",
                                    @"Roads\PedestrianRoads\Stone\Textures\Elevated_Node__AlphaMap.png",
                                    @"Roads\PedestrianRoads\Stone\Textures\Elevated_Segment__XYSMap.png"),
                                new LODTextureSet
                                    (@"Roads\PedestrianRoads\Stone\Textures\Elevated_Trans_LOD__MainTex.png",
                                    @"Roads\PedestrianRoads\Stone\Textures\Elevated_Node_LOD__AlphaMap.png",
                                    @"Roads\PedestrianRoads\Stone\Textures\Elevated_Node_LOD__XYSMap.png"));
                        }
                    }
                    break;
            }
        }
    }
}
