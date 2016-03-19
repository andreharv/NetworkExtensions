using ICities;
using Transit.Framework.Modularity;
using UnityEngine;

namespace Transit.Framework.Mod
{
    public partial class TransitModBase
    {
        private bool _isReleased = true;
        private GameObject _container = null;

        private LocalizationInstaller _localizationInstaller = null;
        private AssetsInstaller _assetsInstaller = null;

        public override void OnCreated(ILoading loading)
        {
            base.OnCreated(loading);

            if (_isReleased)
            {
                if (AssetPath != null && AssetPath != Assets.PATH_NOT_FOUND)
                {
                    _container = new GameObject(Name.Replace(" ", ""));

                    foreach (IModule module in this.GetOrCreateModules())
                        module.OnCreated(loading);

                    _localizationInstaller = _container.AddInstallerComponent<LocalizationInstaller>();
                    _localizationInstaller.Host = this;

                    _assetsInstaller = _container.AddInstallerComponent<AssetsInstaller>();
                    _assetsInstaller.Host = this;

                    foreach (IModule module in this.GetOrCreateModules())
                        module.OnInstallingContent();
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

            foreach (IModule module in this.GetModules())
                module.OnReleased();

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

            if (_container != null)
            {
                Object.Destroy(_container);
                _container = null;
            }

            _isReleased = true;
        }
    }
}
