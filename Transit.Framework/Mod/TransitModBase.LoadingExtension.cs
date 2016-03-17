using ICities;
using Transit.Framework.Modularity;
using Transit.Framework.Prerequisites;

namespace Transit.Framework.Mod
{
    public abstract partial class TransitModBase : LoadingExtensionBase
    {
        public virtual void OnEnabled()
        {
            this.InstallPrerequisites();
            ModuleManager.instance.RegisterModules(this);
            LoadSettings();

            foreach (IModule module in Modules)
            {
                if (!module.IsEnabled)
                {
                    module.OnEnabled();
                }
            }
        }

        public virtual void OnDisabled()
        {
            foreach (IModule module in ModuleManager.instance.GetModules(this))
            {
                if (module.IsEnabled)
                {
                    module.OnDisabled();
                }
            }

            ModuleManager.instance.TryReleaseModules(this);
            this.UninstallPrerequisites();
        }

        public override void OnLevelUnloading()
        {
            foreach (IModule module in Modules)
                module.OnLevelUnloading();
        }

        public override void OnLevelLoaded(LoadMode mode)
        {
            foreach (IModule module in Modules)
                module.OnLevelLoaded(mode);
        }
    }
}
