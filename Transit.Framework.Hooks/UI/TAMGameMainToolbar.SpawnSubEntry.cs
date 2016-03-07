using System;
using System.Linq;
using ColossalFramework.Globalization;
using ColossalFramework.UI;
using System.Reflection;
using UnityEngine;

namespace Transit.Framework.Hooks.UI
{
    public partial class TAMGameMainToolbar : MainToolbar
    {
// ReSharper disable once ParameterHidesMember
        private UIButton SpawnSubEntry(UITabstrip strip, string name, string localeID, string unlockText, string spriteBase, bool enabled, Type type)
        {
            int objectIndex = (int)typeof(MainToolbar).GetField("m_ObjectIndex", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(this);
          
            if (type != null && !type.IsSubclassOf(typeof(GeneratedGroupPanel)))
            {
                type = null;
            }

            if (type == null)
            {
                return null;
            }

            UIButton uIButton;
            if (strip.childCount > objectIndex)
            {
                uIButton = (strip.components[objectIndex] as UIButton);
            }
            else
            {
                GameObject asGameObject = UITemplateManager.GetAsGameObject(kMainToolbarButtonTemplate);
                GameObject asGameObject2 = UITemplateManager.GetAsGameObject(kScrollableSubPanelTemplate);
                uIButton = (strip.AddTab(name, asGameObject, asGameObject2, new Type[] { type }) as UIButton);
            }

            uIButton.isEnabled = enabled;
            uIButton.gameObject.GetComponent<TutorialUITag>().tutorialTag = name;
            GeneratedGroupPanel generatedGroupPanel = strip.GetComponentInContainer(uIButton, type) as GeneratedGroupPanel;
            if (generatedGroupPanel != null)
            {
                generatedGroupPanel.component.isInteractive = true;
                generatedGroupPanel.m_OptionsBar = this.m_OptionsBar;
                generatedGroupPanel.m_DefaultInfoTooltipAtlas = this.m_DefaultInfoTooltipAtlas;
                if (enabled)
                {
                    generatedGroupPanel.RefreshPanel();
                }
            }

            // TAM Edit Start
            var customAtlas = AtlasManager.instance.GetAtlas(name);
            if (customAtlas != null)
            {
                uIButton.atlas = customAtlas;
            }
            // TAM Edit End

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

            if (unlockText != null)
            {
                uIButton.tooltip = Locale.Get(localeID, name) + " - " + unlockText;
            }
            else
            {
                uIButton.tooltip = Locale.Get(localeID, name);
            }

            typeof(MainToolbar).GetField("m_ObjectIndex", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(this, objectIndex + 1);
            return uIButton;
        }
    }
}
