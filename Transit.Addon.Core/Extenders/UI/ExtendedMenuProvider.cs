using System;
using System.Collections.Generic;
using ColossalFramework;

namespace Transit.Addon.Core.Extenders.UI
{
    public class ExtendedMenuProvider : Singleton<ExtendedMenuProvider>
    {
        private readonly IDictionary<string, ICollection<string>> _extendedMenus = new Dictionary<string, ICollection<string>>(StringComparer.InvariantCultureIgnoreCase);

        public void RegisterNewCategory(string newCategory, GeneratedGroupPanel.GroupFilter group, ItemClass.Service service)
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

        public IEnumerable<string> GetNewCategories(GeneratedGroupPanel.GroupFilter group, ItemClass.Service service)
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
