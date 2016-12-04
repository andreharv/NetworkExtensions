using System.IO;
using ColossalFramework.IO;
using ColossalFramework.PlatformServices;
using ICities;
using Transit.Framework;
using Transit.Framework.Modularity;
using UnityEngine;
using static ColossalFramework.Plugins.PluginManager;
using ColossalFramework.Plugins;
using System.Linq;
using System;

#if DEBUG
using Debug = Transit.Framework.Debug;
#endif

namespace Transit.Framework.Mod
{
    public abstract partial class TransitModBase : IUserMod
    {
        public abstract string Name { get; }

        public abstract string Description { get; }

        public abstract string Version { get; }

        public virtual string DefaultFolderPath
        {
            get { return Name; }
        }

        private string _assetPath;
        public virtual string AssetPath
        {
            get
            {
                if (_assetPath == null)
                {
                    var publishedFileID = PluginInfo.publishedFileID.AsUInt64;
                    _assetPath = GetAssetPath(DefaultFolderPath, publishedFileID);

                    if (_assetPath != Assets.PATH_NOT_FOUND)
                    {
                        Debug.Log("TFW: Mod path " + _assetPath);
                    }
                    else
                    {
                        Debug.Log("TFW: Path not found");
                    }
                }
                return _assetPath;
            }
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
        private static string GetAssetPath(string defaultFolderPath, ulong workshopId)
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
