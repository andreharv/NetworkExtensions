using System;
using System.Collections.Generic;
using ColossalFramework;

namespace Transit.Framework.Extenders.UI
{
    public class CategoryOrderProvider : Singleton<CategoryOrderProvider>
    {
        private readonly IDictionary<string, int> _categoryOrders = new Dictionary<string, int>(StringComparer.InvariantCultureIgnoreCase);

        public void RegisterCategory(string category, int order)
        {
            _categoryOrders[category] = order;
        }

        public int? GetOrder(string category)
        {
            if (!_categoryOrders.ContainsKey(category))
            {
                return null;
            }

            return _categoryOrders[category];
        }
    }
}
