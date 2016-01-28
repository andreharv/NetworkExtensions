using System;
using System.Collections.Generic;
using System.Linq;
using ICities;
using Transit.Addon.Core.Extenders.AI;
using Transit.Addon.Core.Extenders.UI;
using Transit.Addon.RoadExtensions.AI;
using Transit.Addon.RoadExtensions.Menus;
using Transit.Framework;
using Transit.Framework.Unsafe;
using Transit.Framework.Builders;
using Transit.Framework.Modularity;
using UnityEngine;
using Transit.Addon.RoadExtensions.Roads.TinyRoads.Alley2L;
using Transit.Addon.RoadExtensions.Roads.TinyRoads.OneWay1L;
using Object = UnityEngine.Object;

namespace Transit.Addon.RoadExtensions
{
    public partial class RExModule
    {
        private bool _isReleased = true;
        private GameObject _container = null;
        private NetCollection _roads = null;
        private PropCollection _props = null;

        private Initializer _initializer = null;
        private LocalizationInstaller _localizationInstaller = null;
        private AssetsInstaller _assetsInstaller = null;
        private RoadsInstaller _roadsInstaller = null;
        private MenuInstaller _menuInstaller = null;

        private IEnumerable<Action> _lateOperations;

        public override void OnCreated(ILoading loading)
        {
            base.OnCreated(loading);

            if (_isReleased)
            {
                ZoneBlocksCreatorProvider.instance.RegisterCustomCreator<TinyRoadZoneBlocksCreator>(Alley2LBuilder.NAME);
                ZoneBlocksCreatorProvider.instance.RegisterCustomCreator<TinyRoadZoneBlocksCreator>(OneWay1LBuilder.NAME);

                if (AssetPath != null && AssetPath != Assets.PATH_NOT_FOUND)
                {
                    _container = new GameObject(REX_OBJECT_NAME);

                    _initializer = _container.AddInstallerComponent<Initializer>();
                    _initializer.InstallationCompleted += InitializationCompleted;
                }

                _isReleased = false;
            }
        }

        private void InitializationCompleted()
        {
            if (_container != null)
            {
                _localizationInstaller = _container.AddInstallerComponent<LocalizationInstaller>();
                _localizationInstaller.Host = this;

                _assetsInstaller = _container.AddInstallerComponent<AssetsInstaller>();
                _assetsInstaller.Host = this;

                _menuInstaller = _container.AddInstallerComponent<MenuInstaller>();
                _menuInstaller.Host = this;

                _roadsInstaller = _container.AddInstallerComponent<RoadsInstaller>();
                _roadsInstaller.Host = this;
            }
        }
		
        public override void OnLevelLoaded(LoadMode mode)
        {
            base.OnLevelLoaded(mode);

            if (_container != null && _menuInstaller == null)
            {
                _menuInstaller = _container.AddInstallerComponent<MenuInstaller>();
            }

            if (_lateOperations != null)
            {
                foreach (var op in _lateOperations)
                {
                    op();
                }

                _lateOperations = null;
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

            if (_localizationInstaller != null)
            {
                Object.Destroy(_localizationInstaller);
                _localizationInstaller = null;
            }

            if (_assetsInstaller != null)
            {
                Object.Destroy(_assetsInstaller);
                _assetsInstaller = null;
            }

            if (_menuInstaller != null)
            {
                Object.Destroy(_menuInstaller);
                _menuInstaller = null;
            }

            if (_roadsInstaller != null)
            {
                Object.Destroy(_roadsInstaller);
                _roadsInstaller = null;
            }

            if (_roads != null)
            {
                Object.Destroy(_roads);
                _roads = null;
            }

            if (_props != null)
            {
                Object.Destroy(_props);
                _props = null;
            }

            if (_container != null)
            {
                Object.Destroy(_container);
                _container = null;
            }

            _isReleased = true;
        }
    }
}
