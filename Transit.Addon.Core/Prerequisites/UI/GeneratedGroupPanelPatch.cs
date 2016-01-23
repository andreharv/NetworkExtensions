using System;
using System.Linq;
using ColossalFramework.Globalization;
using ColossalFramework.UI;
using Transit.Framework;
using Transit.Framework.Unsafe;
using UnityEngine;

namespace Transit.Addon.Core.Prerequisites.UI
{
    public class GeneratedGroupPanelPatch : GeneratedGroupPanel
    {
        private const string kSubbarButtonTemplate = "SubbarButtonTemplate";
        private const string kSubbarPanelTemplate = "SubbarPanelTemplate";

        [RedirectFrom(typeof (GeneratedGroupPanel))]
        protected UIButton SpawnButtonEntry(UITabstrip strip, string name, string category, bool isDefaultCategory,
            string localeID, string unlockText, string spriteBase, bool enabled, bool forceFillContainer)
        {
            // TAM Edit Start
            Type type = typeof(GeneratedScrollPanel)
                .Assembly
                .GetTypes()
                .FirstOrDefault(t => string.Equals(t.Name, name + "Panel", StringComparison.InvariantCultureIgnoreCase));
            // TAM Edit End

            if (type != null && !type.IsSubclassOf(typeof(GeneratedScrollPanel)))
            {
                type = null;
            }

            // TAM Edit Start
            UIButton uIButton = strip
                .components
                .OfType<UIButton>()
                .FirstOrDefault(b => string.Equals(b.name, category, StringComparison.InvariantCultureIgnoreCase));

            if (uIButton != null)
            {
                return uIButton;
            }
            // TAM Edit End

            GameObject asGameObject = UITemplateManager.GetAsGameObject(kSubbarButtonTemplate);
            GameObject asGameObject2 = UITemplateManager.GetAsGameObject(kSubbarPanelTemplate);
            uIButton = (UIButton)(strip.AddTab(category, asGameObject, asGameObject2, type));
            uIButton.isEnabled = enabled;
            uIButton.gameObject.GetComponent<TutorialUITag>().tutorialTag = category;

            GeneratedScrollPanel generatedScrollPanel = strip.GetComponentInContainer(uIButton, type) as GeneratedScrollPanel;
            if (generatedScrollPanel != null)
            {
                generatedScrollPanel.component.isInteractive = true;
                generatedScrollPanel.m_OptionsBar = m_OptionsBar;
                generatedScrollPanel.m_DefaultInfoTooltipAtlas = m_DefaultInfoTooltipAtlas;
                if (forceFillContainer || enabled)
                {
                    generatedScrollPanel.category = ((!isDefaultCategory) ? category : string.Empty);
                    generatedScrollPanel.RefreshPanel();
                }
            }

            // TAM Edit Start
            if (AtlasProvider.instance.HasCustomAtlas(category))
            {
                uIButton.atlas = AtlasProvider.instance.GetCustomAtlas(category);
            }
            // TAM Edit End

            string text = spriteBase + category;
            uIButton.normalFgSprite = text;
            uIButton.focusedFgSprite = text + "Focused";
            uIButton.hoveredFgSprite = text + "Hovered";
            uIButton.pressedFgSprite = text + "Pressed";
            uIButton.disabledFgSprite = text + "Disabled";

            if (!string.IsNullOrEmpty(localeID) && !string.IsNullOrEmpty(unlockText))
            {
                uIButton.tooltip = Locale.Get(localeID, category) + " - " + unlockText;
            }
            else if (!string.IsNullOrEmpty(localeID))
            {
                uIButton.tooltip = Locale.Get(localeID, category);
            }

            return uIButton;
        }
    }
}
