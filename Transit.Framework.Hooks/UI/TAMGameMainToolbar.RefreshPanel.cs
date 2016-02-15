using ColossalFramework;
using ColossalFramework.Globalization;
using ColossalFramework.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Transit.Framework.ExtensionPoints.UI;
using Transit.Framework.Redirection;
using Transit.Framework.UI.Menus.Toolbar.Items;

namespace Transit.Framework.Hooks.UI
{
    public partial class TAMGameMainToolbar : MainToolbar
    {
        private class VanillaToolbarItemInfo : IToolbarItemInfo
        {
            public string Name { get; set; }
            public string UnlockText { get; set; }
            public bool Enabled { get; set; }
            public int Order { get; set; }
        }

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

            var items = new List<IToolbarItemInfo>
            {
                new VanillaToolbarItemInfo
                {
                    Name = "Zoning",
                    UnlockText = GetUnlockText(UnlockManager.Feature.Zoning),
                    Enabled = ZoningPanel.IsZoningPossible(),
                    Order = 20
                },
                new VanillaToolbarItemInfo
                {
                    Name = "District",
                    UnlockText = GetUnlockText(UnlockManager.Feature.Districts),
                    Enabled = IsUnlocked(UnlockManager.Feature.Districts),
                    Order = 30
                },
                new ToolbarBigSeparatorItemInfo(40)
            };

            int[] format = { 3, 6, 8 };
            int formatIndex = 0, orderCount = 1;
            for (int i = 0; i < kServices.Length; i++)
            {
                items.Add(new VanillaToolbarItemInfo
                {
                    Name = kServices[i].enumName,
                    UnlockText = GetUnlockText(kServices[i].enumValue),
                    Enabled = IsUnlocked(kServices[i].enumValue),
                    Order = orderCount * 10
                });

                if (orderCount == 1)
                {
                    orderCount = 5;
                }

                if (format.Length > formatIndex && format[formatIndex] == i)
                {
                    items.Add(new ToolbarSmallSeparatorItemInfo(++orderCount * 10));
                    ++formatIndex;
                }

                ++orderCount;
            }

            items.Add(new VanillaToolbarItemInfo
            {
                Name = "Wonders",
                UnlockText = GetUnlockText(UnlockManager.Feature.Wonders),
                Enabled = IsUnlocked(UnlockManager.Feature.Wonders),
                Order = 180
            });

            items.Add(new ToolbarBigSeparatorItemInfo(190));
            items.AddRange(GameMainToolbarItemsManager.CustomEntries);

            foreach (var entry in items.OrderBy(e => e.Order))
            {
                if (entry is ToolbarSmallSeparatorItemInfo)
                {
                    SpawnSmallSeparator(uiTabstrip);
                }
                else if (entry is ToolbarBigSeparatorItemInfo)
                {
                    SpawnSeparator(uiTabstrip);
                }
                else if (entry is VanillaToolbarItemInfo)
                {
                    var info = entry as VanillaToolbarItemInfo;

                    SpawnSubEntry(uiTabstrip, info.Name, "MAIN_TOOL", info.UnlockText, "ToolbarIcon", info.Enabled);
                }
                else if (entry is IToolbarMenuItemInfo)
                {
                    var info = entry as IToolbarMenuItemInfo;

                    SpawnSubEntry(uiTabstrip, info.Name, "MAIN_TOOL", string.Empty, "ToolbarIcon", true, info.PanelType);
                }
            }

            int policiesIndex, economyIndex;
            if (policiesPanel.m_DockingPosition == PoliciesPanel.DockingPosition.Left)
            {
                UIButton uIButton = SpawnButtonEntry(uiTabstrip, "Policies", "MAIN_TOOL", GetUnlockText(UnlockManager.Feature.Policies), "ToolbarIcon", IsUnlocked(UnlockManager.Feature.Policies));
                policiesIndex = uIButton.zOrder;
                policiesPanel.SetParentButton(uIButton);
                UIButton uIButton2 = SpawnButtonEntry(uiTabstrip, "Money", "MAIN_TOOL", GetUnlockText(UnlockManager.Feature.Economy), "ToolbarIcon", IsUnlocked(UnlockManager.Feature.Economy));
                economyIndex = uIButton2.zOrder;
                economyPanel.SetParentButton(uIButton2);
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
                uiTabstrip.tabPages.components[policiesIndex].eventVisibilityChanged += this.ShowHidePoliciesPanel;
                uiTabstrip.tabPages.components[economyIndex].eventVisibilityChanged += this.ShowHideEconomyPanel;
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
