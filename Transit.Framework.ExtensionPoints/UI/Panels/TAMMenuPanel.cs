using System;
using System.Collections.Generic;
using System.Linq;
using ColossalFramework.Globalization;
using ColossalFramework.UI;
using Transit.Framework.Builders;
using UnityEngine;

namespace Transit.Framework.ExtensionPoints.UI.Toolbar.Panels
{
    public class TAMMenuPanel : GeneratedGroupPanel
    {
        public IEnumerable<IMenuCategoryBuilder> CategoryInfos { get; set; }

        protected sealed override bool CustomRefreshPanel()
        {
            if (CategoryInfos != null)
            {
                foreach (var info in CategoryInfos.OrderBy(c => c.Order))
                {
                    SpawnCategory(info, null, "SubBar", null, true);
                }
            }

            return true;
        }

        protected virtual UIButton SpawnCategory(IMenuCategoryBuilder categoryInfo, string localeID, string spriteBase, string unlockText, bool enabled)
        {
            Type panelType = typeof (TAMMenuCategoryPanel);
            string category = categoryInfo.Name;
            int objectIndex = this.GetFieldValue<int>("m_ObjectIndex");

            UIButton uiButton = null;
            if (m_Strip.childCount > objectIndex)
            {
                uiButton = m_Strip.components[objectIndex] as UIButton;
            }
            else
            {
                GameObject subbarButtonTemplate = UITemplateManager.GetAsGameObject("SubbarButtonTemplate");
                GameObject subbarPanelTemplate = UITemplateManager.GetAsGameObject("SubbarPanelTemplate");
                uiButton = m_Strip.AddTab(category, subbarButtonTemplate, subbarPanelTemplate, new Type[] { panelType }) as UIButton;
            }

            uiButton.isEnabled = enabled;
            uiButton.isVisible = true;
            uiButton.gameObject.GetComponent<TutorialUITag>().tutorialTag = category;
            uiButton.group = m_Strip;

            TAMMenuCategoryPanel panel = m_Strip.GetComponentInContainer(uiButton, panelType) as TAMMenuCategoryPanel;
            if (panel != null)
            {
                panel.ToolBuilders = categoryInfo.ToolBuilders;
                panel.name = category;
                panel.component.isInteractive = true;
                panel.m_OptionsBar = this.m_OptionsBar;
                panel.m_DefaultInfoTooltipAtlas = this.m_DefaultInfoTooltipAtlas;
                if (enabled)
                {
                    panel.category = category ?? string.Empty;
                    panel.RefreshPanel();
                }
            }

            string spriteName = spriteBase + category;
            uiButton.normalFgSprite = spriteName;
            uiButton.focusedFgSprite = spriteName + "Focused";
            uiButton.hoveredFgSprite = spriteName + "Hovered";
            uiButton.pressedFgSprite = spriteName + "Pressed";
            uiButton.disabledFgSprite = spriteName + "Disabled";

            if (!string.IsNullOrEmpty(localeID) && !string.IsNullOrEmpty(unlockText))
            {
                uiButton.tooltip = Locale.Get(localeID, category) + " - " + unlockText;
            }
            else if (!string.IsNullOrEmpty(localeID))
            {
                uiButton.tooltip = Locale.Get(localeID, category);
            }

            this.SetFieldValue("m_ObjectIndex", objectIndex + 1);
            return uiButton;
        }
    }
}
