using Transit.Addon.RoadExtensions.Roads.Common;
using Transit.Framework;
using Transit.Framework.Texturing;
#if DEBUG
using Debug = Transit.Framework.Debug;
#endif
namespace Transit.Addon.RoadExtensions.Roads.WideAvenues.Avenue6LBusCenterBike
{
    public partial class Avenue6LBusCenterBikeBuilderEndNode
    {
        private static void SetupTextures(NetInfo info, NetInfoVersion version)
        {
           
             switch (version)
            {

                case NetInfoVersion.Ground:
                case NetInfoVersion.GroundGrass:
                case NetInfoVersion.GroundTrees:


                    var isGrass = version.ToString().Substring(6).Length > 0;
                    var suffix = isGrass ? "Grass" : "Concrete";

                    info.m_segments[0].SetTextures(
                       new TextureSet
                           (@"Roads\WideAvenues\Avenue6LBusCenterBike\Textures\Ground_Segment_End__MainTex.png",
                           $@"Roads\WideAvenues\Avenue6LBusCenterBike\Textures\Ground_Segment{suffix}End__APRMap.png"),
                   new LODTextureSet
                           (@"Roads\WideAvenues\Avenue6LBusCenterBike\Textures\Ground_Segment_LOD__MainTex.png",
                          $@"Roads\WideAvenues\Avenue6LBusCenterBike\Textures\Ground_SegmentConcrete_LOD__APRMap.png",
                           @"Roads\WideAvenues\Avenue6LBusCenterBike\Textures\Ground_Segment_LOD__XYSMap.png"));
       
                    info.m_segments[1].SetTextures(
                   new TextureSet
                       (@"Roads\WideAvenues\Avenue6LBusCenterBike\Textures\Ground_Segment_End__MainTex1.png",
                       $@"Roads\WideAvenues\Avenue6LBusCenterBike\Textures\Ground_Segment{suffix}BeforeEnd_Inverted__APRMap.png"),
               new LODTextureSet
                       (@"Roads\WideAvenues\Avenue6LBusCenterBike\Textures\Ground_Segment_LOD__MainTex.png",
                      $@"Roads\WideAvenues\Avenue6LBusCenterBike\Textures\Ground_SegmentConcrete_LOD__APRMap.png",
                       @"Roads\WideAvenues\Avenue6LBusCenterBike\Textures\Ground_Segment_LOD__XYSMap.png"));

                    info.m_segments[2].SetTextures(
                   new TextureSet
                       (@"Roads\WideAvenues\Avenue6LBusCenterBike\Textures\Ground_Segment_End__MainTex2.png",
                       $@"Roads\WideAvenues\Avenue6LBusCenterBike\Textures\Ground_Segment{suffix}BeforeEnd_Inverted__APRMap.png"),
               new LODTextureSet
                       (@"Roads\WideAvenues\Avenue6LBusCenterBike\Textures\Ground_Segment_LOD__MainTex.png",
                      $@"Roads\WideAvenues\Avenue6LBusCenterBike\Textures\Ground_SegmentConcrete_LOD__APRMap.png",
                       @"Roads\WideAvenues\Avenue6LBusCenterBike\Textures\Ground_Segment_LOD__XYSMap.png"));

                    
                    info.m_segments[3].SetTextures(
             new TextureSet
                 (@"Roads\WideAvenues\Avenue6LBusCenterBike\Textures\Ground_Segment_End__MainTex3.png",
                 $@"Roads\WideAvenues\Avenue6LBusCenterBike\Textures\Ground_Segment{suffix}BeforeEnd_Inverted__APRMap.png"),
         new LODTextureSet
                 (@"Roads\WideAvenues\Avenue6LBusCenterBike\Textures\Ground_Segment_LOD__MainTex.png",
                $@"Roads\WideAvenues\Avenue6LBusCenterBike\Textures\Ground_SegmentConcrete_LOD__APRMap.png",
                 @"Roads\WideAvenues\Avenue6LBusCenterBike\Textures\Ground_Segment_LOD__XYSMap.png"));
                    
                                        info.m_segments[4].SetTextures(
                                       new TextureSet
                                           (@"Roads\WideAvenues\Avenue6LBusCenterBike\Textures\Ground_Segment_End__MainTex4.png",
                                           $@"Roads\WideAvenues\Avenue6LBusCenterBike\Textures\Ground_Segment{suffix}BeforeEnd_Inverted__APRMap.png"),
                                   new LODTextureSet
                                           (@"Roads\WideAvenues\Avenue6LBusCenterBike\Textures\Ground_Segment_LOD__MainTex.png",
                                          $@"Roads\WideAvenues\Avenue6LBusCenterBike\Textures\Ground_SegmentConcrete_LOD__APRMap.png",
                                           @"Roads\WideAvenues\Avenue6LBusCenterBike\Textures\Ground_Segment_LOD__XYSMap.png"));
                                           
                    info.SetAllNodesTexture(
                     new TextureSet
                         (@"Roads\WideAvenues\Avenue6LBusCenterBike\Textures\Ground_Node_End__MainTex.png",
                         @"Roads\WideAvenues\Avenue6LBusCenterBike\Textures\Ground_Node_End__APRMap.png"),
                     new LODTextureSet
                        (@"Roads\WideAvenues\Avenue6LBusCenterBike\Textures\Ground_Node_LOD__MainTex.png",
                         @"Roads\WideAvenues\Avenue6LBusCenterBike\Textures\Ground_Node_LOD__APRMap.png",
                         @"Roads\WideAvenues\Avenue6LBusCenterBike\Textures\Ground_Segment_LOD__XYSMap.png"));

                    break;

            }
        }
    }
}

