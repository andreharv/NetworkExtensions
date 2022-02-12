using UnityEngine;

#if DEBUG
using Debug = Transit.Framework.Debug;
#endif

namespace Transit.Framework
{
    public delegate void InstallationCompletedEventHandler();

    public interface IInstaller
    {
        event InstallationCompletedEventHandler InstallationCompleted;
    }

    public abstract class Installer : MonoBehaviour, IInstaller
    {
        public event InstallationCompletedEventHandler InstallationCompleted;

        private bool _doneWithInstall = false;

        private void Awake()
        {
            DontDestroyOnLoad(this);
        }

        private void Update()
        {
            if (!_doneWithInstall)
            {
                UpdateInternal();
            }
        }

        private void UpdateInternal()
        {
            if (!_doneWithInstall)
            {
                if (ValidatePrerequisites())
                {
                    Install();
                    Debug.Log(string.Format("TFW: {0} completed", GetType().Name));

                    _doneWithInstall = true;
                }
            }

            if (_doneWithInstall)
            {
                if (InstallationCompleted != null)
                {
                    Loading.QueueAction(() => InstallationCompleted());
                }
            }
        }

        protected abstract bool ValidatePrerequisites();
        protected abstract void Install();
    }

    public abstract class Installer<THost> : MonoBehaviour, IInstaller
    {
        public THost Host { get; set; }

        public event InstallationCompletedEventHandler InstallationCompleted;

        private bool _doneWithInstall = false;

        private void Awake()
        {
            DontDestroyOnLoad(this);
        }

        private void Update()
        {
            if (!_doneWithInstall)
            {
                UpdateInternal();
            }
        }

        private void UpdateInternal()
        {
            if (!_doneWithInstall)
            {
                if (ValidatePrerequisites())
                {
                    Install(Host); // Host is copyied locally to the function, the current object will be destroyed
                    Debug.Log(string.Format("TFW: {0} completed", GetType().Name));

                    _doneWithInstall = true;
                }
            }

            if (_doneWithInstall)
            {
                if (InstallationCompleted != null)
                {
                    Loading.QueueAction(() => InstallationCompleted());
                }
            }
        }

        protected abstract bool ValidatePrerequisites();
        protected abstract void Install(THost host);
    }
}
