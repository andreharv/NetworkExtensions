using System;
using System.Collections.Generic;

namespace Transit.Framework.ExtensionPoints.UI
{
    public class RoadCategoryOrderManager
    {
        private static readonly IDictionary<string, int> _categoryOrders = new Dictionary<string, int>(StringComparer.InvariantCultureIgnoreCase);

        public static void RegisterCategory(string category, int order)
        {
            _categoryOrders[category] = order;
        }

        public static int? GetOrder(string category)
        {
            if (!_categoryOrders.ContainsKey(category))
            {
                return null;
            }

            return _categoryOrders[category];
        }
    }
}
