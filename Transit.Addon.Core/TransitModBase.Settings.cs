using System.IO;
using System.Xml;
using Transit.Framework;
using Transit.Framework.Interfaces;
using Transit.Framework.Modularity;

namespace Transit.Addon.Core
{
    public partial class TransitModBase
    {
        protected abstract string SettingsFile { get; }
        protected abstract string SettingsNode { get; }

        protected virtual void SaveSettings()
        {
            var settingsDoc = new XmlDocument();
            var root = settingsDoc.AppendElement(SettingsFile);

            root.AppendAttribute("Version", Version);

            foreach (IModule module in Modules)
            {
                XmlElement moduleElement = root.AppendElement(module.GetCodeName());
                module.OnSaveSettings(moduleElement);
            }

            try
            {
                settingsDoc.Save(SettingsFile);
            }
            catch (System.Exception)
            {
                // TODO: log error saving file
            }
        }

        protected virtual void LoadSettings()
        {
            XmlDocument settingsDoc = null;
            if (File.Exists(SettingsFile))
            {
                settingsDoc = new XmlDocument();
                settingsDoc.Load(SettingsFile);
            }

            if (settingsDoc != null && settingsDoc.DocumentElement == null)
            {
                settingsDoc = null;
            }

            foreach (IModule module in Modules)
            {
                if (settingsDoc != null)
                {
                    XmlNodeList nodeList = settingsDoc.GetElementsByTagName(module.GetCodeName());
                    if (nodeList.Count > 0)
                    {
                        module.OnLoadSettings(nodeList[0] as XmlElement);
                        continue;
                    }
                }

                // Default 
                module.OnLoadSettings(null);
            }
        }
    }
}
