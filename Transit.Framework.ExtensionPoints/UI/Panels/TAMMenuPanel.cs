using System;
using System.Collections.Generic;
using System.Linq;
using ColossalFramework.Globalization;
using ColossalFramework.UI;
using Transit.Framework.Builders;
using Transit.Framework.ExtensionPoints.UI.Toolbar.Panels;
using UnityEngine;

namespace Transit.Framework.ExtensionPoints.UI.Panels
{
    public class TAMMenuPanel : GeneratedGroupPanel
    {
        public IEnumerable<IMenuCategoryBuilder> CategoryBuilders { get; set; }

        protected sealed override bool CustomRefreshPanel()
        {
            if (CategoryBuilders != null)
            {
                foreach (var info in CategoryBuilders.OrderBy(c => c.Order))
                {
                    if (info is IToolMenuCategoryBuilder)
                    {
                        SpawnCategory((IToolMenuCategoryBuilder)info, null, "SubBar", null, true);
                    }
                    else
                    {
                        throw new NotImplementedException("TAMMenuPanel only support IToolMenuCategoryBuilder (for now)");
                    }
                }
            }

            return true;
        }

        protected virtual UIButton SpawnCategory(IToolMenuCategoryBuilder categoryBuilder, string localeID, string spriteBase, string unlockText, bool enabled)
        {
            Type panelType = typeof (TAMMenuCategoryPanel);
            string category = categoryBuilder.Name;
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
                panel.ToolBuilders = categoryBuilder.ToolBuilders;
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
