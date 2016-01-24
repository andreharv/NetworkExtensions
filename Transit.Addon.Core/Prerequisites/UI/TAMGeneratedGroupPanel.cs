using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using ColossalFramework;
using ColossalFramework.Globalization;
using ColossalFramework.UI;
using Transit.Addon.Core.Extenders.UI;
using Transit.Framework;
using Transit.Framework.Unsafe;
using UnityEngine;

namespace Transit.Addon.Core.Prerequisites.UI
{
    public class TAMGeneratedGroupPanel : GeneratedGroupPanel
    {
        private const string kSubbarButtonTemplate = "SubbarButtonTemplate";
        private const string kSubbarPanelTemplate = "SubbarPanelTemplate";

        [RedirectFrom(typeof (GeneratedGroupPanel))]
        protected UIButton SpawnButtonEntry(UITabstrip strip, string name, string category, bool isDefaultCategory,
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
            {
                type = null;
            }

            UIButton uIButton;
            if (strip.childCount > objectIndex)
            {
                uIButton = (strip.components[objectIndex] as UIButton);
            }
            else
            {
                GameObject asGameObject = UITemplateManager.GetAsGameObject(kSubbarButtonTemplate);
                GameObject asGameObject2 = UITemplateManager.GetAsGameObject(kSubbarPanelTemplate);
                uIButton = (strip.AddTab(category, asGameObject, asGameObject2, type) as UIButton);
            }

            uIButton.isEnabled = enabled;
            uIButton.gameObject.GetComponent<TutorialUITag>().tutorialTag = category;
            GeneratedScrollPanel generatedScrollPanel = strip.GetComponentInContainer(uIButton, type) as GeneratedScrollPanel;
            if (generatedScrollPanel != null)
            {
                generatedScrollPanel.component.isInteractive = true;
                generatedScrollPanel.m_OptionsBar = this.m_OptionsBar;
                generatedScrollPanel.m_DefaultInfoTooltipAtlas = this.m_DefaultInfoTooltipAtlas;
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

            typeof(GeneratedGroupPanel).GetField("m_ObjectIndex", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(this, objectIndex + 1);
            return uIButton;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        [RedirectFrom(typeof(GeneratedGroupPanel))]
        private PoolList<GroupInfo> CollectAssets(GroupFilter filter, Comparison<GroupInfo> comparison)
        {
            PoolList<GroupInfo> poolList = PoolList<GroupInfo>.Obtain();
            if (filter.IsFlagSet(GroupFilter.Net))
            {
                // TAM Edit Start
                poolList.AddRange(CollectAssetsNetInfoGroups().ToArray());
                // TAM Edit End
            }

            if (filter.IsFlagSet(GroupFilter.Building) || 
                filter.IsFlagSet(GroupFilter.Wonder))
            {
                uint num2 = 0u;
                while ((ulong)num2 < (ulong)((long)PrefabCollection<BuildingInfo>.LoadedCount()))
                {
                    BuildingInfo loaded2 = PrefabCollection<BuildingInfo>.GetLoaded(num2);
                    if (loaded2 != null && this.FilterWonders(filter, loaded2) && this.IsServiceValid(loaded2) && this.IsPlacementRelevant(loaded2))
                    {
                        this.AddGroup(poolList, loaded2);
                    }
                    num2 += 1u;
                }
            }

            if (filter.IsFlagSet(GroupFilter.Transport))
            {
                uint num3 = 0u;
                while ((ulong)num3 < (ulong)((long)PrefabCollection<TransportInfo>.LoadedCount()))
                {
                    TransportInfo loaded3 = PrefabCollection<TransportInfo>.GetLoaded(num3);
                    if (loaded3 != null && this.IsServiceValid(loaded3) && this.IsPlacementRelevant(loaded3))
                    {
                        this.AddGroup(poolList, loaded3);
                    }
                    num3 += 1u;
                }
            }

            if (filter.IsFlagSet(GroupFilter.Tree))
            {
                uint num4 = 0u;
                while ((ulong)num4 < (ulong)((long)PrefabCollection<TreeInfo>.LoadedCount()))
                {
                    TreeInfo loaded4 = PrefabCollection<TreeInfo>.GetLoaded(num4);
                    if (loaded4 != null && this.IsServiceValid(loaded4) && this.IsPlacementRelevant(loaded4))
                    {
                        this.AddGroup(poolList, loaded4);
                    }
                    num4 += 1u;
                }
            }

            if (filter.IsFlagSet(GroupFilter.Prop))
            {
                uint num5 = 0u;
                while ((ulong)num5 < (ulong)((long)PrefabCollection<PropInfo>.LoadedCount()))
                {
                    PropInfo loaded5 = PrefabCollection<PropInfo>.GetLoaded(num5);
                    if (loaded5 != null && this.IsServiceValid(loaded5) && this.IsPlacementRelevant(loaded5))
                    {
                        this.AddGroup(poolList, loaded5);
                    }
                    num5 += 1u;
                }
            }

            poolList.Sort(comparison);
            return poolList;
        }

        private IEnumerable<GroupInfo> CollectAssetsNetInfoGroups()
        {
            foreach (var category in CollectAssetsNetInfoCategories().Distinct())
            {
                yield return new GroupInfo(category, GetCategoryOrder(category));
            }
        }

        private IEnumerable<string> CollectAssetsNetInfoCategories()
        {
            for (uint num = 0; num < PrefabCollection<NetInfo>.LoadedCount(); num++)
            {
                var loaded = PrefabCollection<NetInfo>.GetLoaded(num);
                if (loaded != null && this.IsServiceValid(loaded) && this.IsPlacementRelevant(loaded))
                {
                    yield return loaded.category;
                }
            }

            foreach (var cat in ExtendedMenuProvider.instance.GetNewCategories(GroupFilter.Net, service))
            {
                yield return cat;
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        [RedirectTo(typeof(GeneratedGroupPanel))]
        private bool IsPlacementRelevant(NetInfo info)
        {
            throw new NotImplementedException("IsPlacementRelevant is target of redirection and is not implemented.");
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        [RedirectTo(typeof(GeneratedGroupPanel))]
        private bool IsPlacementRelevant(BuildingInfo info)
        {
            throw new NotImplementedException("IsPlacementRelevant is target of redirection and is not implemented.");
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        [RedirectTo(typeof(GeneratedGroupPanel))]
        private bool IsPlacementRelevant(TransportInfo info)
        {
            throw new NotImplementedException("IsPlacementRelevant is target of redirection and is not implemented.");
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        [RedirectTo(typeof(GeneratedGroupPanel))]
        private bool IsPlacementRelevant(TreeInfo info)
        {
            throw new NotImplementedException("IsPlacementRelevant is target of redirection and is not implemented.");
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        [RedirectTo(typeof(GeneratedGroupPanel))]
        private bool IsPlacementRelevant(PropInfo info)
        {
            throw new NotImplementedException("IsPlacementRelevant is target of redirection and is not implemented.");
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        [RedirectTo(typeof(GeneratedGroupPanel))]
        private bool FilterWonders(GroupFilter filter, BuildingInfo info)
        {
            throw new NotImplementedException("FilterWonders is target of redirection and is not implemented.");
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        [RedirectTo(typeof(GeneratedGroupPanel))]
        private void AddGroup(PoolList<GroupInfo> groupItems, PrefabInfo info)
        {
            throw new NotImplementedException("AddGroup is target of redirection and is not implemented.");
        }
    }
}
