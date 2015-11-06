using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Transit.Framework
{
    public static class MaterialExtensions
    {
        public static Material Clone(this Material originalMaterial, TexturesSet newTextures)
        {
            var material = UnityEngine.Object.Instantiate(originalMaterial);

            if (newTextures.MainTex != null)
            {
                material.ModifyTexture("_MainTex", newTextures.MainTex);
            }

            if (newTextures.APRMap != null)
            {
                material.ModifyTexture("_APRMap", newTextures.APRMap);
            }

            if (newTextures.XYSMap != null)
            {
                material.ModifyTexture("_XYSMap", newTextures.XYSMap);
            }

            return material;
        }

        private static void ModifyTexture(this Material material, string propertyName, Texture2D newTexture)
        {
            var currentTexture = material.GetTexture(propertyName) as Texture2D;

            if (currentTexture == null)
            {
                return;
            } 

            material.SetTexture(propertyName, newTexture);
        }
    }
}
