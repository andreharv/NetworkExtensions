using ICities;
using Transit.Framework;
using Transit.Framework.Unsafe;
using UnityEngine;

namespace Transit.Addon.TrafficPP
{
    public partial class TrafficPPModule
    {
        private bool _isReleased = true;
        private GameObject _initializer;

        public override void OnCreated(ILoading loading)
        {
            base.OnCreated(loading);

            if (_isReleased)
            {
                Redirector.PerformRedirections();

                if (AssetPath != null && AssetPath != Assets.PATH_NOT_FOUND)
                {
                    if (_initializer == null)
                    {
                        _initializer = new GameObject("TrafficPP");
                        _initializer.AddComponent<Initializer>();
                    }
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

            Redirector.RevertRedirections();

            if (_initializer != null)
            {
                Object.Destroy(_initializer);
                _initializer = null;
            }

            _isReleased = true;
        }
    }
}
