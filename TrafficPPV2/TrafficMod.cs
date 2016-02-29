using System;
using System.Collections.Generic;
using ColossalFramework;
using ColossalFramework.Globalization;
using ICities;
using Transit.Framework;
using Transit.Framework.Redirection;
using UnityEngine;

namespace CSL_Traffic
{
    public class TrafficMod : LoadingExtensionBase, IUserMod
    {
        public const ulong WORKSHOP_ID = 626024868ul;

        public static OptionsManager.ModOptions Options = OptionsManager.ModOptions.RoadCustomizerTool | OptionsManager.ModOptions.NoDespawn;
        private static readonly OptionsManager sm_optionsManager = new OptionsManager();
        private GameObject m_initializer;
        private static bool sm_redirectionInstalled = false;
        private static bool sm_localizationInitialized = false;

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

        public override void OnLevelLoaded(LoadMode mode)
        {
            base.OnLevelLoaded(mode);

            InstallLocalization();
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
            sm_optionsManager.LoadOptions();
            if (!sm_redirectionInstalled)
            {
                Redirector.PerformRedirections();
                sm_redirectionInstalled = true;
            }

            ExtendedPathManager.DefinePathFinding<TPPPathFind>();
        }

        public void OnDisabled()
        {
            if (sm_redirectionInstalled)
            {
                Redirector.RevertRedirections();
                sm_redirectionInstalled = false;
            }

            ExtendedPathManager.ResetPathFinding();
        }

        private void InstallLocalization()
        {
            if (sm_localizationInitialized)
                return;

            Logger.LogInfo("Updating Localization.");

            try
            {
                // Localization
                Locale locale = (Locale)typeof(LocaleManager).GetFieldByName("m_Locale").GetValue(SingletonLite<LocaleManager>.instance);
                if (locale == null)
                    throw new KeyNotFoundException("Locale is null");

                // Road Customizer Tool Advisor
                Locale.Key k = new Locale.Key()
                {
                    m_Identifier = "TUTORIAL_ADVISER_TITLE",
                    m_Key = "RoadCustomizer"
                };
                locale.AddLocalizedString(k, "Road Customizer Tool");

                k = new Locale.Key()
                {
                    m_Identifier = "TUTORIAL_ADVISER",
                    m_Key = "RoadCustomizer"
                };
                locale.AddLocalizedString(k, "Vehicle and Speed Restrictions:\n\n" +
                                                "1. Hover over roads to display their lanes\n" +
                                                "2. Left-click to toggle selection of lane(s), right-click clears current selection(s)\n" +
                                                "3. With lanes selected, set vehicle and speed restrictions using the menu icons\n\n\n" +
                                                "Lane Changer:\n\n" +
                                                "1. Hover over roads and find an intersection (circle appears), then click to edit it\n" +
                                                "2. Entry points will be shown, click one to select it (right-click goes back to step 1)\n" +
                                                "3. Click the exit routes you wish to allow (right-click goes back to step 2)" +
                                                "\n\nUse PageUp/PageDown to toggle Underground View.");

                sm_localizationInitialized = true;
            }
            catch (ArgumentException e)
            {
                Logger.LogInfo("Unexpected " + e.GetType().Name + " updating localization: " + e.Message + "\n" + e.StackTrace + "\n");
            }

            Logger.LogInfo("Localization successfully updated.");
        }
    }
}
