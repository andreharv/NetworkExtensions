using System;
using System.Linq;
using UnityEngine;

namespace Transit.Framework
{
    public static partial class Prefabs
    {
        public static T Find<T>(string prefabName, bool crashOnNotFound = true) 
            where T : PrefabInfo
        {
            var prefab = Resources.FindObjectsOfTypeAll<T>().FirstOrDefault(p => p.name == prefabName);

            if (prefab == null)
            {
                if (crashOnNotFound)
                {
                    throw new Exception(string.Format("TFW: Prefab {0} not found", prefabName));
                }
            }

            return prefab;
        }
    }
}
