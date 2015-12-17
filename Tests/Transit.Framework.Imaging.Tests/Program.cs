using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace Transit.Framework.Imaging.Tests
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
            var laneStart = 340;
            var laneWidth = 141;
            var lineAlpha = (byte)35;
            var tearsAlpha = (byte)15;

            var mainTex = ImageBlender
                .FromBaseFile(@"Roads\Common\Textures\MainTex\Segment__MainTex.png")
                .WithComponent(@"Roads\Common\Textures\MainTex\Line_White_Solid__MainTex.png", new Point(laneStart, 0))
                .WithComponent(@"Roads\Common\Textures\MainTex\Line_White_Dashed__MainTex.png", new Point(laneWidth, 0))
                .WithComponent(@"Roads\Common\Textures\MainTex\Line_White_Solid__MainTex.png", new Point(laneWidth, 0))
                .Apply();

            var aprTex = ImageBlender
                .FromBaseFile(@"Roads\Common\Textures\APRMap\Segment__APRMap.png")
                .WithComponent(@"Roads\Common\Textures\APRMap\Line_Solid__APRMap.png", new Point(laneStart, 0), lineAlpha)
                .WithComponent(@"Roads\Common\Textures\APRMap\Tearing__APRMap.png", new Point(1, 0), tearsAlpha)
                .WithComponent(@"Roads\Common\Textures\APRMap\Line_Dashed__APRMap.png", new Point(0, 0), lineAlpha)
                .WithComponent(@"Roads\Common\Textures\APRMap\Tearing__APRMap.png", new Point(0, 0), tearsAlpha)
                .WithComponent(@"Roads\Common\Textures\APRMap\Line_Solid__APRMap.png", new Point(2, 0), lineAlpha)
                .Apply();

            //var lookAndFeel = ImageBlender
            //    .FromImage(mainTex)
            //    .WithAlphaComponent(aprTex)
            //    .Apply();

            mainTex.Save(@"D:\Developpement\Git\CSL.TransitAddonMod\Transit.Addon.RoadExtensions\Highways\Highway2L\Textures\Ground_Segment__MainTex.png", ImageFormat.Png);
            aprTex.Save(@"D:\Developpement\Git\CSL.TransitAddonMod\Transit.Addon.RoadExtensions\Highways\Highway2L\Textures\Ground_Segment__APRMap.png", ImageFormat.Png);
            //lookAndFeel.Save("Segment__LAF.png", ImageFormat.Png);
        }

        static void GenerationHW2L_Node()
        {
            var laneStart = 340;
            var laneWidth = 141;
            var lineAlpha = (byte)35;
            var tearsAlpha = (byte)15;

            var mainTex = ImageBlender
                .FromBaseFile(@"Roads\Common\Textures\MainTex\Segment__MainTex.png")
                .WithComponent(@"Roads\Common\Textures\MainTex\Line_White_Solid_Fadeout__MainTex.png", new Point(laneStart, 0))
                .WithComponent(@"Roads\Common\Textures\MainTex\Line_White_Solid_Fadeout__MainTex.png", new Point(laneWidth + 4 + laneWidth, 0))
                .Apply();

            var aprTex = ImageBlender
                .FromBaseFile(@"Roads\Common\Textures\APRMap\Segment__APRMap.png")
                .WithComponent(@"Roads\Common\Textures\APRMap\Line_Solid_Fadeout__APRMap.png", new Point(laneStart, 0), lineAlpha)
                .WithComponent(@"Roads\Common\Textures\APRMap\Tearing_Fadeout__APRMap.png", new Point(1, 0), tearsAlpha)
                .WithComponent(@"Roads\Common\Textures\APRMap\Tearing_Fadeout__APRMap.png", new Point(4, 0), tearsAlpha)
                .WithComponent(@"Roads\Common\Textures\APRMap\Line_Solid_Fadeout__APRMap.png", new Point(2, 0), lineAlpha)
                .Apply();

            //var lookAndFeel = ImageBlender
            //    .FromImage(mainTex)
            //    .WithAlphaComponent(aprTex)
            //    .Apply();

            mainTex.Save(@"D:\Developpement\Git\CSL.TransitAddonMod\Transit.Addon.RoadExtensions\Highways\Highway2L\Textures\Ground_Node__MainTex.png", ImageFormat.Png);
            aprTex.Save(@"D:\Developpement\Git\CSL.TransitAddonMod\Transit.Addon.RoadExtensions\Highways\Highway2L\Textures\Ground_Node__APRMap.png", ImageFormat.Png);
            //lookAndFeel.Save("Node__LAF.png", ImageFormat.Png);
        }
    }
}
