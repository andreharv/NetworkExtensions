using ICities;
using Transit.Framework.Unsafe;
using UnityEngine;

namespace CSL_Traffic
{
    public class TrafficMod : LoadingExtensionBase, IUserMod
    {
        public const ulong WORKSHOP_ID = 626024868ul;

        public static OptionsManager.ModOptions Options = OptionsManager.ModOptions.None;
        private static OptionsManager sm_optionsManager;

        private GameObject m_initializer;
        private bool m_redirectionInstalled = false;

        public string Name
        {
            get { return "Traffic++ V2"; }
        }

        public string Description
        {
            get { return "Adds transit routing and restriction features."; }
        }

        public void OnSettingsUI(UIHelperBase helper)
        {
            if (sm_optionsManager == null)
                sm_optionsManager = new GameObject("OptionsManager").AddComponent<OptionsManager>();

            sm_optionsManager.CreateSettings(helper);
        }

        public override void OnCreated(ILoading loading)
        {
            base.OnCreated(loading);

            if (sm_optionsManager != null)
            {
                sm_optionsManager.LoadOptions();
            }

            if (m_initializer == null)
            {
                m_initializer = new GameObject("CSL-Traffic Custom Prefabs");
                m_initializer.AddComponent<Initializer>();
            }
        }

        public override void OnLevelUnloading()
        {
            base.OnLevelUnloading();

            if (m_initializer != null)
                m_initializer.GetComponent<Initializer>().OnLevelUnloading();
        }

        public override void OnReleased()
        {
            base.OnReleased();

            if (m_initializer != null)
            {
                GameObject.Destroy(m_initializer);
            }
        }

        public void OnEnabled()
        {
            if (!m_redirectionInstalled)
            {
                Redirector.PerformRedirections();
                m_redirectionInstalled = true;
            }
        }

        public void OnDisabled()
        {
            if (m_redirectionInstalled)
            {
                Redirector.RevertRedirections();
                m_redirectionInstalled = false;
            }
        }
    }
}
