using ColossalFramework;
using ICities;
using System;
using System.IO;
using System.Xml.Serialization;

namespace CSL_Traffic
{
    public class OptionsManager
    {
        [Flags]
        public enum ModOptions : long
        {
            None = 0,
            UseRealisticSpeeds = 8,
            NoDespawn = 16,
            RoadCustomizerTool = 1L << 55,
        }

        public void CreateSettings(UIHelperBase helper)
        {
            LoadOptions();

            UIHelperBase group = helper.AddGroup("Traffic++ V2 Options");

            group.AddCheckbox(
                "Road Customizer Tool",
                TrafficMod.Options.IsFlagSet(ModOptions.RoadCustomizerTool),
                isChecked =>
                {
                    if (isChecked)
                    {
                        TrafficMod.Options |= ModOptions.RoadCustomizerTool;
                    }
                    else
                    {
                        TrafficMod.Options &= ~ModOptions.RoadCustomizerTool;
                    }

                    Save();
                });

            group.AddCheckbox(
                "No Despawn by CBeTHaX",
                TrafficMod.Options.IsFlagSet(ModOptions.NoDespawn), 
                isChecked =>
                {
                    if (isChecked)
                    {
                        TrafficMod.Options |= ModOptions.NoDespawn;
                    }
                    else
                    {
                        TrafficMod.Options &= ~ModOptions.NoDespawn;
                    }

                    Save();
                });

            group.AddCheckbox(
                "Realistic Speeds",
                TrafficMod.Options.IsFlagSet(ModOptions.UseRealisticSpeeds),
                isChecked =>
                {
                    if (isChecked)
                    {
                        TrafficMod.Options |= ModOptions.UseRealisticSpeeds;
                    }
                    else
                    {
                        TrafficMod.Options &= ~ModOptions.UseRealisticSpeeds;
                    }

                    Save();
                });
        }

        private void Save()
        {
            var options = new Options();

            if (TrafficMod.Options.IsFlagSet(ModOptions.UseRealisticSpeeds))
            {
                options.realisticSpeeds = true;
            }
            if (TrafficMod.Options.IsFlagSet(ModOptions.NoDespawn))
            {
                options.noDespawn = true;
            }
            if (TrafficMod.Options.IsFlagSet(ModOptions.RoadCustomizerTool))
            {
                options.betaTestRoadCustomizer = true;
            }

            try
            {
                var xmlSerializer = new XmlSerializer(typeof(Options));
                using (var streamWriter = new StreamWriter("CSL-TrafficOptions.xml"))
                {
                    xmlSerializer.Serialize(streamWriter, options);
                }
            }
            catch (Exception e)
            {
                Logger.LogInfo("Unexpected " + e.GetType().Name + " saving options: " + e.Message + "\n" + e.StackTrace);
            }
        }

        private bool m_optionsLoaded = false;
        public void LoadOptions()
        {
            if (m_optionsLoaded)
            {
                return;
            }

            try
            {
                Options options;
                var xmlSerializer = new XmlSerializer(typeof(Options));
                using (var streamReader = new StreamReader("CSL-TrafficOptions.xml"))
                {
                    options = (Options)xmlSerializer.Deserialize(streamReader);
                }

                TrafficMod.Options = ModOptions.None;
                if (options.realisticSpeeds)
                    TrafficMod.Options |= ModOptions.UseRealisticSpeeds;

                if (options.noDespawn)
                    TrafficMod.Options |= ModOptions.NoDespawn;

                if (options.betaTestRoadCustomizer)
                    TrafficMod.Options |= ModOptions.RoadCustomizerTool;
            }
            catch (Exception e)
            {
                Logger.LogInfo("Unexpected " + e.GetType().Name + " loading options: " + e.Message + "\n" + e.StackTrace);
            }

            m_optionsLoaded = true;
        }

        public struct Options
        {
            public bool realisticSpeeds;
            public bool noDespawn;
            public bool betaTestRoadCustomizer;
        }
    }
}
