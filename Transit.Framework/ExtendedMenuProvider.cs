using System;
using System.Collections.Generic;
using ColossalFramework;

namespace Transit.Framework
{
    public class ExtendedMenuProvider : Singleton<ExtendedMenuProvider>
    {
        private readonly IDictionary<GeneratedGroupPanel.GroupFilter, ICollection<string>> _extendedMenus = new Dictionary<GeneratedGroupPanel.GroupFilter, ICollection<string>>();

        public void RegisterNewCategory(string newCategory, GeneratedGroupPanel.GroupFilter menu)
        {
            if (!_extendedMenus.ContainsKey(menu))
            {
                _extendedMenus[menu] = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);
            }

            if (!_extendedMenus[menu].Contains(newCategory))
            {
                _extendedMenus[menu].Add(newCategory);
            }
        }

        public IEnumerable<string> GetNewCategories(GeneratedGroupPanel.GroupFilter menu)
        {
            if (!_extendedMenus.ContainsKey(menu))
            {
                return new string[0];
            }

            return _extendedMenus[menu];
        }
    }
}
