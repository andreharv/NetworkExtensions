using ICities;
using Transit.Framework.Modularity;

namespace Transit.Addon
{
    public partial class Mod : LoadingExtensionBase
    {
        private bool _isGameLoaded = false;

        public void OnGameLoaded()
        {
            if (_isGameLoaded)  // This method will be called everytime the mod is refreshed
                return;         // so we must use this bool to ensure we only call it the first time

            CheckForUpdates();

            foreach (IModule module in Modules)
                module.OnGameLoaded();

            _isGameLoaded = true;
        }

        public override void OnCreated(ILoading loading)
        {
            foreach (IModule module in Modules)
                module.OnCreated(loading);
        }

        public override void OnLevelLoaded(LoadMode mode)
        {
            foreach (IModule module in Modules)
                module.OnLevelLoaded(mode);
        }

        public override void OnLevelUnloading()
        {
            foreach (IModule module in Modules)
                module.OnLevelUnloading();
        }

        public override void OnReleased()
        {
            foreach (IModule module in Modules)
                module.OnReleased();
        }

        public void OnEnabled()
        {
            foreach (IModule module in Modules)
                module.OnEnabled();
        }

        public void OnDisabled()
        {
            foreach (IModule module in Modules)
                module.OnDisabled();
        }

    }
}
