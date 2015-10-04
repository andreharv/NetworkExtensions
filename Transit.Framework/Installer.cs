using UnityEngine;

#if DEBUG
using Debug = Transit.Framework.Debug;
#endif

namespace Transit.Framework
{
    public delegate void InstallationCompletedEventHandler();

    public abstract class Installer : MonoBehaviour
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
                    InstallationCompleted();
                }
            }
        }

        protected abstract bool ValidatePrerequisites();
        protected abstract void Install();
    }
}
