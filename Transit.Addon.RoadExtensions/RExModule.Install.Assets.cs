using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Transit.Framework;
using UnityEngine;

#if DEBUG
using Debug = Transit.Framework.Debug;
#endif


namespace Transit.Addon.RoadExtensions
{
    public partial class RExModule
    {
        [UsedImplicitly]
        private class AssetsInstaller : Installer<RExModule>
        {
            public static bool Done { get; private set; } // Only one Assets installation throughout the application

            protected override bool ValidatePrerequisites()
            {
                return true;
            }

            protected override void Install(RExModule host)
            {
                if (Done) // Only one Assets installation throughout the application
                {
                    return;
                }

                var sequence = new List<Action>();
                sequence.AddRange(AssetManager.instance.CreatePackageLoadingSequence("TAM.Meshes", "TAM/"));
                sequence.AddRange(AssetManager.instance.CreateLoadingSequence(host.AssetPath));

                foreach (var action in sequence)
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
                            Debug.Log("REx: Crashed-AssetsInstaller");
                            Debug.Log("REx: " + ex.Message);
                            Debug.Log("REx: " + ex.ToString());
                        }
                    });
                }

                Done = true;
            }
        }
    }
}
