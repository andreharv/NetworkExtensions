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
                var lineAlpha = (byte)100;

                var mainTex = ImageBlender
                    .FromBaseFile(@"HW2L\MainTex\Segment__MainTex.png")
                    .WithComponent(@"HW2L\MainTex\Line_Solid__MainTex.png",  new Point(laneStart, 0), lineAlpha)
                    .WithComponent(@"HW2L\MainTex\Line_Dashed__MainTex.png", new Point(laneWidth, 0), lineAlpha)
                    .WithComponent(@"HW2L\MainTex\Line_Solid__MainTex.png",  new Point(laneWidth, 0), lineAlpha)
                    .Apply();

                var aprTex = ImageBlender
                    .FromBaseFile(@"HW2L\APRMap\Segment__APRMap.png")
                    .Apply();

                var lookAndFeel = ImageBlender
                    .FromImage(mainTex)
                    .WithAlphaComponent(aprTex)
                    .Apply();

                lookAndFeel.Save("Segment__LAF.png", ImageFormat.Png); ;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Console.ReadLine();
            }
        }
    }
}
