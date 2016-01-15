using Transit.Framework;
using Transit.Framework.Modularity;
using ICities;
using System.Xml;
using System;
using ColossalFramework.UI;

namespace Transit.Addon.TrafficAI
{
    public partial class TrafficAIModule : ModuleBase
    {
        [Flags]
        public enum Options : long
        {
            None = 0,

            CongestionAvoidance = 1,
        }

        private static Options s_options = Options.CongestionAvoidance;
        public static Options TrafficAIOptions { get { return s_options; } }

        public override void OnSettingsUI(UIHelperBase helper)
        {
            base.OnSettingsUI(helper);

            helper.AddCheckbox("Congestion Avoidance", "Vehicles will actively avoid congestions", true, OnCheckboxChanged, true, Options.CongestionAvoidance);
        }        

        public override void OnSaveSettings(XmlElement moduleElement)
        {
            TrafficAIOptions.ToXml(moduleElement);
        }

        public override void OnLoadSettings(XmlElement moduleElement)
        {
            if (moduleElement != null)
            {
                s_options = (Options)s_options.FromXml(moduleElement);
            }
        }

        private void OnCheckboxChanged(UIComponent c, bool isChecked)
        {
            UICheckBox checkBox = c as UICheckBox;
            if (checkBox != null && checkBox.objectUserData != null)
            {
                Options checkboxOption = (Options)checkBox.objectUserData;
                if (isChecked)
                    s_options |= checkboxOption;
                else
                    s_options &= ~checkboxOption;

                FireSaveSettingsNeeded();
            }
        }
    }
}
