using Transit.Framework;
using Transit.Framework.Modularity;
using ICities;
using System.Xml;
using System;
using System.Linq;
using ColossalFramework;
using ColossalFramework.UI;
using UnityEngine;

namespace Transit.Addon.PathFinding
{
    public partial class PathfindingModule : ModuleBase
    {
        [Flags]
        public enum Options : long
        {
            None = 0,
            CongestionAvoidance = 1,
        }

        private static Options s_activeOptions = Options.None;
        public static Options PathfindingOptions { get { return s_activeOptions; } }

        public override void OnSettingsUI(UIHelperBase helper)
        {
            base.OnSettingsUI(helper);
            
            helper.AddCheckbox(
                "Congestion Avoidance", 
                "Vehicles will actively avoid congestions",
                s_activeOptions.IsFlagSet(Options.CongestionAvoidance),
                isChecked =>
                {
                    if (isChecked)
                    {
                        s_activeOptions = s_activeOptions | Options.CongestionAvoidance;
                    }
                    else
                    {
                        s_activeOptions = s_activeOptions & ~Options.CongestionAvoidance;
                    }
                    FireSaveSettingsNeeded();
                },
                true);
        } 

        public override void OnLoadSettings(XmlElement moduleElement)
        {
            foreach (var option in Enum.GetValues(typeof(Options))
                                       .OfType<Options>()
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

            foreach (var option in Enum.GetValues(typeof(Options))
                                       .OfType<Options>()
                                       .Where(o => o != 0))
            {
                moduleElement.AppendElement(
                    option.ToString().ToUpper(),
                    s_activeOptions.HasFlag(option).ToString());
            }
        }
    }
}
