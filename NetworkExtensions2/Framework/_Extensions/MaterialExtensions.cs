using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Transit.Framework
{
    public static class MaterialExtensions
    {
        public static Material CreateRoadMaterial(this Material material, Material originalMaterial)
        {
            var newMaterial = UnityEngine.Object.Instantiate(originalMaterial);
            var texNames = new string[] { "_MainTex", "_APRMap", "_XYSMap" };
            for (int i = 0; i < texNames.Length; i++)
            {
                var texture = material.GetTexture(texNames[i]) as Texture2D;
                if (originalMaterial.GetTexture(texNames[i]) != null)
                {
                    Debug.Log("original " + texNames[i] + " is " + originalMaterial.GetTexture(texNames[i]).name);
                }
                if (texture != null)
                {
                    Debug.Log("new " + texNames[i] + " is " + texture.name);
                    newMaterial.ModifyTexture(texNames[i], texture);
                }
            }
            return newMaterial;
        }

        public static void ModifyTexture(this Material material, string propertyName, Texture2D newTexture)
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
