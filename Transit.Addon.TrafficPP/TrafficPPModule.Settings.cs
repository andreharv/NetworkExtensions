using System;
using System.Linq;
using System.Xml;
using ColossalFramework;
using ICities;
using Transit.Framework;

namespace Transit.Addon.TrafficPP
{
    public partial class TrafficPPModule
    {
        [Flags]
        public enum ModOptions : long
        {
            None = 0,
            RoadCustomizerTool = 1L << 55,

            GhostMode = 1L << 62
        }

        private static ModOptions s_activeOptions = ModOptions.None;
        public static ModOptions ActiveOptions { get { return s_activeOptions; } }

        public override void OnSettingsUI(UIHelperBase helper)
        {
            helper.AddCheckbox(
                "Road Customizer Tool",
                "Road Customizer Tool",
                s_activeOptions.IsFlagSet(ModOptions.RoadCustomizerTool),
                isChecked =>
                {
                    if (isChecked)
                    {
                        s_activeOptions = s_activeOptions | ModOptions.RoadCustomizerTool;
                    }
                    else
                    {
                        s_activeOptions = s_activeOptions & ~ModOptions.RoadCustomizerTool;
                    }
                    FireSaveSettingsNeeded();
                },
                true);

            helper.AddCheckbox(
                "Ghost Mode",
                "Disables all mod functionality leaving only enough logic to load the map",
                s_activeOptions.IsFlagSet(ModOptions.GhostMode),
                isChecked =>
                {
                    if (isChecked)
                    {
                        s_activeOptions = s_activeOptions | ModOptions.GhostMode;
                    }
                    else
                    {
                        s_activeOptions = s_activeOptions & ~ModOptions.GhostMode;
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
                    if (option == ModOptions.GhostMode)
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
                    s_activeOptions = s_activeOptions | option;
                }
                else
                {
                    s_activeOptions = s_activeOptions & ~option;
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
                    s_activeOptions.HasFlag(option).ToString());
            }
        }
    }
}
