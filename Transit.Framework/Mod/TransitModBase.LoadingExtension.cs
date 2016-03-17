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
            this.RegisterModules();
            LoadSettings();

            foreach (IModule module in this.GetOrCreateModules())
            {
                if (!module.IsEnabled)
                {
                    module.OnEnabled();
                }
            }
        }

        public virtual void OnDisabled()
        {
            foreach (IModule module in this.GetModules())
            {
                if (module.IsEnabled)
                {
                    module.OnDisabled();
                }
            }

            this.TryReleaseModules();
            this.UninstallPrerequisites();
        }

        public override void OnLevelUnloading()
        {
            foreach (IModule module in this.GetOrCreateModules())
                module.OnLevelUnloading();
        }

        public override void OnLevelLoaded(LoadMode mode)
        {
            foreach (IModule module in this.GetOrCreateModules())
                module.OnLevelLoaded(mode);
        }
    }
}
