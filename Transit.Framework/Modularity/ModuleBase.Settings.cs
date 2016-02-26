using System.Linq;
using System.Xml;
using ICities;
using Transit.Framework.Interfaces;

namespace Transit.Framework.Modularity
{
    public abstract partial class ModuleBase : IModule
    {
        public virtual void OnSettingsUI(UIHelperBase helper)
        {
            foreach (var part in Parts.OfType<IActivablePart>())
            {
                var partLocal = part;

                helper.AddCheckbox(
                    part.DisplayName,
                    part is IShortDescriptor ? ((IShortDescriptor)part).ShortDescription :
                    part is IDescriptor ? ((IDescriptor)part).Description :
                    null,
                    part.IsEnabled,
                    isChecked =>
                    {
                        partLocal.IsEnabled = isChecked;
                        FireSaveSettingsNeeded();
                    },
                    true);
            }
        }

        public virtual void OnLoadSettings(XmlElement moduleElement)
        {
            foreach (var activablePart in Parts.OfType<IActivablePart>())
            {
                var isEnabled = true;

                if (moduleElement != null)
                {
                    var nodeList = moduleElement.GetElementsByTagName(activablePart.GetCodeName());
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

                activablePart.IsEnabled = isEnabled;
            }
        }

        public virtual void OnSaveSettings(XmlElement moduleElement)
        {
            foreach (var activablePart in Parts.OfType<IActivablePart>())
            {
                moduleElement.AppendElement(
                    activablePart.GetCodeName(),
                    activablePart.IsEnabled.ToString());
            }
        }

        public event SaveSettingsNeededEventHandler SaveSettingsNeeded;

        protected void FireSaveSettingsNeeded()
        {
            if (SaveSettingsNeeded != null)
            {
                SaveSettingsNeeded();
            }
        }
    }
}
