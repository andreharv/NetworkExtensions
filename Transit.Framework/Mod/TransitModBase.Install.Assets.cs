using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Transit.Framework.Builders;
using Transit.Framework.Modularity;
using Transit.Framework.UI;
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
            foreach (IModule module in this.GetOrCreateModules())
                module.OnInstallingAssets();
        }

        private void InstallAtlases()
        {
            foreach (IModule module in this.GetOrCreateModules())
            {
                foreach (var type in module.GetType().Assembly.GetImplementations<IAtlasBuilder>())
                {
                    AtlasManager.instance.Include(type);
                }
            }
        }

        [UsedImplicitly]
        private class AssetInstaller : Installer<TransitModBase>
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
                            Log.Error("TFW: Crashed-AssetsInstaller");
                            Log.Error("TFW: " + ex.Message);
                            Log.Error("TFW: " + ex.ToString());
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
                        Log.Error("TFW: Crashed-AssetsInstaller-Modules");
                        Log.Error("TFW: " + ex.Message);
                        Log.Error("TFW: " + ex.ToString());
                    }
                });

                Loading.QueueAction(() =>
                {
                    try
                    {
                        host.InstallAtlases();
                    }
                    catch (Exception ex)
                    {
                        Log.Error("TFW: Crashed-AtlasInstaller");
                        Log.Error("TFW: " + ex.Message);
                        Log.Error("TFW: " + ex.ToString());
                    }
                });
            }
        }
    }
}
