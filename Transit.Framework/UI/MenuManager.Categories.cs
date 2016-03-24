using System;
using System.Collections.Generic;
using Transit.Framework.UI.Infos;

namespace Transit.Framework.UI
{
    public partial class MenuManager
    {
        private readonly IDictionary<Type, IMenuCategoryInfo> _categories = new Dictionary<Type, IMenuCategoryInfo>();
        private readonly IDictionary<string, int> _categoryOrders = new Dictionary<string, int>(StringComparer.InvariantCultureIgnoreCase);
        private readonly IDictionary<string, ICollection<IMenuCategoryInfo>> _categoryByServices = new Dictionary<string, ICollection<IMenuCategoryInfo>>(StringComparer.InvariantCultureIgnoreCase);

        public void RegisterCategory(Type menuCategoryInfoType)
        {
            if (!typeof(IMenuCategoryInfo).IsAssignableFrom(menuCategoryInfoType))
            {
                throw new Exception(string.Format("Type {0} is not supported by the MenuManager", menuCategoryInfoType));
            }

            if (_categories.ContainsKey(menuCategoryInfoType))
            {
                return;
            }

            RegisterCategoryInstance((IMenuCategoryInfo)Activator.CreateInstance(menuCategoryInfoType));
        }

        private void RegisterCategoryInstance(IMenuCategoryInfo menuCategoryInfo)
        {
            if (_categories.ContainsKey(menuCategoryInfo.GetType()))
            {
                throw new Exception(string.Format("Type {0} is allready registered in the MenuManager", menuCategoryInfo.GetType()));
            }

            Log.Info("TFW: Registering menu category of type " + menuCategoryInfo.GetType());
            _categories[menuCategoryInfo.GetType()] = menuCategoryInfo;
            _categoryOrders[menuCategoryInfo.Name] = menuCategoryInfo.Order;

            if (menuCategoryInfo.Group != null && menuCategoryInfo.Service != null)
            {
                var id = menuCategoryInfo.Group + "." + menuCategoryInfo.Service;

                if (!_categoryByServices.ContainsKey(id))
                {
                    _categoryByServices[id] = new HashSet<IMenuCategoryInfo>();
                }

                _categoryByServices[id].Add(menuCategoryInfo);
            }
        }

        public int? GetCategoryOrder(string category)
        {
            if (!_categoryOrders.ContainsKey(category))
            {
                return null;
            }

            return _categoryOrders[category];
        }

        public IEnumerable<IMenuCategoryInfo> GetAdditionalCategories(GeneratedGroupPanel.GroupFilter group, ItemClass.Service service)
        {
            var id = group + "." + service;

            if (!_categoryByServices.ContainsKey(id))
            {
                return new IMenuCategoryInfo[0];
            }

            return _categoryByServices[id];
        }

        private bool IsCategoryRequired(IMenuCategoryInfo menuCategoryInfo)
        {
            foreach (var tool in _toolBuilders)
            {
                if (tool.UICategory == menuCategoryInfo.Name)
                {
                    return true;
                }
            }

            return false;
        }

        public IEnumerable<IMenuCategoryInfo> GetRequiredCategories(IMenuToolbarItemInfo item)
        {
            foreach (var cat in item.Categories)
            {
                if (IsCategoryRequired(cat))
                {
                    yield return cat;
                }
            }
        }
    }
}
