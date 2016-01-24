using ICities;
using Transit.Framework.Modularity;
using UnityEngine;

namespace Transit.Addon.Core
{
    public abstract partial class TransitModBase : LoadingExtensionBase
    {
        public virtual void OnEnabled()
        {
            InstallPrerequisites();
            LoadModulesIfNeeded();
            LoadSettings();

            foreach (IModule module in Modules)
                module.OnEnabled();
        }

        public virtual void OnDisabled()
        {
            foreach (IModule module in Modules)
                module.OnDisabled();

            UninstallPrerequisites();
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
