using UnityEngine;

namespace Transit.Framework
{
    public static class Color32Extensions
    {
        public static Color32 Dim(this Color32 origin, byte alpha)
        {
            return new Color32(origin.r, origin.g, origin.b, alpha);
        }
    }
}
