using System;
using System.Linq;
using System.Xml;
using ColossalFramework;
using ICities;
using Transit.Framework;

namespace Transit.Addon.ToolsV2
{
    [Flags]
    public enum ModOptions : long
    {
        None = 0,
        UseRealisticSpeeds = 8,
        NoDespawn = 16,
        RoadCustomizerTool = 1L << 55,
    }

    public partial class ToolModuleV2
    {
        static ToolModuleV2()
        {
            ActiveOptions = ModOptions.RoadCustomizerTool | ModOptions.NoDespawn;
        }

        public static ModOptions ActiveOptions { get; private set; }

        public override void OnSettingsUI(UIHelperBase helper)
        {
            helper.AddCheckbox(
                "Road Customizer Tool",
                "Allows you to customize entry and exit points in junctions / restrict vehicle and speed usage on lanes.",
                ActiveOptions.IsFlagSet(ModOptions.RoadCustomizerTool),
                isChecked =>
                {
                    if (isChecked)
                    {
                        ActiveOptions |= ModOptions.RoadCustomizerTool;
                    }
                    else
                    {
                        ActiveOptions &= ~ModOptions.RoadCustomizerTool;
                    }

                    FireSaveSettingsNeeded();
                },
                true);

            helper.AddCheckbox(
                "No Despawn by CBeTHaX",
                null,
                ActiveOptions.IsFlagSet(ModOptions.NoDespawn),
                isChecked =>
                {
                    if (isChecked)
                    {
                        ActiveOptions |= ModOptions.NoDespawn;
                    }
                    else
                    {
                        ActiveOptions &= ~ModOptions.NoDespawn;
                    }

                    FireSaveSettingsNeeded();
                },
                true);

            helper.AddCheckbox(
                "Realistic Speeds",
                null,
                ActiveOptions.IsFlagSet(ModOptions.UseRealisticSpeeds),
                isChecked =>
                {
                    if (isChecked)
                    {
                        ActiveOptions |= ModOptions.UseRealisticSpeeds;
                    }
                    else
                    {
                        ActiveOptions &= ~ModOptions.UseRealisticSpeeds;
                    }

                    FireSaveSettingsNeeded();
                },
                true);
        }

        public override void OnLoadSettings(XmlElement moduleElement)
        {
            foreach (var option in Enum.GetValues(typeof(ModOptions))
                                       .OfType<ModOptions>()
                                       .Where(o => o != 0))
            {
                bool? isEnabled = null;

                if (moduleElement != null)
                {
                    var nodeList = moduleElement.GetElementsByTagName(option.ToString().ToUpper());
                    if (nodeList.Count > 0)
                    {
                        var node = (XmlElement)nodeList[0];
                        var nodeValue = true;

                        if (bool.TryParse(node.InnerText, out nodeValue))
                        {
                            isEnabled = nodeValue;
                        }
                    }
                }

                if (isEnabled == null)
                {
                    // Default
                    if (option == ModOptions.UseRealisticSpeeds)
                    {
                        isEnabled = false;
                    }
                    else
                    {
                        isEnabled = true;
                    }
                }

                if (isEnabled.Value)
                {
                    ActiveOptions = ActiveOptions | option;
                }
                else
                {
                    ActiveOptions = ActiveOptions & ~option;
                }
            }
        }

        public override void OnSaveSettings(XmlElement moduleElement)
        {
            base.OnSaveSettings(moduleElement);

            foreach (var option in Enum.GetValues(typeof(ModOptions))
                                       .OfType<ModOptions>()
                                       .Where(o => o != 0))
            {
                moduleElement.AppendElement(
                    option.ToString().ToUpper(),
                    ActiveOptions.HasFlag(option).ToString());
            }
        }
    }
}
