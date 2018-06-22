using System;
using JetBrains.Annotations;
using Transit.Framework.Modularity;
using UnityEngine;
using Transit.Framework;

#if DEBUG
using Debug = Transit.Framework.Debug;
#endif

namespace TransitPlus.Framework.Mod
{
    public partial class TransitModBase
    {
        public virtual void OnInstallAssets()
        {
            foreach (IModule module in Modules)
                module.OnInstallingAssets();
        }

        [UsedImplicitly]
        private class AssetsInstaller : Installer<TransitModBase>
        {
            private static bool Done { get; set; } // Only one Assets installation throughout the application

            protected override bool ValidatePrerequisites()
            {
                return true;
            }

            protected override void Install(TransitModBase host)
            {
                if (Done) // Only one Assets installation throughout the application
                {
                    return;
                }
                
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

                Done = true;
            }
        }
    }
}
