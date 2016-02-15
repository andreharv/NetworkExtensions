using ColossalFramework.Globalization;
using ColossalFramework.UI;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Transit.Framework.UI.Menus
{
    public abstract class CustomGroupPanelBase : GeneratedGroupPanel
    {
        private bool _panelRefreshed;

        protected UITabstrip _modesBar;

        protected sealed override bool CustomRefreshPanel()
        {
            if (_panelRefreshed)
                return true;

            Initialize();

            _panelRefreshed = true;
            return true;
        }

        protected abstract void Initialize();

        protected virtual UIButton SpawnCategory<T>(string category, string localeID, string spriteBase, string unlockText, bool enabled)
            where T : GeneratedScrollPanel
        {

            var panelType = typeof(T);

            int objectIndex = (int)typeof(GeneratedGroupPanel).GetField("m_ObjectIndex", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(this);

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

            GeneratedScrollPanel generatedScrollPanel = m_Strip.GetComponentInContainer(uiButton, panelType) as GeneratedScrollPanel;
            if (generatedScrollPanel != null)
            {
                generatedScrollPanel.name = category;
                generatedScrollPanel.component.isInteractive = true;
                generatedScrollPanel.m_OptionsBar = this.m_OptionsBar;
                generatedScrollPanel.m_DefaultInfoTooltipAtlas = this.m_DefaultInfoTooltipAtlas;
                if (enabled)
                {
                    generatedScrollPanel.category = category ?? string.Empty;
                    generatedScrollPanel.RefreshPanel();
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

            typeof(GeneratedGroupPanel).GetField("m_ObjectIndex", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(this, objectIndex + 1);
            
            return uiButton;
        }
    }
}
