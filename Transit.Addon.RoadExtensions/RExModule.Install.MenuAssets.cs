using System;
using System.Collections.Generic;
using System.Linq;
using ColossalFramework.UI;
using JetBrains.Annotations;
using Transit.Addon.Core.Extenders.UI;
using Transit.Addon.RoadExtensions.Menus;
using Transit.Framework;
using Transit.Framework.Builders;
using UnityEngine;

#if DEBUG
using Debug = Transit.Framework.Debug;
#endif

namespace Transit.Addon.RoadExtensions
{
    public partial class RExModule
    {
        [UsedImplicitly]
        private class MenuAssetsInstaller : Installer<RExModule>
        {
            private static bool Done { get; set; } //Only one MenuAssets throughout the application

            protected override bool ValidatePrerequisites()
            {
                return true;
            }

            protected override void Install(RExModule host)
            {
                if (Done) //Only one MenuAssets throughout the application
                {
                    return;
                }

                var categories = host.Parts
                    .OfType<IMenuItemBuilder>()
                    .Select(mib => mib.UICategory)
                    .Where(cat => !string.IsNullOrEmpty(cat))
                    .Distinct()
                    .ToArray();
                
                foreach (var cat in categories)
                {
                    ExtendedMenuProvider.instance.RegisterNewCategory(cat, GeneratedGroupPanel.GroupFilter.Net);
                }
                
                Loading.QueueAction(() =>
                {
                    try
                    {
                        var atlas = AssetManager.instance.LoadAdditionnalMenusThumbnails();

                        foreach (var cat in categories)
                        {
                            AtlasProvider.instance.RegisterCustomAtlas(cat, atlas);
                        }
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
