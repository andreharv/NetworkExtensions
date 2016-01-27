using ColossalFramework;
using ColossalFramework.Packaging;
using ColossalFramework.Steamworks;
using ICities;
using Transit.Framework.Modularity;

namespace Transit.Addon
{
    public partial class Mod : LoadingExtensionBase
    {
        private bool _loadTriggered = false;

        public void OnGameLoaded()
        {
            if (_loadTriggered)  // This method will be called everytime the mod is refreshed
                return;          // so we must use this bool to ensure we only call it the first time

            _loadTriggered = true;

            //CheckForUpdates();
            ShowNotification();

            foreach (IModule module in Modules)
                module.OnGameLoaded();
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
