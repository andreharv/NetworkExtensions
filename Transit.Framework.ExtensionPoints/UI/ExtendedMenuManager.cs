using System;
using System.Collections.Generic;

namespace Transit.Framework.ExtensionPoints.UI
{
    public class ExtendedMenuManager
    {
        private static readonly IDictionary<string, ICollection<string>> _extendedMenus = new Dictionary<string, ICollection<string>>(StringComparer.InvariantCultureIgnoreCase);

        public static void RegisterNewCategory(string newCategory, GeneratedGroupPanel.GroupFilter group, ItemClass.Service service)
        {
            var id = group + "." + service;

            if (!_extendedMenus.ContainsKey(id))
            {
                _extendedMenus[id] = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);
            }

            if (!_extendedMenus[id].Contains(newCategory))
            {
                _extendedMenus[id].Add(newCategory);
            }
        }

        public static IEnumerable<string> GetNewCategories(GeneratedGroupPanel.GroupFilter group, ItemClass.Service service)
        {
            var id = group + "." + service;

            if (!_extendedMenus.ContainsKey(id))
            {
                return new string[0];
            }

            return _extendedMenus[id];
        }
    }
}
