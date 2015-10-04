using UnityEngine;

namespace ObjUnity3D
{
    public static class Vector4Ext
    {
        public static Color ToColor(this Vector4 lVector)
        {
            return new Color(lVector.x, lVector.y, lVector.z, lVector.w);
        }
    }
}
