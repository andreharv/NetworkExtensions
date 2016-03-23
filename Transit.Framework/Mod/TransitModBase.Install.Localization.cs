using ColossalFramework;
using ColossalFramework.Globalization;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using Transit.Framework.Modularity;
using UnityEngine;

#if DEBUG
using Transit.Framework;
#endif

namespace Transit.Framework.Mod
{
    public partial class TransitModBase
    {
        public virtual void OnInstallLocalization(Locale locale)
        {
            foreach (IModule module in this.GetOrCreateModules())
                module.OnInstallingLocalization(locale);
        }

        [UsedImplicitly]
        private class LocalizationInstaller : Installer<TransitModBase>
        {
            private static readonly ICollection<ulong> _modLoaded = new HashSet<ulong>(); //Only one localization per mod throughout the application

            protected override bool ValidatePrerequisites()
            {
                var localeManager = SingletonLite<LocaleManager>.instance;
                if (localeManager == null)
                {
                    return false;
                }

                var locale = localeManager.GetLocale();
                if (locale == null)
                {
                    return false;
                }

                return true;
            }

            protected override void Install(TransitModBase host)
            {
                if (_modLoaded.Contains(host.WorkshopId)) //Only one localization per mod throughout the application
                {
                    return;
                }

                _modLoaded.Add(host.WorkshopId);

                var locale = SingletonLite<LocaleManager>.instance.GetLocale();

                Loading.QueueAction(() =>
                {
                    try
                    {
                        MenuManager.instance.InstallLocalization(locale);
                    }
                    catch (Exception ex)
                    {
                        Log.Error("TFW: Crashed-Menu Localization");
                        Log.Error("TFW: " + ex.Message);
                        Log.Error("TFW: " + ex.ToString());
                    }
                });

                Loading.QueueAction(() =>
                {
                    try
                    {
                        host.OnInstallLocalization();
                    }
                    catch (Exception ex)
                    {
                        Log.Error("TFW: Crashed-Localization");
                        Log.Error("TFW: " + ex.Message);
                        Log.Error("TFW: " + ex.ToString());
                    }
                });
            }
        }
    }
}
