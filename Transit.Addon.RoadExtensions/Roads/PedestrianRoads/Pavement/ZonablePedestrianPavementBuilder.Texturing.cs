using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Transit.Framework;
using Transit.Framework.Network;
using Transit.Framework.Texturing;

namespace Transit.Addon.RoadExtensions.Roads.PedestrianRoads.Pavement
{
    public partial class ZonablePedestrianPavementBuilder
    {
        public void SetupTextures(NetInfo info, NetInfoVersion version)
        {
            info.SetAllSegmentsTexture(
                new TextureSet
                   (@"Roads\PedestrianRoads\Pavement\Textures\Elevated_Segment__MainTex.png",
                    @"Roads\PedestrianRoads\Pavement\Textures\Elevated_Segment__AlphaMap.png",
                    @"Roads\PedestrianRoads\StoneSmall\Textures\Elevated_Segment__XYSMap.png"),
                new LODTextureSet
                   (@"Roads\PedestrianRoads\StoneSmall\Textures\Elevated_Segment_LOD__MainTex.png",
                    @"Roads\PedestrianRoads\StoneSmall\Textures\Elevated_Segment_LOD__AlphaMap.png",
                    @"Roads\PedestrianRoads\StoneSmall\Textures\Elevated_Segment_LOD__XYSMap.png"));

            foreach (var node in info.m_nodes)
            {
                if (node.m_flagsRequired != NetNode.Flags.Transition)
                {
                    node.SetTextures(
                        new TextureSet
                            (@"Roads\PedestrianRoads\StoneSmall\Textures\Elevated_Node__MainTex.png",
                                @"Roads\PedestrianRoads\Pavement\Textures\Elevated_Segment__AlphaMap.png"));
                }
                else
                {
                    node.SetTextures(
                        new TextureSet
                            (@"Roads\PedestrianRoads\StoneSmall\Textures\Elevated_Trans__MainTex.png",
                                @"Roads\PedestrianRoads\Pavement\Textures\Elevated_Trans__AlphaMap.png"));
                }
            }
        }
    }
}
