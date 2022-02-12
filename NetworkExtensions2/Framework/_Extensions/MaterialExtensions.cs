using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Transit.Framework
{
    public static class MaterialExtensions
    {
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
