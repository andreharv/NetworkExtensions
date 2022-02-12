using static ColossalFramework.Plugins.PluginManager;
using ColossalFramework.Plugins;
using ColossalFramework.PlatformServices;
using Object = UnityEngine.Object;
using UnityEngine;
using System.Linq;
using ICities;
using System;
using Transit.Framework.Mod;
using System.IO;
using ColossalFramework.IO;

namespace Transit.Framework
{
    public static class Tools
    {
        public static void Compare<T>(T unityObj, T otherUnityObj)
             where T : Object
        {
            Debug.Log(string.Format("TFW: ----->  Comparing {0} with {1}", unityObj.name, otherUnityObj.name));

            var fields = typeof(T).GetAllFieldsFromType();

            foreach (var f in fields)
            {
                var newValue = f.GetValue(unityObj);
                var oldValue = f.GetValue(otherUnityObj);

                if (!Equals(newValue, oldValue))
                {
                    Debug.Log(string.Format("Value {0} not equal (N-O) ({1},{2})", f.Name, newValue, oldValue));
                }
            }
        }

        public static void ListMembers<T>(this T unityObj)
            where T : Object
        {
            Debug.Log(string.Format("TFW: ----->  Listing {0}", unityObj.name));

            var fields = typeof(T).GetAllFieldsFromType();

            foreach (var f in fields)
            {
                var value = f.GetValue(unityObj);
                Debug.Log(string.Format("Member name \"{0}\" value is \"{1}\"", f.Name, value));
            }
        }

        public static string PackageName(string assetName)
        {
            var publishedFileID = PluginInfo.publishedFileID.ToString();
            if (publishedFileID.Equals(PublishedFileId.invalid.ToString()))
            {
                return assetName;
            }
            return publishedFileID;
        }

        private static PluginInfo PluginInfo
        {
            get
            {
                var pluginManager = PluginManager.instance;
                var plugins = pluginManager.GetPluginsInfo();

                foreach (var item in plugins)
                {
                    try
                    {
                        var instances = item.GetInstances<IUserMod>();
                        if (!(instances.FirstOrDefault() is TransitModBase))
                        {
                            continue;
                        }
                        return item;
                    }
                    catch
                    {

                    }
                }
                throw new Exception("Failed to find NetworkExtensions assembly!");

            }
        }

        public static string GetAssetPath(string defaultFolderPath, ulong workshopId)
        {
            // 1. Check Local path (CurrentUser\Appdata\Local\Colossal Order\Cities_Skylines\Addons\Mods)
            var localPath = Path.Combine(DataLocation.modsPath, defaultFolderPath);
            Debug.Log(string.Format("TFW: Exist={0} DataLocation.modsPath={1}", Directory.Exists(localPath), localPath));

            if (Directory.Exists(localPath))
            {
                return localPath;
            }

            // 2. Check Local path (CurrentUser\Appdata\Local\Colossal Order\Cities_Skylines\Addons\Mods) without spaces
            localPath = Path.Combine(DataLocation.modsPath, defaultFolderPath.Replace(" ", ""));
            Debug.Log(string.Format("TFW: Exist={0} DataLocation.modsPath={1}", Directory.Exists(localPath), localPath));

            if (Directory.Exists(localPath))
            {
                return localPath;
            }

            // 3. Check Steam
            foreach (var mod in PlatformService.workshop.GetSubscribedItems())
            {
                if (mod.AsUInt64 == workshopId)
                {
                    var workshopPath = PlatformService.workshop.GetSubscribedItemPath(mod);
                    Debug.Log(string.Format("TFW: Exist={0} WorkshopPath={1}", Directory.Exists(workshopPath), workshopPath));
                    if (Directory.Exists(workshopPath))
                    {
                        return workshopPath;
                    }
                }
            }

            // 4. Check Cities Skylines files folder
            var csFolderPath = Path.Combine(Path.Combine(DataLocation.gameContentPath, "Mods"), defaultFolderPath);
            Debug.Log(string.Format("TFW: Exist={0} DataLocation.gameContentPath={1}", Directory.Exists(csFolderPath), csFolderPath));
            if (Directory.Exists(csFolderPath))
            {
                return csFolderPath;
            }

            // 5. Check Cities Skylines files folder without spaces
            csFolderPath = Path.Combine(Path.Combine(DataLocation.gameContentPath, "Mods"), defaultFolderPath.Replace(" ", ""));
            Debug.Log(string.Format("TFW: Exist={0} DataLocation.gameContentPath={1}", Directory.Exists(csFolderPath), csFolderPath));
            if (Directory.Exists(csFolderPath))
            {
                return csFolderPath;
            }

            return Assets.PATH_NOT_FOUND;
        }
    }
}
