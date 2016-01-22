using ICities;
using Transit.Addon.Core;
using Transit.Framework.Modularity;
using Transit.Framework.Unsafe;

namespace Transit.Addon
{
    public partial class Mod
    {
        //private bool _loadTriggered = false;
        private bool _isReleased = true;

        //public void OnGameLoaded()
        //{
        //    if (_loadTriggered)  // This method will be called everytime the mod is refreshed
        //        return;          // so we must use this bool to ensure we only call it the first time

        //    _loadTriggered = true;

        //    //CheckForUpdates();
        //    ShowNotification();

        //    foreach (IModule module in Modules)
        //        module.OnGameLoaded();
        //}

        public override void OnCreated(ILoading loading)
        {
            if (_isReleased)
            {
                Redirector.PerformRedirections();

                _isReleased = false;
            }

            base.OnCreated(loading);
        }

        public override void OnReleased()
        {
            base.OnReleased();

            if (_isReleased)
            {
                return;
            }

            Redirector.RevertRedirections();

            _isReleased = true;
        }
    }
}
