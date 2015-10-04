using System;
using System.IO;
using System.Reflection;
using ColossalFramework.IO;
using ColossalFramework.Steamworks;
using ICities;
using Transit.Framework;
using UnityEngine;

#if DEBUG
using Debug = Transit.Framework.Debug;
#endif

namespace Transit.Addon.RoadExtensions
{
    public partial class RExModule
    {
        public const string PATH_NOT_FOUND = "NOT_FOUND";

        private static string s_path = null;
        public static string GetPath()
        {
            if (s_path == null)
            {
                s_path = AssetAccess.GetPath("RoadExtensions", WORKSHOP_ID);

                if (s_path != PATH_NOT_FOUND)
                {
                    Debug.Log("REx: Mod path " + s_path);
                }
                else
                {
                    Debug.Log("REx: Path not found");
                }
            }

            return s_path;
        }
    }
}
