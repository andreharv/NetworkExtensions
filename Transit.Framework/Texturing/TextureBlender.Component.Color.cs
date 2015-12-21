using System;
using UnityEngine;

namespace Transit.Framework.Texturing
{
    public partial class TextureBlender
    {
        private class ColorComponent : ComponentBase, ITextureBlenderComponent
        {
            private readonly Color _color;
            private readonly int _width;
            private readonly int? _height;

            public ColorComponent(
                Color color, 
                int width, 
                int? height = null,
                Point position = null,
                bool positionRelativeFromPrevious = true,
                bool increaseXOffset = true,
                bool increaseYOffset = false)
                : base(position, positionRelativeFromPrevious, increaseXOffset, increaseYOffset)
            {
                _color = color;
                _width = width;
                _height = height;
            }

            public void Apply(ref Point offset, Texture2D canvas)
            {
                //Point drawPosition;
                //if (!IsRelativeFromPrevious)
                //{
                //    drawPosition = Position;
                //}
                //else
                //{
                //    drawPosition = new Point(offset.X + Position.X, offset.Y + Position.Y);
                //}

                //var canvasPixels = canvas.GetPixels(drawPosition.X, drawPosition.Y, texture.width, texture.height);

                //for (int i = 0; i < canvasPixels.Length; i++)
                //{
                //    if (i >= texturePixels.Length)
                //    {
                //        throw new Exception("Texture is larger than the canvas");
                //    }

                //    var cPixel = canvasPixels[i];
                //    var tPixel = texturePixels[i];

                //    var alphaLvl = tPixel.g;
                //    var aprLvl = tPixel.b;

                //    var tPixelAlphaLevel = aprLvl / 255f;

                //    cPixel.a = alphaLvl;
                //    cPixel.r = TextureHelper.Lerp(cPixel.r, _color.r, tPixelAlphaLevel);
                //    cPixel.g = TextureHelper.Lerp(cPixel.g, _color.g, tPixelAlphaLevel);
                //    cPixel.b = TextureHelper.Lerp(cPixel.b, _color.b, tPixelAlphaLevel);

                //    canvasPixels[i] = cPixel;
                //}

                //canvas.SetPixels(drawPosition.X, drawPosition.Y, texture.width, texture.height, canvasPixels);

                //if (IncreaseXOffset)
                //{
                //    offset = new Point(drawPosition.X + texture.width, offset.Y);
                //}

                //if (IncreaseYOffset)
                //{
                //    offset = new Point(offset.X, drawPosition.Y + texture.height);
                //}
            }
        }
    }
}
