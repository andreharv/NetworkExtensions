using Transit.Framework;
using Transit.Framework.Modularity;
using ICities;
using System.Xml;
using System;

namespace Transit.Addon.TrafficAI
{
    public partial class TrafficAIModule : ModuleBase
    {
        [Flags]
        public enum Options
        {
            None = 0,

            CongestionAvoidance = 1,
            LaneRestriction = 2
        }

        private static Options s_options = Options.CongestionAvoidance;
        public static Options TrafficAIOptions { get { return s_options; } }

        public override void OnSettingsUI(UIHelperBase helper)
        {
            base.OnSettingsUI(helper);
        }

        public override void OnSaveSettings(XmlElement moduleElement)
        {
            Options opt = Options.CongestionAvoidance | Options.LaneRestriction;

            opt.ToXml(moduleElement);
        }

        public override void OnLoadSettings(XmlElement moduleElement)
        {
            Options opt = Options.None;
            opt = (Options)opt.FromXml(moduleElement);

            System.IO.File.AppendAllText("xmlLoad.txt", opt.ToString());
        }


    }
}
