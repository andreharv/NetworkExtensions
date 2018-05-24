using ColossalFramework.UI;
using ICities;
using Transit.Framework.Modularity;
using UnityEngine;

namespace NetworkExtensions
{
    public partial class Mod
    {
        protected override string SettingsFile
        {
            get { return "NetworkExtensions2Config.xml"; }
        }

        protected override string SettingsNode
        {
            get { return "NetworkExtensions2"; }
        }

        private UIScrollablePanel _optionsPanel;

        public void OnSettingsUI(UIHelperBase helper)
        {
            UIButton tabTemplate = Resources
                .FindObjectsOfTypeAll<UITabstrip>()[1]
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

            foreach (IModule module in Modules)
            {
                if (module.Name == "Roads")
                {
                    addTab(strip, strip.tabCount, module, tabTemplate, "Tiny", "RoadsTiny");
                    addTab(strip, strip.tabCount, module, tabTemplate, "Small", "RoadsSmall");
                    addTab(strip, strip.tabCount, module, tabTemplate, "Sml Hvy", "RoadsSmallHV");
                    addTab(strip, strip.tabCount, module, tabTemplate, "Medium", "RoadsMedium");
                    addTab(strip, strip.tabCount, module, tabTemplate, "Large", "RoadsLarge");
                    addTab(strip, strip.tabCount, module, tabTemplate, "Highway", "RoadsHighway");
                    addTab(strip, strip.tabCount, module, tabTemplate, "Ped", "RoadsPedestrians");
                    addTab(strip, strip.tabCount, module, tabTemplate, "Bus", "RoadsBusways");
                }
                else if (module.Name == "Tools")
                {
                    addTab(strip, strip.tabCount, module, tabTemplate);
                }
            }
        }
        private static void addTab(UITabstrip strip, int tabIndex, IModule module, UIButton tabTemplate, string moduleName = "", string uiCategory = "")
        {
            if (moduleName == "")
            {
                moduleName = module.Name;
            }
            strip.AddTab(moduleName, tabTemplate, true);
            strip.selectedIndex = tabIndex;

            // Get the current container and use the UIHelper to have something in there
            UIPanel stripRoot = strip.tabContainer.components[tabIndex] as UIPanel;
            stripRoot.autoLayout = true;
            stripRoot.autoLayoutDirection = LayoutDirection.Vertical;
            stripRoot.autoLayoutPadding.top = 5;
            stripRoot.autoLayoutPadding.left = 10;
            stripRoot.name = $"{uiCategory}";
            UIHelper stripHelper = new UIHelper(stripRoot);

            module.OnSettingsUI(stripHelper);
        }
    }
}
