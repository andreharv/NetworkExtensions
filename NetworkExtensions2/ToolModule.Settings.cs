using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using ColossalFramework;
using ICities;
using NetworkExtensions;
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
            var zoneModifierCheckbox = helper.AddCheckbox(
                "Road Zone Modifier " + (Mod.FoundZoningAdjuster ? "*disabled: Zoning Adjuster Detected*" : ""),
                Mod.FoundZoningAdjuster ? "Zoning Adjuster Detected. This feature will be disabled*" : "Press SHIFT (or SHIFT+CTRL) on the Upgrade Road tool to use",
                s_activeOptions.IsFlagSet(ModOptions.RoadZoneModifier) && !Mod.FoundZoningAdjuster,
                isChecked =>
                {
                    if (isChecked && !Mod.FoundZoningAdjuster)
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
            zoneModifierCheckbox.readOnly = Mod.FoundZoningAdjuster;
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
