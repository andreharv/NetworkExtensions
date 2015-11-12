using ICities;
using Transit.Framework;
using UnityEngine;
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
        private MenusInstaller _menusInstaller = null;

        public override void OnCreated(ILoading loading)
        {
            base.OnCreated(loading);

            if (_isReleased)
            {
                if (Mod.GetPath() != Assets.PATH_NOT_FOUND)
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

                _roadsInstaller = _container.AddInstallerComponent<RoadsInstaller>();
                _roadsInstaller.Host = this;
            }
        }

        public override void OnLevelLoaded(LoadMode mode)
        {
            base.OnLevelLoaded(mode);

            if (_container != null && _menusInstaller == null)
            {
                _menusInstaller = _container.AddInstallerComponent<MenusInstaller>();
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

            if (_roadsInstaller != null)
            {
                Object.Destroy(_roadsInstaller);
                _roadsInstaller = null;
            }

            if (_menusInstaller != null)
            {
                Object.Destroy(_menusInstaller);
                _menusInstaller = null;
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
