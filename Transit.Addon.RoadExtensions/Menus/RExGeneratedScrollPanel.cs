using System;
using ColossalFramework;
using Transit.Framework.Unsafe;

namespace Transit.Addon.RoadExtensions.Menus
{
    public abstract class RExGeneratedScrollPanel : GeneratedPanel
    {
        [RedirectFrom(typeof (GeneratedScrollPanel))]
        private PoolList<PrefabInfo> CollectAssets(GeneratedScrollPanel.AssetFilter filter,
            Comparison<PrefabInfo> comparison, bool ignoreCategories)
        {
            PoolList<PrefabInfo> poolList = PoolList<PrefabInfo>.Obtain();
            if (filter.IsFlagSet(GeneratedScrollPanel.AssetFilter.Net))
            {
                uint num = 0u;
                while ((ulong) num < (ulong) ((long) PrefabCollection<NetInfo>.LoadedCount()))
                {
                    NetInfo loaded = PrefabCollection<NetInfo>.GetLoaded(num);
                    if (loaded != null &&
                        this.IsServiceValid(loaded) &&
                        this.IsCategoryValid(loaded.category, ignoreCategories) &&
                        this.IsPlacementRelevant(loaded))
                    {
                        poolList.Add(loaded);
                    }
                    num += 1u;
                }
            }
            //if (filter.IsFlagSet(GeneratedScrollPanel.AssetFilter.Building) || filter.IsFlagSet(GeneratedScrollPanel.AssetFilter.Wonder))
            //{
            //    uint num2 = 0u;
            //    while ((ulong)num2 < (ulong)((long)PrefabCollection<BuildingInfo>.LoadedCount()))
            //    {
            //        BuildingInfo loaded2 = PrefabCollection<BuildingInfo>.GetLoaded(num2);
            //        if (loaded2 != null && this.FilterWonders(filter, loaded2) && this.IsServiceValid(loaded2) && this.IsCategoryValid(loaded2.category, ignoreCategories) && this.IsPlacementRelevant(loaded2))
            //        {
            //            poolList.Add(loaded2);
            //        }
            //        num2 += 1u;
            //    }
            //}
            //if (filter.IsFlagSet(GeneratedScrollPanel.AssetFilter.Transport))
            //{
            //    uint num3 = 0u;
            //    while ((ulong)num3 < (ulong)((long)PrefabCollection<TransportInfo>.LoadedCount()))
            //    {
            //        TransportInfo loaded3 = PrefabCollection<TransportInfo>.GetLoaded(num3);
            //        if (loaded3 != null && this.IsServiceValid(loaded3) && this.IsCategoryValid(loaded3.category, ignoreCategories) && this.IsPlacementRelevant(loaded3))
            //        {
            //            poolList.Add(loaded3);
            //        }
            //        num3 += 1u;
            //    }
            //}
            //if (filter.IsFlagSet(GeneratedScrollPanel.AssetFilter.Tree))
            //{
            //    uint num4 = 0u;
            //    while ((ulong)num4 < (ulong)((long)PrefabCollection<TreeInfo>.LoadedCount()))
            //    {
            //        TreeInfo loaded4 = PrefabCollection<TreeInfo>.GetLoaded(num4);
            //        if (loaded4 != null && this.IsServiceValid(loaded4) && this.IsCategoryValid(loaded4.category, ignoreCategories) && this.IsPlacementRelevant(loaded4))
            //        {
            //            poolList.Add(loaded4);
            //        }
            //        num4 += 1u;
            //    }
            //}
            //if (filter.IsFlagSet(GeneratedScrollPanel.AssetFilter.Prop))
            //{
            //    uint num5 = 0u;
            //    while ((ulong)num5 < (ulong)((long)PrefabCollection<PropInfo>.LoadedCount()))
            //    {
            //        PropInfo loaded5 = PrefabCollection<PropInfo>.GetLoaded(num5);
            //        if (loaded5 != null && this.IsServiceValid(loaded5) && this.IsCategoryValid(loaded5.category, ignoreCategories) && this.IsPlacementRelevant(loaded5))
            //        {
            //            poolList.Add(loaded5);
            //        }
            //        num5 += 1u;
            //    }
            //}
            poolList.Sort(comparison);
            return poolList;
        }

        [RedirectTo(typeof(GeneratedScrollPanel))]
        protected virtual bool IsServiceValid(NetInfo info)
        {
            throw new NotImplementedException();
        }

        [RedirectTo(typeof(GeneratedScrollPanel))]
        protected bool IsCategoryValid(string target, bool ignore)
        {
            throw new NotImplementedException();
        }

        [RedirectTo(typeof(GeneratedScrollPanel))]
        protected virtual bool IsPlacementRelevant(NetInfo info)
        {
            throw new NotImplementedException();
        }
    }
}
