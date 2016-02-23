using ColossalFramework.UI;
using ICities;
using System;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;

namespace CSL_Traffic
{
    public class OptionsManager : MonoBehaviour
    {
        [Flags]
        public enum ModOptions : long
        {
            None = 0,
            UseRealisticSpeeds = 8,
            NoDespawn = 16,
            BetaTestRoadCustomizerTool = 1L << 55,
        }

        UICheckBox m_realisticSpeedsCheckBox = null;
        UICheckBox m_noDespawnCheckBox = null;
        UICheckBox m_betaTestRoadCustomizerCheckBox = null;

        void Awake()
        {
            DontDestroyOnLoad(this);
        }

        public void CreateSettings(UIHelperBase helper)
        {
            UIHelperBase group = helper.AddGroup("Traffic++ V2 Options");
            m_noDespawnCheckBox = group.AddCheckbox("No Despawn by CBeTHaX", false, IgnoreMe) as UICheckBox;
            m_realisticSpeedsCheckBox = group.AddCheckbox("Realistic Speeds", false, IgnoreMe) as UICheckBox;
            m_betaTestRoadCustomizerCheckBox = group.AddCheckbox("Road Customizer Tool", false, IgnoreMe) as UICheckBox;

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
            //this.m_optionsPanel.GetComponent<UIPanel>().isVisible = false;

            Options options = new Options();
            TrafficMod.Options = ModOptions.None;
            if (this.m_realisticSpeedsCheckBox.isChecked)
            {
                options.realisticSpeeds = true;
                TrafficMod.Options |= ModOptions.UseRealisticSpeeds;
            }
            if (this.m_noDespawnCheckBox.isChecked)
            {
                options.noDespawn = true;
                TrafficMod.Options |= ModOptions.NoDespawn;
            }
            if (this.m_betaTestRoadCustomizerCheckBox.isChecked)
            {
                options.betaTestRoadCustomizer = true;
                TrafficMod.Options |= ModOptions.BetaTestRoadCustomizerTool;
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
            TrafficMod.Options = ModOptions.None;
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

            this.m_realisticSpeedsCheckBox.isChecked = options.realisticSpeeds;
            this.m_noDespawnCheckBox.isChecked = options.noDespawn;
            this.m_betaTestRoadCustomizerCheckBox.isChecked = options.betaTestRoadCustomizer;

            if (options.realisticSpeeds)
                TrafficMod.Options |= ModOptions.UseRealisticSpeeds;

            if (options.noDespawn)
                TrafficMod.Options |= ModOptions.NoDespawn;

            if (options.betaTestRoadCustomizer)
                TrafficMod.Options |= ModOptions.BetaTestRoadCustomizerTool;
        }

        public struct Options
        {
            public bool allowTrucks;
            public bool allowResidents;
            public bool disableCentralLane;
            public bool realisticSpeeds;
            public bool noDespawn;
            public bool improvedAI;
            public bool disableCustomRoads;
            public bool noStopForCrossing;

            public bool betaTestRoadCustomizer;

            public bool fixCargoTrucksNotSpawning;
        }
    }
}
