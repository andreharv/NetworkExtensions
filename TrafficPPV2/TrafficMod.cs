using ICities;
using Transit.Framework.Unsafe;
using UnityEngine;

namespace CSL_Traffic
{
    public class TrafficMod : LoadingExtensionBase, IUserMod
    {
        public const ulong WORKSHOP_ID = 626024868ul;

        public static OptionsManager.ModOptions Options = OptionsManager.ModOptions.RoadCustomizerTool | OptionsManager.ModOptions.NoDespawn;
        private static readonly OptionsManager sm_optionsManager = new OptionsManager();
        private GameObject m_initializer;

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
            sm_optionsManager.CreateSettings(helper);
        }

        public override void OnCreated(ILoading loading)
        {
            base.OnCreated(loading);

            if (m_initializer == null)
            {
                Redirector.PerformRedirections();
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
                Redirector.RevertRedirections();
            }
        }

        public void OnEnabled()
        {
            sm_optionsManager.LoadOptions();
        }
    }
}
