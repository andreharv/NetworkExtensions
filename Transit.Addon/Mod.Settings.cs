using System.IO;
using System.Xml;
using ICities;
using Transit.Framework;
using Transit.Framework.Modularity;

namespace Transit.Addon
{
    public partial class Mod : IUserMod
    {
        // Change the version only when there are new options
        // and update the notification message!
        private const string s_version = "0.0.1";
        private const string settingsFile = "ProjecTSettings-PleaseChangeMe.xml";

        public void OnSettingsUI(UIHelperBase helper)
        {
            // Quick fix for REx
            foreach (IModule module in Modules)
                module.OnSettingsUI(helper);
            // End of quick fix for REx

            //System.IO.File.AppendAllText("version.txt", "OnSettingsUI\n");
            //OnSaveSettings();
            //OnLoadSettings();
        }

        public void OnSaveSettings()
        {
            XmlDocument settingsDoc = new XmlDocument();
            XmlElement root = settingsDoc.AppendElement("ProjectT");

            root.AppendAttribute("Version", s_version);

            foreach (IModule module in Modules)
            {
                XmlElement moduleElement = root.AppendElement(module.Name);
                module.OnSaveSettings(moduleElement);
            }

            settingsDoc.Save(settingsFile);
        }

        public void OnLoadSettings()
        {
            XmlDocument settingsDoc = null;
            if (File.Exists(settingsFile))
            {
                settingsDoc = new XmlDocument();
                settingsDoc.Load(settingsFile);
            }

            if (settingsDoc == null || settingsDoc.DocumentElement == null)
                return;

            foreach (IModule module in Modules)
            {
                XmlNodeList nodeList = settingsDoc.GetElementsByTagName(module.Name);
                if (nodeList.Count > 0)
                    module.OnLoadSettings(nodeList[0] as XmlElement);
            }
        }

        private void CheckForUpdates()
        {
            File.AppendAllText("version.txt", "CheckForUpdates\n");
            XmlDocument settingsDoc = null;

            if (File.Exists(settingsFile))
            {
                settingsDoc = new XmlDocument();
                settingsDoc.Load(settingsFile);
            }

            if (settingsDoc == null || settingsDoc.DocumentElement == null)
            {
                //NotificationPanel.Panel.Show("Welcome!!!", "Some amazing welcome!", false, "Oh yeah!", null, "On noes! :O", null, true);
                return;
            }

            string fileVersion = settingsDoc.DocumentElement.GetAttribute("Version");
            if (fileVersion != s_version)
            {
                NotificationPanel.Panel.Show("Update!!!", "Some amazing description!", false, "Oh yeah!", null, "On noes! :O", null, true);
            }
        }
    }
}
