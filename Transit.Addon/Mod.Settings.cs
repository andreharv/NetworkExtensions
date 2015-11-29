using System.IO;
using System.Xml;
using ICities;
using Transit.Framework;
using Transit.Framework.Interfaces;
using Transit.Framework.Modularity;
using ColossalFramework.UI;
using UnityEngine;
using System.Xml.Serialization;
using System;

namespace Transit.Addon
{
    public partial class Mod
    {
        // Change the version only when there are new options
        // and update the notification message!
        private const string VERSION = "0.0.1";
        private const string SETTINGS_FILE = "TransitAddonModSettings.xml";

        private UIScrollablePanel _optionsPanel;
        private string _name = "Transit Addons Mod";
        private string _description = "Closed Beta";
        private string _lastNotificationID = "NONE";

        public void OnSettingsUI(UIHelperBase helper)
        {
            LoadSettings();

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
                strip.selectedIndex = tabIndex;

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

        private void ModuleSettingsNeedSave()
        {
            SaveSettings();
        }

        private void SaveSettings()
        {
            var settingsDoc = new XmlDocument();
            var root = settingsDoc.AppendElement("TransitAddonMod");

            root.AppendAttribute("Version", VERSION);
            root.AppendAttribute("LastNotificationID", _lastNotificationID);

            foreach (IModule module in Modules)
            {
                XmlElement moduleElement = root.AppendElement(module.GetCodeName());
                module.OnSaveSettings(moduleElement);
            }

            try
            {
                settingsDoc.Save(SETTINGS_FILE);
            }
            catch (System.Exception)
            {
                // TODO: log error saving file
            }
        }

        private XmlDocument LoadSettings()
        {
            XmlDocument settingsDoc = null;
            if (File.Exists(SETTINGS_FILE))
            {
                settingsDoc = new XmlDocument();
                settingsDoc.Load(SETTINGS_FILE);
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

            return settingsDoc;
        }

        private void CheckForUpdates()
        {
            var settingsDoc = LoadSettings();

            if (settingsDoc == null)
            {
                SaveSettings();
                //NotificationPanel.Panel.Show("Welcome!!!", "Some amazing welcome!", false, "Oh yeah!", null, "On noes! :O", null, true);
                return;
            }

            var settingsDocElement = settingsDoc.DocumentElement;
            var fileVersion = settingsDocElement == null ? string.Empty : settingsDocElement.GetAttribute("Version");
            if (fileVersion != VERSION)
            {
                SaveSettings(); // Updates the version on file so this only shows once
                //NotificationPanel.Panel.Show("Update!!!", "Some amazing description!", false, "Oh yeah!", null, "On noes! :O", null, true);
            }
        }

        private void ShowNotification()
        {
            var settingsDoc = LoadSettings();
            if (settingsDoc == null)
            {
                SaveSettings();
                settingsDoc = LoadSettings();
            }

            var settingsDocElement = settingsDoc.DocumentElement;
            _lastNotificationID = settingsDocElement == null ? "None" : settingsDocElement.GetAttribute("LastNotificationID");

            NotificationInfo notification;
            try
            {
                var assetPath = this.GetAssetPath();
                if (assetPath == Assets.PATH_NOT_FOUND)
                {
                    return;
                }

                var xmlSerializer = new XmlSerializer(typeof(NotificationInfo));
                using (StreamReader streamReader = new StreamReader(Path.Combine(assetPath, "NotificationSettings.xml")))
                {
                    notification = (NotificationInfo)xmlSerializer.Deserialize(streamReader);
                }
            }
            catch
            {
                return;
            }

            _name = notification.modName;
            _description = notification.modDescription;

            if (notification.notificationID == _lastNotificationID)
                return;

            _lastNotificationID = notification.notificationID;

            if (String.IsNullOrEmpty(notification.url))
            {
                NotificationPanel.Panel.Show(notification.title,
                    notification.description.Replace(@"\n", Environment.NewLine),
                    notification.showScrollbar,
                    notification.playFireworks);
            }
            else
            {
                NotificationPanel.Panel.Show(notification.title,
                    notification.description.Replace(@"\n", Environment.NewLine),
                    notification.showScrollbar,
                    notification.url,
                    notification.urlButtonText,
                    notification.playFireworks);
            }

            SaveSettings();
        }

        public struct NotificationInfo
        {
            public string modName;
            public string modDescription;
            public string notificationID;
            public string title;
            public string description;
            public string url;
            public string urlButtonText;
            public bool showScrollbar;
            public bool playFireworks;
        }
    }
}
