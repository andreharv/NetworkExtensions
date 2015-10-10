using UnityEngine;

namespace Transit.Framework
{
    public static class GameObjectExtensions
    {
        public static T AddInstallerComponent<T>(this GameObject container)
            where T : Component, IInstaller
        {
            var installer = container.AddComponent<T>();
            installer.InstallationCompleted += () =>
            {
                if (installer != null)
                {
                    Object.Destroy(installer);
                    installer = null;
                }
            };

            return installer;
        }
    }
}
