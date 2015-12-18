using System;
using UnityEngine;

namespace Transit.Framework.Texturing
{
    public class TextureBlenderComponent : ITextureBlenderComponent
    {
        private readonly Func<Texture2D> _textureProvider;
        public Point Position { get; set; }
        public bool IsRelativeFromPrevious { get; set; }
        public bool IncreaseHOffset { get; set; }
        public bool IncreaseVOffset { get; set; }
        public byte AlphaLevel { get; set; }

        public TextureBlenderComponent(Func<Texture2D> textureProvider)
        {
            _textureProvider = textureProvider;
            IsRelativeFromPrevious = true;
            IncreaseHOffset = true;
            IncreaseVOffset = false;
            AlphaLevel = 255;
        }

        public void Apply(ref Point offset, Texture2D canvas)
        {
            var texture = _textureProvider();
            var texturePixels = _textureProvider().GetPixels();

            if (!IsRelativeFromPrevious)
            {
                offset = Position;
            }
            else
            {
                offset = new Point(offset.X + Position.X, offset.Y + Position.Y);
            }

            var canvasPixels = canvas.GetPixels(offset.X, offset.Y, texture.width, texture.height);

            for (int i = 0; i < canvasPixels.Length; i++)
            {
                if (i >= texturePixels.Length)
                {
                    throw new Exception("Texture is larger than the canvas");
                }

                var cPixel = canvasPixels[i];
                var tPixel = texturePixels[i];

                var tPixelAlphaLevel = tPixel.a * AlphaLevel / 255f;

                cPixel.r = Lerp(cPixel.r, tPixel.r, tPixelAlphaLevel);
                cPixel.g = Lerp(cPixel.g, tPixel.g, tPixelAlphaLevel);
                cPixel.b = Lerp(cPixel.b, tPixel.b, tPixelAlphaLevel);

                canvasPixels[i] = cPixel;
            }

            canvas.SetPixels(offset.X, offset.Y, texture.width, texture.height, canvasPixels);

            if (IncreaseHOffset)
            {
                offset = new Point(offset.X + texture.width, offset.Y);
            }

            if (IncreaseVOffset)
            {
                offset = new Point(offset.X, offset.Y + texture.height);
            }
        }

        private static float Lerp(float baseColorPart, float defaultColorPart, float level)
        {
            return baseColorPart + (defaultColorPart - baseColorPart) * level;
        }
    }

    public class TextureBlenderAlphaComponent : ITextureBlenderComponent
    {
        private readonly Func<Texture2D> _textureProvider;
        public Point Position { get; set; }
        public bool IsRelativeFromPrevious { get; set; }
        public bool IncreaseHOffset { get; set; }
        public bool IncreaseVOffset { get; set; }
        public Color DefaultColor { get; set; }

        public TextureBlenderAlphaComponent(Func<Texture2D> textureProvider)
        {
            _textureProvider = textureProvider;
            IsRelativeFromPrevious = true;
            IncreaseHOffset = true;
            IncreaseVOffset = false;
            DefaultColor = new Color(127f / 255f, 127f / 255f, 127f / 255f);
        }

        public void Apply(ref Point offset, Texture2D canvas)
        {
            var texture = _textureProvider();
            //Bitmap bitmap;

            //if (texture is Bitmap)
            //{
            //    bitmap = texture as Bitmap;
            //}
            //else
            //{
            //    bitmap = new Bitmap(texture);
            //}

            //if (!IsRelativeFromPrevious)
            //{
            //    offset = Position;
            //}
            //else
            //{
            //    offset = new Point(offset.X + Position.X, offset.Y + Position.Y);
            //}

            //for (int y = 0; y < canvas.Height; y++)
            //{
            //    if (y >= bitmap.Height)
            //    {
            //        continue;
            //    }

            //    for (int x = 0; x < canvas.Width; x++)
            //    {
            //        if (x >= bitmap.Width)
            //        {
            //            continue;
            //        }

            //        var cvsPixel = canvas.GetPixel(x, y);
            //        var imgPixel = bitmap.GetPixel(x, y);

            //        var alphaLvl = imgPixel.G;
            //        var aprLvl = imgPixel.B;

            //        var finalPixel = Color.FromArgb(alphaLvl, ApplyAPR(cvsPixel, DefaultColor, aprLvl));

            //        canvas.SetPixel(x, y, finalPixel);
            //    }
            //}

            //if (IncreaseHOffset)
            //{
            //    offset = new Point(offset.X + bitmap.Width, offset.Y);
            //}

            //if (IncreaseVOffset)
            //{
            //    offset = new Point(offset.X, offset.Y + bitmap.Height);
            //}
        }

        //private static Color ApplyAPR(Color baseColor, Color defaultColor, byte aprLevel)
        //{
        //    return new Color(
        //        ApplyAPRPart(baseColor.a, defaultColor.A, aprLevel),
        //        ApplyAPRPart(baseColor.R, defaultColor.R, aprLevel),
        //        ApplyAPRPart(baseColor.G, defaultColor.G, aprLevel),
        //        ApplyAPRPart(baseColor.B, defaultColor.B, aprLevel));
        //}

        //private static byte ApplyAPRPart(byte baseColorPart, byte defaultColorPart, byte aprLevel)
        //{
        //    return (byte)(baseColorPart + (defaultColorPart - baseColorPart) * aprLevel / 255);
        //}
    }
}
