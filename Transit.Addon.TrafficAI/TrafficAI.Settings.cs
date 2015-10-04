using Transit.Framework;
using Transit.Framework.Modularity;
using ICities;
using System.Xml;

namespace Transit.Addon.TrafficAI
{
    public partial class TrafficAIModule : ModuleBase
    {
        public override void OnSettingsUI(UIHelperBase helper)
        {
            base.OnSettingsUI(helper);
        }

        public override void OnSaveSettings(XmlElement moduleElement)
        {
            for (int i = 0; i < 5; i++)
            {
                XmlElement element = moduleElement.AppendElement("option" + i);
                element.InnerText = "value " + (i*5);
                moduleElement.AppendChild(element);
            }
        }

        public override void OnLoadSettings(XmlElement moduleElement)
        {
            foreach (XmlElement element in moduleElement.ChildNodes)
            {
                System.IO.File.AppendAllText("xmlLoad.txt", element.Name + " = " + element.InnerText);
            }
        }
    }
}
