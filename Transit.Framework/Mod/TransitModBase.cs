using System.IO;
using ColossalFramework.IO;
using ColossalFramework.Steamworks;
using ICities;
using Transit.Framework;
using Transit.Framework.Modularity;
using UnityEngine;

#if DEBUG
using Debug = Transit.Framework.Debug;
#endif

namespace Transit.Framework.Mod
{
    public abstract partial class TransitModBase : IUserMod
    {
        public abstract ulong WorkshopId { get; }

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
                    _assetPath = GetAssetPath(DefaultFolderPath, WorkshopId);

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
