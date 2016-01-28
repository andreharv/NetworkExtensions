using ICities;
using Transit.Framework.Hooks;
using Transit.Framework.Modularity;

namespace Transit.Framework.Mod
{
    public abstract partial class TransitModBase : LoadingExtensionBase
    {
        public virtual void OnEnabled()
        {
            HooksHandler.InstallHooksForMod(this);
            LoadModulesIfNeeded();
            LoadSettings();

            foreach (IModule module in Modules)
                module.OnEnabled();
        }

        public virtual void OnDisabled()
        {
            foreach (IModule module in Modules)
                module.OnDisabled();

            HooksHandler.UnInstallHooksForMod(this);
        }

        public override void OnCreated(ILoading loading)
        {
            foreach (IModule module in Modules)
                module.OnCreated(loading);
        }

        public override void OnReleased()
        {
            foreach (IModule module in Modules)
                module.OnReleased();
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
