using ICities;
using Transit.Addon.Core.PathFinding;
using Transit.Addon.TrafficTools.Core.PathFinding;
using Transit.Framework;
using UnityEngine;

namespace Transit.Addon.TrafficTools
{
    public partial class TrafficToolModule
    {
        private bool _isReleased = true;
        private GameObject _initializer;

        public override void OnCreated(ILoading loading)
        {
            base.OnCreated(loading);

            if (_isReleased)
            {
                PathFinder.SetType<ControledPathFinder>();

                if (AssetPath != null && AssetPath != Assets.PATH_NOT_FOUND)
                {
                    if (_initializer == null)
                    {
                        _initializer = new GameObject("TrafficTool");
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

            PathFinder.ResetToDefault();

            if (_initializer != null)
            {
                Object.Destroy(_initializer);
                _initializer = null;
            }

            _isReleased = true;
        }
    }
}
