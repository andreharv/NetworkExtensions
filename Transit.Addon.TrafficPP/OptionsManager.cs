using System;
using System.IO;
using System.Xml.Serialization;
using ColossalFramework.UI;
using ICities;
using UnityEngine;

namespace Transit.Addon.TrafficPP
{
    public class OptionsManager : MonoBehaviour
    {
        [Flags]
        public enum ModOptions : long
        {
            None = 0,
            NoDespawn = 16,
            ImprovedAI = 32,
            BetaTestRoadCustomizerTool = 1L << 55,
            FixCargoTrucksNotSpawning = 1L << 61,

            GhostMode = 1L << 62
        }

        UICheckBox m_noDespawnCheckBox = null;
        UICheckBox m_improvedAICheckBox = null;
        UICheckBox m_betaTestRoadCustomizerCheckBox = null;
        UICheckBox m_ghostModeCheckBox = null;

        void Awake()
        {
            DontDestroyOnLoad(this);
        }

        public void CreateSettings(UIHelperBase helper)
        {
            UIHelperBase group = helper.AddGroup("Traffic++ Options");
            m_noDespawnCheckBox = group.AddCheckbox("No Despawn by CBeTHaX", false, IgnoreMe) as UICheckBox;
            m_betaTestRoadCustomizerCheckBox = group.AddCheckbox("Beta Test: Road Customizer Tool", false, IgnoreMe) as UICheckBox;
            m_improvedAICheckBox = group.AddCheckbox("Beta Test: Improved AI", false, IgnoreMe) as UICheckBox;
            m_ghostModeCheckBox = group.AddCheckbox("Ghost Mode (disables all mod functionality leaving only enough logic to load the map)", false, IgnoreMe) as UICheckBox;

            group.AddButton("Save", OnSave);

            LoadOptions();
        }

        private void IgnoreMe(bool c)
        {
            // The addCheckbox methods above require an event
            // This is temporary as the options panel will be reworked in future release
        }

        private void OnSave()
        {
            Options options = new Options();
            TrafficPPModule.Options = ModOptions.None;
            if (this.m_noDespawnCheckBox.isChecked)
            {
                options.noDespawn = true;
                TrafficPPModule.Options |= ModOptions.NoDespawn;
            }
            if (this.m_improvedAICheckBox.isChecked)
            {
                options.improvedAI = true;
                TrafficPPModule.Options |= ModOptions.ImprovedAI;
            }
            if (this.m_betaTestRoadCustomizerCheckBox.isChecked)
            {
                options.betaTestRoadCustomizer = true;
                TrafficPPModule.Options |= ModOptions.BetaTestRoadCustomizerTool;
            }
            if (this.m_ghostModeCheckBox.isChecked)
            {
                options.ghostMode = true;
                TrafficPPModule.Options |= ModOptions.GhostMode;
            }

            try
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(Options));
                using (StreamWriter streamWriter = new StreamWriter("CSL-TrafficOptions.xml"))
                {
                    xmlSerializer.Serialize(streamWriter, options);
                }
            }
            catch (Exception e)
            {
                Logger.LogInfo("Unexpected " + e.GetType().Name + " saving options: " + e.Message + "\n" + e.StackTrace);
            }
        }

        public void LoadOptions()
        {
            TrafficPPModule.Options = ModOptions.None;
            Options options = new Options();
            try
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(Options));
                using (StreamReader streamReader = new StreamReader("CSL-TrafficOptions.xml"))
                {
                    options = (Options)xmlSerializer.Deserialize(streamReader);
                }
            }
            catch (FileNotFoundException)
            {
                // No options file yet
                return;
            }
            catch (Exception e)
            {
                Logger.LogInfo("Unexpected " + e.GetType().Name + " loading options: " + e.Message + "\n" + e.StackTrace);
                return;
            }

            if (this.m_ghostModeCheckBox != null)
            {
                this.m_noDespawnCheckBox.isChecked = options.noDespawn;
                this.m_improvedAICheckBox.isChecked = options.improvedAI;
                this.m_betaTestRoadCustomizerCheckBox.isChecked = options.betaTestRoadCustomizer;
                this.m_ghostModeCheckBox.isChecked = options.ghostMode;
            }

            if (options.noDespawn)
                TrafficPPModule.Options |= ModOptions.NoDespawn;

            if (options.improvedAI)
                TrafficPPModule.Options |= ModOptions.ImprovedAI;

            if (options.betaTestRoadCustomizer)
                TrafficPPModule.Options |= ModOptions.BetaTestRoadCustomizerTool;

            if (options.ghostMode)
                TrafficPPModule.Options |= ModOptions.GhostMode;

            if (options.fixCargoTrucksNotSpawning)
                TrafficPPModule.Options |= ModOptions.FixCargoTrucksNotSpawning;
        }

        public struct Options
        {
            public bool noDespawn;
            public bool improvedAI;
            public bool noStopForCrossing;
            public bool betaTestRoadCustomizer;
            public bool fixCargoTrucksNotSpawning;
            public bool ghostMode;
        }
    }
}
