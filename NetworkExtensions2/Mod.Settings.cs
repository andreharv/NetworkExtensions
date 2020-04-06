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
                    addTab(strip, strip.tabCount, module, "Tiny", "RoadsTiny");
                    addTab(strip, strip.tabCount, module, "Small", "RoadsSmall");
                    addTab(strip, strip.tabCount, module, "Sml Hvy", "RoadsSmallHV");
                    addTab(strip, strip.tabCount, module, "Medium", "RoadsMedium");
                    addTab(strip, strip.tabCount, module, "Large", "RoadsLarge");
                    addTab(strip, strip.tabCount, module, "Highway", "RoadsHighway");
                    addTab(strip, strip.tabCount, module, "Ped", "RoadsPedestrians");
                    addTab(strip, strip.tabCount, module, "Bus", "RoadsBusways");
                }
                else if (module.Name == "Tools")
                {
                    addTab(strip, strip.tabCount, module);
                }
            }
        }
        private static void addTab(UITabstrip strip, int tabIndex, IModule module, string moduleName = "", string uiCategory = "")
        {
            if (moduleName == "")
            {
                moduleName = module.Name;
            }
            UIButton tabButton = strip.AddTab(moduleName);
            tabButton.normalBgSprite = "SubBarButtonBase";
            tabButton.disabledBgSprite = "SubBarButtonBaseDisabled";
            tabButton.focusedBgSprite = "SubBarButtonBaseFocused";
            tabButton.hoveredBgSprite = "SubBarButtonBaseHovered";
            tabButton.pressedBgSprite = "SubBarButtonBasePressed";
            tabButton.textPadding = new RectOffset(10,10,10,10);
            tabButton.autoSize = true;
            tabButton.tooltip = moduleName;

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
