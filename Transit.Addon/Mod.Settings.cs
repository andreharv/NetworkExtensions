using System.IO;
using System.Xml;
using ICities;
using Transit.Framework;
using Transit.Framework.Modularity;
using ColossalFramework.UI;
using UnityEngine;

namespace Transit.Addon
{
    public partial class Mod : IUserMod
    {
        // Change the version only when there are new options
        // and update the notification message!
        private const string VERSION = "0.0.1";
        private const string SETTINGS_FILE = "TransitAddonMod Settings.xml";

        private UIScrollablePanel _optionsPanel;

        public void OnSettingsUI(UIHelperBase helper)
        {
            OnLoadSettings();

            UIButton tabTemplate = Resources.FindObjectsOfTypeAll<OptionsKeymappingPanel>()[0]
                                            .GetComponentInChildren<UITabstrip>()
                                            .GetComponentInChildren<UIButton>();

            _optionsPanel = ((UIHelper)helper).self as UIScrollablePanel;
            _optionsPanel.autoLayout = false;

            UITabstrip strip = _optionsPanel.AddUIComponent<UITabstrip>();
            strip.relativePosition = new Vector3(0, 0);
            strip.size = new Vector2(744, 40);

            UITabContainer container = _optionsPanel.AddUIComponent<UITabContainer>();
            container.relativePosition = new Vector3(0, 40);
            container.size = new Vector3(744, 713);
            strip.tabPages = container;

            int tabIndex = 0;
            foreach (IModule module in Modules)
            {
                strip.AddTab(module.Name, tabTemplate, true);

                // Get the current container and use the UIHelper to have something in there
                UIPanel stripRoot = strip.tabContainer.components[tabIndex++] as UIPanel;
                stripRoot.autoLayout = true;
                stripRoot.autoLayoutDirection = LayoutDirection.Vertical;
                stripRoot.autoLayoutPadding.top = 5;
                stripRoot.autoLayoutPadding.left = 10;
                UIHelper stripHelper = new UIHelper(stripRoot);

                module.OnSettingsUI(stripHelper);
            }
                
        }

        public void OnSaveSettings()
        {
            XmlDocument settingsDoc = new XmlDocument();
            XmlElement root = settingsDoc.AppendElement("TransitAddonMod");

            root.AppendAttribute("Version", VERSION);

            foreach (IModule module in Modules)
            {
                XmlElement moduleElement = root.AppendElement(module.Name.Replace(' ', '_'));
                module.OnSaveSettings(moduleElement);
            }

            try
            {
                settingsDoc.Save(SETTINGS_FILE);
            }
            catch (System.Exception)
            {
                // TODO: log error saving file
                return;
            }
        }

        public void OnLoadSettings()
        {
            XmlDocument settingsDoc = null;
            if (File.Exists(SETTINGS_FILE))
            {
                settingsDoc = new XmlDocument();
                settingsDoc.Load(SETTINGS_FILE);
            }

            if (settingsDoc == null || settingsDoc.DocumentElement == null)
                return;

            foreach (IModule module in Modules)
            {
                XmlNodeList nodeList = settingsDoc.GetElementsByTagName(module.Name.Replace(' ', '_'));
                if (nodeList.Count > 0)
                    module.OnLoadSettings(nodeList[0] as XmlElement);
            }
        }

        private void CheckForUpdates()
        {
            XmlDocument settingsDoc = null;

            if (File.Exists(SETTINGS_FILE))
            {
                settingsDoc = new XmlDocument();
                settingsDoc.Load(SETTINGS_FILE);
            }

            if (settingsDoc == null || settingsDoc.DocumentElement == null)
            {
                OnSaveSettings();
                NotificationPanel.Panel.Show("Welcome!!!", "Some amazing welcome!", false, "Oh yeah!", null, "On noes! :O", null, true);
                return;
            }

            string fileVersion = settingsDoc.DocumentElement.GetAttribute("Version");
            if (fileVersion != VERSION)
            {
                OnLoadSettings();
                OnSaveSettings(); // Updates the version on file so this only shows once
                NotificationPanel.Panel.Show("Update!!!", "Some amazing description!", false, "Oh yeah!", null, "On noes! :O", null, true);
            }
        }
    }
}
