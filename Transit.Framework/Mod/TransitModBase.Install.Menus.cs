using System;
using JetBrains.Annotations;
using Transit.Framework.Builders;
using Transit.Framework.Modularity;
using Transit.Framework.UI;

namespace Transit.Framework.Mod
{
    public partial class TransitModBase
    {
        public virtual void OnInstallMenus()
        {
            foreach (IModule module in this.GetOrCreateModules())
                module.OnInstallingMenus();
        }

        private void InstallMenus()
        {
            foreach (IModule module in this.GetOrCreateModules())
            {
                foreach (var type in module.GetType().Assembly.GetImplementations<IToolbarItemBuilder>())
                {
                    MenuManager.instance.RegisterToolbarItem(type);
                }

                foreach (var type in module.GetType().Assembly.GetImplementations<IMenuCategoryBuilder>())
                {
                    MenuManager.instance.RegisterCategory(type);
                }
            }
        }

        [UsedImplicitly]
        private class MenuInstaller : Installer<TransitModBase>
        {
            protected override bool ValidatePrerequisites()
            {
                return true;
            }

            protected override void Install(TransitModBase host)
            {
                Loading.QueueAction(() =>
                {
                    try
                    {
                        host.InstallMenus();
                    }
                    catch (Exception ex)
                    {
                        Log.Error("TFW: Crashed-MenusInstaller");
                        Log.Error("TFW: " + ex.Message);
                        Log.Error("TFW: " + ex.ToString());
                    }
                });

                Loading.QueueAction(() =>
                {
                    try
                    {
                        host.OnInstallMenus();
                    }
                    catch (Exception ex)
                    {
                        Log.Error("TFW: Crashed-MenusInstaller-Modules");
                        Log.Error("TFW: " + ex.Message);
                        Log.Error("TFW: " + ex.ToString());
                    }
                });
            }
        }
    }
}
