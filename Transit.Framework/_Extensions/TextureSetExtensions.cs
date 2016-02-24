using Transit.Framework.Texturing;
using UnityEngine;

namespace Transit.Framework
{
    public static class TextureSetExtensions
    {
        public static Material CreateRoadMaterial(this TextureSet textureSet, Material originalMaterial)
        {
            var material = Object.Instantiate(originalMaterial);

            if (textureSet.MainTex != null)
            {
                material.ModifyTexture("_MainTex", textureSet.MainTex);
            }

            if (textureSet.APRMap != null)
            {
                material.ModifyTexture("_APRMap", textureSet.APRMap);
            }

            if (textureSet.XYSMap != null)
            {
                material.ModifyTexture("_XYSMap", textureSet.XYSMap);
            }

            return material;

            ////TODO: Making that work might maybe solve some issues
            //var shader = Shader.Find("Custom/Net/Road");
            //var material = new Material(shader);
            //material.CopyPropertiesFromMaterial(originalMaterial);

            //if (textureSet.MainTex != null)
            //{
            //    material.SetTexture("_MainTex", textureSet.MainTex);
            //}
            ////else
            ////{
            ////    var originalTexture = material.GetTexture("_MainTex") as Texture2D;

            ////    if (originalTexture != null)
            ////    {
            ////        material.SetTexture("_MainTex", originalTexture);
            ////    }
            ////}


            //if (textureSet.APRMap != null)
            //{
            //    material.SetTexture("_APRMap", textureSet.APRMap);
            //}
            ////else
            ////{
            ////    var originalTexture = material.GetTexture("_APRMap") as Texture2D;

            ////    if (originalTexture != null)
            ////    {
            ////        material.SetTexture("_APRMap", originalTexture);
            ////    }
            ////}


            //if (textureSet.XYSMap != null)
            //{
            //    material.SetTexture("_XYSMap", textureSet.XYSMap);
            //}
            ////else
            ////{
            ////    var originalTexture = material.GetTexture("_XYSMap") as Texture2D;

            ////    if (originalTexture != null)
            ////    {
            ////        material.SetTexture("_XYSMap", originalTexture);
            ////    }
            ////}

            //return material;
        }
    }
}
