using ICities;
using UnityEngine;

namespace Transit.Addon.TrafficPP
{
    public partial class TrafficPPModule
    {
        public static OptionsManager.ModOptions Options = OptionsManager.ModOptions.None;
        internal static OptionsManager sm_optionsManager;

        private bool _isReleased = true;
        private GameObject _initializer;

        public override void OnSettingsUI(UIHelperBase helper)
        {
            if (sm_optionsManager == null)
                sm_optionsManager = new GameObject("OptionsManager").AddComponent<OptionsManager>();

            sm_optionsManager.CreateSettings(helper);
        }

        public override void OnCreated(ILoading loading)
        {
            base.OnCreated(loading);

            if (sm_optionsManager != null)
            {
                sm_optionsManager.LoadOptions();
            }

            if (_isReleased)
            {
                if (_initializer == null)
                {
                    _initializer = new GameObject("CSL-Traffic Custom Prefabs");
                    _initializer.AddComponent<Initializer>();
                }

                _isReleased = false;
            }
        }

        public override void OnReleased()
        {
            base.OnReleased();

            if (_isReleased)
            {
                return;
            }
            
            if (_initializer != null)
            {
                Object.Destroy(_initializer);
                _initializer = null;
            }

            _isReleased = true;
        }
    }
}
