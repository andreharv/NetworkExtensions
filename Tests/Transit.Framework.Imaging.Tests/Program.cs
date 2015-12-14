using System.Drawing;

namespace Transit.Framework.Imaging.Tests
{
    class Program
    {
        static void Main(string[] args)
        {
            var laneStart = 340;
            var laneWidth = 140;
            var lineAlpha = (byte)50;
            var b = new ImageBlender(@"HW2L\Canvas__MainTex.png");

            b.AddComponent(new BlendableComponent
            {
                Path = @"HW2L\Line_Solid__MainTex.png",
                Position = new Point(laneStart, 0),
                AlphaLevel = lineAlpha
            });

            b.AddComponent(new BlendableComponent
            {
                Path = @"HW2L\Line_Dashed__MainTex.png",
                Position = new Point(laneWidth, 0),
                AlphaLevel = lineAlpha
            });

            b.AddComponent(new BlendableComponent
            {
                Path = @"HW2L\Line_Solid__MainTex.png",
                Position = new Point(laneWidth, 0),
                AlphaLevel = lineAlpha
            });

            b.Apply();
        }
    }
}
