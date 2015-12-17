using System;
using System.Drawing;
using System.Drawing.Imaging;
using Transit.Addon.RoadExtensions.Highways.Highway2L;

namespace Transit.Framework.Texturing.Tests
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                GenerationHW2L_Segment();
                GenerationHW2L_Node();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Console.ReadLine();
            }
        }

        static void GenerationHW2L_Segment()
        {
            var texturePack = new Highway2LTexturePack();

            var mainTex = texturePack.SegmentMainTex.GetImage();
            var aprTex  = texturePack.SegmentAPRMap.GetImage();

            mainTex.Save(@"D:\Developpement\Git\CSL.TransitAddonMod\Transit.Addon.RoadExtensions\Highways\Highway2L\Textures\Ground_Segment__MainTex.png", ImageFormat.Png);
            aprTex.Save(@"D:\Developpement\Git\CSL.TransitAddonMod\Transit.Addon.RoadExtensions\Highways\Highway2L\Textures\Ground_Segment__APRMap.png", ImageFormat.Png);

            //var lookAndFeel = ImageBlender
            //    .FromImage(mainTex)
            //    .WithAlphaComponent(aprTex)
            //    .Apply();
            //lookAndFeel.Save("Segment__LAF.png", ImageFormat.Png);
        }

        static void GenerationHW2L_Node()
        {
            var texturePack = new Highway2LTexturePack();

            var mainTex = texturePack.NodeMainTex.GetImage();
            var aprTex  = texturePack.NodeAPRMap.GetImage();

            mainTex.Save(@"D:\Developpement\Git\CSL.TransitAddonMod\Transit.Addon.RoadExtensions\Highways\Highway2L\Textures\Ground_Node__MainTex.png", ImageFormat.Png);
            aprTex.Save(@"D:\Developpement\Git\CSL.TransitAddonMod\Transit.Addon.RoadExtensions\Highways\Highway2L\Textures\Ground_Node__APRMap.png", ImageFormat.Png);

            //var lookAndFeel = ImageBlender
            //    .FromImage(mainTex)
            //    .WithAlphaComponent(aprTex)
            //    .Apply();
            //lookAndFeel.Save("Node__LAF.png", ImageFormat.Png);
        }
    }
}
