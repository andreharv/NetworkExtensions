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
                var laneStart = 340;
                var laneWidth = 140;
                var lineAlpha = (byte) 200;
                var tearsAlpha = (byte) 40;

                var mainTex = ImageBlender
                    .FromBaseFile(@"HW2L\MainTex\Segment__MainTex.png")
                    .WithComponent(@"HW2L\MainTex\Line_Yellow_Solid__MainTex.png", new Point(laneStart, 0))
                    .WithComponent(@"HW2L\MainTex\Line_White_Dashed__MainTex.png", new Point(laneWidth, 0))
                    .WithComponent(@"HW2L\MainTex\Line_White_Solid__MainTex.png", new Point(laneWidth, 0))
                    .Apply();

                var aprTex = ImageBlender
                    .FromBaseFile(@"HW2L\APRMap\Segment__APRMap.png")
                    .WithComponent(@"HW2L\APRMap\Line_Solid__APRMap.png",  new Point(laneStart, 0), lineAlpha)
                    .WithComponent(@"HW2L\APRMap\Tearing__APRMap.png",     new Point(0, 0), tearsAlpha)
                    .WithComponent(@"HW2L\APRMap\Line_Dashed__APRMap.png", new Point(0, 0), lineAlpha)
                    .WithComponent(@"HW2L\APRMap\Tearing__APRMap.png",     new Point(0, 0), tearsAlpha)
                    .WithComponent(@"HW2L\APRMap\Line_Solid__APRMap.png",  new Point(0, 0), lineAlpha)
                    .Apply();

                var lookAndFeel = ImageBlender
                    .FromImage(mainTex)
                    .WithAlphaComponent(aprTex)
                    .Apply();

                mainTex.Save("Segment__MainTex.png", ImageFormat.Png);
                aprTex.Save("Segment__APRMap.png", ImageFormat.Png);
                lookAndFeel.Save("Segment__LAF.png", ImageFormat.Png);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Console.ReadLine();
            }
        }
    }
}
