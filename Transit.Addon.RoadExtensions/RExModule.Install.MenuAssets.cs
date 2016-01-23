using System;
using System.Linq;
using ColossalFramework.UI;
using JetBrains.Annotations;
using Transit.Addon.RoadExtensions.Menus;
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
        private class MenuAssetsInstaller : Installer
        {
            private static bool Done { get; set; } //Only one MenuAssets throughout the application

            protected override bool ValidatePrerequisites()
            {
                return true;
            }

            protected override void Install()
            {
                if (Done) //Only one MenuAssets throughout the application
                {
                    return;
                }

                Loading.QueueAction(() =>
                {
                    try
                    {
                        var atlas = AssetManager.instance.LoadAdditionnalMenusThumbnails();

                        AtlasProvider.instance.RegisterCustomAtlas(AdditionnalMenus.ROADS_TINY, atlas);
                        AtlasProvider.instance.RegisterCustomAtlas(AdditionnalMenus.ROADS_SMALL_HV, atlas);
                        AtlasProvider.instance.RegisterCustomAtlas(AdditionnalMenus.ROADS_BUSWAYS, atlas);
                        AtlasProvider.instance.RegisterCustomAtlas(AdditionnalMenus.ROADS_PEDESTRIANS, atlas);
                    }
                    catch (Exception ex)
                    {
                        Debug.Log("REx: Crashed-MenuAssetsInstaller");
                        Debug.Log("REx: " + ex.Message);
                        Debug.Log("REx: " + ex.ToString());
                    }

                    Done = true;
                });
            }
        }
    }
}
