using System;
using System.IO;
using System.Xml.Serialization;
using ColossalFramework;
using ICities;

namespace CSL_Traffic
{
    [Flags]
    public enum ModOptions : long
    {
        None = 0,
        UseRealisticSpeeds = 8,
        NoDespawn = 16,
        RoadCustomizerTool = 1L << 55,
    }

    public partial class Mod
    {
        protected override string SettingsFile
        {
            get { return "TransitAddonModSettings.xml"; }
        }

        protected override string SettingsNode
        {
            get { return "Tools"; }
        }

        public static ModOptions Options = ModOptions.RoadCustomizerTool | ModOptions.NoDespawn;
        private bool m_optionsLoaded = false;

        public void OnSettingsUI(UIHelperBase helper)
        {
            UIHelperBase group = helper.AddGroup("Traffic++ V2 Options");

            group.AddCheckbox(
                "Road Customizer Tool",
                Options.IsFlagSet(ModOptions.RoadCustomizerTool),
                isChecked =>
                {
                    if (isChecked)
                    {
                        Options |= ModOptions.RoadCustomizerTool;
                    }
                    else
                    {
                        Options &= ~ModOptions.RoadCustomizerTool;
                    }

                    SaveSettings();
                });

            group.AddCheckbox(
                "No Despawn by CBeTHaX",
                Options.IsFlagSet(ModOptions.NoDespawn),
                isChecked =>
                {
                    if (isChecked)
                    {
                        Options |= ModOptions.NoDespawn;
                    }
                    else
                    {
                        Options &= ~ModOptions.NoDespawn;
                    }

                    SaveSettings();
                });

            group.AddCheckbox(
                "Realistic Speeds",
                Options.IsFlagSet(ModOptions.UseRealisticSpeeds),
                isChecked =>
                {
                    if (isChecked)
                    {
                        Options |= ModOptions.UseRealisticSpeeds;
                    }
                    else
                    {
                        Options &= ~ModOptions.UseRealisticSpeeds;
                    }

                    SaveSettings();
                });
        }

        protected override void LoadSettings()
        {
            if (m_optionsLoaded)
            {
                return;
            }

            try
            {
                OptionsManager.Options options;
                var xmlSerializer = new XmlSerializer(typeof(OptionsManager.Options));
                using (var streamReader = new StreamReader("CSL-TrafficOptions.xml"))
                {
                    options = (OptionsManager.Options)xmlSerializer.Deserialize(streamReader);
                }

                Options = ModOptions.None;
                if (options.realisticSpeeds)
                {
                    Logger.LogInfo("Option UseRealisticSpeeds");
                    Options |= ModOptions.UseRealisticSpeeds;
                }
                else
                {
                    Logger.LogInfo("Option UseRealisticSpeeds Disabled");
                    Options &= ~ModOptions.UseRealisticSpeeds;
                }

                if (options.noDespawn)
                {
                    Logger.LogInfo("Option NoDespawn");
                    Options |= ModOptions.NoDespawn;
                }
                else
                {
                    Logger.LogInfo("Option NoDespawn Disabled");
                    Options &= ~ModOptions.NoDespawn;
                }

                if (options.betaTestRoadCustomizer)
                {
                    Logger.LogInfo("Option RoadCustomizerTool");
                    Options |= ModOptions.RoadCustomizerTool;
                }
                else
                {
                    Logger.LogInfo("Option RoadCustomizerTool Disabled");
                    Options &= ~ModOptions.RoadCustomizerTool;
                }
            }
            catch (Exception e)
            {
                Logger.LogInfo("Unexpected " + e.GetType().Name + " loading options: " + e.Message + "\n" + e.StackTrace);
            }

            m_optionsLoaded = true;
        }

        protected override void SaveSettings()
        {
            var options = new OptionsManager.Options();

            if (Options.IsFlagSet(ModOptions.UseRealisticSpeeds))
            {
                options.realisticSpeeds = true;
            }
            if (Options.IsFlagSet(ModOptions.NoDespawn))
            {
                options.noDespawn = true;
            }
            if (Options.IsFlagSet(ModOptions.RoadCustomizerTool))
            {
                options.betaTestRoadCustomizer = true;
            }

            try
            {
                var xmlSerializer = new XmlSerializer(typeof(OptionsManager.Options));
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
    }
}
