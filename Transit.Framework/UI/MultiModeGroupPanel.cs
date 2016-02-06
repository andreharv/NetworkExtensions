using ColossalFramework.Globalization;
using ColossalFramework.UI;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Transit.Framework.UI
{
    public abstract class MultiModeGroupPanel : GeneratedGroupPanel
    {
        private GameObject _modeButtonTemplate;
        private bool _panelRefreshed;
        private int _numModes;
        private List<Action> _modeActions;
        private Dictionary<string, List<UIComponent>> _modePanels;

        protected UITabstrip _modesBar;

        protected sealed override bool CustomRefreshPanel()
        {
            if (_panelRefreshed)
                return true;

            GetComponent<UIComponent>().eventVisibilityChanged += (component, isVisible) =>
            {
                if (isVisible && _modesBar != null)
                {
                    _modeActions[_modesBar.selectedIndex]();
                }
            };

            _numModes = 0;
            _modeActions = new List<Action>();
            _modePanels = new Dictionary<string, List<UIComponent>>();

            Initialize();

            if (_modesBar != null)
            {
                _modesBar.startSelectedIndex = 0;
                _modesBar.selectedIndex = 0;
            }

            _panelRefreshed = true;
            return true;
        }

        protected abstract void Initialize();

        protected virtual UIButton SpawnMenuEntry(string mode, Type panelType, string name, string localeID, string spriteBase, string unlockText, bool enabled)
        {
            if (panelType != null && !panelType.IsSubclassOf(typeof(GeneratedScrollPanel)))
                panelType = null;

            if (panelType == null)
                return null;

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
                uiButton = m_Strip.AddTab(name, subbarButtonTemplate, subbarPanelTemplate, new Type[] { panelType }) as UIButton;
            }

            uiButton.isEnabled = enabled;
            uiButton.isVisible = true;
            uiButton.gameObject.GetComponent<TutorialUITag>().tutorialTag = name;
            uiButton.group = m_Strip;

            GeneratedScrollPanel generatedScrollPanel = m_Strip.GetComponentInContainer(uiButton, panelType) as GeneratedScrollPanel;
            if (generatedScrollPanel != null)
            {
                generatedScrollPanel.name = name;
                generatedScrollPanel.component.isInteractive = true;
                generatedScrollPanel.m_OptionsBar = this.m_OptionsBar;
                generatedScrollPanel.m_DefaultInfoTooltipAtlas = this.m_DefaultInfoTooltipAtlas;
                if (enabled)
                {
                    generatedScrollPanel.category = name ?? string.Empty;
                    generatedScrollPanel.RefreshPanel();
                }

                if (mode != null)
                {
                    List<UIComponent> panels;
                    if (_modePanels.TryGetValue(mode, out panels))
                    {
                        panels.Add(generatedScrollPanel.component);
                        panels.Add(uiButton);
                    }
                }
            }

            string spriteName = spriteBase + name;
            uiButton.normalFgSprite = spriteName;
            uiButton.focusedFgSprite = spriteName + "Focused";
            uiButton.hoveredFgSprite = spriteName + "Hovered";
            uiButton.pressedFgSprite = spriteName + "Pressed";
            uiButton.disabledFgSprite = spriteName + "Disabled";

            if (!string.IsNullOrEmpty(localeID) && !string.IsNullOrEmpty(unlockText))
            {
                uiButton.tooltip = Locale.Get(localeID, name) + " - " + unlockText;
            }
            else if (!string.IsNullOrEmpty(localeID))
            {
                uiButton.tooltip = Locale.Get(localeID, name);
            }

            typeof(GeneratedGroupPanel).GetField("m_ObjectIndex", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(this, objectIndex + 1);
            
            return uiButton;
        }

        protected UIButton AddMode<T>(string name) 
            where T : ToolBase
        {
            if (_modesBar == null)
                CreateModesPanel();

            var tool = toolController.AddTool<T>();

            GameObject newMode = GameObject.Instantiate(_modeButtonTemplate);
            UIButton modeButton = _modesBar.AttachUIComponent(newMode) as UIButton;

            modeButton.name = name;
            modeButton.tooltip = name;
            modeButton.group = _modesBar;
            modeButton.zOrder = _numModes++;

            var customAtlas = AtlasManager.instance.GetAtlas(name);
            if (customAtlas != null)
            {
                modeButton.atlas = customAtlas;
            }

            modeButton.normalFgSprite = name;
            modeButton.focusedFgSprite = name + "Focused";
            modeButton.hoveredFgSprite = name + "Hovered";
            modeButton.pressedFgSprite = name + "Pressed";
            modeButton.disabledFgSprite = name + "Disabled";

            modeButton.normalBgSprite = "OptionBase";
            modeButton.focusedBgSprite = "OptionBase" + "Focused";
            modeButton.hoveredBgSprite = "OptionBase" + "Hovered";
            modeButton.pressedBgSprite = "OptionBase" + "Pressed";
            modeButton.disabledBgSprite = "OptionBase" + "Disabled";

            Action onClickAction = () =>
            {
                SetTool<T>();

                foreach (KeyValuePair<string, List<UIComponent>> pair in _modePanels)
                {
                    bool belongsToMode = name == pair.Key;
                    bool anySelected = false;
                    for (int i = 0; i < pair.Value.Count; i+=2)
                    {
                        UIComponent panel = pair.Value[i];
                        UIButton btn = pair.Value[i + 1] as UIButton;

                        if (belongsToMode && pair.Value.Count == 2)
                        {
                            panel.isVisible = true;
                            btn.isVisible = false;
                        }
                        else if (belongsToMode && !anySelected)
                        {
                            btn.isVisible = true;
                            panel.isVisible = true;

                            for (int j = 0; j < m_Strip.tabs.Count; j++)
                            {
                                if (m_Strip.tabs[j] == btn)
                                {
                                    m_Strip.selectedIndex = j;
                                    break;
                                }
                            }

                            anySelected = true;
                        }
                        else
                        {
                            panel.isVisible = false;
                            btn.isVisible = belongsToMode;
                        }
                    }
                }
            };

            modeButton.eventClicked += (component, eventParam) =>
            {
                onClickAction();
            };

            _modeActions.Add(onClickAction);
            _modePanels[name] = new List<UIComponent>();

            return modeButton;
        }

        private bool CreateModesPanel()
        {
            if (_modesBar != null)
                return false;

            UIComponent templatePanel = this.m_OptionsBar.Find("ZoningOptionPanel");
            if (templatePanel != null)
            {
                Vector2 relativePos = templatePanel.relativePosition;
                UIComponent newOptionsPanel = GameObject.Instantiate<UIComponent>(templatePanel);

                _modesBar = m_OptionsBar.AttachUIComponent(newOptionsPanel.gameObject) as UITabstrip;
                _modesBar.cachedName = GetType().Name;
                _modesBar.relativePosition = relativePos;
                _modesBar.GetComponent<TutorialUITag>().m_ParentOverride = this.component;

                GameObject.Destroy(_modesBar.GetComponent<ZoningOptionPanel>());
                GameObject.Destroy(_modesBar.GetComponent<TutorialUITag>());

                _modeButtonTemplate = templatePanel.transform.FindChild("Fill").gameObject;

                for (int i = _modesBar.transform.childCount - 1; i >= 0; --i)
                {
                    GameObject.Destroy(_modesBar.transform.GetChild(i).gameObject);
                }

                GetComponent<UIComponent>().eventVisibilityChanged += (component, isVisible) =>
                {
                    _modesBar.isVisible = isVisible;
                };
            }

            return true;
        }
    }
}
