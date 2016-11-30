using ColossalFramework.UI;
using ICities;
using Transit.Framework.Modularity;
using UnityEngine;

namespace Transit.Addon
{
    public partial class Mod
    {
        protected override string SettingsFile
        {
            get { return "TransitAddonModSettings.xml"; }
        }

        protected override string SettingsNode
        {
            get { return "TransitAddonMod"; }
        }

        private UIScrollablePanel _optionsPanel;

        public void OnSettingsUI(UIHelperBase helper)
        {
            var tabTemplate = Resources
                .FindObjectsOfTypeAll<UITabstrip>()[0]
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
    }
}
