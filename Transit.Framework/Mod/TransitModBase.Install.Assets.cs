using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Transit.Framework.Builders;
using Transit.Framework.Modularity;
using UnityEngine;

#if DEBUG
using Transit.Framework;
#endif

namespace Transit.Framework.Mod
{
    public partial class TransitModBase
    {
        public virtual void OnInstallAssets()
        {
            foreach (IModule module in Modules)
                module.OnInstallingAssets();
        }

        private void InstallAtlas()
        {
            var atlasBuilderType = typeof(IAtlasBuilder);

            var atlasBuilderTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a =>
                {
                    try
                    {
                        return a.GetTypes();
                    }
                    catch (Exception ex)
                    {
                        Debug.Log("TFW: InstallAtlas looking into assembly " + a.FullName);
                        Debug.Log("TFW: " + ex.Message);
                        Debug.Log("TFW: " + ex.ToString());
                        return new Type[] {};
                    }
                })
                .Where(t => !t.IsAbstract && !t.IsInterface)
                .Where(t => atlasBuilderType.IsAssignableFrom(t));

            foreach (var type in atlasBuilderTypes)
            {
                AtlasManager.instance.Include(type);
            }
        }

        [UsedImplicitly]
        private class AssetsInstaller : Installer<TransitModBase>
        {
            private static readonly ICollection<string> _pathsLoaded = new HashSet<string>(); // Only one Assets installation per paths throughout the application

            protected override bool ValidatePrerequisites()
            {
                return true;
            }

            protected override void Install(TransitModBase host)
            {
                if (_pathsLoaded.Contains(host.AssetPath)) // Only one Assets installation per paths throughout the application
                {
                    return;
                }

                _pathsLoaded.Add(host.AssetPath);

                foreach (var action in AssetManager.instance.CreateLoadingSequence(host.AssetPath))
                {
                    var localAction = action;

                    Loading.QueueAction(() =>
                    {
                        try
                        {
                            localAction();
                        }
                        catch (Exception ex)
                        {
                            Debug.Log("TFW: Crashed-AssetsInstaller");
                            Debug.Log("TFW: " + ex.Message);
                            Debug.Log("TFW: " + ex.ToString());
                        }
                    });
                }

                Loading.QueueAction(() =>
                {
                    try
                    {
                        host.OnInstallAssets();
                    }
                    catch (Exception ex)
                    {
                        Debug.Log("TFW: Crashed-AssetsInstaller-Modules");
                        Debug.Log("TFW: " + ex.Message);
                        Debug.Log("TFW: " + ex.ToString());
                    }
                });

                Loading.QueueAction(() =>
                {
                    try
                    {
                        host.InstallAtlas();
                    }
                    catch (Exception ex)
                    {
                        Debug.Log("TFW: Crashed-AtlasInstaller");
                        Debug.Log("TFW: " + ex.Message);
                        Debug.Log("TFW: " + ex.ToString());
                    }
                });
            }
        }
    }
}
