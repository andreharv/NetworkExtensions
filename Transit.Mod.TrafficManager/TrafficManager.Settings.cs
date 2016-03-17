using System;
using ICities;
using Transit.Framework.Modularity;
using UnityEngine;
using ColossalFramework.UI;

namespace Transit.Mod
{
    public partial class TrafficManager
    {
        private UIScrollablePanel _optionsPanel;

        public void OnSettingsUI(UIHelperBase helper)
        {
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

        protected override string SettingsFile
        {
            get
            {
                throw new NotImplementedException(); // Manually handled by module
            }
        }

        protected override string SettingsNode
        {
            get
            {
                throw new NotImplementedException(); // Manually handled by module
            }
        }

        public override void LoadSettings()
        {
            //base.LoadSettings(); Manually handled by module
        }

        public override void SaveSettings()
        {
            //base.SaveSettings(); Manually handled by module
        }
    }
}
