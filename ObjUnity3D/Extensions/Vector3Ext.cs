using UnityEngine;

namespace ObjUnity3D
{
    public static class Vector3Ext
    {
        public static readonly Vector3 IgnoreX = new Vector3(0f, 1f, 1f);

        public static readonly Vector3 IgnoreY = new Vector3(1f, 0f, 1f);

        public static readonly Vector3 IgnoreZ = new Vector3(1f, 1f, 0f);

        public static Color ToColor(this Vector3 lVector)
        {
            return new Color(lVector.x, lVector.y, lVector.z);
        }
    }
}
