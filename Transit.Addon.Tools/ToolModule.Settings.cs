using System;
using System.Linq;
using System.Xml;
using ColossalFramework;
using ICities;
using Transit.Framework;

namespace Transit.Addon.Tools
{
    public partial class ToolModule
    {
        [Flags]
        public enum ModOptions : long
        {
            None = 0,
            RoadZoneModifier = 1L << 0
        }

        private static ModOptions s_activeOptions = ModOptions.None;
        public static ModOptions ActiveOptions { get { return s_activeOptions; } }

        public override void OnSettingsUI(UIHelperBase helper)
        {
            helper.AddCheckbox(
                "Road Zone Modifier",
                "Press SHIFT (or SHIFT+CTRL) on the Upgrade Road tool to use",
                s_activeOptions.IsFlagSet(ModOptions.RoadZoneModifier), 
                isChecked =>
                {
                    if (isChecked)
                    {
                        s_activeOptions = s_activeOptions | ModOptions.RoadZoneModifier;
                    }
                    else
                    {
                        s_activeOptions = s_activeOptions & ~ModOptions.RoadZoneModifier;
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
                    isEnabled = true;
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
