using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using ColossalFramework;
using ColossalFramework.Globalization;
using ColossalFramework.UI;
using Transit.Framework.ExtensionPoints.UI;
using Transit.Framework.Redirection;
using UnityEngine;

namespace Transit.Framework.Hooks.UI
{
    public class TAMGameMainToolbar : GameMainToolbar
    {
        private static int s_lastSelection = -1;

        private static readonly PositionData<ItemClass.Service>[] kServices = Utils.GetOrderedEnumData<ItemClass.Service>("Game");
        private const string kMainToolbarButtonTemplate = "MainToolbarButtonTemplate";
        private const string kScrollableSubPanelTemplate = "ScrollableSubPanelTemplate";

        #region Redirected Methods

        [RedirectFrom(typeof(GameMainToolbar))]
        internal new void RefreshPanel()
        {
            m_IsRefreshing = true;

            UITabstrip uiTabstrip = mainToolbar.component as UITabstrip;
            s_lastSelection = uiTabstrip.selectedIndex;
            uiTabstrip.selectedIndex = -1;

            base.RefreshPanel();

            string unlockText = GetUnlockText(UnlockManager.Feature.Bulldozer);
            if (unlockText != null)
            {
                m_BulldozerButton.tooltip = Locale.Get("MAIN_TOOL", "Bulldozer") + " - " + unlockText;
            }

            var entries = new List<ToolbarItemInfo>
            {
                new ToolbarItemInfo
                {
                    Name = "Zoning",
                    UnlockText = GetUnlockText(UnlockManager.Feature.Zoning),
                    SpriteBase = "ToolbarIcon",
                    Enabled = ZoningPanel.IsZoningPossible(),
                    Order = 20
                },
                new ToolbarItemInfo
                {
                    Name = "District",
                    UnlockText = GetUnlockText(UnlockManager.Feature.Districts),
                    SpriteBase = "ToolbarIcon",
                    Enabled = IsUnlocked(UnlockManager.Feature.Districts),
                    Order = 30
                },
                new ToolbarItemInfo
                {
                    Order = 40,
                    BigSeparator = true
                }
            };

            int[] format = new int[] { 3, 6, 8 };
            int formatIndex = 0, orderCount = 1;
            for (int i = 0; i < kServices.Length; i++)
            {
                entries.Add(new ToolbarItemInfo
                {
                    Name = kServices[i].enumName,
                    UnlockText = GetUnlockText(kServices[i].enumValue),
                    SpriteBase = "ToolbarIcon",
                    Enabled = IsUnlocked(kServices[i].enumValue),
                    Order = orderCount * 10
                });

                if (orderCount == 1)
                {
                    orderCount = 5;
                }

                if (format.Length > formatIndex && format[formatIndex] == i)
                {
                    entries.Add(new ToolbarItemInfo
                    {
                        Order = ++orderCount * 10,
                        SmallSeparator = true
                    });
                    ++formatIndex;
                }

                ++orderCount;
            }

            entries.Add(new ToolbarItemInfo
            {
                Name = "Wonders",
                UnlockText = GetUnlockText(UnlockManager.Feature.Wonders),
                SpriteBase = "ToolbarIcon",
                Enabled = IsUnlocked(UnlockManager.Feature.Wonders),
                Order = 180
            });

            entries.Add(new ToolbarItemInfo
            {
                Order = 190,
                BigSeparator = true
            });

            entries.AddRange(GameMainToolbarItemsProvider.CustomEntries);
            entries = entries.OrderBy(e => e.Order).ToList();

            foreach (var entry in entries)
            {
                if (entry.SmallSeparator)
                {
                    SpawnSmallSeparator(uiTabstrip);
                }
                else if (entry.BigSeparator)
                {
                    SpawnSeparator(uiTabstrip);
                }
                else if (entry.Type != null)
                {
                    SpawnEntry(uiTabstrip, entry.Name, "MAIN_TOOL", entry.UnlockText, entry.SpriteBase, entry.Enabled, entry.Type);
                }
                else
                {
                    SpawnSubEntry(uiTabstrip, entry.Name, "MAIN_TOOL", entry.UnlockText, entry.SpriteBase, entry.Enabled);
                }
            }

            int policiesIndex, economyIndex;
            if (policiesPanel.m_DockingPosition == PoliciesPanel.DockingPosition.Left)
            {
                UIButton uIButton = SpawnButtonEntry(uiTabstrip, "Policies", "MAIN_TOOL", GetUnlockText(UnlockManager.Feature.Policies), "ToolbarIcon", IsUnlocked(UnlockManager.Feature.Policies));
                policiesIndex = uIButton.zOrder;
                ToolsModifierControl.policiesPanel.SetParentButton(uIButton);
                UIButton uIButton2 = SpawnButtonEntry(uiTabstrip, "Money", "MAIN_TOOL", GetUnlockText(UnlockManager.Feature.Economy), "ToolbarIcon", IsUnlocked(UnlockManager.Feature.Economy));
                economyIndex = uIButton2.zOrder;
                ToolsModifierControl.economyPanel.SetParentButton(uIButton2);
            }
            else
            {
                UIButton uIButton3 = SpawnButtonEntry(uiTabstrip, "Money", "MAIN_TOOL", GetUnlockText(UnlockManager.Feature.Economy), "ToolbarIcon", IsUnlocked(UnlockManager.Feature.Economy));
                economyIndex = uIButton3.zOrder;
                ToolsModifierControl.economyPanel.SetParentButton(uIButton3);
                UIButton uIButton4 = SpawnButtonEntry(uiTabstrip, "Policies", "MAIN_TOOL", GetUnlockText(UnlockManager.Feature.Policies), "ToolbarIcon", IsUnlocked(UnlockManager.Feature.Policies));
                policiesIndex = uIButton4.zOrder;
                ToolsModifierControl.policiesPanel.SetParentButton(uIButton4);
            }

            FieldInfo m_eventsRegistered = typeof(GameMainToolbar).GetField("m_EventsRegistered", BindingFlags.Instance | BindingFlags.NonPublic);
            if (!(bool)m_eventsRegistered.GetValue(this))
            {
                uiTabstrip.tabPages.components[policiesIndex].eventVisibilityChanged += new PropertyChangedEventHandler<bool>(this.ShowHidePoliciesPanel);
                uiTabstrip.tabPages.components[economyIndex].eventVisibilityChanged += new PropertyChangedEventHandler<bool>(this.ShowHideEconomyPanel);
            }
            m_eventsRegistered.SetValue(this, true);

            if (s_lastSelection != -1)
            {
                uiTabstrip.selectedIndex = s_lastSelection;
            }
            s_lastSelection = -1;

            // set m_PoliciesIndex and m_EconomyIndex
            typeof(GameMainToolbar).GetField("m_PoliciesIndex", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(this, policiesIndex);
            typeof(GameMainToolbar).GetField("m_EconomyIndex", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(this, economyIndex);

            m_IsRefreshing = false;
        }

        internal UIButton SpawnEntry(UITabstrip strip, string name, string localeID, string unlockText, string spriteBase, bool enabled, Type type = null)
        {
            int objectIndex = (int)typeof(MainToolbar).GetField("m_ObjectIndex", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(this);

            string groupName = name + "Group";
            if (type == null)
            {
                Debug.Log("Custom Entry " + name + " without type");
                type = Type.GetType(groupName + "Panel");
            }


            if (type != null && !type.IsSubclassOf(typeof(GeneratedGroupPanel)))
                type = null;

            if (type == null)
                return null;

            UIButton uiButton;
            if (strip.childCount > objectIndex)
            {
                uiButton = strip.components[objectIndex] as UIButton;
            }
            else
            {
                GameObject mainToolbarButtonTemplate = UITemplateManager.GetAsGameObject(kMainToolbarButtonTemplate);
                GameObject scrollableSubPanelTemplate = UITemplateManager.GetAsGameObject(kScrollableSubPanelTemplate);
                uiButton = strip.AddTab(name, mainToolbarButtonTemplate, scrollableSubPanelTemplate, new Type[] { type }) as UIButton;
            }

            uiButton.isEnabled = enabled;
            uiButton.GetComponent<TutorialUITag>().tutorialTag = name;

            GeneratedGroupPanel generatedGroupPanel = strip.GetComponentInContainer(uiButton, type) as GeneratedGroupPanel;
            if (generatedGroupPanel != null)
            {
                generatedGroupPanel.component.isInteractive = true;
                generatedGroupPanel.m_OptionsBar = m_OptionsBar;
                generatedGroupPanel.m_DefaultInfoTooltipAtlas = m_DefaultInfoTooltipAtlas;
                if (enabled)
                    generatedGroupPanel.RefreshPanel();
            }


            uiButton.normalBgSprite = GetBackgroundSprite(uiButton, spriteBase, name, "Normal");
            uiButton.focusedBgSprite = GetBackgroundSprite(uiButton, spriteBase, name, "Focused");
            uiButton.hoveredBgSprite = GetBackgroundSprite(uiButton, spriteBase, name, "Hovered");
            uiButton.pressedBgSprite = GetBackgroundSprite(uiButton, spriteBase, name, "Pressed");
            uiButton.disabledBgSprite = GetBackgroundSprite(uiButton, spriteBase, name, "Disabled");

            string fgSpriteBase = spriteBase + name;
            uiButton.normalFgSprite = fgSpriteBase;
            uiButton.focusedFgSprite = fgSpriteBase + "Focused";
            uiButton.hoveredFgSprite = fgSpriteBase + "Hovered";
            uiButton.pressedFgSprite = fgSpriteBase + "Pressed";
            uiButton.disabledFgSprite = fgSpriteBase + "Disabled";

            if (unlockText != null)
                uiButton.tooltip = Locale.Get(localeID, name) + " - " + unlockText;
            else
                uiButton.tooltip = Locale.Get(localeID, name);

            typeof(MainToolbar).GetField("m_ObjectIndex", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(this, objectIndex + 1);
            return uiButton;
        }

        #region Proxy Methods

        [MethodImpl(MethodImplOptions.NoInlining)]
        [RedirectTo(typeof(GameMainToolbar))]
        private string GetUnlockText(ItemClass.Service service)
        {
            throw new NotImplementedException("GetUnlockText is target of redirection and is not implemented.");
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        [RedirectTo(typeof(GameMainToolbar))]
        private string GetUnlockText(UnlockManager.Feature feature)
        {
            throw new NotImplementedException("GetUnlockText is target of redirection and is not implemented.");
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        [RedirectTo(typeof(GameMainToolbar))]
        private void ShowHidePoliciesPanel(UIComponent comp, bool visible)
        {
            throw new NotImplementedException("ShowHidePoliciesPanel is target of redirection and is not implemented.");
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        [RedirectTo(typeof(GameMainToolbar))]
        private void ShowHideEconomyPanel(UIComponent comp, bool visible)
        {
            throw new NotImplementedException("ShowHideEconomyPanel is target of redirection and is not implemented.");
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        [RedirectTo(typeof(MainToolbar))]
        private string GetBackgroundSprite(UIButton button, string spriteBase, string name, string state)
        {
            throw new NotImplementedException("GetBackgroundSprite is target of redirection and is not implemented.");
        }

        #endregion

        #endregion
    }
}
