using System;
using System.Collections.Generic;
using Transit.Framework.Builders;

namespace Transit.Framework.UI
{
    public partial class MenuManager
    {
        private readonly IDictionary<Type, IMenuCategoryBuilder> _categoryBuilders = new Dictionary<Type, IMenuCategoryBuilder>();
        private readonly IDictionary<string, int> _categoryOrders = new Dictionary<string, int>(StringComparer.InvariantCultureIgnoreCase);
        private readonly IDictionary<string, ICollection<IMenuCategoryBuilder>> _categoryByServices = new Dictionary<string, ICollection<IMenuCategoryBuilder>>(StringComparer.InvariantCultureIgnoreCase);       
        
        public void RegisterCategory(Type menuCategoryBuilderType)
        {
            if (!typeof(IMenuCategoryBuilder).IsAssignableFrom(menuCategoryBuilderType))
            {
                throw new Exception(string.Format("Type {0} is not supported by the MenuManager", menuCategoryBuilderType));
            }

            if (_categoryBuilders.ContainsKey(menuCategoryBuilderType))
            {
                return;
            }

            RegisterCategoryInstance((IMenuCategoryBuilder) Activator.CreateInstance(menuCategoryBuilderType));
        }

        public void RegisterCategoryInstance(IMenuCategoryBuilder menuCategoryBuilder)
        {
            if (_categoryBuilders.ContainsKey(menuCategoryBuilder.GetType()))
            {
                throw new Exception(string.Format("Type {0} is allready registered in the MenuManager", menuCategoryBuilder.GetType()));
            }

            _categoryBuilders[menuCategoryBuilder.GetType()] = menuCategoryBuilder;
            _categoryOrders[menuCategoryBuilder.Name] = menuCategoryBuilder.Order;

            if (menuCategoryBuilder.Group != null && menuCategoryBuilder.Service != null)
            {
                var id = menuCategoryBuilder.Group + "." + menuCategoryBuilder.Service;

                if (!_categoryByServices.ContainsKey(id))
                {
                    _categoryByServices[id] = new HashSet<IMenuCategoryBuilder>();
                }

                _categoryByServices[id].Add(menuCategoryBuilder);
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

        public IEnumerable<IMenuCategoryBuilder> GetAdditionalCategories(GeneratedGroupPanel.GroupFilter group, ItemClass.Service service)
        {
            var id = group + "." + service;

            if (!_categoryByServices.ContainsKey(id))
            {
                return new IMenuCategoryBuilder[0];
            }

            return _categoryByServices[id];
        }

        private bool IsCategoryRequired(IMenuCategoryBuilder menuCategoryBuilder)
        {
            foreach (var tool in _toolBuilders)
            {
                if (tool.UICategory == menuCategoryBuilder.Name)
                {
                    return true;
                }
            }

            return false;
        }

        public IEnumerable<IMenuCategoryBuilder> GetRequiredCategories(IToolbarItemBuilder item)
        {
            foreach (var cat in item.CategoryBuilders)
            {
                if (IsCategoryRequired(cat))
                {
                    yield return cat;
                }
            }
        }
    }
}
