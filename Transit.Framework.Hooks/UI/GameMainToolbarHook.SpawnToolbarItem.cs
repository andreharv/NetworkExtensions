using System;
using System.Linq;
using ColossalFramework.Globalization;
using ColossalFramework.UI;
using System.Reflection;
using Transit.Framework.Builders;
using Transit.Framework.ExtensionPoints.UI.Panels;
using Transit.Framework.ExtensionPoints.UI.Toolbar.Panels;
using UnityEngine;

namespace Transit.Framework.Hooks.UI
{
    public partial class GameMainToolbarHook
    {
        private UIButton SpawnToolbarItem(IToolbarItemBuilder info, UITabstrip strip, string unlockText, string spriteBase, bool enabled)
        {
            string name = info.Name;
            Type panelType = typeof(TAMMenuPanel);
            int objectIndex = this.GetFieldValue<int>("m_ObjectIndex");

            UIButton uIButton;
            if (strip.childCount > objectIndex)
            {
                uIButton = (strip.components[objectIndex] as UIButton);
            }
            else
            {
                GameObject asGameObject = UITemplateManager.GetAsGameObject(kMainToolbarButtonTemplate);
                GameObject asGameObject2 = UITemplateManager.GetAsGameObject(kScrollableSubPanelTemplate);
                uIButton = (strip.AddTab(name, asGameObject, asGameObject2, new Type[] { panelType }) as UIButton);
            }

            uIButton.isEnabled = enabled;
            uIButton.gameObject.GetComponent<TutorialUITag>().tutorialTag = name;
            TAMMenuPanel panel = strip.GetComponentInContainer(uIButton, panelType) as TAMMenuPanel;
            if (panel != null)
            {
                panel.CategoryBuilders = info.MenuBuilder.CategoryBuilders;
                panel.component.isInteractive = true;
                panel.m_OptionsBar = this.m_OptionsBar;
                panel.m_DefaultInfoTooltipAtlas = this.m_DefaultInfoTooltipAtlas;
                if (enabled)
                {
                    panel.RefreshPanel();
                }
            }

            var customAtlas = AtlasManager.instance.GetAtlas(name);
            if (customAtlas != null)
            {
                uIButton.atlas = customAtlas;
            }

            uIButton.normalBgSprite = this.GetBackgroundSprite(uIButton, spriteBase, name, "Normal");
            uIButton.focusedBgSprite = this.GetBackgroundSprite(uIButton, spriteBase, name, "Focused");
            uIButton.hoveredBgSprite = this.GetBackgroundSprite(uIButton, spriteBase, name, "Hovered");
            uIButton.pressedBgSprite = this.GetBackgroundSprite(uIButton, spriteBase, name, "Pressed");
            uIButton.disabledBgSprite = this.GetBackgroundSprite(uIButton, spriteBase, name, "Disabled");
            string text = spriteBase + name;
            uIButton.normalFgSprite = text;
            uIButton.focusedFgSprite = text + "Focused";
            uIButton.hoveredFgSprite = text + "Hovered";
            uIButton.pressedFgSprite = text + "Pressed";
            uIButton.disabledFgSprite = text + "Disabled";
            uIButton.tooltip = info.DisplayName;

            this.SetFieldValue("m_ObjectIndex", objectIndex + 1);
            return uIButton;
        }
    }
}
