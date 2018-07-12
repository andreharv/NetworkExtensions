using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using ColossalFramework;
using ColossalFramework.Globalization;
using ColossalFramework.UI;
using Transit.Framework.ExtensionPoints.UI;
using Transit.Framework;
using Transit.Framework.Redirection;
using UnityEngine;

namespace Transit.Framework.Hooks.UI
{
    public class TAMGeneratedGroupPanel : GeneratedGroupPanel
    {
        private const string kSubbarButtonTemplate = "SubbarButtonTemplate";
        private const string kSubbarPanelTemplate = "SubbarPanelTemplate";

        [RedirectFrom(typeof(GeneratedGroupPanel))]
#pragma warning disable 108,114
        protected UIButton SpawnButtonEntry(UITabstrip strip, string name, string category, bool isDefaultCategory,
#pragma warning restore 108,114
            string localeID, string unlockText, string spriteBase, bool enabled, bool forceFillContainer)
        {
            int objectIndex = (int)typeof(GeneratedGroupPanel).GetField("m_ObjectIndex", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(this);
            // TAM Edit Start
            Type type = typeof(GeneratedScrollPanel)
                .Assembly
                .GetTypes()
                .FirstOrDefault(t => string.Equals(t.Name, name + "Panel", StringComparison.InvariantCultureIgnoreCase));
            // TAM Edit End
            if (type != null && !type.IsSubclassOf(typeof(GeneratedScrollPanel)))
                type = (System.Type)null;
            UIButton uiButton;
            if (strip.childCount > objectIndex)
            {
                uiButton = strip.components[objectIndex] as UIButton;
            }
            else
            {
                GameObject asGameObject1 = UITemplateManager.GetAsGameObject(kSubbarButtonTemplate);
                GameObject asGameObject2 = UITemplateManager.GetAsGameObject(kSubbarPanelTemplate);
                uiButton = strip.AddTab(category, asGameObject1, asGameObject2, type) as UIButton;
            }
            uiButton.isEnabled = enabled;
            uiButton.gameObject.GetComponent<TutorialUITag>().tutorialTag = category + "Group";
            GeneratedScrollPanel componentInContainer = strip.GetComponentInContainer((UIComponent)uiButton, type) as GeneratedScrollPanel;
            if (componentInContainer != null)
            {
                componentInContainer.component.isInteractive = true;
                componentInContainer.m_OptionsBar = this.m_OptionsBar;
                componentInContainer.m_DefaultInfoTooltipAtlas = this.m_DefaultInfoTooltipAtlas;
                if (forceFillContainer || enabled)
                {
                    componentInContainer.category = !isDefaultCategory ? category : string.Empty;
                    componentInContainer.RefreshPanel();
                }
            }
            // TAM Edit Start
            var customAtlas = AtlasManager.instance.GetAtlas(category);
            if (customAtlas != null)
            {
                uiButton.atlas = customAtlas;
            }
            // TAM Edit End
            string str = spriteBase + category;
            uiButton.normalFgSprite = str;
            uiButton.focusedFgSprite = str + "Focused";
            uiButton.hoveredFgSprite = str + "Hovered";
            uiButton.pressedFgSprite = str + "Pressed";
            uiButton.disabledFgSprite = str + "Disabled";
            if (!string.IsNullOrEmpty(localeID) && !string.IsNullOrEmpty(unlockText))
                uiButton.tooltip = ColossalFramework.Globalization.Locale.Get(localeID, category) + " - " + unlockText;
            else if (!string.IsNullOrEmpty(localeID))
                uiButton.tooltip = ColossalFramework.Globalization.Locale.Get(localeID, category);
            typeof(GeneratedGroupPanel).GetField("m_ObjectIndex", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(this, objectIndex + 1);
            return uiButton;

        }

        //[MethodImpl(MethodImplOptions.NoInlining)]
        //[RedirectFrom(typeof(GeneratedGroupPanel))]
        //private PoolList<GroupInfo> CollectAssets(GroupFilter filter, Comparison<GroupInfo> comparison)
        //{
        //    PoolList<GeneratedGroupPanel.GroupInfo> groupItems = PoolList<GroupInfo>.Obtain();
        //    if (filter.IsFlagSet(GroupFilter.Net))
        //    {
        //        // TAM Edit Start
        //        //groupItems.AddRange(CollectAssetsNetInfoGroups().ToArray());
        //        // TAM Edit End
        //    }
        //    if (filter.IsFlagSet(GeneratedGroupPanel.GroupFilter.Net))
        //    {
        //        for (uint index = 0; (long)index < (long)PrefabCollection<NetInfo>.LoadedCount(); ++index)
        //        {
        //            NetInfo loaded = PrefabCollection<NetInfo>.GetLoaded(index);
        //            if (loaded != null && this.IsServiceValid((PrefabInfo)loaded) && this.IsPlacementRelevant(loaded))
        //                this.AddGroup(groupItems, (PrefabInfo)loaded);
        //        }
        //    }
        //    if (filter.IsFlagSet(GeneratedGroupPanel.GroupFilter.Building) || filter.IsFlagSet(GeneratedGroupPanel.GroupFilter.Wonder))
        //    {
        //        for (uint index = 0; (long)index < (long)PrefabCollection<BuildingInfo>.LoadedCount(); ++index)
        //        {
        //            BuildingInfo loaded = PrefabCollection<BuildingInfo>.GetLoaded(index);
        //            if (loaded != null && this.FilterWonders(filter, loaded) && (this.IsServiceValid((PrefabInfo)loaded) && this.IsPlacementRelevant(loaded)))
        //                this.AddGroup(groupItems, (PrefabInfo)loaded);
        //        }
        //    }
        //    if (filter.IsFlagSet(GeneratedGroupPanel.GroupFilter.Transport))
        //    {
        //        for (uint index = 0; (long)index < (long)PrefabCollection<TransportInfo>.LoadedCount(); ++index)
        //        {
        //            TransportInfo loaded = PrefabCollection<TransportInfo>.GetLoaded(index);
        //            if (loaded != null && this.IsServiceValid((PrefabInfo)loaded) && this.IsPlacementRelevant(loaded))
        //                this.AddGroup(groupItems, (PrefabInfo)loaded);
        //        }
        //    }
        //    if (filter.IsFlagSet(GeneratedGroupPanel.GroupFilter.Tree))
        //    {
        //        for (uint index = 0; (long)index < (long)PrefabCollection<TreeInfo>.LoadedCount(); ++index)
        //        {
        //            TreeInfo loaded = PrefabCollection<TreeInfo>.GetLoaded(index);
        //            if (loaded != null && this.IsServiceValid((PrefabInfo)loaded) && this.IsPlacementRelevant(loaded))
        //                this.AddGroup(groupItems, (PrefabInfo)loaded);
        //        }
        //    }
        //    if (filter.IsFlagSet(GeneratedGroupPanel.GroupFilter.Prop))
        //    {
        //        for (uint index = 0; (long)index < (long)PrefabCollection<PropInfo>.LoadedCount(); ++index)
        //        {
        //            PropInfo loaded = PrefabCollection<PropInfo>.GetLoaded(index);
        //            if (loaded != null && this.IsServiceValid((PrefabInfo)loaded) && this.IsPlacementRelevant(loaded))
        //                this.AddGroup(groupItems, (PrefabInfo)loaded);
        //        }
        //    }
        //    if (filter.IsFlagSet(GeneratedGroupPanel.GroupFilter.Disaster))
        //    {
        //        for (uint index = 0; (long)index < (long)PrefabCollection<DisasterInfo>.LoadedCount(); ++index)
        //        {
        //            DisasterInfo loaded = PrefabCollection<DisasterInfo>.GetLoaded(index);
        //            if (loaded != null && this.IsPlacementRelevant(loaded))
        //                this.AddGroup(groupItems, (PrefabInfo)loaded);
        //        }
        //    }
        //    groupItems.Sort(comparison);
        //    return groupItems;
        //}

        //private IEnumerable<GroupInfo> CollectAssetsNetInfoGroups()
        //{
        //    foreach (var category in CollectAssetsNetInfoCategories().Distinct())
        //    {
        //        yield return new GroupInfo(category, GetCategoryOrder(category));
        //    }
        //}

        //private IEnumerable<string> CollectAssetsNetInfoCategories()
        //{
        //    for (uint num = 0; num < PrefabCollection<NetInfo>.LoadedCount(); num++)
        //    {
        //        var loaded = PrefabCollection<NetInfo>.GetLoaded(num);
        //        if (loaded != null && this.IsServiceValid(loaded) && this.IsPlacementRelevant(loaded))
        //        {
        //            yield return loaded.category;
        //        }
        //    }

        //    foreach (var cat in ExtendedMenuManager.GetNewCategories(GroupFilter.Net, service))
        //    {
        //        yield return cat;
        //    }
        //}

        //[MethodImpl(MethodImplOptions.NoInlining)]
        //[RedirectTo(typeof(GeneratedGroupPanel))]
        //private bool IsPlacementRelevant(NetInfo info)
        //{
        //    throw new NotImplementedException("IsPlacementRelevant is target of redirection and is not implemented.");
        //}

        //[MethodImpl(MethodImplOptions.NoInlining)]
        //[RedirectTo(typeof(GeneratedGroupPanel))]
        //private bool IsPlacementRelevant(BuildingInfo info)
        //{
        //    throw new NotImplementedException("IsPlacementRelevant is target of redirection and is not implemented.");
        //}

        //[MethodImpl(MethodImplOptions.NoInlining)]
        //[RedirectTo(typeof(GeneratedGroupPanel))]
        //private bool IsPlacementRelevant(TransportInfo info)
        //{
        //    throw new NotImplementedException("IsPlacementRelevant is target of redirection and is not implemented.");
        //}

        //[MethodImpl(MethodImplOptions.NoInlining)]
        //[RedirectTo(typeof(GeneratedGroupPanel))]
        //private bool IsPlacementRelevant(TreeInfo info)
        //{
        //    throw new NotImplementedException("IsPlacementRelevant is target of redirection and is not implemented.");
        //}

        //[MethodImpl(MethodImplOptions.NoInlining)]
        //[RedirectTo(typeof(GeneratedGroupPanel))]
        //private bool IsPlacementRelevant(PropInfo info)
        //{
        //    throw new NotImplementedException("IsPlacementRelevant is target of redirection and is not implemented.");
        //}

        //[MethodImpl(MethodImplOptions.NoInlining)]
        //[RedirectTo(typeof(GeneratedGroupPanel))]
        //private bool IsPlacementRelevant(DisasterInfo info)
        //{
        //    throw new NotImplementedException("IsPlacementRelevant is target of redirection and is not implemented.");
        //}

        //[MethodImpl(MethodImplOptions.NoInlining)]
        //[RedirectTo(typeof(GeneratedGroupPanel))]
        //private bool FilterWonders(GroupFilter filter, BuildingInfo info)
        //{
        //    throw new NotImplementedException("FilterWonders is target of redirection and is not implemented.");
        //}

        //[MethodImpl(MethodImplOptions.NoInlining)]
        //[RedirectTo(typeof(GeneratedGroupPanel))]
        //private void AddGroup(PoolList<GroupInfo> groupItems, PrefabInfo info)
        //{
        //    throw new NotImplementedException("AddGroup is target of redirection and is not implemented.");
        //}
    }
}
