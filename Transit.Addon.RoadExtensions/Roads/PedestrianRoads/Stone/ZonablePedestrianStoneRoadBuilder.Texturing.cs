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
                            @"Roads\PedestrianRoads\Stone\Textures\Ground_Segment__XYSMap.png"));

                    foreach (var node in info.m_nodes)
                    {
                        if (node.m_flagsRequired == NetNode.Flags.Transition)
                        {
                            node.SetTextures(
                                new TextureSet
                                    (@"Roads\PedestrianRoads\Stone\Textures\Ground_Trans__MainTex.png",
                                        @"Roads\PedestrianRoads\Stone\Textures\Ground_Trans__AlphaMap.png"));
                        }
                        else
                        {
                            node.SetTextures(
                                new TextureSet
                                    (@"Roads\PedestrianRoads\Stone\Textures\Ground_Node__MainTex.png",
                                        @"Roads\PedestrianRoads\Stone\Textures\Ground_Node__AlphaMap.png",
                                        @"Roads\PedestrianRoads\Stone\Textures\Ground_Node__XYSMap.png"));
                        }
                    }
                    break;
            }
        }
    }
}
