using System.Linq;
using System.Xml;
using ColossalFramework;
using ICities;
using System;
using Transit.Framework;
using Transit.Framework.Modularity;

namespace Transit.Addon.ToolsV2
{
    public partial class ToolModuleV2 : ModuleBase
    {
        [Flags]
        public enum ModOptions : long
        {
            None = 0,
            LaneRoutingTool         = 1L << 1,
            LaneRestrictorTool      = 1L << 5,
            TrafficLightsTool       = 1L << 10,
            //LanePrioritiesTool      = 1 << 29
        }

        private static ModOptions s_activeOptions = ModOptions.LaneRoutingTool | ModOptions.LaneRestrictorTool | ModOptions.TrafficLightsTool;
        public static ModOptions TrafficToolsOptions { get { return s_activeOptions; } }

        //public override void OnSettingsUI(UIHelperBase helper)
        //{
        //    helper.AddCheckbox(
        //        "Lane Routing",
        //        "Allows you to customize entry and exit points in junctions.",
        //        s_activeOptions.IsFlagSet(ModOptions.LaneRoutingTool), 
        //        isChecked =>
        //        {
        //            if (isChecked)
        //            {
        //                s_activeOptions = s_activeOptions | ModOptions.LaneRoutingTool;
        //            }
        //            else
        //            {
        //                s_activeOptions = s_activeOptions & ~ModOptions.LaneRoutingTool;
        //            }
        //            FireSaveSettingsNeeded();
        //        },
        //        true);

        //    helper.AddCheckbox(
        //        "Lane Restrictor",
        //        "Allows you to restrict vehicle and speed usage on lanes.",
        //        s_activeOptions.IsFlagSet(ModOptions.LaneRestrictorTool),
        //        isChecked =>
        //        {
        //            if (isChecked)
        //            {
        //                s_activeOptions = s_activeOptions | ModOptions.LaneRestrictorTool;
        //            }
        //            else
        //            {
        //                s_activeOptions = s_activeOptions & ~ModOptions.LaneRestrictorTool;
        //            }
        //            FireSaveSettingsNeeded();
        //        },
        //        true);

        //    helper.AddCheckbox(
        //        "Traffic Lights",
        //        "Allows you to toggle and setup sequences at traffic lights.",
        //        s_activeOptions.IsFlagSet(ModOptions.LaneRestrictorTool),
        //        isChecked =>
        //        {
        //            if (isChecked)
        //            {
        //                s_activeOptions = s_activeOptions | ModOptions.LaneRestrictorTool;
        //            }
        //            else
        //            {
        //                s_activeOptions = s_activeOptions & ~ModOptions.LaneRestrictorTool;
        //            }
        //            FireSaveSettingsNeeded();
        //        },
        //        true);
        //}

        //public override void OnLoadSettings(XmlElement moduleElement)
        //{
        //    foreach (var option in Enum.GetValues(typeof(ModOptions))
        //                               .OfType<ModOptions>()
        //                               .Where(o => o != 0))
        //    {
        //        bool? isEnabled = null;

        //        if (moduleElement != null)
        //        {
        //            var nodeList = moduleElement.GetElementsByTagName(option.ToString().ToUpper());
        //            if (nodeList.Count > 0)
        //            {
        //                var node = (XmlElement)nodeList[0];
        //                var nodeValue = true;

        //                if (bool.TryParse(node.InnerText, out nodeValue))
        //                {
        //                    isEnabled = nodeValue;
        //                }
        //            }
        //        }

        //        if (isEnabled == null)
        //        {
        //            isEnabled = true;
        //        }

        //        if (isEnabled.Value)
        //        {
        //            s_activeOptions = s_activeOptions | option;
        //        }
        //        else
        //        {
        //            s_activeOptions = s_activeOptions & ~option;
        //        }
        //    }
        //}

        //public override void OnSaveSettings(XmlElement moduleElement)
        //{
        //    base.OnSaveSettings(moduleElement);

        //    foreach (var option in Enum.GetValues(typeof(ModOptions))
        //                               .OfType<ModOptions>()
        //                               .Where(o => o != 0))
        //    {
        //        moduleElement.AppendElement(
        //            option.ToString().ToUpper(),
        //            s_activeOptions.HasFlag(option).ToString());
        //    }
        //}
    }
}
