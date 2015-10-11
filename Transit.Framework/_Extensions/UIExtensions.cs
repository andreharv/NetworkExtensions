using System;
using ColossalFramework.UI;
using ICities;
using ColossalFramework.Plugins;

namespace Transit.Framework
{
    public static class UIExtensions
    {
        private const string CHECKBOX_TEMPLATE = "OptionsCheckBoxTemplate";
        private const string CHECKBOX_TEMPLATE_CHECKED_SPRITE = "AchievementCheckedTrue";
        private const string CHECKBOX_TEMPLATE_UNCHECKED_SPRITE = "AchievementCheckedFalse";

        public static UICheckBox AddCheckbox(this UIHelperBase uiHelper, string name, string tooltip, bool defaultValue, OnCheckChanged onChangedCallback, bool useSquareSprites = false, object objUserData = null)
        {
            UICheckBox checkbox = uiHelper.AddCheckbox(name, defaultValue, onChangedCallback) as UICheckBox;
            checkbox.tooltip = tooltip;
            checkbox.objectUserData = objUserData;
            checkbox.FitTo(((UIHelper)uiHelper).self as UIComponent, LayoutDirection.Horizontal);
            
            if (useSquareSprites)
            {
                // Change the checkbox sprites
                UISprite sprite = checkbox.checkedBoxObject as UISprite;
                sprite.spriteName = CHECKBOX_TEMPLATE_CHECKED_SPRITE;
                sprite.transform.parent.GetComponent<UISprite>().spriteName = CHECKBOX_TEMPLATE_UNCHECKED_SPRITE;
            }

            return checkbox;
        }

        public static UICheckBox AddCheckbox(this UIHelperBase uiHelper, string name, string tooltip, bool defaultValue, PropertyChangedEventHandler<bool> onChangedCallback, bool useSquareSprites = false, object objUserData = null)
        {
            if (onChangedCallback != null && !string.IsNullOrEmpty(name))
            {
                UICheckBox uICheckBox = ((UIComponent)(((UIHelper)uiHelper).self)).AttachUIComponent(UITemplateManager.GetAsGameObject(CHECKBOX_TEMPLATE)) as UICheckBox;
                uICheckBox.text = name;
                uICheckBox.tooltip = tooltip;
                uICheckBox.objectUserData = objUserData;
                uICheckBox.isChecked = defaultValue;
                uICheckBox.eventCheckChanged += onChangedCallback;

                if (useSquareSprites)
                {
                    // Change the checkbox sprites
                    UISprite sprite = uICheckBox.checkedBoxObject as UISprite;
                    sprite.spriteName = CHECKBOX_TEMPLATE_CHECKED_SPRITE;
                    sprite.transform.parent.GetComponent<UISprite>().spriteName = CHECKBOX_TEMPLATE_UNCHECKED_SPRITE;
                }

                return uICheckBox;
            }
            DebugOutputPanel.AddMessage(PluginManager.MessageType.Warning, "Cannot create checkbox with no name or no event");
            return null;
        }
    }
}
