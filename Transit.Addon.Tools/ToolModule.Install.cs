﻿using ICities;
using Transit.Addon.Tools.Core;
using Transit.Framework.ExtensionPoints.PathFinding;
using Transit.Framework;
using UnityEngine;

namespace Transit.Addon.Tools
{
    public partial class ToolModule
    {
        private bool _isReleased = true;
        private GameObject _initializer;

        public override void OnCreated(ILoading loading)
        {
            base.OnCreated(loading);

            if (_isReleased)
            {
                PathFindingManager.instance.DefineCustomLaneRouting(LanesManager.instance);
                PathFindingManager.instance.DefineCustomLaneSpeed(LanesManager.instance);

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

            PathFindingManager.instance.DisableCustomLaneRouting();
            PathFindingManager.instance.DisableCustomLaneSpeed();

            if (_initializer != null)
            {
                Object.Destroy(_initializer);
                _initializer = null;
            }

            _isReleased = true;
        }
    }
}