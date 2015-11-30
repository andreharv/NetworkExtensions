using System.IO;
using ColossalFramework.IO;
using ColossalFramework.Steamworks;
using Transit.Framework.Modularity;
using UnityEngine;

namespace Transit.Framework
{
    public static class TransitModExtensions
    {
        public static string GetAssetPath(this ITransitMod transitMod)
        {
            var basePath = GetAssetPathInternal(transitMod.Name, transitMod.WorkshopId);

            if (basePath != Assets.PATH_NOT_FOUND)
            {
                Debug.Log("TFW: Mod path " + basePath);
            }
            else
            {
                Debug.Log("TFW: Path not found");
            }

            return basePath;
        }

        private static string GetAssetPathInternal(string folderName, ulong workshopId)
        {
            // 1. Check Local path (CurrentUser\Appdata\Local\Colossal Order\Cities_Skylines\Addons\Mods)
            var localPath = Path.Combine(DataLocation.modsPath, folderName);
            Debug.Log(string.Format("TFW: Exist={0} DataLocation.modsPath={1}", Directory.Exists(localPath), localPath));

            if (Directory.Exists(localPath))
            {
                return localPath;
            }

            // 2. Check Local path (CurrentUser\Appdata\Local\Colossal Order\Cities_Skylines\Addons\Mods) without spaces
            localPath = Path.Combine(DataLocation.modsPath, folderName.Replace(" ", ""));
            Debug.Log(string.Format("TFW: Exist={0} DataLocation.modsPath={1}", Directory.Exists(localPath), localPath));

            if (Directory.Exists(localPath))
            {
                return localPath;
            }

            // 3. Check Steam
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

            // 4. Check Cities Skylines files folder
            var csFolderPath = Path.Combine(Path.Combine(DataLocation.gameContentPath, "Mods"), folderName);
            Debug.Log(string.Format("TFW: Exist={0} DataLocation.gameContentPath={1}", Directory.Exists(csFolderPath), csFolderPath));
            if (Directory.Exists(csFolderPath))
            {
                return csFolderPath;
            }

            // 5. Check Cities Skylines files folder without spaces
            csFolderPath = Path.Combine(Path.Combine(DataLocation.gameContentPath, "Mods"), folderName.Replace(" ", ""));
            Debug.Log(string.Format("TFW: Exist={0} DataLocation.gameContentPath={1}", Directory.Exists(csFolderPath), csFolderPath));
            if (Directory.Exists(csFolderPath))
            {
                return csFolderPath;
            }

            return Assets.PATH_NOT_FOUND;
        }
    }
}
