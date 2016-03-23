using ColossalFramework;
using ColossalFramework.Globalization;
using ColossalFramework.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Transit.Framework.ExtensionPoints.UI;
using Transit.Framework.ExtensionPoints.UI.Toolbar;
using Transit.Framework.Prerequisites;
using Transit.Framework.Redirection;

namespace Transit.Framework.Hooks.UI
{
    public partial class GameMainToolbarHook : MainToolbar
    {
        private static readonly PositionData<ItemClass.Service>[] kServices = Utils.GetOrderedEnumData<ItemClass.Service>("Game");
        private const string kMainToolbarButtonTemplate = "MainToolbarButtonTemplate";
        private const string kScrollableSubPanelTemplate = "ScrollableSubPanelTemplate";

        [RedirectFrom(typeof(GameMainToolbar), (ulong)PrerequisiteType.UI)]
        public override void RefreshPanel()
        {
            GeneratedPanel.m_IsRefreshing = true;
            UITabstrip uITabstrip = ToolsModifierControl.mainToolbar.component as UITabstrip;
            this.SetFieldValue("m_LastSelection", uITabstrip.selectedIndex);
            uITabstrip.selectedIndex = -1;
            base.RefreshPanel();
            string unlockText = this.GetUnlockText(UnlockManager.Feature.Bulldozer);
            if (unlockText != null)
            {
                this.m_BulldozerButton.tooltip = Locale.Get("MAIN_TOOL", "Bulldozer") + " - " + unlockText;
            }
            int[] array = new int[]
            {
                3,
                3,
                3,
                2,
                3,
                2
            };
            int num = 1;
            int i = 0;
            int num2 = 0;
            while (i < kServices.Length)
            {
                bool enabled = ToolsModifierControl.IsUnlocked(kServices[i].enumValue);
                base.SpawnSubEntry(uITabstrip, kServices[i].enumName, "MAIN_TOOL", this.GetUnlockText(kServices[i].enumValue), "ToolbarIcon", enabled);
                if (i == 0)
                {
                    // TAM Modification
                    var items = MenuManager
                        .instance
                        .ToolbarItems
                        .OrderBy(e => e.Order)
                        .ToArray();

                    if (items.Any())
                    {
                        foreach (var info in items)
                        {
                            SpawnToolbarItem(info, uITabstrip, "MAIN_TOOL", "ToolbarIcon", true);
                        }
                        base.SpawnSmallSeparator(uITabstrip);
                    }
                    // TAM Modification end
                    base.SpawnSubEntry(uITabstrip, "Zoning", "MAIN_TOOL", this.GetUnlockText(UnlockManager.Feature.Zoning), "ToolbarIcon", ZoningPanel.IsZoningPossible());
                    base.SpawnSubEntry(uITabstrip, "District", "MAIN_TOOL", this.GetUnlockText(UnlockManager.Feature.Districts), "ToolbarIcon", ToolsModifierControl.IsUnlocked(UnlockManager.Feature.Districts));
                    base.SpawnSeparator(uITabstrip);
                }
                else if (num2 % array[num] == 0)
                {
                    base.SpawnSmallSeparator(uITabstrip);
                    num++;
                    num2 = 0;
                }
                i++;
                num2++;
            }
            base.SpawnSubEntry(uITabstrip, "Wonders", "MAIN_TOOL", this.GetUnlockText(UnlockManager.Feature.Wonders), "ToolbarIcon", ToolsModifierControl.IsUnlocked(UnlockManager.Feature.Wonders));
            base.SpawnSeparator(uITabstrip);
            base.SpawnSubEntry(uITabstrip, "Landscaping", "MAIN_TOOL", this.GetUnlockText(UnlockManager.Feature.Landscaping), "ToolbarIcon", ToolsModifierControl.IsUnlocked(UnlockManager.Feature.Landscaping));
            base.SpawnSeparator(uITabstrip);
            if (ToolsModifierControl.policiesPanel.m_DockingPosition == PoliciesPanel.DockingPosition.Left)
            {
                UIButton uIButton = base.SpawnButtonEntry(uITabstrip, "Policies", "MAIN_TOOL", this.GetUnlockText(UnlockManager.Feature.Policies), "ToolbarIcon", ToolsModifierControl.IsUnlocked(UnlockManager.Feature.Policies));
                this.SetFieldValue("m_PoliciesIndex", uIButton.zOrder);
                ToolsModifierControl.policiesPanel.SetParentButton(uIButton);
                UIButton uIButton2 = base.SpawnButtonEntry(uITabstrip, "Money", "MAIN_TOOL", this.GetUnlockText(UnlockManager.Feature.Economy), "ToolbarIcon", ToolsModifierControl.IsUnlocked(UnlockManager.Feature.Economy));
                this.SetFieldValue("m_EconomyIndex", uIButton2.zOrder);
                ToolsModifierControl.economyPanel.SetParentButton(uIButton2);
            }
            else
            {
                UIButton uIButton3 = base.SpawnButtonEntry(uITabstrip, "Money", "MAIN_TOOL", this.GetUnlockText(UnlockManager.Feature.Economy), "ToolbarIcon", ToolsModifierControl.IsUnlocked(UnlockManager.Feature.Economy));
                this.SetFieldValue("m_EconomyIndex", uIButton3.zOrder);
                ToolsModifierControl.economyPanel.SetParentButton(uIButton3);
                UIButton uIButton4 = base.SpawnButtonEntry(uITabstrip, "Policies", "MAIN_TOOL", this.GetUnlockText(UnlockManager.Feature.Policies), "ToolbarIcon", ToolsModifierControl.IsUnlocked(UnlockManager.Feature.Policies));
                this.SetFieldValue("m_PoliciesIndex", uIButton4.zOrder);
                ToolsModifierControl.policiesPanel.SetParentButton(uIButton4);
            }
            if (!this.GetFieldValue<bool>("m_EventsRegistered"))
            {
                uITabstrip.tabPages.components[this.GetFieldValue<int>("m_PoliciesIndex")].eventVisibilityChanged += new PropertyChangedEventHandler<bool>(this.ShowHidePoliciesPanel);
                uITabstrip.tabPages.components[this.GetFieldValue<int>("m_EconomyIndex")].eventVisibilityChanged += new PropertyChangedEventHandler<bool>(this.ShowHideEconomyPanel);
            }
            this.SetFieldValue("m_EventsRegistered", true);
            if (this.GetFieldValue<int>("m_LastSelection") != -1)
            {
                uITabstrip.selectedIndex = this.GetFieldValue<int>("m_LastSelection");
            }
            this.SetFieldValue("m_LastSelection", -1);
            GeneratedPanel.m_IsRefreshing = false;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        [RedirectTo(typeof(GameMainToolbar), (ulong)PrerequisiteType.UI)]
        private string GetUnlockText(ItemClass.Service service)
        {
            throw new NotImplementedException("GetUnlockText is target of redirection and is not implemented.");
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        [RedirectTo(typeof(GameMainToolbar), (ulong)PrerequisiteType.UI)]
        private string GetUnlockText(UnlockManager.Feature feature)
        {
            throw new NotImplementedException("GetUnlockText is target of redirection and is not implemented.");
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        [RedirectTo(typeof(GameMainToolbar), (ulong)PrerequisiteType.UI)]
        private void ShowHidePoliciesPanel(UIComponent comp, bool visible)
        {
            throw new NotImplementedException("ShowHidePoliciesPanel is target of redirection and is not implemented.");
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        [RedirectTo(typeof(GameMainToolbar), (ulong)PrerequisiteType.UI)]
        private void ShowHideEconomyPanel(UIComponent comp, bool visible)
        {
            throw new NotImplementedException("ShowHideEconomyPanel is target of redirection and is not implemented.");
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        [RedirectTo(typeof(MainToolbar), (ulong)PrerequisiteType.UI)]
        private string GetBackgroundSprite(UIButton button, string spriteBase, string name, string state)
        {
            throw new NotImplementedException("GetBackgroundSprite is target of redirection and is not implemented.");
        }
    }
}
