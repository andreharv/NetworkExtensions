using System.IO;
using System.Xml;
using ColossalFramework.UI;
using ICities;
using Transit.Framework;
using Transit.Framework.Interfaces;
using Transit.Framework.Modularity;
using UnityEngine;

namespace NetworkExtensions
{
    public partial class Mod
    {
        // Change the version only when there are new options
        private const string VERSION = "1.0.0";
        private const string SETTINGS_FILE = "NetworkExtensionsConfig.xml";

        private UIScrollablePanel _optionsPanel;
        private const string NAME = "Network Extensions";
        private const string DESCRIPTION = "An addition of highways and roads";

        public void OnSettingsUI(UIHelperBase helper)
        {
            LoadModulesIfNeeded();
            LoadSettings();

            UIButton tabTemplate = Resources.FindObjectsOfTypeAll<OptionsKeymappingPanel>()[0]
                                            .GetComponentInChildren<UITabstrip>()
                                            .GetComponentInChildren<UIButton>();

            _optionsPanel = ((UIHelper)helper).self as UIScrollablePanel;
            _optionsPanel.autoLayout = false;

            if (IsTAMInstalled)
            {
                UILabel label = _optionsPanel.AddUIComponent<UILabel>();
                label.text = "Transit Addons Mod (TAM) has been detected - Network Extensions have been disabled";
                return;
            }

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

        protected override void SaveSettings()
        {
            var settingsDoc = new XmlDocument();
            var root = settingsDoc.AppendElement("NetworkExtensions");

            root.AppendAttribute("Version", VERSION);

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

        private void LoadSettings()
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
        }
    }
}
