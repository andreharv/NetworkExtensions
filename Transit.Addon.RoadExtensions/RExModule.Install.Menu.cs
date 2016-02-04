using System;
using System.Collections.Generic;
using System.Linq;
using ColossalFramework.UI;
using JetBrains.Annotations;
using Transit.Framework.ExtensionPoints.UI;
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
        private class MenuInstaller : Installer<RExModule>
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

                RoadCategoryOrderManager.RegisterCategory(RExExtendedMenus.ROADS_TINY, 5);
                RoadCategoryOrderManager.RegisterCategory(RExExtendedMenus.ROADS_SMALL_HV, 20);
                RoadCategoryOrderManager.RegisterCategory(RExExtendedMenus.ROADS_BUSWAYS, 65);
                RoadCategoryOrderManager.RegisterCategory(RExExtendedMenus.ROADS_PEDESTRIANS, 75);

                var categories = host.Parts
                    .OfType<IMenuItemBuilder>()
                    .WhereActivated()
                    .Select(mib => mib.UICategory)
                    .Where(cat => !string.IsNullOrEmpty(cat))
                    .Distinct()
                    .ToArray();
                
                foreach (var cat in categories)
                {
                    ExtendedMenuManager.RegisterNewCategory(cat, GeneratedGroupPanel.GroupFilter.Net, ItemClass.Service.Road);
                }
                
                Loading.QueueAction(() =>
                {
                    try
                    {
                        var atlas = AssetManager.instance.LoadAdditionnalMenusThumbnails();

                        AtlasProvider.RegisterCustomAtlas(RExExtendedMenus.ROADS_TINY, atlas);
                        AtlasProvider.RegisterCustomAtlas(RExExtendedMenus.ROADS_SMALL_HV, atlas);
                        AtlasProvider.RegisterCustomAtlas(RExExtendedMenus.ROADS_BUSWAYS, atlas);
                        AtlasProvider.RegisterCustomAtlas(RExExtendedMenus.ROADS_PEDESTRIANS, atlas);
                    }
                    catch (Exception ex)
                    {
                        Debug.Log("REx: Crashed-MenuInstaller");
                        Debug.Log("REx: " + ex.Message);
                        Debug.Log("REx: " + ex.ToString());
                    }

                    Done = true;
                });
            }
        }
    }
}
