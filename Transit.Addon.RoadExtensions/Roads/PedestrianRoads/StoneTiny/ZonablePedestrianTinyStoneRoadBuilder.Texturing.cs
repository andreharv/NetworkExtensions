using Transit.Framework;
using Transit.Framework.Texturing;

namespace Transit.Addon.RoadExtensions.Roads.PedestrianRoads.StoneTiny
{
    public partial class ZonablePedestrianTinyStoneRoadBuilder
    {
        private static void SetupTextures(NetInfo info, NetInfoVersion version)
        {
            switch (version)
            {
                case NetInfoVersion.Ground:
                    {
                        info.SetAllSegmentsTexture(
                            new TextureSet
                               (@"Roads\PedestrianRoads\StoneTiny\Textures\Ground_Segment__MainTex.png",
                                @"Roads\PedestrianRoads\StoneTiny\Textures\Ground_Segment__AlphaMap.png",
                                @"Roads\PedestrianRoads\StoneTiny\Textures\Ground_Segment__XYSMap.png"),
                            new LODTextureSet
                               (@"Roads\PedestrianRoads\StoneTiny\Textures\Ground_Segment_LOD__MainTex.png",
                                @"Roads\PedestrianRoads\StoneTiny\Textures\Ground_Segment_LOD__AlphaMap.png",
                                @"Roads\PedestrianRoads\StoneTiny\Textures\Ground_Segment_LOD__XYSMap.png"));

                        foreach (var node in info.m_nodes)
                        {
                            if (node.m_flagsRequired != NetNode.Flags.Transition)
                            {
                                node.SetTextures(
                                    new TextureSet
                                        (@"Roads\PedestrianRoads\Common\Textures\Ground_Node__MainTex.png",
                                         @"Roads\PedestrianRoads\Common\Textures\Ground_Node__AlphaMap.png",
                                         @"Roads\PedestrianRoads\Common\Textures\Ground_Node__XYSMap.png"),
                                    new LODTextureSet
                                        (@"Roads\PedestrianRoads\Common\Textures\Ground_Node_LOD__MainTex.png",
                                         @"Roads\PedestrianRoads\Common\Textures\Ground_Node_LOD__AlphaMap.png",
                                         @"Roads\PedestrianRoads\Common\Textures\Ground_Node_LOD__XYSMap.png"));
                            }
                            else
                            {
                                node.SetTextures(
                                    new TextureSet
                                        (@"Roads\PedestrianRoads\Common\Textures\Ground_Trans__MainTex.png",
                                         @"Roads\PedestrianRoads\Common\Textures\Ground_Trans__AlphaMap.png",
                                         @"Roads\PedestrianRoads\Common\Textures\Ground_Node__XYSMap.png"),
                                    new LODTextureSet
                                        (@"Roads\PedestrianRoads\Common\Textures\Ground_Trans_LOD__MainTex.png",
                                         @"Roads\PedestrianRoads\Common\Textures\Ground_Trans_LOD__AlphaMap.png",
                                         @"Roads\PedestrianRoads\Common\Textures\Ground_Node_LOD__XYSMap.png"));
                            }
                        }

                        break;
                    }
                case NetInfoVersion.Elevated:
                case NetInfoVersion.Bridge:
                    { 
                        info.SetAllSegmentsTexture(
                            new TextureSet
                               (@"Roads\PedestrianRoads\StoneTiny\Textures\Elevated_Segment__MainTex.png",
                                @"Roads\PedestrianRoads\StoneTiny\Textures\Elevated_Segment__AlphaMap.png",
                                @"Roads\PedestrianRoads\Common\Textures\Elevated_Segment__XYSMap.png"),
                            new LODTextureSet
                               (@"Roads\PedestrianRoads\Common\Textures\Elevated_Segment_LOD__MainTex.png",
                                @"Roads\PedestrianRoads\Common\Textures\Elevated_Segment_LOD__AlphaMap.png",
                                @"Roads\PedestrianRoads\Common\Textures\Elevated_Segment_LOD__XYSMap.png"));

                        foreach (var node in info.m_nodes)
                        {
                            if (node.m_flagsRequired != NetNode.Flags.Transition)
                            {
                                node.SetTextures(
                                    new TextureSet
                                        (@"Roads\PedestrianRoads\Common\Textures\Elevated_Node__MainTex.png",
                                         @"Roads\PedestrianRoads\Common\Textures\Elevated_Node__AlphaMap.png"));
                            }
                            else
                            {
                                node.SetTextures(
                                    new TextureSet
                                        (@"Roads\PedestrianRoads\Common\Textures\Elevated_Trans__MainTex.png",
                                         @"Roads\PedestrianRoads\Common\Textures\Elevated_Trans__AlphaMap.png"));
                            }
                        }

                        break;
                    }
            }
        }
    }
}
