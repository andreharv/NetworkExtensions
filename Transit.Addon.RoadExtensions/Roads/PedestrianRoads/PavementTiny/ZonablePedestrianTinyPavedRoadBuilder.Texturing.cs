using Transit.Framework;
using Transit.Framework.Texturing;

namespace Transit.Addon.RoadExtensions.Roads.PedestrianRoads.PavementTiny
{
    public partial class ZonablePedestrianTinyPavedRoadBuilder
    {
        public void SetupTextures(NetInfo info, NetInfoVersion version)
        {
            info.SetAllSegmentsTexture(
                new TextureSet
                   (@"Roads\PedestrianRoads\PavementTiny\Textures\Elevated_Segment__MainTex.png",
                    @"Roads\PedestrianRoads\PavementTiny\Textures\Elevated_Segment__AlphaMap.png",
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
                             @"Roads\PedestrianRoads\PavementTiny\Textures\Elevated_Segment__AlphaMap.png"));
                }
                else
                {
                    node.SetTextures(
                        new TextureSet
                            (@"Roads\PedestrianRoads\Common\Textures\Elevated_Trans__MainTex.png",
                             @"Roads\PedestrianRoads\PavementTiny\Textures\Elevated_Trans__AlphaMap.png"));
                }
            }
        }
    }
}
