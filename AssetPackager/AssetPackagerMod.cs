using System;
using System.IO;
using System.Linq;
using ColossalFramework.Packaging;
using ColossalFramework.Plugins;
using ICities;
using Transit.Framework;
using Transit.Framework.Texturing;
using UnityEngine;

namespace AssetPackager
{
    public class AssetPackerMod : LoadingExtensionBase, IUserMod
    {
        public string Name { get { return "AssetPackager For TAM"; } }
        public string Description { get { return "Devlopment Tool"; } }

        private void Go()
        {
            var path = PluginManager.instance.GetPluginsInfo().First(p => p.publishedFileID.AsUInt64 == 478820060).modPath;
            
            Package package = new Package("TAM");
            foreach (var assetInfo in AssetManager.instance.LoadAllAssets(path))
            {
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

                switch (assetInfo.Type)
                {
                    case AssetType.Texture:
                        package.AddAsset(name, ((byte[])assetInfo.Asset).AsTexture(name, TextureType.Default), true);
                        break;
                    case AssetType.Mesh:
                        package.AddAsset(name, (Mesh)assetInfo.Asset, true);
                        break;
                }
            }

            package.Save("TAMAssetPackage");
        }

        public void OnEnabled()
        {
            try
            {
                Go();
            }
            catch (Exception ex)
            {
                Debug.Log("APM: Crashed-CreateZoneBlocks");
                Debug.Log("APM: " + ex.Message);
                Debug.Log("APM: " + ex.ToString());
            }
        }
    }
}
