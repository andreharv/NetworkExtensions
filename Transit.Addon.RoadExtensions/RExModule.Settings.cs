using System.Linq;
using System.Xml;
using ICities;
using Transit.Framework;
using Transit.Framework.Interfaces;
using Transit.Framework.Modularity;

namespace Transit.Addon.RoadExtensions
{
    public partial class RExModule
    {
        public override void OnSettingsUI(UIHelperBase helper)
        {
            base.OnSettingsUI(helper);

            foreach (var part in Parts.OfType<IActivablePart>())
            {
                var partLocal = part;

                helper.AddCheckbox(
                    part.DisplayName, 
                    null, // TODO: add description of road here -> part.DisplayDescription ?
                    part.IsEnabled, 
                    isChecked =>
                    {
                        partLocal.IsEnabled = isChecked;
                        FireSaveSettingsNeeded();
                    },
                    true);
            }
        }

        public override void OnLoadSettings(XmlElement moduleElement)
        {
            foreach (var activablePart in Parts.OfType<IActivablePart>())
            {
                var isEnabled = true;

                if (moduleElement != null)
                {
                    var nodeList = moduleElement.GetElementsByTagName(activablePart.GetCodeName());
                    if (nodeList.Count > 0)
                    {
                        var node = (XmlElement) nodeList[0];
                        var nodeValue = true;

                        if (!bool.TryParse(node.InnerText, out nodeValue))
                        {
                            nodeValue = true;
                        }

                        isEnabled = nodeValue;
                    }
                }

                activablePart.IsEnabled = isEnabled;
            }
        }

        public override void OnSaveSettings(XmlElement moduleElement)
        {
            base.OnSaveSettings(moduleElement);

            foreach (var activablePart in Parts.OfType<IActivablePart>())
            {
                moduleElement.AppendElement(
                    activablePart.GetCodeName(), 
                    activablePart.IsEnabled.ToString());
            }
        }
    }
}
