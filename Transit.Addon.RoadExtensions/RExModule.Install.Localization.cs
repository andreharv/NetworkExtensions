using System;
using System.Collections.Generic;
using System.Linq;
using ColossalFramework;
using ColossalFramework.Globalization;
using JetBrains.Annotations;
using Transit.Framework;
using Transit.Framework.Builders;
using Transit.Framework.Interfaces;
using UnityEngine;

#if DEBUG
using Debug = Transit.Framework.Debug;
#endif

namespace Transit.Addon.RoadExtensions
{
    public partial class RExModule
    {
        [UsedImplicitly]
        private class LocalizationInstaller : Installer<RExModule>
        {
            public static bool Done { get; private set; } //Only one localization throughout the application

            protected override bool ValidatePrerequisites()
            {
                var localeManager = SingletonLite<LocaleManager>.instance;
                if (localeManager == null)
                {
                    return false;
                }


                var localeField = typeof (LocaleManager).GetFieldByName("m_Locale");
                if (localeField == null)
                {
                    return false;
                }


                var locale = (Locale) localeField.GetValue(localeManager);
                if (locale == null)
                {
                    return false;
                }

                return true;
            }

            protected override void Install(RExModule host)
            {
                if (Done) //Only one localization throughout the application
                {
                    return;
                }

                Loading.QueueAction(() =>
                {
                    try
                    {
                        //Debug.Log("REx: Localization");
                        var locale = SingletonLite<LocaleManager>.instance.GetLocale();

                        locale.CreateMenuTitleLocalizedString(Menus.AdditionnalMenus.ROADS_SMALL_HV, "Small Heavy Roads");
                        locale.CreateMenuTitleLocalizedString(Menus.AdditionnalMenus.ROADS_BUSWAYS, "Buslane Roads");
                        locale.CreateMenuTitleLocalizedString(Menus.AdditionnalMenus.ROADS_PEDESTRIANS, "Pedestrian Roads");

                        var menuItemBuilders = new List<IMenuItemBuilder>();
                        menuItemBuilders.AddRange(host.Parts.OfType<IMenuItemBuilder>());
                        menuItemBuilders.AddRange(host.Parts.OfType<IMenuItemBuildersProvider>().SelectMany(mib => mib.MenuItemBuilders));

                        foreach (var builder in menuItemBuilders)
                        {
                            locale.CreateNetTitleLocalizedString(builder.Name, builder.DisplayName);
                            locale.CreateNetDescriptionLocalizedString(builder.Name, builder.Description);
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.Log("REx: Crashed-Localization");
                        Debug.Log("REx: " + ex.Message);
                        Debug.Log("REx: " + ex.ToString());
                    }

                    Done = true;
                });
            }
        }
    }
}
