using ColossalFramework;
using ColossalFramework.Globalization;
using JetBrains.Annotations;
using System;
using Transit.Framework.Modularity;

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
                module.OnInstallLocalization();
        }

        [UsedImplicitly]
        private class LocalizationInstaller : Installer<TransitModBase>
        {
            private static bool Done { get; set; } //Only one localization throughout the application

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
                if (Done) //Only one localization throughout the application
                {
                    return;
                }

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

                    Done = true;
                });
            }
        }
    }
}
