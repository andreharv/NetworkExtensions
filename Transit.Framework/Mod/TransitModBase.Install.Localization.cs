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
        public virtual void OnInstallLocalization()
        {
            foreach (IModule module in Modules)
                module.OnInstallingLocalization();
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

                Loading.QueueAction(() =>
                {
                    try
                    {
                        host.OnInstallLocalization();
                    }
                    catch (Exception ex)
                    {
                        Debug.Log("TFW: Crashed-Localization");
                        Debug.Log("TFW: " + ex.Message);
                        Debug.Log("TFW: " + ex.ToString());
                    }
                });
            }
        }
    }
}
