using System.IO;
using ColossalFramework.IO;
using ColossalFramework.Steamworks;
using UnityEngine;

#if DEBUG
using Debug = Transit.Framework.Debug;
#endif

namespace Transit.Framework
{
    public static class Assets
    {
        public const string PATH_NOT_FOUND = "NOT_FOUND";

        public static string GetPath(string folderName, ulong workshopId)
        {
            // 1. Check Local path (CurrentUser\Appdata\Local\Colossal Order\Cities_Skylines\Addons\Mods)
            var localPath = Path.Combine(DataLocation.modsPath, folderName);
            Debug.Log(string.Format("TFW: Exist={0} DataLocation.modsPath={1}", Directory.Exists(localPath), localPath));

            if (Directory.Exists(localPath))
            {
                return localPath;
            }

            // 2. Check Steam
            foreach (var mod in Steam.workshop.GetSubscribedItems())
            {
                if (mod.AsUInt64 == workshopId)
                {
                    var workshopPath = Steam.workshop.GetSubscribedItemPath(mod);
                    Debug.Log(string.Format("TFW: Exist={0} WorkshopPath={1}", Directory.Exists(workshopPath), workshopPath));
                    if (Directory.Exists(workshopPath))
                    {
                        return workshopPath;
                    }
                }
            }

            // 3. Check Cities Skylines files folder
            var csFolderPath = Path.Combine(Path.Combine(DataLocation.gameContentPath, "Mods"), folderName);
            Debug.Log(string.Format("TFW: Exist={0} DataLocation.gameContentPath={1}", Directory.Exists(csFolderPath), csFolderPath));
            if (Directory.Exists(csFolderPath))
            {
                return csFolderPath;
            }

            return PATH_NOT_FOUND;
        }
    }
}
