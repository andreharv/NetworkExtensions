using System;
using System.Collections.Generic;
using ColossalFramework;
using ColossalFramework.Globalization;
using CSL_Traffic;
using ICities;
using Transit.Addon.ToolsV2.PathFinding.ExtendedFeatures;
using Transit.Framework;
using Transit.Framework.ExtensionPoints.PathFinding;
using Transit.Framework.Modularity;
using Transit.Framework.Redirection;

namespace Transit.Addon.ToolsV2
{
    [Module("Transit.Addon.Mod")]
    public partial class ToolModuleV2 : ModuleBase
    {
        private static bool sm_redirectionInstalled = false;

        public override string Name
        {
            get { return "Tools"; }
        }

        public override void OnLevelLoaded(LoadMode mode)
        {
            base.OnLevelLoaded(mode);

            if (mode == LoadMode.NewGame || mode == LoadMode.LoadGame)
            {
                if ((ActiveOptions & ModOptions.RoadCustomizerTool) == ModOptions.RoadCustomizerTool)
                    ToolsModifierControl.toolController.AddTool<RoadCustomizerTool>();

                if ((ActiveOptions & ModOptions.UseRealisticSpeeds) == ModOptions.UseRealisticSpeeds)
                    ActivateRealisticSpeed();
            }
        }

        public override void OnLevelUnloading()
        {
            base.OnLevelUnloading();

            if ((ActiveOptions & ModOptions.RoadCustomizerTool) == ModOptions.RoadCustomizerTool)
                ToolsModifierControl.toolController.RemoveTool<RoadCustomizerTool>();

            if ((ActiveOptions & ModOptions.UseRealisticSpeeds) == ModOptions.UseRealisticSpeeds)
                DeactivateRealisticSpeed();
        }

        public override void OnEnabled()
        {
            base.OnEnabled();
            
            if (!sm_redirectionInstalled)
            {
                Redirector.PerformRedirections();
                sm_redirectionInstalled = true;
            }

            ExtendedPathManager.instance.DefinePathFinding<TPPPathFind>();
        }

        public override void OnDisabled()
        {
            if (sm_redirectionInstalled)
            {
                Redirector.RevertRedirections();
                sm_redirectionInstalled = false;
            }

            ExtendedPathManager.instance.ResetPathFinding<TPPPathFind>();
        }

        public override void OnInstallingLocalization()
        {
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
            }
            catch (ArgumentException e)
            {
                Logger.LogInfo("Unexpected " + e.GetType().Name + " updating localization: " + e.Message + "\n" + e.StackTrace + "\n");
            }

            Logger.LogInfo("Localization successfully updated.");
        }

        private void ActivateRealisticSpeed()
        {
            SetRealisticSpeed(true);
        }

        private void DeactivateRealisticSpeed()
        {
            SetRealisticSpeed(false);
        }

        private void SetRealisticSpeed(bool activating)
        {
            for (uint i = 0; i < PrefabCollection<CitizenInfo>.LoadedCount(); i++)
            {
                CitizenInfo cit = PrefabCollection<CitizenInfo>.GetLoaded(i);
                float m_walkSpeedMultiplier = 0.5f;

                if (!activating)
                {
                    m_walkSpeedMultiplier = 1f / m_walkSpeedMultiplier;
                }

                cit.m_walkSpeed *= m_walkSpeedMultiplier;
            }

            for (uint i = 0; i < PrefabCollection<VehicleInfo>.LoadedCount(); i++)
            {
                VehicleInfo vehicle = PrefabCollection<VehicleInfo>.GetLoaded(i);
                float accelerationMultiplier;
                float maxSpeedMultiplier;

                var name = vehicle.name.ToLowerInvariant();

                if (name.Contains("bus") ||
                    name.Contains("truck") ||
                    name.Contains("tractor") ||
                    name.Contains("trailer") ||
                    name.Contains("van"))
                {
                    accelerationMultiplier = 0.25f;
                    maxSpeedMultiplier = 0.5f;
                }
                else
                {
                    accelerationMultiplier = 0.5f;
                    maxSpeedMultiplier = 0.5f;
                }

                if (!activating)
                {
                    accelerationMultiplier = 1f / accelerationMultiplier;
                    maxSpeedMultiplier = 1f / maxSpeedMultiplier;
                }

                vehicle.m_acceleration *= accelerationMultiplier;
                vehicle.m_maxSpeed *= maxSpeedMultiplier;
            }
        }
    }
}
