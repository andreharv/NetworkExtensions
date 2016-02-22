using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using ColossalFramework.Packaging;
using ColossalFramework.Plugins;
using ColossalFramework.UI;
using ICities;
using Transit.Framework;
using Transit.Framework.Modularity;
using Transit.Framework.Texturing;
using UnityEngine;

namespace AssetPackager
{
    public class AssetPackerMod : LoadingExtensionBase, IUserMod
    {
        public string Name { get { return "AssetPackager For TAM"; } }
        public string Description { get { return "Devlopment Tool"; } }

        public void OnSettingsUI(UIHelperBase helper)
        {
            UIHelperBase group = helper.AddGroup("AssetPackager Options");

            try
            {
                group.AddButton("Pack data", PackData);
            }
            catch (Exception ex)
            {
                Debug.Log("APM: Crashed-PackData");
                Debug.Log("APM: " + ex.Message);
                Debug.Log("APM: " + ex.ToString());
            }
        }

        public void PackData()
        {
            Debug.Log("APM: Loading Assets...");
            var path = PluginManager.instance.GetPluginsInfo().First(p => p.publishedFileID.AsUInt64 == 478820060).modPath;
            var assets = AssetManager
                .instance
                .LoadAllAssets(path)
                .Where(a => a.Type == AssetType.Mesh)
                .ToArray();
            Debug.Log("APM: Assets loaded...");

            //new Thread(() =>
            //{
                try
                {
                    PackDataInternal(assets);
                }
                catch (Exception ex)
                {
                    Debug.Log("APM: Crashed-PackData");
                    Debug.Log("APM: " + ex.Message);
                    Debug.Log("APM: " + ex.ToString());
                }
            //}).Start();
        }

        private void PackDataInternal(ICollection<AssetInfo> assets)
        {
            Package package = new Package("TAM.Meshes");

            for (int i = 0; i < assets.Count(); i++)
            {
                var assetInfo = assets.ElementAt(i);
                var name = "";

                switch (assetInfo.Type)
                {
                    case AssetType.Texture:
                        name = assetInfo.RelativePath.ToLowerInvariant().TrimEnd(".png");
                        break;
                    case AssetType.Mesh:
                        name = assetInfo.RelativePath.ToLowerInvariant().TrimEnd(".obj");
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                name = "TAM/" + name.Replace("\\", "/");
                
                Debug.Log(string.Format("{0}/{1} APM: Packing asset {2}...", i + 1, assets.Count(), name));

                switch (assetInfo.Type)
                {
                    case AssetType.Texture:
                        var tex = ((byte[]) assetInfo.Asset).AsTexture(name, TextureType.Default);
                        tex.Compress(true);

                        package.AddAsset(name, tex);
                        break;
                    case AssetType.Mesh:
                        package.AddAsset(name, (Mesh)assetInfo.Asset);
                        break;
                }
            }

            package.Save("Meshes.crp");
        }
    }
}
