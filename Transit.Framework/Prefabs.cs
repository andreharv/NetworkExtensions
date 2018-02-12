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
            T prefab = null;
            try
            {
                prefab = PrefabCollection<T>.FindLoaded(prefabName);
            }
            catch (NullReferenceException ex)
            {
                UnityEngine.Debug.LogError(string.Format("TFW: Prefab {0} not found.", prefabName));
                if (crashOnNotFound)
                {
                    throw;
                }
            }

            return prefab;
        }
        public static T FindResource<T>(string prefabName, bool crashOnNotFound = true)
            where T : PrefabInfo
        {
            T prefab = null;
            try
            {
                prefab = Resources.FindObjectsOfTypeAll<T>().FirstOrDefault(p => p.name == prefabName);
            }
            catch (NullReferenceException ex)
            {
                UnityEngine.Debug.LogError(string.Format("TFW: Prefab {0} not found.", prefabName));
                if (crashOnNotFound)
                {
                    throw;
                }
            }

            return prefab;
        }
    }
}
