using Transit.Framework;
using Transit.Addon.RoadExtensions.Install;
using ICities;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Transit.Addon.RoadExtensions
{
    public partial class RExModule
    {
        private bool _isReleased = true;
        private GameObject _container = null;
        private NetCollection _roads = null;

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
                if (GetPath() != PATH_NOT_FOUND)
                {
                    _container = new GameObject(REX_OBJECT_NAME);

                    _initializer = _container.AddComponent<Initializer>();
                    _initializer.InstallationCompleted += InitializationCompleted;
                }

                _isReleased = false;
            }
        }

        private void InitializationCompleted()
        {
            Loading.QueueAction(() =>
            {
                if (_initializer != null)
                {
                    Object.Destroy(_initializer);
                    _initializer = null;
                }

                if (_container != null)
                {
                    _localizationInstaller = _container.AddComponent<LocalizationInstaller>();
                    _localizationInstaller.Host = this;
                    _localizationInstaller.InstallationCompleted += LocInstallationCompleted;

                    _assetsInstaller = _container.AddComponent<AssetsInstaller>();
                    _assetsInstaller.InstallationCompleted += AssetsInstallationCompleted;

                    _roadsInstaller = _container.AddComponent<RoadsInstaller>();
                    _roadsInstaller.Host = this;
                    _roadsInstaller.InstallationCompleted += RoadsInstallationCompleted;
                }
            });
        }

        private void LocInstallationCompleted()
        {
            Loading.QueueAction(() =>
            {
                if (_localizationInstaller != null)
                {
                    _localizationInstaller.Host = null;
                    Object.Destroy(_localizationInstaller);
                    _localizationInstaller = null;
                }
            });
        }

        private void AssetsInstallationCompleted()
        {
            Loading.QueueAction(() =>
            {
                if (_assetsInstaller != null)
                {
                    Object.Destroy(_assetsInstaller);
                    _assetsInstaller = null;
                }
            });
        }

        private void RoadsInstallationCompleted()
        {
            Loading.QueueAction(() =>
            {
                if (_roadsInstaller != null)
                {
                    _roadsInstaller.Host = null;
                    Object.Destroy(_roadsInstaller);
                    _roadsInstaller = null;
                }
            });
        }

        private void MenusInstallationCompleted()
        {
            Loading.QueueAction(() =>
            {
                if (_menusInstaller != null)
                {
                    Object.Destroy(_menusInstaller);
                    _menusInstaller = null;
                }
            });
        }

        public override void OnLevelLoaded(LoadMode mode)
        {
            base.OnLevelLoaded(mode);

            if (_container != null && _menusInstaller == null)
            {
                _menusInstaller = _container.AddComponent<MenusInstaller>();
                _menusInstaller.InstallationCompleted += MenusInstallationCompleted;
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

            if (_container != null)
            {
                Object.Destroy(_container);
                _container = null;
            }

            _isReleased = true;
        }
    }
}
